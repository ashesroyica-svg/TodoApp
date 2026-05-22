namespace Application.DTOs.Todo;

/// <summary>A compact project summary used inside the dashboard response.</summary>
public class ProjectSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedCount { get; set; }
    public double CompletionPercentage { get; set; }
}
