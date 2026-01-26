-- ============================================
-- HARAYANA STATISTICAL ABSTRACT DATABASE
-- Complete Database Creation Script
-- SQL Server Database Schema
-- Server: KAPILP\SQLEXPRESS
-- Database: HaryanaStatAbstractDb
-- Authentication: Windows Authentication
-- ============================================

-- Create Database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HaryanaStatAbstractDb')
BEGIN
    CREATE DATABASE [HaryanaStatAbstractDb];
    PRINT 'Database HaryanaStatAbstractDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database HaryanaStatAbstractDb already exists.';
END
GO

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- CENSUS POPULATION TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[census_population]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[census_population] (
        [census_year] INT NOT NULL PRIMARY KEY,
        [total_population] BIGINT NOT NULL,
        [variation_in_population] BIGINT NULL,
        [decennial_percentage_increase] DECIMAL(5,2) NULL,
        [male_population] BIGINT NOT NULL,
        [female_population] BIGINT NOT NULL,
        [sex_ratio] INT NOT NULL,
        [source_document] NVARCHAR(255) NULL,
        [source_table] NVARCHAR(50) NULL,
        [source_reference] NVARCHAR(500) NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        -- Check constraint: Total population should equal male + female
        CONSTRAINT [CK_census_population_TotalEqualsSum] 
            CHECK ([total_population] = [male_population] + [female_population]),
        
        -- Check constraints for valid ranges
        CONSTRAINT [CK_census_population_YearRange] 
            CHECK ([census_year] >= 1900 AND [census_year] <= 2100),
        
        CONSTRAINT [CK_census_population_PopulationPositive] 
            CHECK ([total_population] > 0 AND [male_population] > 0 AND [female_population] > 0),
        
        CONSTRAINT [CK_census_population_SexRatioRange] 
            CHECK ([sex_ratio] >= 0 AND [sex_ratio] <= 2000)
    );

    CREATE UNIQUE INDEX [IX_census_population_Year] ON [dbo].[census_population]([census_year]);
    PRINT 'Table census_population created successfully.';
END
ELSE
BEGIN
    PRINT 'Table census_population already exists.';
END
GO

-- ============================================
-- ROLES TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX [IX_Roles_Name] ON [dbo].[Roles]([Name]);
    CREATE INDEX [IX_Roles_IsActive] ON [dbo].[Roles]([IsActive]);
    PRINT 'Table Roles created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Roles already exists.';
END
GO

-- ============================================
-- USERS TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(100) NOT NULL UNIQUE,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [FirstName] NVARCHAR(100) NULL,
        [LastName] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [EmailConfirmed] BIT NOT NULL DEFAULT 0,
        [LastLoginAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
    CREATE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
    CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users]([IsActive]);
    PRINT 'Table Users created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Users already exists.';
END
GO

-- ============================================
-- USER ROLES TABLE (Many-to-Many Relationship)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles]([UserId]);
    CREATE INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles]([RoleId]);
    PRINT 'Table UserRoles created successfully.';
END
ELSE
BEGIN
    PRINT 'Table UserRoles already exists.';
END
GO

-- ============================================
-- REFRESH TOKENS TABLE (For JWT Refresh Token Support)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshTokens] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Token] NVARCHAR(500) NOT NULL UNIQUE,
        [ExpiresAt] DATETIME2 NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [RevokedAt] DATETIME2 NULL,
        [ReplacedByToken] NVARCHAR(500) NULL,
        CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens]([UserId]);
    CREATE INDEX [IX_RefreshTokens_Token] ON [dbo].[RefreshTokens]([Token]);
    CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [dbo].[RefreshTokens]([ExpiresAt]);
    PRINT 'Table RefreshTokens created successfully.';
END
ELSE
BEGIN
    PRINT 'Table RefreshTokens already exists.';
END
GO

-- ============================================
-- TRIGGERS FOR UPDATED_AT
-- ============================================

-- Trigger for Roles table
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Roles_UpdatedAt')
    DROP TRIGGER [dbo].[TR_Roles_UpdatedAt];
GO

CREATE TRIGGER [dbo].[TR_Roles_UpdatedAt]
ON [dbo].[Roles]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Roles]
    SET [UpdatedAt] = GETUTCDATE()
    FROM [dbo].[Roles] r
    INNER JOIN inserted i ON r.[Id] = i.[Id];
END
GO

-- Trigger for Users table
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Users_UpdatedAt')
    DROP TRIGGER [dbo].[TR_Users_UpdatedAt];
GO

CREATE TRIGGER [dbo].[TR_Users_UpdatedAt]
ON [dbo].[Users]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Users]
    SET [UpdatedAt] = GETUTCDATE()
    FROM [dbo].[Users] u
    INNER JOIN inserted i ON u.[Id] = i.[Id];
END
GO

-- Trigger for census_population table
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_census_population_UpdatedAt')
    DROP TRIGGER [dbo].[TR_census_population_UpdatedAt];
GO

CREATE TRIGGER [dbo].[TR_census_population_UpdatedAt]
ON [dbo].[census_population]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[census_population]
    SET [updated_at] = GETUTCDATE()
    FROM [dbo].[census_population] c
    INNER JOIN inserted i ON c.[census_year] = i.[census_year];
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'Database schema created successfully!';
PRINT 'Database: HaryanaStatAbstractDb';
PRINT 'Server: KAPILP\SQLEXPRESS';
PRINT '===========================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Run the application to seed initial data';
PRINT '2. Or manually insert seed data using the seed script';
PRINT '';
GO
