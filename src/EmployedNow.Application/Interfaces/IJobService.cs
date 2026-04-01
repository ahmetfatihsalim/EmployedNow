using EmployedNow.Application.DTOs;

namespace EmployedNow.Application.Interfaces;

public interface IJobService
{
    Task<JobSummaryResponse> CreateAsync(Guid companyId, CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<JobSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<JobDetailResponse> GetByIdAsync(Guid jobId, CancellationToken cancellationToken = default);
}
