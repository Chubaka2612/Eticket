
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ETicket.Bll.Services.Notifications
{
    public class NotificationProducer : INotificationProducer
    {
        // the client that owns the connection and can be used to create senders and receivers
        private readonly ServiceBusClient _busClient;
        private readonly ILogger<NotificationProducer> _logger;

        public NotificationProducer(ServiceBusClient busClient, ILogger<NotificationProducer> logger)
        {
            _busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishMessageAsync<T>(T message, string queueName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));
            }

            ServiceBusSender sender = null;

            try
            {
                sender = _busClient.CreateSender(queueName);

                var serializedMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                var queueMessage = new ServiceBusMessage(new BinaryData(serializedMessage));

                await sender.SendMessageAsync(queueMessage, cancellationToken);
                _logger.LogInformation("A message was sent to the queue '{QueueName}'", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred when sending a message to the queue '{QueueName}'", queueName);
                throw;
            }
        }
    }
}
