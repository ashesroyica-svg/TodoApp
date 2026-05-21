namespace Application.DTOs.Todo;

/// <summary>Data transfer object representing a task in API responses.</summary>
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Task { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}
