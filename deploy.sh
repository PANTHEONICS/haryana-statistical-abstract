#!/bin/bash
# Linux Deployment Script
# Run this script from the project root directory

set -e  # Exit on error

# Configuration
BACKEND_PUBLISH_PATH="/var/www/haryana-api"
FRONTEND_PUBLISH_PATH="/var/www/haryana-frontend"
SKIP_BUILD=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --backend-path)
            BACKEND_PUBLISH_PATH="$2"
            shift 2
            ;;
        --frontend-path)
            FRONTEND_PUBLISH_PATH="$2"
            shift 2
            ;;
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo "========================================"
echo "Haryana Statistical Abstract Deployment"
echo "========================================"
echo ""

# Check prerequisites
echo "Checking prerequisites..."
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 8.0 SDK."
    exit 1
fi
echo "✅ .NET SDK found: $(dotnet --version)"

if ! command -v node &> /dev/null; then
    echo "❌ Node.js not found. Please install Node.js 18+."
    exit 1
fi
echo "✅ Node.js found: $(node --version)"

# Build Backend
if [ "$SKIP_BUILD" = false ]; then
    echo ""
    echo "Building backend..."
    cd backend/HaryanaStatAbstract.API
    
    dotnet publish -c Release -o "$BACKEND_PUBLISH_PATH"
    
    if [ $? -ne 0 ]; then
        echo "❌ Backend build failed!"
        exit 1
    fi
    
    echo "✅ Backend built successfully"
    cd ../..
else
    echo "⏭️  Skipping backend build"
fi

# Build Frontend
if [ "$SKIP_BUILD" = false ]; then
    echo ""
    echo "Building frontend..."
    cd frontend
    
    # Check if .env.production exists
    if [ ! -f ".env.production" ]; then
        echo "⚠️  Warning: .env.production not found."
        if [ -f ".env.production.example" ]; then
            cp ".env.production.example" ".env.production"
            echo "⚠️  Please update .env.production with your production API URL!"
        fi
    fi
    
    npm install
    if [ $? -ne 0 ]; then
        echo "❌ npm install failed!"
        exit 1
    fi
    
    npm run build
    if [ $? -ne 0 ]; then
        echo "❌ Frontend build failed!"
        exit 1
    fi
    
    echo "✅ Frontend built successfully"
    cd ..
else
    echo "⏭️  Skipping frontend build"
fi

# Copy Frontend Files
if [ -n "$FRONTEND_PUBLISH_PATH" ]; then
    echo ""
    echo "Copying frontend files to $FRONTEND_PUBLISH_PATH..."
    
    sudo mkdir -p "$FRONTEND_PUBLISH_PATH"
    sudo cp -r frontend/dist/* "$FRONTEND_PUBLISH_PATH/"
    sudo chown -R www-data:www-data "$FRONTEND_PUBLISH_PATH"
    sudo chmod -R 755 "$FRONTEND_PUBLISH_PATH"
    
    echo "✅ Frontend files copied"
fi

# Verify appsettings.Production.json
echo ""
echo "Checking production configuration..."
PROD_CONFIG="$BACKEND_PUBLISH_PATH/appsettings.Production.json"
if [ -f "$PROD_CONFIG" ]; then
    if grep -q "YOUR_DB_SERVER" "$PROD_CONFIG"; then
        echo "⚠️  Warning: appsettings.Production.json contains placeholder values!"
        echo "   Please update the connection string and JWT secret key."
    else
        echo "✅ Production configuration found"
    fi
else
    echo "⚠️  Warning: appsettings.Production.json not found in publish folder"
fi

echo ""
echo "========================================"
echo "Deployment Complete!"
echo "========================================"
echo ""
echo "Next steps:"
echo "1. Update appsettings.Production.json with your database connection string"
echo "2. Update JWT SecretKey in appsettings.Production.json"
echo "3. Create systemd service file (see DEPLOYMENT_PLAN.md)"
echo "4. Configure Nginx reverse proxy (see DEPLOYMENT_PLAN.md)"
echo "5. Update CORS settings in Program.cs and rebuild if needed"
echo "6. Configure SSL certificate"
echo "7. Test the application"
echo ""
echo "Backend location: $BACKEND_PUBLISH_PATH"
if [ -n "$FRONTEND_PUBLISH_PATH" ]; then
    echo "Frontend location: $FRONTEND_PUBLISH_PATH"
fi
echo ""
