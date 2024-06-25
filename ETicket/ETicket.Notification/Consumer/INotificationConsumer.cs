
namespace ETicket.Notification.Consumer
{
    public interface INotificationConsumer
    {
        Task ConsumeMessagesAsync(Messaging.Models.Notification notification, CancellationToken cancellationToken);
    }
}
