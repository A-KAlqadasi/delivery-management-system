using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Modules.Common.Application.MediatR.Pipeline;

namespace Modules.Common.Application.MediatR.Dispatching;
/// <summary>
/// Caches immutable execution plans for request/response pairs.
/// Each plan contains compiled delegates and reusable type metadata only.
/// No resolved DI instances are cached.
/// </summary>
public sealed class RequestExecutionPlanCache : IRequestExecutionPlanCache
{
		private static readonly ConcurrentDictionary<ExecutionPlanCacheKey, RequestExecutionPlan> Cache = new();

		/// <inheritdoc />
		public RequestExecutionPlan GetOrAdd(Type requestType, Type responseType)
		{
				ArgumentNullException.ThrowIfNull(requestType);
				ArgumentNullException.ThrowIfNull(responseType);

				var key = new ExecutionPlanCacheKey(requestType, responseType);

				return Cache.GetOrAdd(key, static key => BuildExecutionPlan(key.RequestType, key.ResponseType));
		}

		private static RequestExecutionPlan BuildExecutionPlan(Type requestType, Type responseType)
		{
				Type handlerServiceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
				Type behaviorServiceType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);

				UntypedHandlerInvoker handlerInvoker = BuildHandlerInvoker(requestType, responseType, handlerServiceType);
				UntypedBehaviorInvoker behaviorInvoker = BuildBehaviorInvoker(requestType, responseType, behaviorServiceType);

				return new RequestExecutionPlan(
						handlerServiceType,
						behaviorServiceType,
						handlerInvoker,
						behaviorInvoker);
		}

		private static UntypedHandlerInvoker BuildHandlerInvoker(
				Type requestType,
				Type responseType,
				Type handlerServiceType)
		{
				MethodInfo handleMethod = handlerServiceType.GetMethod(
						nameof(IRequestHandler<IRequest<object>, object>.Handle),
						[requestType, typeof(CancellationToken)])
						?? throw new InvalidOperationException(
								$"Could not find Handle method on handler type '{handlerServiceType.FullName}'.");

				ParameterExpression handlerParameter = Expression.Parameter(typeof(object), "handler");
				ParameterExpression requestParameter = Expression.Parameter(typeof(object), "request");
				ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

				UnaryExpression castedHandler = Expression.Convert(handlerParameter, handlerServiceType);
				UnaryExpression castedRequest = Expression.Convert(requestParameter, requestType);

				MethodCallExpression handleCall = Expression.Call(
						castedHandler,
						handleMethod,
						castedRequest,
						cancellationTokenParameter);

				MethodInfo boxTaskResultMethod = typeof(RequestExecutionPlanCache)
								.GetMethod(nameof(BoxTaskResultAsync), BindingFlags.Static | BindingFlags.Public)!
								.MakeGenericMethod(responseType);

				MethodCallExpression body = Expression.Call(boxTaskResultMethod, handleCall);

				return Expression
						.Lambda<UntypedHandlerInvoker>(body, handlerParameter, requestParameter, cancellationTokenParameter)
						.Compile();
		}

		private static UntypedBehaviorInvoker BuildBehaviorInvoker(
				Type requestType,
				Type responseType,
				Type behaviorServiceType)
		{
				Type requestHandlerDelegateType = typeof(RequestHandlerDelegate<>).MakeGenericType(responseType);

				MethodInfo handleMethod = behaviorServiceType.GetMethod(
						nameof(IPipelineBehavior<IRequest<object>, object>.Handle),
						[requestType, requestHandlerDelegateType, typeof(CancellationToken)])
						?? throw new InvalidOperationException(
								$"Could not find Handle method on behavior type '{behaviorServiceType.FullName}'.");

				ParameterExpression behaviorParameter = Expression.Parameter(typeof(object), "behavior");
				ParameterExpression requestParameter = Expression.Parameter(typeof(object), "request");
				ParameterExpression nextParameter = Expression.Parameter(typeof(Func<Task<object?>>), "next");
				ParameterExpression cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

				UnaryExpression castedBehavior = Expression.Convert(behaviorParameter, behaviorServiceType);
				UnaryExpression castedRequest = Expression.Convert(requestParameter, requestType);

				MethodInfo adaptNextMethod = typeof(RequestExecutionPlanCache)
						.GetMethod(nameof(AdaptNext), BindingFlags.Static | BindingFlags.Public)!
						.MakeGenericMethod(responseType);

				MethodCallExpression typedNextDelegate = Expression.Call(adaptNextMethod, nextParameter);

				MethodCallExpression handleCall = Expression.Call(
						castedBehavior,
						handleMethod,
						castedRequest,
						typedNextDelegate,
						cancellationTokenParameter);

				MethodInfo boxTaskResultMethod = typeof(RequestExecutionPlanCache)
						 .GetMethod(nameof(BoxTaskResultAsync), BindingFlags.Static | BindingFlags.Public)!
						 .MakeGenericMethod(responseType);

				MethodCallExpression body = Expression.Call(boxTaskResultMethod, handleCall);

				return Expression
						.Lambda<UntypedBehaviorInvoker>(body, behaviorParameter, requestParameter, nextParameter, cancellationTokenParameter)
						.Compile();
		}

		/// <summary>
		/// Boxes a typed task result into <see cref="object"/>.
		/// </summary>
		public static async Task<object?> BoxTaskResultAsync<TResponse>(Task<TResponse> task)
		{
				return await task.ConfigureAwait(false);
		}

		/// <summary>
		/// Adapts an untyped next delegate to a typed <see cref="RequestHandlerDelegate{TResponse}"/>.
		/// </summary>
		public static RequestHandlerDelegate<TResponse> AdaptNext<TResponse>(Func<Task<object?>> next)
		{
				ArgumentNullException.ThrowIfNull(next);

				return async () =>
				{
						object? result = await next().ConfigureAwait(false);

						if (result is null)
						{
								return default!;
						}

						return (TResponse)result;
				};
		}

		private readonly record struct ExecutionPlanCacheKey(Type RequestType, Type ResponseType);
}