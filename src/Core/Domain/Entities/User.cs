namespace Domain.Entities;

/// <summary>Represents an ICA application user.</summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
