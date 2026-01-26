# Entity Relationship Diagram (Visual)

## Complete Database Schema Diagram

```mermaid
erDiagram
    CATEGORIES ||--o{ ENTRIES : "has"
    ENTRIES ||--o{ ENTRY_DETAILS : "has"
    ENTRIES ||--o{ ACTIVITIES : "generates"
    ENTRIES ||--o{ KANBAN_CARDS : "linked_to"
    USERS ||--o{ ACTIVITIES : "performs"
    USERS ||--o{ USER_PREFERENCES : "has"
    WORKFLOW_DEFINITIONS ||--o{ WORKFLOW_INSTANCES : "defines"
    WORKFLOW_INSTANCES ||--o{ WORKFLOW_STEP_DATA : "contains"
    KANBAN_BOARDS ||--o{ KANBAN_COLUMNS : "has"
    KANBAN_COLUMNS ||--o{ KANBAN_CARDS : "contains"
    KPI_DEFINITIONS ||--o{ KPI_VALUES : "has"
    
    CATEGORIES {
        int id PK
        string code UK
        string name
        text description
        int parent_id FK
        int sort_order
        boolean is_active
        timestamp created_at
        timestamp updated_at
    }
    
    ENTRIES {
        int id PK
        string entry_code UK
        string name
        text description
        int category_id FK
        string status
        decimal value
        string value_currency
        jsonb metadata
        timestamp created_at
        timestamp updated_at
        int created_by FK
        int updated_by FK
        timestamp deleted_at
    }
    
    ENTRY_DETAILS {
        int id PK
        int entry_id FK
        string field_key
        text field_value
        string field_type
        timestamp created_at
        timestamp updated_at
    }
    
    ACTIVITIES {
        int id PK
        string entity_type
        int entity_id
        string activity_type
        string title
        text description
        string status
        jsonb metadata
        int performed_by FK
        timestamp performed_at
    }
    
    WORKFLOW_DEFINITIONS {
        int id PK
        string code UK
        string name
        text description
        jsonb steps
        boolean is_active
        timestamp created_at
        timestamp updated_at
    }
    
    WORKFLOW_INSTANCES {
        int id PK
        int workflow_definition_id FK
        string entity_type
        int entity_id
        int current_step
        string status
        timestamp started_at
        timestamp completed_at
        jsonb metadata
        int created_by FK
    }
    
    WORKFLOW_STEP_DATA {
        int id PK
        int workflow_instance_id FK
        int step_index
        jsonb step_data
        timestamp completed_at
    }
    
    KANBAN_BOARDS {
        int id PK
        string code UK
        string name
        text description
        boolean is_active
        timestamp created_at
        timestamp updated_at
        int created_by FK
    }
    
    KANBAN_COLUMNS {
        int id PK
        int board_id FK
        string code
        string name
        int sort_order
        string color
        boolean is_active
    }
    
    KANBAN_CARDS {
        int id PK
        int board_id FK
        int column_id FK
        int entry_id FK
        string title
        text description
        jsonb metadata
        int sort_order
        timestamp created_at
        timestamp updated_at
        int created_by FK
    }
    
    KPI_DEFINITIONS {
        int id PK
        string code UK
        string name
        text description
        string calculation_type
        text calculation_query
        string unit
        boolean is_active
        timestamp created_at
    }
    
    KPI_VALUES {
        int id PK
        int kpi_id FK
        string period_type
        date period_start
        date period_end
        decimal value
        decimal previous_value
        decimal change_percentage
        timestamp calculated_at
    }
    
    CHART_DATA {
        int id PK
        string chart_type
        string chart_code
        string period_type
        date period_start
        jsonb data_points
        timestamp created_at
    }
    
    USERS {
        int id PK
        string username UK
        string email UK
        string password_hash
        string first_name
        string last_name
        string role
        boolean is_active
        timestamp last_login_at
        timestamp created_at
        timestamp updated_at
    }
    
    USER_PREFERENCES {
        int id PK
        int user_id FK
        string preference_key
        text preference_value
    }
    
    SYSTEM_SETTINGS {
        int id PK
        string setting_key UK
        text setting_value
        string setting_type
        text description
        timestamp updated_at
        int updated_by FK
    }
```

## Key Relationships Explained

1. **Categories → Entries**: One category can have many entries (optional relationship)
2. **Entries → Entry Details**: Flexible attribute storage for entries
3. **Entries → Activities**: Complete audit trail for all entry changes
4. **Workflow Definitions → Instances → Step Data**: Workflow execution tracking
5. **Kanban Boards → Columns → Cards**: Hierarchical kanban structure
6. **KPI Definitions → KPI Values**: Time-series metric storage
7. **Users → Activities**: Track who performed what actions
8. **Users → Preferences**: User-specific settings storage

## Data Cardinality

- **1:Many** relationships are the primary pattern
- **Optional** relationships allow for flexible data models
- **Soft deletes** on entries preserve referential integrity
- **JSONB fields** provide extensibility without schema changes
