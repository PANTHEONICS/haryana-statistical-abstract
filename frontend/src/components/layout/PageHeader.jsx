import { ChevronRight, Home } from "lucide-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"

export default function PageHeader({
  title,
  breadcrumbs = [],
  primaryAction,
  secondaryActions = [],
  description,
}) {
  return (
    <div className="border-b bg-background">
      <div className="flex flex-col gap-4 p-6">
        {breadcrumbs.length > 0 && (
          <nav className="flex items-center gap-2 text-sm text-muted-foreground">
            <Home className="h-4 w-4" />
            {breadcrumbs.map((crumb, index) => (
              <div key={index} className="flex items-center gap-2">
                <ChevronRight className="h-4 w-4" />
                {index === breadcrumbs.length - 1 ? (
                  <span className="text-foreground font-medium">{crumb}</span>
                ) : (
                  <span>{crumb}</span>
                )}
              </div>
            ))}
          </nav>
        )}

        <div className="flex items-start justify-between gap-4">
          <div className="flex-1">
            <h1 className="text-3xl font-semibold tracking-tight">{title}</h1>
            {description && (
              <p className="mt-2 text-sm text-muted-foreground">{description}</p>
            )}
          </div>

          <div className="flex items-center gap-2">
            {secondaryActions.map((action, index) => (
              <Button
                key={index}
                variant={action.variant || "outline"}
                size={action.size || "default"}
                onClick={action.onClick}
              >
                {action.icon && <action.icon className="mr-2 h-4 w-4" />}
                {action.label}
              </Button>
            ))}
            {primaryAction && (
              <Button
                variant={primaryAction.variant || "default"}
                size={primaryAction.size || "default"}
                onClick={primaryAction.onClick}
                disabled={primaryAction.disabled}
                title={primaryAction.tooltip}
              >
                {primaryAction.icon && (
                  <primaryAction.icon className="mr-2 h-4 w-4" />
                )}
                {primaryAction.label}
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
