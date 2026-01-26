-- ============================================
-- Workflow "Happy Path" Refactoring Script
-- ============================================
-- This script refactors the workflow from a linear flow with negative statuses
-- to a "Happy Path" workflow where rejected statuses loop back to previous states
-- and are mapped to visual stages for UI display.
--
-- Changes:
-- 1. Add Visual_Stage_Key column to Mst_WorkflowStatus
-- 2. Map statuses to visual stages (Draft, Checker Review, DESA Head Review, Approved)
-- 3. Rejected statuses (3, 5) remain in database for audit but map to previous visual stages
--
-- Execute this script on: HaryanaStatAbstractDb
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Step 1: Add Visual_Stage_Key Column
-- ============================================
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Mst_WorkflowStatus]') 
    AND name = 'Visual_Stage_Key'
)
BEGIN
    ALTER TABLE [dbo].[Mst_WorkflowStatus]
    ADD [Visual_Stage_Key] NVARCHAR(50) NULL;
    
    PRINT 'Added Visual_Stage_Key column to Mst_WorkflowStatus table.';
END
ELSE
BEGIN
    PRINT 'Visual_Stage_Key column already exists in Mst_WorkflowStatus table.';
END
GO

-- ============================================
-- Step 2: Update Visual Stage Mappings
-- ============================================
-- Map statuses to visual stages:
-- Status 1 (Maker Entry) → "Draft" (StatusCode remains 'Draft' for backward compatibility)
-- Status 2 (Pending Checker) → "CheckerReview"
-- Status 3 (Rejected by Checker) → "Draft" (loops back visually)
-- Status 4 (Pending DESA Head) → "DESAHeadReview"
-- Status 5 (Rejected by DESA Head) → "CheckerReview" (loops back visually)
-- Status 6 (Approved) → "Approved"

-- Status 1: Maker Entry (StatusCode: 'Draft')
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'Draft'
WHERE [StatusCode] = 'Draft' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'Draft');
PRINT 'Mapped Status 1 (Maker Entry) to Visual Stage: Draft';

-- Status 2: Pending Checker
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'CheckerReview'
WHERE [StatusCode] = 'PendingChecker' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'CheckerReview');
PRINT 'Mapped Status 2 (Pending Checker) to Visual Stage: CheckerReview';

-- Status 3: Rejected by Checker → Maps to "Draft" visual stage (loops back)
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'Draft'
WHERE [StatusCode] = 'RejectedByChecker' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'Draft');
PRINT 'Mapped Status 3 (Rejected by Checker) to Visual Stage: Draft (loops back)';

-- Status 4: Pending DESA Head
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'DESAHeadReview'
WHERE [StatusCode] = 'PendingHead' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'DESAHeadReview');
PRINT 'Mapped Status 4 (Pending DESA Head) to Visual Stage: DESAHeadReview';

-- Status 5: Rejected by DESA Head → Maps to "CheckerReview" visual stage (loops back)
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'CheckerReview'
WHERE [StatusCode] = 'RejectedByHead' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'CheckerReview');
PRINT 'Mapped Status 5 (Rejected by DESA Head) to Visual Stage: CheckerReview (loops back)';

-- Status 6: Approved
UPDATE [dbo].[Mst_WorkflowStatus]
SET [Visual_Stage_Key] = 'Approved'
WHERE [StatusCode] = 'Approved' AND ([Visual_Stage_Key] IS NULL OR [Visual_Stage_Key] != 'Approved');
PRINT 'Mapped Status 6 (Approved) to Visual Stage: Approved';
GO

-- ============================================
-- Step 3: Add Index for Performance
-- ============================================
IF NOT EXISTS (
    SELECT * FROM sys.indexes 
    WHERE name = 'IX_Mst_WorkflowStatus_VisualStageKey' 
    AND object_id = OBJECT_ID(N'[dbo].[Mst_WorkflowStatus]')
)
BEGIN
    CREATE INDEX [IX_Mst_WorkflowStatus_VisualStageKey] 
    ON [dbo].[Mst_WorkflowStatus]([Visual_Stage_Key]);
    PRINT 'Created index on Visual_Stage_Key column.';
END
ELSE
BEGIN
    PRINT 'Index IX_Mst_WorkflowStatus_VisualStageKey already exists.';
END
GO

-- ============================================
-- Step 4: Verification Query
-- ============================================
PRINT '';
PRINT '===========================================';
PRINT 'Visual Stage Mapping Verification:';
PRINT '===========================================';
SELECT 
    [StatusID],
    [StatusCode],
    [StatusName],
    [Visual_Stage_Key],
    [DisplayOrder]
FROM [dbo].[Mst_WorkflowStatus]
ORDER BY [DisplayOrder], [StatusID];
GO

PRINT '';
PRINT '===========================================';
PRINT 'Workflow Happy Path Refactoring Complete!';
PRINT '===========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update C# Model (MstWorkflowStatus.cs) to include Visual_Stage_Key property';
PRINT '2. Update WorkflowService.cs to loop back on rejections:';
PRINT '   - Checker Reject: Set status to 1 (Maker Entry) instead of 3';
PRINT '   - DESA Head Reject: Set status to 2 (Pending Checker) instead of 5';
PRINT '3. Update React WorkflowStatusBar.jsx to use Visual_Stage_Key for UI display';
PRINT '4. Show error/warning color for rejection statuses (3, 5) but position at correct visual stage';
PRINT '';
GO
