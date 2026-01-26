# Generic Workflow Engine - Implementation Guide

## Overview

This is a **generic, reusable workflow engine** that can be integrated with any business table in your application. It provides a Maker → Checker → DESA Head approval workflow that can be applied to all 350+ data entry screens without rewriting logic.

---

## Database Setup

### 1. Run Schema Scripts

```sql
-- 1. Create workflow tables
\backend\database\workflow-engine-schema.sql

-- 2. Seed workflow statuses
\backend\database\workflow-engine-seed-data.sql
```

### 2. Integrate Workflow with Business Tables

Every business table that needs workflow integration must have:

```sql
-- Example: Add workflow columns to an existing table
ALTER TABLE [dbo].[Tbl_PopulationData]
ADD [CurrentStatusID] INT NOT NULL DEFAULT 1,
    [CreatedByUserID] INT NOT NULL;

-- Add foreign key constraints
ALTER TABLE [dbo].[Tbl_PopulationData]
ADD CONSTRAINT [FK_Tbl_PopulationData_Status] 
    FOREIGN KEY ([CurrentStatusID]) REFERENCES [dbo].[Mst_WorkflowStatus]([StatusID]),
CONSTRAINT [FK_Tbl_PopulationData_CreatedBy] 
    FOREIGN KEY ([CreatedByUserID]) REFERENCES [dbo].[Master_User]([UserID]);
```

**Requirements:**
- `CurrentStatusID`: INT NOT NULL DEFAULT 1 (References Mst_WorkflowStatus.StatusID)
- `CreatedByUserID`: INT NOT NULL (References Master_User.UserID)

---

## Workflow Status Flow

| StatusID | StatusCode | StatusName | Description |
|----------|-----------|------------|-------------|
| 1 | Draft | Draft | Initial state. Maker can edit and submit |
| 2 | PendingChecker | Pending Checker | Submitted. Maker locked. Waiting for checker |
| 3 | RejectedByChecker | Rejected by Checker | Checker rejected. Returns to maker with remarks |
| 4 | PendingHead | Pending DESA Head | Checker approved. Maker & Checker locked. Waiting for head |
| 5 | RejectedByHead | Rejected by DESA Head | Head rejected. Returns to checker with remarks |
| 6 | Approved | Approved | Final approved state. Locked for all |

---

## Backend API Endpoints

### POST `/api/workflow/execute`
Execute a workflow action.

**Request Body:**
```json
{
  "tableName": "Tbl_PopulationData",
  "recordId": 123,
  "action": "SubmitToChecker", // SubmitToChecker | CheckerReject | CheckerApprove | HeadReject | HeadApprove
  "remarks": "Optional remarks (mandatory for rejections)"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Action completed successfully",
  "newStatusID": 2,
  "newStatusName": "Pending Checker",
  "auditID": 456
}
```

### GET `/api/workflow/history/{tableName}/{recordId}`
Get audit history for a record.

**Response:**
```json
[
  {
    "auditID": 1,
    "targetTableName": "Tbl_PopulationData",
    "targetRecordID": 123,
    "actionName": "Submit to Checker",
    "fromStatusID": 1,
    "fromStatusName": "Draft",
    "toStatusID": 2,
    "toStatusName": "Pending Checker",
    "remarks": null,
    "actionByUserID": 1,
    "actionByUserName": "John Doe",
    "actionDate": "2026-01-21T10:30:00Z"
  }
]
```

### GET `/api/workflow/status/{tableName}/{recordId}`
Get current workflow status for a record.

**Response:**
```json
{
  "statusId": 2
}
```

---

## Frontend Integration

### 1. Basic Usage

```jsx
import WorkflowActionPanel from '@/components/workflow/WorkflowActionPanel';

// In your component
<WorkflowActionPanel
  tableName="Tbl_PopulationData"
  recordId={record.id}
  currentStatusId={record.currentStatusID}
  statusName={record.statusName} // Optional
/>
```

### 2. With Status Badge Only

```jsx
import WorkflowStatusBadge from '@/components/workflow/WorkflowStatusBadge';

<WorkflowStatusBadge statusId={record.currentStatusID} statusName={record.statusName} />
```

### 3. Listening to Status Changes

```jsx
useEffect(() => {
  const handleStatusChange = (event) => {
    const { tableName, recordId, newStatusId } = event.detail;
    // Refresh your data or update UI
    loadRecord(recordId);
  };

  window.addEventListener('workflowStatusChanged', handleStatusChange);
  return () => window.removeEventListener('workflowStatusChanged', handleStatusChange);
}, []);
```

---

## Workflow Actions by Role

### Maker (Department Maker)
- **Draft (1)**: Can submit to checker
- **Rejected by Checker (3)**: Can submit to checker (after corrections)
- **Other statuses**: Locked (read-only)

### Checker (Department Checker)
- **Pending Checker (2)**: Can approve or reject
  - Approve → Status 4 (Pending DESA Head)
  - Reject → Status 3 (Rejected by Checker) - **Requires remarks**
- **Other statuses**: Locked or no action available

### DESA Head
- **Pending DESA Head (4)**: Can approve or reject
  - Approve → Status 6 (Approved) - Final state
  - Reject → Status 5 (Rejected by DESA Head) - **Requires remarks**
- **Other statuses**: Locked or no action available

### System Admin
- Full access to all workflow actions (implementation can be extended)

---

## Security & Validation

1. **SQL Injection Prevention**: Table names are validated using regex patterns
2. **Status Validation**: Actions are validated against current status
3. **Remarks Validation**: Rejections require mandatory remarks
4. **Role-Based Access**: Frontend checks user roles before showing actions
5. **Audit Trail**: All actions are logged in Workflow_AuditHistory

---

## Example: Integrating with Census Population Table

```sql
-- Step 1: Add workflow columns
ALTER TABLE [dbo].[CensusPopulation]
ADD [CurrentStatusID] INT NOT NULL DEFAULT 1,
    [CreatedByUserID] INT NOT NULL;

-- Step 2: Add constraints
ALTER TABLE [dbo].[CensusPopulation]
ADD CONSTRAINT [FK_CensusPopulation_Status] 
    FOREIGN KEY ([CurrentStatusID]) REFERENCES [dbo].[Mst_WorkflowStatus]([StatusID]),
CONSTRAINT [FK_CensusPopulation_CreatedBy] 
    FOREIGN KEY ([CreatedByUserID]) REFERENCES [dbo].[Master_User]([UserID]);

-- Step 3: Update existing records (set status to Draft)
UPDATE [dbo].[CensusPopulation]
SET [CurrentStatusID] = 1, [CreatedByUserID] = 1 -- Use actual admin user ID
WHERE [CurrentStatusID] IS NULL;
```

```jsx
// Step 4: Add component to your screen
import WorkflowActionPanel from '@/components/workflow/WorkflowActionPanel';

function CensusPopulationForm({ record }) {
  return (
    <div>
      {/* Your form fields */}
      
      {/* Workflow panel */}
      <div className="mt-6 border-t pt-6">
        <WorkflowActionPanel
          tableName="CensusPopulation"
          recordId={record.year}
          currentStatusId={record.currentStatusID}
        />
      </div>
    </div>
  );
}
```

---

## Testing Checklist

- [ ] Run database schema scripts
- [ ] Run seed data script
- [ ] Integrate workflow columns to at least one business table
- [ ] Test Maker submission (Draft → Pending Checker)
- [ ] Test Checker approval (Pending Checker → Pending Head)
- [ ] Test Checker rejection (Pending Checker → Rejected, with remarks)
- [ ] Test DESA Head approval (Pending Head → Approved)
- [ ] Test DESA Head rejection (Pending Head → Rejected, with remarks)
- [ ] Verify audit history is created for all actions
- [ ] Verify status locks work correctly (Maker cannot edit when locked)
- [ ] Test WorkflowActionPanel component on frontend

---

## Notes

- The workflow engine uses **dynamic SQL** to update any table name securely
- All actions are **transactional** and logged in audit history
- The frontend component is **self-contained** - just drop it on any screen
- Status changes trigger a custom event `workflowStatusChanged` for parent components
- The service automatically validates status transitions and user permissions

---

## Support

For issues or questions, refer to:
- Backend: `backend/HaryanaStatAbstract.API/Services/WorkflowService.cs`
- Frontend: `frontend/src/components/workflow/WorkflowActionPanel.jsx`
- API: `backend/HaryanaStatAbstract.API/Controllers/WorkflowController.cs`
