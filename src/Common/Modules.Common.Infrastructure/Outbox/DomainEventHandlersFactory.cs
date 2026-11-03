using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Modules.Common.Application.Messaging;

namespace Modules.Common.Infrastructure.Outbox;

/// <summary>
/// Factory for creating domain event handlers. It uses a concurrent dictionary to cache the handlers for each domain event type and assembly combination.
/// </summary>
public static class DomainEventHandlersFactory
{
	private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new ConcurrentDictionary<string, Type[]>(StringComparer.Ordinal);

	public static IEnumerable<IDomainEventHandler> GetHandlers(
		Type type,
		IServiceProvider serviceProvider,
		Assembly assembly)
	{
		Type[] domainEventHandlerTypes = HandlersDictionary.GetOrAdd(
			assembly.GetName().Name + type.Name,
			_ =>
			{
				Type[] domainEventHandlerTypes = assembly.GetTypes()
					.Where(t => t.IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(type)))
					.ToArray();

				return domainEventHandlerTypes;
			});

		List<IDomainEventHandler> handlers = [];
		foreach (Type domainEventHandlerType in domainEventHandlerTypes)
		{
			object domainEventHandler = serviceProvider.GetRequiredService(domainEventHandlerType);

			handlers.Add((domainEventHandler as IDomainEventHandler)!);
		}

		return handlers;
	}
}
