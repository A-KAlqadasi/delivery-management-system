using Modules.Common.Domain;

namespace Modules.Common.Application.Messaging;

/// <summary>
/// An abstract base class for domain event handlers that provides a common implementation for handling domain events. It defines an abstract Handle method that must be implemented by derived classes to handle specific domain events. The base class also provides a non-generic Handle method that casts the incoming IDomainEvent to the specific TDomainEvent type and calls the abstract Handle method, allowing for a consistent handling mechanism for all domain events while still enforcing type safety and separation of concerns in the implementation of individual event handlers.
/// </summary>
/// <typeparam name="TDomainEvent"></typeparam>
public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
where TDomainEvent : IDomainEvent
{
		public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);

		public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
		{
				return Handle((TDomainEvent)domainEvent, cancellationToken);
		}
}
