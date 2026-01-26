# User Management & Authentication Module - Setup Guide

## Overview
This guide explains how to set up and use the User Management & Authentication module that has been integrated into the Haryana Stat Abstract application.

## Backend Setup

### 1. Database Setup

#### Option A: Using SQL Server (Recommended for Production)
1. Open SQL Server Management Studio (SSMS) or use your preferred SQL Server client
2. Execute the database schema script:
   ```sql
   -- Run the script located at:
   backend/database/user-management-schema.sql
   ```
3. This will create the following tables:
   - `Users` - User accounts
   - `Roles` - User roles (Admin, User, Viewer)
   - `UserRoles` - Many-to-many relationship between users and roles
   - `RefreshTokens` - JWT refresh tokens for token management

#### Option B: Using Entity Framework Migrations
1. Navigate to the backend project directory:
   ```bash
   cd backend/HaryanaStatAbstract.API
   ```
2. Create a migration:
   ```bash
   dotnet ef migrations add AddUserManagementAndAuthentication
   ```
3. Apply the migration:
   ```bash
   dotnet ef database update
   ```

### 2. Configuration

#### JWT Settings
Update `appsettings.json` or `appsettings.Development.json` with your JWT configuration:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "HaryanaStatAbstractAPI",
    "Audience": "HaryanaStatAbstractClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Important:** Change the `SecretKey` to a secure random string in production!

### 3. Install Dependencies

The following NuGet packages have been added:
- `Microsoft.AspNetCore.Authentication.JwtBearer` (v8.0.0)
- `BCrypt.Net-Next` (v4.0.3)
- `System.IdentityModel.Tokens.Jwt` (v7.0.3)

Install them if needed:
```bash
dotnet restore
```

### 4. Run the Application

```bash
cd backend/HaryanaStatAbstract.API
dotnet run
```

The API will be available at `http://localhost:5000` (or the port configured in `launchSettings.json`).

## Frontend Setup

### 1. Install Dependencies

All required dependencies are already included in `package.json`. If needed, install them:

```bash
cd frontend
npm install
```

### 2. Environment Variables

Create a `.env` file in the `frontend` directory (optional):

```env
VITE_API_BASE_URL=http://localhost:5000/api
```

If not set, it defaults to `http://localhost:5000/api`.

### 3. Run the Frontend

```bash
cd frontend
npm run dev
```

The frontend will be available at `http://localhost:5173` (Vite default port).

## Default Admin User

After running the database schema script, a default admin user is created:

- **Username:** `admin`
- **Email:** `admin@haryanastatabstract.com`
- **Password:** You need to set this manually (see below)

### Setting Admin Password

The SQL script includes a placeholder password hash. To set a real password:

1. Register a new admin user through the API/UI, OR
2. Use the following C# code to generate a BCrypt hash and update the database:

```csharp
var passwordHash = BCrypt.Net.BCrypt.HashPassword("YourPassword123!");
// Update the Users table with this hash
```

Or use the registration endpoint to create the first admin user.

## API Endpoints

### Authentication Endpoints

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login with username/email and password
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/revoke` - Revoke refresh token (requires authentication)
- `POST /api/auth/logout` - Logout current user (requires authentication)
- `GET /api/auth/me` - Get current user information (requires authentication)

### User Management Endpoints (Admin Only)

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user (soft delete)
- `GET /api/users/roles` - Get all roles
- `POST /api/users/{userId}/roles/{roleId}` - Assign role to user
- `DELETE /api/users/{userId}/roles/{roleId}` - Remove role from user

## Frontend Routes

### Public Routes
- `/login` - Login page
- `/register` - Registration page

### Protected Routes (Require Authentication)
- `/` - Dashboard
- `/data` - Data Management
- `/census` - Census
- `/detail` - Detail View
- `/workflow` - Workflow
- `/board` - Board View
- `/analytics` - Analytics

### Admin-Only Routes
- `/users` - User Management (requires Admin role)

## Features

### Authentication
- JWT-based authentication with access and refresh tokens
- Password hashing using BCrypt
- Token expiration and refresh mechanism
- Secure logout with token revocation

### User Management
- User registration and login
- Role-based access control (RBAC)
- User profile management
- Admin user management interface
- Role assignment and removal

### Security Features
- Password validation (minimum 6 characters)
- Email validation
- Username/email uniqueness checks
- Token-based session management
- Protected API endpoints
- CORS configuration

## Testing

### Using Swagger UI
1. Navigate to `http://localhost:5000` (or your API base URL)
2. Use the Swagger UI to test authentication endpoints
3. Click "Authorize" and enter the JWT token after logging in

### Manual Testing
1. Register a new user via `/api/auth/register`
2. Login via `/api/auth/login` to get tokens
3. Use the access token in the `Authorization` header: `Bearer <token>`
4. Test protected endpoints

## Troubleshooting

### Common Issues

1. **CORS Errors**
   - Ensure the frontend URL is added to CORS policy in `Program.cs`
   - Check that credentials are allowed

2. **Token Expiration**
   - Access tokens expire after 60 minutes (configurable)
   - Use the refresh token endpoint to get a new access token

3. **Database Connection**
   - Verify SQL Server connection string in `appsettings.json`
   - Ensure SQL Server is running and accessible

4. **BCrypt Password Verification**
   - Ensure BCrypt.Net-Next package is installed
   - Passwords are hashed using BCrypt with default work factor

## Next Steps

1. **Production Deployment:**
   - Change JWT secret key to a secure random value
   - Use environment variables for sensitive configuration
   - Enable HTTPS
   - Configure proper CORS policies
   - Set up database backups

2. **Additional Features (Optional):**
   - Email verification
   - Password reset functionality
   - Two-factor authentication (2FA)
   - Account lockout after failed login attempts
   - Audit logging for user actions

## Support

For issues or questions, refer to the main project README or contact the development team.
