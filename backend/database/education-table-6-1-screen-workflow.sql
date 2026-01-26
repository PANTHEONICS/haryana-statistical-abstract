-- ============================================
-- EDUCATION DEPARTMENT - TABLE 6.1
-- Screen Workflow Entry
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Insert Screen Workflow Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    
    -- Get Education Department ID (try both 'EDU' and 'Education' codes)
    DECLARE @EducationDeptID INT = NULL;
    SET @EducationDeptID = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
    IF @EducationDeptID IS NULL
    BEGIN
        SET @EducationDeptID = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'Education');
    END
    IF @EducationDeptID IS NULL
    BEGIN
        SET @EducationDeptID = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentName] LIKE '%Education%');
    END
    
    -- Get Admin User ID
    DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
    
    -- Get Maker Entry Status ID (StatusCode is 'Draft', not 'STATUS_MAKER_ENTRY')
    DECLARE @MakerEntryStatusID INT = (SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft');
    
    -- If StatusID is still NULL, try to get StatusID = 1 (which should be Maker Entry)
    IF @MakerEntryStatusID IS NULL
    BEGIN
        SET @MakerEntryStatusID = (SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusID] = 1);
    END

    -- Debug output
    PRINT '========================================';
    PRINT 'Screen Workflow Entry Creation';
    PRINT '========================================';
    PRINT 'Education Department ID: ' + ISNULL(CAST(@EducationDeptID AS NVARCHAR(10)), 'NULL');
    PRINT 'Admin User ID: ' + ISNULL(CAST(@AdminUserID AS NVARCHAR(10)), 'NULL');
    PRINT 'Maker Entry Status ID: ' + ISNULL(CAST(@MakerEntryStatusID AS NVARCHAR(10)), 'NULL');
    PRINT '';

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
        PRINT '✅ Screen Workflow entry created successfully for ED_TABLE_6_1_INSTITUTIONS.';
        PRINT '';
        PRINT 'Verification:';
        SELECT 
            [ScreenWorkflowID],
            [ScreenCode],
            [ScreenName],
            [TableName],
            [CurrentStatusID],
            [CreatedByUserID],
            [IsActive]
        FROM [dbo].[Workflow_Screen]
        WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS';
    END
    ELSE
    BEGIN
        PRINT '❌ ERROR: Required dependencies not found.';
        PRINT '';
        PRINT 'Missing dependencies:';
        IF @AdminUserID IS NULL
        BEGIN
            PRINT '  ❌ Admin User (LoginID: admin) - NOT FOUND';
            PRINT '     ⚠️  Make sure admin user exists in Master_User table';
        END
        ELSE
        BEGIN
            PRINT '  ✅ Admin User (LoginID: admin) - FOUND (UserID: ' + CAST(@AdminUserID AS NVARCHAR(10)) + ')';
        END
        
        IF @MakerEntryStatusID IS NULL
        BEGIN
            PRINT '  ❌ Maker Entry Status (StatusCode: Draft) - NOT FOUND';
            PRINT '     ⚠️  Run: backend/database/workflow-engine-seed-data.sql';
            PRINT '     ⚠️  Or check if StatusID = 1 exists in Mst_WorkflowStatus';
        END
        ELSE
        BEGIN
            PRINT '  ✅ Maker Entry Status - FOUND (StatusID: ' + CAST(@MakerEntryStatusID AS NVARCHAR(10)) + ')';
        END
        
        IF @EducationDeptID IS NULL
        BEGIN
            PRINT '  ⚠️  Education Department - NOT FOUND (not critical for workflow entry)';
        END
        ELSE
        BEGIN
            PRINT '  ✅ Education Department - FOUND (DepartmentID: ' + CAST(@EducationDeptID AS NVARCHAR(10)) + ')';
        END
    END
END
ELSE
BEGIN
    PRINT 'Screen Workflow entry already exists for ED_TABLE_6_1_INSTITUTIONS.';
    PRINT '';
    PRINT 'Existing entry:';
    SELECT 
        [ScreenWorkflowID],
        [ScreenCode],
        [ScreenName],
        [TableName],
        [CurrentStatusID],
        [CreatedByUserID],
        [IsActive]
    FROM [dbo].[Workflow_Screen]
    WHERE [ScreenCode] = 'EDUCATION_TABLE_6_1_INSTITUTIONS';
END
GO
