
using Modules.Users.Infrastructure.Database;

namespace ModularMonolith.Host.Extensions;

internal static class MigrationsExtensions
{
	public static async Task ApplyMigrations(this WebApplication app)
	{

		await app.InitialiseUsersModuleDatabaseAsync();

	}

}
