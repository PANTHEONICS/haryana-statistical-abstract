-- ============================================
-- EDUCATION DEPARTMENT - TABLE 6.1
-- Screen Registry Entry
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Check if Education Department exists, if not create it
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Education', 'EDU');
    PRINT 'Created Education Department.';
END
ELSE
BEGIN
    PRINT 'Education Department already exists.';
END
GO

-- Insert Screen Registry Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'ED_TABLE_6_1_INSTITUTIONS')
BEGIN
    DECLARE @EducationDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU');
    
    IF @EducationDeptID IS NOT NULL
    BEGIN
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
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES (
            'ED_TABLE_6_1_INSTITUTIONS',
            'Institutions Management (Table 6.1)',
            'Ed_Table_6_1_Institutions',
            @EducationDeptID,
            '6.1',
            'Statistical Abstract of Haryana 2023-24',
            'Table 6.1',
            'Number of recognised universities/colleges/schools in Haryana',
            1,
            1,
            GETUTCDATE(),
            GETUTCDATE()
        );
        PRINT 'Screen Registry entry created for ED_TABLE_6_1_INSTITUTIONS.';
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Education Department not found. Please create it first.';
    END
END
ELSE
BEGIN
    PRINT 'Screen Registry entry already exists for ED_TABLE_6_1_INSTITUTIONS.';
END
GO
