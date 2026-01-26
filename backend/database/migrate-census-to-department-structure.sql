-- ============================================
-- Migrate Census Screen to Department Structure
-- Rename census_population table to AP_Table_3_2_CensusPopulation
-- Update Workflow_Screen with new screen code
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Step 1: Rename census_population table to AP_Table_3_2_CensusPopulation
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'census_population' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    -- Check if new table already exists
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AP_Table_3_2_CensusPopulation' AND schema_id = SCHEMA_ID('dbo'))
    BEGIN
        -- Rename the table
        EXEC sp_rename 'dbo.census_population', 'AP_Table_3_2_CensusPopulation';
        PRINT 'Table renamed from census_population to AP_Table_3_2_CensusPopulation.';
    END
    ELSE
    BEGIN
        PRINT 'Table AP_Table_3_2_CensusPopulation already exists. Skipping rename.';
        PRINT 'Note: If you want to re-run this migration, drop AP_Table_3_2_CensusPopulation first.';
    END
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AP_Table_3_2_CensusPopulation' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Table AP_Table_3_2_CensusPopulation already exists. Migration may have been run previously.';
END
ELSE
BEGIN
    PRINT 'WARNING: Neither census_population nor AP_Table_3_2_CensusPopulation table exists.';
    PRINT 'Please ensure the table exists before running this migration.';
END
GO

-- ============================================
-- Step 2: Update Workflow_Screen table with new screen code
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_Screen' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    -- Update ScreenCode from 'CENSUS_POPULATION' to 'AP_TABLE_3_2_CENSUS_POPULATION'
    IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'CENSUS_POPULATION')
    BEGIN
        UPDATE [dbo].[Workflow_Screen]
        SET [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION',
            [ScreenName] = 'Census Population Management',
            [TableName] = 'AP_Table_3_2_CensusPopulation',
            [UpdatedAt] = GETUTCDATE()
        WHERE [ScreenCode] = 'CENSUS_POPULATION';
        
        PRINT 'Updated Workflow_Screen: ScreenCode changed to AP_TABLE_3_2_CENSUS_POPULATION.';
    END
    ELSE IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
    BEGIN
        -- Update TableName if it's still pointing to old table name
        IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION' AND [TableName] = 'census_population')
        BEGIN
            UPDATE [dbo].[Workflow_Screen]
            SET [TableName] = 'AP_Table_3_2_CensusPopulation',
                [UpdatedAt] = GETUTCDATE()
            WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
            
            PRINT 'Updated Workflow_Screen: TableName changed to AP_Table_3_2_CensusPopulation.';
        END
        ELSE
        BEGIN
            PRINT 'Workflow_Screen already has the new screen code AP_TABLE_3_2_CENSUS_POPULATION.';
        END
    END
    ELSE
    BEGIN
        -- Create new Workflow_Screen entry if it doesn't exist
        DECLARE @APDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'AP');
        DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
        
        IF @APDeptID IS NOT NULL AND @AdminUserID IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[Workflow_Screen] (
                [ScreenCode],
                [ScreenName],
                [TableName],
                [CurrentStatusID],
                [CreatedByUserID]
            )
            VALUES (
                'AP_TABLE_3_2_CENSUS_POPULATION',
                'Census Population Management',
                'AP_Table_3_2_CensusPopulation',
                1, -- Maker Entry status
                @AdminUserID
            );
            
            PRINT 'Created new Workflow_Screen entry for AP_TABLE_3_2_CENSUS_POPULATION.';
        END
        ELSE
        BEGIN
            PRINT 'WARNING: Could not create Workflow_Screen entry. Department or Admin user not found.';
        END
    END
END
ELSE
BEGIN
    PRINT 'Workflow_Screen table does not exist. Please run screen-workflow-schema.sql first.';
END
GO

-- ============================================
-- Step 3: Update Mst_ScreenRegistry with new table name
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_ScreenRegistry' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
    BEGIN
        UPDATE [dbo].[Mst_ScreenRegistry]
        SET [TableName] = 'AP_Table_3_2_CensusPopulation',
            [UpdatedAt] = GETUTCDATE()
        WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
        
        PRINT 'Updated Mst_ScreenRegistry: TableName changed to AP_Table_3_2_CensusPopulation.';
    END
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Census Screen Migration Complete!';
PRINT '===========================================';
PRINT 'Next Steps:';
PRINT '1. Update backend code to use new table name and namespace';
PRINT '2. Update frontend code to use new file paths';
PRINT '3. Update API routes to /api/v1/AP/Table3_2/CensusPopulation';
PRINT '4. Test the Census screen';
PRINT '===========================================';
GO
