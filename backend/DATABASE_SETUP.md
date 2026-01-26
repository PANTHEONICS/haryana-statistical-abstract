# Database Setup Guide

## Overview
This guide explains how to set up the SQL Server database for the Haryana Statistical Abstract application.

## Database Configuration
- **Server**: `KAPILP\SQLEXPRESS`
- **Database**: `HaryanaStatAbstractDb`
- **Authentication**: Windows Authentication (Integrated Security)
- **Connection**: Trusted Connection (Windows Authentication)

## Prerequisites
1. SQL Server Express installed and running on `KAPILP\SQLEXPRESS`
2. .NET 8.0 SDK installed
3. Entity Framework Core Tools installed globally:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

## Database Setup - Option 1: Using SQL Script (Recommended)

### Step 1: Create Database and Tables
1. Open SQL Server Management Studio (SSMS) or use `sqlcmd`
2. Connect to `KAPILP\SQLEXPRESS` using Windows Authentication
3. Execute the database creation script:
   ```sql
   -- Run the script located at:
   backend/database/create-database-script.sql
   ```

This script will:
- Create the `HaryanaStatAbstractDb` database if it doesn't exist
- Create all necessary tables:
  - `census_population` - Census population data
  - `Users` - User accounts
  - `Roles` - User roles (Admin, User, Viewer)
  - `UserRoles` - Many-to-many relationship between users and roles
  - `RefreshTokens` - JWT refresh tokens
- Create indexes, constraints, and triggers
- Set up auto-update triggers for `UpdatedAt` columns

### Step 2: Run the Application
The application will automatically:
- Apply any pending migrations
- Seed initial data (roles and default admin user)

## Database Setup - Option 2: Using Entity Framework Migrations

### Step 1: Ensure Connection String is Correct
Verify the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=KAPILP\\SQLEXPRESS;Database=HaryanaStatAbstractDb;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### Step 2: Create Database and Apply Migrations
Run the following commands from the `backend/HaryanaStatAbstract.API` directory:

```bash
# Navigate to the API project directory
cd backend/HaryanaStatAbstract.API

# Create and apply migrations
dotnet ef database update
```

This will:
- Create the database if it doesn't exist
- Create all tables based on the migrations
- Apply the database schema

### Step 3: Run the Application
Start the application:
```bash
dotnet run
```

The application will automatically seed initial data on first run.

## Verification

### Verify Database Connection
1. Open SSMS and connect to `KAPILP\SQLEXPRESS`
2. Expand `Databases` and verify `HaryanaStatAbstractDb` exists
3. Expand the database and verify all tables are created:
   - `census_population`
   - `Users`
   - `Roles`
   - `UserRoles`
   - `RefreshTokens`

### Verify Tables Have Data
After running the application for the first time:
1. Check the `Roles` table - should have 3 rows (Admin, User, Viewer)
2. Check the `Users` table - should have 1 row (admin user)
3. Check the `UserRoles` table - should have 1 row (admin user with Admin role)
4. Check the `census_population` table - should have 12 rows (1901-2011 census data)

### Test Application
1. Start the application:
   ```bash
   cd backend/HaryanaStatAbstract.API
   dotnet run
   ```

2. Access Swagger UI at `http://localhost:5000` or `https://localhost:5001`

3. Test the authentication endpoint:
   - POST `/api/auth/register` - Register a new user
   - POST `/api/auth/login` - Login with credentials
     - Default admin: `admin` / `Admin@123`

4. Test the Census Population endpoints (requires authentication):
   - GET `/api/censuspopulation` - Get all census data
   - GET `/api/censuspopulation/{year}` - Get census data for a specific year

## Troubleshooting

### Connection Issues
If you encounter connection issues:

1. **Verify SQL Server is running:**
   - Open Services (`services.msc`)
   - Verify `SQL Server (SQLEXPRESS)` is running

2. **Verify SQL Server is configured to accept connections:**
   - Open SQL Server Configuration Manager
   - Enable TCP/IP protocol
   - Restart SQL Server service

3. **Verify Windows Authentication:**
   - Ensure you're using the correct Windows account
   - Verify the account has permissions on the SQL Server instance

4. **Check firewall:**
   - Ensure Windows Firewall allows SQL Server connections
   - Default SQL Server port: 1433

### Database Already Exists Error
If the database already exists:
1. Option 1: Drop and recreate (⚠️ This will delete all data):
   ```sql
   USE master;
   DROP DATABASE IF EXISTS HaryanaStatAbstractDb;
   ```
   Then run the setup script again.

2. Option 2: Use migrations to update existing database:
   ```bash
   dotnet ef database update
   ```

### Migration Issues
If migrations fail:
1. Check connection string is correct
2. Verify database exists
3. Check user permissions
4. Try removing and recreating migrations:
   ```bash
   dotnet ef migrations remove
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Default Admin Credentials
- **Username**: `admin`
- **Password**: `Admin@123`
- **Email**: `admin@haryanastatabstract.com`

⚠️ **Important**: Change the default admin password after first login in production!

## Database Schema

### Tables Overview

1. **census_population**
   - Stores census population data for Haryana (1901-2011)
   - Primary Key: `census_year`
   - Includes check constraints for data integrity

2. **Users**
   - User accounts with authentication information
   - Primary Key: `Id` (Identity)
   - Unique constraints on `Username` and `Email`

3. **Roles**
   - System roles (Admin, User, Viewer)
   - Primary Key: `Id` (Identity)
   - Unique constraint on `Name`

4. **UserRoles**
   - Many-to-many relationship between Users and Roles
   - Composite Primary Key: `UserId`, `RoleId`

5. **RefreshTokens**
   - JWT refresh tokens for token-based authentication
   - Primary Key: `Id` (Identity)
   - Foreign Key: `UserId`

## Next Steps

After database setup:
1. Run the application and verify all endpoints work
2. Test authentication flow
3. Test CRUD operations on census population data
4. Integrate with the frontend application

For more information, refer to:
- `backend/README.md` - Backend API documentation
- `backend/AUTHENTICATION_SETUP.md` - Authentication setup guide
- `backend/QUICK_START.md` - Quick start guide
