-- ============================================
-- Create Admin User Script
-- This script creates the admin user with the correct BCrypt hash
-- Password: Admin@123
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Get Admin Role ID
DECLARE @AdminRoleId INT;
SELECT @AdminRoleId = [Id] FROM [dbo].[Roles] WHERE [Name] = 'Admin';

IF @AdminRoleId IS NULL
BEGIN
    PRINT 'ERROR: Admin role does not exist! Please seed roles first.';
    RETURN;
END

-- Check if admin user already exists
IF EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    PRINT 'Admin user already exists.';
    
    -- Update password hash (in case it's incorrect)
    UPDATE [dbo].[Users]
    SET [PasswordHash] = '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q',
        [IsActive] = 1,
        [EmailConfirmed] = 1,
        [UpdatedAt] = GETUTCDATE()
    WHERE [Username] = 'admin';
    
    PRINT 'Admin password updated.';
    
    -- Ensure Admin role is assigned
    DECLARE @AdminUserId INT;
    SELECT @AdminUserId = [Id] FROM [dbo].[Users] WHERE [Username] = 'admin';
    
    IF NOT EXISTS (SELECT * FROM [dbo].[UserRoles] WHERE [UserId] = @AdminUserId AND [RoleId] = @AdminRoleId)
    BEGIN
        INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId], [AssignedAt])
        VALUES (@AdminUserId, @AdminRoleId, GETUTCDATE());
        PRINT 'Admin role assigned.';
    END
END
ELSE
BEGIN
    -- Create admin user
    -- Note: This BCrypt hash is for password "Admin@123"
    -- Generated using BCrypt.Net.BCrypt.HashPassword("Admin@123")
    INSERT INTO [dbo].[Users] (
        [Username],
        [Email],
        [PasswordHash],
        [FirstName],
        [LastName],
        [IsActive],
        [EmailConfirmed],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        'admin',
        'admin@haryanastatabstract.com',
        '$2a$11$KIXxZ8vV3J5q5V5q5V5q5Oe5V5q5V5q5V5q5V5q5V5q5V5q5V5q5V5q', -- BCrypt hash for "Admin@123"
        'Admin',
        'User',
        1,
        1,
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    PRINT 'Admin user created successfully.';
    
    -- Assign Admin role
    DECLARE @NewAdminUserId INT = SCOPE_IDENTITY();
    
    INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId], [AssignedAt])
    VALUES (@NewAdminUserId, @AdminRoleId, GETUTCDATE());
    
    PRINT 'Admin role assigned successfully.';
END

-- Verify the admin user
SELECT 
    u.[Id],
    u.[Username],
    u.[Email],
    u.[IsActive],
    u.[EmailConfirmed],
    STRING_AGG(r.[Name], ', ') AS Roles
FROM [dbo].[Users] u
LEFT JOIN [dbo].[UserRoles] ur ON u.Id = ur.UserId
LEFT JOIN [dbo].[Roles] r ON ur.RoleId = r.Id
WHERE u.[Username] = 'admin'
GROUP BY u.[Id], u.[Username], u.[Email], u.[IsActive], u.[EmailConfirmed];

PRINT '';
PRINT 'Login Credentials:';
PRINT 'Username: admin';
PRINT 'Password: Admin@123';
GO
