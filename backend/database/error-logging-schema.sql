-- ============================================
-- Error Logging System - Database Schema
-- Purpose: Store runtime errors and exceptions for debugging and monitoring
-- ============================================

USE [HaryanaStatAbstractDb];
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- ============================================
-- Error_Logs Table
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Error_Logs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Error_Logs] (
        [ErrorLogID] BIGINT IDENTITY(1,1) NOT NULL,
        [ErrorLevel] NVARCHAR(50) NOT NULL, -- Error, Warning, Information, Critical
        [ErrorMessage] NVARCHAR(MAX) NOT NULL,
        [ExceptionType] NVARCHAR(200) NULL,
        [StackTrace] NVARCHAR(MAX) NULL,
        [InnerException] NVARCHAR(MAX) NULL,
        [Source] NVARCHAR(500) NULL, -- Controller, Service, Middleware, etc.
        [MethodName] NVARCHAR(200) NULL,
        [RequestPath] NVARCHAR(500) NULL,
        [RequestMethod] NVARCHAR(10) NULL, -- GET, POST, PUT, DELETE
        [UserID] INT NULL, -- FK to Master_User (if user is authenticated)
        [UserLoginID] NVARCHAR(50) NULL, -- LoginID for quick reference
        [IPAddress] NVARCHAR(50) NULL,
        [RequestHeaders] NVARCHAR(MAX) NULL, -- JSON string of request headers
        [RequestBody] NVARCHAR(MAX) NULL, -- Request body (if available)
        [QueryString] NVARCHAR(MAX) NULL,
        [AdditionalData] NVARCHAR(MAX) NULL, -- JSON string for additional context
        [IsResolved] BIT NOT NULL DEFAULT 0,
        [ResolvedBy] INT NULL, -- FK to Master_User
        [ResolvedAt] DATETIME2 NULL,
        [ResolutionNotes] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT [PK_Error_Logs] PRIMARY KEY CLUSTERED ([ErrorLogID] ASC),
        CONSTRAINT [FK_Error_Logs_User] FOREIGN KEY ([UserID]) 
            REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Error_Logs_ResolvedBy] FOREIGN KEY ([ResolvedBy]) 
            REFERENCES [dbo].[Master_User]([UserID]) ON DELETE NO ACTION,
        CONSTRAINT [CK_Error_Logs_ErrorLevel] CHECK ([ErrorLevel] IN ('Error', 'Warning', 'Information', 'Critical', 'Debug'))
    );
    
    -- Indexes for performance
    CREATE INDEX [IX_Error_Logs_CreatedAt] ON [dbo].[Error_Logs]([CreatedAt] DESC);
    CREATE INDEX [IX_Error_Logs_ErrorLevel] ON [dbo].[Error_Logs]([ErrorLevel]);
    CREATE INDEX [IX_Error_Logs_IsResolved] ON [dbo].[Error_Logs]([IsResolved]);
    CREATE INDEX [IX_Error_Logs_UserID] ON [dbo].[Error_Logs]([UserID]);
    CREATE INDEX [IX_Error_Logs_Source] ON [dbo].[Error_Logs]([Source]);
    CREATE INDEX [IX_Error_Logs_RequestPath] ON [dbo].[Error_Logs]([RequestPath]);
    
    PRINT 'Error_Logs table created successfully.';
END
ELSE
BEGIN
    PRINT 'Error_Logs table already exists.';
END
GO

-- ============================================
-- Stored Procedure: Log Error
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_LogError]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_LogError];
END
GO

CREATE PROCEDURE [dbo].[sp_LogError]
    @ErrorLevel NVARCHAR(50),
    @ErrorMessage NVARCHAR(MAX),
    @ExceptionType NVARCHAR(200) = NULL,
    @StackTrace NVARCHAR(MAX) = NULL,
    @InnerException NVARCHAR(MAX) = NULL,
    @Source NVARCHAR(500) = NULL,
    @MethodName NVARCHAR(200) = NULL,
    @RequestPath NVARCHAR(500) = NULL,
    @RequestMethod NVARCHAR(10) = NULL,
    @UserID INT = NULL,
    @UserLoginID NVARCHAR(50) = NULL,
    @IPAddress NVARCHAR(50) = NULL,
    @RequestHeaders NVARCHAR(MAX) = NULL,
    @RequestBody NVARCHAR(MAX) = NULL,
    @QueryString NVARCHAR(MAX) = NULL,
    @AdditionalData NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        INSERT INTO [dbo].[Error_Logs] (
            [ErrorLevel],
            [ErrorMessage],
            [ExceptionType],
            [StackTrace],
            [InnerException],
            [Source],
            [MethodName],
            [RequestPath],
            [RequestMethod],
            [UserID],
            [UserLoginID],
            [IPAddress],
            [RequestHeaders],
            [RequestBody],
            [QueryString],
            [AdditionalData]
        )
        VALUES (
            @ErrorLevel,
            @ErrorMessage,
            @ExceptionType,
            @StackTrace,
            @InnerException,
            @Source,
            @MethodName,
            @RequestPath,
            @RequestMethod,
            @UserID,
            @UserLoginID,
            @IPAddress,
            @RequestHeaders,
            @RequestBody,
            @QueryString,
            @AdditionalData
        );
        
        SELECT SCOPE_IDENTITY() AS ErrorLogID;
    END TRY
    BEGIN CATCH
        -- If error logging fails, log to SQL Server error log
        DECLARE @ErrorMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        RAISERROR('Failed to log error: %s', 16, 1, @ErrorMsg);
    END CATCH
END
GO

PRINT 'Stored procedure sp_LogError created successfully.';
GO

-- ============================================
-- Stored Procedure: Get Error Logs
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetErrorLogs]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_GetErrorLogs];
END
GO

CREATE PROCEDURE [dbo].[sp_GetErrorLogs]
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @ErrorLevel NVARCHAR(50) = NULL,
    @IsResolved BIT = NULL,
    @UserID INT = NULL,
    @Source NVARCHAR(500) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        [ErrorLogID],
        [ErrorLevel],
        [ErrorMessage],
        [ExceptionType],
        [StackTrace],
        [InnerException],
        [Source],
        [MethodName],
        [RequestPath],
        [RequestMethod],
        [UserID],
        [UserLoginID],
        [IPAddress],
        [RequestHeaders],
        [RequestBody],
        [QueryString],
        [AdditionalData],
        [IsResolved],
        [ResolvedBy],
        [ResolvedAt],
        [ResolutionNotes],
        [CreatedAt]
    FROM [dbo].[Error_Logs]
    WHERE 
        (@StartDate IS NULL OR [CreatedAt] >= @StartDate)
        AND (@EndDate IS NULL OR [CreatedAt] <= @EndDate)
        AND (@ErrorLevel IS NULL OR [ErrorLevel] = @ErrorLevel)
        AND (@IsResolved IS NULL OR [IsResolved] = @IsResolved)
        AND (@UserID IS NULL OR [UserID] = @UserID)
        AND (@Source IS NULL OR [Source] LIKE '%' + @Source + '%')
    ORDER BY [CreatedAt] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Error_Logs]
    WHERE 
        (@StartDate IS NULL OR [CreatedAt] >= @StartDate)
        AND (@EndDate IS NULL OR [CreatedAt] <= @EndDate)
        AND (@ErrorLevel IS NULL OR [ErrorLevel] = @ErrorLevel)
        AND (@IsResolved IS NULL OR [IsResolved] = @IsResolved)
        AND (@UserID IS NULL OR [UserID] = @UserID)
        AND (@Source IS NULL OR [Source] LIKE '%' + @Source + '%');
END
GO

PRINT 'Stored procedure sp_GetErrorLogs created successfully.';
GO

-- ============================================
-- Stored Procedure: Mark Error as Resolved
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_MarkErrorResolved]') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE [dbo].[sp_MarkErrorResolved];
END
GO

CREATE PROCEDURE [dbo].[sp_MarkErrorResolved]
    @ErrorLogID BIGINT,
    @ResolvedBy INT,
    @ResolutionNotes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Error_Logs]
    SET 
        [IsResolved] = 1,
        [ResolvedBy] = @ResolvedBy,
        [ResolvedAt] = GETUTCDATE(),
        [ResolutionNotes] = @ResolutionNotes
    WHERE [ErrorLogID] = @ErrorLogID;
    
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Error log with ID %d not found', 16, 1, @ErrorLogID);
    END
END
GO

PRINT 'Stored procedure sp_MarkErrorResolved created successfully.';
GO

PRINT '';
PRINT '==========================================';
PRINT 'Error Logging System Setup Complete!';
PRINT '==========================================';
PRINT '';
PRINT 'Created:';
PRINT '  - Error_Logs table';
PRINT '  - sp_LogError stored procedure';
PRINT '  - sp_GetErrorLogs stored procedure';
PRINT '  - sp_MarkErrorResolved stored procedure';
PRINT '';
GO
