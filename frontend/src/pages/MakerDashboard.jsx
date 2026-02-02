import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import PageHeader from "@/components/layout/PageHeader"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Loader2, FileText, AlertCircle, CheckCircle2, Clock, ArrowRight } from "lucide-react"
import dashboardApi from "@/services/dashboardApi"
import { useAuth } from "@/contexts/AuthContext"
import menuApi from "@/services/menuApi"

const STATUS_COLORS = {
  1: "bg-blue-100 text-blue-800 border-blue-300", // Maker Entry
  2: "bg-yellow-100 text-yellow-800 border-yellow-300", // Pending Checker
  4: "bg-purple-100 text-purple-800 border-purple-300", // Pending DESA Head
  6: "bg-green-100 text-green-800 border-green-300", // Approved
}

const STATUS_ICONS = {
  1: FileText,
  2: Clock,
  4: Clock,
  6: CheckCircle2,
}

export default function MakerDashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const [loading, setLoading] = useState(true)
  const [screensData, setScreensData] = useState(null)
  const [menus, setMenus] = useState([])
  const [error, setError] = useState(null)

  useEffect(() => {
    loadDashboardData()
  }, [])

  const loadDashboardData = async () => {
    try {
      setLoading(true)
      setError(null)
      
      // Load screens waiting for action
      const screensResponse = await dashboardApi.getScreensWaitingForAction()
      setScreensData(screensResponse)

      // Load user menus to get screen routes
      const userMenus = await menuApi.getUserMenus()
      setMenus(userMenus)
    } catch (err) {
      console.error("Failed to load dashboard data:", err)
      setError(err.message || "Failed to load dashboard data")
    } finally {
      setLoading(false)
    }
  }

  const getScreenRoute = (screenCode) => {
    // Try to find menu path for this screen
    const menu = menus.find(m => 
      m.menuPath && (
        m.menuPath.toLowerCase().includes(screenCode.toLowerCase()) ||
        m.menuName.toLowerCase().includes(screenCode.toLowerCase())
      )
    )
    
    if (menu) return menu.menuPath
    
    // Fallback: try common patterns
    if (screenCode.includes("CENSUS") || screenCode.includes("TABLE_3_2")) {
      return "/census"
    }
    if (screenCode.includes("EDUCATION") || screenCode.includes("TABLE_6_1")) {
      return "/education/table6-1"
    }
    if (screenCode.includes("TABLE_7_1")) return "/social-security/table7-1"
    if (screenCode.includes("TABLE_7_6") || (screenCode.includes("PRISONER") && !screenCode.includes("EXPENDITURE"))) return "/social-security/table7-6"
    if (screenCode.includes("TABLE_7_7") || screenCode.includes("EXPENDITURE") || screenCode.includes("MAINTENANCE")) return "/social-security/table7-7"
    if (screenCode.includes("TABLE_7_8") || screenCode.includes("JAIL_INDUSTRY") || screenCode.includes("PRODUCTION_PROGRESS")) return "/social-security/table7-8"
    if (screenCode.includes("SSD")) return "/social-security/table7-1"
    
    return "#"
  }

  const handleScreenClick = (screen) => {
    const route = getScreenRoute(screen.screenCode)
    if (route && route !== "#") {
      navigate(route)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-gray-400" />
        <span className="ml-2 text-gray-600">Loading dashboard...</span>
      </div>
    )
  }

  if (error) {
    return (
      <div className="p-6">
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          <div className="flex items-center">
            <AlertCircle className="h-5 w-5 mr-2" />
            <span>{error}</span>
          </div>
        </div>
      </div>
    )
  }

  const waitingForAction = screensData?.screensByStatus?.filter(
    status => status.waitingForActionCount > 0
  ) || []

  const allScreens = screensData?.screensByStatus || []

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Maker Dashboard"
        breadcrumbs={["Home", "Dashboard"]}
      />

      {/* Summary Cards */}
      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Screens</CardTitle>
            <FileText className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{screensData?.totalScreens || 0}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Waiting for Action</CardTitle>
            <AlertCircle className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">
              {screensData?.totalWaitingForAction || 0}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              Screens requiring your attention
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Approved</CardTitle>
            <CheckCircle2 className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">
              {allScreens.find(s => s.statusId === 6)?.count || 0}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              Completed screens
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Screens Waiting for Action */}
      {waitingForAction.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <AlertCircle className="h-5 w-5 mr-2 text-yellow-600" />
              Screens Waiting for Your Action
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {waitingForAction.map((statusGroup) => (
                <div key={statusGroup.statusId}>
                  <h3 className="text-sm font-semibold text-gray-700 mb-2">
                    {statusGroup.statusName} ({statusGroup.waitingForActionCount} screens)
                  </h3>
                  <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
                    {statusGroup.screens
                      .filter(screen => screen.requiresAction)
                      .map((screen) => {
                        const StatusIcon = STATUS_ICONS[statusGroup.statusId] || FileText
                        return (
                          <Card
                            key={screen.screenWorkflowID}
                            className="cursor-pointer hover:shadow-md transition-shadow"
                            onClick={() => handleScreenClick(screen)}
                          >
                            <CardContent className="p-4">
                              <div className="flex items-start justify-between">
                                <div className="flex-1">
                                  <h4 className="font-semibold text-sm mb-1">
                                    {screen.screenName}
                                  </h4>
                                  <p className="text-xs text-gray-600 mb-2">
                                    {screen.actionType}
                                  </p>
                                  <Badge
                                    variant="outline"
                                    className={STATUS_COLORS[statusGroup.statusId] || ""}
                                  >
                                    {statusGroup.statusName}
                                  </Badge>
                                </div>
                                <ArrowRight className="h-5 w-5 text-gray-400" />
                              </div>
                            </CardContent>
                          </Card>
                        )
                      })}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* All Screens by Status */}
      <Card>
        <CardHeader>
          <CardTitle>All Screens by Status</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-6">
            {allScreens.map((statusGroup) => {
              const StatusIcon = STATUS_ICONS[statusGroup.statusId] || FileText
              return (
                <div key={statusGroup.statusId}>
                  <div className="flex items-center justify-between mb-3">
                    <h3 className="text-sm font-semibold text-gray-700 flex items-center">
                      <StatusIcon className="h-4 w-4 mr-2" />
                      {statusGroup.statusName}
                    </h3>
                    <Badge variant="outline" className={STATUS_COLORS[statusGroup.statusId] || ""}>
                      {statusGroup.count} screens
                    </Badge>
                  </div>
                  <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
                    {statusGroup.screens.map((screen) => (
                      <Card
                        key={screen.screenWorkflowID}
                        className={`cursor-pointer hover:shadow-md transition-shadow ${
                          screen.requiresAction ? "ring-2 ring-yellow-400" : ""
                        }`}
                        onClick={() => handleScreenClick(screen)}
                      >
                        <CardContent className="p-4">
                          <div className="flex items-start justify-between">
                            <div className="flex-1">
                              <h4 className="font-semibold text-sm mb-1">
                                {screen.screenName}
                              </h4>
                              <p className="text-xs text-gray-500 mb-2">
                                Created by: {screen.createdByUserName}
                              </p>
                              {screen.requiresAction && (
                                <Badge variant="outline" className="bg-yellow-100 text-yellow-800 border-yellow-300 text-xs">
                                  Action Required
                                </Badge>
                              )}
                            </div>
                            <ArrowRight className="h-5 w-5 text-gray-400" />
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                </div>
              )
            })}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
