using EmployedNow.Application.DTOs;

namespace EmployedNow.Application.Interfaces;

public interface IApplicationService
{
    Task<JobApplicationResponse> ApplyAsync(Guid userId, CreateApplicationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobApplicationResponse>> GetByJobForCompanyAsync(Guid companyId, Guid jobId, CancellationToken cancellationToken = default);
}
