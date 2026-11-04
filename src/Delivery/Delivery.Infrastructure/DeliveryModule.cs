using Microsoft.EntityFrameworkCore;

using Delivery.Infrastructure.Database;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Modules.Common.Api.Extensions;
using Modules.Common.Application;
using Microsoft.EntityFrameworkCore.Migrations;
using Delivery.Features.Abstractions;

namespace Delivery.Infrastructure;

public static class DeliveryModule
{

	public static IServiceCollection AddDeliveryModule(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddEndpoints();

		// Common Application configurations
		services.AddApplication([Features.AssemblyReference.Assembly]);

		string databaseConnectionString = configuration.GetConnectionString("Database")!;
		services.AddDbContext<DeliveryDbContext>((sp, options) =>
					options.UseSqlServer(databaseConnectionString,
					sqlServerOptions => sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Delivery))
					.UseUpperCaseNamingConvention()
				);

		services.AddScoped<IDeliveryDbContext>(c => c.GetRequiredService<DeliveryDbContext>());
		services.AddScoped<DeliveryDbContext>();

		return services;
	}
}
