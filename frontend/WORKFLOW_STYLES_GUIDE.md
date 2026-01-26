# Workflow Styles Guide

## Available UI Components for Workflow Display

### 1. **Badge Component** (`@/components/ui/badge`)

Available variants for status badges:

```jsx
<Badge variant="default">Default</Badge>
<Badge variant="secondary">Secondary</Badge>
<Badge variant="destructive">Destructive (Red)</Badge>
<Badge variant="outline">Outline</Badge>
<Badge variant="success">Success (Green)</Badge>
<Badge variant="warning">Warning (Yellow)</Badge>
<Badge variant="info">Info (Blue)</Badge>
```

**Usage Example:**
```jsx
import { Badge } from '@/components/ui/badge';

<Badge variant="success">Approved</Badge>
<Badge variant="warning">Pending</Badge>
<Badge variant="destructive">Rejected</Badge>
```

### 2. **Button Component** (`@/components/ui/button`)

Available variants for action buttons:

```jsx
<Button variant="default">Default</Button>
<Button variant="destructive">Destructive (Red)</Button>
<Button variant="outline">Outline</Button>
<Button variant="secondary">Secondary</Button>
<Button variant="ghost">Ghost</Button>
<Button variant="link">Link</Button>
```

**Sizes:**
- `default` - h-10 px-4 py-2
- `sm` - h-9 rounded-md px-3
- `lg` - h-11 rounded-md px-8
- `icon` - h-10 w-10

**Usage Example:**
```jsx
import { Button } from '@/components/ui/button';

<Button variant="default" size="sm">Submit</Button>
<Button variant="destructive" size="sm">Reject</Button>
<Button variant="outline" size="sm">Cancel</Button>
```

### 3. **Timeline Component** (`@/components/ui/Timeline`)

Vertical timeline with icons for workflow stages:

**Status Types:**
- `completed` - Green checkmark circle
- `active` - Blue clock icon
- `pending` - Gray empty circle

**Usage Example:**
```jsx
import { Timeline } from '@/components/ui/Timeline';

<Timeline items={[
  { title: 'Maker Entry', status: 'completed', time: '10:00 AM' },
  { title: 'Checker Review', status: 'active', time: '11:00 AM' },
  { title: 'DESA Head Review', status: 'pending' },
  { title: 'Approved', status: 'pending' }
]} />
```

### 4. **StatusBadge Component** (`@/components/ui/StatusBadge`)

Pre-configured status badges:

**Status Types:**
- `success` - Green badge
- `warning` - Yellow badge
- `error` - Red badge (destructive)
- `info` - Blue badge
- `pending` - Secondary (gray)
- `active` - Default (primary)
- `inactive` - Outline

**Usage Example:**
```jsx
import { StatusBadge } from '@/components/ui/StatusBadge';

<StatusBadge status="success" label="Approved" />
<StatusBadge status="warning" label="Pending Review" />
<StatusBadge status="error" label="Rejected" />
```

### 5. **Current WorkflowStatusBar Styles**

The current implementation uses:

**Color States:**
- `completed` - Green (`bg-green-600 text-white border-green-700`)
- `current` - Blue (`bg-blue-600 text-white border-blue-700`)
- `pending` - Gray (`bg-gray-300 text-gray-600 border-gray-400`)
- `rejected` - Red (`bg-red-600 text-white border-red-700`)

**Visual Indicators:**
- Yellow dot for current status
- Red dot for rejected status
- Ring effects for emphasis
- ChevronRight arrows between stages

## Alternative Workflow Display Styles

### Style Option 1: **Vertical Timeline**
Replace the horizontal bar with a vertical timeline:

```jsx
<Timeline items={[
  { 
    title: 'Maker Entry', 
    status: 'completed', 
    description: 'Data entry completed',
    time: '10:00 AM'
  },
  { 
    title: 'Checker Review', 
    status: 'active',
    description: 'Under review',
    time: '11:00 AM'
  },
  { 
    title: 'DESA Head Review', 
    status: 'pending'
  },
  { 
    title: 'Approved', 
    status: 'pending'
  }
]} />
```

### Style Option 2: **Badge-Based Status Display**
Use badges instead of buttons:

```jsx
<div className="flex items-center gap-2">
  <Badge variant="success">Maker Entry</Badge>
  <ChevronRight className="h-4 w-4 text-gray-400" />
  <Badge variant="warning">Checker Review</Badge>
  <ChevronRight className="h-4 w-4 text-gray-400" />
  <Badge variant="info">DESA Head Review</Badge>
  <ChevronRight className="h-4 w-4 text-gray-400" />
  <Badge variant="outline">Approved</Badge>
</div>
```

### Style Option 3: **Progress Bar Style**
Linear progress indicator:

```jsx
<div className="w-full bg-gray-200 rounded-full h-2.5">
  <div 
    className="bg-blue-600 h-2.5 rounded-full transition-all"
    style={{ width: `${(currentStage / totalStages) * 100}%` }}
  />
</div>
```

### Style Option 4: **Stepper Style**
Numbered steps with connections:

```jsx
<div className="flex items-center">
  {stages.map((stage, index) => (
    <div key={index} className="flex items-center">
      <div className={cn(
        "flex items-center justify-center w-10 h-10 rounded-full border-2",
        stage.completed ? "bg-green-600 border-green-600 text-white" :
        stage.current ? "bg-blue-600 border-blue-600 text-white" :
        "bg-gray-200 border-gray-300 text-gray-500"
      )}>
        {stage.completed ? <CheckCircle2 className="h-5 w-5" /> : index + 1}
      </div>
      {index < stages.length - 1 && (
        <div className={cn(
          "h-0.5 w-16 mx-2",
          stage.completed ? "bg-green-600" : "bg-gray-300"
        )} />
      )}
    </div>
  ))}
</div>
```

### Style Option 5: **Card-Based Status Display**
Individual cards for each stage:

```jsx
<div className="grid grid-cols-4 gap-4">
  {stages.map((stage) => (
    <Card key={stage.key} className={cn(
      stage.completed && "border-green-500 bg-green-50",
      stage.current && "border-blue-500 bg-blue-50",
      stage.pending && "border-gray-300 bg-gray-50"
    )}>
      <CardHeader>
        <CardTitle className="text-sm">{stage.label}</CardTitle>
      </CardHeader>
    </Card>
  ))}
</div>
```

## Color Schemes Available

### Primary Colors (Theme-based):
- `primary` - Main theme color
- `primary-foreground` - Text on primary
- `secondary` - Secondary theme color
- `secondary-foreground` - Text on secondary

### Semantic Colors:
- `success` / `green-*` - Success states
- `warning` / `yellow-*` - Warning states
- `destructive` / `red-*` - Error/rejection states
- `info` / `blue-*` - Information states
- `muted` / `gray-*` - Neutral states

### Tailwind Color Classes:
- `bg-green-600`, `bg-green-500`, `bg-green-100`
- `bg-blue-600`, `bg-blue-500`, `bg-blue-100`
- `bg-yellow-600`, `bg-yellow-500`, `bg-yellow-100`
- `bg-red-600`, `bg-red-500`, `bg-red-100`
- `bg-gray-600`, `bg-gray-500`, `bg-gray-300`, `bg-gray-100`

## Current Implementation Details

**File:** `frontend/src/components/workflow/WorkflowStatusBar.jsx`

**Current Style:**
- Horizontal timeline with rounded buttons
- Color-coded by state (green=completed, blue=current, gray=pending, red=rejected)
- ChevronRight arrows between stages
- Yellow dot indicator for current status
- Ring effects for emphasis

**To Change Styles:**
1. Modify `getStatusColor()` function for different color schemes
2. Replace the horizontal flex layout with Timeline component for vertical display
3. Use Badge components instead of custom divs
4. Add progress bar or stepper styles as shown above
