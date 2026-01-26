-- ============================================
-- EDUCATION DEPARTMENT - TABLE 6.1
-- Number of recognised universities/colleges/schools in Haryana
-- Table Name: Ed_Table_6_1_Institutions
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Ed_Table_6_1_Institutions
-- Stores data for Table 6.1: Number of recognised universities/colleges/schools in Haryana
-- Department: Education
-- Source: Statistical Abstract of Haryana 2023-24
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Ed_Table_6_1_Institutions] (
        [InstitutionID] INT IDENTITY(1,1) PRIMARY KEY,
        [InstitutionType] NVARCHAR(200) NOT NULL UNIQUE,
        [Year_1966_67] INT NULL,
        [Year_1970_71] INT NULL,
        [Year_1980_81] INT NULL,
        [Year_1990_91] INT NULL,
        [Year_2000_01] INT NULL,
        [Year_2010_11] INT NULL,
        [Year_2016_17] INT NULL,
        [Year_2017_18] INT NULL,
        [Year_2018_19] INT NULL,
        [Year_2019_20] INT NULL,
        [Year_2020_21] INT NULL,
        [Year_2021_22] INT NULL,
        [Year_2022_23] INT NULL,
        [Year_2023_24] INT NULL,
        [Year_2024_25] INT NULL,
        [SourceDocument] NVARCHAR(255) NULL DEFAULT 'Statistical Abstract of Haryana 2023-24',
        [SourceTable] NVARCHAR(50) NULL DEFAULT 'Table 6.1',
        [SourceReference] NVARCHAR(500) NULL DEFAULT 'Departments of Technical/Higher/Secondary/Elementary Education, Haryana',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE UNIQUE INDEX [IX_Ed_Table_6_1_Institutions_InstitutionType] ON [dbo].[Ed_Table_6_1_Institutions]([InstitutionType]);
    CREATE INDEX [IX_Ed_Table_6_1_Institutions_CreatedAt] ON [dbo].[Ed_Table_6_1_Institutions]([CreatedAt]);
    
    PRINT 'Table Ed_Table_6_1_Institutions created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Ed_Table_6_1_Institutions already exists.';
END
GO
