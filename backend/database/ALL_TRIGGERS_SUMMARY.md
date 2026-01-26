# All Database Triggers Summary

## Triggers Found in Database Scripts

Based on the codebase analysis, the following triggers are defined in SQL scripts:

### 1. **TR_Roles_UpdatedAt**
- **Table:** `Roles` (Legacy table - may not exist)
- **Type:** AFTER UPDATE
- **Purpose:** Auto-update `UpdatedAt` column
- **Location:** `backend/database/create-database-script.sql`
- **Status:** ⚠️ **Legacy** - This table may have been replaced by `MstRole`

### 2. **TR_Users_UpdatedAt**
- **Table:** `Users` (Legacy table - may not exist)
- **Type:** AFTER UPDATE
- **Purpose:** Auto-update `UpdatedAt` column
- **Location:** `backend/database/create-database-script.sql`
- **Status:** ⚠️ **Legacy** - This table may have been replaced by `Master_User`

### 3. **TR_census_population_UpdatedAt**
- **Table:** `census_population` (Old table name - may not exist)
- **Type:** AFTER UPDATE
- **Purpose:** Auto-update `updated_at` column
- **Location:** `backend/database/create-database-script.sql`
- **Status:** ⚠️ **Legacy** - Table was renamed to `AP_Table_3_2_CensusPopulation`

### 4. **TR_AP_Table_3_2_CensusPopulation_UpdatedAt**
- **Table:** `AP_Table_3_2_CensusPopulation`
- **Type:** AFTER UPDATE
- **Purpose:** Auto-update `ModifiedDateTime` column
- **Location:** `backend/database/fix-census-population-trigger.sql`
- **Status:** ✅ **Active** (Should be removed per IST migration)

### 5. **TR_Ed_Table_6_1_Institutions_UpdatedAt**
- **Table:** `Ed_Table_6_1_Institutions`
- **Type:** AFTER UPDATE
- **Purpose:** Auto-update `ModifiedDateTime` column
- **Location:** Mentioned in documentation but not created
- **Status:** ❌ **Not Created** (No trigger exists)

---

## Current System Tables

Based on `ApplicationDbContext.cs`, the current active tables are:

### User Management Module
- `MstRole` - **No triggers found**
- `MstDepartment` - **No triggers found**
- `MstSecurityQuestion` - **No triggers found**
- `Master_User` - **No triggers found**

### Area & Population Department
- `AP_Table_3_2_CensusPopulation` - Has trigger `TR_AP_Table_3_2_CensusPopulation_UpdatedAt` ⚠️

### Education Department
- `Ed_Table_6_1_Institutions` - **No triggers**

### Menu Management Module
- `MstMenu` - **No triggers found**
- `DepartmentMenuMapping` - **No triggers found**
- `RoleMenuMapping` - **No triggers found**

### Workflow Engine Module
- `MstWorkflowStatus` - **No triggers found**
- `WorkflowAuditHistory` - **No triggers found**
- `ScreenWorkflow` - **No triggers found**

### Error Logging Module
- `ErrorLog` - **No triggers found**

---

## How to Check Active Triggers

Run the following SQL script to see all active triggers in your database:

```sql
-- Execute: backend/database/check-all-triggers.sql
```

Or run this query directly:

```sql
SELECT 
    t.name AS TriggerName,
    OBJECT_NAME(t.parent_id) AS TableName,
    t.is_disabled AS IsDisabled,
    t.create_date AS CreatedDate
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
WHERE o.type = 'U' -- User tables only
ORDER BY TableName, TriggerName;
```

---

## Recommendations

1. **Run `check-all-triggers.sql`** to see what triggers actually exist in your database
2. **Remove legacy triggers** if the tables (`Roles`, `Users`, `census_population`) no longer exist
3. **Remove `TR_AP_Table_3_2_CensusPopulation_UpdatedAt`** as per IST migration requirements
4. **Verify no triggers exist** on other tables that might interfere with IST datetime implementation

---

## Notes

- Legacy triggers (`TR_Roles_UpdatedAt`, `TR_Users_UpdatedAt`, `TR_census_population_UpdatedAt`) are defined in `create-database-script.sql` but may not exist if those tables were replaced
- The current system uses `Master_User` and `MstRole` instead of `Users` and `Roles`
- All triggers use `GETUTCDATE()` which conflicts with the IST datetime requirement
