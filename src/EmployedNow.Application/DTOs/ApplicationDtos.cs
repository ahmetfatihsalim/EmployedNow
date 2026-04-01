namespace EmployedNow.Application.DTOs;

public record CreateApplicationRequest(Guid JobId);
public record JobApplicationResponse(Guid Id, Guid JobPostingId, Guid UserId, string UserEmail, DateTime AppliedAt);
