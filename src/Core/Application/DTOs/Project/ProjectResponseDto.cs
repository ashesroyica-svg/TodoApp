namespace Application.DTOs.Project;

/// <summary>Data transfer object representing a project in API responses.</summary>
public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
