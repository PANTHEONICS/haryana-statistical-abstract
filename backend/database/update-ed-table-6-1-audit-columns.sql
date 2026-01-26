-- ============================================
-- UPDATE Ed_Table_6_1_Institutions TABLE
-- Remove SourceDocument, SourceTable, SourceReference
-- Rename CreatedAt to CreatedDateTime
-- Rename UpdatedAt to ModifiedDateTime
-- Add audit columns: CreatedBy, CreatedByIPAddress, ModifiedBy, ModifiedByIPAddress
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'UPDATING Ed_Table_6_1_Institutions';
PRINT '========================================';
PRINT '';

-- Step 1: Drop default constraints and source columns if they exist
-- Drop SourceDocument column and its default constraint
DECLARE @SourceDocumentConstraint NVARCHAR(128);
SELECT @SourceDocumentConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]'), 'SourceDocument', 'ColumnId');

IF @SourceDocumentConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP CONSTRAINT [' + @SourceDocumentConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceDocumentConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'SourceDocument')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP COLUMN [SourceDocument];
    PRINT '✅ Dropped column: SourceDocument';
END
ELSE
BEGIN
    PRINT '⚠️  Column SourceDocument does not exist';
END
GO

-- Drop SourceTable column and its default constraint
DECLARE @SourceTableConstraint NVARCHAR(128);
SELECT @SourceTableConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]'), 'SourceTable', 'ColumnId');

IF @SourceTableConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP CONSTRAINT [' + @SourceTableConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceTableConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'SourceTable')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP COLUMN [SourceTable];
    PRINT '✅ Dropped column: SourceTable';
END
ELSE
BEGIN
    PRINT '⚠️  Column SourceTable does not exist';
END
GO

-- Drop SourceReference column and its default constraint
DECLARE @SourceReferenceConstraint NVARCHAR(128);
SELECT @SourceReferenceConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]'), 'SourceReference', 'ColumnId');

IF @SourceReferenceConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP CONSTRAINT [' + @SourceReferenceConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceReferenceConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'SourceReference')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions] DROP COLUMN [SourceReference];
    PRINT '✅ Dropped column: SourceReference';
END
ELSE
BEGIN
    PRINT '⚠️  Column SourceReference does not exist';
END
GO

-- Step 2: Rename CreatedAt to CreatedDateTime
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'CreatedAt')
BEGIN
    EXEC sp_rename '[dbo].[Ed_Table_6_1_Institutions].CreatedAt', 'CreatedDateTime', 'COLUMN';
    PRINT '✅ Renamed column: CreatedAt → CreatedDateTime';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'CreatedDateTime')
BEGIN
    PRINT '⚠️  Column CreatedDateTime already exists';
END
ELSE
BEGIN
    PRINT '⚠️  Column CreatedAt does not exist';
END
GO

-- Step 3: Rename UpdatedAt to ModifiedDateTime
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'UpdatedAt')
BEGIN
    EXEC sp_rename '[dbo].[Ed_Table_6_1_Institutions].UpdatedAt', 'ModifiedDateTime', 'COLUMN';
    PRINT '✅ Renamed column: UpdatedAt → ModifiedDateTime';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'ModifiedDateTime')
BEGIN
    PRINT '⚠️  Column ModifiedDateTime already exists';
END
ELSE
BEGIN
    PRINT '⚠️  Column UpdatedAt does not exist';
END
GO

-- Step 4: Add new audit columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD [CreatedBy] INT NULL;
    PRINT '✅ Added column: CreatedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Column CreatedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'CreatedByIPAddress')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD [CreatedByIPAddress] NVARCHAR(50) NULL;
    PRINT '✅ Added column: CreatedByIPAddress';
END
ELSE
BEGIN
    PRINT '⚠️  Column CreatedByIPAddress already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'ModifiedBy')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD [ModifiedBy] INT NULL;
    PRINT '✅ Added column: ModifiedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Column ModifiedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Ed_Table_6_1_Institutions]') AND name = 'ModifiedByIPAddress')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD [ModifiedByIPAddress] NVARCHAR(50) NULL;
    PRINT '✅ Added column: ModifiedByIPAddress';
END
ELSE
BEGIN
    PRINT '⚠️  Column ModifiedByIPAddress already exists';
END
GO

-- Step 5: Add foreign key constraints (optional - for referential integrity)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Ed_Table_6_1_Institutions_CreatedBy')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD CONSTRAINT [FK_Ed_Table_6_1_Institutions_CreatedBy]
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    PRINT '✅ Added foreign key: FK_Ed_Table_6_1_Institutions_CreatedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Foreign key FK_Ed_Table_6_1_Institutions_CreatedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Ed_Table_6_1_Institutions_ModifiedBy')
BEGIN
    ALTER TABLE [dbo].[Ed_Table_6_1_Institutions]
    ADD CONSTRAINT [FK_Ed_Table_6_1_Institutions_ModifiedBy]
    FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    PRINT '✅ Added foreign key: FK_Ed_Table_6_1_Institutions_ModifiedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Foreign key FK_Ed_Table_6_1_Institutions_ModifiedBy already exists';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Ed_Table_6_1_Institutions UPDATE COMPLETE';
PRINT '========================================';
PRINT '';
GO
