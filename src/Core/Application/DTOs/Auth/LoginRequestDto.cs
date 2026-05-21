using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

/// <summary>Data transfer object for a user login request.</summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}
