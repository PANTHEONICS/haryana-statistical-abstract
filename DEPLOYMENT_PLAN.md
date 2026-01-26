# Haryana Statistical Abstract - Production Deployment Plan

## 📋 Table of Contents
1. [Prerequisites](#prerequisites)
2. [Server Requirements](#server-requirements)
3. [Deployment Architecture](#deployment-architecture)
4. [Backend Deployment](#backend-deployment)
5. [Frontend Deployment](#frontend-deployment)
6. [Database Configuration](#database-configuration)
7. [Security Configuration](#security-configuration)
8. [Environment Variables](#environment-variables)
9. [Reverse Proxy Setup](#reverse-proxy-setup)
10. [SSL/HTTPS Configuration](#sslhttps-configuration)
11. [Monitoring & Logging](#monitoring--logging)
12. [Post-Deployment Verification](#post-deployment-verification)
13. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software
- **.NET 8.0 Runtime** (for backend)
- **Node.js 18+** (for building frontend)
- **SQL Server** (already deployed on public server)
- **IIS 10+** (Windows) or **Nginx** (Linux)
- **SSL Certificate** (for HTTPS)

### Required Information
- ✅ Database server hostname/IP
- ✅ Database name
- ✅ Database credentials (username/password or Windows Authentication)
- ✅ Production domain name
- ✅ SSL certificate files

---

## Server Requirements

### Minimum Requirements
- **CPU**: 2 cores
- **RAM**: 4 GB
- **Storage**: 20 GB SSD
- **OS**: Windows Server 2019+ or Linux (Ubuntu 20.04+)

### Recommended Requirements
- **CPU**: 4 cores
- **RAM**: 8 GB
- **Storage**: 50 GB SSD
- **OS**: Windows Server 2022 or Linux (Ubuntu 22.04+)

---

## Deployment Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Internet Users                       │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              Reverse Proxy (IIS/Nginx)                  │
│              Port 80/443 (HTTPS)                        │
└──────────────┬──────────────────────┬───────────────────┘
               │                      │
               ▼                      ▼
    ┌──────────────────┐    ┌──────────────────┐
    │   Frontend       │    │   Backend API    │
    │   (Static Files) │    │   (ASP.NET Core) │
    │   Port: 80       │    │   Port: 5000     │
    └──────────────────┘    └────────┬─────────┘
                                     │
                                     ▼
                          ┌──────────────────┐
                          │  SQL Server      │
                          │  (Public Server) │
                          │  Port: 1433      │
                          └──────────────────┘
```

---

## Backend Deployment

### Option 1: Windows Server with IIS (Recommended for Windows)

#### Step 1: Install .NET 8.0 Hosting Bundle
```powershell
# Download and install from:
# https://dotnet.microsoft.com/download/dotnet/8.0
# Install: .NET 8.0 Hosting Bundle (includes runtime + IIS module)
```

#### Step 2: Build the Application
```powershell
cd C:\Nilesh_Code\Haryana_Stat_Abstract\backend\HaryanaStatAbstract.API
dotnet publish -c Release -o C:\inetpub\wwwroot\haryana-api
```

#### Step 3: Create IIS Application Pool
```powershell
# Open IIS Manager
# Right-click "Application Pools" → "Add Application Pool"
# Name: HaryanaStatAbstractAPI
# .NET CLR Version: No Managed Code
# Managed Pipeline Mode: Integrated
```

#### Step 4: Create IIS Website
```powershell
# Right-click "Sites" → "Add Website"
# Site name: HaryanaStatAbstractAPI
# Application pool: HaryanaStatAbstractAPI
# Physical path: C:\inetpub\wwwroot\haryana-api
# Binding: 
#   - Type: http
#   - IP address: All Unassigned
#   - Port: 5000
#   - Host name: (leave empty)
```

#### Step 5: Configure Application Pool
```powershell
# In IIS Manager, select Application Pool → Advanced Settings
# .NET CLR Version: No Managed Code
# Start Mode: AlwaysRunning
# Idle Timeout: 0 (disable)
```

### Option 2: Linux Server with Systemd

#### Step 1: Install .NET 8.0 Runtime
```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0
```

#### Step 2: Build the Application
```bash
cd /path/to/Haryana_Stat_Abstract/backend/HaryanaStatAbstract.API
dotnet publish -c Release -o /var/www/haryana-api
```

#### Step 3: Create Systemd Service
Create file: `/etc/systemd/system/haryana-api.service`
```ini
[Unit]
Description=Haryana Statistical Abstract API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/haryana-api
ExecStart=/usr/bin/dotnet /var/www/haryana-api/HaryanaStatAbstract.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=haryana-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

#### Step 4: Start the Service
```bash
sudo systemctl daemon-reload
sudo systemctl enable haryana-api
sudo systemctl start haryana-api
sudo systemctl status haryana-api
```

---

## Frontend Deployment

### Step 1: Build the Frontend
```bash
cd C:\Nilesh_Code\Haryana_Stat_Abstract\frontend

# Create .env.production file
echo "VITE_API_BASE_URL=https://api.yourdomain.com/api" > .env.production

# Install dependencies (if not already done)
npm install

# Build for production
npm run build
```

### Step 2: Deploy Static Files

#### Option A: IIS (Windows)
```powershell
# Create new website in IIS
# Physical path: C:\inetpub\wwwroot\haryana-frontend
# Copy files from: frontend\dist\* to C:\inetpub\wwwroot\haryana-frontend\
```

#### Option B: Nginx (Linux)
```bash
# Copy files to web directory
sudo cp -r frontend/dist/* /var/www/haryana-frontend/

# Set permissions
sudo chown -R www-data:www-data /var/www/haryana-frontend
sudo chmod -R 755 /var/www/haryana-frontend
```

---

## Database Configuration

### Step 1: Update Connection String

Create `appsettings.Production.json` in backend:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_DB_SERVER;Database=HaryanaStatAbstractDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true;Encrypt=true"
  },
  "JwtSettings": {
    "SecretKey": "CHANGE_THIS_TO_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG!",
    "Issuer": "HaryanaStatAbstractAPI",
    "Audience": "HaryanaStatAbstractClient",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "AllowedHosts": "yourdomain.com,www.yourdomain.com",
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/haryana-api/haryana-stat-abstract-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

### Step 2: Test Database Connection
```bash
# From backend server, test connection
sqlcmd -S YOUR_DB_SERVER -U YOUR_USERNAME -P YOUR_PASSWORD -d HaryanaStatAbstractDb -Q "SELECT @@VERSION"
```

---

## Security Configuration

### Step 1: Update JWT Secret Key
```bash
# Generate a strong secret key (at least 32 characters)
# Use a secure random generator or:
openssl rand -base64 32
```

Update in `appsettings.Production.json`:
```json
"JwtSettings": {
  "SecretKey": "GENERATED_SECRET_KEY_HERE"
}
```

### Step 2: Update CORS Settings

Update `Program.cs` CORS configuration:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Step 3: Disable Swagger in Production

Update `Program.cs`:
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Swagger is automatically disabled in Production
```

---

## Environment Variables

### Backend Environment Variables

#### Windows (IIS)
Set in Application Pool → Advanced Settings → Environment Variables:
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://localhost:5000
```

#### Linux (Systemd)
Already configured in service file above.

### Frontend Environment Variables

Create `.env.production` in frontend directory:
```env
VITE_API_BASE_URL=https://api.yourdomain.com/api
```

---

## Reverse Proxy Setup

### Option 1: IIS as Reverse Proxy (Windows)

#### Install URL Rewrite and ARR
```powershell
# Download and install:
# 1. URL Rewrite Module: https://www.iis.net/downloads/microsoft/url-rewrite
# 2. Application Request Routing: https://www.iis.net/downloads/microsoft/application-request-routing
```

#### Configure web.config for Frontend
Create `web.config` in frontend root:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="React Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/index.html" />
        </rule>
      </rules>
    </rewrite>
  </system.webServer>
</configuration>
```

#### Configure Reverse Proxy for API
In IIS Manager:
1. Select frontend website
2. Install "Application Request Routing" module
3. Create reverse proxy rule:
   - Pattern: `^api/(.*)`
   - Rewrite URL: `http://localhost:5000/api/{R:1}`

### Option 2: Nginx Configuration (Linux)

Create `/etc/nginx/sites-available/haryana-stat-abstract`:
```nginx
# Frontend
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    
    root /var/www/haryana-frontend;
    index index.html;

    # Frontend routes
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API reverse proxy
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    # Static assets caching
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

Enable site:
```bash
sudo ln -s /etc/nginx/sites-available/haryana-stat-abstract /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

---

## SSL/HTTPS Configuration

### Option 1: Let's Encrypt (Free SSL)

#### Linux (Certbot)
```bash
# Install Certbot
sudo apt-get install certbot python3-certbot-nginx

# Obtain certificate
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com

# Auto-renewal (already configured)
sudo certbot renew --dry-run
```

#### Windows (Win-ACME)
```powershell
# Download from: https://www.win-acme.com/
# Run and follow wizard to obtain certificate
```

### Option 2: Commercial SSL Certificate

#### IIS
1. Import certificate in IIS Manager → Server Certificates
2. Add HTTPS binding to website
3. Select imported certificate

#### Nginx
```nginx
server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;
    
    ssl_certificate /path/to/certificate.crt;
    ssl_certificate_key /path/to/private.key;
    
    # SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    # ... rest of configuration
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

---

## Monitoring & Logging

### Application Logs

#### Windows (IIS)
- Logs location: `C:\inetpub\wwwroot\haryana-api\logs\`
- IIS logs: `C:\inetpub\logs\LogFiles\`

#### Linux
- Application logs: `/var/log/haryana-api/`
- Systemd logs: `journalctl -u haryana-api -f`

### Health Check Endpoint
```bash
# Test backend health
curl https://api.yourdomain.com/health
```

### Log Rotation

#### Linux (logrotate)
Create `/etc/logrotate.d/haryana-api`:
```
/var/log/haryana-api/*.log {
    daily
    rotate 30
    compress
    delaycompress
    missingok
    notifempty
    create 0644 www-data www-data
}
```

---

## Post-Deployment Verification

### 1. Backend API Tests
```bash
# Health check
curl https://api.yourdomain.com/health

# Test authentication
curl -X POST https://api.yourdomain.com/api/UserManagement/login \
  -H "Content-Type: application/json" \
  -d '{"loginID":"admin","password":"Admin@123"}'
```

### 2. Frontend Tests
- ✅ Open `https://yourdomain.com` in browser
- ✅ Verify login page loads
- ✅ Test login functionality
- ✅ Verify API calls work (check browser console)
- ✅ Test all major features

### 3. Database Connection
- ✅ Verify database connection in application logs
- ✅ Test CRUD operations
- ✅ Verify audit fields are populated

### 4. Security Checks
- ✅ HTTPS is enforced
- ✅ CORS is configured correctly
- ✅ JWT tokens are working
- ✅ Swagger is disabled in production

---

## Troubleshooting

### Backend Not Starting

#### Check Logs
```bash
# Windows
Get-Content C:\inetpub\wwwroot\haryana-api\logs\*.log -Tail 50

# Linux
journalctl -u haryana-api -n 50
```

#### Common Issues
1. **Database connection failed**
   - Verify connection string
   - Check firewall rules
   - Verify SQL Server is accessible

2. **Port already in use**
   - Change port in `appsettings.json` or service file
   - Check for other running instances

3. **Missing dependencies**
   - Verify .NET 8.0 runtime is installed
   - Rebuild application

### Frontend Not Loading

#### Check Browser Console
- Look for CORS errors
- Verify API URL is correct
- Check network tab for failed requests

#### Common Issues
1. **API calls failing**
   - Verify `VITE_API_BASE_URL` in `.env.production`
   - Rebuild frontend after changing env vars
   - Check CORS configuration

2. **404 errors on routes**
   - Verify web.config (IIS) or nginx rewrite rules
   - Ensure index.html is served for all routes

### Database Issues

#### Connection Timeout
```sql
-- Check SQL Server is accepting connections
SELECT @@SERVERNAME, @@VERSION;

-- Check firewall
-- Allow port 1433 in Windows Firewall or Linux iptables
```

#### Migration Errors
```bash
# Run migrations manually
cd backend/HaryanaStatAbstract.API
dotnet ef database update
```

---

## Deployment Checklist

### Pre-Deployment
- [ ] Database is deployed and accessible
- [ ] SSL certificate is obtained
- [ ] Domain DNS is configured
- [ ] Server meets requirements
- [ ] .NET 8.0 runtime is installed
- [ ] Firewall rules are configured

### Backend Deployment
- [ ] Application is built in Release mode
- [ ] `appsettings.Production.json` is configured
- [ ] Connection string is updated
- [ ] JWT secret key is changed
- [ ] CORS origins are updated
- [ ] Application is deployed to server
- [ ] Service/IIS is configured and running
- [ ] Health check endpoint responds

### Frontend Deployment
- [ ] `.env.production` is created with correct API URL
- [ ] Frontend is built (`npm run build`)
- [ ] Static files are deployed
- [ ] Reverse proxy is configured
- [ ] Routes are working (SPA routing)

### Security
- [ ] HTTPS is enabled and working
- [ ] HTTP redirects to HTTPS
- [ ] Swagger is disabled in production
- [ ] JWT secret is strong and secure
- [ ] CORS is restricted to production domain
- [ ] Database credentials are secure

### Post-Deployment
- [ ] Health check passes
- [ ] Login works
- [ ] API endpoints respond correctly
- [ ] Database operations work
- [ ] Logs are being written
- [ ] Monitoring is set up

---

## Quick Reference

### Important URLs
- **Frontend**: `https://yourdomain.com`
- **Backend API**: `https://api.yourdomain.com` or `https://yourdomain.com/api`
- **Health Check**: `https://api.yourdomain.com/health`
- **Swagger** (Dev only): `https://api.yourdomain.com/swagger`

### Important Files
- **Backend Config**: `appsettings.Production.json`
- **Frontend Config**: `.env.production`
- **IIS Config**: `web.config`
- **Nginx Config**: `/etc/nginx/sites-available/haryana-stat-abstract`
- **Systemd Service**: `/etc/systemd/system/haryana-api.service`

### Useful Commands

#### Windows
```powershell
# Restart IIS
iisreset

# Check application pool status
Get-WebAppPoolState -Name HaryanaStatAbstractAPI

# View logs
Get-Content C:\inetpub\wwwroot\haryana-api\logs\*.log -Tail 50
```

#### Linux
```bash
# Restart backend service
sudo systemctl restart haryana-api

# Check status
sudo systemctl status haryana-api

# View logs
journalctl -u haryana-api -f

# Restart Nginx
sudo systemctl restart nginx
```

---

## Support & Maintenance

### Regular Maintenance Tasks
1. **Weekly**: Review application logs
2. **Monthly**: Update .NET runtime if security patches available
3. **Quarterly**: Review and rotate JWT secret key
4. **As needed**: Database backups

### Backup Strategy
- **Database**: Daily automated backups
- **Application**: Version control (Git)
- **Configuration**: Secure backup of production configs

---

**Last Updated**: January 2025
**Version**: 1.0
