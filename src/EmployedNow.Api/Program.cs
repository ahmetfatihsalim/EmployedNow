using System.Text;
using System.Text.Json.Serialization;
using EmployedNow.Api.Middleware;
using EmployedNow.Infrastructure.Data;
using EmployedNow.Infrastructure.DependencyInjection;
using EmployedNow.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Normalizes SQLite data source to an absolute path for stable local startup.
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=data/employednow.db";
var sqliteBuilder = new SqliteConnectionStringBuilder(rawConnectionString);
if (string.IsNullOrWhiteSpace(sqliteBuilder.DataSource))
{
    sqliteBuilder.DataSource = "data/employednow.db";
}

var absoluteDbPath = Path.IsPathRooted(sqliteBuilder.DataSource)
    ? sqliteBuilder.DataSource
    : Path.GetFullPath(sqliteBuilder.DataSource, builder.Environment.ContentRootPath);

var dbDirectory = Path.GetDirectoryName(absoluteDbPath);
if (!string.IsNullOrWhiteSpace(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

sqliteBuilder.DataSource = absoluteDbPath;
builder.Configuration["ConnectionStrings:DefaultConnection"] = sqliteBuilder.ToString();

// Registers Infrastructure dependencies (DbContext + application services).
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keeps enum values human-readable for frontend contracts ("User"/"Company").
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

var corsOriginsRaw = builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:5173";
var allowedOrigins = corsOriginsRaw
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .ToArray();

// Enables browser requests from configured frontend origins.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configures Swagger with JWT Bearer security metadata for authenticated endpoints.
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EmployedNow API",
        Version = "v1",
        Description = "LinkedIn-like MVP Web API built with .NET 8"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Input JWT token in the format: Bearer {token}",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [securityScheme] = Array.Empty<string>()
    });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EmployedNow";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EmployedNowClients";

// Configures JWT validation using issuer, audience, and signing key from configuration.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        // Applies migrations automatically in normal runtime.
        await dbContext.Database.MigrateAsync();
    }
    catch (InvalidOperationException)
    {
        // Falls back to EnsureCreated when migrations metadata is unavailable.
        await dbContext.Database.EnsureCreatedAsync();
    }

    // Seeds deterministic sample data for immediate API testing.
    await SeedData.InitializeAsync(dbContext);
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
