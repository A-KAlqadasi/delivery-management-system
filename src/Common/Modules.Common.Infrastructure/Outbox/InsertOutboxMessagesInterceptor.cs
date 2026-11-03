using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Modules.Common.Domain;

using Newtonsoft.Json;

namespace Modules.Common.Infrastructure.Outbox;

/// <summary>
/// An interceptor that listens for changes in the DbContext and automatically inserts outbox messages for any domain events that are generated during the saving of entities. It intercepts the SavingChangesAsync event of the DbContext, retrieves all entities that have domain events, extracts those events, and creates corresponding outbox messages that are then added to the database context. This allows for a seamless integration of the outbox pattern, ensuring that all domain events are captured and stored as outbox messages without requiring explicit handling in the application code. The interceptor ensures that outbox messages are created for all relevant domain events, enabling reliable event-driven communication and eventual consistency across different parts of the application.
/// </summary>
public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
		{
			InsertOutboxMessages(eventData.Context);
		}
		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}


	private static void InsertOutboxMessages(DbContext context)
	{
		List<OutboxMessage> outboxMessages = context
			.ChangeTracker
			.Entries<Entity>()
			.Select(entry => entry.Entity)
			.SelectMany(entity =>
			{
				IReadOnlyCollection<IDomainEvent> domainEvents = entity.DomainEvents.ToList();

				entity.ClearDomainEvents();

				return domainEvents;
			})
			.Select(domainEvent => new OutboxMessage
			{
				Id = domainEvent.Id,
				OccurredOnUtc = domainEvent.OccurredOnUtc,
				Type = domainEvent.GetType().AssemblyQualifiedName!,
				Content = JsonConvert.SerializeObject(domainEvent)
			})
			.ToList();
		context.Set<OutboxMessage>().AddRange(outboxMessages);
	}
}
