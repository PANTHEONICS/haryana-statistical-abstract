-- ============================================
-- SOCIAL SECURITY AND SOCIAL DEFENCE - TABLE 7.8
-- Progress of Jail Industry in Haryana (Value of goods produced)
-- Table Name: SSD_Table_7_8_Jail_Industry_Production_Progress
-- ============================================
-- Source: Statistical Abstract of Haryana 2023-24
-- Screen: Table 7.8 Progress of Jail Industry in Haryana - Value of goods produced
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'TABLE 7.8 - SOCIAL SECURITY AND SOCIAL DEFENCE';
PRINT 'Jail Industry Production Progress';
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

-- Step 2: Create SSD_Table_7_8_Jail_Industry_Production_Progress Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Year] NVARCHAR(20) NOT NULL,
        [Carpentry] BIGINT NOT NULL DEFAULT 0,
        [Textile] BIGINT NOT NULL DEFAULT 0,
        [Leather] BIGINT NOT NULL DEFAULT 0,
        [Durries] BIGINT NOT NULL DEFAULT 0,
        [Tailoring] BIGINT NOT NULL DEFAULT 0,
        [Munj] BIGINT NOT NULL DEFAULT 0,
        [Chicks] BIGINT NOT NULL DEFAULT 0,
        [Oil_OilCake] BIGINT NOT NULL DEFAULT 0,
        [Tents] BIGINT NOT NULL DEFAULT 0,
        [Blankets] BIGINT NOT NULL DEFAULT 0,
        [Smithy] BIGINT NOT NULL DEFAULT 0,
        [Niwar_Tapes] BIGINT NOT NULL DEFAULT 0,
        [Misc] BIGINT NOT NULL DEFAULT 0,
        [Total] BIGINT NOT NULL DEFAULT 0,
        [CreatedBy] INT NULL,
        [CreatedByIPAddress] NVARCHAR(50) NULL,
        [CreatedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedBy] INT NULL,
        [ModifiedByIPAddress] NVARCHAR(50) NULL,
        [ModifiedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX [IX_SSD_Table_7_8_Year] ON [dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress]([Year]);
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Master_User')
    BEGIN
        ALTER TABLE [dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress] ADD CONSTRAINT [FK_SSD_Table_7_8_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
        ALTER TABLE [dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress] ADD CONSTRAINT [FK_SSD_Table_7_8_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    END
    PRINT '✅ Table SSD_Table_7_8_Jail_Industry_Production_Progress created.';
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
    IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS')
    BEGIN
        DECLARE @SSDDeptID2 INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD');
        IF @SSDDeptID2 IS NOT NULL
        INSERT INTO [dbo].[Mst_ScreenRegistry] ([ScreenCode], [ScreenName], [TableName], [TableNumber], [DepartmentID], [SourceDocument], [SourceTable], [Description], [IsActive], [DisplayOrder], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS', 'Jail Industry Production Progress (Table 7.8)', 'SSD_Table_7_8_Jail_Industry_Production_Progress', '7.8', @SSDDeptID2, 'Statistical Abstract of Haryana 2023-24', 'Table 7.8', 'Progress of Jail Industry in Haryana - Value of goods produced', 1, 8, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 5: Screen Workflow
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_Screen')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS')
    BEGIN
        DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
        DECLARE @DraftStatusID INT = COALESCE((SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft'), 1);
        IF @AdminUserID IS NOT NULL
        INSERT INTO [dbo].[Workflow_Screen] ([ScreenCode], [ScreenName], [TableName], [CurrentStatusID], [CreatedByUserID], [IsActive], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS', 'Jail Industry Production Progress (Table 7.8)', 'SSD_Table_7_8_Jail_Industry_Production_Progress', @DraftStatusID, @AdminUserID, 1, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 6: Menu Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/social-security/table7-8')
BEGIN
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription], [CreatedAt], [UpdatedAt])
    VALUES ('Jail Industry Production (Table 7.8)', '/social-security/table7-8', 'Shield', NULL, @NextDisplayOrder, 1, 0, 'Progress of Jail Industry in Haryana - Value of goods produced', GETUTCDATE(), GETUTCDATE());
END
GO

-- Step 7: Department-Menu Mapping
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Department_Menu_Mapping')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Mst_Department_Menu_Mapping] dm INNER JOIN [dbo].[Mst_Departments] d ON dm.[DepartmentID] = d.[DepartmentID] INNER JOIN [dbo].[Mst_Menus] m ON dm.[MenuID] = m.[MenuID] WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-8')
    INSERT INTO [dbo].[Mst_Department_Menu_Mapping] ([DepartmentID], [MenuID], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT d.[DepartmentID], m.[MenuID], 1, GETUTCDATE(), GETUTCDATE()
    FROM [dbo].[Mst_Departments] d, [dbo].[Mst_Menus] m WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-8';
END
GO

-- Step 8: Import Reference Data from PDF
MERGE [dbo].[SSD_Table_7_8_Jail_Industry_Production_Progress] AS target
USING (VALUES
    ('1966', 292469, 8681, 4999, 42941, 8551, 29431, 11563, 35467, 608772, 170597, 12663, 36792, 38546, 1362389),
    ('1970', 681781, 195746, 9723, 67295, 53397, 53181, 18205, 80955, 186510, 49891, 46201, 48618, 109900, 1704905),
    ('1980', 735280, 317582, 11078, 21563, 82547, 29863, 29845, 2991, 1451182, 0, 236522, 215850, 126325, 3546814),
    ('1990-91', 519302, 1541223, 59240, 17258, 182539, 0, 13250, 0, 3071052, 0, 404079, 481458, 648265, 7221840),
    ('2000-01', 245819, 715578, 61855, 2100, 1320165, 0, 0, 0, 5190005, 0, 230918, 718706, 762852, 10836007),
    ('2010-11', 501715, 166083, 0, 76199, 661193, 0, 0, 0, 1128344, 30264, 7177, 120532, 125103, 4223245),
    ('2018-19', 1581589, 55317, 0, 0, 496215, 0, 0, 10869302, 151850, 0, 2295, 10646, 374477, 16563973),
    ('2019-20', 2429389, 117455, 0, 0, 835217, 0, 0, 17198289, 131380, 0, 621250, 0, 11220475, 34486046),
    ('2020-21', 154378, 47476, 0, 0, 388993, 0, 0, 17743961, 537469, 0, 116704, 0, 409231, 22090610),
    ('2021-22', 1395673, 74390, 0, 0, 1160290, 0, 0, 16289200, 1676705, 0, 2750, 0, 1798392, 24626911),
    ('2022-23', 3468614, 382723, 0, 0, 847560, 0, 0, 24552762, 19927519, 0, 0, 0, 3022559, 54535790),
    ('2023-24(P)', 8559921, 546354, 0, 9000, 1130537, 0, 0, 31088435, 14546967, 0, 52595, 0, 2520639, 61647728)
) AS source ([Year], [Carpentry], [Textile], [Leather], [Durries], [Tailoring], [Munj], [Chicks], [Oil_OilCake], [Tents], [Blankets], [Smithy], [Niwar_Tapes], [Misc], [Total])
ON target.[Year] = source.[Year]
WHEN NOT MATCHED BY TARGET THEN INSERT ([Year], [Carpentry], [Textile], [Leather], [Durries], [Tailoring], [Munj], [Chicks], [Oil_OilCake], [Tents], [Blankets], [Smithy], [Niwar_Tapes], [Misc], [Total])
VALUES (source.[Year], source.[Carpentry], source.[Textile], source.[Leather], source.[Durries], source.[Tailoring], source.[Munj], source.[Chicks], source.[Oil_OilCake], source.[Tents], source.[Blankets], source.[Smithy], source.[Niwar_Tapes], source.[Misc], source.[Total]);
GO

PRINT 'TABLE 7.8 SETUP COMPLETE';
GO
