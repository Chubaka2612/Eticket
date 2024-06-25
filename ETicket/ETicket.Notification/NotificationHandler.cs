
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using ETicket.Notification.Consumer;
using ETicket.Notification.Configuration;

namespace ETicket.Notification
{
    public class NotificationHandler : BackgroundService
    {
        private readonly ILogger<NotificationHandler> _logger;
        private readonly ServiceBusReceiver _receiver;
        private readonly INotificationConsumer _notificationConsumer;

        public NotificationHandler(IAzureClientFactory<ServiceBusReceiver> clientFactory, MessageQueueConfiguration configuration, INotificationConsumer notificationConsumer, ILogger<NotificationHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _receiver = clientFactory?.CreateClient(configuration.QueueName) ?? throw new ArgumentNullException(nameof(clientFactory));
            _notificationConsumer = notificationConsumer ?? throw new ArgumentNullException(nameof(notificationConsumer));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting retriewing messages from the queue.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = await _receiver.ReceiveMessageAsync(maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: stoppingToken);
                    if (message != null)
                    {
                        _logger.LogInformation("Retriewe a message with the ID '{MessageId}'.", message.MessageId);
                        await HandleMessageAsync(message, stoppingToken);
                        _logger.LogInformation("Successfully retriewed the message with the ID '{MessageId}'.", message.MessageId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception is occured during the {Listener} execution.", nameof(NotificationHandler));
                }
            }

            _logger.LogInformation("Stopping the {Listener}.", nameof(NotificationHandler));

        }

        private async Task HandleMessageAsync(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            Messaging.Models.Notification notification;

            try
            {
                var json = message.Body.ToString();
                notification = JsonConvert.DeserializeObject<Messaging.Models.Notification>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                if (notification == null)
                {
                    _logger.LogWarning("Failed to retriewe message with the ID '{MessageId}'.", message.MessageId);
                    return;
                }

                await _notificationConsumer.ConsumeMessagesAsync(notification, cancellationToken);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message with the ID '{MessageId}' into a '{Notification}' type.", message.MessageId, typeof(Messaging.Models.Notification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception was thrown while handling the message with the ID '{MessageId}'.", message.MessageId);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping the {Listener}.", nameof(NotificationHandler));
            await _receiver.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
