import { Badge } from "./badge"
import { cn } from "@/lib/utils"

const statusConfig = {
  success: { variant: "success", label: "Success" },
  warning: { variant: "warning", label: "Warning" },
  error: { variant: "destructive", label: "Error" },
  info: { variant: "info", label: "Info" },
  pending: { variant: "secondary", label: "Pending" },
  active: { variant: "default", label: "Active" },
  inactive: { variant: "outline", label: "Inactive" },
}

export function StatusBadge({ status, label, className }) {
  const config = statusConfig[status] || statusConfig.pending
  return (
    <Badge
      variant={config.variant}
      className={cn(className)}
    >
      {label || config.label}
    </Badge>
  )
}
