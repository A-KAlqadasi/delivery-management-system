
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;

namespace Modules.Users.Infrastructure.Database;

internal sealed class UsersDbContext(
	DbContextOptions<UsersDbContext> options)
	: IdentityUserContext<User, Guid>(options), IUsersDbContext
{
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Permission> Permissions => Set<Permission>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.HasDefaultSchema(Schemas.Users);
		builder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

	}
}
