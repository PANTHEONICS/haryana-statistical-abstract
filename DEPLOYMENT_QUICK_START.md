# Quick Deployment Guide

This is a condensed version of the full deployment plan. Use this for quick reference.

## Prerequisites Checklist
- [ ] Database server is accessible from production server
- [ ] Database credentials are available
- [ ] Domain name is configured
- [ ] SSL certificate is obtained
- [ ] Server has .NET 8.0 Runtime installed

## Quick Deployment Steps

### 1. Configure Backend (5 minutes)

#### Update `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_DB_SERVER;Database=HaryanaStatAbstractDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true;Encrypt=true"
  },
  "JwtSettings": {
    "SecretKey": "GENERATE_STRONG_SECRET_HERE_32_CHARS_MIN"
  }
}
```

#### Generate JWT Secret:
```bash
# Linux/Mac
openssl rand -base64 32

# Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

### 2. Build Backend (2 minutes)

```bash
cd backend/HaryanaStatAbstract.API
dotnet publish -c Release -o ./publish
```

### 3. Configure Frontend (2 minutes)

#### Create `.env.production`:
```env
VITE_API_BASE_URL=https://api.yourdomain.com/api
```

#### Build Frontend:
```bash
cd frontend
npm install
npm run build
```

### 4. Deploy Files

#### Backend:
- Copy `publish` folder contents to server
- Windows: `C:\inetpub\wwwroot\haryana-api\`
- Linux: `/var/www/haryana-api/`

#### Frontend:
- Copy `frontend/dist` folder contents to server
- Windows: `C:\inetpub\wwwroot\haryana-frontend\`
- Linux: `/var/www/haryana-frontend/`

### 5. Configure Server

#### Windows (IIS):
1. Install .NET 8.0 Hosting Bundle
2. Create Application Pool (No Managed Code)
3. Create Website pointing to backend folder
4. Configure reverse proxy for API calls

#### Linux (Systemd + Nginx):
1. Create systemd service (see full deployment plan)
2. Configure Nginx reverse proxy (see full deployment plan)
3. Enable and start services

### 6. Configure SSL

#### Let's Encrypt (Linux):
```bash
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

### 7. Update CORS in Program.cs

```csharp
policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
```

Rebuild and redeploy after this change.

### 8. Verify Deployment

```bash
# Health check
curl https://api.yourdomain.com/health

# Test login
curl -X POST https://api.yourdomain.com/api/UserManagement/login \
  -H "Content-Type: application/json" \
  -d '{"loginID":"admin","password":"Admin@123"}'
```

## Common Issues

### Database Connection Failed
- Check firewall allows port 1433
- Verify connection string format
- Test connection: `sqlcmd -S SERVER -U USER -P PASS -d DB`

### CORS Errors
- Verify frontend URL in CORS configuration
- Rebuild backend after CORS changes
- Check browser console for exact error

### 404 on Frontend Routes
- Verify web.config (IIS) or nginx rewrite rules
- Ensure index.html is served for all routes

## Next Steps
See `DEPLOYMENT_PLAN.md` for detailed instructions.
