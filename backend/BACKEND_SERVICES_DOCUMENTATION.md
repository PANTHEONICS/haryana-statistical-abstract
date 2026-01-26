# Backend Services Documentation

## Overview

This document lists all backend services, APIs, and dependencies that run in the background to support the frontend application.

---

## 🚀 Core Backend Service

### **HaryanaStatAbstract.API** (.NET 8 Web API)
- **Port**: `5000` (HTTP) / `5001` (HTTPS)
- **Base URL**: `http://localhost:5000/api`
- **Swagger UI**: `http://localhost:5000/swagger`
- **Health Check**: `http://localhost:5000/health`

---

## 📊 Database Service

### **SQL Server Database**
- **Server**: `KAPILP\SQLEXPRESS`
- **Database**: `HaryanaStatAbstractDb`
- **Authentication**: Windows Authentication (Integrated Security)
- **Connection String**: Configured in `appsettings.json`

**Tables**:
- `census_population` - Census population data
- `Master_User` - User accounts
- `Mst_Roles` - User roles
- `Mst_Departments` - Departments
- `Mst_SecurityQuestions` - Security questions
- `Mst_Menus` - Menu items
- `Mst_Department_Menu_Mapping` - Department-menu assignments
- `Mst_Role_Menu_Mapping` - Role-menu assignments
- `Mst_WorkflowStatus` - Workflow statuses
- `Screen_Workflow` - Screen-level workflow tracking
- `Workflow_AuditHistory` - Workflow audit trail

---

## 🔧 Registered Services (Dependency Injection)

### 1. **CensusPopulationService**
- **Interface**: `ICensusPopulationService`
- **Lifetime**: Scoped (per request)
- **Purpose**: Manages census population CRUD operations
- **Used by**: `CensusPopulationController`

### 2. **AuthService**
- **Interface**: `IAuthService`
- **Lifetime**: Scoped
- **Purpose**: Handles authentication (login, register, token refresh)
- **Used by**: `AuthController`

### 3. **UserManagementService**
- **Interface**: `IUserManagementService`
- **Lifetime**: Scoped
- **Purpose**: Manages users, roles, departments, security questions
- **Used by**: `UserManagementController`

### 4. **MenuService**
- **Interface**: `IMenuService`
- **Lifetime**: Scoped
- **Purpose**: Manages menus, menu access control, department-menu mappings
- **Used by**: `MenuController`

### 5. **WorkflowService**
- **Interface**: `IWorkflowService`
- **Lifetime**: Scoped
- **Purpose**: Handles workflow state transitions and audit history
- **Used by**: `WorkflowController`

### 6. **ApplicationDbContext**
- **Lifetime**: Scoped
- **Purpose**: Entity Framework Core database context
- **Database**: SQL Server (HaryanaStatAbstractDb)

---

## 🌐 API Controllers & Endpoints

### 1. **AuthController** (`/api/auth`)
**Purpose**: Legacy authentication (may be deprecated in favor of UserManagement)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login with username/email | No |
| POST | `/api/auth/refresh` | Refresh access token | No |
| POST | `/api/auth/revoke` | Revoke refresh token | Yes |
| POST | `/api/auth/logout` | Logout user | Yes |
| GET | `/api/auth/me` | Get current user | Yes |

---

### 2. **UserManagementController** (`/api/UserManagement`)
**Purpose**: User management and authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/UserManagement/login` | Login with LoginID and password | No |
| POST | `/api/UserManagement/users` | Create new user | Yes |
| GET | `/api/UserManagement/users` | Get all users | Yes |
| GET | `/api/UserManagement/users/{id}` | Get user by ID | Yes |
| GET | `/api/UserManagement/me` | Get current logged-in user | Yes |
| GET | `/api/UserManagement/roles` | Get all roles | Yes |
| GET | `/api/UserManagement/departments` | Get all departments | Yes |
| GET | `/api/UserManagement/security-questions` | Get all security questions | Yes |
| POST | `/api/UserManagement/fix-all-test-passwords` | Fix test user passwords (dev only) | No |

---

### 3. **CensusPopulationController** (`/api/v1/CensusPopulation`)
**Purpose**: Census population data management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/v1/CensusPopulation` | Get all census records | Yes |
| GET | `/api/v1/CensusPopulation/{year}` | Get census record by year | Yes |
| GET | `/api/v1/CensusPopulation/range?startYear={start}&endYear={end}` | Get records by year range | Yes |
| POST | `/api/v1/CensusPopulation` | Create new census record | Yes |
| PUT | `/api/v1/CensusPopulation/{id}` | Update census record by ID | Yes |
| DELETE | `/api/v1/CensusPopulation/{id}` | Delete census record by ID | Yes |

---

### 4. **MenuController** (`/api/menu`)
**Purpose**: Menu management and access control

| Method | Endpoint | Description | Auth Required | Role Required |
|--------|----------|-------------|---------------|---------------|
| GET | `/api/menu` | Get all menus | Yes | System Admin |
| GET | `/api/menu/user-menus` | Get user's accessible menus | Yes | - |
| GET | `/api/menu/check-access?menuPath={path}` | Check if user can access menu | Yes | - |
| GET | `/api/menu/department-mappings` | Get all department-menu mappings | Yes | System Admin |
| GET | `/api/menu/department/{departmentId}` | Get menus for a department | Yes | System Admin |
| POST | `/api/menu/assign-to-department` | Assign menus to department | Yes | System Admin |

---

### 5. **WorkflowController** (`/api/workflow`)
**Purpose**: Workflow engine for approval processes

#### Record-Level Workflow

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/workflow/execute` | Execute workflow action on a record | Yes |
| GET | `/api/workflow/history/{tableName}/{recordId}` | Get audit history for a record | Yes |
| GET | `/api/workflow/status/{tableName}/{recordId}` | Get current status for a record | Yes |
| GET | `/api/workflow/statuses` | Get all workflow statuses | Yes |

#### Screen-Level Workflow

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/workflow/screen/execute` | Execute workflow action on a screen | Yes |
| GET | `/api/workflow/screen/history/{screenCode}` | Get audit history for a screen | Yes |
| GET | `/api/workflow/screen/status/{screenCode}` | Get current status for a screen | Yes |

**Workflow Actions**:
- `SubmitToChecker` - Submit to checker (Status 1 → 2)
- `CheckerReject` - Checker rejects (Status 2 → 3)
- `CheckerApprove` - Checker approves (Status 2 → 4)
- `HeadReject` - DESA Head rejects (Status 4 → 5)
- `HeadApprove` - DESA Head approves (Status 4 → 6)

---

## 🔐 Authentication & Authorization

### JWT Authentication
- **Scheme**: JWT Bearer Token
- **Configuration**: `appsettings.json` → `JwtSettings`
- **Secret Key**: Configured in `JwtSettings.SecretKey`
- **Issuer**: `HaryanaStatAbstractAPI`
- **Audience**: `HaryanaStatAbstractClient`
- **Token Expiration**: 60 minutes (configurable)
- **Refresh Token Expiration**: 7 days (configurable)

### Authorization Policies
- **Default**: All authenticated users
- **System Admin**: Required for admin-only endpoints
- **Role-Based**: Based on user roles (System Admin, DESA Head, Department Maker, Department Checker)

---

## 🛠️ Middleware Pipeline

### 1. **RequestLoggingMiddleware**
- **Purpose**: Logs all incoming HTTP requests
- **Logging**: Uses Serilog

### 2. **ErrorHandlingMiddleware**
- **Purpose**: Global error handling and exception logging
- **Response**: Returns standardized error responses

### 3. **CORS Middleware**
- **Purpose**: Allows frontend to make requests
- **Allowed Origins**: 
  - `http://localhost:5173` (Vite default)
  - `http://localhost:3000` (React default)
- **Methods**: All HTTP methods
- **Headers**: All headers
- **Credentials**: Enabled

### 4. **Authentication Middleware**
- **Purpose**: Validates JWT tokens
- **Scheme**: JWT Bearer

### 5. **Authorization Middleware**
- **Purpose**: Enforces role-based access control

---

## 📝 Logging

### Serilog Configuration
- **Console Logging**: Enabled
- **File Logging**: Enabled
- **Log Path**: `logs/haryana-stat-abstract-{date}.log`
- **Rolling Interval**: Daily
- **Log Level**: Information (default), Warning (Microsoft/System)

---

## 🔍 Health Checks

### Health Check Endpoint
- **URL**: `http://localhost:5000/health`
- **Purpose**: Monitor API health status
- **Response**: HTTP 200 if healthy

---

## 🗄️ Database Migrations

### Entity Framework Core Migrations
- **Auto-Apply**: Migrations are automatically applied on startup
- **Location**: `backend/HaryanaStatAbstract.API/Data/Migrations/`
- **Seed Data**: Automatically seeded on startup via `SeedData.SeedAsync()`

---

## 🔄 Startup Process

1. **Configure Serilog** - Set up logging
2. **Add Services** - Register all services, controllers, database context
3. **Configure CORS** - Allow frontend origins
4. **Configure JWT** - Set up authentication
5. **Configure Database** - Set up Entity Framework Core
6. **Build App** - Create application instance
7. **Configure Middleware** - Set up request pipeline
8. **Apply Migrations** - Run database migrations
9. **Seed Data** - Populate initial data
10. **Start Server** - Listen on port 5000

---

## 📦 Dependencies

### NuGet Packages
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Microsoft.EntityFrameworkCore.Tools` - EF Core tools
- `Serilog.AspNetCore` - Logging
- `BCrypt.Net-Next` - Password hashing
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI

---

## 🌍 Environment Configuration

### Development
- **Port**: 5000 (HTTP), 5001 (HTTPS)
- **Swagger**: Enabled at root (`/`)
- **CORS**: Allows localhost:5173 and localhost:3000
- **Logging**: Console + File

### Production (Future)
- **HTTPS**: Required
- **Swagger**: Disabled
- **CORS**: Restricted to production frontend URL
- **Logging**: File only (or external logging service)

---

## 🔗 Frontend Dependencies

The frontend depends on these backend services:

1. **UserManagementController** - Login, user management
2. **MenuController** - Menu access control
3. **CensusPopulationController** - Census data CRUD
4. **WorkflowController** - Workflow state management
5. **AuthController** - Legacy authentication (optional)

---

## 📊 Service Status

To check if services are running:

```bash
# Check API health
curl http://localhost:5000/health

# Check Swagger
curl http://localhost:5000/swagger

# Check database connection
# (Check application logs for connection status)
```

---

## 🚨 Troubleshooting

### API Not Responding
1. Check if backend is running on port 5000
2. Check application logs in `logs/` directory
3. Verify database connection in `appsettings.json`
4. Check CORS configuration matches frontend URL

### Database Connection Issues
1. Verify SQL Server is running (`KAPILP\SQLEXPRESS`)
2. Check Windows Authentication permissions
3. Verify database `HaryanaStatAbstractDb` exists
4. Check connection string in `appsettings.json`

### Authentication Issues
1. Verify JWT secret key is configured
2. Check token expiration settings
3. Verify user exists in `Master_User` table
4. Check password hash is correct (BCrypt)

---

## 📚 Related Documentation

- **API Testing Guide**: `backend/API_TESTING_GUIDE.md`
- **Database Setup**: `backend/DATABASE_SETUP.md`
- **Authentication Setup**: `backend/AUTHENTICATION_SETUP.md`
- **Workflow Engine**: `backend/WORKFLOW_ENGINE_IMPLEMENTATION.md`

---

## ✅ Summary

**Backend Services Running**:
1. ✅ .NET 8 Web API (Port 5000)
2. ✅ SQL Server Database (KAPILP\SQLEXPRESS)
3. ✅ JWT Authentication Service
4. ✅ 5 Business Services (Census, Auth, UserManagement, Menu, Workflow)
5. ✅ 5 API Controllers (Auth, UserManagement, Census, Menu, Workflow)
6. ✅ Entity Framework Core (Database ORM)
7. ✅ Serilog (Logging)
8. ✅ Swagger/OpenAPI (API Documentation)

**Total API Endpoints**: ~30+ endpoints across 5 controllers
