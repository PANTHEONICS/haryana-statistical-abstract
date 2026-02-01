-- ============================================
-- SOCIAL SECURITY AND SOCIAL DEFENCE - TABLE 7.1
-- Sanctioned Strength of Haryana Police
-- Table Name: SSD_Table_7_1_Sanctioned_strength_Police
-- ============================================
-- Source: Statistical Abstract of Haryana 2023-24
-- Screen Header: "Table 7.1 Information relating to the sanctioned strength of Haryana Police."
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'TABLE 7.1 - SOCIAL SECURITY AND SOCIAL DEFENCE';
PRINT 'Sanctioned Strength of Haryana Police';
PRINT '========================================';
PRINT '';

-- Step 1: Create Social Security Defence Department
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Social Security Defence', 'SSD');
    PRINT '✅ Created Social Security Defence Department (Code: SSD).';
END
GO

-- Step 2: Create SSD_Table_7_1_Sanctioned_strength_Police Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SSD_Table_7_1_Sanctioned_strength_Police]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SSD_Table_7_1_Sanctioned_strength_Police] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Year] INT NOT NULL,
        [DG_ADG_IG_DyIG] INT NOT NULL DEFAULT 0,
        [Asst_IG] INT NOT NULL DEFAULT 0,
        [Superintendents_Addl_Dy_Asst] INT NOT NULL DEFAULT 0,
        [Inspectors_SI_ASI] INT NOT NULL DEFAULT 0,
        [Head_Constables_RC] INT NOT NULL DEFAULT 0,
        [Mounted_Foot_Constables] INT NOT NULL DEFAULT 0,
        [Total] INT NOT NULL DEFAULT 0,
        [CreatedBy] INT NULL,
        [CreatedByIPAddress] NVARCHAR(50) NULL,
        [CreatedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedBy] INT NULL,
        [ModifiedByIPAddress] NVARCHAR(50) NULL,
        [ModifiedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX [IX_SSD_Table_7_1_Year] ON [dbo].[SSD_Table_7_1_Sanctioned_strength_Police]([Year]);
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Master_User')
    BEGIN
        ALTER TABLE [dbo].[SSD_Table_7_1_Sanctioned_strength_Police] ADD CONSTRAINT [FK_SSD_Table_7_1_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
        ALTER TABLE [dbo].[SSD_Table_7_1_Sanctioned_strength_Police] ADD CONSTRAINT [FK_SSD_Table_7_1_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    END
    PRINT '✅ Table SSD_Table_7_1_Sanctioned_strength_Police created.';
END
GO

-- Step 3: Create SSD Department Users (RoleID 3=Maker, 4=Checker)
DECLARE @SSDDeptID INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD');
DECLARE @MakerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Maker');
DECLARE @CheckerRoleID INT = (SELECT [RoleID] FROM [dbo].[Mst_Roles] WHERE [RoleName] = 'Department Checker');

IF @SSDDeptID IS NOT NULL AND @MakerRoleID IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'ssd_maker')
    INSERT INTO [dbo].[Master_User] ([LoginID], [UserPassword], [UserMobileNo], [UserEmailID], [FullName], [RoleID], [DepartmentID], [IsActive])
    VALUES ('ssd_maker', '$2a$11$jqubu2.casSAV5fUEL7ipumZ/gUOgnrkskYTKO0AA.eiC3I6qaAky', '9998887760', 'ssd.maker@haryanastatabstract.com', 'SSD Department Maker', @MakerRoleID, @SSDDeptID, 1);

IF @SSDDeptID IS NOT NULL AND @CheckerRoleID IS NOT NULL AND NOT EXISTS (SELECT * FROM [dbo].[Master_User] WHERE [LoginID] = 'ssd_checker')
    INSERT INTO [dbo].[Master_User] ([LoginID], [UserPassword], [UserMobileNo], [UserEmailID], [FullName], [RoleID], [DepartmentID], [IsActive])
    VALUES ('ssd_checker', '$2a$11$jqubu2.casSAV5fUEL7ipumZ/gUOgnrkskYTKO0AA.eiC3I6qaAky', '9998887761', 'ssd.checker@haryanastatabstract.com', 'SSD Department Checker', @CheckerRoleID, @SSDDeptID, 1);
GO

-- Step 4: Screen Registry (Mst_ScreenRegistry - required for department-wise DESA Head dashboard)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_ScreenRegistry')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE')
    BEGIN
        DECLARE @SSDDeptID2 INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD');
        IF @SSDDeptID2 IS NOT NULL
        INSERT INTO [dbo].[Mst_ScreenRegistry] ([ScreenCode], [ScreenName], [TableName], [TableNumber], [DepartmentID], [SourceDocument], [SourceTable], [Description], [IsActive], [DisplayOrder], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE', 'Sanctioned Strength of Police Management (Table 7.1)', 'SSD_Table_7_1_Sanctioned_strength_Police', '7.1', @SSDDeptID2, 'Statistical Abstract of Haryana 2023-24', 'Table 7.1', 'Information relating to the sanctioned strength of Haryana Police', 1, 1, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 5: Screen Workflow
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_Screen')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE')
    BEGIN
        DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
        DECLARE @DraftStatusID INT = COALESCE((SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft'), 1);
        IF @AdminUserID IS NOT NULL
        INSERT INTO [dbo].[Workflow_Screen] ([ScreenCode], [ScreenName], [TableName], [CurrentStatusID], [CreatedByUserID], [IsActive], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE', 'Sanctioned Strength of Police Management (Table 7.1)', 'SSD_Table_7_1_Sanctioned_strength_Police', @DraftStatusID, @AdminUserID, 1, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 6: Menu Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/social-security/table7-1')
BEGIN
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription], [CreatedAt], [UpdatedAt])
    VALUES ('Sanctioned Strength of Police (Table 7.1)', '/social-security/table7-1', 'Shield', NULL, @NextDisplayOrder, 1, 0, 'Information relating to the sanctioned strength of Haryana Police', GETUTCDATE(), GETUTCDATE());
END
GO

-- Step 7: Department-Menu Mapping
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Department_Menu_Mapping')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Mst_Department_Menu_Mapping] dm INNER JOIN [dbo].[Mst_Departments] d ON dm.[DepartmentID] = d.[DepartmentID] INNER JOIN [dbo].[Mst_Menus] m ON dm.[MenuID] = m.[MenuID] WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-1')
    INSERT INTO [dbo].[Mst_Department_Menu_Mapping] ([DepartmentID], [MenuID], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT d.[DepartmentID], m.[MenuID], 1, GETUTCDATE(), GETUTCDATE()
    FROM [dbo].[Mst_Departments] d, [dbo].[Mst_Menus] m WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-1';
END
GO

-- Step 8: Import Reference Data from PDF
MERGE [dbo].[SSD_Table_7_1_Sanctioned_strength_Police] AS target
USING (VALUES
    (1966, 3, 3, 32, 676, 956, 5715, 7385),
    (1970, 3, 3, 38, 709, 905, 5740, 7398),
    (1980, 10, 4, 110, 1738, 4102, 12267, 18231),
    (1990, 16, 3, 164, 3029, 7084, 18741, 29037),
    (2000, 30, 3, 193, 3795, 8007, 25195, 37223),
    (2010, 46, 4, 274, 7151, 9036, 40094, 56605),
    (2016, 50, 4, 311, 7401, 9230, 39993, 56989),
    (2017, 50, 4, 316, 8154, 10093, 42758, 61375),
    (2018, 50, 4, 329, 8341, 10287, 43363, 62374),
    (2019, 53, 3, 391, 10343, 12320, 47552, 70662),
    (2020, 54, 3, 402, 10754, 12638, 48449, 72300),
    (2021, 54, 3, 415, 10948, 12778, 49195, 73393),
    (2022, 54, 3, 428, 11416, 13067, 49731, 74699),
    (2023, 54, 3, 429, 11250, 12796, 49040, 73572)
) AS source ([Year], [DG_ADG_IG_DyIG], [Asst_IG], [Superintendents_Addl_Dy_Asst], [Inspectors_SI_ASI], [Head_Constables_RC], [Mounted_Foot_Constables], [Total])
ON target.[Year] = source.[Year]
WHEN NOT MATCHED BY TARGET THEN INSERT ([Year], [DG_ADG_IG_DyIG], [Asst_IG], [Superintendents_Addl_Dy_Asst], [Inspectors_SI_ASI], [Head_Constables_RC], [Mounted_Foot_Constables], [Total])
VALUES (source.[Year], source.[DG_ADG_IG_DyIG], source.[Asst_IG], source.[Superintendents_Addl_Dy_Asst], source.[Inspectors_SI_ASI], source.[Head_Constables_RC], source.[Mounted_Foot_Constables], source.[Total]);
GO

PRINT 'TABLE 7.1 SETUP COMPLETE';
GO
