using Microsoft.Extensions.Logging;

using Modules.Common.Application.MediatR;
using Modules.Common.Application.MediatR.Pipeline;

namespace Modules.Common.Application.Behaviors;

public sealed partial class LoggingBehavior<TRequest, TResponse>
		: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;

		LogHandlingRequest(_logger, requestName);

		var response = await next();

		LogRequestCompleted(_logger, requestName);

		return response;
	}

	[LoggerMessage(
			EventId = 1,
			Level = LogLevel.Information,
			Message = "Handling request {RequestName}")]
	private static partial void LogHandlingRequest(ILogger logger, string requestName);

	[LoggerMessage(
			EventId = 2,
			Level = LogLevel.Information,
			Message = "Request {RequestName} completed")]
	private static partial void LogRequestCompleted(ILogger logger, string requestName);
}