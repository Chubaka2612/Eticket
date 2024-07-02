
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ETicket.Notification.Configuration;
using ETicket.Notification.Consumer;
using ETicket.Notification.Email.Client;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

namespace ETicket.Notification
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration for MessageQueueConfiguration
            services.Configure<MessageQueueConfiguration>(Configuration.GetSection("MessageQueue"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MessageQueueConfiguration>>().Value);

            // Configuration for EmailClientConfiguration
            services.Configure<EmailClientConfiguration>(Configuration.GetSection("Email"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailClientConfiguration>>().Value);

            // Register IEmailClient and its implementation
            services.AddTransient<IEmailClient, EmailClient>();

            // Register INotificationConsumer and its implementation
            services.AddTransient<INotificationConsumer, NotificationConsumer>();

            // Register the NotificationHandler as a hosted service
            services.AddHostedService<NotificationHandler>();

            // Add Azure Service Bus clients
            var queueConfiguration = Configuration.GetSection("MessageQueue").Get<MessageQueueConfiguration>();
            services.AddAzureClients(builder =>
            {
                builder.AddServiceBusClientWithNamespace(queueConfiguration.QueueNamespace);

                builder.AddClient<ServiceBusReceiver, ServiceBusReceiverOptions>((options, credential, serviceProvider) =>
                {
                    var serviceClient = serviceProvider.GetRequiredService<ServiceBusClient>();
                    var configuration = serviceProvider.GetRequiredService<MessageQueueConfiguration>();
                    options.ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete;

                    return serviceClient.CreateReceiver(configuration.QueueName, options);
                })
                .WithName(queueConfiguration.QueueName);

                builder.UseCredential(new DefaultAzureCredential());
            });

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { }
    }
}
