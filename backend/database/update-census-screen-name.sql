-- ============================================
-- Update Screen Names from "Census Population Management" to "AREA AND POPULATION"
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Update Workflow_Screen table
IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenName] = 'Census Population Management')
BEGIN
    UPDATE [dbo].[Workflow_Screen]
    SET [ScreenName] = 'AREA AND POPULATION',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenName] = 'Census Population Management';
    
    PRINT 'Updated Workflow_Screen: ScreenName changed to "AREA AND POPULATION".';
END
ELSE IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
BEGIN
    UPDATE [dbo].[Workflow_Screen]
    SET [ScreenName] = 'AREA AND POPULATION',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
    
    PRINT 'Updated Workflow_Screen: ScreenName changed to "AREA AND POPULATION" for AP_TABLE_3_2_CENSUS_POPULATION.';
END
ELSE
BEGIN
    PRINT 'Workflow_Screen entry not found.';
END
GO

-- Update Mst_ScreenRegistry table
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenName] = 'Census Population Management')
BEGIN
    UPDATE [dbo].[Mst_ScreenRegistry]
    SET [ScreenName] = 'AREA AND POPULATION',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenName] = 'Census Population Management';
    
    PRINT 'Updated Mst_ScreenRegistry: ScreenName changed to "AREA AND POPULATION".';
END
ELSE IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
BEGIN
    UPDATE [dbo].[Mst_ScreenRegistry]
    SET [ScreenName] = 'AREA AND POPULATION',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
    
    PRINT 'Updated Mst_ScreenRegistry: ScreenName changed to "AREA AND POPULATION" for AP_TABLE_3_2_CENSUS_POPULATION.';
END
ELSE
BEGIN
    PRINT 'Mst_ScreenRegistry entry not found.';
END
GO

-- Verify the updates
PRINT '';
PRINT '===========================================';
PRINT 'Verification:';
PRINT '===========================================';
PRINT '';

-- Check Workflow_Screen
IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
BEGIN
    SELECT [ScreenWorkflowID], [ScreenName], [ScreenCode], [TableName]
    FROM [dbo].[Workflow_Screen]
    WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
END

-- Check Mst_ScreenRegistry
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
BEGIN
    SELECT [ScreenRegistryID], [ScreenName], [ScreenCode], [TableName]
    FROM [dbo].[Mst_ScreenRegistry]
    WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION';
END

PRINT '';
PRINT '===========================================';
PRINT 'Screen Name Update Complete!';
PRINT '===========================================';
PRINT '';
PRINT 'Updated Screen Name:';
PRINT '  "Census Population Management" → "AREA AND POPULATION"';
PRINT '';
GO
