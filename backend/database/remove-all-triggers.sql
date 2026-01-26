-- ============================================
-- REMOVE ALL TRIGGERS FROM DATABASE
-- Removes all triggers that auto-update datetime fields
-- Includes both active and legacy triggers
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'REMOVING ALL TRIGGERS FROM DATABASE';
PRINT '========================================';
PRINT '';

-- ============================================
-- Remove triggers from AP_Table_3_2_CensusPopulation
-- ============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AP_Table_3_2_CensusPopulation_UpdatedAt')
BEGIN
    DROP TRIGGER [dbo].[TR_AP_Table_3_2_CensusPopulation_UpdatedAt];
    PRINT '✅ Dropped trigger: TR_AP_Table_3_2_CensusPopulation_UpdatedAt';
END
ELSE
BEGIN
    PRINT '⚠️  Trigger TR_AP_Table_3_2_CensusPopulation_UpdatedAt does not exist';
END
GO

-- ============================================
-- Remove old trigger from census_population (if it exists)
-- ============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_census_population_UpdatedAt')
BEGIN
    DROP TRIGGER [dbo].[TR_census_population_UpdatedAt];
    PRINT '✅ Dropped legacy trigger: TR_census_population_UpdatedAt';
END
GO

-- ============================================
-- Remove triggers from Ed_Table_6_1_Institutions
-- ============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Ed_Table_6_1_Institutions_UpdatedAt')
BEGIN
    DROP TRIGGER [dbo].[TR_Ed_Table_6_1_Institutions_UpdatedAt];
    PRINT '✅ Dropped trigger: TR_Ed_Table_6_1_Institutions_UpdatedAt';
END
ELSE
BEGIN
    PRINT '⚠️  Trigger TR_Ed_Table_6_1_Institutions_UpdatedAt does not exist';
END
GO

-- ============================================
-- Remove legacy triggers from Roles table (if it exists)
-- ============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Roles_UpdatedAt')
BEGIN
    DROP TRIGGER [dbo].[TR_Roles_UpdatedAt];
    PRINT '✅ Dropped legacy trigger: TR_Roles_UpdatedAt';
END
GO

-- ============================================
-- Remove legacy triggers from Users table (if it exists)
-- ============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Users_UpdatedAt')
BEGIN
    DROP TRIGGER [dbo].[TR_Users_UpdatedAt];
    PRINT '✅ Dropped legacy trigger: TR_Users_UpdatedAt';
END
GO

-- ============================================
-- List any remaining triggers (for verification)
-- ============================================
PRINT '';
PRINT '========================================';
PRINT 'REMAINING TRIGGERS (if any)';
PRINT '========================================';
PRINT '';

DECLARE @TriggerCount INT;
SELECT @TriggerCount = COUNT(*)
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
WHERE o.type = 'U'; -- User tables only

IF @TriggerCount > 0
BEGIN
    PRINT '⚠️  WARNING: ' + CAST(@TriggerCount AS VARCHAR(10)) + ' trigger(s) still exist:';
    PRINT '';
    SELECT 
        t.name AS TriggerName,
        OBJECT_NAME(t.parent_id) AS TableName
    FROM sys.triggers t
    INNER JOIN sys.objects o ON t.parent_id = o.object_id
    WHERE o.type = 'U'
    ORDER BY TableName, TriggerName;
END
ELSE
BEGIN
    PRINT '✅ No triggers found on user tables';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'ALL TRIGGERS REMOVAL COMPLETE';
PRINT '========================================';
GO
