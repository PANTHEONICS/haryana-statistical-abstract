-- ============================================
-- VERIFY EDUCATION TABLE 6.1 SETUP
-- Diagnostic script to check all required entries
-- ============================================

USE [HaryanaStatAbstractDb];
GO

PRINT '========================================';
PRINT 'EDUCATION TABLE 6.1 - SETUP VERIFICATION';
PRINT '========================================';
PRINT '';

-- 1. Check if table exists
PRINT '1. Checking if Ed_Table_6_1_Institutions table exists...';
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND type in (N'U'))
BEGIN
    PRINT '   ✅ Table exists';
    DECLARE @RowCount INT = (SELECT COUNT(*) FROM [dbo].[Ed_Table_6_1_Institutions]);
    PRINT '   📊 Row count: ' + CAST(@RowCount AS NVARCHAR(10));
END
ELSE
BEGIN
    PRINT '   ❌ Table does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-table-6-1-schema.sql';
END
PRINT '';

-- 2. Check if Education Department exists
PRINT '2. Checking if Education Department exists...';
DECLARE @EducationDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
IF @EducationDeptID IS NOT NULL
BEGIN
    DECLARE @DeptName NVARCHAR(200) = (SELECT [DepartmentName] FROM [dbo].[Mst_Departments] WHERE [DepartmentID] = @EducationDeptID);
    PRINT '   ✅ Department exists';
    PRINT '   📋 Department ID: ' + CAST(@EducationDeptID AS NVARCHAR(10));
    PRINT '   📋 Department Name: ' + @DeptName;
END
ELSE
BEGIN
    PRINT '   ❌ Education Department does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-table-6-1-screen-registry.sql';
END
PRINT '';

-- 3. Check if Menu Entry exists
PRINT '3. Checking if Menu Entry exists...';
IF EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1')
BEGIN
    DECLARE @MenuID INT = (SELECT [MenuID] FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1');
    DECLARE @MenuName NVARCHAR(200) = (SELECT [MenuName] FROM [dbo].[Mst_Menus] WHERE [MenuID] = @MenuID);
    DECLARE @MenuActive BIT = (SELECT [IsActive] FROM [dbo].[Mst_Menus] WHERE [MenuID] = @MenuID);
    PRINT '   ✅ Menu entry exists';
    PRINT '   📋 Menu ID: ' + CAST(@MenuID AS NVARCHAR(10));
    PRINT '   📋 Menu Name: ' + @MenuName;
    PRINT '   📋 Is Active: ' + CASE WHEN @MenuActive = 1 THEN 'Yes' ELSE 'No' END;
END
ELSE
BEGIN
    PRINT '   ❌ Menu entry does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-table-6-1-menu-entry.sql';
END
PRINT '';

-- 4. Check if Menu is assigned to Education Department
PRINT '4. Checking if Menu is assigned to Education Department...';
IF @EducationDeptID IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM [dbo].[Mst_Department_Menu_Mapping] 
               WHERE [DepartmentID] = @EducationDeptID 
               AND [MenuID] = (SELECT [MenuID] FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1')
               AND [IsActive] = 1)
    BEGIN
        PRINT '   ✅ Menu is assigned to Education Department';
    END
    ELSE
    BEGIN
        PRINT '   ❌ Menu is NOT assigned to Education Department';
        PRINT '   ⚠️  Go to Menu Configuration (/menu-config) and assign the menu';
    END
END
ELSE
BEGIN
    PRINT '   ⚠️  Cannot check menu assignment (Department not found)';
END
PRINT '';

-- 5. Check if Screen Registry Entry exists
PRINT '5. Checking if Screen Registry Entry exists...';
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'Ed_Table_6_1_Institutions')
BEGIN
    DECLARE @ScreenName NVARCHAR(200) = (SELECT [ScreenName] FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'Ed_Table_6_1_Institutions');
    PRINT '   ✅ Screen Registry entry exists';
    PRINT '   📋 Screen Name: ' + @ScreenName;
END
ELSE
BEGIN
    PRINT '   ❌ Screen Registry entry does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-table-6-1-screen-registry.sql';
END
PRINT '';

-- 6. Check if Screen Workflow Entry exists
PRINT '6. Checking if Screen Workflow Entry exists...';
IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'Ed_Table_6_1_Institutions')
BEGIN
    DECLARE @CurrentStatusID INT = (SELECT [CurrentStatusID] FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'Ed_Table_6_1_Institutions');
    DECLARE @StatusName NVARCHAR(100) = (SELECT [StatusName] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusID] = @CurrentStatusID);
    DECLARE @IsActive BIT = (SELECT [IsActive] FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'Ed_Table_6_1_Institutions');
    PRINT '   ✅ Screen Workflow entry exists';
    PRINT '   📋 Current Status ID: ' + CAST(@CurrentStatusID AS NVARCHAR(10));
    PRINT '   📋 Status Name: ' + @StatusName;
    PRINT '   📋 Is Active: ' + CASE WHEN @IsActive = 1 THEN 'Yes' ELSE 'No' END;
END
ELSE
BEGIN
    PRINT '   ❌ Screen Workflow entry does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-table-6-1-screen-workflow.sql';
    PRINT '   ⚠️  This is likely the cause of the screen not opening!';
END
PRINT '';

-- 7. Check if Education users exist
PRINT '7. Checking if Education users exist...';
IF EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_maker')
BEGIN
    DECLARE @MakerActive BIT = (SELECT [IsActive] FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_maker');
    PRINT '   ✅ edu_maker user exists';
    PRINT '   📋 Is Active: ' + CASE WHEN @MakerActive = 1 THEN 'Yes' ELSE 'No' END;
END
ELSE
BEGIN
    PRINT '   ❌ edu_maker user does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-department-users.sql';
END

IF EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_check')
BEGIN
    DECLARE @CheckerActive BIT = (SELECT [IsActive] FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_check');
    PRINT '   ✅ edu_check user exists';
    PRINT '   📋 Is Active: ' + CASE WHEN @CheckerActive = 1 THEN 'Yes' ELSE 'No' END;
END
ELSE
BEGIN
    PRINT '   ❌ edu_check user does NOT exist';
    PRINT '   ⚠️  Run: backend/database/education-department-users.sql';
END
PRINT '';

PRINT '========================================';
PRINT 'VERIFICATION COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'If any entries are missing, run the corresponding SQL scripts listed above.';
PRINT 'The most critical entry is the Screen Workflow entry (#6).';
GO
