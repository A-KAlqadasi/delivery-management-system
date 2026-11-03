namespace Modules.Common.Application.EventBus;

/// <summary>
/// An interface for an event bus that allows for publishing integration events in the application. It defines a generic PublishAsync method that takes an integration event of type T and a cancellation token, allowing for asynchronous publishing of events. The method is designed to be flexible and can be implemented using various messaging technologies or frameworks, enabling decoupled communication between different parts of the application through the use of integration events. This interface serves as a contract for implementing an event bus that can be used to publish events in a consistent and organized manner across the application.
/// </summary>
public interface IEventBus
{
		Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}
