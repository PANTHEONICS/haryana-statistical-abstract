# Security Policy

## Supported Versions

We actively support the following versions with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability, please **do not** open a public issue. Instead, please report it via one of the following methods:

1. **Email**: Send details to [security@yourdomain.com] (replace with your security email)
2. **Private Security Advisory**: Use GitHub's private vulnerability reporting feature (if enabled)

### What to Include

When reporting a vulnerability, please include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if you have one)
- Your contact information

### Response Time

We aim to:
- Acknowledge receipt within 48 hours
- Provide initial assessment within 7 days
- Provide regular updates on progress
- Release a fix within 30 days (depending on severity)

## Security Best Practices

### For Developers

1. **Never commit secrets**: Use environment variables or secure configuration management
2. **Keep dependencies updated**: Regularly update NuGet packages and npm packages
3. **Use strong passwords**: Enforce password complexity requirements
4. **Enable HTTPS**: Always use HTTPS in production
5. **Review code**: Conduct security code reviews before merging

### For Administrators

1. **Change default credentials**: Immediately change all default passwords
2. **Use strong JWT secrets**: Generate strong, random secret keys
3. **Restrict database access**: Use least-privilege database accounts
4. **Enable firewall rules**: Restrict access to necessary ports only
5. **Regular backups**: Maintain regular database backups
6. **Monitor logs**: Regularly review application and security logs
7. **Update regularly**: Keep the application and dependencies updated

## Known Security Considerations

### Development Endpoints

The following endpoints are for development/testing purposes and should be **removed or secured** in production:

- `POST /api/UserManagement/fix-admin-password` - Allows anonymous password reset
- `POST /api/UserManagement/fix-all-test-passwords` - Allows anonymous password reset for test users

**Recommendation**: 
- Remove these endpoints in production, OR
- Secure them with `[Authorize(Roles = "SystemAdmin")]` and remove `[AllowAnonymous]`

### Default Credentials

The application includes default test users with known passwords:
- Login ID: `admin`
- Password: `Admin@123`

**Action Required**: Change all default passwords immediately after deployment.

### Configuration Files

- `appsettings.json` contains placeholder values - ensure production values are secure
- `appsettings.Production.json` should never be committed to version control
- Use environment variables or secure vaults for production secrets

## Security Checklist

Before deploying to production:

- [ ] Change all default passwords
- [ ] Generate strong JWT secret key (32+ characters)
- [ ] Remove or secure development endpoints
- [ ] Enable HTTPS/SSL
- [ ] Configure CORS to restrict origins
- [ ] Disable Swagger in production
- [ ] Review and restrict database user permissions
- [ ] Enable firewall rules
- [ ] Set up log monitoring
- [ ] Configure backup strategy
- [ ] Review and update dependencies
- [ ] Conduct security audit

## Dependency Security

We use the following tools to monitor dependencies:

- **.NET**: Built-in security scanning
- **npm**: `npm audit` for vulnerability scanning

Regularly run:
```bash
# Backend
dotnet list package --vulnerable

# Frontend
npm audit
```

## Data Protection

This application handles statistical data. Ensure compliance with:

- Data protection regulations (GDPR, local data protection laws)
- Government data handling policies
- Access control and audit requirements

## Contact

For security concerns, contact: [your-security-email@domain.com]

---

**Last Updated**: January 2025
