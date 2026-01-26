# Workflow Role-Based Access Control

## Overview

The workflow system now implements **role-based access control** where only the assigned user (based on workflow status) can edit the screen. Once a screen is approved or rejected and assigned to another user, the previous user loses edit permissions.

---

## Access Control Rules

### Status-Based Role Assignment

| Status ID | Status Name | Assigned Role | Who Can Edit |
|-----------|-------------|---------------|--------------|
| 1 | Draft | Department Maker | Only Maker |
| 2 | Pending Checker | Department Checker | Only Checker |
| 3 | Rejected by Checker | Department Maker | Only Maker (loops back to Draft) |
| 4 | Pending DESA Head | DESA Head | Only DESA Head |
| 5 | Rejected by DESA Head | Department Checker | Only Checker (loops back to Pending Checker) |
| 6 | Approved | None | No one (locked) |

### Special Cases

- **System Admin**: Always has edit access regardless of status
- **Approved Status (6)**: No one can edit, including Admin (final state)

---

## Implementation

### Updated Hook: `useWorkflowLock`

**File:** `frontend/src/hooks/useWorkflowLock.js`

**New Features:**
- `canEdit`: Boolean indicating if current user can edit
- `assignedRole`: The role assigned to edit based on current status
- `isLocked`: True if current user cannot edit (inverse of `canEdit`)
- Role-aware lock messages

**Usage:**
```javascript
const { 
  isLocked, 
  canEdit, 
  assignedRole, 
  lockedMessage, 
  checkAndPreventAction 
} = useWorkflowLock('CENSUS_POPULATION');

// Disable buttons based on canEdit
<Button disabled={!canEdit} onClick={handleEdit}>
  Edit
</Button>
```

---

## Workflow Transitions & Permission Changes

### Maker Submits to Checker
- **Before:** Status 1 (Draft) - Maker can edit
- **After:** Status 2 (Pending Checker) - Checker can edit, Maker is locked
- **Permission Change:** Maker loses edit access, Checker gains edit access

### Checker Approves
- **Before:** Status 2 (Pending Checker) - Checker can edit
- **After:** Status 4 (Pending DESA Head) - DESA Head can edit, Checker is locked
- **Permission Change:** Checker loses edit access, DESA Head gains edit access

### Checker Rejects (Happy Path)
- **Before:** Status 2 (Pending Checker) - Checker can edit
- **After:** Status 1 (Draft) - Maker can edit, Checker is locked
- **Permission Change:** Checker loses edit access, Maker gains edit access

### DESA Head Approves
- **Before:** Status 4 (Pending DESA Head) - DESA Head can edit
- **After:** Status 6 (Approved) - No one can edit
- **Permission Change:** DESA Head loses edit access, screen is permanently locked

### DESA Head Rejects (Happy Path)
- **Before:** Status 4 (Pending DESA Head) - DESA Head can edit
- **After:** Status 2 (Pending Checker) - Checker can edit, DESA Head is locked
- **Permission Change:** DESA Head loses edit access, Checker gains edit access

---

## User Experience

### Lock Messages

When a user tries to edit but doesn't have permission, they see:

- **Status 2 (Pending Checker) - Maker tries to edit:**
  > "Screen is locked. This record is assigned to Department Checker and cannot be modified by Department Maker."

- **Status 4 (Pending DESA Head) - Maker/Checker tries to edit:**
  > "Screen is locked. This record is assigned to DESA Head and cannot be modified by [User Role]."

- **Status 6 (Approved) - Anyone tries to edit:**
  > "Screen is locked. Record has been approved and cannot be modified."

### UI Behavior

1. **Add Record Button**: Disabled when `!canEdit`
2. **Edit Action**: Disabled in row actions when `!canEdit`
3. **Delete Action**: Disabled in row actions when `!canEdit`
4. **Tooltips**: Show lock message when hovering over disabled buttons

---

## Example: Census Population Screen

```javascript
// In Census.jsx
const { 
  isLocked, 
  canEdit, 
  assignedRole, 
  lockedMessage 
} = useWorkflowLock('CENSUS_POPULATION');

// Primary action (Add Record)
<PageHeader
  primaryAction={{
    label: "Add Record",
    onClick: handleOpenCreate,
    disabled: !canEdit, // Only enabled if user can edit
    tooltip: !canEdit ? lockedMessage : undefined,
  }}
/>

// Row actions (Edit/Delete)
const handleRowAction = (row) => {
  const actions = [];
  
  if (canEdit) { // Only show if user can edit
    actions.push({ 
      label: "Edit", 
      onClick: () => handleOpenEdit(row) 
    });
    actions.push({ 
      label: "Delete", 
      onClick: () => handleDelete(row),
      destructive: true 
    });
  } else {
    actions.push({ 
      label: lockedMessage || "Screen is locked", 
      onClick: () => {},
      disabled: true 
    });
  }
  
  return actions;
};
```

---

## Benefits

1. **Clear Ownership**: Each status has a clear assigned role
2. **Security**: Users can only edit when assigned to them
3. **Automatic Permission Transfer**: Permissions change automatically when status changes
4. **User Feedback**: Clear messages explain why actions are disabled
5. **Consistent Behavior**: Same logic across all 350+ screens

---

## Testing Checklist

- [ ] Maker can edit when status is Draft (1)
- [ ] Maker cannot edit when status is Pending Checker (2)
- [ ] Checker can edit when status is Pending Checker (2)
- [ ] Checker cannot edit when status is Pending DESA Head (4)
- [ ] DESA Head can edit when status is Pending DESA Head (4)
- [ ] No one can edit when status is Approved (6)
- [ ] After Checker rejects, Maker regains edit access (status 1)
- [ ] After DESA Head rejects, Checker regains edit access (status 2)
- [ ] Admin can always edit (except when Approved)
- [ ] Lock messages are clear and informative

---

**Last Updated:** 2024
**Status:** ✅ Implemented
