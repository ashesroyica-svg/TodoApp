using Domain.Entities;

namespace Application.DTOs.Todo;

/// <summary>Data transfer object representing a task in API responses.</summary>
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectColor { get; set; }
}
