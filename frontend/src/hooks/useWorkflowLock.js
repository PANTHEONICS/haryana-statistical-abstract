import { useState, useEffect, useCallback } from 'react';
import workflowApi from '@/services/workflowApi';
import { useAuth } from '@/contexts/AuthContext';

/**
 * Reusable hook for workflow lock functionality with role-based access control
 * Can be used across all 350+ screens that need workflow-based locking
 * 
 * Only the assigned user (based on workflow status) can edit:
 * - Status 1 (Maker Entry): Maker can edit
 * - Status 2 (Pending Checker): Checker can review/approve/reject, Maker is locked
 * - Status 4 (Pending DESA Head): DESA Head can approve/reject, Maker and Checker are locked
 * - Status 6 (Approved): Everyone is locked
 * 
 * @param {string} screenCode - The screen code (e.g., 'CENSUS_POPULATION')
 * @returns {object} - { isLocked, canEdit, statusId, assignedRole, lockedMessage, checkAndPreventAction, refreshStatus }
 */
export function useWorkflowLock(screenCode) {
  const { user } = useAuth();
  const [statusId, setStatusId] = useState(1); // Default to Maker Entry (1)
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Workflow status constants
  const STATUS_DRAFT = 1;
  const STATUS_PENDING_CHECKER = 2;
  const STATUS_REJECTED_BY_CHECKER = 3;
  const STATUS_PENDING_HEAD = 4;
  const STATUS_REJECTED_BY_HEAD = 5;
  const STATUS_APPROVED = 6;

  // Get user role
  const userRole = user?.roles?.[0] || user?.RoleName || user?.roleName || '';
  const isMaker = userRole === 'Department Maker';
  const isChecker = userRole === 'Department Checker';
  const isHead = userRole === 'DESA Head';
  const isAdmin = userRole === 'System Admin';

  // Determine which role is assigned to edit based on status
  const getAssignedRoleForStatus = useCallback((status) => {
    switch (status) {
      case STATUS_DRAFT:
      case STATUS_REJECTED_BY_CHECKER: // After Happy Path refactoring, this loops back to Maker Entry (1)
        return 'Department Maker';
      case STATUS_PENDING_CHECKER:
      case STATUS_REJECTED_BY_HEAD: // After Happy Path refactoring, this loops back to Pending Checker (2)
        return 'Department Checker';
      case STATUS_PENDING_HEAD:
        return 'DESA Head';
      case STATUS_APPROVED:
        return null; // No one can edit when approved
      default:
        return null;
    }
  }, []);

  // Get assigned role for current status
  const assignedRole = getAssignedRoleForStatus(statusId);

  // Check if current user can edit based on status and role
  const canEdit = useCallback(() => {
    if (!user || !userRole) return false;
    
    // Admin always has access
    if (isAdmin) return true;
    
    // If no role is assigned (e.g., Approved status), no one can edit
    if (!assignedRole) return false;
    
    // Check if current user's role matches the assigned role
    return userRole === assignedRole;
  }, [user, userRole, assignedRole, isAdmin]);

  // Locked states: Screen is locked if current user is not the assigned user
  // Locked states: 2 (Pending Checker - only Checker can edit), 4 (Pending DESA Head - only Head can edit), 6 (Approved - no one can edit)
  // Unlocked states: 1 (Maker Entry - only Maker can edit)
  const canEditValue = canEdit();
  const isLocked = !canEditValue;

  // Load workflow status for the screen
  const loadStatus = useCallback(async () => {
    if (!screenCode) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const currentStatusId = await workflowApi.getScreenCurrentStatus(screenCode);
      if (currentStatusId) {
        setStatusId(currentStatusId);
      } else {
        // Default to Maker Entry if no status found
        setStatusId(STATUS_DRAFT);
      }
    } catch (err) {
      console.error(`Failed to load workflow status for ${screenCode}:`, err);
      setError(err.message);
      // Default to Maker Entry on error
      setStatusId(STATUS_DRAFT);
    } finally {
      setLoading(false);
    }
  }, [screenCode]);

  // Load status on mount and when screenCode changes
  useEffect(() => {
    loadStatus();
  }, [loadStatus]);

  // Listen for workflow status changes
  useEffect(() => {
    const handleStatusChange = async (event) => {
      const { screenCode: changedScreenCode, newStatusId, screenLevel } = event.detail;
      if (changedScreenCode === screenCode && screenLevel && newStatusId) {
        setStatusId(newStatusId);
      }
    };

    window.addEventListener('workflowStatusChanged', handleStatusChange);
    return () => window.removeEventListener('workflowStatusChanged', handleStatusChange);
  }, [screenCode]);

  // Get locked message based on status and user role
  const getLockedMessage = useCallback(() => {
    if (statusId === STATUS_APPROVED) {
      return "Screen is locked. Record has been approved and cannot be modified.";
    }
    
    if (assignedRole && userRole !== assignedRole) {
      return `Screen is locked. This record is assigned to ${assignedRole} and cannot be modified by ${userRole}.`;
    }
    
    if (!assignedRole) {
      return "Screen is locked. No user is assigned to edit this record.";
    }
    
    return "Screen is locked.";
  }, [statusId, assignedRole, userRole]);

  // Check and prevent action if locked (returns true if action should be prevented)
  const checkAndPreventAction = useCallback((actionName = 'This action') => {
    if (isLocked) {
      const message = getLockedMessage();
      alert(`${actionName} is not allowed. ${message}`);
      return true; // Action should be prevented
    }
    return false; // Action is allowed
  }, [isLocked, getLockedMessage]);

  // Refresh status manually (useful after workflow actions)
  const refreshStatus = useCallback(async () => {
    await loadStatus();
  }, [loadStatus]);

  return {
    // State
    isLocked,
    canEdit: canEditValue,
    statusId,
    assignedRole,
    loading,
    error,
    
    // Messages
    lockedMessage: isLocked ? getLockedMessage() : null,
    
    // Helper functions
    checkAndPreventAction,
    refreshStatus,
    getLockedMessage,
    
    // Status constants (for reference)
    STATUS_DRAFT,
    STATUS_PENDING_CHECKER,
    STATUS_REJECTED_BY_CHECKER,
    STATUS_PENDING_HEAD,
    STATUS_REJECTED_BY_HEAD,
    STATUS_APPROVED,
    
    // User role info
    userRole,
    isMaker,
    isChecker,
    isHead,
    isAdmin,
  };
}

export default useWorkflowLock;
