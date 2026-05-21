using Domain.Entities;

namespace Application.RepositoryInterfaces;

/// <summary>Repository interface for user data access operations.</summary>
public interface IUserRepository
{
    /// <summary>Retrieves a user by their email address.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Persists a new user to the database.</summary>
    Task<User> CreateAsync(User user, CancellationToken ct = default);

    /// <summary>Checks whether an email address is already registered.</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
