import { Search, Bell, Menu, LogOut, Settings, Loader2, Key } from "lucide-react"
import { Input } from "@/components/ui/input"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useAuth } from "@/contexts/AuthContext"
import { useNavigate, useLocation } from "react-router-dom"
import { cn } from "@/lib/utils"
import { useState, useEffect } from "react"
import menuApi from "@/services/menuApi"
import * as LucideIcons from "lucide-react"
import ChangePasswordDialog from "@/components/ChangePasswordDialog"

export default function TopNav() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [userMenus, setUserMenus] = useState([])
  const [loadingMenus, setLoadingMenus] = useState(true)
  const [showChangePasswordDialog, setShowChangePasswordDialog] = useState(false)
  const isAdmin = user?.roles?.includes('Admin') || user?.roles?.includes('System Admin') || user?.RoleName === 'System Admin'

  useEffect(() => {
    loadUserMenus()
  }, [user])

  const loadUserMenus = async () => {
    try {
      setLoadingMenus(true)
      const menus = await menuApi.getUserMenus()
      setUserMenus(menus)
    } catch (error) {
      console.error('Failed to load user menus:', error)
      setUserMenus([])
    } finally {
      setLoadingMenus(false)
    }
  }

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const getInitials = (name) => {
    if (!name) return 'U'
    const parts = name.split(' ')
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase()
    }
    return name.substring(0, 2).toUpperCase()
  }

  // Support both old user format and new UserManagement format
  const displayName = user?.fullName || user?.FullName || user?.username || user?.LoginID || 'User'
  const userRole = user?.roles?.[0] || user?.RoleName || 'User'
  const lastLogin = user?.lastLoginDateTime || user?.LastLoginDateTime || user?.LastLoginAt

  const isActive = (path) => {
    if (path === "/") {
      return location.pathname === "/"
    }
    return location.pathname.startsWith(path)
  }

  const handleNavigation = (path) => {
    navigate(path)
  }

  const getIconComponent = (iconName) => {
    if (!iconName) return Menu
    // Try to get the icon from lucide-react
    const IconComponent = LucideIcons[iconName] || Menu
    return IconComponent
  }

  return (
    <header className="sticky top-0 z-40 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="flex h-16 items-center px-4 lg:px-6">
        {/* Navigation Menu Dropdown */}
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" className="mr-2">
              <Menu className="h-5 w-5" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" className="w-56">
            <DropdownMenuLabel>Navigation</DropdownMenuLabel>
            <DropdownMenuSeparator />
            {loadingMenus ? (
              <DropdownMenuItem disabled>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                <span>Loading menus...</span>
              </DropdownMenuItem>
            ) : (
              <>
                {userMenus
                  .filter(menu => !menu.parentMenuID) // Only show top-level menus
                  .sort((a, b) => a.displayOrder - b.displayOrder)
                  .map((menu) => {
                    const IconComponent = getIconComponent(menu.menuIcon)
                    const active = isActive(menu.menuPath)
                    return (
                      <DropdownMenuItem
                        key={menu.menuID}
                        onClick={() => handleNavigation(menu.menuPath)}
                        className={cn(
                          "cursor-pointer",
                          active && "bg-accent"
                        )}
                      >
                        <IconComponent className="mr-2 h-4 w-4" />
                        <span>{menu.menuName}</span>
                      </DropdownMenuItem>
                    )
                  })}
                
                {isAdmin && (
                  <>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      onClick={() => handleNavigation("/menu-config")}
                      className={cn(
                        "cursor-pointer",
                        isActive("/menu-config") && "bg-accent"
                      )}
                    >
                      <Settings className="mr-2 h-4 w-4" />
                      <span>Menu Configuration</span>
                    </DropdownMenuItem>
                  </>
                )}
              </>
            )}
          </DropdownMenuContent>
        </DropdownMenu>
        
        {/* Logo and Title - Removed as DepartmentHeader now handles this */}

        <div className="flex flex-1 items-center justify-end gap-4">
          <div className="relative hidden md:block max-w-sm w-full">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              type="search"
              placeholder="Search..."
              className="pl-9 w-full"
            />
          </div>

          <Button variant="ghost" size="icon" className="relative">
            <Bell className="h-5 w-5" />
            <span className="absolute top-1 right-1 h-2 w-2 bg-destructive rounded-full"></span>
          </Button>

          <Separator orientation="vertical" className="h-6" />

          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <div className="flex items-center gap-3 cursor-pointer hover:opacity-80 transition-opacity">
                <Avatar>
                  <AvatarFallback>{getInitials(displayName)}</AvatarFallback>
                </Avatar>
                <div className="hidden lg:block text-left">
                  <p className="text-sm font-medium">Welcome {displayName}</p>
                  <p className="text-xs text-muted-foreground">
                    {userRole}
                    {lastLogin && ` • Last Login: ${new Date(lastLogin).toLocaleString()}`}
                  </p>
                </div>
              </div>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>
                <div className="flex flex-col space-y-1">
                  <p className="text-sm font-medium leading-none">{displayName}</p>
                  <p className="text-xs leading-none text-muted-foreground">
                    {user?.email || user?.UserEmailID || user?.LoginID}
                  </p>
                  {lastLogin && (
                    <p className="text-xs leading-none text-muted-foreground mt-1">
                      Last Login: {new Date(lastLogin).toLocaleString()}
                    </p>
                  )}
                </div>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem 
                onClick={() => setShowChangePasswordDialog(true)}
                className="cursor-pointer"
              >
                <Key className="mr-2 h-4 w-4" />
                <span>Change Password</span>
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={handleLogout}>
                <LogOut className="mr-2 h-4 w-4" />
                <span>Log out</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      {/* Change Password Dialog */}
      <ChangePasswordDialog 
        open={showChangePasswordDialog} 
        onOpenChange={setShowChangePasswordDialog} 
      />
    </header>
  )
}
