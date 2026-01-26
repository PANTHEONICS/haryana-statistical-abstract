# Workflow Lock Usage Guide

## Overview

The `useWorkflowLock` hook provides a **generalized, reusable solution** for implementing workflow-based locking across all 350+ screens in the application. This hook automatically handles:

- ✅ Loading workflow status for a screen
- ✅ Determining if the screen is locked
- ✅ Providing locked messages
- ✅ Preventing actions when locked
- ✅ Auto-updating when workflow status changes

---

## Quick Start

### 1. Import the Hook

```jsx
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
```

### 2. Use in Your Component

```jsx
export default function YourScreen() {
  // Replace 'YOUR_SCREEN_CODE' with your actual screen code
  const { isLocked, statusId, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('YOUR_SCREEN_CODE')
  
  // Your component logic...
}
```

### 3. Apply to Actions

```jsx
// Disable "Add" button
<Button 
  onClick={handleAdd}
  disabled={isLocked}
  title={isLocked ? lockedMessage : undefined}
>
  Add Record
</Button>

// Prevent actions in handlers
const handleEdit = (record) => {
  if (checkAndPreventAction('Editing this record')) {
    return // Action prevented
  }
  // Proceed with edit...
}

// In row actions
const handleRowAction = (row) => {
  const actions = []
  
  if (!isLocked) {
    actions.push({
      label: "Edit",
      onClick: () => handleEdit(row),
    })
    actions.push({
      label: "Delete",
      onClick: () => handleDelete(row),
      destructive: true,
    })
  }
  
  if (isLocked && actions.length === 0) {
    actions.push({
      label: lockedMessage || "Screen is locked",
      onClick: () => {},
      disabled: true,
    })
  }
  
  return actions
}
```

---

## Complete Example

Here's a complete example for a typical data management screen:

```jsx
import { useState, useEffect } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { DataTable } from "@/components/ui/DataTable"
import { Button } from "@/components/ui/button"
import { Plus, Download } from "lucide-react"
import WorkflowStatusBar from "@/components/workflow/WorkflowStatusBar"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import yourScreenApi from "@/services/yourScreenApi"

export default function YourScreen() {
  const [data, setData] = useState([])
  const [loading, setLoading] = useState(true)
  const [dialogOpen, setDialogOpen] = useState(false)
  
  // ✅ STEP 1: Use the workflow lock hook
  const { 
    isLocked, 
    statusId, 
    lockedMessage, 
    checkAndPreventAction, 
    refreshStatus 
  } = useWorkflowLock('YOUR_SCREEN_CODE') // Replace with your screen code
  
  // Load data
  useEffect(() => {
    loadData()
  }, [])
  
  const loadData = async () => {
    try {
      setLoading(true)
      const records = await yourScreenApi.getAll()
      setData(records)
    } catch (error) {
      console.error('Failed to load data:', error)
    } finally {
      setLoading(false)
    }
  }
  
  // ✅ STEP 2: Apply lock check to actions
  const handleAdd = () => {
    if (checkAndPreventAction('Adding a new record')) {
      return
    }
    // Proceed with add...
    setDialogOpen(true)
  }
  
  const handleEdit = (record) => {
    if (checkAndPreventAction('Editing this record')) {
      return
    }
    // Proceed with edit...
    setDialogOpen(true)
  }
  
  const handleDelete = (record) => {
    if (checkAndPreventAction('Deleting this record')) {
      return
    }
    if (window.confirm(`Are you sure you want to delete this record?`)) {
      // Proceed with delete...
    }
  }
  
  // ✅ STEP 3: Filter row actions based on lock status
  const handleRowAction = (row) => {
    const actions = []
    
    if (!isLocked) {
      actions.push({
        label: "Edit",
        onClick: () => handleEdit(row),
      })
      actions.push({
        label: "Delete",
        onClick: () => handleDelete(row),
        destructive: true,
      })
    }
    
    if (isLocked && actions.length === 0) {
      actions.push({
        label: lockedMessage || "Screen is locked",
        onClick: () => {},
        disabled: true,
      })
    }
    
    return actions
  }
  
  // ✅ STEP 4: Add Workflow Status Bar (if data exists)
  // ✅ STEP 5: Disable primary action button
  
  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Your Screen Title"
        breadcrumbs={["Home", "Your Screen"]}
        description="Manage your data"
        primaryAction={{
          label: "Add Record",
          icon: Plus,
          onClick: handleAdd,
          disabled: isLocked, // ✅ Disable when locked
          tooltip: isLocked ? lockedMessage : undefined, // ✅ Show tooltip
        }}
        secondaryActions={[
          {
            label: "Export CSV",
            icon: Download,
            variant: "outline",
            onClick: handleExport,
          },
        ]}
      />
      
      {/* Workflow Status Bar - Only show if data exists */}
      {data.length > 0 && !loading && (
        <div className="mb-4">
          <WorkflowStatusBar
            screenCode="YOUR_SCREEN_CODE"
            currentStatusId={statusId}
            onStatusChange={async (newStatusId, remarks = null) => {
              await refreshStatus() // ✅ Refresh lock status
              await loadData() // Reload data if needed
            }}
          />
        </div>
      )}
      
      {/* Data Table */}
      <DataTable
        columns={columns}
        data={data}
        searchable={true}
        selectable={true}
        onRowAction={handleRowAction} // ✅ Lock-aware actions
      />
    </div>
  )
}
```

---

## Hook API Reference

### Parameters

- `screenCode` (string, required): The screen code registered in `Screen_Workflow` table
  - Example: `'CENSUS_POPULATION'`, `'POPULATION_DATA'`, etc.

### Returns

#### State Properties

| Property | Type | Description |
|----------|------|-------------|
| `isLocked` | `boolean` | `true` if screen is locked, `false` otherwise |
| `statusId` | `number` | Current workflow status ID (1-6) |
| `loading` | `boolean` | `true` while loading workflow status |
| `error` | `string \| null` | Error message if status loading failed |

#### Message Properties

| Property | Type | Description |
|----------|------|-------------|
| `lockedMessage` | `string \| null` | Human-readable message explaining why screen is locked |

#### Helper Functions

| Function | Parameters | Returns | Description |
|----------|-----------|---------|-------------|
| `checkAndPreventAction` | `actionName?: string` | `boolean` | Returns `true` if action should be prevented. Shows alert if locked. |
| `refreshStatus` | None | `Promise<void>` | Manually refresh workflow status from API |
| `getLockedMessage` | None | `string` | Get locked message for current status |

#### Status Constants (for reference)

| Constant | Value | Description |
|----------|-------|-------------|
| `STATUS_DRAFT` | 1 | Draft state - unlocked |
| `STATUS_PENDING_CHECKER` | 2 | Pending Checker - locked |
| `STATUS_REJECTED_BY_CHECKER` | 3 | Rejected by Checker - unlocked |
| `STATUS_PENDING_HEAD` | 4 | Pending DESA Head - locked |
| `STATUS_REJECTED_BY_HEAD` | 5 | Rejected by DESA Head - locked |
| `STATUS_APPROVED` | 6 | Approved - locked |

#### Status Arrays

| Property | Type | Description |
|----------|------|-------------|
| `LOCKED_STATUSES` | `number[]` | `[2, 4, 5, 6]` - Array of locked status IDs |
| `UNLOCKED_STATUSES` | `number[]` | `[1, 3]` - Array of unlocked status IDs |

---

## Workflow Status Flow

```
Draft (1) → Submit to Checker → Pending Checker (2) [LOCKED]
                                            ↓
                                    Checker Approves
                                            ↓
                                  Pending DESA Head (4) [LOCKED]
                                            ↓
                                    DESA Head Approves
                                            ↓
                                      Approved (6) [LOCKED]

If rejected:
- Checker Rejects (2) → Rejected by Checker (3) [UNLOCKED] → Can resubmit
- DESA Head Rejects (4) → Rejected by DESA Head (5) [LOCKED] → Goes to Checker
```

---

## Lock Logic

- **Locked States (2, 4, 5, 6)**: Add, Edit, Delete operations are **disabled**
- **Unlocked States (1, 3)**: Add, Edit, Delete operations are **enabled**

---

## Integration Checklist

When implementing workflow lock in a new screen:

- [ ] Import `useWorkflowLock` hook
- [ ] Call hook with your `screenCode`
- [ ] Disable "Add" button when `isLocked` is `true`
- [ ] Add tooltip to "Add" button showing `lockedMessage` when locked
- [ ] Use `checkAndPreventAction()` in `handleAdd`, `handleEdit`, `handleDelete`
- [ ] Filter row actions based on `isLocked`
- [ ] Add `WorkflowStatusBar` component (if data exists)
- [ ] Call `refreshStatus()` in workflow status change handler
- [ ] Ensure `screenCode` is registered in `Screen_Workflow` table in database

---

## Database Setup

Before using the hook, ensure your screen is registered in the database:

```sql
-- Add screen workflow entry
INSERT INTO [dbo].[Screen_Workflow] 
  ([ScreenCode], [ScreenName], [TargetTableName], [CurrentStatusID], [CreatedByUserID])
VALUES 
  ('YOUR_SCREEN_CODE', 'Your Screen Name', 'your_table_name', 1, @AdminUserID);
```

---

## Benefits

✅ **Single Source of Truth**: All lock logic in one reusable hook  
✅ **Consistent Behavior**: Same locking behavior across all 350+ screens  
✅ **Auto-Updates**: Automatically updates when workflow status changes  
✅ **Type-Safe**: Full TypeScript support (if using TypeScript)  
✅ **Easy Integration**: Just import and use - no duplicate code  
✅ **Maintainable**: Update lock logic in one place, applies everywhere  

---

## Need Help?

Refer to `Census.jsx` for a complete working example of the workflow lock integration.
