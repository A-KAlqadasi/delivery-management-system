using Modules.Common.Application.Clock;

namespace Modules.Common.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
}
