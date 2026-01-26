-- ============================================
-- Mark Entity Framework Migration as Applied
-- This script marks the InitialCreate migration as already applied
-- Use this if you manually created the database tables
-- ============================================

USE [HaryanaStatAbstractDb];
GO

-- Create the __EFMigrationsHistory table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] NVARCHAR(150) NOT NULL,
        [ProductVersion] NVARCHAR(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT '__EFMigrationsHistory table created.';
END
ELSE
BEGIN
    PRINT '__EFMigrationsHistory table already exists.';
END
GO

-- Mark the InitialCreate migration as applied (if not already marked)
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20260121131321_InitialCreate')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260121131321_InitialCreate', '8.0.0');
    PRINT 'Migration 20260121131321_InitialCreate marked as applied.';
END
ELSE
BEGIN
    PRINT 'Migration 20260121131321_InitialCreate is already marked as applied.';
END
GO

PRINT '';
PRINT 'Migration status updated successfully!';
PRINT 'You can now run the application and it will use the existing database schema.';
GO
