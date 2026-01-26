-- ============================================
-- RENAME TABLE: EDUCATION_TABLE_6_1_INSTITUTIONS to Ed_Table_6_1_Institutions
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'RENAMING TABLE: EDUCATION_TABLE_6_1_INSTITUTIONS';
PRINT '========================================';
PRINT '';

-- Check if old table exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EDUCATION_TABLE_6_1_INSTITUTIONS]') AND type in (N'U'))
BEGIN
    -- Check if new table already exists
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND type in (N'U'))
    BEGIN
        PRINT '⚠️  Table Ed_Table_6_1_Institutions already exists.';
        PRINT '   Skipping rename operation.';
    END
    ELSE
    BEGIN
        -- Rename the table
        EXEC sp_rename '[dbo].[EDUCATION_TABLE_6_1_INSTITUTIONS]', 'Ed_Table_6_1_Institutions';
        PRINT '✅ Table renamed successfully:';
        PRINT '   OLD: EDUCATION_TABLE_6_1_INSTITUTIONS';
        PRINT '   NEW: Ed_Table_6_1_Institutions';
        
        -- Rename indexes if they exist
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EDUCATION_TABLE_6_1_INSTITUTIONS_InstitutionType' AND object_id = OBJECT_ID('Ed_Table_6_1_Institutions'))
        BEGIN
            EXEC sp_rename 'Ed_Table_6_1_Institutions.IX_EDUCATION_TABLE_6_1_INSTITUTIONS_InstitutionType', 'IX_Ed_Table_6_1_Institutions_InstitutionType', 'INDEX';
            PRINT '✅ Index renamed: IX_Ed_Table_6_1_Institutions_InstitutionType';
        END
        
        IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EDUCATION_TABLE_6_1_INSTITUTIONS_CreatedAt' AND object_id = OBJECT_ID('Ed_Table_6_1_Institutions'))
        BEGIN
            EXEC sp_rename 'Ed_Table_6_1_Institutions.IX_EDUCATION_TABLE_6_1_INSTITUTIONS_CreatedAt', 'IX_Ed_Table_6_1_Institutions_CreatedAt', 'INDEX';
            PRINT '✅ Index renamed: IX_Ed_Table_6_1_Institutions_CreatedAt';
        END
    END
END
ELSE
BEGIN
    PRINT '⚠️  Table EDUCATION_TABLE_6_1_INSTITUTIONS does not exist.';
    PRINT '   It may have already been renamed or does not exist.';
END

PRINT '';
PRINT '========================================';
PRINT 'TABLE RENAME COMPLETE';
PRINT '========================================';
PRINT '';
GO
