-- ============================================
-- USER MANAGEMENT & AUTHENTICATION MODULE
-- SQL Server Database Schema
-- ============================================

-- Create Database (if not exists)
-- Uncomment the following line if you want to create a new database
-- CREATE DATABASE HaryanaStatAbstractDb;
-- GO

-- USE HaryanaStatAbstractDb;
-- GO

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
END
GO

-- ============================================
-- USER ROLES (Many-to-Many Relationship)
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
END
GO

-- ============================================
-- SEED DATA - Default Roles
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = 'Admin')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsActive])
    VALUES ('Admin', 'Administrator with full access', 1);
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = 'User')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsActive])
    VALUES ('User', 'Standard user with limited access', 1);
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Roles] WHERE [Name] = 'Viewer')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsActive])
    VALUES ('Viewer', 'Read-only access', 1);
END
GO

-- ============================================
-- SEED DATA - Default Admin User
-- Password: Admin@123 (BCrypt hash)
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    DECLARE @AdminUserId INT;
    
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [FirstName], [LastName], [IsActive], [EmailConfirmed])
    VALUES (
        'admin',
        'admin@haryanastatabstract.com',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- This will be replaced with actual BCrypt hash
        'System',
        'Administrator',
        1,
        1
    );
    
    SET @AdminUserId = SCOPE_IDENTITY();
    
    -- Assign Admin role
    INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId])
    SELECT @AdminUserId, [Id] FROM [dbo].[Roles] WHERE [Name] = 'Admin';
END
GO

-- ============================================
-- STORED PROCEDURES
-- ============================================

-- Procedure to get user with roles
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetUserWithRoles]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[sp_GetUserWithRoles];
GO

CREATE PROCEDURE [dbo].[sp_GetUserWithRoles]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.[Id],
        u.[Username],
        u.[Email],
        u.[FirstName],
        u.[LastName],
        u.[IsActive],
        u.[EmailConfirmed],
        u.[LastLoginAt],
        u.[CreatedAt],
        u.[UpdatedAt],
        r.[Id] AS RoleId,
        r.[Name] AS RoleName,
        r.[Description] AS RoleDescription
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[UserRoles] ur ON u.[Id] = ur.[UserId]
    LEFT JOIN [dbo].[Roles] r ON ur.[RoleId] = r.[Id]
    WHERE u.[Id] = @UserId AND u.[IsActive] = 1;
END
GO

-- Procedure to update user's last login
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateUserLastLogin]') AND type in (N'P'))
    DROP PROCEDURE [dbo].[sp_UpdateUserLastLogin];
GO

CREATE PROCEDURE [dbo].[sp_UpdateUserLastLogin]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Users]
    SET [LastLoginAt] = GETUTCDATE(),
        [UpdatedAt] = GETUTCDATE()
    WHERE [Id] = @UserId;
END
GO

PRINT 'User Management & Authentication schema created successfully!';
GO
