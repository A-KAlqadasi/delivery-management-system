using Microsoft.Extensions.Logging;

using Modules.Common.Application.Exceptions;
using Modules.Common.Application.MediatR;
using Modules.Common.Application.MediatR.Pipeline;

namespace Modules.Common.Application.Behaviors;

/// <summary>
/// A pipeline behavior for handling unhandled exceptions that occur during the processing of requests in the MediatR pipeline. It catches any exceptions that are not handled by other parts of the application, logs the error details using the provided logger, and then rethrows a custom EventlyException with relevant information about the request and the original exception. This behavior ensures that unhandled exceptions are properly logged and can be handled consistently across the application, providing a centralized mechanism for exception handling in the MediatR pipeline.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="logger"></param>
public class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
		: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
{
		private readonly ILogger<TRequest> _logger = logger;

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
				try
				{
						return await next();
				}
				catch (Exception ex)
				{
						string requestName = typeof(TRequest).Name;

						_logger.LogError(ex, "Request: Unhandled Exception for Request {Name} {Request}", requestName, request);

						throw new MyModularException(requestName, innerException: ex);
				}
		}
}
