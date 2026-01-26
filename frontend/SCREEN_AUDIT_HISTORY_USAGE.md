# Screen Audit History Component - Usage Guide

## Overview

The `ScreenAuditHistoryModal` is a **reusable, generalized component** for displaying workflow audit history across all 350+ screens. It shows:

- ✅ Complete audit trail in a tabular format
- ✅ **Highlighted rejection reason** at the top (if screen is rejected)
- ✅ User-friendly table with all workflow actions
- ✅ Status badges and icons for quick understanding
- ✅ Works with any screen code

---

## Quick Start

### 1. Import the Component

```jsx
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import { History } from "lucide-react"
```

### 2. Add State for Modal

```jsx
const [showAuditHistory, setShowAuditHistory] = useState(false)
```

### 3. Add Button to Trigger Modal

```jsx
// In PageHeader secondaryActions or anywhere in your component
<Button 
  variant="outline" 
  onClick={() => setShowAuditHistory(true)}
>
  <History className="mr-2 h-4 w-4" />
  Audit History
</Button>
```

### 4. Add the Modal Component

```jsx
<ScreenAuditHistoryModal
  open={showAuditHistory}
  onOpenChange={setShowAuditHistory}
  screenCode="YOUR_SCREEN_CODE"
  screenName="Your Screen Name" // Optional
  currentStatusId={currentStatusId} // Optional, for highlighting rejections
/>
```

---

## Complete Example

```jsx
import { useState } from "react"
import { History } from "lucide-react"
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"

export default function YourScreen() {
  const { statusId } = useWorkflowLock('YOUR_SCREEN_CODE')
  const [showAuditHistory, setShowAuditHistory] = useState(false)

  return (
    <div>
      <PageHeader
        title="Your Screen"
        secondaryActions={[
          {
            label: "Audit History",
            icon: History,
            variant: "outline",
            onClick: () => setShowAuditHistory(true),
          },
        ]}
      />

      {/* Your screen content */}

      {/* Audit History Modal */}
      <ScreenAuditHistoryModal
        open={showAuditHistory}
        onOpenChange={setShowAuditHistory}
        screenCode="YOUR_SCREEN_CODE"
        screenName="Your Screen Name"
        currentStatusId={statusId}
      />
    </div>
  )
}
```

---

## Component Props

| Prop | Type | Required | Description |
|------|------|----------|-------------|
| `open` | `boolean` | ✅ Yes | Whether modal is open |
| `onOpenChange` | `function` | ✅ Yes | Callback when modal open state changes |
| `screenCode` | `string` | ✅ Yes | Screen code (e.g., 'CENSUS_POPULATION') |
| `screenName` | `string` | ❌ No | Screen name for display in modal title |
| `currentStatusId` | `number` | ❌ No | Current workflow status ID (for highlighting rejections) |

---

## Features

### 1. **Rejection Reason Highlighting**
If the screen is rejected (Status 3 or 5), the most recent rejection reason is displayed prominently at the top in a red alert box with:
- Rejection reason text
- Who rejected it
- When it was rejected

### 2. **Tabular Display**
All audit history entries are shown in a clean table with:
- **Date & Time**: When the action occurred
- **Action**: What action was performed (with icon)
- **From Status**: Previous status (with badge)
- **To Status**: New status (with badge)
- **Action By**: User who performed the action
- **Remarks**: Any remarks/notes (if available)

### 3. **Visual Indicators**
- ✅ **Green checkmark** for approvals
- ❌ **Red X** for rejections
- ⏰ **Clock icon** for pending/submitted actions
- **Color-coded badges** for different statuses
- **Highlighted rows** for rejection entries

### 4. **Responsive Design**
- Scrollable table for long histories
- Responsive layout
- Clean, professional appearance

---

## Integration Checklist

When adding to a new screen:

- [ ] Import `ScreenAuditHistoryModal` component
- [ ] Import `History` icon from `lucide-react`
- [ ] Add `showAuditHistory` state
- [ ] Add "Audit History" button (in PageHeader or elsewhere)
- [ ] Add `<ScreenAuditHistoryModal />` component
- [ ] Set `screenCode` prop to your screen's code
- [ ] Optionally set `screenName` for better UX
- [ ] Optionally set `currentStatusId` for rejection highlighting

---

## Screen Code Registration

Before using, ensure your screen is registered in the database:

```sql
INSERT INTO [dbo].[Screen_Workflow] 
  ([ScreenCode], [ScreenName], [TargetTableName], [CurrentStatusID], [CreatedByUserID])
VALUES 
  ('YOUR_SCREEN_CODE', 'Your Screen Name', 'your_table_name', 1, @AdminUserID);
```

---

## Example: Census.jsx Implementation

```jsx
// 1. Import
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import { History } from "lucide-react"

// 2. State
const [showAuditHistory, setShowAuditHistory] = useState(false)
const { statusId: screenWorkflowStatus } = useWorkflowLock('CENSUS_POPULATION')

// 3. Button in PageHeader
secondaryActions={[
  {
    label: "Audit History",
    icon: History,
    variant: "outline",
    onClick: () => setShowAuditHistory(true),
  },
]}

// 4. Modal Component
<ScreenAuditHistoryModal
  open={showAuditHistory}
  onOpenChange={setShowAuditHistory}
  screenCode="CENSUS_POPULATION"
  screenName="Census Population Management"
  currentStatusId={screenWorkflowStatus}
/>
```

---

## Benefits

✅ **Reusable**: Works across all 350+ screens  
✅ **Consistent**: Same look and feel everywhere  
✅ **User-Friendly**: Clear rejection reasons highlighted  
✅ **Professional**: Clean table layout  
✅ **Easy Integration**: Just 4 steps to add to any screen  
✅ **Automatic**: Fetches data from API automatically  

---

## API Endpoint Used

The component uses:
- `GET /api/workflow/screen/history/{screenCode}`

This endpoint is already implemented in `WorkflowController.cs`.

---

## Styling

The component uses:
- Shadcn UI components (Dialog, Table, Badge, Alert)
- Tailwind CSS for styling
- Lucide React icons
- Format utilities for date formatting

All styling is consistent with the rest of the application.

---

## Need Help?

Refer to `Census.jsx` for a complete working example.
