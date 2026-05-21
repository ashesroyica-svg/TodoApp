namespace Domain.Entities;

/// <summary>Represents a user's task item.</summary>
public class TaskItem
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
