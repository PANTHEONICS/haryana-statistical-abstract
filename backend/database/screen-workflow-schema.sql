-- ============================================
-- Screen-Level Workflow Management Schema
-- ============================================
-- This table tracks workflow status at the SCREEN level, not individual records
-- Each screen/module will have one record in this table

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Workflow_Screen Table
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Workflow_Screen]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Workflow_Screen] (
        [ScreenWorkflowID] INT IDENTITY(1,1) PRIMARY KEY,
        [ScreenName] NVARCHAR(100) NOT NULL,
        [ScreenCode] NVARCHAR(50) NOT NULL UNIQUE,
        [TableName] NVARCHAR(100) NOT NULL, -- The business table this screen manages
        [CurrentStatusID] INT NOT NULL DEFAULT 1, -- FK to Mst_WorkflowStatus
        [CurrentStatusName] NVARCHAR(100) NULL, -- Denormalized for quick access
        [CreatedByUserID] INT NOT NULL, -- FK to Master_User
        [CreatedByUserName] NVARCHAR(100) NULL, -- Denormalized for quick access
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        -- Foreign Keys
        CONSTRAINT [FK_Workflow_Screen_Status] FOREIGN KEY ([CurrentStatusID]) 
        REFERENCES [dbo].[Mst_WorkflowStatus]([StatusID]),
        CONSTRAINT [FK_Workflow_Screen_CreatedBy] FOREIGN KEY ([CreatedByUserID]) 
        REFERENCES [dbo].[Master_User]([UserID]),
            
        -- Indexes
        INDEX [IX_Workflow_Screen_ScreenCode] NONCLUSTERED ([ScreenCode]),
        INDEX [IX_Workflow_Screen_TableName] NONCLUSTERED ([TableName]),
        INDEX [IX_Workflow_Screen_CurrentStatusID] NONCLUSTERED ([CurrentStatusID])
    );
    PRINT 'Workflow_Screen table created successfully.';
END
ELSE
BEGIN
    PRINT 'Workflow_Screen table already exists.';
END
GO

-- ============================================
-- Update Workflow_AuditHistory to support Screen-Level tracking
-- ============================================
-- Add ScreenWorkflowID column to track screen-level audit history
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Workflow_AuditHistory]') AND name = 'ScreenWorkflowID')
BEGIN
    ALTER TABLE [dbo].[Workflow_AuditHistory]
    ADD [ScreenWorkflowID] INT NULL;
    
    -- Foreign Key
    ALTER TABLE [dbo].[Workflow_AuditHistory]
    ADD CONSTRAINT [FK_Workflow_AuditHistory_ScreenWorkflow] FOREIGN KEY ([ScreenWorkflowID]) 
        REFERENCES [dbo].[Workflow_Screen]([ScreenWorkflowID]);
    
    -- Index
    CREATE NONCLUSTERED INDEX [IX_Workflow_AuditHistory_ScreenWorkflowID] 
        ON [dbo].[Workflow_AuditHistory]([ScreenWorkflowID]);
    
    PRINT 'Added ScreenWorkflowID column to Workflow_AuditHistory table.';
END
ELSE
BEGIN
    PRINT 'ScreenWorkflowID column already exists in Workflow_AuditHistory table.';
END
GO

-- ============================================
-- Remove workflow columns from census_population table
-- ============================================
-- Note: We'll keep this commented out for now. Execute separately after verification.
/*
-- Remove foreign key constraints first
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CensusPopulation_WorkflowStatus')
BEGIN
    ALTER TABLE [dbo].[census_population] DROP CONSTRAINT [FK_CensusPopulation_WorkflowStatus];
    PRINT 'Dropped FK_CensusPopulation_WorkflowStatus constraint.';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CensusPopulation_CreatedBy')
BEGIN
    ALTER TABLE [dbo].[census_population] DROP CONSTRAINT [FK_CensusPopulation_CreatedBy];
    PRINT 'Dropped FK_CensusPopulation_CreatedBy constraint.';
END

-- Remove columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[census_population]') AND name = 'CurrentStatusID')
BEGIN
    ALTER TABLE [dbo].[census_population] DROP COLUMN [CurrentStatusID];
    PRINT 'Removed CurrentStatusID column from census_population table.';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[census_population]') AND name = 'CreatedByUserID')
BEGIN
    ALTER TABLE [dbo].[census_population] DROP COLUMN [CreatedByUserID];
    PRINT 'Removed CreatedByUserID column from census_population table.';
END
*/
GO

PRINT '';
PRINT '===========================================';
PRINT 'Screen-Level Workflow Schema created successfully!';
PRINT '===========================================';
GO
