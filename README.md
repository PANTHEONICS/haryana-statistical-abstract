# Haryana Statistical Abstract Management System

A comprehensive web application for managing and maintaining statistical data for the Haryana Statistical Abstract. This system provides a complete workflow management solution with role-based access control, audit trails, and data management capabilities.

## 🌟 Features

- **Multi-Department Data Management**: Manage statistical data across multiple departments (Area & Population, Education, etc.)
- **Workflow Engine**: Complete workflow management with status tracking and approval processes
- **Role-Based Access Control**: System Admin, DESA Head, Department Maker, Department Checker roles
- **Audit Trail**: Complete audit history for all data changes and workflow actions
- **User Management**: Comprehensive user management with security questions and password management
- **Menu Management**: Dynamic menu system with role-based visibility
- **RESTful API**: Clean, well-documented API with Swagger integration
- **Modern UI**: React-based frontend with responsive design

## 🏗️ Architecture

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **UI Library**: Radix UI + Tailwind CSS
- **Routing**: React Router
- **Charts**: Recharts

## 📋 Prerequisites

- **.NET 8.0 SDK** (for backend)
- **Node.js 18+** and npm (for frontend)
- **SQL Server** (2019 or later)
- **Visual Studio 2022** or **VS Code** (recommended)

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/haryana-stat-abstract.git
cd haryana-stat-abstract
```

### 2. Database Setup

1. Create a SQL Server database:
```sql
CREATE DATABASE HaryanaStatAbstractDb;
```

2. Run the database scripts in order:
   - `backend/database/create-database-script.sql` (if available)
   - Or use Entity Framework migrations (see below)

### 3. Backend Setup

1. Navigate to backend directory:
```bash
cd backend/HaryanaStatAbstract.API
```

2. Configure database connection:
   - Copy `appsettings.Example.json` to `appsettings.json`
   - Update the connection string with your database details

3. Install dependencies and run migrations:
```bash
dotnet restore
dotnet ef database update
```

4. Run the application:
```bash
dotnet run
```

The API will be available at `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### 4. Frontend Setup

1. Navigate to frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Configure API URL (optional, defaults to `http://localhost:5000/api`):
   - Create `.env` file:
   ```env
   VITE_API_BASE_URL=http://localhost:5000/api
   ```

4. Start development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

### 5. Default Login Credentials

After seeding the database, you can use:
- **Login ID**: `admin`
- **Password**: `Admin@123`

⚠️ **Important**: Change default passwords in production!

## 📁 Project Structure

```
Haryana_Stat_Abstract/
├── backend/
│   ├── HaryanaStatAbstract.API/     # Main API project
│   │   ├── Controllers/             # API controllers
│   │   ├── Services/                # Business logic
│   │   ├── Models/                  # Data models
│   │   ├── Data/                    # DbContext and migrations
│   │   └── Middleware/              # Custom middleware
│   ├── database/                    # SQL scripts and migrations
│   └── *.md                         # Documentation
├── frontend/
│   ├── src/
│   │   ├── components/             # React components
│   │   ├── pages/                  # Page components
│   │   ├── services/               # API services
│   │   └── router/                 # Routing configuration
│   └── public/                     # Static assets
├── Resources/                      # PDF resources and extracted data
└── docs/                           # Additional documentation
```

## 🔧 Configuration

### Backend Configuration

Key configuration files:
- `appsettings.json` - Main configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production configuration (not in repo)

**Important Settings:**
- `ConnectionStrings:DefaultConnection` - Database connection string
- `JwtSettings:SecretKey` - JWT signing key (change in production!)
- `Cors:AllowedOrigins` - CORS allowed origins

### Frontend Configuration

- `.env` - Development environment variables
- `.env.production` - Production environment variables (not in repo)

**Environment Variables:**
- `VITE_API_BASE_URL` - Backend API base URL

## 📚 Documentation

- [Deployment Guide](DEPLOYMENT_PLAN.md) - Complete production deployment guide
- [Quick Start Guide](DEPLOYMENT_QUICK_START.md) - Quick deployment reference
- [Backend Documentation](backend/README.md) - Backend API documentation
- [Database Setup](backend/DATABASE_SETUP.md) - Database configuration guide
- [Authentication Setup](backend/AUTHENTICATION_SETUP.md) - Authentication configuration

## 🧪 Testing

### Backend API Testing

```bash
cd backend/HaryanaStatAbstract.API
dotnet test
```

### Frontend Testing

```bash
cd frontend
npm test
```

## 🚢 Deployment

See [DEPLOYMENT_PLAN.md](DEPLOYMENT_PLAN.md) for detailed deployment instructions.

Quick deployment:
- **Windows**: Use `deploy.ps1` script
- **Linux**: Use `deploy.sh` script

## 🔒 Security Considerations

Before deploying to production:

1. **Change JWT Secret Key**: Generate a strong secret key
2. **Update Database Credentials**: Use secure credentials
3. **Configure CORS**: Restrict to your domain
4. **Enable HTTPS**: Use SSL/TLS certificates
5. **Remove Development Endpoints**: Remove or secure admin endpoints
6. **Review User Permissions**: Audit role assignments

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Your Name** - *Initial work*

## 🙏 Acknowledgments

- Haryana DESA (Directorate of Economics and Statistical Analysis)
- All contributors and testers

## 📞 Support

For issues and questions:
- Open an issue on GitHub
- Check the documentation in the `docs/` folder
- Review existing issues and discussions

## 🔄 Version History

- **v1.0.0** - Initial release
  - Multi-department data management
  - Workflow engine
  - User management
  - Audit trails

---

**Note**: This is a government statistical data management system. Ensure compliance with data protection regulations and security policies before deployment.
