-- ============================================
-- SOCIAL SECURITY AND SOCIAL DEFENCE - TABLE 7.6
-- Number of prisoners (class-wise) in Haryana
-- Table Name: SSD_Table_7_6_NoOfPrisoners_Classwise
-- ============================================
-- Source: Statistical Abstract of Haryana 2023-24
-- Screen Header: "Table 7.6 Number of prisoners (class-wise) in Haryana"
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'TABLE 7.6 - SOCIAL SECURITY AND SOCIAL DEFENCE';
PRINT 'Number of prisoners (class-wise) in Haryana';
PRINT '========================================';
PRINT '';

-- Step 1: Create Social Security Defence Department (if not exists)
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD')
BEGIN
    INSERT INTO [dbo].[Mst_Departments] ([DepartmentName], [DepartmentCode])
    VALUES ('Social Security Defence', 'SSD');
    PRINT '✅ Created Social Security Defence Department (Code: SSD).';
END
GO

-- Step 2: Create SSD_Table_7_6_NoOfPrisoners_Classwise Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Year] NVARCHAR(20) NOT NULL,
        [Beg_Convicted] INT NOT NULL DEFAULT 0,
        [Beg_UnderTrial] INT NOT NULL DEFAULT 0,
        [Beg_Civil] INT NOT NULL DEFAULT 0,
        [Adm_Convicted] INT NOT NULL DEFAULT 0,
        [Adm_UnderTrial] INT NOT NULL DEFAULT 0,
        [Adm_Civil] INT NOT NULL DEFAULT 0,
        [Dis_Convicted] INT NOT NULL DEFAULT 0,
        [Dis_UnderTrial] INT NOT NULL DEFAULT 0,
        [Dis_Civil] INT NOT NULL DEFAULT 0,
        [Rem_Convicted] INT NOT NULL DEFAULT 0,
        [Rem_UnderTrial] INT NOT NULL DEFAULT 0,
        [Rem_Civil] INT NOT NULL DEFAULT 0,
        [CreatedBy] INT NULL,
        [CreatedByIPAddress] NVARCHAR(50) NULL,
        [CreatedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedBy] INT NULL,
        [ModifiedByIPAddress] NVARCHAR(50) NULL,
        [ModifiedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX [IX_SSD_Table_7_6_Year] ON [dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise]([Year]);
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Master_User')
    BEGIN
        ALTER TABLE [dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise] ADD CONSTRAINT [FK_SSD_Table_7_6_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
        ALTER TABLE [dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise] ADD CONSTRAINT [FK_SSD_Table_7_6_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    END
    PRINT '✅ Table SSD_Table_7_6_NoOfPrisoners_Classwise created.';
END
GO

-- Step 3: Create SSD Department Users (if not exists)
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

-- Step 4: Screen Registry (Mst_ScreenRegistry)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_ScreenRegistry')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE')
    BEGIN
        DECLARE @SSDDeptID2 INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD');
        IF @SSDDeptID2 IS NOT NULL
        INSERT INTO [dbo].[Mst_ScreenRegistry] ([ScreenCode], [ScreenName], [TableName], [TableNumber], [DepartmentID], [SourceDocument], [SourceTable], [Description], [IsActive], [DisplayOrder], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE', 'Prisoners Class-wise (Table 7.6)', 'SSD_Table_7_6_NoOfPrisoners_Classwise', '7.6', @SSDDeptID2, 'Statistical Abstract of Haryana 2023-24', 'Table 7.6', 'Number of prisoners (class-wise) in Haryana', 1, 6, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 5: Screen Workflow
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_Screen')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE')
    BEGIN
        DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
        DECLARE @DraftStatusID INT = COALESCE((SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft'), 1);
        IF @AdminUserID IS NOT NULL
        INSERT INTO [dbo].[Workflow_Screen] ([ScreenCode], [ScreenName], [TableName], [CurrentStatusID], [CreatedByUserID], [IsActive], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE', 'Prisoners Class-wise (Table 7.6)', 'SSD_Table_7_6_NoOfPrisoners_Classwise', @DraftStatusID, @AdminUserID, 1, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 6: Menu Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/social-security/table7-6')
BEGIN
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription], [CreatedAt], [UpdatedAt])
    VALUES ('Prisoners Class-wise (Table 7.6)', '/social-security/table7-6', 'Shield', NULL, @NextDisplayOrder, 1, 0, 'Number of prisoners (class-wise) in Haryana', GETUTCDATE(), GETUTCDATE());
END
GO

-- Step 7: Department-Menu Mapping
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Department_Menu_Mapping')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Mst_Department_Menu_Mapping] dm INNER JOIN [dbo].[Mst_Departments] d ON dm.[DepartmentID] = d.[DepartmentID] INNER JOIN [dbo].[Mst_Menus] m ON dm.[MenuID] = m.[MenuID] WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-6')
    INSERT INTO [dbo].[Mst_Department_Menu_Mapping] ([DepartmentID], [MenuID], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT d.[DepartmentID], m.[MenuID], 1, GETUTCDATE(), GETUTCDATE()
    FROM [dbo].[Mst_Departments] d, [dbo].[Mst_Menus] m WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-6';
END
GO

-- Step 8: Import Reference Data from PDF
MERGE [dbo].[SSD_Table_7_6_NoOfPrisoners_Classwise] AS target
USING (VALUES
    ('1966', 3534, 2076, 7, 14450, 14760, 60, 14106, 15214, 55, 3878, 1622, 12),
    ('1970', 1756, 964, 6, 7500, 17595, 608, 7389, 17596, 605, 1867, 963, 9),
    ('1980', 1433, 1144, 1, 5964, 23110, 301, 6183, 23109, 287, 1214, 1145, 15),
    ('1990-91', 1481, 2105, 2, 5643, 37900, 1380, 5484, 37771, 1358, 1640, 2234, 24),
    ('2000-01', 3006, 6166, 2, 7537, 60905, 2169, 7824, 60181, 2159, 2719, 6890, 12),
    ('2017-18', 7438, 11636, 47, 5400, 53611, 40, 5771, 53071, 60, 7067, 12176, 27),
    ('2018-19', 7067, 12176, 27, 5400, 55679, 60, 5312, 54668, 6, 7155, 13187, 81),
    ('2019-20', 7236, 13160, 27, 563, 38461, 0, 4461, 36670, 7, 3338, 14951, 20),
    ('2020-21', 3338, 14951, 20, 4500, 39413, 30, 1958, 36127, 9, 5880, 18237, 41),
    ('2021-22', 5880, 18237, 41, 4721, 61733, 212, 4646, 60520, 15, 5955, 19450, 238),
    ('2022-23', 5955, 19450, 238, 5473, 63155, 121, 5135, 62713, 111, 6293, 19892, 248)
) AS source ([Year], [Beg_Convicted], [Beg_UnderTrial], [Beg_Civil], [Adm_Convicted], [Adm_UnderTrial], [Adm_Civil], [Dis_Convicted], [Dis_UnderTrial], [Dis_Civil], [Rem_Convicted], [Rem_UnderTrial], [Rem_Civil])
ON target.[Year] = source.[Year]
WHEN NOT MATCHED BY TARGET THEN INSERT ([Year], [Beg_Convicted], [Beg_UnderTrial], [Beg_Civil], [Adm_Convicted], [Adm_UnderTrial], [Adm_Civil], [Dis_Convicted], [Dis_UnderTrial], [Dis_Civil], [Rem_Convicted], [Rem_UnderTrial], [Rem_Civil])
VALUES (source.[Year], source.[Beg_Convicted], source.[Beg_UnderTrial], source.[Beg_Civil], source.[Adm_Convicted], source.[Adm_UnderTrial], source.[Adm_Civil], source.[Dis_Convicted], source.[Dis_UnderTrial], source.[Dis_Civil], source.[Rem_Convicted], source.[Rem_UnderTrial], source.[Rem_Civil]);
GO

PRINT 'TABLE 7.6 SETUP COMPLETE';
GO
