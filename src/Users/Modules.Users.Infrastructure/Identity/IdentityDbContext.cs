
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Modules.Users.Infrastructure.Identity;

internal sealed class IdentityDbContext(
	DbContextOptions<IdentityDbContext> options)
	: IdentityDbContext<ApplicationIdentityUser>(options)
{
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<ApplicationIdentityUser>()
			.HasIndex(u => u.NormalizedEmail)
			.IsUnique();
	}
}
