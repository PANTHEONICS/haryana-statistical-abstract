import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import workflowApi from '@/services/workflowApi';
import { CheckCircle2, XCircle, Send, Loader2, ChevronRight } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';
import { cn } from '@/lib/utils';

/**
 * Workflow Status Bar Component - Shows all statuses in a visual timeline
 * 
 * Props:
 * @param {string} screenCode - Screen code (e.g., 'CENSUS_POPULATION')
 * @param {number} currentStatusId - Current workflow status ID
 * @param {function} onStatusChange - Callback when status changes
 */
const WorkflowStatusBar = ({ screenCode, currentStatusId, onStatusChange }) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [pendingAction, setPendingAction] = useState(null);
  const [remarks, setRemarks] = useState('');
  const [statuses, setStatuses] = useState([]);
  const [currentStatus, setCurrentStatus] = useState(currentStatusId || 1);
  const [loadingStatuses, setLoadingStatuses] = useState(true);

  // Get user role
  const userRole = user?.roles?.[0] || user?.RoleName || user?.roleName || '';
  const isMaker = userRole === 'Department Maker';
  const isChecker = userRole === 'Department Checker';
  const isHead = userRole === 'DESA Head';
  const isAdmin = userRole === 'System Admin';

  // Load all workflow statuses
  useEffect(() => {
    const loadStatuses = async () => {
      try {
        setLoadingStatuses(true);
        const allStatuses = await workflowApi.getAllStatuses();
        setStatuses(allStatuses.sort((a, b) => (a.displayOrder || a.DisplayOrder) - (b.displayOrder || b.DisplayOrder) || (a.statusID || a.StatusID) - (b.statusID || b.StatusID)));
      } catch (err) {
        console.error('Failed to load workflow statuses:', err);
      } finally {
        setLoadingStatuses(false);
      }
    };
    loadStatuses();
  }, []);

  // Update current status when prop changes
  useEffect(() => {
    if (currentStatusId !== undefined) {
      setCurrentStatus(currentStatusId);
    }
  }, [currentStatusId]);

  const handleAction = async (action, requireRemarks = false) => {
    // Show confirmation for SubmitToChecker, CheckerApprove, and HeadApprove
    if (action === 'SubmitToChecker' || action === 'CheckerApprove' || action === 'HeadApprove') {
      setPendingAction(action);
      setShowConfirmModal(true);
      return;
    }
    
    if (requireRemarks) {
      setPendingAction(action);
      setShowRejectModal(true);
      return;
    }
    await executeAction(action, '');
  };

  const handleConfirmAction = () => {
    setShowConfirmModal(false);
    executeAction(pendingAction, '');
  };

  const executeAction = async (action, remarksValue) => {
    try {
      setLoading(true);
      setError('');
      setSuccess('');

      // For screen-level workflow (recordId = 0), we need to handle batch operations
      // This would require backend support for batch operations
      // For now, we'll handle it at the component level by calling onStatusChange
      // The parent component will handle updating all records
      
      // Screen-level workflow - use screen code instead of table/record
      const response = await workflowApi.executeScreenAction(
        screenCode,
        action,
        remarksValue || null
      );

      setSuccess(response.message || 'Action completed successfully');
      const newStatusId = response.newStatusID;
      setCurrentStatus(newStatusId);
      setShowRejectModal(false);
      const remarksToPass = remarksValue || '';
      setRemarks('');
      setPendingAction(null);

      setTimeout(() => setSuccess(''), 3000);

      if (onStatusChange) {
        // Pass remarks for rejections
        onStatusChange(newStatusId, remarksToPass || null);
      }

      // Emit event for screen-level workflow
      if (window.dispatchEvent) {
        window.dispatchEvent(new CustomEvent('workflowStatusChanged', {
          detail: { screenCode, newStatusId, screenLevel: true }
        }));
      }
    } catch (err) {
      setError(err.message || 'Failed to execute action');
      setShowRejectModal(false);
      setRemarks('');
      setPendingAction(null);
    } finally {
      setLoading(false);
    }
  };

  const getNewStatusIdFromAction = (action, currentStatusId) => {
    // Map workflow actions to new status IDs
    switch (action) {
      case 'SubmitToChecker':
        return 2; // Pending Checker
      case 'CheckerApprove':
        return 4; // Pending DESA Head
      case 'CheckerReject':
        return 3; // Rejected by Checker
      case 'HeadApprove':
        return 6; // Approved
      case 'HeadReject':
        return 5; // Rejected by DESA Head
      default:
        return currentStatusId;
    }
  };

  const handleRejectConfirm = () => {
    if (!remarks.trim()) {
      setError('Remarks are mandatory for rejection');
      return;
    }
    // Pass remarks to executeAction which will handle both screen-level and record-level
    executeAction(pendingAction, remarks.trim());
  };

  /**
   * Maps a status ID to its visual stage key
   * @param {number} statusId - The workflow status ID
   * @param {Array} statuses - Array of all workflow statuses from API
   * @returns {string} Visual stage key (Draft, CheckerReview, DESAHeadReview, Approved)
   */
  const getVisualStageKey = (statusId, statuses) => {
    const status = statuses.find(s => (s.statusID || s.StatusID) === statusId);
    return status?.visualStageKey || status?.VisualStageKey || status?.Visual_Stage_Key || 'Draft'; // StatusCode remains 'Draft'
  };

  /**
   * Gets the visual stage order for a status ID
   * @param {number} statusId - The workflow status ID
   * @param {Array} statuses - Array of all workflow statuses from API
   * @returns {number} Visual stage order (1-4)
   */
  const getVisualStageOrder = (statusId, statuses) => {
    const visualStageKey = getVisualStageKey(statusId, statuses);
    const stageOrderMap = {
      'Draft': 1,
      'CheckerReview': 2,
      'DESAHeadReview': 3,
      'Approved': 4
    };
    return stageOrderMap[visualStageKey] || 1;
  };

  /**
   * Checks if a status ID represents a rejection status
   * @param {number} statusId - The workflow status ID
   * @returns {boolean} True if status is a rejection (3 or 5)
   */
  const isRejectionStatus = (statusId) => {
    return statusId === 3 || statusId === 5; // Rejected by Checker or Rejected by DESA Head
  };

  // Get unique visual stages (only show 4 stages: Maker Entry, CheckerReview, DESAHeadReview, Approved)
  const visualStages = [
    { key: 'Draft', label: 'Maker Entry', order: 1 },
    { key: 'CheckerReview', label: 'Checker Review', order: 2 },
    { key: 'DESAHeadReview', label: 'DESA Head Review', order: 3 },
    { key: 'Approved', label: 'Approved', order: 4 }
  ];

  // Get current visual stage
  const currentVisualStage = getVisualStageKey(currentStatus, statuses);
  const currentVisualStageOrder = getVisualStageOrder(currentStatus, statuses);
  // Note: Rejection statuses (3, 5) are no longer used as current statuses after refactoring
  // They loop back immediately to previous states (1, 2). Rejection info is in audit history.
  const isCurrentlyRejected = false; // Will be enhanced later to check audit history if needed

  // Determine status state based on visual stage, not raw status ID
  const getStatusState = (visualStageKey, visualStageOrder) => {
    if (visualStageOrder === currentVisualStageOrder) {
      return isCurrentlyRejected ? 'rejected' : 'current'; // Show error color if rejected
    } else if (visualStageOrder < currentVisualStageOrder) {
      return 'completed'; // Previous stage - highlighted
    } else {
      return 'pending'; // Future stage - greyed out
    }
  };

    // Determine which actions are available
    const getAvailableActions = () => {
      const actions = [];
      const status = currentStatus;

      // Maker can submit to checker (from Maker Entry - status 1, which includes both initial entry and after checker rejection)
      // Note: Status 3 (Rejected by Checker) now loops back to status 1 (Maker Entry), so we only check for status 1
      if (status === 1 && isMaker) {
        actions.push({ name: 'SubmitToChecker', label: 'Submit to Checker', icon: Send, color: 'blue' });
      }

      // Checker can approve or reject (from Pending Checker - status 2)
      // Note: Status 5 (Rejected by DESA Head) now loops back to status 2 (Pending Checker), so checker sees status 2
      if (status === 2 && isChecker) {
        actions.push({ name: 'CheckerApprove', label: 'Approve', icon: CheckCircle2, color: 'green' });
        actions.push({ name: 'CheckerReject', label: 'Reject', icon: XCircle, color: 'red', requireRemarks: true });
      }

      // DESA Head can approve or reject (from Pending Head - status 4)
      if (status === 4 && isHead) {
        actions.push({ name: 'HeadApprove', label: 'Approve', icon: CheckCircle2, color: 'green' });
        actions.push({ name: 'HeadReject', label: 'Reject', icon: XCircle, color: 'red', requireRemarks: true });
      }

      return actions;
    };

  const getStatusColor = (state, isRejected = false) => {
    if (isRejected) {
      return 'bg-red-600 text-white border-red-700'; // Error color for rejections
    }
    switch (state) {
      case 'current':
        return 'bg-blue-600 text-white border-blue-700';
      case 'completed':
        return 'bg-green-600 text-white border-green-700';
      case 'pending':
        return 'bg-gray-300 text-gray-600 border-gray-400';
      case 'rejected':
        return 'bg-red-600 text-white border-red-700';
      default:
        return 'bg-gray-300 text-gray-600 border-gray-400';
    }
  };

  const getStatusBorderColor = (state, isRejected = false) => {
    if (isRejected) {
      return 'border-red-700';
    }
    switch (state) {
      case 'current':
        return 'border-blue-700';
      case 'completed':
        return 'border-green-700';
      case 'pending':
        return 'border-gray-400';
      case 'rejected':
        return 'border-red-700';
      default:
        return 'border-gray-400';
    }
  };

  if (loadingStatuses) {
    return (
      <div className="flex items-center justify-center p-4">
        <Loader2 className="h-5 w-5 animate-spin mr-2" />
        <span className="text-sm text-gray-600">Loading workflow statuses...</span>
      </div>
    );
  }

  const availableActions = getAvailableActions();

  return (
    <div className="space-y-4">
      {/* Error/Success Messages */}
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      {success && (
        <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded">
          {success}
        </div>
      )}

      {/* Workflow Status Timeline Bar */}
      <div className="bg-white border rounded-lg p-4">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-sm font-semibold text-gray-700">Workflow Status</h3>
          {availableActions.length > 0 && (
            <div className="flex items-center gap-2">
              {availableActions.map((action) => {
                const Icon = action.icon;
                const isDestructive = action.color === 'red';
                return (
                  <Button
                    key={action.name}
                    onClick={() => handleAction(action.name, action.requireRemarks)}
                    disabled={loading}
                    variant={isDestructive ? "destructive" : "default"}
                    size="sm"
                    className={cn(
                      !isDestructive && action.color === 'green' && "bg-green-600 hover:bg-green-700",
                      !isDestructive && action.color === 'blue' && "bg-blue-600 hover:bg-blue-700"
                    )}
                  >
                    <Icon className="mr-2 h-4 w-4" />
                    {action.label}
                  </Button>
                );
              })}
            </div>
          )}
        </div>

        {/* Status Timeline - Only show 4 visual stages */}
        <div className="flex items-center gap-2 overflow-x-auto pb-2">
          {visualStages.map((stage, index) => {
            const state = getStatusState(stage.key, stage.order);
            const isRejected = stage.order === currentVisualStageOrder && isCurrentlyRejected;
            const isLast = index === visualStages.length - 1;
            
            return (
              <div key={stage.key} className="flex items-center flex-shrink-0">
                {/* Status Item */}
                <div className={cn(
                  "relative px-4 py-2 rounded-lg border-2 transition-all",
                  getStatusColor(state, isRejected),
                  state === 'current' && !isRejected && "ring-2 ring-blue-400 ring-offset-2",
                  state === 'completed' && "ring-1 ring-green-400",
                  isRejected && "ring-2 ring-red-400 ring-offset-2"
                )}>
                  <div className="text-xs font-medium whitespace-nowrap">
                    {stage.label}
                    {isRejected && (
                      <span className="ml-2 text-xs">⚠️ Rejected</span>
                    )}
                  </div>
                  {state === 'current' && !isRejected && (
                    <div className="absolute -top-1 -right-1 w-3 h-3 bg-yellow-400 rounded-full border-2 border-white"></div>
                  )}
                  {isRejected && (
                    <div className="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full border-2 border-white"></div>
                  )}
                </div>
                
                {/* Connector Arrow */}
                {!isLast && (
                  <ChevronRight className={cn(
                    "h-5 w-5 mx-1 flex-shrink-0",
                    state === 'completed' ? "text-green-600" : 
                    state === 'current' ? (isRejected ? "text-red-600" : "text-blue-600") : 
                    "text-gray-400"
                  )} />
                )}
              </div>
            );
          })}
        </div>

        {/* Locked State Message */}
        {availableActions.length === 0 && (currentStatus === 2 || currentStatus === 4 || currentStatus === 6) && (
          <div className="mt-3 text-xs text-gray-500 italic text-center">
            {currentStatus === 6 
              ? 'This record is approved and locked for all users.'
              : currentStatus === 4
              ? 'This record is pending DESA Head approval and is locked for maker and checker.'
              : 'This record is pending checker approval and is locked for maker.'}
          </div>
        )}
      </div>

      {/* Confirmation Modal for Submit/Approve */}
      <Dialog open={showConfirmModal} onOpenChange={setShowConfirmModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>
              {pendingAction === 'SubmitToChecker' ? 'Submit to Checker' : 
               pendingAction === 'CheckerApprove' ? 'Approve and Forward to DESA Head' : 
               'Approve Record'}
            </DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4">
            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <div className="flex items-start">
                <div className="flex-shrink-0">
                  <svg className="h-5 w-5 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-3">
                  <h3 className="text-sm font-medium text-yellow-800">
                    Important: Data will not be editable once approved
                  </h3>
                  <div className="mt-2 text-sm text-yellow-700">
                    {pendingAction === 'SubmitToChecker' ? (
                      <p>
                        Once you submit this data to the checker, you will not be able to edit it until it is reviewed and returned to you (if rejected).
                      </p>
                    ) : pendingAction === 'CheckerApprove' ? (
                      <p>
                        Once you approve this data, it will be forwarded to DESA Head for final approval. You will not be able to edit it after this action.
                      </p>
                    ) : (
                      <p>
                        Once you approve this data, it will be locked and cannot be edited by anyone. This action is final.
                      </p>
                    )}
                  </div>
                </div>
              </div>
            </div>
            <p className="text-sm text-gray-600">
              Are you sure you want to proceed?
            </p>
          </div>

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setShowConfirmModal(false);
                setPendingAction(null);
              }}
              disabled={loading}
            >
              No, Cancel
            </Button>
            <Button
              onClick={handleConfirmAction}
              disabled={loading}
              className={pendingAction === 'HeadApprove' || pendingAction === 'CheckerApprove' ? 'bg-green-600 hover:bg-green-700' : 'bg-blue-600 hover:bg-blue-700'}
            >
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing...
                </>
              ) : (
                <>
                  <CheckCircle2 className="mr-2 h-4 w-4" />
                  Yes, {pendingAction === 'SubmitToChecker' ? 'Submit' : 'Approve'}
                </>
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Reject Modal */}
      <Dialog open={showRejectModal} onOpenChange={setShowRejectModal}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Reject Record</DialogTitle>
          </DialogHeader>
          
          <div className="space-y-4">
            <div>
              <Label htmlFor="remarks">Remarks * (Required)</Label>
              <Textarea
                id="remarks"
                value={remarks}
                onChange={(e) => setRemarks(e.target.value)}
                placeholder="Please provide a reason for rejection..."
                className="mt-2 min-h-[100px]"
                required
              />
              <p className="text-xs text-gray-500 mt-1">
                Remarks are mandatory for rejection
              </p>
            </div>
          </div>

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => {
                setShowRejectModal(false);
                setRemarks('');
                setPendingAction(null);
              }}
              disabled={loading}
            >
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleRejectConfirm}
              disabled={loading || !remarks.trim()}
            >
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing...
                </>
              ) : (
                <>
                  <XCircle className="mr-2 h-4 w-4" />
                  Confirm Rejection
                </>
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default WorkflowStatusBar;
