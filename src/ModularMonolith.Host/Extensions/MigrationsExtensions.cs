
using Microsoft.EntityFrameworkCore;

using Modules.Users.Infrastructure.Database;

namespace ModularMonolith.Host.Extensions;

internal static class MigrationsExtensions
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using IServiceScope scope = app.ApplicationServices.CreateScope();
		ApplyMigrations<UsersDbContext>(scope);

	}

	private static void ApplyMigrations<TDbContext>(IServiceScope scope)
		where TDbContext : DbContext
	{
		using TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
		context.Database.Migrate();

	}
}
