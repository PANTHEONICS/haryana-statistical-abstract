# Education Department Users Setup Guide

## Overview
This guide helps you create the Education Department and its Maker/Checker users for testing the Education Table 6.1 screen.

## Step 1: Execute SQL Script

Run the SQL script in SQL Server Management Studio:

```sql
-- Execute this file:
backend/database/education-department-users.sql
```

This will:
- ✅ Create Education Department (Code: EDU)
- ✅ Create `edu_maker` user (Department Maker role)
- ✅ Create `edu_check` user (Department Checker role)

## Step 2: Set Passwords

The SQL script creates users with placeholder password hashes. You need to set the correct password hash for "Admin@123".

### Option 1: Use API Endpoint (Recommended)

After running the SQL script, call this API endpoint to fix all test user passwords:

```bash
# Using PowerShell
Invoke-RestMethod -Uri "http://localhost:5000/api/UserManagement/fix-all-test-passwords" -Method POST -ContentType "application/json"

# Using curl
curl -X POST http://localhost:5000/api/UserManagement/fix-all-test-passwords
```

This endpoint will:
- Find all test users (including `edu_maker` and `edu_check`)
- Update their passwords to "Admin@123" with proper BCrypt hash

### Option 2: Create Users via API

Alternatively, you can create the users directly via the API (which will hash passwords automatically):

```bash
# Create edu_maker
POST http://localhost:5000/api/UserManagement/create
{
  "loginID": "edu_maker",
  "password": "Admin@123",
  "userMobileNo": "9998887770",
  "userEmailID": "edu.maker@haryanastatabstract.com",
  "fullName": "Education Maker",
  "roleID": <Department Maker RoleID>,
  "departmentID": <Education DepartmentID>,
  "isActive": true
}

# Create edu_check
POST http://localhost:5000/api/UserManagement/create
{
  "loginID": "edu_check",
  "password": "Admin@123",
  "userMobileNo": "9998887771",
  "userEmailID": "edu.checker@haryanastatabstract.com",
  "fullName": "Education Checker",
  "roleID": <Department Checker RoleID>,
  "departmentID": <Education DepartmentID>,
  "isActive": true
}
```

## Step 3: Assign Menu to Education Department

After creating the users, you need to assign the Education Table 6.1 menu to the Education Department:

1. Log in as `admin` (System Admin)
2. Navigate to Menu Configuration (`/menu-config`)
3. Find "Institutions Management (Table 6.1)" menu
4. Assign it to Education Department

## Login Credentials

After completing the setup:

| Login ID | Password | Role | Department |
|----------|----------|------|------------|
| `edu_maker` | `Admin@123` | Department Maker | Education |
| `edu_check` | `Admin@123` | Department Checker | Education |

## Verification

1. **Test Login:**
   - Try logging in with `edu_maker` / `Admin@123`
   - Try logging in with `edu_check` / `Admin@123`

2. **Verify Department:**
   ```sql
   SELECT * FROM [dbo].[Mst_Departments] WHERE [DepartmentCode] = 'EDU';
   ```

3. **Verify Users:**
   ```sql
   SELECT 
       u.[LoginID],
       u.[FullName],
       r.[RoleName],
       d.[DepartmentName]
   FROM [dbo].[Master_User] u
   INNER JOIN [dbo].[Mst_Roles] r ON u.[RoleID] = r.[RoleID]
   LEFT JOIN [dbo].[Mst_Departments] d ON u.[DepartmentID] = d.[DepartmentID]
   WHERE u.[LoginID] IN ('edu_maker', 'edu_check');
   ```

## Troubleshooting

### Issue: Password not working
**Solution:** Call the API endpoint `/api/UserManagement/fix-all-test-passwords` to update password hashes.

### Issue: Cannot create edu_check (unique constraint violation)
**Solution:** Only one Department Checker is allowed per department. Check if another checker exists for Education Department and deactivate it first.

### Issue: Users cannot see Education menu
**Solution:** 
1. Ensure menu is assigned to Education Department in Menu Configuration
2. Ensure users have the correct role (Department Maker/Checker)
3. Ensure users are assigned to Education Department

## Next Steps

1. ✅ Execute SQL script
2. ✅ Set passwords via API
3. ✅ Assign menu to Education Department
4. ✅ Test login with both users
5. ✅ Verify access to `/education/table6-1` screen
