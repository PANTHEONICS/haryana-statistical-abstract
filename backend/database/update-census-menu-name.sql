-- ============================================
-- Update Census Menu Name to "AREA AND POPULATION (Table 3.1)"
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Update the menu name
IF EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/census')
BEGIN
    UPDATE [dbo].[Mst_Menus]
    SET [MenuName] = 'AREA AND POPULATION (Table 3.1)',
        [MenuDescription] = 'Area and Population data management (Table 3.1)',
        [UpdatedAt] = GETUTCDATE()
    WHERE [MenuPath] = '/census';
    
    PRINT 'Menu name updated from "Census" to "AREA AND POPULATION (Table 3.1)".';
END
ELSE
BEGIN
    PRINT 'Menu with path "/census" not found.';
END
GO

-- Verify the update
SELECT [MenuID], [MenuName], [MenuPath], [MenuDescription]
FROM [dbo].[Mst_Menus]
WHERE [MenuPath] = '/census';
GO

PRINT '';
PRINT '===========================================';
PRINT 'Census Menu Name Update Complete!';
PRINT '===========================================';
PRINT '';
PRINT 'Updated Menu:';
PRINT '  Name: AREA AND POPULATION (Table 3.1)';
PRINT '  Path: /census';
PRINT '';
GO
