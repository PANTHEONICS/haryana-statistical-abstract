# Contributing to Haryana Statistical Abstract

Thank you for your interest in contributing to the Haryana Statistical Abstract Management System! This document provides guidelines and instructions for contributing.

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Respect different viewpoints and experiences

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in the Issues section
2. If not, create a new issue with:
   - Clear title and description
   - Steps to reproduce
   - Expected vs actual behavior
   - Environment details (OS, .NET version, Node version)
   - Screenshots if applicable

### Suggesting Features

1. Check if the feature has already been suggested
2. Create a new issue with:
   - Clear description of the feature
   - Use case and benefits
   - Possible implementation approach (if you have ideas)

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**:
   - Follow the coding standards
   - Write or update tests
   - Update documentation if needed

4. **Commit your changes**:
   ```bash
   git commit -m "Add: Description of your changes"
   ```
   Use clear, descriptive commit messages.

5. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request**:
   - Provide a clear title and description
   - Reference any related issues
   - Wait for review and feedback

## Development Setup

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+
- SQL Server (local or remote)
- Git

### Setup Steps

1. Fork and clone the repository
2. Set up the database (see README.md)
3. Configure `appsettings.json` (copy from `appsettings.Example.json`)
4. Run migrations: `dotnet ef database update`
5. Start backend: `dotnet run`
6. Start frontend: `npm run dev`

## Coding Standards

### C# (Backend)

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML comments for public APIs
- Keep methods focused and small
- Use async/await for I/O operations
- Handle exceptions appropriately

Example:
```csharp
/// <summary>
/// Retrieves census population data by year.
/// </summary>
/// <param name="year">The census year</param>
/// <returns>The census population record</returns>
public async Task<Table3_2CensusPopulation?> GetByYearAsync(int year)
{
    return await _context.Table3_2CensusPopulation
        .FirstOrDefaultAsync(c => c.Year == year);
}
```

### JavaScript/React (Frontend)

- Use functional components with hooks
- Follow React best practices
- Use meaningful component and variable names
- Keep components small and focused
- Use TypeScript for type safety (if applicable)
- Follow ESLint rules

Example:
```jsx
/**
 * Census Population Table Component
 * Displays census data in a table format
 */
const CensusPopulationTable = ({ data, onEdit, onDelete }) => {
  // Component implementation
};
```

## Testing

- Write unit tests for new features
- Ensure existing tests pass
- Test edge cases and error scenarios
- Test in both development and production-like environments

## Documentation

- Update README.md if adding new features
- Add comments for complex logic
- Update API documentation (Swagger comments)
- Document breaking changes

## Commit Message Guidelines

Use clear, descriptive commit messages:

- **Add**: New feature
- **Fix**: Bug fix
- **Update**: Update existing feature
- **Remove**: Remove feature or code
- **Refactor**: Code refactoring
- **Docs**: Documentation changes
- **Style**: Code style changes (formatting, etc.)
- **Test**: Adding or updating tests

Examples:
```
Add: User management API endpoints
Fix: CORS configuration for production
Update: Database migration scripts
Refactor: Extract common validation logic
```

## Review Process

1. All pull requests require at least one review
2. Address review comments promptly
3. Keep discussions constructive and focused
4. Be patient - maintainers are volunteers

## Questions?

If you have questions:
- Open a discussion in GitHub Discussions
- Ask in an issue
- Contact the maintainers

Thank you for contributing! 🎉
