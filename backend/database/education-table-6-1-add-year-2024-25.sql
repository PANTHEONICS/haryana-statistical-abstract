-- ============================================
-- EDUCATION DEPARTMENT - TABLE 6.1
-- Add Year_2024_25 column to existing table
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Add Year_2024_25 column if it doesn't exist
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') 
    AND name = 'Year_2024_25'
)
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD [Year_2024_25] INT NULL;
    
    PRINT 'Column Year_2024_25 added successfully to Ed_Table_6_1_Institutions table.';
END
ELSE
BEGIN
    PRINT 'Column Year_2024_25 already exists in Ed_Table_6_1_Institutions table.';
END
GO
