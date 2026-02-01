import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import PageHeader from "@/components/layout/PageHeader"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Loader2, FileText, AlertCircle, CheckCircle2, Clock, ArrowRight, Shield, Building2 } from "lucide-react"
import dashboardApi from "@/services/dashboardApi"
import { useAuth } from "@/contexts/AuthContext"
import menuApi from "@/services/menuApi"

const STATUS_COLORS = {
  1: "bg-blue-100 text-blue-800 border-blue-300",
  2: "bg-yellow-100 text-yellow-800 border-yellow-300",
  4: "bg-purple-100 text-purple-800 border-purple-300",
  6: "bg-green-100 text-green-800 border-green-300",
}

const STATUS_ICONS = {
  1: FileText,
  2: Clock,
  4: Clock,
  6: CheckCircle2,
}

export default function DesaHeadDashboard() {
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
      
      const screensResponse = await dashboardApi.getScreensWaitingForAction()
      setScreensData(screensResponse)

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
    const menu = menus.find(m => 
      m.menuPath && (
        m.menuPath.toLowerCase().includes(screenCode.toLowerCase()) ||
        m.menuName.toLowerCase().includes(screenCode.toLowerCase())
      )
    )
    
    if (menu) return menu.menuPath
    
    if (screenCode.includes("CENSUS") || screenCode.includes("TABLE_3_2")) {
      return "/census"
    }
    if (screenCode.includes("EDUCATION") || screenCode.includes("TABLE_6_1")) {
      return "/education/table6-1"
    }
    if (screenCode.includes("SSD") || screenCode.includes("TABLE_7_1")) {
      return "/social-security/table7-1"
    }
    
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

  // DESA Head gets screensByDepartment instead of screensByStatus
  const screensByDepartment = screensData?.screensByDepartment || []
  
  // Calculate summary statistics across all departments
  const totalWaitingForAction = screensData?.totalWaitingForAction || 0
  const totalScreens = screensData?.totalScreens || 0
  
  // Calculate department-wise statistics
  const departmentStats = screensByDepartment.map(dept => {
    const approved = dept.screensByStatus.find(s => s.statusId === 6)?.count || 0
    const pendingHead = dept.screensByStatus.find(s => s.statusId === 4)?.count || 0
    const pendingChecker = dept.screensByStatus.find(s => s.statusId === 2)?.count || 0
    const makerEntry = dept.screensByStatus.find(s => s.statusId === 1)?.count || 0
    
    return {
      ...dept,
      approved,
      pendingHead,
      pendingChecker,
      makerEntry
    }
  })

  const totalApproved = departmentStats.reduce((sum, dept) => sum + dept.approved, 0)
  const totalInProgress = departmentStats.reduce((sum, dept) => sum + dept.pendingChecker + dept.pendingHead, 0)

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="DESA Head Dashboard"
        breadcrumbs={["Home", "Dashboard"]}
      />

      {/* Summary Cards */}
      <div className="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Screens</CardTitle>
            <FileText className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{totalScreens}</div>
            <p className="text-xs text-muted-foreground mt-1">
              Across all departments
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Pending Final Approval</CardTitle>
            <Shield className="h-4 w-4 text-purple-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-purple-600">
              {totalWaitingForAction}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              Awaiting your final approval
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
              {totalApproved}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              Finalized screens
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">In Progress</CardTitle>
            <Clock className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-yellow-600">
              {totalInProgress}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              Under review
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Department-wise Screens */}
      <div className="space-y-6">
        {screensByDepartment.map((department) => {
          const waitingForAction = department.screensByStatus.filter(
            status => status.waitingForActionCount > 0
          )

          return (
            <Card key={department.departmentID || department.departmentName}>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center">
                    <Building2 className="h-5 w-5 mr-2 text-blue-600" />
                    {department.departmentName}
                  </CardTitle>
                  <div className="flex items-center gap-2">
                    <Badge variant="outline" className="bg-blue-100 text-blue-800 border-blue-300">
                      {department.totalScreens} screens
                    </Badge>
                    {department.totalWaitingForAction > 0 && (
                      <Badge variant="outline" className="bg-purple-100 text-purple-800 border-purple-300">
                        {department.totalWaitingForAction} pending
                      </Badge>
                    )}
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                {/* Department Statistics */}
                <div className="grid grid-cols-4 gap-4 mb-6 pb-4 border-b">
                  <div className="text-center">
                    <div className="text-2xl font-bold text-blue-600">{department.makerEntry || 0}</div>
                    <div className="text-xs text-gray-600">Maker Entry</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-yellow-600">{department.pendingChecker || 0}</div>
                    <div className="text-xs text-gray-600">Pending Checker</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-purple-600">{department.pendingHead || 0}</div>
                    <div className="text-xs text-gray-600">Pending Head</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-green-600">{department.approved || 0}</div>
                    <div className="text-xs text-gray-600">Approved</div>
                  </div>
                </div>

                {/* Screens Waiting for Action */}
                {waitingForAction.length > 0 && (
                  <div className="mb-6">
                    <h3 className="text-sm font-semibold text-purple-700 mb-3 flex items-center">
                      <Shield className="h-4 w-4 mr-2" />
                      Screens Pending Final Approval
                    </h3>
                    <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
                      {waitingForAction.flatMap(statusGroup =>
                        statusGroup.screens
                          .filter(screen => screen.requiresAction)
                          .map((screen) => {
                            const StatusIcon = STATUS_ICONS[statusGroup.statusId] || FileText
                            return (
                              <Card
                                key={screen.screenWorkflowID}
                                className="cursor-pointer hover:shadow-md transition-shadow ring-2 ring-purple-400"
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
                          })
                      )}
                    </div>
                  </div>
                )}

                {/* All Screens by Status */}
                <div className="space-y-4">
                  {department.screensByStatus.map((statusGroup) => {
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
                                screen.requiresAction ? "ring-2 ring-purple-400" : ""
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
                                      <Badge variant="outline" className="bg-purple-100 text-purple-800 border-purple-300 text-xs">
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
          )
        })}
      </div>
    </div>
  )
}
