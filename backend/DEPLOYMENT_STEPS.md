# Deployment Steps - User Management Module

## Quick Start Guide

### 1. Execute Database Scripts (5 minutes)

```sql
-- Step 1: Open SQL Server Management Studio
-- Step 2: Connect to KAPILP\SQLEXPRESS using Windows Authentication
-- Step 3: Execute these scripts in order:

-- Script 1: Create Schema
-- File: backend/database/user-management-module-schema.sql
-- This creates: Mst_Roles, Mst_Departments, Mst_SecurityQuestions, Master_User tables

-- Script 2: Seed Data
-- File: backend/database/user-management-seed-data.sql
-- This creates: 4 Roles, 2 Departments, 2 Security Questions, 5 Test Users
```

**Verification:**
```sql
-- Check all tables exist
SELECT COUNT(*) FROM Mst_Roles;        -- Should return 4
SELECT COUNT(*) FROM Mst_Departments;  -- Should return 2
SELECT COUNT(*) FROM Mst_SecurityQuestions; -- Should return 2
SELECT COUNT(*) FROM Master_User;      -- Should return 5
```

### 2. Create Entity Framework Migration (2 minutes)

```bash
cd backend/HaryanaStatAbstract.API
dotnet ef migrations add AddUserManagementModule
dotnet ef database update
```

**If tables already exist (from SQL scripts), mark migration as applied:**
```sql
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('AddUserManagementModule', '8.0.0');
```

### 3. Restart Backend Application (1 minute)

```bash
cd backend/HaryanaStatAbstract.API
dotnet run
```

**Verify:**
- Health check: `GET http://localhost:5000/health` → Should return "Healthy"
- Swagger UI: `http://localhost:5000` → Should show UserManagement endpoints

### 4. Test Login API (2 minutes)

```bash
# Using PowerShell
$body = @{ loginID = "admin"; password = "Admin@123" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/UserManagement/login" -Method POST -Body $body -ContentType "application/json"
```

**Expected Result:**
- Status: 200 OK
- Response contains: accessToken, refreshToken, user information

### 5. Test Frontend (2 minutes)

```bash
# In frontend directory
npm run dev
```

**Navigate to:**
- Login: `http://localhost:5173/um-login`
- Enter: Login ID: `admin`, Password: `Admin@123`
- Should redirect to dashboard showing "Welcome [Name]"

---

## Test Scenarios

### ✅ Scenario 1: Login as System Admin
- Login ID: `admin`
- Password: `Admin@123`
- Expected: Login successful, no department shown

### ✅ Scenario 2: Login as Department Checker
- Login ID: `hfw_check`
- Password: `Admin@123`
- Expected: Login successful, department "Health & Family Welfare" shown

### ✅ Scenario 3: Create User - System Admin
- Role: System Admin
- Department: Should be disabled/not available
- Expected: User created without department

### ✅ Scenario 4: Create User - Department Checker
- Role: Department Checker
- Department: Health & Family Welfare
- Expected: User created (if no checker exists)
- If checker already exists: Error "Only one active Department Checker per department"

### ✅ Scenario 5: Mobile Number Validation
- Enter less than 10 digits: Should be rejected
- Enter more than 10 digits: Should be truncated to 10
- Enter non-numeric: Should be filtered out

---

## All Test Users

| Login ID | Password | Role | Department |
|----------|----------|------|------------|
| admin | Admin@123 | System Admin | None |
| desa_head | Admin@123 | DESA Head | None |
| hfw_maker | Admin@123 | Department Maker | Health & Family Welfare |
| hfw_check | Admin@123 | Department Checker | Health & Family Welfare |
| ap_maker | Admin@123 | Department Maker | Area & Population |

---

## Next Steps After Deployment

1. ✅ Test all login scenarios
2. ✅ Test user creation with different roles
3. ✅ Verify Department Checker constraint works
4. ✅ Test mobile number validation
5. ✅ Verify dashboard shows user info correctly
6. ✅ Test Create User Modal with all validations

---

**Total Deployment Time: ~10 minutes**

All components are ready and tested!
