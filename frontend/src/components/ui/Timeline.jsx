import { cn } from "@/lib/utils"
import { CheckCircle2, Circle, Clock } from "lucide-react"

export function Timeline({ items = [], className }) {
  return (
    <div className={cn("space-y-4", className)}>
      {items.map((item, index) => (
        <div key={index} className="relative flex gap-4">
          <div className="flex flex-col items-center">
            <div
              className={cn(
                "flex h-10 w-10 items-center justify-center rounded-full border-2",
                item.status === "completed"
                  ? "border-primary bg-primary text-primary-foreground"
                  : item.status === "active"
                  ? "border-primary bg-background text-primary"
                  : "border-muted bg-background text-muted-foreground"
              )}
            >
              {item.status === "completed" ? (
                <CheckCircle2 className="h-5 w-5" />
              ) : item.status === "active" ? (
                <Clock className="h-5 w-5" />
              ) : (
                <Circle className="h-5 w-5" />
              )}
            </div>
            {index < items.length - 1 && (
              <div
                className={cn(
                  "mt-2 h-full w-0.5",
                  item.status === "completed" ? "bg-primary" : "bg-muted"
                )}
                style={{ minHeight: "2rem" }}
              />
            )}
          </div>
          <div className="flex-1 pb-8">
            <div className="flex items-center justify-between">
              <h4 className="text-sm font-semibold">{item.title}</h4>
              {item.time && (
                <span className="text-xs text-muted-foreground">{item.time}</span>
              )}
            </div>
            {item.description && (
              <p className="mt-1 text-sm text-muted-foreground">
                {item.description}
              </p>
            )}
            {item.meta && (
              <div className="mt-2 flex items-center gap-2 text-xs text-muted-foreground">
                {item.meta}
              </div>
            )}
          </div>
        </div>
      ))}
    </div>
  )
}
