namespace Application.DTOs.Auth;

/// <summary>Data transfer object returned on successful authentication.</summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int UserId { get; set; }
}
