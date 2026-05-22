namespace Application.DTOs.Todo;

/// <summary>Data transfer object containing task statistics for the dashboard.</summary>
public class DashboardDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int RemainingTasks { get; set; }
    public int CompletedToday { get; set; }
    public double CompletedTodayPercentage { get; set; }
}
