using System.Data;
using System.Data.Common;

using Dapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Modules.Common.Application.Clock;
using Modules.Common.Application.Messaging;
using Modules.Common.Domain;
using Modules.Common.Infrastructure.Data;
using Modules.Common.Infrastructure.Outbox;

using Newtonsoft.Json;

using Quartz;

namespace Modules.Users.Infrastructure.Outbox;

/// <summary>
/// A Quartz job that processes outbox messages for the Users module by retrieving unprocessed messages from the database, deserializing the contained domain events, and invoking the corresponding domain event handlers. It ensures that each message is processed only once by marking them as processed in the database after handling, and logs any exceptions that occur during processing for troubleshooting purposes.
/// </summary>
[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
	IDbConnectionFactory dbConnectionFactory,
	IServiceScopeFactory serviceScopeFactory,
	IDateTimeProvider dateTimeProvider,
	IOptions<OutboxOptions> outboxOptions,
	ILogger<ProcessOutboxJob> logger) : IJob
{
	private const string ModuleName = "Users";

	public async Task Execute(IJobExecutionContext context)
	{
		// logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);

		await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
		await using DbTransaction transaction = await connection.BeginTransactionAsync();

		IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

		foreach (OutboxMessageResponse outboxMessage in outboxMessages)
		{
			Exception? exception = null;
			try
			{

				Type? type = Type.GetType(outboxMessage.Type);

				IDomainEvent domainEvent = (IDomainEvent)JsonConvert.DeserializeObject(outboxMessage.Content, type!)!;

				using IServiceScope scope = serviceScopeFactory.CreateScope();

				IEnumerable<IDomainEventHandler> domainEventHandlers = DomainEventHandlersFactory.GetHandlers(domainEvent.GetType(), scope.ServiceProvider, Features.AssemblyReference.Assembly);

				foreach (IDomainEventHandler domainEventHandler in domainEventHandlers)
				{
					await domainEventHandler.Handle(domainEvent);
				}
			}
			catch (Exception caughtException)
			{
				logger.LogError(
					caughtException,
					"{Module} - Exception while processing outbox message {MessageId}",
					ModuleName,
					outboxMessage.Id);

				exception = caughtException;
			}

			await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
		}

		await transaction.CommitAsync();

		// logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
		IDbConnection connection,
		IDbTransaction transaction)
	{
		string sql =
			$"""
             SELECT
                id AS {nameof(OutboxMessageResponse.Id)},
                content AS {nameof(OutboxMessageResponse.Content)},
                type AS {nameof(OutboxMessageResponse.Type)}
             FROM users.outbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occurred_on_utc
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

		IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
			sql,
			transaction: transaction);

		return outboxMessages.ToList();
	}

	private async Task UpdateOutboxMessageAsync(
		IDbConnection connection,
		IDbTransaction transaction,
		OutboxMessageResponse outboxMessage,
		Exception? exception)
	{
		const string sql =
			"""
            UPDATE users.outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
            WHERE id = @Id
            """;

		await connection.ExecuteAsync(
			sql,
			new
			{
				outboxMessage.Id,
				ProcessedOnUtc = dateTimeProvider.UtcNow,
				Error = exception?.ToString()
			},
			transaction: transaction);
	}

	internal sealed record OutboxMessageResponse(Guid Id, string Content, string Type);
}

// TODO:
//1-Optimized database queries with smart indexing
//2-Improved message publishing with batching => only if the consumer has a high throughput, otherwise it adds unnecessary complexity
//3-Streamlined database updates with batching =? using many ways and one of the way id the DynamicParameters of Dapper => to reduce the number of database round-trips when marking messages as processed,
//4-Scaled out Outbox processing with parallel workers => only if the consumer has a high throughput, otherwise it adds unnecessary complexity and can lead to concurrency issues
//5-Leveraged RabbitMQ's batch publishing feature => to improve performance when publishing a large number of messages,
//6-Do cahsing for this part "Type? type = Type.GetType(outboxMessage.Type);" insted of doing rflexion every time for each message, 
//we can cache the type information in a ]concurrentDictionary] to avoid the overhead of reflection and improve performance when processing outbox messages.
