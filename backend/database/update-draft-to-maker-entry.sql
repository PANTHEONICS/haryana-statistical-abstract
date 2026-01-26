-- ============================================
-- Update Workflow Status: "Draft" to "Maker Entry"
-- ============================================
-- This script updates the StatusName from "Draft" to "Maker Entry"
-- while keeping the StatusCode as "Draft" for backward compatibility
--
-- Execute this script on: HaryanaStatAbstractDb
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Update StatusName from "Draft" to "Maker Entry"
UPDATE [dbo].[Mst_WorkflowStatus]
SET [StatusName] = 'Maker Entry',
    [Description] = 'Initial maker entry state. Maker can edit and submit.',
    [UpdatedAt] = GETUTCDATE()
WHERE [StatusCode] = 'Draft';

IF @@ROWCOUNT > 0
BEGIN
    PRINT 'Updated StatusName from "Draft" to "Maker Entry" successfully.';
END
ELSE
BEGIN
    PRINT 'No records updated. Status with StatusCode "Draft" not found.';
END
GO

-- Verification Query
PRINT '';
PRINT '===========================================';
PRINT 'Verification: Updated Status';
PRINT '===========================================';
SELECT 
    [StatusID],
    [StatusCode],
    [StatusName],
    [Description],
    [DisplayOrder]
FROM [dbo].[Mst_WorkflowStatus]
WHERE [StatusCode] = 'Draft';
GO

PRINT '';
PRINT '===========================================';
PRINT 'Update Complete!';
PRINT '===========================================';
PRINT '';
PRINT 'StatusName has been changed from "Draft" to "Maker Entry"';
PRINT 'StatusCode remains "Draft" for backward compatibility.';
PRINT '';
GO
