namespace Modules.Common.Domain;
public interface INotificationDispatcher
{
    Task Publish(
        INotification notification,
        CancellationToken cancellationToken = default);
}