import { Routes, Route } from "react-router-dom"
import Dashboard from "@/pages/Dashboard"
import DataManagement from "@/pages/DataManagement"
import DetailView from "@/pages/DetailView"
import Workflow from "@/pages/Workflow"
import BoardView from "@/pages/BoardView"
import Analytics from "@/pages/Analytics"
import Census from "@/pages/AreaAndPopulation/Table3_2_CensusPopulation"
import Table6_1_Institutions from "@/pages/Education/Table6_1_Institutions"
import Table7_1_SanctionedStrengthPolice from "@/pages/SocialSecurityAndSocialDefence/Table7_1_SanctionedStrengthPolice"
import Login from "@/pages/Login"
import Register from "@/pages/Register"
import UserManagementLogin from "@/pages/UserManagementLogin"
import UserManagement from "@/pages/UserManagement"
import MenuConfiguration from "@/pages/MenuConfiguration"
import ProtectedRoute from "@/components/ProtectedRoute"

export function Router() {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/login" element={<Login />} />
      <Route path="/um-login" element={<UserManagementLogin />} />
      <Route path="/register" element={<Register />} />
      
      {/* Protected routes */}
      <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
      <Route path="/data" element={<ProtectedRoute><DataManagement /></ProtectedRoute>} />
      <Route path="/detail" element={<ProtectedRoute><DetailView /></ProtectedRoute>} />
      <Route path="/workflow" element={<ProtectedRoute><Workflow /></ProtectedRoute>} />
      <Route path="/board" element={<ProtectedRoute><BoardView /></ProtectedRoute>} />
      <Route path="/analytics" element={<ProtectedRoute><Analytics /></ProtectedRoute>} />
      <Route path="/census" element={<ProtectedRoute><Census /></ProtectedRoute>} />
      <Route path="/education/table6-1" element={<ProtectedRoute><Table6_1_Institutions /></ProtectedRoute>} />
      <Route path="/social-security/table7-1" element={<ProtectedRoute><Table7_1_SanctionedStrengthPolice /></ProtectedRoute>} />
      <Route path="/users" element={<ProtectedRoute requireAdmin><UserManagement /></ProtectedRoute>} />
      <Route path="/menu-config" element={<ProtectedRoute requireAdmin><MenuConfiguration /></ProtectedRoute>} />
    </Routes>
  )
}
