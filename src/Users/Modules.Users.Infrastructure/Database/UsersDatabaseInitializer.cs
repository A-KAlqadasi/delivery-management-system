using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Modules.Users.Domain.Users;

namespace Modules.Users.Infrastructure.Database;


internal sealed class UsersDatabaseInitializer(UsersDbContext context, UserManager<User> userManager, ILogger<UsersDatabaseInitializer> logger)
{
	public async Task InitialiseAsync(CancellationToken ct = default)
	{
		try
		{
			await context.Database.MigrateAsync(ct);
		}
		catch (Exception)
		{
			logger.LogError("An error occurred while initialising the database.");
			throw;
		}
	}

	public async Task SeedAsync()
	{
		try
		{
			await TrySeedAsync();
		}
		catch (Exception)
		{
			logger.LogError("An error occurred while seeding the database");
			throw;
		}
	}
	private async Task TrySeedAsync()
	{
		string email = "admin@localhost";
		string firstName = "Admin";
		string lastName = "Admin";
		string password = "Admin123!";
		if (await userManager.Users.AnyAsync())
		{
			return;
		}

		Role? adminRole = await context.Roles
			.FirstOrDefaultAsync(r => r.Name == Role.Admin.Name);

		if (adminRole is null)
		{
			throw new InvalidOperationException("Admin role was not found.");
		}

		var user = User.Create(
			email,
			firstName,
			lastName,
			adminRole);

		IdentityResult result = await userManager.CreateAsync(user.Value, password);

		if (!result.Succeeded)
		{
			throw new InvalidOperationException(
				string.Join(", ", result.Errors.Select(e => e.Description)));
		}


	}
}

public static class InitialiserExtensions
{
	public static async Task InitialiseUsersModuleDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var initialiser = scope.ServiceProvider.GetRequiredService<UsersDatabaseInitializer>();

		await initialiser.InitialiseAsync();
		await initialiser.SeedAsync();
	}
}
