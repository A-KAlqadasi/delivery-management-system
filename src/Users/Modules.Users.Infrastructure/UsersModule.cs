using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Modules.Common.Application.EventBus;
using Modules.Common.Infrastructure;
using Modules.Common.Api.Extensions;
using Modules.Common.Infrastructure.Outbox;
using Modules.Users.Features.Abstractions.Data;
using Modules.Users.Features.Abstractions.Identity;
using Modules.Users.Infrastructure.Authorization;
using Modules.Users.Infrastructure.Database;
using Modules.Users.Infrastructure.Identity;
using Modules.Users.Infrastructure.Inbox;
using Modules.Users.Infrastructure.Outbox;
using Modules.Common.Application;
using Modules.Common.Application.Authorization;
using Modules.Common.Application.Messaging;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Modules.Users.Infrastructure;


public static class UsersModule
{

	public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddDomainEventHandlers();

		services.AddIntegrationEventHandlers();
		// Commong API configurations
		services.AddEndpoints();

		// Common Application configurations
		services.AddApplication([Features.AssemblyReference.Assembly]);

		services.AddUsersInfrastructure(configuration);

		return services;
	}
	private static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IPermissionService, PermissionService>();

		services
		.AddIdentityCore<ApplicationIdentityUser>(options =>
		{
			options.User.RequireUniqueEmail = true;

			options.Password.RequiredLength = 8;
			options.Password.RequireDigit = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = false;
		})
		.AddEntityFrameworkStores<IdentityDbContext>()
		.AddDefaultTokenProviders();

		services.AddTransient<IIdentityProviderService, IdentityProviderService>();

		string databaseConnectionString = configuration.GetConnectionString("Database")!;
		// Common infrastructure configurations
		services.AddCoreInfrastructure(databaseConnectionString);

		services.AddDbContext<UsersDbContext>((sp, options) =>
			options.UseNpgsql(databaseConnectionString,
			npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
			.UseSnakeCaseNamingConvention()
			.AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>())
		);

		services.AddScoped<IUsersDbContext>(c => c.GetRequiredService<UsersDbContext>());

		services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

		services.ConfigureOptions<ConfigureProcessOutboxJob>();


		return services;
	}

	private static void AddDomainEventHandlers(this IServiceCollection services)
	{
		Type[] domainEventHandlers = Features.AssemblyReference.Assembly
			.GetTypes()
			.Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
			.ToArray();

		foreach (Type domainEventHandler in domainEventHandlers)
		{
			services.TryAddScoped(domainEventHandler);

			Type domainEvent = domainEventHandler
				.GetInterfaces()
				.Single(i => i.IsGenericType)
				.GetGenericArguments()
				.Single();

			Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

			services.Decorate(domainEventHandler, closedIdempotentHandler);
		}
	}
	private static void AddIntegrationEventHandlers(this IServiceCollection services)
	{
		Type[] integrationEventHandlers = Features.AssemblyReference.Assembly
			.GetTypes()
			.Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
			.ToArray();

		foreach (Type integrationEventHandler in integrationEventHandlers)
		{
			services.TryAddScoped(integrationEventHandler);

			Type integrationEvent = integrationEventHandler
				.GetInterfaces()
				.Single(i => i.IsGenericType)
				.GetGenericArguments()
				.Single();

			Type closedIdempotentHandler =
				typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

			services.Decorate(integrationEventHandler, closedIdempotentHandler);
		}
	}

}
