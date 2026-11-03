using AlatrafClinic.Infrastructure.Data.Interceptors;

using MassTransit;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Modules.Common.Application.Clock;
using Modules.Common.Application.EventBus;
using Modules.Common.Infrastructure.Authentication;
using Modules.Common.Infrastructure.Authorization;
using Modules.Common.Infrastructure.Clock;
using Modules.Common.Infrastructure.EventBuses;
using Modules.Common.Infrastructure.Outbox;


using Quartz;

namespace Modules.Common.Infrastructure;


public static class InfrastructureConfiguration
{
	public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services)
	{
		services.AddAuthenticationInternal();
		services.AddAuthorizationInternal();


		services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

		services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
		services.TryAddScoped<AuditableEntityInterceptor>();

		// Quartz configuration with unique scheduler ID and name to avoid conflicts in tests
		services.AddQuartz(configurator =>
		{
			var scheduler = Guid.NewGuid();
			configurator.SchedulerId = $"default-id-{scheduler}";
			configurator.SchedulerName = $"default-name-{scheduler}";
		});

		services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


		services.TryAddSingleton<IEventBus, EventBus>();

		services.AddMassTransit(configure =>
		{
			// foreach (Action<IRegistrationConfigurator> configureConsumer in moduleConfigureConsumers)
			// {
			// 	configureConsumer(configure);
			// }

			configure.SetKebabCaseEndpointNameFormatter();
			configure.UsingInMemory((context, cfg) =>
			{
				cfg.ConfigureEndpoints(context);
			});
		});

		return services;
	}
}
