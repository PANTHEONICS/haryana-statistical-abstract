-- ============================================
-- Master Screen Registry Table
-- Purpose: Centralized tracking of all 350+ data entry screens
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Mst_ScreenRegistry (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_ScreenRegistry' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Mst_ScreenRegistry] (
        [ScreenRegistryID] INT IDENTITY(1,1) NOT NULL,
        [ScreenCode] NVARCHAR(100) NOT NULL UNIQUE, -- e.g., "AP_TABLE_3_2_CENSUS_POPULATION"
        [ScreenName] NVARCHAR(200) NOT NULL, -- e.g., "Census Population Management"
        [TableName] NVARCHAR(100) NOT NULL UNIQUE, -- e.g., "AP_Table_3_2_CensusPopulation"
        [DepartmentID] INT NOT NULL, -- FK to Mst_Departments
        [TableNumber] NVARCHAR(50) NOT NULL, -- e.g., "3.2"
        [SourceDocument] NVARCHAR(200) NULL, -- e.g., "Statistical Abstract of Haryana 2023-24"
        [SourceTable] NVARCHAR(100) NULL, -- e.g., "Table 3.2"
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [CreatedByUserID] INT NULL,
        
        CONSTRAINT [PK_Mst_ScreenRegistry] PRIMARY KEY CLUSTERED ([ScreenRegistryID] ASC),
        CONSTRAINT [FK_Mst_ScreenRegistry_Department] 
            FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Mst_Departments]([DepartmentID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Mst_ScreenRegistry_CreatedBy] 
            FOREIGN KEY ([CreatedByUserID]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_Mst_ScreenRegistry_ScreenCode] UNIQUE ([ScreenCode]),
        CONSTRAINT [UQ_Mst_ScreenRegistry_TableName] UNIQUE ([TableName])
    );
    
    CREATE INDEX [IX_Mst_ScreenRegistry_DepartmentID] ON [dbo].[Mst_ScreenRegistry]([DepartmentID]);
    CREATE INDEX [IX_Mst_ScreenRegistry_ScreenCode] ON [dbo].[Mst_ScreenRegistry]([ScreenCode]);
    CREATE INDEX [IX_Mst_ScreenRegistry_TableName] ON [dbo].[Mst_ScreenRegistry]([TableName]);
    CREATE INDEX [IX_Mst_ScreenRegistry_IsActive] ON [dbo].[Mst_ScreenRegistry]([IsActive]);
    CREATE INDEX [IX_Mst_ScreenRegistry_DisplayOrder] ON [dbo].[Mst_ScreenRegistry]([DisplayOrder]);
    
    PRINT 'Mst_ScreenRegistry table created successfully.';
END
ELSE
BEGIN
    PRINT 'Mst_ScreenRegistry table already exists.';
END
GO

-- ============================================
-- Seed Data: Register Census Screen
-- ============================================
IF EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'AP_TABLE_3_2_CENSUS_POPULATION')
BEGIN
    PRINT 'Census screen already registered in Mst_ScreenRegistry.';
END
ELSE
BEGIN
    -- Get DepartmentID for "Area & Population" (assuming DepartmentCode = 'AP')
    DECLARE @APDepartmentID INT;
    SELECT @APDepartmentID = [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'AP';
    
    IF @APDepartmentID IS NULL
    BEGIN
        PRINT 'WARNING: Department "Area & Population" (Code: AP) not found. Please create it first.';
    END
    ELSE
    BEGIN
        -- Get Admin UserID (assuming LoginID = 'admin')
        DECLARE @AdminUserID INT;
        SELECT @AdminUserID = [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin';
        
        INSERT INTO [dbo].[Mst_ScreenRegistry] (
            [ScreenCode],
            [ScreenName],
            [TableName],
            [DepartmentID],
            [TableNumber],
            [SourceDocument],
            [SourceTable],
            [Description],
            [IsActive],
            [DisplayOrder],
            [CreatedByUserID]
        )
        VALUES (
            'AP_TABLE_3_2_CENSUS_POPULATION',
            'Census Population Management',
            'census_population', -- Current table name (will be renamed to AP_Table_3_2_CensusPopulation)
            @APDepartmentID,
            '3.2',
            'Statistical Abstract of Haryana 2023-24',
            'Table 3.2',
            'Census population data for Haryana (1901-2011 Census)',
            1,
            1,
            @AdminUserID
        );
        
        PRINT 'Census screen registered in Mst_ScreenRegistry.';
    END
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Master Screen Registry Setup Complete!';
PRINT '===========================================';
GO
