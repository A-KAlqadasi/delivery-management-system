using Microsoft.Extensions.DependencyInjection;

using Modules.Common.Domain;

namespace Modules.Common.Application.MediatR.Dispatching;

public sealed class NotificationDispatcher : INotificationDispatcher
{
		private readonly IServiceProvider _serviceProvider;

		public NotificationDispatcher(IServiceProvider serviceProvider)
		{
				_serviceProvider = serviceProvider;
		}

		public async Task Publish(
				INotification notification,
				CancellationToken cancellationToken = default)
		{
				ArgumentNullException.ThrowIfNull(notification);

				var notificationType = notification.GetType();

				var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

				var handlers = _serviceProvider
						.GetServices(handlerType)
						.Cast<object>()
						.ToList();

				foreach (var handler in handlers)
				{
						await ((dynamic)handler).Handle((dynamic)notification, cancellationToken);
				}
		}
}