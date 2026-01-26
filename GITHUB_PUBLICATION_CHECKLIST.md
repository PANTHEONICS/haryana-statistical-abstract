# GitHub Publication Checklist

Use this checklist before publishing the repository to GitHub to ensure no sensitive information is exposed.

## ✅ Pre-Publication Checklist

### 1. Sensitive Data Removal

- [ ] **Connection Strings**: Remove or sanitize database connection strings
  - [ ] Check `appsettings.json` - should use `appsettings.Example.json` instead
  - [ ] Check `appsettings.Development.json` - remove real server names
  - [ ] Check `appsettings.Production.json` - should NOT be in repo (already in .gitignore)

- [ ] **JWT Secret Keys**: Remove or replace with placeholder
  - [ ] Check `appsettings.json` - replace with placeholder
  - [ ] Check `appsettings.Development.json` - replace with placeholder

- [ ] **Server Names**: Remove specific server names
  - [ ] Check for `KAPILP\SQLEXPRESS` or other specific server names
  - [ ] Replace with placeholders like `YOUR_SERVER`

- [ ] **Passwords**: Remove any hardcoded passwords
  - [ ] Check seed data files
  - [ ] Check documentation files
  - [ ] Check SQL scripts

- [ ] **API Keys**: Remove any API keys or tokens
  - [ ] Check configuration files
  - [ ] Check environment files

### 2. Configuration Files

- [ ] Create `appsettings.Example.json` with placeholder values ✅ (Created)
- [ ] Update `.gitignore` to ensure sensitive files are excluded
- [ ] Verify `appsettings.Production.json` is in `.gitignore`
- [ ] Verify `.env.production` is in `.gitignore` (frontend)

### 3. Code Review

- [ ] **Remove Development Endpoints**: 
  - [ ] Check `UserManagementController.cs` for `fix-admin-password` endpoint
  - [ ] Consider removing or securing these endpoints
  - [ ] Add `[Authorize(Roles = "SystemAdmin")]` if keeping them

- [ ] **Remove Debug Code**: 
  - [ ] Remove console.log statements
  - [ ] Remove debug breakpoints
  - [ ] Remove test data that shouldn't be public

- [ ] **Review Comments**: 
  - [ ] Remove any internal notes or sensitive information
  - [ ] Remove TODO comments with sensitive context

### 4. Documentation

- [ ] **README.md**: 
  - [ ] Update with project description ✅ (Created)
  - [ ] Remove any internal references
  - [ ] Update default credentials section with warning

- [ ] **CONTRIBUTING.md**: 
  - [ ] Create contribution guidelines ✅ (Created)

- [ ] **LICENSE**: 
  - [ ] Add appropriate license ✅ (Created - MIT)

- [ ] **Documentation Files**: 
  - [ ] Review all .md files for sensitive information
  - [ ] Remove internal server names
  - [ ] Remove specific deployment details

### 5. Git History

- [ ] **Check Git History**: 
  - [ ] Review commit history for sensitive data
  - [ ] Consider using `git filter-branch` or BFG Repo-Cleaner if needed
  - [ ] Or start fresh repository if history contains sensitive data

### 6. Files to Exclude

Verify `.gitignore` includes:
- [ ] `appsettings.Production.json`
- [ ] `appsettings.*.json` (except example and development)
- [ ] `.env.production`
- [ ] `logs/` directory
- [ ] `bin/` and `obj/` directories
- [ ] `node_modules/`
- [ ] `.vs/` and `.vscode/` (optional)
- [ ] `*.log` files

### 7. Repository Settings

- [ ] **Repository Description**: Add clear description
- [ ] **Topics/Tags**: Add relevant tags (aspnet-core, react, sql-server, etc.)
- [ ] **Visibility**: Choose Public or Private
- [ ] **Branch Protection**: Set up branch protection rules
- [ ] **Issues**: Enable issues for bug tracking
- [ ] **Discussions**: Enable if you want community discussions
- [ ] **Wiki**: Enable if needed

### 8. Security Settings

- [ ] **Dependabot**: Enable for security updates
- [ ] **Secret Scanning**: Enable GitHub secret scanning
- [ ] **Code Scanning**: Consider enabling if available
- [ ] **Security Policy**: Create `SECURITY.md` if needed

## 🔍 Files to Review Before Publishing

### Backend Files
```
backend/HaryanaStatAbstract.API/appsettings.json
backend/HaryanaStatAbstract.API/appsettings.Development.json
backend/HaryanaStatAbstract.API/Data/SeedData.cs
backend/database/*.sql (check for passwords)
```

### Frontend Files
```
frontend/.env
frontend/.env.production
frontend/src/services/*.js (check for hardcoded URLs)
```

### Documentation Files
```
DEPLOYMENT_PLAN.md (check for server names)
DEPLOYMENT_QUICK_START.md (check for credentials)
*.md files (review all)
```

## 🛠️ Quick Fixes

### Replace Connection String in appsettings.json
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=HaryanaStatAbstractDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

### Replace JWT Secret
```json
"SecretKey": "CHANGE_THIS_TO_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG"
```

### Update .gitignore
Ensure these are excluded:
```
appsettings.Production.json
appsettings.*.json
!appsettings.Example.json
!appsettings.Development.json
.env.production
logs/
```

## 📝 Post-Publication

After publishing:

1. **Monitor**: Watch for any exposed secrets
2. **Update**: Keep documentation up to date
3. **Respond**: Engage with contributors and issues
4. **Maintain**: Regular updates and security patches

## ⚠️ Important Notes

- **Never commit** real credentials, even in private repos
- **Use environment variables** for sensitive configuration
- **Rotate secrets** if accidentally exposed
- **Review pull requests** carefully before merging
- **Use GitHub Secrets** for CI/CD pipelines

## 🔐 If Secrets Were Exposed

If you accidentally commit secrets:

1. **Immediately rotate** all exposed secrets
2. **Remove from Git history** using:
   ```bash
   git filter-branch --force --index-filter \
   "git rm --cached --ignore-unmatch path/to/file" \
   --prune-empty --tag-name-filter cat -- --all
   ```
3. **Force push** (if safe to do so)
4. **Notify affected parties** if necessary
5. **Review access logs** if available

---

**Status**: Ready for review
**Last Updated**: January 2025
