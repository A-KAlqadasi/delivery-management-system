using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Modules.Common.Application.Behaviors;
using Modules.Common.Application.MediatR;
using Modules.Common.Application.MediatR.Dispatching;
using Modules.Common.Application.MediatR.Pipeline;
using Modules.Common.Domain;

using System.Reflection;

namespace Modules.Common.Application;

public static class ApplicationConfiguration
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services,
		Assembly[] moduleAssemblies)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(moduleAssemblies);

		var assemblies = moduleAssemblies
			.Append(typeof(ApplicationAssemblyMarker).Assembly)
			.Distinct()
			.ToArray();

		// Validators
		services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

		// Dispatchers
		services.AddSingleton<IRequestExecutionPlanCache, RequestExecutionPlanCache>();
		services.AddScoped<ISender, RequestDispatcher>();
		services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

		// Behaviors
		RegisterBehaviors(services);

		// Handlers
		services.RegisterRequestHandlers(assemblies);
		services.RegisterNotificationHandlers(assemblies);

		return services;
	}

	private static void RegisterBehaviors(IServiceCollection services)
	{
		services.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
		services.AddOpenBehavior(typeof(LoggingBehavior<,>));
		services.AddOpenBehavior(typeof(ValidationBehavior<,>));
	}

	public static IServiceCollection AddOpenBehavior(
		this IServiceCollection services,
		Type behaviorType)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(behaviorType);

		if (!behaviorType.IsGenericTypeDefinition)
		{
			throw new ArgumentException(
				"Behavior must be an open generic type.",
				nameof(behaviorType));
		}

		bool implementsPipelineBehavior = behaviorType
			.GetInterfaces()
			.Any(i =>
				i.IsGenericType &&
				i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

		if (!implementsPipelineBehavior)
		{
			throw new ArgumentException(
				$"Behavior type '{behaviorType.Name}' must implement IPipelineBehavior<,>.",
				nameof(behaviorType));
		}

		services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);

		return services;
	}

	private static IServiceCollection RegisterRequestHandlers(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));
		return services;
	}

	private static IServiceCollection RegisterNotificationHandlers(
		this IServiceCollection services,
		params Assembly[] assemblies)
	{
		RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
		return services;
	}

	private static void RegisterHandlers(
		IServiceCollection services,
		Assembly[] assemblies,
		Type openGenericHandlerType)
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(assemblies);
		ArgumentNullException.ThrowIfNull(openGenericHandlerType);

		var concreteTypes = assemblies
			.Distinct()
			.SelectMany(GetLoadableTypes)
			.Where(static type =>
				type is { IsAbstract: false, IsInterface: false } &&
				!type.ContainsGenericParameters)
			.ToArray();

		foreach (var implementationType in concreteTypes)
		{
			var serviceTypes = implementationType
				.GetInterfaces()
				.Where(i =>
					i.IsGenericType &&
					i.GetGenericTypeDefinition() == openGenericHandlerType)
				.ToArray();

			foreach (var serviceType in serviceTypes)
			{
				services.TryAddEnumerable(
					ServiceDescriptor.Scoped(serviceType, implementationType));
			}
		}
	}

	private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where(static t => t is not null)!;
		}
	}
}
