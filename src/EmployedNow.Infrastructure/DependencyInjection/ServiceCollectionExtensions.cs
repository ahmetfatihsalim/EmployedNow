using EmployedNow.Application.Interfaces;
using EmployedNow.Infrastructure.Data;
using EmployedNow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployedNow.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers persistence and application service dependencies.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPremiumService, PremiumService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
