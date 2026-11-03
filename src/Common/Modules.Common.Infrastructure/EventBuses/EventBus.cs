using MassTransit;

using Modules.Common.Application.EventBus;

namespace Modules.Common.Infrastructure.EventBuses;

internal sealed class EventBus(IBus bus) : IEventBus
{
	public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent
	{
		await bus.Publish(integrationEvent, cancellationToken);
	}
}
