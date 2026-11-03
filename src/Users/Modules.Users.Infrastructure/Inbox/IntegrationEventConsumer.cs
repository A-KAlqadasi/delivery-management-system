using System.Data.Common;

using Dapper;

using MassTransit;

using Modules.Common.Application.EventBus;
using Modules.Common.Infrastructure.Data;
using Modules.Common.Infrastructure.Inbox;

using Newtonsoft.Json;

namespace Modules.Users.Infrastructure.Inbox;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory dbConnectionFactory)
	: IConsumer<TIntegrationEvent>
	where TIntegrationEvent : IntegrationEvent
{
	public async Task Consume(ConsumeContext<TIntegrationEvent> context)
	{
		await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

		TIntegrationEvent integrationEvent = context.Message;

		InboxMessage inboxMessage = new InboxMessage
		{
			Id = integrationEvent.Id,
			Type = integrationEvent.GetType().AssemblyQualifiedName!,
			Content = JsonConvert.SerializeObject(integrationEvent),
			OccurredOnUtc = integrationEvent.OccurredOnUtc
		};

		const string sql =
			"""
            INSERT INTO users.inbox_messages(id, type, content, occurred_on_utc)
            VALUES (@Id, @Type, @Content::json, @OccurredOnUtc)
            """;

		await connection.ExecuteAsync(sql, inboxMessage);
	}
}
