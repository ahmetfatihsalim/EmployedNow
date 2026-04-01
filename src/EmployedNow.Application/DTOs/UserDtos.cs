using EmployedNow.Domain.Enums;

namespace EmployedNow.Application.DTOs;

public record UserSummaryResponse(Guid Id, string Email, UserRole Role, bool IsPremium, DateTime CreatedAt);
public record UserProfileResponse(Guid Id, string Email, UserRole Role, bool IsPremium, DateTime CreatedAt);
public record UpdatePremiumRequest(bool IsPremium);
