namespace Modules.Common.Application.EventBus;

/// <summary>
/// An interface representing an integration event in the application. It defines properties for the unique identifier of the event (Id) and the timestamp of when the event occurred (OccurredOnUtc). This interface serves as a contract for implementing integration events that can be published and consumed across different parts of the application, allowing for decoupled communication and event-driven architecture. Implementing this interface ensures that all integration events have a consistent structure and can be easily identified and processed by event handlers and the event bus.
/// </summary>
public interface IIntegrationEvent
{
		Guid Id { get; }
		DateTime OccurredOnUtc { get; }
}
