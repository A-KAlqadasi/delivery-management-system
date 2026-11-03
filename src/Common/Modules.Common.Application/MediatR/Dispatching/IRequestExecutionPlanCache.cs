namespace Modules.Common.Application.MediatR.Dispatching;
/// <summary>
/// Provides cached execution plans for request/response pairs.
/// </summary>
public interface IRequestExecutionPlanCache
{
		/// <summary>
		/// Gets an existing execution plan from the cache or creates and stores it if missing.
		/// </summary>
		/// <param name="requestType">The concrete runtime request type.</param>
		/// <param name="responseType">The response type.</param>
		/// <returns>A cached execution plan.</returns>
		RequestExecutionPlan GetOrAdd(Type requestType, Type responseType);
}

/// <summary>
/// Represents the cached execution metadata for a request/response pair.
/// It contains only immutable invocation metadata and compiled delegates.
/// It does not contain resolved service instances.
/// </summary>
public sealed class RequestExecutionPlan
{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestExecutionPlan"/> class.
		/// </summary>
		/// <param name="handlerServiceType">
		/// The closed generic handler service type for the request.
		/// </param>
		/// <param name="behaviorServiceType">
		/// The closed generic pipeline behavior service type for the request.
		/// </param>
		/// <param name="handlerInvoker">
		/// A compiled delegate that invokes the request handler.
		/// </param>
		/// <param name="behaviorInvoker">
		/// A compiled delegate that invokes a pipeline behavior.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when any argument is <see langword="null"/>.
		/// </exception>
		public RequestExecutionPlan(
				Type handlerServiceType,
				Type behaviorServiceType,
				UntypedHandlerInvoker handlerInvoker,
				UntypedBehaviorInvoker behaviorInvoker)
		{
				HandlerServiceType = handlerServiceType ?? throw new ArgumentNullException(nameof(handlerServiceType));
				BehaviorServiceType = behaviorServiceType ?? throw new ArgumentNullException(nameof(behaviorServiceType));
				HandlerInvoker = handlerInvoker ?? throw new ArgumentNullException(nameof(handlerInvoker));
				BehaviorInvoker = behaviorInvoker ?? throw new ArgumentNullException(nameof(behaviorInvoker));
		}

		/// <summary>
		/// Gets the closed generic request handler service type.
		/// </summary>
		public Type HandlerServiceType { get; }

		/// <summary>
		/// Gets the closed generic pipeline behavior service type.
		/// </summary>
		public Type BehaviorServiceType { get; }

		/// <summary>
		/// Gets the compiled delegate used to invoke a request handler.
		/// </summary>
		public UntypedHandlerInvoker HandlerInvoker { get; }

		/// <summary>
		/// Gets the compiled delegate used to invoke a pipeline behavior.
		/// </summary>
		public UntypedBehaviorInvoker BehaviorInvoker { get; }
}

/// <summary>
/// Represents an untyped request handler invoker.
/// </summary>
/// <param name="handler">The resolved handler instance.</param>
/// <param name="request">The request instance.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The boxed handler response.</returns>
public delegate Task<object?> UntypedHandlerInvoker(
		object handler,
		object request,
		CancellationToken cancellationToken);

/// <summary>
/// Represents an untyped pipeline behavior invoker.
/// </summary>
/// <param name="behavior">The resolved behavior instance.</param>
/// <param name="request">The request instance.</param>
/// <param name="next">The next delegate in the pipeline.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>The boxed pipeline response.</returns>
public delegate Task<object?> UntypedBehaviorInvoker(
		object behavior,
		object request,
		Func<Task<object?>> next,
		CancellationToken cancellationToken);