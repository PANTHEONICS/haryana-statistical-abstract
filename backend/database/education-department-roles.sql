-- ============================================
-- EDUCATION DEPARTMENT - ROLES
-- Create Education Department specific roles if they don't exist
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Check if Education Department exists
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Education', 'EDU');
    PRINT 'Created Education Department.';
END
GO

-- Get Education Department ID
DECLARE @EducationDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');

IF @EducationDeptID IS NULL
BEGIN
    PRINT 'ERROR: Education Department not found. Cannot create roles.';
    RETURN;
END

-- Check if Education_Maker role exists (Department Maker role for Education)
-- Note: The system uses generic "Department Maker" and "Department Checker" roles
-- Department-specific roles are not needed as the system uses DepartmentID to filter
-- However, if you want department-specific role names, you can create them here

-- For now, we'll just ensure the Education Department exists
-- The existing "Department Maker" and "Department Checker" roles will work with any department

PRINT 'Education Department setup complete.';
PRINT 'Use existing "Department Maker" and "Department Checker" roles with Education Department users.';
GO
