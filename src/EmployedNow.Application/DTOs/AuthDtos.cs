using EmployedNow.Domain.Enums;

namespace EmployedNow.Application.DTOs;

public record RegisterRequest(string Email, string Password, UserRole Role);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, Guid UserId, string Email, UserRole Role, bool IsPremium);
