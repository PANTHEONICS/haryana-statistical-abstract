-- ============================================
-- Menu Management Module - Seed Data
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- 1. Insert Default Menus
-- ============================================

-- Dashboard (Available to all)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Dashboard', '/', 'LayoutDashboard', NULL, 1, 1, 0, 'Main dashboard page');
    PRINT 'Dashboard menu added.';
END
GO

-- Data Management (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/data')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Data Management', '/data', 'Database', NULL, 2, 1, 0, 'Department data management');
    PRINT 'Data Management menu added.';
END
GO

-- AREA AND POPULATION (Table 3.1) (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/census')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('AREA AND POPULATION (Table 3.1)', '/census', 'Users', NULL, 3, 1, 0, 'Area and Population data management (Table 3.1)');
    PRINT 'AREA AND POPULATION (Table 3.1) menu added.';
END
GO

-- Detail View (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/detail')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Detail View', '/detail', 'FileText', NULL, 4, 1, 0, 'Detailed data view');
    PRINT 'Detail View menu added.';
END
GO

-- Workflow (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/workflow')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Workflow', '/workflow', 'Workflow', NULL, 5, 1, 0, 'Workflow management');
    PRINT 'Workflow menu added.';
END
GO

-- Board View (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/board')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Board View', '/board', 'Kanban', NULL, 6, 1, 0, 'Kanban board view');
    PRINT 'Board View menu added.';
END
GO

-- Analytics (Department-specific)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/analytics')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Analytics', '/analytics', 'BarChart3', NULL, 7, 1, 0, 'Analytics and reports');
    PRINT 'Analytics menu added.';
END
GO

-- User Management (Admin Only)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/users')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('User Management', '/users', 'UserCog', NULL, 8, 1, 1, 'User management and configuration');
    PRINT 'User Management menu added.';
END
GO

-- Menu Configuration (Admin Only)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/menu-config')
BEGIN
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription])
    VALUES ('Menu Configuration', '/menu-config', 'Settings', NULL, 9, 1, 1, 'Configure menus for departments');
    PRINT 'Menu Configuration menu added.';
END
GO

-- ============================================
-- 2. Assign Menus to DESA Head Role
-- DESA Head should have access to all department menus
-- ============================================

DECLARE @DESAHeadRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'DESA Head');

IF @DESAHeadRoleID IS NOT NULL
BEGIN
    -- Get all non-admin menus
    DECLARE @MenuCursor CURSOR;
    SET @MenuCursor = CURSOR FOR
        SELECT [MenuID] FROM [dbo].[Mst_Menus] 
        WHERE [IsAdminOnly] = 0 AND [IsActive] = 1;
    
    DECLARE @MenuID INT;
    OPEN @MenuCursor;
    FETCH NEXT FROM @MenuCursor INTO @MenuID;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Role_Menu_Mapping] 
                       WHERE [RoleID] = @DESAHeadRoleID AND [MenuID] = @MenuID)
        BEGIN
            INSERT INTO [dbo].[Mst_Role_Menu_Mapping] ([RoleID], [MenuID], [IsActive])
            VALUES (@DESAHeadRoleID, @MenuID, 1);
        END
        
        FETCH NEXT FROM @MenuCursor INTO @MenuID;
    END
    
    CLOSE @MenuCursor;
    DEALLOCATE @MenuCursor;
    
    PRINT 'Menus assigned to DESA Head role.';
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Menu Management Module seed data inserted successfully!';
PRINT '===========================================';
GO
