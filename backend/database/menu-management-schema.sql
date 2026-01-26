-- ============================================
-- Menu Management Module - Database Schema
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- 1. Mst_Menus (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Menus' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Mst_Menus] (
        [MenuID] INT IDENTITY(1,1) NOT NULL,
        [MenuName] NVARCHAR(100) NOT NULL,
        [MenuPath] NVARCHAR(200) NOT NULL,
        [MenuIcon] NVARCHAR(50) NULL,
        [ParentMenuID] INT NULL,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsAdminOnly] BIT NOT NULL DEFAULT 0,
        [MenuDescription] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        
        CONSTRAINT [PK_Mst_Menus] PRIMARY KEY CLUSTERED ([MenuID] ASC),
        CONSTRAINT [FK_Mst_Menus_ParentMenu] FOREIGN KEY ([ParentMenuID]) 
            REFERENCES [dbo].[Mst_Menus] ([MenuID]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_Mst_Menus_MenuPath] UNIQUE ([MenuPath])
    );
    
    CREATE INDEX [IX_Mst_Menus_ParentMenuID] ON [dbo].[Mst_Menus]([ParentMenuID]);
    CREATE INDEX [IX_Mst_Menus_DisplayOrder] ON [dbo].[Mst_Menus]([DisplayOrder]);
    CREATE INDEX [IX_Mst_Menus_IsActive] ON [dbo].[Mst_Menus]([IsActive]);
    
    PRINT 'Mst_Menus table created successfully.';
END
GO

-- ============================================
-- 2. Mst_Department_Menu_Mapping (Mapping Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Department_Menu_Mapping' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Mst_Department_Menu_Mapping] (
        [MappingID] INT IDENTITY(1,1) NOT NULL,
        [DepartmentID] INT NOT NULL,
        [MenuID] INT NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] INT NULL,
        [UpdatedAt] DATETIME2 NULL,
        [UpdatedBy] INT NULL,
        
        CONSTRAINT [PK_Mst_Department_Menu_Mapping] PRIMARY KEY CLUSTERED ([MappingID] ASC),
        CONSTRAINT [FK_Mst_Department_Menu_Mapping_Department] FOREIGN KEY ([DepartmentID]) 
            REFERENCES [dbo].[Mst_Departments] ([DepartmentID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Mst_Department_Menu_Mapping_Menu] FOREIGN KEY ([MenuID]) 
            REFERENCES [dbo].[Mst_Menus] ([MenuID]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Mst_Department_Menu_Mapping] UNIQUE ([DepartmentID], [MenuID])
    );
    
    CREATE INDEX [IX_Mst_Department_Menu_Mapping_DepartmentID] ON [dbo].[Mst_Department_Menu_Mapping]([DepartmentID]);
    CREATE INDEX [IX_Mst_Department_Menu_Mapping_MenuID] ON [dbo].[Mst_Department_Menu_Mapping]([MenuID]);
    CREATE INDEX [IX_Mst_Department_Menu_Mapping_IsActive] ON [dbo].[Mst_Department_Menu_Mapping]([IsActive]);
    
    PRINT 'Mst_Department_Menu_Mapping table created successfully.';
END
GO

-- ============================================
-- 3. Mst_Role_Menu_Mapping (For Special Role Access)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Role_Menu_Mapping' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Mst_Role_Menu_Mapping] (
        [MappingID] INT IDENTITY(1,1) NOT NULL,
        [RoleID] INT NOT NULL,
        [MenuID] INT NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] INT NULL,
        [UpdatedAt] DATETIME2 NULL,
        [UpdatedBy] INT NULL,
        
        CONSTRAINT [PK_Mst_Role_Menu_Mapping] PRIMARY KEY CLUSTERED ([MappingID] ASC),
        CONSTRAINT [FK_Mst_Role_Menu_Mapping_Role] FOREIGN KEY ([RoleID]) 
            REFERENCES [dbo].[Mst_Roles] ([RoleID]) ON DELETE CASCADE,
        CONSTRAINT [FK_Mst_Role_Menu_Mapping_Menu] FOREIGN KEY ([MenuID]) 
            REFERENCES [dbo].[Mst_Menus] ([MenuID]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Mst_Role_Menu_Mapping] UNIQUE ([RoleID], [MenuID])
    );
    
    CREATE INDEX [IX_Mst_Role_Menu_Mapping_RoleID] ON [dbo].[Mst_Role_Menu_Mapping]([RoleID]);
    CREATE INDEX [IX_Mst_Role_Menu_Mapping_MenuID] ON [dbo].[Mst_Role_Menu_Mapping]([MenuID]);
    CREATE INDEX [IX_Mst_Role_Menu_Mapping_IsActive] ON [dbo].[Mst_Role_Menu_Mapping]([IsActive]);
    
    PRINT 'Mst_Role_Menu_Mapping table created successfully.';
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Menu Management Module tables created successfully!';
PRINT '===========================================';
GO
