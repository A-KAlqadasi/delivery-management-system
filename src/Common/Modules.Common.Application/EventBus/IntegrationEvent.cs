namespace Modules.Common.Application.EventBus;

/// <summary>
/// An abstract base class for integration events that implements the IIntegrationEvent interface. 
/// It provides a common implementation for integration events, including properties 
/// for the unique identifier of the event (Id) and the timestamp of when the event occurred (OccurredOnUtc). 
/// This base class can be extended by specific integration event classes to include additional properties 
/// and logic relevant to the specific event being represented. 
/// By inheriting from this base class, all integration events in the application will have a consistent 
/// structure and can be easily identified and processed by event handlers and the event bus, 
/// facilitating decoupled communication and event-driven architecture across different parts of the application.
/// </summary>
public abstract class IntegrationEvent : IIntegrationEvent
{

		protected IntegrationEvent(Guid id, DateTime occurredOnUtc)
		{
				Id = id;
				OccurredOnUtc = occurredOnUtc;
		}
		public Guid Id { get; init; }
		public DateTime OccurredOnUtc { get; init; }
}
