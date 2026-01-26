# Table Rename: Screen_Workflow → Workflow_Screen

## Overview
Renamed the database table from `Screen_Workflow` to `Workflow_Screen` and updated all code references.

## Changes Made

### 1. Database Migration Script
- **File:** `backend/database/rename-screen-workflow-to-workflow-screen.sql`
- Renames the table using `sp_rename`
- Renames all indexes:
  - `IX_Screen_Workflow_ScreenCode` → `IX_Workflow_Screen_ScreenCode`
  - `IX_Screen_Workflow_TableName` → `IX_Workflow_Screen_TableName`
  - `IX_Screen_Workflow_CurrentStatusID` → `IX_Workflow_Screen_CurrentStatusID`
- Renames all foreign key constraints:
  - `FK_Screen_Workflow_Status` → `FK_Workflow_Screen_Status`
  - `FK_Screen_Workflow_CreatedBy` → `FK_Workflow_Screen_CreatedBy`

### 2. C# Model Update
- **File:** `backend/HaryanaStatAbstract.API/Models/ScreenWorkflow.cs`
- Changed `[Table("Screen_Workflow")]` to `[Table("Workflow_Screen")]`
- **Note:** Class name `ScreenWorkflow` remains unchanged (only table name changed)

### 3. SQL Scripts Updated (9 files)
All SQL scripts that referenced `Screen_Workflow` have been updated to use `Workflow_Screen`:

1. `screen-workflow-schema.sql` - Table creation script
2. `screen-workflow-seed-data.sql` - Seed data script
3. `update-education-table-6-1-screen-code.sql`
4. `verify-education-table-6-1-setup.sql`
5. `fix-education-table-6-1-complete-setup.sql`
6. `education-table-6-1-screen-workflow.sql`
7. `update-census-screen-name.sql`
8. `migrate-census-to-department-structure.sql`
9. `rename-screen-workflow-to-workflow-screen.sql` (migration script)

### 4. C# Code (No Changes Needed)
The following files use the class name `ScreenWorkflow` which remains unchanged:
- `ApplicationDbContext.cs` - DbSet name stays as `ScreenWorkflows`
- `WorkflowService.cs` - Uses `_context.ScreenWorkflows`
- `WorkflowController.cs` - Uses `_context.ScreenWorkflows`
- `WorkflowAuditHistory.cs` - Navigation property stays as `ScreenWorkflow`

**Note:** Only the database table name changed. The C# class name and all code references remain as `ScreenWorkflow` (PascalCase).

## Migration Steps

### Step 1: Run Database Migration
Execute the following SQL script in SQL Server Management Studio:
```sql
-- Run: backend/database/rename-screen-workflow-to-workflow-screen.sql
```

### Step 2: Restart Backend API
After running the migration script, restart the backend API to load the updated model.

### Step 3: Verify
Check that the application works correctly:
- Workflow actions should work
- Screen workflow status should be accessible
- Audit history should display correctly

## Rollback Plan

If you need to rollback:
1. Run the reverse migration:
   ```sql
   EXEC sp_rename '[dbo].[Workflow_Screen]', 'Screen_Workflow';
   ```
2. Revert the `[Table]` attribute in `ScreenWorkflow.cs`
3. Revert all SQL scripts (or restore from git)

## Verification Query

After migration, verify the table exists with the new name:
```sql
SELECT * FROM [dbo].[Workflow_Screen];
```

Check indexes:
```sql
SELECT name FROM sys.indexes 
WHERE object_id = OBJECT_ID('dbo.Workflow_Screen');
```

Check foreign keys:
```sql
SELECT name FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('dbo.Workflow_Screen');
```
