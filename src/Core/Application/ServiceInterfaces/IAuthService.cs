using Application.DTOs.Auth;
using Application.Wrappers;

namespace Application.ServiceInterfaces;

/// <summary>Service interface for authentication operations.</summary>
public interface IAuthService
{
    /// <summary>Registers a new user account.</summary>
    Task<ApiResponse<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);

    /// <summary>Authenticates a user and returns a signed JWT token.</summary>
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
}
