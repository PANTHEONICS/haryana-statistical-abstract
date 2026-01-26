import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from '@/components/ui/table';
import { 
  Clock, 
  User, 
  MessageSquare, 
  AlertCircle, 
  CheckCircle2, 
  XCircle,
  Loader2,
  History,
  RotateCcw,
  AlertTriangle
} from 'lucide-react';
import workflowApi from '@/services/workflowApi';
import { formatDate } from '@/utils/format';
import { cn } from '@/lib/utils';
import { useAuth } from '@/contexts/AuthContext';

/**
 * Screen-Level Audit History Modal Component
 * 
 * Reusable component for displaying workflow audit history for any screen
 * Shows rejection reasons prominently at the top if screen is rejected
 * 
 * @param {boolean} open - Whether modal is open
 * @param {function} onOpenChange - Callback when modal open state changes
 * @param {string} screenCode - Screen code (e.g., 'CENSUS_POPULATION')
 * @param {string} screenName - Optional: Screen name for display
 * @param {number} currentStatusId - Optional: Current workflow status ID
 * 
 * @example
 * <ScreenAuditHistoryModal
 *   open={showHistory}
 *   onOpenChange={setShowHistory}
 *   screenCode="CENSUS_POPULATION"
 *   screenName="Census Population Management"
 *   currentStatusId={3}
 * />
 */
const ScreenAuditHistoryModal = ({ 
  open, 
  onOpenChange, 
  screenCode, 
  screenName = null,
  currentStatusId = null,
  onWorkflowReset = null // Callback when workflow is reset
}) => {
  const { user } = useAuth();
  const [history, setHistory] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [currentRejection, setCurrentRejection] = useState(null);
  const [showResetConfirm, setShowResetConfirm] = useState(false);
  const [resetting, setResetting] = useState(false);

  // Check if user is admin
  const isAdmin = user?.roles?.includes('System Admin') || 
                  user?.RoleName === 'System Admin' ||
                  user?.roleName === 'System Admin';

  useEffect(() => {
    if (open && screenCode) {
      loadHistory();
    } else {
      // Reset state when modal closes
      setHistory([]);
      setCurrentRejection(null);
      setError('');
    }
  }, [open, screenCode]);

  const loadHistory = async () => {
    if (!screenCode) {
      setError('Screen code is required');
      return;
    }

    try {
      setLoading(true);
      setError('');
      const data = await workflowApi.getScreenAuditHistory(screenCode);
      
      // Sort by date (newest first)
      const sortedData = data.sort((a, b) => 
        new Date(b.actionDate) - new Date(a.actionDate)
      );
      
      setHistory(sortedData);
      
      // Find current rejection reason (if status is 3 or 5)
      if (currentStatusId === 3 || currentStatusId === 5) {
        // Find the most recent rejection with remarks
        const rejection = sortedData.find(item => 
          (item.toStatusID === 3 || item.toStatusID === 5) && 
          item.remarks && 
          item.remarks.trim() !== ''
        );
        setCurrentRejection(rejection || null);
      } else {
        setCurrentRejection(null);
      }
    } catch (err) {
      setError(err.message || 'Failed to load audit history');
      console.error('Error loading audit history:', err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (statusId, statusName) => {
    const statusConfig = {
      1: { label: statusName || 'Maker Entry', variant: 'secondary', color: 'bg-gray-100 text-gray-800' },
      2: { label: statusName || 'Pending Checker', variant: 'default', color: 'bg-yellow-100 text-yellow-800' },
      3: { label: statusName || 'Rejected by Checker', variant: 'destructive', color: 'bg-red-100 text-red-800' },
      4: { label: statusName || 'Pending DESA Head', variant: 'default', color: 'bg-blue-100 text-blue-800' },
      5: { label: statusName || 'Rejected by DESA Head', variant: 'destructive', color: 'bg-red-100 text-red-800' },
      6: { label: statusName || 'Approved', variant: 'default', color: 'bg-green-100 text-green-800' },
    };

    const config = statusConfig[statusId] || statusConfig[1];
    
    return (
      <Badge className={cn('font-medium', config.color)}>
        {config.label}
      </Badge>
    );
  };

  const getActionIcon = (toStatusId) => {
    if (toStatusId === 3 || toStatusId === 5) {
      return <XCircle className="h-4 w-4 text-red-600" />;
    } else if (toStatusId === 6) {
      return <CheckCircle2 className="h-4 w-4 text-green-600" />;
    } else {
      return <Clock className="h-4 w-4 text-blue-600" />;
    }
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return '-';
    try {
      // Parse the date string (already in IST from backend)
      const date = new Date(dateString);
      
      // Format date directly (no conversion needed as it's already in IST)
      const dateStr = date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
      
      const timeStr = date.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      });
      
      return `${dateStr} ${timeStr}`;
    } catch {
      return dateString;
    }
  };

  const handleResetWorkflow = async () => {
    if (!screenCode) {
      setError('Screen code is required');
      return;
    }

    try {
      setResetting(true);
      setError('');
      
      await workflowApi.resetScreenWorkflow(screenCode);
      
      // Reload history (should be empty now)
      await loadHistory();
      
      // Close confirmation dialog
      setShowResetConfirm(false);
      
      // Notify parent component if callback provided
      if (onWorkflowReset) {
        onWorkflowReset();
      }
      
      // Show success message briefly
      alert('Workflow reset successfully! All audit history has been cleared and status reset to Maker Entry.');
    } catch (err) {
      setError(err.message || 'Failed to reset workflow');
      console.error('Error resetting workflow:', err);
    } finally {
      setResetting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-5xl max-h-[90vh] flex flex-col">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            Workflow Audit History
            {screenName && (
              <span className="text-sm font-normal text-muted-foreground">
                - {screenName}
              </span>
            )}
          </DialogTitle>
        </DialogHeader>

        <div className="flex-1 flex flex-col min-h-0 overflow-hidden">
          {/* Current Rejection Alert - Highlighted at Top */}
          {currentRejection && (
            <Alert variant="destructive" className="mb-4 border-2 border-red-500 flex-shrink-0">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>
                <div className="space-y-2">
                  <div className="font-semibold text-base">
                    Current Rejection Reason:
                  </div>
                  <div className="text-sm bg-red-50 p-3 rounded border border-red-200">
                    <div className="font-medium mb-1">
                      Rejected by: {currentRejection.actionByUserName}
                    </div>
                    <div className="text-gray-700">
                      {currentRejection.remarks}
                    </div>
                    <div className="text-xs text-gray-500 mt-2">
                      {formatDateTime(currentRejection.actionDate)}
                    </div>
                  </div>
                </div>
              </AlertDescription>
            </Alert>
          )}

          {/* Loading State */}
          {loading ? (
            <div className="flex items-center justify-center py-12 flex-shrink-0">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2 text-muted-foreground">Loading audit history...</span>
            </div>
          ) : error ? (
            <Alert variant="destructive" className="flex-shrink-0">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          ) : history.length === 0 ? (
            <div className="text-center py-12 text-muted-foreground flex-shrink-0">
              <History className="h-12 w-12 mx-auto mb-4 opacity-50" />
              <p>No audit history available for this screen.</p>
            </div>
          ) : (
            /* Audit History Table */
            <div className="flex-1 border rounded-md overflow-hidden flex flex-col min-h-0" style={{ height: '500px', maxHeight: 'calc(90vh - 300px)' }}>
              <style dangerouslySetInnerHTML={{__html: `
                .audit-history-scroll::-webkit-scrollbar {
                  width: 8px;
                  height: 8px;
                }
                .audit-history-scroll::-webkit-scrollbar-track {
                  background: #f1f5f9;
                  border-radius: 4px;
                }
                .audit-history-scroll::-webkit-scrollbar-thumb {
                  background: #cbd5e1;
                  border-radius: 4px;
                }
                .audit-history-scroll::-webkit-scrollbar-thumb:hover {
                  background: #94a3b8;
                }
              `}} />
              <div 
                className="audit-history-scroll overflow-y-auto overflow-x-auto w-full h-full" 
                style={{ 
                  scrollbarWidth: 'thin', 
                  scrollbarColor: '#cbd5e1 #f1f5f9'
                }}
              >
                <Table>
                  <TableHeader className="sticky top-0 bg-background z-10 border-b shadow-sm">
                    <TableRow>
                      <TableHead className="w-[120px]">Date & Time</TableHead>
                      <TableHead className="w-[150px]">Action</TableHead>
                      <TableHead className="w-[120px]">From Status</TableHead>
                      <TableHead className="w-[120px]">To Status</TableHead>
                      <TableHead className="w-[150px]">Action By</TableHead>
                      <TableHead>Remarks</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {history.map((item, index) => (
                      <TableRow 
                        key={item.auditID || index}
                        className={cn(
                          // Highlight rejection rows
                          (item.toStatusID === 3 || item.toStatusID === 5) && 
                          'bg-red-50 hover:bg-red-100'
                        )}
                      >
                        {/* Date & Time */}
                        <TableCell className="font-mono text-xs">
                          <div className="flex items-center gap-1">
                            <Clock className="h-3 w-3 text-muted-foreground" />
                            {formatDateTime(item.actionDate)}
                          </div>
                        </TableCell>

                        {/* Action */}
                        <TableCell>
                          <div className="flex items-center gap-2">
                            {getActionIcon(item.toStatusID)}
                            <span className="font-medium">{item.actionName}</span>
                          </div>
                        </TableCell>

                        {/* From Status */}
                        <TableCell>
                          {item.fromStatusName ? (
                            getStatusBadge(item.fromStatusID, item.fromStatusName)
                          ) : (
                            <span className="text-muted-foreground">-</span>
                          )}
                        </TableCell>

                        {/* To Status */}
                        <TableCell>
                          {item.toStatusName ? (
                            getStatusBadge(item.toStatusID, item.toStatusName)
                          ) : (
                            <span className="text-muted-foreground">-</span>
                          )}
                        </TableCell>

                        {/* Action By */}
                        <TableCell>
                          <div className="flex items-center gap-2">
                            <User className="h-4 w-4 text-muted-foreground" />
                            <span className="text-sm">{item.actionByUserName || 'System'}</span>
                          </div>
                        </TableCell>

                        {/* Remarks */}
                        <TableCell>
                          {item.remarks ? (
                            <div className="flex items-start gap-2 max-w-md">
                              <MessageSquare className="h-4 w-4 text-muted-foreground mt-0.5 flex-shrink-0" />
                              <span className="text-sm text-muted-foreground line-clamp-2">
                                {item.remarks}
                              </span>
                            </div>
                          ) : (
                            <span className="text-muted-foreground text-sm">-</span>
                          )}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="flex justify-between items-center pt-4 border-t">
          {/* Admin Reset Button */}
          {isAdmin && (
            <Button 
              variant="destructive" 
              onClick={() => setShowResetConfirm(true)}
              disabled={resetting}
              className="flex items-center gap-2"
            >
              <RotateCcw className="h-4 w-4" />
              {resetting ? 'Resetting...' : 'Reset Workflow to Maker Entry'}
            </Button>
          )}
          
          <div className="flex gap-2 ml-auto">
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Close
            </Button>
          </div>
        </div>
      </DialogContent>

      {/* Reset Confirmation Dialog */}
      <Dialog open={showResetConfirm} onOpenChange={setShowResetConfirm}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2 text-red-600">
              <AlertTriangle className="h-5 w-5" />
              Reset Workflow - Warning
            </DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>
                <div className="space-y-2">
                  <p className="font-semibold">This action cannot be undone!</p>
                  <ul className="list-disc list-inside space-y-1 text-sm">
                    <li>All audit history will be permanently deleted</li>
                    <li>Workflow status will be reset to Maker Entry (Status 1)</li>
                    <li>This will make it appear as a fresh workflow</li>
                  </ul>
                  <p className="text-sm font-medium mt-2 text-yellow-700">
                    ⚠️ This feature is only for testing purposes.
                  </p>
                </div>
              </AlertDescription>
            </Alert>
            
            <div className="bg-yellow-50 border border-yellow-200 rounded-md p-3">
              <p className="text-sm text-yellow-800">
                <strong>Screen:</strong> {screenName || screenCode}
              </p>
              <p className="text-sm text-yellow-800 mt-1">
                <strong>Current Status:</strong> {currentStatusId === 1 ? 'Maker Entry' : 
                 currentStatusId === 2 ? 'Pending Checker' :
                 currentStatusId === 3 ? 'Rejected by Checker' :
                 currentStatusId === 4 ? 'Pending DESA Head' :
                 currentStatusId === 5 ? 'Rejected by DESA Head' :
                 currentStatusId === 6 ? 'Approved' : 'Unknown'}
              </p>
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-4 border-t">
            <Button 
              variant="outline" 
              onClick={() => setShowResetConfirm(false)}
              disabled={resetting}
            >
              Cancel
            </Button>
            <Button 
              variant="destructive" 
              onClick={handleResetWorkflow}
              disabled={resetting}
              className="flex items-center gap-2"
            >
              {resetting ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Resetting...
                </>
              ) : (
                <>
                  <RotateCcw className="h-4 w-4" />
                  Confirm Reset
                </>
              )}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </Dialog>
  );
};

export default ScreenAuditHistoryModal;
