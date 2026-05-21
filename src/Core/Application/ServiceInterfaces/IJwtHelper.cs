using Domain.Entities;

namespace Application.ServiceInterfaces;

/// <summary>Abstraction for generating signed JWT tokens — implemented in Infrastructure.</summary>
public interface IJwtHelper
{
    /// <summary>Generates a signed JWT token for the given user.</summary>
    string GenerateToken(User user);
}
