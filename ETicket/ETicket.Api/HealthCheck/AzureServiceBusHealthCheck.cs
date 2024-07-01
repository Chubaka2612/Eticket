using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ETicket.Api.HealthCheck
{
    public class AzureServiceBusHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusHealthCheck(string connectionString, string queueName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var client = new ServiceBusClient(_connectionString);
                var sender = client.CreateSender(_queueName);

                // Send a test message to check connectivity
                var message = new ServiceBusMessage("Health Check Message");
                await sender.SendMessageAsync(message, cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Failed to connect to Service Bus: {ex.Message}");
            }
        }
    }
}