using EmployedNow.Domain.Enums;

namespace EmployedNow.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsPremium { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    public ICollection<Connection> SentConnections { get; set; } = new List<Connection>();
    public ICollection<Connection> ReceivedConnections { get; set; } = new List<Connection>();
}
