-- Enterprise UI Design System - Database Schema
-- Database: PostgreSQL (recommended) or MySQL/MariaDB

-- ============================================
-- CORE ENTITIES
-- ============================================

-- Categories Table
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    parent_id INTEGER REFERENCES categories(id),
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER,
    updated_by INTEGER
);

CREATE INDEX idx_categories_parent ON categories(parent_id);
CREATE INDEX idx_categories_active ON categories(is_active);

-- Entries Table (Primary Data Entity)
CREATE TABLE entries (
    id SERIAL PRIMARY KEY,
    entry_code VARCHAR(50) UNIQUE NOT NULL, -- e.g., "ENT-001"
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category_id INTEGER REFERENCES categories(id),
    status VARCHAR(50) NOT NULL DEFAULT 'pending', -- active, pending, inactive
    value DECIMAL(15, 2),
    value_currency VARCHAR(3) DEFAULT 'USD',
    metadata JSONB, -- Flexible metadata storage
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER,
    updated_by INTEGER,
    deleted_at TIMESTAMP NULL
);

CREATE INDEX idx_entries_code ON entries(entry_code);
CREATE INDEX idx_entries_category ON entries(category_id);
CREATE INDEX idx_entries_status ON entries(status);
CREATE INDEX idx_entries_created ON entries(created_at);
CREATE INDEX idx_entries_metadata ON entries USING GIN(metadata);
CREATE INDEX idx_entries_deleted ON entries(deleted_at) WHERE deleted_at IS NULL;

-- Entry Details Table (Extended attributes)
CREATE TABLE entry_details (
    id SERIAL PRIMARY KEY,
    entry_id INTEGER NOT NULL REFERENCES entries(id) ON DELETE CASCADE,
    field_key VARCHAR(100) NOT NULL,
    field_value TEXT,
    field_type VARCHAR(50) DEFAULT 'text', -- text, number, date, boolean, json
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(entry_id, field_key)
);

CREATE INDEX idx_entry_details_entry ON entry_details(entry_id);
CREATE INDEX idx_entry_details_key ON entry_details(field_key);

-- ============================================
-- ACTIVITY & AUDIT
-- ============================================

-- Activity Log Table
CREATE TABLE activities (
    id SERIAL PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL, -- 'entry', 'workflow', 'board', etc.
    entity_id INTEGER NOT NULL,
    activity_type VARCHAR(50) NOT NULL, -- 'created', 'updated', 'deleted', 'status_changed'
    title VARCHAR(255) NOT NULL,
    description TEXT,
    status VARCHAR(50) DEFAULT 'completed', -- completed, active, pending
    metadata JSONB,
    performed_by INTEGER,
    performed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_activities_entity ON activities(entity_type, entity_id);
CREATE INDEX idx_activities_type ON activities(activity_type);
CREATE INDEX idx_activities_performed_at ON activities(performed_at);
CREATE INDEX idx_activities_status ON activities(status);

-- ============================================
-- WORKFLOW MANAGEMENT
-- ============================================

-- Workflow Definitions
CREATE TABLE workflow_definitions (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    steps JSONB NOT NULL, -- Array of step definitions
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_workflow_definitions_code ON workflow_definitions(code);
CREATE INDEX idx_workflow_definitions_active ON workflow_definitions(is_active);

-- Workflow Instances
CREATE TABLE workflow_instances (
    id SERIAL PRIMARY KEY,
    workflow_definition_id INTEGER REFERENCES workflow_definitions(id),
    entity_type VARCHAR(50), -- What entity this workflow is for
    entity_id INTEGER,
    current_step INTEGER DEFAULT 0,
    status VARCHAR(50) DEFAULT 'pending', -- pending, in_progress, completed, failed
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMP,
    metadata JSONB,
    created_by INTEGER
);

CREATE INDEX idx_workflow_instances_definition ON workflow_instances(workflow_definition_id);
CREATE INDEX idx_workflow_instances_entity ON workflow_instances(entity_type, entity_id);
CREATE INDEX idx_workflow_instances_status ON workflow_instances(status);

-- Workflow Step Data
CREATE TABLE workflow_step_data (
    id SERIAL PRIMARY KEY,
    workflow_instance_id INTEGER REFERENCES workflow_instances(id) ON DELETE CASCADE,
    step_index INTEGER NOT NULL,
    step_data JSONB, -- Step-specific data
    completed_at TIMESTAMP,
    UNIQUE(workflow_instance_id, step_index)
);

CREATE INDEX idx_workflow_step_data_instance ON workflow_step_data(workflow_instance_id);

-- ============================================
-- KANBAN BOARDS
-- ============================================

-- Kanban Boards
CREATE TABLE kanban_boards (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER
);

CREATE INDEX idx_kanban_boards_code ON kanban_boards(code);

-- Kanban Columns
CREATE TABLE kanban_columns (
    id SERIAL PRIMARY KEY,
    board_id INTEGER NOT NULL REFERENCES kanban_boards(id) ON DELETE CASCADE,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    sort_order INTEGER DEFAULT 0,
    color VARCHAR(7), -- Hex color code
    is_active BOOLEAN DEFAULT true,
    UNIQUE(board_id, code)
);

CREATE INDEX idx_kanban_columns_board ON kanban_columns(board_id);
CREATE INDEX idx_kanban_columns_order ON kanban_columns(board_id, sort_order);

-- Kanban Cards
CREATE TABLE kanban_cards (
    id SERIAL PRIMARY KEY,
    board_id INTEGER NOT NULL REFERENCES kanban_boards(id) ON DELETE CASCADE,
    column_id INTEGER NOT NULL REFERENCES kanban_columns(id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    entry_id INTEGER REFERENCES entries(id), -- Link to entry if applicable
    metadata JSONB,
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER
);

CREATE INDEX idx_kanban_cards_board ON kanban_cards(board_id);
CREATE INDEX idx_kanban_cards_column ON kanban_cards(column_id);
CREATE INDEX idx_kanban_cards_entry ON kanban_cards(entry_id);
CREATE INDEX idx_kanban_cards_order ON kanban_cards(column_id, sort_order);

-- ============================================
-- ANALYTICS & METRICS
-- ============================================

-- KPI Definitions
CREATE TABLE kpi_definitions (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    calculation_type VARCHAR(50) NOT NULL, -- 'count', 'sum', 'avg', 'custom'
    calculation_query TEXT, -- SQL or formula
    unit VARCHAR(50), -- 'number', 'currency', 'percentage'
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_kpi_definitions_code ON kpi_definitions(code);

-- KPI Values (Time-series data)
CREATE TABLE kpi_values (
    id SERIAL PRIMARY KEY,
    kpi_id INTEGER NOT NULL REFERENCES kpi_definitions(id),
    period_type VARCHAR(20) NOT NULL, -- 'daily', 'weekly', 'monthly'
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    value DECIMAL(20, 4) NOT NULL,
    previous_value DECIMAL(20, 4),
    change_percentage DECIMAL(10, 2),
    calculated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(kpi_id, period_type, period_start)
);

CREATE INDEX idx_kpi_values_kpi ON kpi_values(kpi_id);
CREATE INDEX idx_kpi_values_period ON kpi_values(period_type, period_start);

-- Chart Data (Pre-aggregated for performance)
CREATE TABLE chart_data (
    id SERIAL PRIMARY KEY,
    chart_type VARCHAR(50) NOT NULL, -- 'line', 'bar', 'area', 'pie'
    chart_code VARCHAR(50) NOT NULL,
    period_type VARCHAR(20) NOT NULL,
    period_start DATE NOT NULL,
    data_points JSONB NOT NULL, -- Array of {name, value, value2, ...}
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(chart_code, period_type, period_start)
);

CREATE INDEX idx_chart_data_code ON chart_data(chart_code);
CREATE INDEX idx_chart_data_period ON chart_data(period_type, period_start);

-- ============================================
-- USER MANAGEMENT
-- ============================================

-- Users Table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    role VARCHAR(50) DEFAULT 'user', -- 'admin', 'user', 'viewer'
    is_active BOOLEAN DEFAULT true,
    last_login_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);

-- User Preferences
CREATE TABLE user_preferences (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    preference_key VARCHAR(100) NOT NULL,
    preference_value TEXT,
    UNIQUE(user_id, preference_key)
);

CREATE INDEX idx_user_preferences_user ON user_preferences(user_id);

-- ============================================
-- SYSTEM SETTINGS
-- ============================================

-- System Settings
CREATE TABLE system_settings (
    id SERIAL PRIMARY KEY,
    setting_key VARCHAR(100) UNIQUE NOT NULL,
    setting_value TEXT,
    setting_type VARCHAR(50) DEFAULT 'string', -- string, number, boolean, json
    description TEXT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_by INTEGER
);

CREATE INDEX idx_system_settings_key ON system_settings(setting_key);

-- ============================================
-- TRIGGERS FOR UPDATED_AT
-- ============================================

CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_entries_updated_at BEFORE UPDATE ON entries
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_workflow_definitions_updated_at BEFORE UPDATE ON workflow_definitions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_kanban_boards_updated_at BEFORE UPDATE ON kanban_boards
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_kanban_cards_updated_at BEFORE UPDATE ON kanban_cards
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
