-- ============================================
-- Generic Workflow Engine - Seed Data
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Insert Workflow Statuses
-- ============================================

-- Status 1: Maker Entry (formerly Draft)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Maker Entry', 'Draft', 'Initial maker entry state. Maker can edit and submit.', 1, 1);
    PRINT 'Maker Entry status added.';
END
GO

-- Status 2: Pending Checker
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'PendingChecker')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Pending Checker', 'PendingChecker', 'Submitted to checker. Maker cannot edit. Waiting for checker approval.', 2, 1);
    PRINT 'Pending Checker status added.';
END
GO

-- Status 3: Rejected by Checker
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'RejectedByChecker')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Rejected by Checker', 'RejectedByChecker', 'Rejected by checker. Returns to maker for corrections.', 3, 1);
    PRINT 'Rejected by Checker status added.';
END
GO

-- Status 4: Pending DESA Head
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'PendingHead')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Pending DESA Head', 'PendingHead', 'Approved by checker. Locked for maker and checker. Waiting for DESA Head approval.', 4, 1);
    PRINT 'Pending DESA Head status added.';
END
GO

-- Status 5: Rejected by DESA Head
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'RejectedByHead')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Rejected by DESA Head', 'RejectedByHead', 'Rejected by DESA Head. Returns to checker for corrections.', 5, 1);
    PRINT 'Rejected by DESA Head status added.';
END
GO

-- Status 6: Approved
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Approved')
BEGIN
    INSERT INTO [dbo].[Mst_WorkflowStatus] ([StatusName], [StatusCode], [Description], [DisplayOrder], [IsActive])
    VALUES ('Approved', 'Approved', 'Final approved state. Locked for all users.', 6, 1);
    PRINT 'Approved status added.';
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Workflow Engine seed data inserted successfully!';
PRINT '===========================================';
GO
