-- ============================================
-- UPDATE AP_Table_3_2_CensusPopulation TABLE
-- Remove SourceDocument, SourceTable, SourceReference
-- Rename created_at to CreatedDateTime
-- Rename updated_at to ModifiedDateTime
-- Add audit columns: CreatedBy, CreatedByIPAddress, ModifiedBy, ModifiedByIPAddress
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'UPDATING AP_Table_3_2_CensusPopulation';
PRINT '========================================';
PRINT '';

-- Step 1: Drop default constraints and source columns if they exist
-- Drop source_document column and its default constraint
DECLARE @SourceDocumentConstraint NVARCHAR(128);
SELECT @SourceDocumentConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]'), 'source_document', 'ColumnId');

IF @SourceDocumentConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP CONSTRAINT [' + @SourceDocumentConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceDocumentConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'source_document')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP COLUMN [source_document];
    PRINT '✅ Dropped column: source_document';
END
ELSE
BEGIN
    PRINT '⚠️  Column source_document does not exist';
END
GO

-- Drop source_table column and its default constraint
DECLARE @SourceTableConstraint NVARCHAR(128);
SELECT @SourceTableConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]'), 'source_table', 'ColumnId');

IF @SourceTableConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP CONSTRAINT [' + @SourceTableConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceTableConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'source_table')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP COLUMN [source_table];
    PRINT '✅ Dropped column: source_table';
END
ELSE
BEGIN
    PRINT '⚠️  Column source_table does not exist';
END
GO

-- Drop source_reference column and its default constraint
DECLARE @SourceReferenceConstraint NVARCHAR(128);
SELECT @SourceReferenceConstraint = name 
FROM sys.default_constraints 
WHERE parent_object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') 
  AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]'), 'source_reference', 'ColumnId');

IF @SourceReferenceConstraint IS NOT NULL
BEGIN
    EXEC('ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP CONSTRAINT [' + @SourceReferenceConstraint + ']');
    PRINT '✅ Dropped default constraint: ' + @SourceReferenceConstraint;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'source_reference')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation] DROP COLUMN [source_reference];
    PRINT '✅ Dropped column: source_reference';
END
ELSE
BEGIN
    PRINT '⚠️  Column source_reference does not exist';
END
GO

-- Step 2: Rename created_at to CreatedDateTime
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'created_at')
BEGIN
    EXEC sp_rename '[dbo].[AP_Table_3_2_CensusPopulation].created_at', 'CreatedDateTime', 'COLUMN';
    PRINT '✅ Renamed column: created_at → CreatedDateTime';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'CreatedDateTime')
BEGIN
    PRINT '⚠️  Column CreatedDateTime already exists';
END
ELSE
BEGIN
    PRINT '⚠️  Column created_at does not exist';
END
GO

-- Step 3: Rename updated_at to ModifiedDateTime
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'updated_at')
BEGIN
    EXEC sp_rename '[dbo].[AP_Table_3_2_CensusPopulation].updated_at', 'ModifiedDateTime', 'COLUMN';
    PRINT '✅ Renamed column: updated_at → ModifiedDateTime';
END
ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'ModifiedDateTime')
BEGIN
    PRINT '⚠️  Column ModifiedDateTime already exists';
END
ELSE
BEGIN
    PRINT '⚠️  Column updated_at does not exist';
END
GO

-- Step 4: Add new audit columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD [CreatedBy] INT NULL;
    PRINT '✅ Added column: CreatedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Column CreatedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'CreatedByIPAddress')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD [CreatedByIPAddress] NVARCHAR(50) NULL;
    PRINT '✅ Added column: CreatedByIPAddress';
END
ELSE
BEGIN
    PRINT '⚠️  Column CreatedByIPAddress already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'ModifiedBy')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD [ModifiedBy] INT NULL;
    PRINT '✅ Added column: ModifiedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Column ModifiedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AP_Table_3_2_CensusPopulation]') AND name = 'ModifiedByIPAddress')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD [ModifiedByIPAddress] NVARCHAR(50) NULL;
    PRINT '✅ Added column: ModifiedByIPAddress';
END
ELSE
BEGIN
    PRINT '⚠️  Column ModifiedByIPAddress already exists';
END
GO

-- Step 5: Add foreign key constraints (optional - for referential integrity)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AP_Table_3_2_CensusPopulation_CreatedBy')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD CONSTRAINT [FK_AP_Table_3_2_CensusPopulation_CreatedBy]
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    PRINT '✅ Added foreign key: FK_AP_Table_3_2_CensusPopulation_CreatedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Foreign key FK_AP_Table_3_2_CensusPopulation_CreatedBy already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AP_Table_3_2_CensusPopulation_ModifiedBy')
BEGIN
    ALTER TABLE [dbo].[AP_Table_3_2_CensusPopulation]
    ADD CONSTRAINT [FK_AP_Table_3_2_CensusPopulation_ModifiedBy]
    FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    PRINT '✅ Added foreign key: FK_AP_Table_3_2_CensusPopulation_ModifiedBy';
END
ELSE
BEGIN
    PRINT '⚠️  Foreign key FK_AP_Table_3_2_CensusPopulation_ModifiedBy already exists';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'AP_Table_3_2_CensusPopulation UPDATE COMPLETE';
PRINT '========================================';
PRINT '';
GO
