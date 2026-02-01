# Azure App Service Deployment Guide

This guide explains how to deploy the Haryana Statistical Abstract backend API to Azure App Service.

## Prerequisites

1. **Azure Account** - Sign up at https://azure.microsoft.com
2. **Azure CLI** (optional but recommended) - Install from https://docs.microsoft.com/cli/azure/install-azure-cli
3. **.NET 8.0 SDK** - Already installed for development
4. **Database** - Already configured and accessible (public database at 193.35.19.176)

## Step 1: Prepare Your Code

Ensure your code is ready for deployment:

```bash
cd backend/HaryanaStatAbstract.API
dotnet build --configuration Release
```

## Step 2: Create Azure App Service

### Option A: Using Azure Portal (Recommended for beginners)

1. **Login to Azure Portal**
   - Go to https://portal.azure.com
   - Sign in with your Azure account

2. **Create App Service**
   - Click **"Create a resource"**
   - Search for **"Web App"** or **"App Service"**
   - Click **"Create"**

3. **Configure Basic Settings**
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or select existing
     - Name: `haryana-stat-abstract-rg`
   - **Name**: `haryana-stat-api` (must be globally unique)
   - **Publish**: Code
   - **Runtime stack**: `.NET 8`
   - **Operating System**: Windows
   - **Region**: Choose closest to your database (e.g., East US, West Europe)
   - **App Service Plan**: 
     - Create new or select existing
     - **SKU and size**: Basic B1 (for testing) or Standard S1 (for production)
   - Click **"Review + create"** then **"Create"**

### Option B: Using Azure CLI

```bash
# Login to Azure
az login

# Create Resource Group
az group create --name haryana-stat-abstract-rg --location eastus

# Create App Service Plan
az appservice plan create \
  --name haryana-stat-plan \
  --resource-group haryana-stat-abstract-rg \
  --sku B1 \
  --is-linux false

# Create Web App
az webapp create \
  --name haryana-stat-api \
  --resource-group haryana-stat-abstract-rg \
  --plan haryana-stat-plan \
  --runtime "DOTNET|8.0"
```

## Step 3: Configure Application Settings

### In Azure Portal:

1. Go to your App Service
2. Navigate to **Configuration** > **Application settings**
3. Click **"+ New application setting"** and add:

#### Connection String:
- **Name**: `ConnectionStrings__DefaultConnection`
- **Value**: `Server=193.35.19.176;Database=HaryanaStatAbstractDb;User Id=HaryanaStat;Password=H@ry@n@#St@t@123;TrustServerCertificate=true;MultipleActiveResultSets=true`
- **Type**: Custom

#### JWT Settings:
- **Name**: `JwtSettings__SecretKey`
- **Value**: `YOUR_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG` (Generate a strong key!)
- **Type**: Custom

- **Name**: `JwtSettings__Issuer`
- **Value**: `HaryanaStatAbstractAPI`
- **Type**: Custom

- **Name**: `JwtSettings__Audience`
- **Value**: `HaryanaStatAbstractClient`
- **Type**: Custom

- **Name**: `JwtSettings__AccessTokenExpirationMinutes`
- **Value**: `60`
- **Type**: Custom

#### CORS Settings (if needed):
- **Name**: `Cors__AllowedOrigins`
- **Value**: `https://your-frontend-domain.vercel.app,https://your-custom-domain.com`
- **Type**: Custom

#### Environment:
- **Name**: `ASPNETCORE_ENVIRONMENT`
- **Value**: `Production`
- **Type**: Custom

4. Click **"Save"** (this will restart your app)

## Step 4: Configure CORS for Frontend

Update CORS in your App Service to allow requests from your Vercel frontend:

1. Go to **API** > **CORS**
2. Add allowed origins:
   - `https://your-frontend.vercel.app`
   - `https://your-custom-domain.com`
   - (Add your Vercel deployment URL)
3. Enable **"Access-Control-Allow-Credentials"**
4. Click **"Save"**

## Step 5: Deploy Your Code

### Option A: Deploy from Visual Studio (Easiest)

1. **Right-click** on `HaryanaStatAbstract.API` project
2. Select **"Publish"**
3. Choose **"Azure"** > **"Azure App Service (Windows)"**
4. Select your subscription and App Service
5. Click **"Publish"**

### Option B: Deploy using Azure CLI

```bash
# Navigate to API project directory
cd backend/HaryanaStatAbstract.API

# Publish the project
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
Compress-Archive -Path * -DestinationPath ../deploy.zip

# Deploy to Azure
az webapp deployment source config-zip \
  --resource-group haryana-stat-abstract-rg \
  --name haryana-stat-api \
  --src ../deploy.zip
```

### Option C: Deploy using GitHub Actions (CI/CD)

1. **Create `.github/workflows/azure-deploy.yml`**:

```yaml
name: Deploy to Azure App Service

on:
  push:
    branches:
      - main
    paths:
      - 'backend/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: dotnet build backend/HaryanaStatAbstract.API/HaryanaStatAbstract.API.csproj --configuration Release
    
    - name: Publish
      run: dotnet publish backend/HaryanaStatAbstract.API/HaryanaStatAbstract.API.csproj --configuration Release --output ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'haryana-stat-api'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

2. **Get Publish Profile**:
   - Go to Azure Portal > Your App Service
   - Click **"Get publish profile"**
   - Copy the content
   - Go to GitHub > Repository > Settings > Secrets > Actions
   - Add secret: `AZURE_WEBAPP_PUBLISH_PROFILE` (paste the content)

### Option D: Deploy using VS Code

1. Install **Azure App Service** extension in VS Code
2. Sign in to Azure
3. Right-click on your project folder
4. Select **"Deploy to Web App"**
5. Choose your App Service

## Step 6: Verify Deployment

1. **Check App Service Status**
   - Go to Azure Portal > Your App Service
   - Check **"Overview"** for status (should be "Running")

2. **Test the API**
   - Your API URL: `https://haryana-stat-api.azurewebsites.net`
   - Test Swagger: `https://haryana-stat-api.azurewebsites.net/swagger`
   - Test Health: `https://haryana-stat-api.azurewebsites.net/health`

3. **Check Logs**
   - Go to **"Log stream"** to see real-time logs
   - Or go to **"Logs"** > **"Application Logging"** to download logs

## Step 7: Update Frontend Configuration

Update your Vercel environment variable:

```
VITE_API_BASE_URL=https://haryana-stat-api.azurewebsites.net/api
```

## Step 8: Database Connection Security

Since your database is public, ensure:

1. **Firewall Rules**: Allow Azure App Service IPs
   - Go to your SQL Server in Azure Portal
   - Add firewall rule for Azure services
   - Or add your App Service outbound IPs

2. **Connection String**: Already configured in Step 3

## Troubleshooting

### Issue: App won't start

**Solution**:
- Check **"Log stream"** for errors
- Verify connection string is correct
- Check that .NET 8 runtime is selected
- Review **"Diagnose and solve problems"** in Azure Portal

### Issue: Database connection fails

**Solution**:
- Verify connection string in Application Settings
- Check SQL Server firewall allows Azure IPs
- Test connection string locally first
- Check SQL Server is accessible from internet

### Issue: 500 Internal Server Error

**Solution**:
- Enable **"Application Logging"** in App Service
- Check **"Log stream"** for detailed errors
- Verify all environment variables are set
- Check JWT secret key is configured

### Issue: CORS errors from frontend

**Solution**:
- Add frontend URL to CORS settings in Azure Portal
- Or update CORS in `Program.cs` to include Vercel domain
- Verify CORS middleware is configured correctly

### Issue: Swagger not showing

**Solution**:
- Swagger is only enabled in Development by default
- To enable in Production, update `Program.cs`:
  ```csharp
  // Remove or modify this condition
  if (app.Environment.IsDevelopment())
  {
      app.UseSwagger();
      app.UseSwaggerUI();
  }
  
  // Or enable for all environments (not recommended for production)
  app.UseSwagger();
  app.UseSwaggerUI();
  ```

## Security Best Practices

1. **Generate Strong JWT Secret**:
   ```csharp
   // Use a tool to generate a strong secret key
   // At least 32 characters, mix of letters, numbers, symbols
   ```

2. **Use Azure Key Vault** (Recommended):
   - Store connection strings and secrets in Azure Key Vault
   - Reference from App Service Configuration

3. **Enable HTTPS Only**:
   - Go to **"TLS/SSL settings"**
   - Enable **"HTTPS Only"**

4. **Configure Authentication** (Optional):
   - Add Azure AD authentication for admin access

5. **Enable Application Insights**:
   - Monitor performance and errors
   - Go to **"Application Insights"** and enable

## Cost Optimization

- **Development**: Use Basic B1 plan (~$13/month)
- **Production**: Use Standard S1 plan (~$70/month)
- **Scale down** when not in use to save costs
- **Use App Service Plan sharing** for multiple apps

## Monitoring

1. **Application Insights**: Enable for detailed monitoring
2. **Log Stream**: Real-time log viewing
3. **Metrics**: CPU, Memory, Response time
4. **Alerts**: Set up alerts for errors and performance

## Next Steps

1. ✅ Deploy backend to Azure
2. ✅ Update frontend VITE_API_BASE_URL
3. ✅ Test end-to-end functionality
4. ✅ Set up custom domain (optional)
5. ✅ Configure SSL certificate
6. ✅ Set up CI/CD pipeline

## Support

- **Azure Documentation**: https://docs.microsoft.com/azure/app-service
- **Azure Support**: https://azure.microsoft.com/support
- **Stack Overflow**: Tag with `azure-app-service` and `asp.net-core`

---

**Your API will be available at**: `https://haryana-stat-api.azurewebsites.net`
