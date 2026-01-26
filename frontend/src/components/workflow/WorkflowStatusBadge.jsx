import { Badge } from '@/components/ui/badge';

/**
 * Workflow Status Badge Component
 * Displays a colored badge based on workflow status ID
 * 
 * Status IDs:
 * 1 = Draft (Gray)
 * 2 = Pending Checker (Yellow)
 * 3 = Rejected by Checker (Red)
 * 4 = Pending DESA Head (Blue)
 * 5 = Rejected by DESA Head (Red)
 * 6 = Approved (Green)
 */
const WorkflowStatusBadge = ({ statusId, statusName }) => {
  const getStatusConfig = (id) => {
    switch (id) {
      case 1: // Maker Entry (formerly Draft)
        return { variant: 'secondary', label: statusName || 'Maker Entry', className: 'bg-gray-100 text-gray-800' };
      case 2: // Pending Checker
        return { variant: 'default', label: statusName || 'Pending Checker', className: 'bg-yellow-100 text-yellow-800' };
      case 3: // Rejected by Checker
        return { variant: 'destructive', label: statusName || 'Rejected by Checker', className: 'bg-red-100 text-red-800' };
      case 4: // Pending DESA Head
        return { variant: 'default', label: statusName || 'Pending DESA Head', className: 'bg-blue-100 text-blue-800' };
      case 5: // Rejected by DESA Head
        return { variant: 'destructive', label: statusName || 'Rejected by DESA Head', className: 'bg-red-100 text-red-800' };
      case 6: // Approved
        return { variant: 'default', label: statusName || 'Approved', className: 'bg-green-100 text-green-800' };
      default:
        return { variant: 'secondary', label: statusName || 'Unknown', className: 'bg-gray-100 text-gray-800' };
    }
  };

  const config = getStatusConfig(statusId);

  return (
    <Badge className={config.className}>
      {config.label}
    </Badge>
  );
};

export default WorkflowStatusBadge;
