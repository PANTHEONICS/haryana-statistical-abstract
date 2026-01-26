# Database Migration Guide: Adding CensusID Primary Key

## Overview
This migration adds a `CensusID` (INT IDENTITY) primary key to the `census_population` table, while keeping `census_year` as a unique constraint. All internal CRUD operations will use `CensusID`, while the frontend/API will continue to use `year` for backward compatibility.

## Migration Steps

### Step 1: Run the Migration Script
Execute the following SQL script in SQL Server Management Studio:

```sql
-- File: backend/database/add-id-to-census-population.sql
```

### Step 2: Verify the Changes
After running the script, verify:

1. **Primary Key**: `CensusID` should be the primary key (IDENTITY column)
2. **Unique Constraint**: `census_year` should have a unique index
3. **Data Integrity**: All existing records should have CensusID values populated
4. **Foreign Keys**: Workflow foreign keys should still be intact
5. **Triggers**: The update trigger should use CensusID

### Step 3: Code Changes (Already Done)
- ✅ Model updated: `CensusPopulation.cs` - added CensusID as primary key
- ✅ DbContext updated: `ApplicationDbContext.cs` - configured CensusID as PK
- ✅ Service updated: All CRUD operations use CensusID internally
- ✅ Controller updated: WorkflowController converts year to CensusID for census_population

### Step 4: Restart the Backend API
After migration, restart the backend API to load the new model configuration.

## Important Notes

1. **Frontend Unchanged**: The frontend will continue to use `year` for all operations. The backend automatically converts year ↔ CensusID.

2. **Workflow Integration**: The workflow service will automatically use CensusID as the primary key for census_population table.

3. **Backward Compatibility**: All existing API endpoints continue to work with `year` parameter.

## Rollback (if needed)
If you need to rollback, you would need to:
1. Recreate the primary key on `census_year`
2. Drop the `CensusID` column
3. Update the code to use `Year` as primary key again

**Note**: This migration is designed to be one-way. Ensure you have a database backup before running the migration.
