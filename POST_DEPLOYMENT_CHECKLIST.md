# Post-Deployment Checklist

After deploying your backend to Azure, follow these steps to verify and complete the setup.

## ✅ Step 1: Verify Backend Deployment

### Test Your API Endpoints

1. **Health Check**:
   ```
   https://your-app-name.azurewebsites.net/health
   ```
   Expected: Should return `200 OK` or health status

2. **Swagger UI** (if enabled):
   ```
   https://your-app-name.azurewebsites.net/swagger
   ```
   Expected: Should show API documentation

3. **API Root**:
   ```
   https://your-app-name.azurewebsites.net/api
   ```
   Expected: Should return API response or 404 (if no root endpoint)

### Check Logs

1. Go to **Azure Portal** → Your App Service
2. Navigate to **"Log stream"**
3. Verify no errors are showing
4. Check application is starting correctly

## ✅ Step 2: Verify Application Settings

Confirm all settings are configured in Azure Portal:

### Required Settings:
- ✅ `ConnectionStrings__DefaultConnection` - Database connection string
- ✅ `JwtSettings__SecretKey` - JWT secret key
- ✅ `JwtSettings__Issuer` - HaryanaStatAbstractAPI
- ✅ `JwtSettings__Audience` - HaryanaStatAbstractClient
- ✅ `JwtSettings__AccessTokenExpirationMinutes` - 60
- ✅ `ASPNETCORE_ENVIRONMENT` - Production

### Verify Connection String:
- Go to **Configuration** → **Connection strings**
- Verify `DefaultConnection` is set correctly
- Test database connectivity

## ✅ Step 3: Test Authentication

### Test Login Endpoint

Use Postman, curl, or browser to test:

```bash
POST https://your-app-name.azurewebsites.net/api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin",
  "password": "Admin@123"
}
```

**Expected Response**:
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "",
  "expiresAt": "2026-01-26T...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@haryanastatabstract.com",
    "roles": ["System Admin"]
  }
}
```

### If Login Fails:
1. Check **Log stream** for errors
2. Verify database connection string
3. Verify admin user exists in database
4. Check JWT secret key is set

## ✅ Step 4: Configure CORS for Frontend

### In Azure Portal:

1. Go to your App Service
2. Navigate to **API** → **CORS**
3. Add your frontend URLs:
   - `https://your-frontend.vercel.app`
   - `https://your-custom-domain.com` (if applicable)
   - `http://localhost:5173` (for local development testing)
4. Enable **"Access-Control-Allow-Credentials"**
5. Click **"Save"**

### Or Update Program.cs (Alternative):

If CORS is configured in code, ensure it includes your Vercel domain.

## ✅ Step 5: Update Frontend Configuration

### In Vercel Dashboard:

1. Go to your Vercel project
2. Navigate to **Settings** → **Environment Variables**
3. Update or add:
   ```
   VITE_API_BASE_URL = https://your-app-name.azurewebsites.net/api
   ```
4. **Important**: Select all environments (Production, Preview, Development)
5. Click **"Save"**

### Redeploy Frontend:

After updating the environment variable:
1. Go to **Deployments** in Vercel
2. Click **"Redeploy"** on the latest deployment
3. Or push a new commit to trigger redeployment

## ✅ Step 6: Test End-to-End

### Test Frontend → Backend Connection:

1. Open your frontend URL: `https://your-frontend.vercel.app`
2. Try to login with:
   - Username: `admin`
   - Password: `Admin@123`
3. Verify:
   - ✅ Login succeeds
   - ✅ Token is received
   - ✅ User is redirected to dashboard
   - ✅ API calls work from frontend

### Common Issues:

**CORS Error**:
- Solution: Add frontend URL to CORS settings in Azure Portal

**401 Unauthorized**:
- Solution: Check JWT secret key matches
- Verify token is being sent in Authorization header

**Network Error**:
- Solution: Verify `VITE_API_BASE_URL` is set correctly
- Check backend URL is accessible

## ✅ Step 7: Enable Monitoring (Optional but Recommended)

### Application Insights:

1. Go to **Azure Portal** → Your App Service
2. Navigate to **Application Insights**
3. Click **"Turn on Application Insights"**
4. Create new or use existing resource
5. This enables:
   - Performance monitoring
   - Error tracking
   - Request analytics

### Log Stream:

Keep **Log stream** open during initial testing to catch errors immediately.

## ✅ Step 8: Security Hardening

### Enable HTTPS Only:

1. Go to **TLS/SSL settings**
2. Enable **"HTTPS Only"**
3. Click **"Save"**

### Review Security Headers:

Your `web.config` already includes security headers. Verify they're working:
- X-Content-Type-Options
- X-Frame-Options
- X-XSS-Protection

## ✅ Step 9: Performance Optimization

### Enable Always On:

1. Go to **Configuration** → **General settings**
2. Enable **"Always On"**
3. Click **"Save"**
4. **Note**: This prevents cold starts but may increase costs

### Scale Settings:

For production, consider:
- **Basic B1**: ~$13/month (testing)
- **Standard S1**: ~$70/month (production)
- Auto-scale based on traffic

## ✅ Step 10: Backup and Recovery

### Database Backups:

Ensure your SQL Server database has:
- ✅ Automated backups enabled
- ✅ Point-in-time restore configured
- ✅ Backup retention policy set

### Application Settings Backup:

1. Go to **Configuration** → **Application settings**
2. Click **"Download"** to save settings
3. Store securely (not in Git!)

## Troubleshooting Common Issues

### Issue: 500 Internal Server Error

**Check**:
1. **Log stream** for detailed errors
2. All application settings are configured
3. Connection string is correct
4. Database is accessible

**Solution**:
- Enable detailed error messages in Azure Portal
- Check application logs
- Verify all required settings

### Issue: Database Connection Timeout

**Check**:
1. SQL Server firewall allows Azure IPs
2. Connection string is correct
3. Database server is running

**Solution**:
- Add Azure service IPs to SQL Server firewall
- Verify connection string format
- Test connection from Azure Portal

### Issue: CORS Errors from Frontend

**Check**:
1. Frontend URL is in CORS settings
2. CORS credentials are enabled
3. Frontend is using correct API URL

**Solution**:
- Add exact frontend URL to CORS (including https://)
- Enable "Access-Control-Allow-Credentials"
- Verify VITE_API_BASE_URL in Vercel

### Issue: JWT Token Invalid

**Check**:
1. JWT secret key is set correctly
2. Secret key matches between environments
3. Token expiration settings

**Solution**:
- Verify `JwtSettings__SecretKey` in Azure Portal
- Ensure secret key is strong (64+ characters)
- Check token expiration settings

## Quick Test Commands

### Test Health Endpoint:
```bash
curl https://your-app-name.azurewebsites.net/health
```

### Test Login:
```bash
curl -X POST https://your-app-name.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"Admin@123"}'
```

### Test with Token:
```bash
curl https://your-app-name.azurewebsites.net/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Next Steps

1. ✅ Backend deployed and tested
2. ✅ Frontend configured with new API URL
3. ✅ CORS configured
4. ✅ End-to-end testing complete
5. ✅ Monitoring enabled
6. ✅ Security hardened

## Support Resources

- **Azure Portal**: https://portal.azure.com
- **Vercel Dashboard**: https://vercel.com/dashboard
- **Azure Documentation**: https://docs.microsoft.com/azure/app-service
- **Application Logs**: Azure Portal → Your App Service → Log stream

---

**Your Backend URL**: `https://your-app-name.azurewebsites.net`  
**Your Frontend URL**: `https://your-frontend.vercel.app`
