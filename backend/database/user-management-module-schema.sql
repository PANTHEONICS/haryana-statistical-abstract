-- ============================================
-- USER MANAGEMENT & AUTHENTICATION MODULE
-- SQL Server Database Schema
-- Server: KAPILP\SQLEXPRESS
-- Database: HaryanaStatAbstractDb
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Mst_Roles (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mst_Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Mst_Roles] (
        [RoleID] INT IDENTITY(1,1) PRIMARY KEY,
        [RoleName] NVARCHAR(100) NOT NULL UNIQUE
    );

    CREATE INDEX [IX_Mst_Roles_RoleName] ON [dbo].[Mst_Roles]([RoleName]);
    PRINT 'Table Mst_Roles created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Mst_Roles already exists.';
END
GO

-- ============================================
-- Mst_Departments (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mst_Departments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Mst_Departments] (
        [DepartmentID] INT IDENTITY(1,1) PRIMARY KEY,
        [DepartmentName] NVARCHAR(200) NOT NULL,
        [DepartmentCode] NVARCHAR(50) NOT NULL UNIQUE
    );

    CREATE INDEX [IX_Mst_Departments_DepartmentCode] ON [dbo].[Mst_Departments]([DepartmentCode]);
    CREATE INDEX [IX_Mst_Departments_DepartmentName] ON [dbo].[Mst_Departments]([DepartmentName]);
    PRINT 'Table Mst_Departments created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Mst_Departments already exists.';
END
GO

-- ============================================
-- Mst_SecurityQuestions (Master Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mst_SecurityQuestions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Mst_SecurityQuestions] (
        [SecurityQuestionID] INT IDENTITY(1,1) PRIMARY KEY,
        [QuestionText] NVARCHAR(500) NOT NULL UNIQUE
    );

    CREATE INDEX [IX_Mst_SecurityQuestions_QuestionText] ON [dbo].[Mst_SecurityQuestions]([QuestionText]);
    PRINT 'Table Mst_SecurityQuestions created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Mst_SecurityQuestions already exists.';
END
GO

-- ============================================
-- Master_User (Transaction Table)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Master_User]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Master_User] (
        [UserID] INT IDENTITY(1,1) PRIMARY KEY,
        [LoginID] NVARCHAR(50) NOT NULL UNIQUE,
        [UserPassword] NVARCHAR(MAX) NOT NULL,
        [UserMobileNo] NVARCHAR(10) NOT NULL,
        [UserEmailID] NVARCHAR(100) NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [RoleID] INT NOT NULL,
        [DepartmentID] INT NULL,
        [SecurityQuestionID] INT NULL,
        [SecurityQuestionAnswer] NVARCHAR(100) NULL,
        [LoggedInSessionID] NVARCHAR(100) NULL,
        [CurrentLoginDateTime] DATETIME2 NULL,
        [LastLoginDateTime] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [FK_Master_User_Mst_Roles] FOREIGN KEY ([RoleID]) 
            REFERENCES [dbo].[Mst_Roles]([RoleID]),
        CONSTRAINT [FK_Master_User_Mst_Departments] FOREIGN KEY ([DepartmentID]) 
            REFERENCES [dbo].[Mst_Departments]([DepartmentID]),
        CONSTRAINT [FK_Master_User_Mst_SecurityQuestions] FOREIGN KEY ([SecurityQuestionID]) 
            REFERENCES [dbo].[Mst_SecurityQuestions]([SecurityQuestionID]),
        CONSTRAINT [CK_Master_User_UserMobileNo] 
            CHECK (LEN([UserMobileNo]) = 10)
    );

    CREATE INDEX [IX_Master_User_LoginID] ON [dbo].[Master_User]([LoginID]);
    CREATE INDEX [IX_Master_User_RoleID] ON [dbo].[Master_User]([RoleID]);
    CREATE INDEX [IX_Master_User_DepartmentID] ON [dbo].[Master_User]([DepartmentID]);
    CREATE INDEX [IX_Master_User_IsActive] ON [dbo].[Master_User]([IsActive]);
    CREATE INDEX [IX_Master_User_LoggedInSessionID] ON [dbo].[Master_User]([LoggedInSessionID]);
    
    PRINT 'Table Master_User created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Master_User already exists.';
END
GO

-- ============================================
-- FILTERED UNIQUE INDEX: Only ONE Department Checker per Department
-- Note: This index must be created after seed data using dynamic SQL
-- See: backend/database/create-filtered-index.sql
-- ============================================
-- Index creation is deferred to after seed data due to SQL Server limitations
-- Run: backend/database/create-filtered-index.sql after seed data
GO

PRINT '';
PRINT '===========================================';
PRINT 'User Management Module schema created successfully!';
PRINT '===========================================';
GO
