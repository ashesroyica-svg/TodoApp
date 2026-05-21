using Application.DTOs.Auth;
using Application.RepositoryInterfaces;
using Application.ServiceInterfaces;
using Application.Wrappers;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>Handles user registration and authentication.</summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtHelper _jwtHelper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _logger = logger;
    }

    /// <summary>Registers a new user, hashing the password with BCrypt work factor 12.</summary>
    public async Task<ApiResponse<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email, ct))
            return ApiResponse<object>.Fail("An account with this email already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.CreateAsync(user, ct);
        _logger.LogInformation("New user registered: {Email}", dto.Email);
        return ApiResponse<object>.Ok(new object(), "Registration successful.");
    }

    /// <summary>Authenticates a user and returns a JWT token on success.</summary>
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, ct);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password.");

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.Fail("This account has been deactivated.");

        var token = _jwtHelper.GenerateToken(user);

        var response = new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            UserId = user.Id
        };

        _logger.LogInformation("User authenticated: {Email}", dto.Email);
        return ApiResponse<LoginResponseDto>.Ok(response, "Login successful.");
    }
}
