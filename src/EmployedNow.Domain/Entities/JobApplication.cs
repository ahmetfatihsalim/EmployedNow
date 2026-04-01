namespace EmployedNow.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobPostingId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public JobPosting JobPosting { get; set; } = null!;
    public User User { get; set; } = null!;
}
