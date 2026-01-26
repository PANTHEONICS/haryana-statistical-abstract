-- ============================================
-- RENAME TABLE: Screen_Workflow to Workflow_Screen
-- Updates table name and all related objects
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'RENAMING TABLE: Screen_Workflow to Workflow_Screen';
PRINT '========================================';
PRINT '';

-- Check if the table exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Screen_Workflow]') AND type in (N'U'))
BEGIN
    -- Check if the new table name already exists
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]') AND type in (N'U'))
    BEGIN
        PRINT '⚠️  Table Workflow_Screen already exists. Cannot rename.';
        PRINT '   Please check if the table was already renamed.';
    END
    ELSE
    BEGIN
        -- Rename the table
        EXEC sp_rename '[dbo].[Screen_Workflow]', 'Workflow_Screen';
        PRINT '✅ Renamed table: Screen_Workflow → Workflow_Screen';
        
        -- Rename indexes if they exist
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screen_Workflow_ScreenCode' AND object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]'))
        BEGIN
            EXEC sp_rename '[dbo].[Workflow_Screen].[IX_Screen_Workflow_ScreenCode]', 'IX_Workflow_Screen_ScreenCode', 'INDEX';
            PRINT '✅ Renamed index: IX_Screen_Workflow_ScreenCode → IX_Workflow_Screen_ScreenCode';
        END
        
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screen_Workflow_TableName' AND object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]'))
        BEGIN
            EXEC sp_rename '[dbo].[Workflow_Screen].[IX_Screen_Workflow_TableName]', 'IX_Workflow_Screen_TableName', 'INDEX';
            PRINT '✅ Renamed index: IX_Screen_Workflow_TableName → IX_Workflow_Screen_TableName';
        END
        
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Screen_Workflow_CurrentStatusID' AND object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]'))
        BEGIN
            EXEC sp_rename '[dbo].[Workflow_Screen].[IX_Screen_Workflow_CurrentStatusID]', 'IX_Workflow_Screen_CurrentStatusID', 'INDEX';
            PRINT '✅ Renamed index: IX_Screen_Workflow_CurrentStatusID → IX_Workflow_Screen_CurrentStatusID';
        END
        
        -- Rename foreign key constraints if they exist
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Screen_Workflow_Status' AND parent_object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]'))
        BEGIN
            EXEC sp_rename '[dbo].[FK_Screen_Workflow_Status]', 'FK_Workflow_Screen_Status', 'OBJECT';
            PRINT '✅ Renamed foreign key: FK_Screen_Workflow_Status → FK_Workflow_Screen_Status';
        END
        
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Screen_Workflow_CreatedBy' AND parent_object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]'))
        BEGIN
            EXEC sp_rename '[dbo].[FK_Screen_Workflow_CreatedBy]', 'FK_Workflow_Screen_CreatedBy', 'OBJECT';
            PRINT '✅ Renamed foreign key: FK_Screen_Workflow_CreatedBy → FK_Workflow_Screen_CreatedBy';
        END
    END
END
ELSE IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]') AND type in (N'U'))
BEGIN
    PRINT '✅ Table Workflow_Screen already exists (may have been renamed previously).';
END
ELSE
BEGIN
    PRINT '⚠️  Table Screen_Workflow does not exist.';
    PRINT '   Please ensure the table exists before running this script.';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'TABLE RENAME COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update C# model to use [Table("Workflow_Screen")]';
PRINT '2. Update all SQL scripts that reference Screen_Workflow';
PRINT '3. Restart the application';
GO
