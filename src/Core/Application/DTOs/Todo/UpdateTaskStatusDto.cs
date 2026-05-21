using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Todo;

/// <summary>Data transfer object for updating the completion status of a task.</summary>
public class UpdateTaskStatusDto
{
    [Required]
    public bool IsCompleted { get; set; }
}
