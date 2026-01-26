-- ============================================
-- UPDATE SCREEN CODE: EDUCATION_TABLE_6_1_INSTITUTIONS to ED_TABLE_6_1_INSTITUTIONS
-- Also update TableName references
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'UPDATING SCREEN CODE AND TABLE NAME';
PRINT '========================================';
PRINT '';

-- Update Mst_ScreenRegistry
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'EDUCATION_TABLE_6_1_INSTITUTIONS')
BEGIN
    UPDATE [dbo].[Mst_ScreenRegistry]
    SET 
        [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS',
        [TableName] = 'Ed_Table_6_1_Institutions',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenCode] = 'EDUCATION_TABLE_6_1_INSTITUTIONS';
    
    PRINT '✅ Updated Mst_ScreenRegistry:';
    PRINT '   ScreenCode: EDUCATION_TABLE_6_1_INSTITUTIONS → ED_TABLE_6_1_INSTITUTIONS';
    PRINT '   TableName: EDUCATION_TABLE_6_1_INSTITUTIONS → Ed_Table_6_1_Institutions';
END
ELSE
BEGIN
    PRINT '⚠️  No Mst_ScreenRegistry entry found with ScreenCode = EDUCATION_TABLE_6_1_INSTITUTIONS';
    PRINT '   (This is normal if the screen registry entry hasn''t been created yet)';
    PRINT '   You can create it using: backend/database/education-table-6-1-screen-registry.sql';
END
GO

-- Update Workflow_Screen
IF EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'EDUCATION_TABLE_6_1_INSTITUTIONS')
BEGIN
    UPDATE [dbo].[Workflow_Screen]
    SET 
        [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS',
        [TableName] = 'Ed_Table_6_1_Institutions',
        [UpdatedAt] = GETUTCDATE()
    WHERE [ScreenCode] = 'EDUCATION_TABLE_6_1_INSTITUTIONS';
    
    PRINT '✅ Updated Workflow_Screen:';
    PRINT '   ScreenCode: EDUCATION_TABLE_6_1_INSTITUTIONS → ED_TABLE_6_1_INSTITUTIONS';
    PRINT '   TableName: EDUCATION_TABLE_6_1_INSTITUTIONS → Ed_Table_6_1_Institutions';
END
ELSE
BEGIN
    PRINT '⚠️  No Workflow_Screen entry found with ScreenCode = EDUCATION_TABLE_6_1_INSTITUTIONS';
    PRINT '   (This is normal if the screen workflow entry hasn''t been created yet)';
    PRINT '   You can create it using: backend/database/education-table-6-1-screen-workflow.sql';
END
GO

-- Update Workflow_AuditHistory (if any records exist)
-- Note: Workflow_AuditHistory uses TargetTableName, not TableName
IF EXISTS (SELECT * FROM [dbo].[Workflow_AuditHistory] WHERE [TargetTableName] = 'EDUCATION_TABLE_6_1_INSTITUTIONS')
BEGIN
    UPDATE [dbo].[Workflow_AuditHistory]
    SET 
        [TargetTableName] = 'Ed_Table_6_1_Institutions'
    WHERE [TargetTableName] = 'EDUCATION_TABLE_6_1_INSTITUTIONS';
    
    DECLARE @AuditCount INT = @@ROWCOUNT;
    PRINT '✅ Updated Workflow_AuditHistory:';
    PRINT '   TargetTableName: EDUCATION_TABLE_6_1_INSTITUTIONS → Ed_Table_6_1_Institutions';
    PRINT '   Records updated: ' + CAST(@AuditCount AS NVARCHAR(10));
END
ELSE
BEGIN
    PRINT '⚠️  No Workflow_AuditHistory records found with TargetTableName = EDUCATION_TABLE_6_1_INSTITUTIONS';
    PRINT '   (This is normal if no workflow actions have been performed yet)';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'SCREEN CODE UPDATE COMPLETE';
PRINT '========================================';
PRINT '';
GO
