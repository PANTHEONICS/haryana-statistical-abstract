-- ============================================
-- COMPLETE FIX FOR EDUCATION TABLE 6.1
-- This script ensures all required entries exist
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'EDUCATION TABLE 6.1 - COMPLETE SETUP';
PRINT '========================================';
PRINT '';

-- Step 1: Ensure Education Department exists
PRINT 'Step 1: Checking Education Department...';
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Education', 'EDU');
    PRINT '  ✅ Created Education Department';
END
ELSE
BEGIN
    PRINT '  ✅ Education Department already exists';
END
GO

-- Step 2: Ensure Menu Entry exists
PRINT '';
PRINT 'Step 2: Checking Menu Entry...';
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1')
BEGIN
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    
    INSERT INTO [dbo].[Mst_Menus] (
        [MenuName],
        [MenuPath],
        [MenuIcon],
        [ParentMenuID],
        [DisplayOrder],
        [IsActive],
        [IsAdminOnly],
        [MenuDescription],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        'Institutions Management (Table 6.1)',
        '/education/table6-1',
        'Building2',
        NULL,
        @NextDisplayOrder,
        1,
        0,
        'Number of recognised universities/colleges/schools in Haryana',
        GETUTCDATE(),
        GETUTCDATE()
    );
    PRINT '  ✅ Created Menu Entry';
END
ELSE
BEGIN
    PRINT '  ✅ Menu Entry already exists';
END
GO

-- Step 3: Ensure Screen Registry Entry exists
PRINT '';
PRINT 'Step 3: Checking Screen Registry Entry...';
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    DECLARE @EducationDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
    
    IF @EducationDeptID IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Mst_ScreenRegistry] (
            [ScreenCode],
            [ScreenName],
            [TableName],
            [DepartmentID],
            [TableNumber],
            [SourceDocument],
            [SourceTable],
            [Description],
            [IsActive],
            [DisplayOrder],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES (
            'ED_TABLE_6_1_INSTITUTIONS',
            'Institutions Management (Table 6.1)',
            'Ed_Table_6_1_Institutions',
            @EducationDeptID,
            '6.1',
            'Statistical Abstract of Haryana 2023-24',
            'Table 6.1',
            'Number of recognised universities/colleges/schools in Haryana',
            1,
            1,
            GETUTCDATE(),
            GETUTCDATE()
        );
        PRINT '  ✅ Created Screen Registry Entry';
    END
    ELSE
    BEGIN
        PRINT '  ❌ ERROR: Education Department not found';
    END
END
ELSE
BEGIN
    PRINT '  ✅ Screen Registry Entry already exists';
END
GO

-- Step 4: Ensure Screen Workflow Entry exists (CRITICAL)
PRINT '';
PRINT 'Step 4: Checking Screen Workflow Entry (CRITICAL)...';
IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    -- Get Admin User ID
    DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
    
    -- Get Maker Entry Status ID (StatusCode is 'Draft')
    DECLARE @MakerEntryStatusID INT = (SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft');
    
    -- Fallback to StatusID = 1 if StatusCode lookup fails
    IF @MakerEntryStatusID IS NULL
    BEGIN
        SET @MakerEntryStatusID = (SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusID] = 1);
    END
    
    IF @AdminUserID IS NOT NULL AND @MakerEntryStatusID IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Workflow_Screen] (
            [ScreenCode],
            [ScreenName],
            [TableName],
            [CurrentStatusID],
            [CreatedByUserID],
            [IsActive],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES (
            'ED_TABLE_6_1_INSTITUTIONS',
            'Institutions Management (Table 6.1)',
            'Ed_Table_6_1_Institutions',
            @MakerEntryStatusID,
            @AdminUserID,
            1,
            GETUTCDATE(),
            GETUTCDATE()
        );
        PRINT '  ✅ Created Screen Workflow Entry';
        PRINT '     ScreenCode: ED_TABLE_6_1_INSTITUTIONS';
        PRINT '     StatusID: ' + CAST(@MakerEntryStatusID AS NVARCHAR(10));
        PRINT '     CreatedByUserID: ' + CAST(@AdminUserID AS NVARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT '  ❌ ERROR: Required dependencies not found';
        IF @AdminUserID IS NULL
            PRINT '     - Admin User (LoginID: admin) NOT FOUND';
        IF @MakerEntryStatusID IS NULL
            PRINT '     - Maker Entry Status (StatusCode: Draft) NOT FOUND';
    END
END
ELSE
BEGIN
    PRINT '  ✅ Screen Workflow Entry already exists';
END
GO

-- Step 5: Verify all entries
PRINT '';
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';
PRINT '';

-- Verify Menu Entry
PRINT '1. Menu Entry:';
IF EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1')
BEGIN
    SELECT 
        [MenuID],
        [MenuName],
        [MenuPath],
        [IsActive]
    FROM [dbo].[Mst_Menus]
    WHERE [MenuPath] = '/education/table6-1';
END
ELSE
BEGIN
    PRINT '  ❌ Menu Entry NOT FOUND';
END
PRINT '';

-- Verify Screen Registry
PRINT '2. Screen Registry Entry:';
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    SELECT 
        [ScreenRegistryID],
        [ScreenCode],
        [ScreenName],
        [TableName],
        [DepartmentID],
        [TableNumber],
        [SourceDocument],
        [SourceTable],
        [IsActive]
    FROM [dbo].[Mst_ScreenRegistry]
    WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS';
END
ELSE
BEGIN
    PRINT '  ❌ Screen Registry Entry NOT FOUND';
END
PRINT '';

-- Verify Screen Workflow (MOST CRITICAL)
PRINT '3. Screen Workflow Entry (MOST CRITICAL):';
IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    SELECT 
        sw.[ScreenWorkflowID],
        sw.[ScreenCode],
        sw.[ScreenName],
        sw.[TableName],
        sw.[CurrentStatusID],
        ws.[StatusName],
        ws.[StatusCode],
        sw.[CreatedByUserID],
        sw.[IsActive]
    FROM [dbo].[Workflow_Screen] sw
    LEFT JOIN [dbo].[Mst_WorkflowStatus] ws ON sw.[CurrentStatusID] = ws.[StatusID]
    WHERE sw.[ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS';
END
ELSE
BEGIN
    PRINT '  ❌ Screen Workflow Entry NOT FOUND - THIS IS THE PROBLEM!';
    PRINT '     The screen will not load without this entry.';
END
PRINT '';

-- Verify Menu Assignment to Education Department
PRINT '4. Menu Assignment to Education Department:';
DECLARE @MenuID INT = (SELECT [MenuID] FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1');
DECLARE @EducationDeptID2 INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');

IF @MenuID IS NOT NULL AND @EducationDeptID2 IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM [dbo].[Mst_Department_Menu_Mapping] 
               WHERE [DepartmentID] = @EducationDeptID2 
               AND [MenuID] = @MenuID
               AND [IsActive] = 1)
    BEGIN
        PRINT '  ✅ Menu is assigned to Education Department';
    END
    ELSE
    BEGIN
        PRINT '  ⚠️  Menu is NOT assigned to Education Department';
        PRINT '     Go to Menu Configuration (/menu-config) and assign the menu';
    END
END
ELSE
BEGIN
    PRINT '  ⚠️  Cannot verify menu assignment (Menu or Department not found)';
END
PRINT '';

PRINT '========================================';
PRINT 'SETUP COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'If Screen Workflow Entry exists, the screen should load.';
PRINT 'If it still does not load, check browser console for JavaScript errors.';
PRINT '';
GO
