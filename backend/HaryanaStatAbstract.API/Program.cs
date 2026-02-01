using HaryanaStatAbstract.API.Configuration;
using HaryanaStatAbstract.API.Data;
using HaryanaStatAbstract.API.Middleware;
using HaryanaStatAbstract.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/haryana-stat-abstract-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Haryana Statistical Abstract API",
        Version = "v1",
        Description = "API for managing census population data from Statistical Abstract of Haryana 2023-24",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@haryanastatabstract.com"
        }
    });

    // Include XML comments for Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            // Production: Use configured origins from appsettings
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Development: localhost (Vite may use 5174, 5175 when ports in use)
            policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175", "http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configure Database - Use SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
}

// Use SQL Server for all environments
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("HaryanaStatAbstract.API");
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Register services
// Area & Population Department
builder.Services.AddScoped<HaryanaStatAbstract.API.Services.AreaAndPopulation.ITable3_2CensusPopulationService, 
    HaryanaStatAbstract.API.Services.AreaAndPopulation.Table3_2CensusPopulationService>();
// Education Department
builder.Services.AddScoped<HaryanaStatAbstract.API.Services.Education.ITable6_1InstitutionsService,
    HaryanaStatAbstract.API.Services.Education.Table6_1InstitutionsService>();
// Social Security and Social Defence Department
builder.Services.AddScoped<HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.ITable7_1SanctionedStrengthPoliceService,
    HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.Table7_1SanctionedStrengthPoliceService>();
builder.Services.AddScoped<HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.ITable7_6NoOfPrisonersClasswiseService,
    HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.Table7_6NoOfPrisonersClasswiseService>();
builder.Services.AddScoped<HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.ITable7_7PrisonerMaintenanceExpenditureService,
    HaryanaStatAbstract.API.Services.SocialSecurityAndSocialDefence.Table7_7PrisonerMaintenanceExpenditureService>();
// Legacy (to be removed after migration)
// builder.Services.AddScoped<ICensusPopulationService, CensusPopulationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IErrorLoggingService, ErrorLoggingService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Haryana Statistical Abstract API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the root
    });
}

// Middleware pipeline - Global Error Handling should be first to catch all exceptions
app.UseMiddleware<GlobalErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Apply migrations and seed data on application startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Test database connection first
        logger.LogInformation("Testing database connection...");
        var canConnect = await context.Database.CanConnectAsync();
        
        if (!canConnect)
        {
            logger.LogWarning("Cannot connect to database. The application will start but database operations may fail.");
            logger.LogWarning("Please verify:");
            logger.LogWarning("  1. Database server is running and accessible");
            logger.LogWarning("  2. Connection string is correct in appsettings.json");
            logger.LogWarning("  3. Firewall allows connections on port 1433 (SQL Server)");
            logger.LogWarning("  4. SQL Server is configured to accept remote connections");
        }
        else
        {
            // Apply pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
            
            // Seed initial data
            await SeedData.SeedAsync(context);
            logger.LogInformation("Database seeded successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database");
        logger.LogWarning("The application will continue to start, but database operations may fail.");
        logger.LogWarning("Please verify:");
        logger.LogWarning("  1. Database server is running and accessible");
        logger.LogWarning("  2. Connection string is correct in appsettings.json");
        logger.LogWarning("  3. SQL Server authentication credentials are correct");
        logger.LogWarning("  4. SQL Server is configured to accept remote connections");
        // Don't throw - allow app to start even if database is unavailable
        // This allows the API to start and show proper error messages
    }
}

Log.Information("Application started");

app.Run();