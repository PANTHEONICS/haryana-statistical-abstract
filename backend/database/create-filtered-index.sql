-- ============================================
-- Create Filtered Unique Index for Department Checker
-- Only ONE Department Checker per Department
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Set required options for filtered index
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Drop index if exists
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Master_User_UniqueCheckerPerDepartment' AND object_id = OBJECT_ID('Master_User'))
BEGIN
    DROP INDEX IX_Master_User_UniqueCheckerPerDepartment ON Master_User;
    PRINT 'Existing index dropped.';
END
GO

-- Get Department Checker RoleID
DECLARE @CheckerRoleID INT;
SELECT @CheckerRoleID = RoleID FROM Mst_Roles WHERE RoleName = 'Department Checker';

IF @CheckerRoleID IS NULL
BEGIN
    PRINT 'ERROR: Department Checker role not found!';
    RETURN;
END

-- Create filtered unique index using dynamic SQL
DECLARE @sql NVARCHAR(MAX);
SET @sql = N'
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
CREATE UNIQUE NONCLUSTERED INDEX IX_Master_User_UniqueCheckerPerDepartment
ON Master_User(DepartmentID, RoleID)
WHERE RoleID = ' + CAST(@CheckerRoleID AS NVARCHAR(10)) + N'
  AND DepartmentID IS NOT NULL 
  AND IsActive = 1;
';

EXEC sp_executesql @sql;
PRINT 'Filtered unique index IX_Master_User_UniqueCheckerPerDepartment created successfully.';
PRINT 'This ensures only ONE active Department Checker per Department.';
GO
