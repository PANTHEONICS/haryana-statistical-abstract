# Workflow Reset Feature - Usage Guide

## Overview

The **Workflow Reset** feature allows System Admins to reset any screen's workflow back to the Draft phase for testing purposes. This feature:

- ✅ **Admin Only**: Only visible to System Admin users
- ✅ **Clears Audit History**: Permanently deletes all workflow audit history
- ✅ **Resets to Draft**: Sets workflow status back to Status 1 (Draft)
- ✅ **Warning Dialog**: Shows clear warning before reset
- ✅ **Generalized**: Works across all 350+ screens

---

## How It Works

### Backend Implementation

1. **API Endpoint**: `POST /api/workflow/screen/reset/{screenCode}`
   - **Authorization**: Requires `System Admin` role
   - **Action**: Deletes all audit history and resets status to Draft

2. **Service Method**: `ResetScreenWorkflowAsync(screenCode, currentUserId)`
   - Validates user is admin
   - Deletes all `Workflow_AuditHistory` records for the screen
   - Resets `Screen_Workflow.CurrentStatusID` to 1 (Draft)
   - Logs the action for audit purposes

### Frontend Implementation

1. **Component**: `ScreenAuditHistoryModal`
   - Shows "Reset Workflow to Draft" button (admin only)
   - Opens confirmation dialog with warning
   - Calls reset API
   - Refreshes history after reset

---

## User Experience

### For Admin Users

1. Open Audit History modal (click "Audit History" button)
2. See "Reset Workflow to Draft" button at bottom left (red/destructive style)
3. Click button → Warning dialog appears
4. Review warning:
   - ⚠️ All audit history will be permanently deleted
   - ⚠️ Workflow status will reset to Draft
   - ⚠️ This is only for testing
5. Click "Confirm Reset" → Workflow resets
6. Modal refreshes showing empty history
7. Screen workflow status is now Draft

### For Non-Admin Users

- Button is **not visible**
- No access to reset functionality

---

## Warning Dialog Content

```
⚠️ Reset Workflow - Warning

This action cannot be undone!
• All audit history will be permanently deleted
• Workflow status will be reset to Draft (Status 1)
• This will make it appear as a fresh workflow

⚠️ This feature is only for testing purposes.

Screen: [Screen Name]
Current Status: [Current Status]
```

---

## Integration

### Already Integrated

✅ **Census.jsx** - Reset button available in Audit History modal

### For Other Screens

The reset functionality is **automatically available** in `ScreenAuditHistoryModal`. Just use the component:

```jsx
<ScreenAuditHistoryModal
  open={showAuditHistory}
  onOpenChange={setShowAuditHistory}
  screenCode="YOUR_SCREEN_CODE"
  screenName="Your Screen Name"
  currentStatusId={statusId}
  onWorkflowReset={async () => {
    // Optional: Refresh workflow status after reset
    await refreshStatus();
    await refreshData();
  }}
/>
```

The reset button will automatically appear for admin users.

---

## Security

### Backend Security

- ✅ **Role Check**: Only `System Admin` role can reset
- ✅ **Authorization**: `[Authorize(Roles = "System Admin")]` on endpoint
- ✅ **User Validation**: Verifies user exists and has admin role
- ✅ **Logging**: All reset actions are logged

### Frontend Security

- ✅ **Role Check**: Button only visible if `user.roles.includes('System Admin')`
- ✅ **Confirmation**: Double confirmation required
- ✅ **Warning**: Clear warning about permanent deletion

---

## API Details

### Endpoint

```
POST /api/workflow/screen/reset/{screenCode}
```

### Authorization

- **Required**: Bearer Token
- **Role**: System Admin

### Request

```json
// No body required - screenCode in URL
```

### Response

```json
{
  "success": true,
  "message": "Workflow reset to Draft phase successfully. All audit history has been cleared.",
  "newStatusID": 1,
  "newStatusName": "Draft",
  "auditID": null
}
```

### Error Responses

- **401 Unauthorized**: User not authenticated
- **403 Forbidden**: User is not System Admin
- **404 Not Found**: Screen workflow not found
- **500 Internal Server Error**: Server error

---

## Database Changes

### What Gets Deleted

All records from `Workflow_AuditHistory` where:
- `ScreenWorkflowID` matches the screen's `ScreenWorkflowID`

### What Gets Updated

- `Screen_Workflow.CurrentStatusID` → Set to `1` (Draft)
- `Screen_Workflow.CurrentStatusName` → Set to `"Draft"` or cleared
- `Screen_Workflow.UpdatedAt` → Set to current UTC time

### What Stays

- ✅ Screen workflow record itself
- ✅ All business data (e.g., census records)
- ✅ User records
- ✅ Other screens' workflow data

---

## Use Cases

### Testing Workflow Flow

1. Admin tests complete workflow: Draft → Pending Checker → Approved
2. Admin resets to Draft
3. Admin tests again with different scenarios
4. Repeat as needed

### Clearing Test Data

1. During development/testing
2. Reset screens to clean state
3. Start fresh workflow testing

### Training

1. Train users on workflow
2. Reset after each training session
3. Start fresh for next training

---

## Important Notes

⚠️ **Permanent Deletion**: Audit history cannot be recovered after reset

⚠️ **Testing Only**: This feature is intended for testing/development only

⚠️ **Admin Only**: Only System Admin can access this feature

⚠️ **No Business Impact**: Resetting workflow does NOT delete business data (e.g., census records)

---

## Example: Census Screen

```jsx
// In Census.jsx
<ScreenAuditHistoryModal
  open={showAuditHistory}
  onOpenChange={setShowAuditHistory}
  screenCode="CENSUS_POPULATION"
  screenName="Census Population Management"
  currentStatusId={screenWorkflowStatus}
  onWorkflowReset={async () => {
    // Refresh workflow status
    await refreshStatus();
    // Reload data
    await refreshData();
  }}
/>
```

**Result**: Admin sees "Reset Workflow to Draft" button in Audit History modal.

---

## Troubleshooting

### Button Not Showing

- ✅ Check user role is "System Admin"
- ✅ Check `isAdmin` logic in component
- ✅ Verify user object structure

### Reset Fails

- ✅ Check backend logs for errors
- ✅ Verify user has System Admin role in database
- ✅ Check screen code is correct
- ✅ Verify screen workflow exists in database

### History Not Clearing

- ✅ Check database for `Workflow_AuditHistory` records
- ✅ Verify `ScreenWorkflowID` matches
- ✅ Check backend logs for deletion count

---

## Summary

✅ **Generalized**: Works for all 350+ screens  
✅ **Secure**: Admin-only access with role validation  
✅ **Safe**: Warning dialog prevents accidental resets  
✅ **Complete**: Clears history and resets status  
✅ **Logged**: All resets are logged for audit  

The reset functionality is **automatically available** in the `ScreenAuditHistoryModal` component - no additional integration needed for other screens!
