
using Microsoft.EntityFrameworkCore;

using Modules.Common.Infrastructure.Inbox;
using Modules.Common.Infrastructure.Outbox;
using Modules.Users.Domain.Users;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Infrastructure.Users;

namespace Modules.Users.Infrastructure.Database;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUsersDbContext
{
	public DbSet<User> Users { get; set; }
	public DbSet<Permission> Permissions { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema(Schemas.Users);

		modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
		modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
		modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new PermissionConfiguration());
		modelBuilder.ApplyConfiguration(new RoleConfiguration());
	}
}
