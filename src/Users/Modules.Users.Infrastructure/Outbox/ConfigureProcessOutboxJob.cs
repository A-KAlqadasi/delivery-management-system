using Microsoft.Extensions.Options;

using Quartz;

namespace Modules.Users.Infrastructure.Outbox;
/// <summary>
/// A Quartz job configuration class that sets up the ProcessOutboxJob to run at a specified interval defined in the OutboxOptions. It ensures that the job is scheduled to execute periodically to process outbox messages for the Users module, allowing for reliable and timely handling of domain events contained in the outbox messages.
/// </summary>
internal sealed class ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions)
	: IConfigureOptions<QuartzOptions>
{
	private readonly OutboxOptions _outboxOptions = outboxOptions.Value;

	public void Configure(QuartzOptions options)
	{
		string jobName = typeof(ProcessOutboxJob).FullName!;

		options
			.AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
			.AddTrigger(configure =>
				configure
					.ForJob(jobName)
					.WithSimpleSchedule(schedule =>
						schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
	}
}
