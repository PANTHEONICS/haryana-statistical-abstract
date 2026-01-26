-- ============================================
-- CHECK ALL TRIGGERS IN THE DATABASE
-- Lists all triggers and the tables they are on
-- ============================================

USE [HaryanaStatAbstractDb];
GO

PRINT '========================================';
PRINT 'ALL TRIGGERS IN DATABASE';
PRINT '========================================';
PRINT '';

-- Query to get all triggers with their associated tables
SELECT 
    t.name AS TriggerName,
    OBJECT_NAME(t.parent_id) AS TableName,
    t.is_disabled AS IsDisabled,
    t.create_date AS CreatedDate,
    t.modify_date AS ModifiedDate
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
WHERE o.type = 'U' -- User tables only
ORDER BY TableName, TriggerName;

PRINT '';
PRINT '========================================';
PRINT 'TRIGGER COUNT BY TABLE';
PRINT '========================================';
PRINT '';

-- Count triggers per table
SELECT 
    OBJECT_NAME(t.parent_id) AS TableName,
    COUNT(*) AS TriggerCount
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
WHERE o.type = 'U' -- User tables only
GROUP BY OBJECT_NAME(t.parent_id)
ORDER BY TableName;

PRINT '';
PRINT '========================================';
PRINT 'CHECK COMPLETE';
PRINT '========================================';
GO
