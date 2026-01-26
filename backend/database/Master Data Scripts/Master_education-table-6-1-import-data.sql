-- ============================================
-- EDUCATION TABLE 6.1 - IMPORT DATA FROM PDF
-- Source: Resources/Education_6_1.pdf
-- Statistical Abstract of Haryana 2023-24
-- Table Name: Ed_Table_6_1_Institutions
-- ============================================
-- 
-- USAGE INSTRUCTIONS:
-- ============================================
-- To TRUNCATE and recreate all data:
--   1. Uncomment the TRUNCATE TABLE line below (recommended - faster)
--   2. Run this script
--   3. All 12 institution types will be imported
--
-- To DELETE and recreate (resets identity seed):
--   1. Uncomment the DELETE and DBCC CHECKIDENT lines below
--   2. Run this script
--   3. All 12 institution types will be imported
--
-- To add only missing records (default behavior):
--   1. Keep TRUNCATE/DELETE commented out
--   2. Script will skip existing records (based on InstitutionType)
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'EDUCATION TABLE 6.1 - DATA IMPORT';
PRINT 'Table: Ed_Table_6_1_Institutions';
PRINT '========================================';
PRINT '';

-- ============================================
-- OPTION 1: TRUNCATE TABLE (Recommended - Faster, but resets identity)
-- Uncomment the line below to clear all data before importing
-- ============================================
-- TRUNCATE TABLE [dbo].[Ed_Table_6_1_Institutions];
-- PRINT '✅ Cleared all existing data using TRUNCATE.';
-- PRINT '';

-- ============================================
-- OPTION 2: DELETE with Identity Reset
-- Uncomment the lines below to delete all data and reset identity seed
-- ============================================
-- DELETE FROM [dbo].[Ed_Table_6_1_Institutions];
-- DBCC CHECKIDENT ('[dbo].[Ed_Table_6_1_Institutions]', RESEED, 0);
-- PRINT '✅ Cleared all existing data using DELETE.';
-- PRINT '✅ Reset identity seed to 0.';
-- PRINT '';

-- Insert data from PDF
PRINT 'Inserting data from Education_6_1.pdf...';
PRINT '';

-- Helper function to convert dash/empty to NULL
-- Note: SQL Server doesn't support user-defined functions in this context, so we'll use CASE statements

-- 1. Universities (Including State / Private/ Deemed / Central / Technical/Others)
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Universities (Including State / Private/ Deemed / Central / Technical/Others)')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Universities (Including State / Private/ Deemed / Central / Technical/Others)',
        1, 1, 3, 3, 4, 24,
        43, 46, 55, 56, 56, 57,
        57, 58, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Universities';
END
ELSE
BEGIN
    PRINT '  ⚠️  Universities already exists';
END
GO

-- 2. Arts and Science Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Arts and Science Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Arts and Science Colleges',
        40, 65, 98, 120, 150, 190,
        274, 297, 342, 359, 357, 364,
        373, 374, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Arts and Science Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  Arts and Science Colleges already exists';
END
GO

-- 3. Engineering Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Engineering Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Engineering Colleges',
        1, 1, 1, 2, 25, 155,
        175, 127, 109, 101, 90, 91,
        93, 94, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Engineering Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  Engineering Colleges already exists';
END
GO

-- 4. Polytechnics
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Polytechnics')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Polytechnics',
        NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, 181, 185, 192, 189, 191,
        191, 200, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Polytechnics';
END
ELSE
BEGIN
    PRINT '  ⚠️  Polytechnics already exists';
END
GO

-- 5. MBA Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'MBA Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'MBA Colleges',
        NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, 124, 99, 89, 83, 99,
        115, 119, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: MBA Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  MBA Colleges already exists';
END
GO

-- 6. MCA Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'MCA Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'MCA Colleges',
        NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, 44, 34, 33, 32, 31,
        32, 46, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: MCA Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  MCA Colleges already exists';
END
GO

-- 7. B.Pharmacy Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'B.Pharmacy Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'B.Pharmacy Colleges',
        NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, 32, 43, 52, 66, 67,
        72, 82, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: B.Pharmacy Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  B.Pharmacy Colleges already exists';
END
GO

-- 8. B.Arch Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'B.Arch Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'B.Arch Colleges',
        NULL, NULL, NULL, NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, 7,
        7, 6, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: B.Arch Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  B.Arch Colleges already exists';
END
GO

-- 9. Teachers Training Colleges
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Teachers Training Colleges')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Teachers Training Colleges',
        5, 12, 20, 18, 20, 472,
        491, 491, 491, 475, 491, 491,
        489, NULL, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Teachers Training Colleges';
END
ELSE
BEGIN
    PRINT '  ⚠️  Teachers Training Colleges already exists';
END
GO

-- 10. High/Senior Secondary
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'High/Senior Secondary')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'High/Senior Secondary',
        597, 975, 1473, 2356, 4138, 6983,
        7782, 8024, 8308, 8575, 8782, 9023,
        9078, 9354, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: High/Senior Secondary';
END
ELSE
BEGIN
    PRINT '  ⚠️  High/Senior Secondary already exists';
END
GO

-- 11. Middle Schools
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Middle Schools')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Middle Schools',
        735, 760, 881, 1399, 1887, 3483,
        4986, 5228, 5673, 5704, 5833, 5888,
        5916, 5640, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Middle Schools';
END
ELSE
BEGIN
    PRINT '  ⚠️  Middle Schools already exists';
END
GO

-- 12. Primary/Pre-Primary
IF NOT EXISTS (SELECT * FROM [dbo].[Ed_Table_6_1_Institutions] WHERE [InstitutionType] = 'Primary/Pre-Primary')
BEGIN
    INSERT INTO [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionType],
        [Year_1966_67], [Year_1970_71], [Year_1980_81], [Year_1990_91], [Year_2000_01], [Year_2010_11],
        [Year_2016_17], [Year_2017_18], [Year_2018_19], [Year_2019_20], [Year_2020_21], [Year_2021_22],
        [Year_2022_23], [Year_2023_24], [Year_2024_25],
        [SourceDocument], [SourceTable], [SourceReference],
        [CreatedAt], [UpdatedAt]
    )
    VALUES (
        'Primary/Pre-Primary',
        4447, 4204, 4934, 5109, 11013, 14004,
        9968, 9974, 9972, 9928, 9895, 9896,
        9882, 9872, NULL,
        'Statistical Abstract of Haryana 2023-24', 'Table 6.1', 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        GETUTCDATE(), GETUTCDATE()
    );
    PRINT '  ✅ Inserted: Primary/Pre-Primary';
END
ELSE
BEGIN
    PRINT '  ⚠️  Primary/Pre-Primary already exists';
END
GO

-- Verification
PRINT '';
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';
PRINT '';
PRINT 'Total records inserted:';
SELECT COUNT(*) AS TotalRecords FROM [dbo].[Ed_Table_6_1_Institutions];
PRINT '';
PRINT 'Institution Types:';
SELECT 
    [InstitutionID],
    [InstitutionType],
    [Year_1966_67],
    [Year_2023_24],
    [CreatedAt]
FROM [dbo].[Ed_Table_6_1_Institutions]
ORDER BY [InstitutionType];
PRINT '';
PRINT '========================================';
PRINT 'DATA IMPORT COMPLETE';
PRINT '========================================';
PRINT '';
PRINT 'All 12 institution types have been imported from Education_6_1.pdf';
PRINT 'Source: Statistical Abstract of Haryana 2023-24, Table 6.1';
PRINT '';
GO
