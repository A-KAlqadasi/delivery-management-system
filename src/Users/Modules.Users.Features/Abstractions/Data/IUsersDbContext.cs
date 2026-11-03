using Microsoft.EntityFrameworkCore;

using Modules.Users.Domain.Users;

namespace Modules.Users.Features.Abstractions.Data;

public interface IUsersDbContext
{
	DbSet<User> Users { get; }
	DbSet<Role> Roles { get; }
	DbSet<Permission> Permissions { get; }
	DbSet<RefreshToken> RefreshTokens { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
