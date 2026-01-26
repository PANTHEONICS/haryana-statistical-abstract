-- ============================================
-- EDUCATION DEPARTMENT - USERS CREATION
-- Create Education Department and Maker/Checker users
-- Password: Admin@123 (BCrypt hash)
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Step 1: Create Education Department
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Education', 'EDU');
    PRINT 'Education Department created successfully.';
END
ELSE
BEGIN
    PRINT 'Education Department already exists.';
END
GO

-- ============================================
-- Step 2: Get Role IDs and Department ID
-- ============================================
DECLARE @MakerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker');
DECLARE @CheckerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker');
DECLARE @EducationDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');

-- Validate required data exists
IF @MakerRoleID IS NULL
BEGIN
    PRINT 'ERROR: Department Maker role not found. Please run user-management-seed-data.sql first.';
    RETURN;
END

IF @CheckerRoleID IS NULL
BEGIN
    PRINT 'ERROR: Department Checker role not found. Please run user-management-seed-data.sql first.';
    RETURN;
END

IF @EducationDeptID IS NULL
BEGIN
    PRINT 'ERROR: Education Department not found. Cannot create users.';
    RETURN;
END

-- ============================================
-- Step 3: Create Education Maker User
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_maker')
BEGIN
    DECLARE @MakerRoleIDForMaker INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker');
    DECLARE @EducationDeptIDForMaker INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
    
    IF @MakerRoleIDForMaker IS NOT NULL AND @EducationDeptIDForMaker IS NOT NULL
    BEGIN
        -- Note: This BCrypt hash will be updated via API endpoint /api/UserManagement/fix-all-test-passwords
        -- Or you can use the backend service to generate the correct hash
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
            'edu_maker',
            '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- Placeholder - will be fixed via API
            '9998887770',
            'edu.maker@haryanastatabstract.com',
            'Education Maker',
            @MakerRoleIDForMaker,
            @EducationDeptIDForMaker,
            1
        );
        
        PRINT 'Education Maker user (edu_maker) created successfully.';
        PRINT '  LoginID: edu_maker';
        PRINT '  Password: Admin@123 (needs to be fixed via API endpoint)';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Cannot create edu_maker. Required role or department not found.';
    END
END
ELSE
BEGIN
    PRINT 'Education Maker user (edu_maker) already exists.';
END
GO

-- ============================================
-- Step 4: Create Education Checker User
-- ============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'edu_check')
BEGIN
    DECLARE @CheckerRoleIDForChecker INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker');
    DECLARE @EducationDeptIDForChecker INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
    
    IF @CheckerRoleIDForChecker IS NOT NULL AND @EducationDeptIDForChecker IS NOT NULL
    BEGIN
        -- Check if there's already a checker for Education Department
        IF EXISTS (
            SELECT * FROM [dbo].[Master_User] 
            WHERE [RoleID] = @CheckerRoleIDForChecker 
            AND [DepartmentID] = @EducationDeptIDForChecker 
            AND [IsActive] = 1
        )
        BEGIN
            PRINT 'WARNING: A Department Checker already exists for Education Department.';
            PRINT '  The filtered unique index allows only one active checker per department.';
            PRINT '  If you want to create edu_check, first deactivate the existing checker.';
        END
        ELSE
        BEGIN
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
                'edu_check',
                '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- Placeholder - will be fixed via API
                '9998887771',
                'edu.checker@haryanastatabstract.com',
                'Education Checker',
                @CheckerRoleIDForChecker,
                @EducationDeptIDForChecker,
                1
            );
            
            PRINT 'Education Checker user (edu_check) created successfully.';
            PRINT '  LoginID: edu_check';
            PRINT '  Password: Admin@123 (needs to be fixed via API endpoint)';
        END
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Cannot create edu_check. Required role or department not found.';
    END
END
ELSE
BEGIN
    PRINT 'Education Checker user (edu_check) already exists.';
END
GO

-- ============================================
-- Step 5: Display Summary
-- ============================================
PRINT '';
PRINT '===========================================';
PRINT 'Education Department Users Created';
PRINT '===========================================';
PRINT '';
PRINT 'Users Created:';
PRINT '1. edu_maker (Department Maker - Education)';
PRINT '2. edu_check (Department Checker - Education)';
PRINT '';
PRINT 'IMPORTANT: Password Setup Required';
PRINT '-----------------------------------';
PRINT 'The password hashes are placeholders. To set the correct password:';
PRINT '';
PRINT 'Option 1: Use API Endpoint (Recommended)';
PRINT '  POST http://localhost:5000/api/UserManagement/fix-all-test-passwords';
PRINT '  This will update ALL test user passwords to Admin@123';
PRINT '';
PRINT 'Option 2: Use Backend Service';
PRINT '  The backend will automatically hash passwords when creating users via API';
PRINT '';
PRINT 'Login Credentials (after password fix):';
PRINT '  LoginID: edu_maker, Password: Admin@123';
PRINT '  LoginID: edu_check, Password: Admin@123';
PRINT '';
GO
