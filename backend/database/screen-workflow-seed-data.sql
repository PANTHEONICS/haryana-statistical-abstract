-- ============================================
-- Screen-Level Workflow Seed Data
-- ============================================
-- Insert initial screen workflow records for each screen/module

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Insert Screen Workflow for Census Population Management
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'CENSUS_POPULATION')
BEGIN
    DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
    
    IF @AdminUserID IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Workflow_Screen] (
            [ScreenName],
            [ScreenCode],
            [TableName],
            [CurrentStatusID],
            [CurrentStatusName],
            [CreatedByUserID],
            [CreatedByUserName],
            [CreatedAt],
            [IsActive]
        )
        VALUES (
            'Census Population Management',
            'CENSUS_POPULATION',
            'census_population',
            1, -- Draft status
            'Draft',
            @AdminUserID,
            'Admin',
            GETUTCDATE(),
            1
        );
        PRINT 'Screen workflow for Census Population Management created.';
    END
    ELSE
    BEGIN
        PRINT 'Warning: Admin user not found. Cannot create screen workflow.';
    END
END
ELSE
BEGIN
    PRINT 'Screen workflow for Census Population Management already exists.';
END
GO

-- ============================================
-- Note: Add more screen workflow records here as you create new screens
-- Example:
-- IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'SCREEN_CODE')
-- BEGIN
--     INSERT INTO [dbo].[Workflow_Screen] (...)
--     VALUES (...)
-- END
-- ============================================

PRINT '';
PRINT '===========================================';
PRINT 'Screen-Level Workflow seed data inserted successfully!';
PRINT '===========================================';
GO
