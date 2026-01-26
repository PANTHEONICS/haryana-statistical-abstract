# Entity Relationship Diagram

## ERD Overview

This document provides a visual representation of the database relationships and entity connections.

## Core Entities

```
┌─────────────┐
│  Categories │
└──────┬──────┘
       │ 1
       │
       │ *
┌──────▼──────┐
│   Entries   │
└──────┬──────┘
       │ 1
       │
       │ *
┌──────▼──────────────┐
│  Entry Details      │
└─────────────────────┘
```

## Activity & Audit Trail

```
┌─────────────┐
│   Entries   │
└──────┬──────┘
       │ 1
       │
       │ *
┌──────▼──────┐
│ Activities  │
└─────────────┘
```

## Workflow Management

```
┌─────────────────────┐
│ Workflow Definitions│
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│ Workflow Instances  │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│  Workflow Step Data │
└─────────────────────┘
```

## Kanban Board Structure

```
┌──────────────┐
│ Kanban Boards│
└──────┬───────┘
       │ 1
       │
       │ *
┌──────▼────────┐
│ Kanban Columns│
└──────┬────────┘
       │ 1
       │
       │ *
┌──────▼────────┐
│ Kanban Cards  │
└──────┬────────┘
       │
       │ * (optional)
       │
┌──────▼──────┐
│   Entries  │
└─────────────┘
```

## Analytics Structure

```
┌─────────────────┐
│ KPI Definitions │
└────────┬────────┘
         │ 1
         │
         │ *
┌────────▼────────┐
│   KPI Values     │
│  (Time-Series)   │
└──────────────────┘

┌──────────────┐
│  Chart Data  │
│ (Pre-aggregated)│
└──────────────┘
```

## User Management

```
┌─────────────┐
│    Users    │
└──────┬──────┘
       │ 1
       │
       │ *
┌──────▼──────────────┐
│ User Preferences    │
└─────────────────────┘
```

## Complete Relationship Map

```
Categories (1) ──< (*) Entries (1) ──< (*) Entry Details
                              │
                              │ (1)
                              │
                              │ (*)
                         Activities
                              │
                              │ (performed_by)
                              │
                              │ (*)
                         Users (1) ──< (*) User Preferences

Entries (1) ──< (*) Kanban Cards (1) ──< (*) Kanban Columns (1) ──< (*) Kanban Boards

Workflow Definitions (1) ──< (*) Workflow Instances (1) ──< (*) Workflow Step Data
                                                                  │
                                                                  │ (entity_id)
                                                                  │
                                                                  │ (*)
                                                                 Entries

KPI Definitions (1) ──< (*) KPI Values
```

## Key Relationships

### One-to-Many Relationships

1. **Category → Entries**
   - One category can have many entries
   - Entry belongs to one category (nullable)

2. **Entry → Entry Details**
   - One entry can have many detail fields
   - Flexible attribute storage

3. **Entry → Activities**
   - One entry can have many activity logs
   - Complete audit trail

4. **Workflow Definition → Workflow Instances**
   - One definition can have many instances
   - Reusable workflow templates

5. **Kanban Board → Columns → Cards**
   - Hierarchical structure
   - Cards can be moved between columns

6. **User → Preferences**
   - One user can have many preferences
   - Key-value storage

### Many-to-Many Relationships

1. **Entries ↔ Kanban Cards**
   - One entry can be linked to multiple cards
   - One card can reference one entry (optional)

### Referential Integrity

- **Cascade Deletes:**
  - Entry Details (when Entry deleted)
  - Workflow Step Data (when Instance deleted)
  - Kanban Columns (when Board deleted)
  - Kanban Cards (when Board/Column deleted)
  - User Preferences (when User deleted)

- **Restrict Deletes:**
  - Entries (if referenced by Activities)
  - Categories (if referenced by Entries)
  - Workflow Definitions (if referenced by Instances)

## Data Cardinality Summary

| Parent Entity | Child Entity | Relationship | Cardinality |
|--------------|--------------|--------------|-------------|
| Categories | Entries | One-to-Many | 1 : 0..* |
| Entries | Entry Details | One-to-Many | 1 : 0..* |
| Entries | Activities | One-to-Many | 1 : 0..* |
| Users | Activities | One-to-Many | 1 : 0..* |
| Workflow Definitions | Workflow Instances | One-to-Many | 1 : 0..* |
| Workflow Instances | Workflow Step Data | One-to-Many | 1 : 0..* |
| Kanban Boards | Kanban Columns | One-to-Many | 1 : 1..* |
| Kanban Columns | Kanban Cards | One-to-Many | 1 : 0..* |
| Entries | Kanban Cards | One-to-Many (optional) | 1 : 0..* |
| KPI Definitions | KPI Values | One-to-Many | 1 : 0..* |
| Users | User Preferences | One-to-Many | 1 : 0..* |
