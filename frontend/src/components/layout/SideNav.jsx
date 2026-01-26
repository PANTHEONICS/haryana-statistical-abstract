import { NavLink } from "react-router-dom"
import {
  LayoutDashboard,
  Database,
  FileText,
  Workflow,
  Kanban,
  BarChart3,
  Users,
  UserCog,
  ChevronLeft,
  ChevronRight,
} from "lucide-react"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/contexts/AuthContext"

const navItems = [
  { path: "/", label: "Dashboard", icon: LayoutDashboard },
  { path: "/data", label: "Data Management", icon: Database },
  { path: "/census", label: "Census", icon: Users },
  { path: "/detail", label: "Detail View", icon: FileText },
  { path: "/workflow", label: "Workflow", icon: Workflow },
  { path: "/board", label: "Board View", icon: Kanban },
  { path: "/analytics", label: "Analytics", icon: BarChart3 },
]

export default function SideNav({ collapsed = false, onToggle }) {
  const { user } = useAuth()
  const isAdmin = user?.roles?.includes('Admin')

  return (
    <aside
      className={cn(
        "fixed left-0 top-16 z-30 h-[calc(100vh-4rem)] border-r bg-background transition-all duration-300",
        collapsed ? "w-16" : "w-64"
      )}
    >
      <div className="flex h-full flex-col">
        <nav className="flex-1 space-y-1 p-4">
          {navItems.map((item) => {
            const Icon = item.icon
            return (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
                  cn(
                    "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                    "hover:bg-accent hover:text-accent-foreground",
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
                    isActive
                      ? "bg-accent text-accent-foreground"
                      : "text-muted-foreground"
                  )
                }
              >
                <Icon className="h-5 w-5 shrink-0" />
                {!collapsed && <span>{item.label}</span>}
              </NavLink>
            )
          })}
          
          {isAdmin && (
            <NavLink
              to="/users"
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                  "hover:bg-accent hover:text-accent-foreground",
                  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring",
                  isActive
                    ? "bg-accent text-accent-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              <UserCog className="h-5 w-5 shrink-0" />
              {!collapsed && <span>User Management</span>}
            </NavLink>
          )}
        </nav>

        <div className="border-t p-4">
          <Button
            variant="ghost"
            size="icon"
            className="w-full"
            onClick={onToggle}
          >
            {collapsed ? (
              <ChevronRight className="h-5 w-5" />
            ) : (
              <ChevronLeft className="h-5 w-5" />
            )}
          </Button>
        </div>
      </div>
    </aside>
  )
}
