import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import WorkflowStatusBadge from './WorkflowStatusBadge';
import AuditHistoryModal from './AuditHistoryModal';
import workflowApi from '@/services/workflowApi';
import { CheckCircle2, XCircle, Send, History, Loader2 } from 'lucide-react';
import { useAuth } from '@/contexts/AuthContext';

/**
 * Workflow Action Panel Component (Main Widget)
 * 
 * Props:
 * @param {string} tableName - Target table name (e.g., 'Tbl_PopulationData')
 * @param {number} recordId - Primary key of the record
 * @param {number} currentStatusId - Current workflow status ID
 * @param {string} statusName - Optional: Current status name (if not provided, will be shown from badge)
 * 
 * Usage:
 * <WorkflowActionPanel 
 *   tableName="Tbl_PopulationData" 
 *   recordId={123} 
 *   currentStatusId={1} 
 * />
 */
const WorkflowActionPanel = ({ tableName, recordId, currentStatusId, statusName }) => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [pendingAction, setPendingAction] = useState(null);
  const [remarks, setRemarks] = useState('');
  const [showHistoryModal, setShowHistoryModal] = useState(false);
  const [currentStatus, setCurrentStatus] = useState(currentStatusId || 1);

  // Get user role - check multiple possible structures
  const userRole = user?.roles?.[0] || user?.RoleName || user?.roleName || user?.role?.roleName || '';
  const isMaker = userRole === 'Department Maker' || userRole === 'Maker';
  const isChecker = userRole === 'Department Checker' || userRole === 'Checker';
  const isHead = userRole === 'DESA Head' || userRole === 'Head';
  const isAdmin = userRole === 'System Admin' || userRole === 'Admin';
  
  // Update currentStatus when currentStatusId prop changes
  useEffect(() => {
    if (currentStatusId !== undefined) {
      setCurrentStatus(currentStatusId);
    }
  }, [currentStatusId]);

  const handleAction = async (action, requireRemarks = false) => {
    if (requireRemarks) {
      // Open reject modal first
      setPendingAction(action);
      setShowRejectModal(true);
      return;
    }

    await executeAction(action, '');
  };

  const executeAction = async (action, remarksValue) => {
    try {
      setLoading(true);
      setError('');
      setSuccess('');

      const response = await workflowApi.executeAction(
        tableName,
        recordId,
        action,
        remarksValue || null
      );

      setSuccess(response.message || 'Action completed successfully');
      setCurrentStatus(response.newStatusID);
      setShowRejectModal(false);
      setRemarks('');
      setPendingAction(null);

      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(''), 3000);

      // Refresh the page or emit event to parent component
      if (window.dispatchEvent) {
        window.dispatchEvent(new CustomEvent('workflowStatusChanged', {
          detail: { tableName, recordId, newStatusId: response.newStatusID }
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

  const handleRejectConfirm = () => {
    if (!remarks.trim()) {
      setError('Remarks are mandatory for rejection');
      return;
    }

    executeAction(pendingAction, remarks.trim());
  };

  // Determine which buttons to show based on status and role
  // Use currentStatus (state) which updates after workflow actions
  const status = currentStatus || currentStatusId;
  const canSubmitToChecker = (status === 1 || status === 3) && isMaker;
  const canCheckerApprove = status === 2 && isChecker;
  const canCheckerReject = status === 2 && isChecker;
  const canHeadApprove = status === 4 && isHead;
  const canHeadReject = status === 4 && isHead;
  const isLocked = status === 6 || status === 4 || status === 2;

  const getActionButtons = () => {
    const buttons = [];

    // Maker can submit to checker (from Draft or Rejected by Checker)
    if (canSubmitToChecker) {
      buttons.push(
        <Button
          key="submit"
          onClick={() => handleAction('SubmitToChecker')}
          disabled={loading}
          className="bg-blue-600 hover:bg-blue-700"
        >
          <Send className="mr-2 h-4 w-4" />
          Submit to Checker
        </Button>
      );
    }

    // Checker can approve or reject (from Pending Checker)
    if (canCheckerApprove) {
      buttons.push(
        <Button
          key="checker-approve"
          onClick={() => handleAction('CheckerApprove')}
          disabled={loading}
          className="bg-green-600 hover:bg-green-700"
        >
          <CheckCircle2 className="mr-2 h-4 w-4" />
          Approve
        </Button>
      );
    }

    if (canCheckerReject) {
      buttons.push(
        <Button
          key="checker-reject"
          onClick={() => handleAction('CheckerReject', true)}
          disabled={loading}
          variant="destructive"
        >
          <XCircle className="mr-2 h-4 w-4" />
          Reject
        </Button>
      );
    }

    // DESA Head can approve or reject (from Pending Head)
    if (canHeadApprove) {
      buttons.push(
        <Button
          key="head-approve"
          onClick={() => handleAction('HeadApprove')}
          disabled={loading}
          className="bg-green-600 hover:bg-green-700"
        >
          <CheckCircle2 className="mr-2 h-4 w-4" />
          Approve
        </Button>
      );
    }

    if (canHeadReject) {
      buttons.push(
        <Button
          key="head-reject"
          onClick={() => handleAction('HeadReject', true)}
          disabled={loading}
          variant="destructive"
        >
          <XCircle className="mr-2 h-4 w-4" />
          Reject
        </Button>
      );
    }

    return buttons;
  };

  return (
    <div className="space-y-4">
      {/* Status Badge */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-gray-700">Status:</span>
          <WorkflowStatusBadge statusId={currentStatus || currentStatusId} statusName={statusName} />
        </div>
        
        {/* View History Button */}
        <Button
          variant="outline"
          size="sm"
          onClick={() => setShowHistoryModal(true)}
          className="gap-2"
        >
          <History className="h-4 w-4" />
          View History
        </Button>
      </div>

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

      {/* Action Buttons - Always show panel, even if no buttons */}
      {getActionButtons().length > 0 && (
        <div className="flex items-center gap-2">
          {getActionButtons()}
        </div>
      )}

      {/* Locked State Message */}
      {isLocked && getActionButtons().length === 0 && (
        <div className="text-sm text-gray-500 italic">
          {(currentStatus || currentStatusId) === 6 
            ? 'This record is approved and locked for all users.'
            : (currentStatus || currentStatusId) === 4
            ? 'This record is pending DESA Head approval and is locked for maker and checker.'
            : 'This record is pending checker approval and is locked for maker.'}
        </div>
      )}

      {/* Debug Info - Remove in production */}
      {process.env.NODE_ENV === 'development' && (
        <div className="text-xs text-gray-400 mt-2 p-2 bg-gray-50 rounded">
          <div>Status: {currentStatus || currentStatusId}</div>
          <div>User Role: {userRole || 'Not found'}</div>
          <div>isMaker: {isMaker ? 'Yes' : 'No'}, isChecker: {isChecker ? 'Yes' : 'No'}, isHead: {isHead ? 'Yes' : 'No'}</div>
        </div>
      )}

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

      {/* Audit History Modal */}
      <AuditHistoryModal
        open={showHistoryModal}
        onOpenChange={setShowHistoryModal}
        tableName={tableName}
        recordId={recordId}
      />
    </div>
  );
};

export default WorkflowActionPanel;
