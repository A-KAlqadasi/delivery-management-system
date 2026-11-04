using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Delivery.Infrastructure.Database;


internal sealed class DeliveryDatabaseInitializer(DeliveryDbContext context, ILogger<DeliveryDatabaseInitializer> logger)
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

	// public async Task SeedAsync()
	// {
	// 	try
	// 	{
	// 		await TrySeedAsync();
	// 	}
	// 	catch (Exception)
	// 	{
	// 		logger.LogError("An error occurred while seeding the database");
	// 		throw;
	// 	}
	// }
	// 	private static async Task TrySeedAsync()
	// 	{
	// 		return Task.CompletedTask;
	// 	}
	// }
}
public static class InitialiserExtensions
{
	public static async Task InitialiseDeliveryModuleDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		var initialiser = scope.ServiceProvider.GetRequiredService<DeliveryDatabaseInitializer>();

		await initialiser.InitialiseAsync();
		//  await initialiser.SeedAsync();
	}
}
