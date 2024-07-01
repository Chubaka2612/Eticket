namespace IWent.Services.Notifications.Configuration;

public class ServiceBusConfiguration
{
    public string Namespace { get; set; }

    public string QueueName { get; set; }

    public string ConnectionString { get; set; }
}
