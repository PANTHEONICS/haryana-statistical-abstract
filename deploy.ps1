# PowerShell Deployment Script for Windows Server
# Run this script from the project root directory

param(
    [Parameter(Mandatory=$true)]
    [string]$PublishPath = "C:\inetpub\wwwroot\haryana-api",
    
    [Parameter(Mandatory=$false)]
    [string]$FrontendPath = "C:\inetpub\wwwroot\haryana-frontend",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Haryana Statistical Abstract Deployment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Checking prerequisites..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET SDK not found. Please install .NET 8.0 SDK." -ForegroundColor Red
    exit 1
}
Write-Host "✅ .NET SDK found: $dotnetVersion" -ForegroundColor Green

# Build Backend
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building backend..." -ForegroundColor Yellow
    Set-Location "backend\HaryanaStatAbstract.API"
    
    dotnet publish -c Release -o $PublishPath
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Backend build failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Backend built successfully" -ForegroundColor Green
    Set-Location ..\..
} else {
    Write-Host "⏭️  Skipping backend build" -ForegroundColor Yellow
}

# Build Frontend
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building frontend..." -ForegroundColor Yellow
    Set-Location "frontend"
    
    # Check if .env.production exists
    if (-not (Test-Path ".env.production")) {
        Write-Host "⚠️  Warning: .env.production not found. Using .env.production.example if available." -ForegroundColor Yellow
        if (Test-Path ".env.production.example") {
            Copy-Item ".env.production.example" ".env.production"
            Write-Host "⚠️  Please update .env.production with your production API URL!" -ForegroundColor Yellow
        }
    }
    
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ npm install failed!" -ForegroundColor Red
        exit 1
    }
    
    npm run build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Frontend build failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Frontend built successfully" -ForegroundColor Green
    Set-Location ..
} else {
    Write-Host "⏭️  Skipping frontend build" -ForegroundColor Yellow
}

# Copy Frontend Files
if ($FrontendPath) {
    Write-Host ""
    Write-Host "Copying frontend files to $FrontendPath..." -ForegroundColor Yellow
    
    if (-not (Test-Path $FrontendPath)) {
        New-Item -ItemType Directory -Path $FrontendPath -Force | Out-Null
    }
    
    Copy-Item -Path "frontend\dist\*" -Destination $FrontendPath -Recurse -Force
    Write-Host "✅ Frontend files copied" -ForegroundColor Green
}

# Verify appsettings.Production.json
Write-Host ""
Write-Host "Checking production configuration..." -ForegroundColor Yellow
$prodConfig = Join-Path $PublishPath "appsettings.Production.json"
if (Test-Path $prodConfig) {
    $config = Get-Content $prodConfig | ConvertFrom-Json
    if ($config.ConnectionStrings.DefaultConnection -like "*YOUR_DB_SERVER*") {
        Write-Host "⚠️  Warning: appsettings.Production.json contains placeholder values!" -ForegroundColor Yellow
        Write-Host "   Please update the connection string and JWT secret key." -ForegroundColor Yellow
    } else {
        Write-Host "✅ Production configuration found" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️  Warning: appsettings.Production.json not found in publish folder" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Update appsettings.Production.json with your database connection string" -ForegroundColor White
Write-Host "2. Update JWT SecretKey in appsettings.Production.json" -ForegroundColor White
Write-Host "3. Configure IIS Application Pool and Website" -ForegroundColor White
Write-Host "4. Update CORS settings in Program.cs and rebuild if needed" -ForegroundColor White
Write-Host "5. Configure SSL certificate" -ForegroundColor White
Write-Host "6. Test the application" -ForegroundColor White
Write-Host ""
Write-Host "Backend location: $PublishPath" -ForegroundColor Yellow
if ($FrontendPath) {
    Write-Host "Frontend location: $FrontendPath" -ForegroundColor Yellow
}
Write-Host ""
