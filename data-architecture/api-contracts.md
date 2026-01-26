# API Contracts

## Base URL
```
/api/v1
```

## Authentication
All endpoints require authentication via Bearer token:
```
Authorization: Bearer <token>
```

---

## Entries API

### GET /entries
List entries with filtering, sorting, and pagination.

**Query Parameters:**
- `page` (integer, default: 1)
- `limit` (integer, default: 20, max: 100)
- `search` (string) - Search in name and description
- `category_id` (integer) - Filter by category
- `status` (string) - Filter by status (active, pending, inactive)
- `date_from` (date) - Filter by created date from
- `date_to` (date) - Filter by created date to
- `sort_by` (string, default: created_at) - Field to sort by
- `sort_order` (string, default: desc) - asc or desc

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "entry_code": "ENT-001",
      "name": "Item A",
      "description": "Description text",
      "category": {
        "id": 1,
        "code": "CAT-001",
        "name": "Category 1"
      },
      "status": "active",
      "value": 1234.00,
      "value_currency": "USD",
      "metadata": {},
      "created_at": "2024-01-15T10:00:00Z",
      "updated_at": "2024-01-20T14:30:00Z",
      "created_by": {
        "id": 1,
        "username": "john.doe"
      }
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "total_pages": 8
  }
}
```

### GET /entries/:id
Get a single entry by ID.

**Response:**
```json
{
  "data": {
    "id": 1,
    "entry_code": "ENT-001",
    "name": "Primary Entry",
    "description": "Detailed description",
    "category": { ... },
    "status": "active",
    "value": 1234.00,
    "value_currency": "USD",
    "metadata": {
      "field1": "value1",
      "field2": "value2"
    },
    "details": [
      {
        "field_key": "field1",
        "field_value": "value1",
        "field_type": "text"
      }
    ],
    "created_at": "2024-01-15T10:00:00Z",
    "updated_at": "2024-01-20T14:30:00Z"
  }
}
```

### POST /entries
Create a new entry.

**Request Body:**
```json
{
  "name": "New Entry",
  "description": "Entry description",
  "category_id": 1,
  "status": "pending",
  "value": 5000.00,
  "value_currency": "USD",
  "metadata": {
    "custom_field": "value"
  },
  "details": [
    {
      "field_key": "field1",
      "field_value": "value1",
      "field_type": "text"
    }
  ]
}
```

**Response:** 201 Created
```json
{
  "data": {
    "id": 123,
    "entry_code": "ENT-123",
    ...
  }
}
```

### PUT /entries/:id
Update an existing entry.

**Request Body:** (same as POST, all fields optional)

**Response:** 200 OK

### DELETE /entries/:id
Soft delete an entry.

**Response:** 204 No Content

---

## Categories API

### GET /categories
List all categories.

**Query Parameters:**
- `parent_id` (integer) - Filter by parent category
- `is_active` (boolean) - Filter by active status

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "code": "CAT-001",
      "name": "Category 1",
      "description": "Category description",
      "parent_id": null,
      "sort_order": 0,
      "is_active": true
    }
  ]
}
```

---

## Activities API

### GET /activities
Get activity log.

**Query Parameters:**
- `entity_type` (string) - Filter by entity type
- `entity_id` (integer) - Filter by entity ID
- `activity_type` (string) - Filter by activity type
- `page` (integer)
- `limit` (integer)

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "entity_type": "entry",
      "entity_id": 1,
      "activity_type": "updated",
      "title": "Entry Updated",
      "description": "Details were modified by user",
      "status": "completed",
      "performed_by": {
        "id": 1,
        "username": "john.doe"
      },
      "performed_at": "2024-01-20T12:00:00Z"
    }
  ],
  "pagination": { ... }
}
```

---

## Workflows API

### GET /workflows/definitions
List workflow definitions.

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "code": "WF-001",
      "name": "Standard Workflow",
      "description": "Default workflow process",
      "steps": [
        {
          "index": 0,
          "title": "Initialization",
          "description": "Set up parameters"
        },
        {
          "index": 1,
          "title": "Processing",
          "description": "Execute process"
        }
      ],
      "is_active": true
    }
  ]
}
```

### GET /workflows/instances
List workflow instances.

**Query Parameters:**
- `workflow_definition_id` (integer)
- `entity_type` (string)
- `entity_id` (integer)
- `status` (string)

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "workflow_definition": { ... },
      "entity_type": "entry",
      "entity_id": 1,
      "current_step": 1,
      "status": "in_progress",
      "started_at": "2024-01-20T10:00:00Z",
      "step_data": [
        {
          "step_index": 0,
          "step_data": { ... },
          "completed_at": "2024-01-20T10:15:00Z"
        }
      ]
    }
  ]
}
```

### POST /workflows/instances
Create a new workflow instance.

**Request Body:**
```json
{
  "workflow_definition_id": 1,
  "entity_type": "entry",
  "entity_id": 1
}
```

---

## Kanban Boards API

### GET /kanban/boards
List kanban boards.

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "code": "KB-001",
      "name": "Main Board",
      "description": "Primary kanban board",
      "columns": [
        {
          "id": 1,
          "code": "todo",
          "name": "To Do",
          "sort_order": 0,
          "color": "#3b82f6",
          "card_count": 5
        }
      ],
      "cards": [
        {
          "id": 1,
          "column_id": 1,
          "title": "Task A",
          "description": "Task description",
          "entry_id": 1,
          "sort_order": 0
        }
      ]
    }
  ]
}
```

### PUT /kanban/cards/:id/move
Move a card to a different column.

**Request Body:**
```json
{
  "column_id": 2,
  "sort_order": 0
}
```

---

## Analytics API

### GET /analytics/kpis
Get KPI values.

**Query Parameters:**
- `kpi_code` (string) - KPI code
- `period_type` (string) - daily, weekly, monthly
- `period_start` (date)
- `period_end` (date)

**Response:**
```json
{
  "data": [
    {
      "kpi": {
        "code": "TOTAL_ITEMS",
        "name": "Total Items",
        "unit": "number"
      },
      "period_type": "monthly",
      "period_start": "2024-01-01",
      "period_end": "2024-01-31",
      "value": 12345,
      "previous_value": 10987,
      "change_percentage": 12.5
    }
  ]
}
```

### GET /analytics/charts/:chart_code
Get chart data.

**Query Parameters:**
- `period_type` (string)
- `period_start` (date)
- `period_end` (date)

**Response:**
```json
{
  "data": {
    "chart_code": "PERFORMANCE_OVERVIEW",
    "chart_type": "line",
    "period_type": "monthly",
    "data_points": [
      {
        "name": "Jan",
        "value": 4000,
        "value2": 2400
      },
      {
        "name": "Feb",
        "value": 3000,
        "value2": 1398
      }
    ]
  }
}
```

---

## Error Responses

All endpoints return errors in this format:

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "name",
        "message": "Name is required"
      }
    ]
  }
}
```

**HTTP Status Codes:**
- 200 OK - Success
- 201 Created - Resource created
- 204 No Content - Success with no body
- 400 Bad Request - Invalid input
- 401 Unauthorized - Authentication required
- 403 Forbidden - Insufficient permissions
- 404 Not Found - Resource not found
- 500 Internal Server Error - Server error
