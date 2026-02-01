-- ============================================
-- SOCIAL SECURITY AND SOCIAL DEFENCE - TABLE 7.7
-- Expenditure on maintenance of prisoners in jails/subsidiary jails* in Haryana
-- Table Name: SSD_Table_7_7_PrisonerMaintenance_Expenditure
-- ============================================
-- Source: Statistical Abstract of Haryana 2023-24
-- Screen Header: "Table 7.7 Expenditure on maintenance of prisoners in jails/subsidiary jails* in Haryana"
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '========================================';
PRINT 'TABLE 7.7 - SOCIAL SECURITY AND SOCIAL DEFENCE';
PRINT 'Prisoner Maintenance Expenditure';
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

-- Step 2: Create SSD_Table_7_7_PrisonerMaintenance_Expenditure Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Year] NVARCHAR(20) NOT NULL,
        [Avg_Prisoners] BIGINT NOT NULL DEFAULT 0,
        [Exp_Establishment] BIGINT NOT NULL DEFAULT 0,
        [Exp_Diet] BIGINT NOT NULL DEFAULT 0,
        [Exp_Others] BIGINT NOT NULL DEFAULT 0,
        [Exp_Total] BIGINT NOT NULL DEFAULT 0,
        [Cost_Per_Prisoner] BIGINT NOT NULL DEFAULT 0,
        [CreatedBy] INT NULL,
        [CreatedByIPAddress] NVARCHAR(50) NULL,
        [CreatedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ModifiedBy] INT NULL,
        [ModifiedByIPAddress] NVARCHAR(50) NULL,
        [ModifiedDateTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE UNIQUE INDEX [IX_SSD_Table_7_7_Year] ON [dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure]([Year]);
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Master_User')
    BEGIN
        ALTER TABLE [dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure] ADD CONSTRAINT [FK_SSD_Table_7_7_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
        ALTER TABLE [dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure] ADD CONSTRAINT [FK_SSD_Table_7_7_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION;
    END
    PRINT '✅ Table SSD_Table_7_7_PrisonerMaintenance_Expenditure created.';
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
    IF NOT EXISTS (SELECT * FROM [dbo].[Mst_ScreenRegistry] WHERE [ScreenCode] = 'SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE')
    BEGIN
        DECLARE @SSDDeptID2 INT = (SELECT [DepartmentID] FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'SSD');
        IF @SSDDeptID2 IS NOT NULL
        INSERT INTO [dbo].[Mst_ScreenRegistry] ([ScreenCode], [ScreenName], [TableName], [TableNumber], [DepartmentID], [SourceDocument], [SourceTable], [Description], [IsActive], [DisplayOrder], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE', 'Prisoner Maintenance Expenditure (Table 7.7)', 'SSD_Table_7_7_PrisonerMaintenance_Expenditure', '7.7', @SSDDeptID2, 'Statistical Abstract of Haryana 2023-24', 'Table 7.7', 'Expenditure on maintenance of prisoners in jails/subsidiary jails* in Haryana', 1, 7, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 5: Screen Workflow
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Workflow_Screen')
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[Workflow_Screen] WHERE [ScreenCode] = 'SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE')
    BEGIN
        DECLARE @AdminUserID INT = (SELECT TOP 1 [UserID] FROM [dbo].[Master_User] WHERE [LoginID] = 'admin');
        DECLARE @DraftStatusID INT = COALESCE((SELECT [StatusID] FROM [dbo].[Mst_WorkflowStatus] WHERE [StatusCode] = 'Draft'), 1);
        IF @AdminUserID IS NOT NULL
        INSERT INTO [dbo].[Workflow_Screen] ([ScreenCode], [ScreenName], [TableName], [CurrentStatusID], [CreatedByUserID], [IsActive], [CreatedAt], [UpdatedAt])
        VALUES ('SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE', 'Prisoner Maintenance Expenditure (Table 7.7)', 'SSD_Table_7_7_PrisonerMaintenance_Expenditure', @DraftStatusID, @AdminUserID, 1, GETUTCDATE(), GETUTCDATE());
    END
END
GO

-- Step 6: Menu Entry
IF NOT EXISTS (SELECT * FROM [dbo].[Mst_Menus] WHERE [MenuPath] = '/social-security/table7-7')
BEGIN
    DECLARE @NextDisplayOrder INT = (SELECT ISNULL(MAX([DisplayOrder]), 0) + 1 FROM [dbo].[Mst_Menus]);
    INSERT INTO [dbo].[Mst_Menus] ([MenuName], [MenuPath], [MenuIcon], [ParentMenuID], [DisplayOrder], [IsActive], [IsAdminOnly], [MenuDescription], [CreatedAt], [UpdatedAt])
    VALUES ('Prisoner Maintenance Expenditure (Table 7.7)', '/social-security/table7-7', 'Shield', NULL, @NextDisplayOrder, 1, 0, 'Expenditure on maintenance of prisoners in jails/subsidiary jails* in Haryana', GETUTCDATE(), GETUTCDATE());
END
GO

-- Step 7: Department-Menu Mapping
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mst_Department_Menu_Mapping')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Mst_Department_Menu_Mapping] dm INNER JOIN [dbo].[Mst_Departments] d ON dm.[DepartmentID] = d.[DepartmentID] INNER JOIN [dbo].[Mst_Menus] m ON dm.[MenuID] = m.[MenuID] WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-7')
    INSERT INTO [dbo].[Mst_Department_Menu_Mapping] ([DepartmentID], [MenuID], [IsActive], [CreatedAt], [UpdatedAt])
    SELECT d.[DepartmentID], m.[MenuID], 1, GETUTCDATE(), GETUTCDATE()
    FROM [dbo].[Mst_Departments] d, [dbo].[Mst_Menus] m WHERE d.[DepartmentCode] = 'SSD' AND m.[MenuPath] = '/social-security/table7-7';
END
GO

-- Step 8: Import Reference Data from PDF
MERGE [dbo].[SSD_Table_7_7_PrisonerMaintenance_Expenditure] AS target
USING (VALUES
    ('1966', 2707, 561000, 673000, 628000, 1862000, 688),
    ('1970', 2804, 1410000, 1180000, 662000, 3252000, 1160),
    ('1980', 2474, 4485000, 2452000, 2012000, 8949000, 3617),
    ('1990-91', 3295, 20581000, 11092000, 6269000, 37942000, 11515),
    ('2000-01', 8360, 67699000, 52225000, 28903000, 148827000, 17802),
    ('2010-11', 14178, 538782000, 137590000, 156468000, 832840000, 58741),
    ('2015-16', 17796, 1341163000, 288628000, 157800000, 1787591000, 100449),
    ('2020-21', 20510, 2775009000, 357006000, 28044000, 3160059000, 154074),
    ('2021-22', 24158, 3096124000, 407007000, 0, 3503131000, 145009),
    ('2022-23', 25523, 3503324000, 562765000, 0, 4066089000, 159311),
    ('2023-24 (P)', 25990, 3388420000, 761917603, 407827917, 4558165520, 175382)
) AS source ([Year], [Avg_Prisoners], [Exp_Establishment], [Exp_Diet], [Exp_Others], [Exp_Total], [Cost_Per_Prisoner])
ON target.[Year] = source.[Year]
WHEN NOT MATCHED BY TARGET THEN INSERT ([Year], [Avg_Prisoners], [Exp_Establishment], [Exp_Diet], [Exp_Others], [Exp_Total], [Cost_Per_Prisoner])
VALUES (source.[Year], source.[Avg_Prisoners], source.[Exp_Establishment], source.[Exp_Diet], source.[Exp_Others], source.[Exp_Total], source.[Cost_Per_Prisoner]);
GO

PRINT 'TABLE 7.7 SETUP COMPLETE';
GO
