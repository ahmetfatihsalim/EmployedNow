using EmployedNow.Application.Common;
using EmployedNow.Application.DTOs;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Entities;
using EmployedNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployedNow.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Registers a new account with hashed password and immediately returns a JWT.
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Normalizes email for consistent uniqueness checks.
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ApiException("A valid email address is required.", 400);
        }

        if (request.Password.Length < 6)
        {
            throw new ApiException("Password must be at least 6 characters.", 400);
        }

        var exists = await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (exists)
        {
            throw new ApiException("Email is already registered.", 409);
        }

        // Stores a one-way BCrypt hash instead of raw password.
        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsPremium = false
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email, user.Role, user.IsPremium);
    }

    /// <summary>
    /// Validates credentials and returns a JWT for authenticated access.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        // Verifies raw password against stored BCrypt hash.
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new ApiException("Invalid email or password.", 401);
        }

        var token = _tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Id, user.Email, user.Role, user.IsPremium);
    }
}
