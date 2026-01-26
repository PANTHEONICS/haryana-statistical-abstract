-- ============================================
-- Generic Workflow Engine - Database Schema
-- ============================================
-- This schema provides a generic, reusable workflow engine
-- that can be integrated with any business table

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- 1. Mst_WorkflowStatus (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_WorkflowStatus' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Mst_WorkflowStatus] (
        [StatusID] INT IDENTITY(1,1) NOT NULL,
        [StatusName] NVARCHAR(100) NOT NULL,
        [StatusCode] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        
        CONSTRAINT [PK_Mst_WorkflowStatus] PRIMARY KEY CLUSTERED ([StatusID] ASC),
        CONSTRAINT [UQ_Mst_WorkflowStatus_StatusCode] UNIQUE ([StatusCode])
    );
    
    CREATE INDEX [IX_Mst_WorkflowStatus_StatusCode] ON [dbo].[Mst_WorkflowStatus]([StatusCode]);
    CREATE INDEX [IX_Mst_WorkflowStatus_IsActive] ON [dbo].[Mst_WorkflowStatus]([IsActive]);
    CREATE INDEX [IX_Mst_WorkflowStatus_DisplayOrder] ON [dbo].[Mst_WorkflowStatus]([DisplayOrder]);
    
    PRINT 'Mst_WorkflowStatus table created successfully.';
END
GO

-- ============================================
-- 2. Workflow_AuditHistory (Generic Audit Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_AuditHistory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Workflow_AuditHistory] (
        [AuditID] INT IDENTITY(1,1) NOT NULL,
        [TargetTableName] NVARCHAR(100) NOT NULL,
        [TargetRecordID] INT NOT NULL,
        [ActionName] NVARCHAR(50) NOT NULL,
        [FromStatusID] INT NULL,
        [ToStatusID] INT NOT NULL,
        [Remarks] NVARCHAR(MAX) NULL,
        [ActionByUserID] INT NOT NULL,
        [ActionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_Workflow_AuditHistory] PRIMARY KEY CLUSTERED ([AuditID] ASC),
        CONSTRAINT [FK_Workflow_AuditHistory_FromStatus] FOREIGN KEY ([FromStatusID]) 
            REFERENCES [dbo].[Mst_WorkflowStatus] ([StatusID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Workflow_AuditHistory_ToStatus] FOREIGN KEY ([ToStatusID]) 
            REFERENCES [dbo].[Mst_WorkflowStatus] ([StatusID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Workflow_AuditHistory_User] FOREIGN KEY ([ActionByUserID]) 
            REFERENCES [dbo].[Master_User] ([UserID]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_Workflow_AuditHistory_TargetTable_Record] ON [dbo].[Workflow_AuditHistory]([TargetTableName], [TargetRecordID]);
    CREATE INDEX [IX_Workflow_AuditHistory_ActionByUser] ON [dbo].[Workflow_AuditHistory]([ActionByUserID]);
    CREATE INDEX [IX_Workflow_AuditHistory_ActionDate] ON [dbo].[Workflow_AuditHistory]([ActionDate] DESC);
    CREATE INDEX [IX_Workflow_AuditHistory_FromStatus] ON [dbo].[Workflow_AuditHistory]([FromStatusID]);
    CREATE INDEX [IX_Workflow_AuditHistory_ToStatus] ON [dbo].[Workflow_AuditHistory]([ToStatusID]);
    
    PRINT 'Workflow_AuditHistory table created successfully.';
END
GO

-- ============================================
-- Business Table Integration Instructions
-- ============================================
-- Every business table that needs workflow integration must have:
-- 1. CurrentStatusID INT NOT NULL DEFAULT 1 (References Mst_WorkflowStatus.StatusID)
--    - Default value 1 = Draft status
-- 2. CreatedByUserID INT NOT NULL (References Master_User.UserID)
--
-- Example ALTER statement for an existing table:
-- ALTER TABLE [dbo].[Tbl_PopulationData]
-- ADD [CurrentStatusID] INT NOT NULL DEFAULT 1,
--     [CreatedByUserID] INT NOT NULL;
--
-- ALTER TABLE [dbo].[Tbl_PopulationData]
-- ADD CONSTRAINT [FK_Tbl_PopulationData_Status] 
--     FOREIGN KEY ([CurrentStatusID]) REFERENCES [dbo].[Mst_WorkflowStatus]([StatusID]),
--     CONSTRAINT [FK_Tbl_PopulationData_CreatedBy] 
--     FOREIGN KEY ([CreatedByUserID]) REFERENCES [dbo].[Master_User]([UserID]);
-- ============================================

PRINT '';
PRINT '===========================================';
PRINT 'Workflow Engine Schema created successfully!';
PRINT '===========================================';
GO
