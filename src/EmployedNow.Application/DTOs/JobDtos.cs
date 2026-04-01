namespace EmployedNow.Application.DTOs;

public record CreateJobRequest(string Title, string Description);
public record UpdateJobRequest(string Title, string Description);
public record JobSummaryResponse(Guid Id, string Title, string Description, Guid CompanyId, DateTime CreatedAt);
public record JobDetailResponse(Guid Id, string Title, string Description, Guid CompanyId, string CompanyEmail, DateTime CreatedAt);
public record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount);
