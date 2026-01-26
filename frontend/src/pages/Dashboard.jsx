import { useAuth } from "@/contexts/AuthContext"
import AdminDashboard from "./AdminDashboard"
import MakerDashboard from "./MakerDashboard"
import CheckerDashboard from "./CheckerDashboard"
import DesaHeadDashboard from "./DesaHeadDashboard"
import { Loader2 } from "lucide-react"

export default function Dashboard() {
  const { user } = useAuth()

  // Get user role
  const userRole = user?.roles?.[0] || user?.RoleName || user?.roleName || ""

  // Route to appropriate dashboard based on role
  if (!userRole) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-gray-400" />
        <span className="ml-2 text-gray-600">Loading...</span>
      </div>
    )
  }

  if (userRole === "System Admin") {
    return <AdminDashboard />
  }

  if (userRole === "Department Maker") {
    return <MakerDashboard />
  }

  if (userRole === "Department Checker") {
    return <CheckerDashboard />
  }

  if (userRole === "DESA Head") {
    return <DesaHeadDashboard />
  }

  // Fallback to admin dashboard if role is not recognized
  return <AdminDashboard />
}
