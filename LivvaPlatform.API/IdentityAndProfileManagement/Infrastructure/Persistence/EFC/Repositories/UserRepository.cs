using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Model.Aggregates;
using LivvaPlatform.API.IdentityAndProfileManagement.Domain.Repositories;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using LivvaPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LivvaPlatform.API.IdentityAndProfileManagement.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for user persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await Context.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await Context.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);
}