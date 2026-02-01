# Vercel Deployment Guide

This guide explains how to deploy the Haryana Statistical Abstract frontend to Vercel.

## Prerequisites

1. A Vercel account (sign up at https://vercel.com)
2. Your backend API deployed and accessible
3. GitHub repository connected to Vercel (optional but recommended)

## Step 1: Prepare Your Repository

Ensure your code is pushed to GitHub:
```bash
git add .
git commit -m "Prepare for Vercel deployment"
git push origin main
```

## Step 2: Deploy to Vercel

### Option A: Deploy via Vercel Dashboard

1. Go to [Vercel Dashboard](https://vercel.com/dashboard)
2. Click **"Add New Project"**
3. Import your GitHub repository: `PANTHEONICS/haryanaDemo`
4. Configure the project:
   - **Framework Preset**: Vite
   - **Root Directory**: `frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist`
   - **Install Command**: `npm install`

### Option B: Deploy via Vercel CLI

```bash
# Install Vercel CLI
npm i -g vercel

# Navigate to frontend directory
cd frontend

# Deploy
vercel

# Follow the prompts:
# - Link to existing project? (No for first time)
# - Project name: haryana-stat-abstract
# - Directory: ./
# - Override settings? (No)
```

## Step 3: Configure Environment Variables

### In Vercel Dashboard:

1. Go to your project in Vercel Dashboard
2. Navigate to **Settings** > **Environment Variables**
3. Add the following variables:

#### For Production:
```
VITE_API_BASE_URL = https://your-production-backend-url.com/api
```

#### For Preview/Development:
```
VITE_API_BASE_URL = http://localhost:5000/api
```

### Environment Variable Details:

| Variable | Description | Example |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Backend API base URL | `https://api.example.com/api` |

**Important Notes:**
- Variables must start with `VITE_` to be exposed to client-side code
- Set different values for Production, Preview, and Development environments
- After adding variables, redeploy your application

## Step 4: Build Configuration

Vercel will automatically detect Vite, but you can customize in `vercel.json`:

```json
{
  "buildCommand": "npm run build",
  "outputDirectory": "dist",
  "devCommand": "npm run dev",
  "installCommand": "npm install",
  "framework": "vite"
}
```

## Step 5: Deploy

1. Click **"Deploy"** in Vercel Dashboard
2. Wait for the build to complete
3. Your app will be available at: `https://your-project.vercel.app`

## Step 6: Custom Domain (Optional)

1. Go to **Settings** > **Domains**
2. Add your custom domain
3. Follow DNS configuration instructions
4. Vercel will automatically configure SSL

## Troubleshooting

### Build Fails

**Issue**: Build command fails
**Solution**: 
- Check that `package.json` has a `build` script
- Ensure all dependencies are in `package.json`
- Check build logs in Vercel Dashboard

### API Calls Fail

**Issue**: Frontend can't connect to backend
**Solution**:
- Verify `VITE_API_BASE_URL` is set correctly in Vercel
- Check CORS settings on your backend
- Ensure backend is accessible from the internet

### Environment Variables Not Working

**Issue**: Variables not available in code
**Solution**:
- Ensure variables start with `VITE_` prefix
- Redeploy after adding variables
- Check variable names match exactly (case-sensitive)

## Backend CORS Configuration

Make sure your backend allows requests from Vercel domain:

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://your-project.vercel.app",
            "https://your-custom-domain.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

## Continuous Deployment

Once connected to GitHub:
- Every push to `main` branch = Production deployment
- Every pull request = Preview deployment
- Automatic deployments enabled by default

## Environment-Specific URLs

- **Production**: `https://your-project.vercel.app`
- **Preview**: `https://your-project-git-branch.vercel.app`
- **Development**: Local development server

## Support

For issues:
- Check Vercel [Documentation](https://vercel.com/docs)
- Review build logs in Vercel Dashboard
- Check [Vercel Status](https://www.vercel-status.com/)
