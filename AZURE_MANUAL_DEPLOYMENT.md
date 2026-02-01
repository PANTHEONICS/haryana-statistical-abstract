# Manual Deployment to Azure App Service via Portal

This guide explains how to manually upload your backend code to Azure App Service through the Azure Portal.

## Prerequisites

1. ✅ Azure App Service resource already created
2. ✅ Backend code ready on your local machine
3. ✅ .NET 8.0 SDK installed locally

## Step 1: Build and Publish Your Code Locally

First, build and publish your project locally:

```bash
# Navigate to your API project
cd backend/HaryanaStatAbstract.API

# Publish the project (creates deployable files)
dotnet publish -c Release -o ./publish
```

This creates a `publish` folder with all the files needed for deployment.

## Step 2: Create Deployment Package (ZIP file)

### Option A: Using PowerShell (Windows)

```powershell
# Navigate to publish folder
cd backend/HaryanaStatAbstract.API/publish

# Create ZIP file
Compress-Archive -Path * -DestinationPath ../deploy.zip -Force

# The deploy.zip file will be in: backend/HaryanaStatAbstract.API/
```

### Option B: Using File Explorer

1. Navigate to `backend/HaryanaStatAbstract.API/publish` folder
2. Select all files and folders (Ctrl+A)
3. Right-click → **"Send to"** → **"Compressed (zipped) folder"**
4. Rename it to `deploy.zip`
5. Move it to `backend/HaryanaStatAbstract.API/` folder for easy access

## Step 3: Upload via Azure Portal - Method 1 (Kudu Console - Recommended)

### Step 3.1: Access Kudu Console

1. Go to **Azure Portal** → Your App Service
2. In the left menu, scroll down to **"Development Tools"**
3. Click **"Advanced Tools (Kudu)"**
4. Click **"Go"** (opens in new tab)

### Step 3.2: Navigate to Deployment Folder

1. In Kudu, click **"Debug console"** → **"CMD"** (or **"PowerShell"**)
2. Navigate to: `site/wwwroot`
   ```bash
   cd site/wwwroot
   ```

### Step 3.3: Delete Existing Files (if any)

```bash
# Delete all existing files
del *.* /s /q
# Or in PowerShell:
Remove-Item * -Recurse -Force
```

### Step 3.4: Upload ZIP File

1. In Kudu, go to **"Tools"** → **"Zip Push Deploy"**
2. Click **"Choose File"**
3. Select your `deploy.zip` file
4. Click **"Upload"**
5. Wait for extraction to complete

### Step 3.5: Verify Files

1. Go back to **"Debug console"** → **"CMD"**
2. Navigate to `site/wwwroot`
3. List files: `dir` (should show your published files)
4. Verify `HaryanaStatAbstract.API.dll` exists

## Step 4: Upload via Azure Portal - Method 2 (App Service Editor)

### Step 4.1: Access App Service Editor

1. Go to **Azure Portal** → Your App Service
2. In left menu, find **"Development Tools"** → **"App Service Editor"**
3. Click **"Go"** (opens in new tab)

### Step 4.2: Upload Files

1. In App Service Editor, you'll see the file structure
2. Navigate to `wwwroot` folder
3. Click **"Upload"** button (top toolbar)
4. Select all files from your `publish` folder
5. Wait for upload to complete

**Note**: This method is slower for many files. Use Method 1 (Kudu) for better performance.

## Step 5: Upload via Azure Portal - Method 3 (FTP)

### Step 5.1: Get FTP Credentials

1. Go to **Azure Portal** → Your App Service
2. Click **"Deployment Center"** (or **"Deployment"** → **"FTP"**)
3. Click **"FTPS credentials"**
4. Copy:
   - **FTPS endpoint**
   - **Username**
   - **Password**

### Step 5.2: Connect via FTP Client

1. **Download FTP Client** (if needed):
   - FileZilla (free): https://filezilla-project.org
   - WinSCP (free): https://winscp.net

2. **Connect to FTP**:
   - Host: Your FTPS endpoint
   - Username: Your username
   - Password: Your password
   - Port: 21 (or 990 for FTPS)

3. **Upload Files**:
   - Navigate to `/site/wwwroot` folder
   - Delete existing files (if any)
   - Upload all files from your `publish` folder
   - Maintain folder structure

## Step 6: Configure Application Settings (Important!)

Before testing, configure your application settings:

1. Go to **Azure Portal** → Your App Service
2. Navigate to **"Configuration"** → **"Application settings"**
3. Add these settings:

### Connection String:
- Click **"+ New connection string"**
- **Name**: `DefaultConnection`
- **Value**: `Server=193.35.19.176;Database=HaryanaStatAbstractDb;User Id=HaryanaStat;Password=H@ry@n@#St@t@123;TrustServerCertificate=true;MultipleActiveResultSets=true`
- **Type**: SQLAzure (or Custom)

### Application Settings:
Click **"+ New application setting"** and add:

```
JwtSettings__SecretKey = YOUR_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG
JwtSettings__Issuer = HaryanaStatAbstractAPI
JwtSettings__Audience = HaryanaStatAbstractClient
JwtSettings__AccessTokenExpirationMinutes = 60
ASPNETCORE_ENVIRONMENT = Production
```

4. Click **"Save"** (restarts the app)

## Step 7: Verify Deployment

### Step 7.1: Check App Status

1. Go to **"Overview"** in your App Service
2. Status should be **"Running"**
3. Click **"Browse"** to open your app

### Step 7.2: Test Endpoints

1. **Health Check**: `https://your-app-name.azurewebsites.net/health`
2. **Swagger** (if enabled): `https://your-app-name.azurewebsites.net/swagger`
3. **API Root**: `https://your-app-name.azurewebsites.net/api`

### Step 7.3: Check Logs

1. Go to **"Log stream"** in Azure Portal
2. You should see application logs
3. If there are errors, check the logs for details

## Step 8: Enable Application Logging (If Needed)

1. Go to **"App Service logs"** in your App Service
2. Enable:
   - **Application Logging (Filesystem)**: On
   - **Level**: Information (or Verbose for debugging)
   - **Web server logging**: On
3. Click **"Save"**

## Troubleshooting

### Issue: App shows "Application Error" or blank page

**Solution**:
1. Check **"Log stream"** for errors
2. Verify all application settings are configured
3. Check connection string is correct
4. Verify .NET 8 runtime is selected:
   - Go to **"Configuration"** → **"General settings"**
   - **Stack**: .NET
   - **Version**: 8.0

### Issue: Files not uploading

**Solution**:
1. Make sure you're uploading to `site/wwwroot` folder
2. Delete existing files first
3. Use ZIP upload method (Method 1) for better reliability
4. Check file size limits (max 100MB per file in Kudu)

### Issue: Database connection fails

**Solution**:
1. Verify connection string in Application Settings
2. Check SQL Server firewall allows Azure IPs
3. Test connection string format
4. Check logs for specific error messages

### Issue: 500 Internal Server Error

**Solution**:
1. Enable detailed error pages:
   - Go to **"Configuration"** → **"General settings"**
   - Enable **"Always On"**
   - Enable **"Detailed error messages"**
2. Check **"Log stream"** for detailed errors
3. Verify JWT secret key is set

## Quick Reference: File Locations

- **Local Publish Folder**: `backend/HaryanaStatAbstract.API/publish/`
- **Azure Deployment Folder**: `site/wwwroot/`
- **Logs Folder**: `LogFiles/Application/`

## Alternative: Use Azure CLI (Faster)

If you have Azure CLI installed, this is faster:

```bash
# Navigate to API project
cd backend/HaryanaStatAbstract.API

# Publish
dotnet publish -c Release -o ./publish

# Create ZIP
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip

# Deploy via CLI
az webapp deployment source config-zip \
  --resource-group YOUR_RESOURCE_GROUP \
  --name YOUR_APP_NAME \
  --src ../deploy.zip
```

## Next Steps

1. ✅ Code uploaded to Azure
2. ✅ Application settings configured
3. ✅ Test the API endpoints
4. ✅ Update frontend VITE_API_BASE_URL
5. ✅ Configure CORS for frontend

---

**Your API URL**: `https://your-app-name.azurewebsites.net`
