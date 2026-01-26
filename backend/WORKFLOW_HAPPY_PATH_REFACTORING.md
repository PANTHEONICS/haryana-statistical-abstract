# Workflow "Happy Path" Refactoring Guide

## Overview

This document outlines the refactoring steps to convert the linear workflow (with negative statuses) to a "Happy Path" workflow where:
- **Rejected statuses are removed from the visual progress bar**
- **Rejections loop back to previous states** instead of moving forward to a "Rejected" status
- **Visual stages** are used to group statuses for UI display

---

## Current Status Flow

| StatusID | StatusCode | StatusName | Current Behavior |
|----------|-----------|------------|------------------|
| 1 | Draft | Draft | Initial state |
| 2 | PendingChecker | Pending Checker | Submitted to checker |
| 3 | RejectedByChecker | Rejected by Checker | **Shown in UI** - Negative status |
| 4 | PendingHead | Pending DESA Head | Submitted to DESA Head |
| 5 | RejectedByHead | Rejected by DESA Head | **Shown in UI** - Negative status |
| 6 | Approved | Approved | Final approved state |

---

## Target "Happy Path" Flow

### Visual Stages (UI Display)

| Visual Stage | Statuses Mapped | Display Order |
|--------------|----------------|---------------|
| **Draft** | Status 1 (Draft), Status 3 (Rejected by Checker) | 1 |
| **Checker Review** | Status 2 (Pending Checker), Status 5 (Rejected by DESA Head) | 2 |
| **DESA Head Review** | Status 4 (Pending DESA Head) | 3 |
| **Approved** | Status 6 (Approved) | 4 |

### Backend Logic Changes

**When Checker Rejects:**
- **Current:** Status changes from 2 → 3 (Rejected by Checker)
- **New:** Status changes from 2 → 1 (Draft) - **Loops back**
- **Audit:** Still logs rejection with remarks, but `ToStatusID` = 1

**When DESA Head Rejects:**
- **Current:** Status changes from 4 → 5 (Rejected by DESA Head)
- **New:** Status changes from 4 → 2 (Pending Checker) - **Loops back**
- **Audit:** Still logs rejection with remarks, but `ToStatusID` = 2

---

## Implementation Steps

### Step 1: SQL Database Changes ✅

**File:** `backend/database/workflow-happy-path-refactor.sql`

**Changes:**
1. Add `Visual_Stage_Key` column to `Mst_WorkflowStatus` table
2. Map existing statuses to visual stages:
   - Status 1 & 3 → `'Draft'`
   - Status 2 & 5 → `'CheckerReview'`
   - Status 4 → `'DESAHeadReview'`
   - Status 6 → `'Approved'`
3. Create index on `Visual_Stage_Key` for performance

**Execute:**
```sql
-- Run in SQL Server Management Studio
-- File: backend/database/workflow-happy-path-refactor.sql
```

---

### Step 2: Backend C# Model Update

**File:** `backend/HaryanaStatAbstract.API/Models/MstWorkflowStatus.cs`

**Add Property:**
```csharp
[MaxLength(50)]
[Column("Visual_Stage_Key")]
public string? VisualStageKey { get; set; }
```

---

### Step 3: Backend Workflow Service Update

**File:** `backend/HaryanaStatAbstract.API/Services/WorkflowService.cs`

**Changes Required:**

#### 3.1 Update `ProcessActionAsync` Method

**Checker Reject Action:**
```csharp
case "checkerreject":
    if (fromStatusId != STATUS_PENDING_CHECKER && fromStatusId != STATUS_REJECTED_BY_HEAD)
    {
        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
    }
    if (string.IsNullOrWhiteSpace(remarks))
    {
        throw new ArgumentException("Remarks are mandatory for rejection");
    }
    // CHANGE: Loop back to Draft instead of Rejected by Checker
    toStatusId = STATUS_DRAFT; // Changed from STATUS_REJECTED_BY_CHECKER
    actionName = fromStatusId == STATUS_REJECTED_BY_HEAD 
        ? "Rejected by Checker (returned to Draft)" 
        : "Rejected by Checker (returned to Draft)";
    break;
```

**DESA Head Reject Action:**
```csharp
case "headreject":
    if (fromStatusId != STATUS_PENDING_HEAD)
    {
        throw new InvalidOperationException($"Cannot reject. Current status: {fromStatusId}");
    }
    if (string.IsNullOrWhiteSpace(remarks))
    {
        throw new ArgumentException("Remarks are mandatory for rejection");
    }
    // CHANGE: Loop back to Pending Checker instead of Rejected by DESA Head
    toStatusId = STATUS_PENDING_CHECKER; // Changed from STATUS_REJECTED_BY_HEAD
    actionName = "Rejected by DESA Head (returned to Checker)";
    break;
```

#### 3.2 Update `ProcessScreenWorkflowActionAsync` Method

Apply the same changes as above for screen-level workflow.

#### 3.3 Update Submit Logic

**Maker Submit Action:**
```csharp
case "submittochecker":
    // Allow from Draft OR Rejected by Checker (which now maps to Draft)
    if (fromStatusId != STATUS_DRAFT && fromStatusId != STATUS_REJECTED_BY_CHECKER)
    {
        throw new InvalidOperationException($"Cannot submit to checker. Current status: {fromStatusId}");
    }
    toStatusId = STATUS_PENDING_CHECKER;
    actionName = "Submitted to Checker";
    break;
```

**Note:** The submit logic can remain the same since it already handles both Draft (1) and Rejected by Checker (3).

---

### Step 4: Frontend React Component Update

**File:** `frontend/src/components/workflow/WorkflowStatusBar.jsx`

#### 4.1 Add Helper Function to Get Visual Stage

```javascript
/**
 * Maps a status ID to its visual stage key
 * @param {number} statusId - The workflow status ID
 * @param {Array} statuses - Array of all workflow statuses from API
 * @returns {string} Visual stage key (Draft, CheckerReview, DESAHeadReview, Approved)
 */
const getVisualStageKey = (statusId, statuses) => {
  const status = statuses.find(s => (s.statusID || s.StatusID) === statusId);
  return status?.visualStageKey || status?.Visual_Stage_Key || 'Draft';
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
```

#### 4.2 Update Status Display Logic

**Filter Statuses for Visual Display:**
```javascript
// Get unique visual stages (only show 4 stages: Draft, CheckerReview, DESAHeadReview, Approved)
const visualStages = [
  { key: 'Draft', label: 'Draft', order: 1 },
  { key: 'CheckerReview', label: 'Checker Review', order: 2 },
  { key: 'DESAHeadReview', label: 'DESA Head Review', order: 3 },
  { key: 'Approved', label: 'Approved', order: 4 }
];

// Get current visual stage
const currentVisualStage = getVisualStageKey(currentStatus, statuses);
const currentVisualStageOrder = getVisualStageOrder(currentStatus, statuses);
const isCurrentlyRejected = isRejectionStatus(currentStatus);
```

#### 4.3 Update Status State Logic

```javascript
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
```

#### 4.4 Update Status Color Logic

```javascript
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
    default:
      return 'bg-gray-300 text-gray-600 border-gray-400';
  }
};
```

#### 4.5 Update Status Timeline Rendering

```javascript
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
```

---

## Testing Checklist

### Backend Testing

- [ ] Execute SQL migration script successfully
- [ ] Verify `Visual_Stage_Key` column exists and has correct values
- [ ] Test Checker Reject: Status should change from 2 → 1 (not 3)
- [ ] Test DESA Head Reject: Status should change from 4 → 2 (not 5)
- [ ] Verify audit history still logs rejections with remarks
- [ ] Test Maker Submit: Should work from both Draft (1) and Rejected by Checker (3)
- [ ] Test screen-level workflow actions

### Frontend Testing

- [ ] Verify only 4 visual stages appear in progress bar
- [ ] Verify rejected statuses show error/warning color
- [ ] Verify rejected statuses appear at correct visual stage position
- [ ] Verify workflow actions work correctly after refactoring
- [ ] Test with different user roles (Maker, Checker, DESA Head)
- [ ] Verify audit history modal still shows correct rejection reasons

---

## Migration Notes

### Data Preservation

- **Existing audit history is preserved** - All historical records remain unchanged
- **Rejected statuses (3, 5) remain in database** - They are still used for audit tracking
- **Visual mapping is additive** - No data is deleted, only new column added

### Rollback Plan

If rollback is needed:
1. Remove `Visual_Stage_Key` column from `Mst_WorkflowStatus`
2. Revert `WorkflowService.cs` to use original status IDs (3, 5) for rejections
3. Revert `WorkflowStatusBar.jsx` to show all 6 statuses

---

## Summary

This refactoring:
- ✅ **Removes negative statuses from UI** - Only 4 visual stages shown
- ✅ **Loops back on rejections** - Status returns to previous state
- ✅ **Preserves audit trail** - All rejections still logged with remarks
- ✅ **Maintains backward compatibility** - Existing audit history unchanged
- ✅ **Improves UX** - Cleaner, more intuitive workflow visualization

---

## Next Steps

1. **Execute SQL script** (`workflow-happy-path-refactor.sql`)
2. **Update C# model** (`MstWorkflowStatus.cs`)
3. **Update backend service** (`WorkflowService.cs`)
4. **Update frontend component** (`WorkflowStatusBar.jsx`)
5. **Test thoroughly** using the checklist above
6. **Deploy to production** after successful testing

---

**Created:** 2024
**Last Updated:** 2024
