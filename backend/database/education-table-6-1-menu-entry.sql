-- ============================================
-- EDUCATION DEPARTMENT - TABLE 6.1
-- Create Menu Entry for Institutions Management (Table 6.1)
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Create Menu Entry for Education Table 6.1
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/education/table6-1')
BEGIN
    -- Get the next display order (after the last menu)
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    
    INSERT INTO [dbo].[Mst_Menus] (
        [MenuName], 
        [MenuPath], 
        [MenuIcon], 
        [ParentMenuID], 
        [DisplayOrder], 
        [IsActive], 
        [IsAdminOnly], 
        [MenuDescription],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        'Institutions Management (Table 6.1)',
        '/education/table6-1',
        'Building2',
        NULL,
        @NextDisplayOrder,
        1,
        0,
        'Number of recognised universities/colleges/schools in Haryana',
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    PRINT 'Menu entry created for Institutions Management (Table 6.1).';
    PRINT '  Menu Path: /education/table6-1';
    PRINT '  Display Order: ' + CAST(@NextDisplayOrder AS NVARCHAR(10));
END
ELSE
BEGIN
    PRINT 'Menu entry for /education/table6-1 already exists.';
END
GO

-- ============================================
-- Display Summary
-- ============================================
PRINT '';
PRINT '===========================================';
PRINT 'Menu Entry Created';
PRINT '===========================================';
PRINT '';
PRINT 'Menu: Institutions Management (Table 6.1)';
PRINT 'Path: /education/table6-1';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Go to Menu Configuration (/menu-config)';
PRINT '2. Select Education Department';
PRINT '3. Check the box for "Institutions Management (Table 6.1)"';
PRINT '4. Click "Save Configuration"';
PRINT '';
GO
