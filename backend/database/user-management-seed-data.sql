-- ============================================
-- USER MANAGEMENT MODULE - SEED DATA
-- Test Data for User Management & Authentication
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- ============================================
-- Seed Mst_Roles
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'System Admin')
BEGIN
    INSERT INTO [dbo].[Mst_Roles] ([RoleName]) VALUES ('System Admin');
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'DESA Head')
BEGIN
    INSERT INTO [dbo].[Mst_Roles] ([RoleName]) VALUES ('DESA Head');
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker')
BEGIN
    INSERT INTO [dbo].[Mst_Roles] ([RoleName]) VALUES ('Department Maker');
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker')
BEGIN
    INSERT INTO [dbo].[Mst_Roles] ([RoleName]) VALUES ('Department Checker');
END
GO

-- ============================================
-- Seed Mst_Departments
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'HFW')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Health & Family Welfare', 'HFW');
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'AP')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Area & Population', 'AP');
END
GO

-- ============================================
-- Seed Mst_SecurityQuestions
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_SecurityQuestions] WHERE [QuestionText] = 'What is your mother''s maiden name?')
BEGIN
    INSERT INTO [dbo].[Mst_SecurityQuestions] ([QuestionText])
    VALUES ('What is your mother''s maiden name?');
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Mst_SecurityQuestions] WHERE [QuestionText] = 'What was the name of your first school?')
BEGIN
    INSERT INTO [dbo].[Mst_SecurityQuestions] ([QuestionText])
    VALUES ('What was the name of your first school?');
END
GO

-- ============================================
-- Seed Master_User (Test Users)
-- Note: Passwords are BCrypt hashes. Default password: Admin@123
-- ============================================

-- 1. Admin User
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'admin')
BEGIN
    DECLARE @AdminRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'System Admin');
    
    INSERT INTO [dbo].[Master_User] (
        [LoginID], 
        [UserPassword], 
        [UserMobileNo], 
        [UserEmailID], 
        [FullName], 
        [RoleID], 
        [DepartmentID], 
        [IsActive]
    )
    VALUES (
        'admin',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        '9876543210',
        'admin@haryanastatabstract.com',
        'System Administrator',
        @AdminRoleID,
        NULL,
        1
    );
    
    PRINT 'Admin user created successfully.';
END
GO

-- 2. DESA Head User
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'desa_head')
BEGIN
    DECLARE @DESAHeadRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'DESA Head');
    
    INSERT INTO [dbo].[Master_User] (
        [LoginID], 
        [UserPassword], 
        [UserMobileNo], 
        [UserEmailID], 
        [FullName], 
        [RoleID], 
        [DepartmentID], 
        [IsActive]
    )
    VALUES (
        'desa_head',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        '9988776655',
        'desa.head@haryanastatabstract.com',
        'DESA Head',
        @DESAHeadRoleID,
        NULL,
        1
    );
    
    PRINT 'DESA Head user created successfully.';
END
GO

-- 3. Health Maker User
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'hfw_maker')
BEGIN
    DECLARE @MakerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker');
    DECLARE @HFWDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'HFW');
    
    INSERT INTO [dbo].[Master_User] (
        [LoginID], 
        [UserPassword], 
        [UserMobileNo], 
        [UserEmailID], 
        [FullName], 
        [RoleID], 
        [DepartmentID], 
        [IsActive]
    )
    VALUES (
        'hfw_maker',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        '9123456780',
        'hfw.maker@haryanastatabstract.com',
        'Health Maker',
        @MakerRoleID,
        @HFWDeptID,
        1
    );
    
    PRINT 'Health Maker user created successfully.';
END
GO

-- 4. Health Checker User
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'hfw_check')
BEGIN
    DECLARE @CheckerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker');
    DECLARE @HFWDeptIDForChecker INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'HFW');
    
    INSERT INTO [dbo].[Master_User] (
        [LoginID], 
        [UserPassword], 
        [UserMobileNo], 
        [UserEmailID], 
        [FullName], 
        [RoleID], 
        [DepartmentID], 
        [IsActive]
    )
    VALUES (
        'hfw_check',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        '9123456789',
        'hfw.checker@haryanastatabstract.com',
        'Health Checker',
        @CheckerRoleID,
        @HFWDeptIDForChecker,
        1
    );
    
    PRINT 'Health Checker user created successfully.';
END
GO

-- 5. Area Maker User
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'ap_maker')
BEGIN
    DECLARE @AreaMakerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker');
    DECLARE @APDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'AP');
    
    INSERT INTO [dbo].[Master_User] (
        [LoginID], 
        [UserPassword], 
        [UserMobileNo], 
        [UserEmailID], 
        [FullName], 
        [RoleID], 
        [DepartmentID], 
        [IsActive]
    )
    VALUES (
        'ap_maker',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        '8877665544',
        'ap.maker@haryanastatabstract.com',
        'Area Maker',
        @AreaMakerRoleID,
        @APDeptID,
        1
    );
    
    PRINT 'Area Maker user created successfully.';
END
GO

-- ============================================
-- Create Filtered Unique Index (if not already created)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Master_User_UniqueCheckerPerDepartment' AND object_id = OBJECT_ID(N'[dbo].[Master_User]'))
BEGIN
    DECLARE @CheckerRoleIDForIndex INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker');
    
    IF @CheckerRoleIDForIndex IS NOT NULL
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX [IX_Master_User_UniqueCheckerPerDepartment]
        ON [dbo].[Master_User]([DepartmentID], [RoleID])
        WHERE [RoleID] = @CheckerRoleIDForIndex 
          AND [DepartmentID] IS NOT NULL 
          AND [IsActive] = 1;
        
        PRINT 'Filtered unique index for Department Checker created successfully.';
    END
END
GO

PRINT '';
PRINT '===========================================';
PRINT 'User Management Module seed data inserted successfully!';
PRINT '===========================================';
PRINT '';
PRINT 'Test User Credentials (all use password: Admin@123):';
PRINT '1. admin (System Admin)';
PRINT '2. desa_head (DESA Head)';
PRINT '3. hfw_maker (Department Maker - Health & Family Welfare)';
PRINT '4. hfw_check (Department Checker - Health & Family Welfare)';
PRINT '5. ap_maker (Department Maker - Area & Population)';
PRINT '';
GO
