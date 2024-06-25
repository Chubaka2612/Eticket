
using ETicket.Messaging.Models;
using ETicket.Notification.Configuration;
using ETicket.Notification.Email.Builder;
using ETicket.Notification.Email.Client;

namespace ETicket.Notification.Consumer
{
    public class NotificationConsumer : INotificationConsumer
    {
        private readonly ILogger<NotificationConsumer> _logger;
        private readonly IEmailClient _emailClient;
        private readonly EmailClientConfiguration _clientConfiguration;
  
        public NotificationConsumer(ILogger<NotificationConsumer> logger, IEmailClient emailClient, EmailClientConfiguration clientConfiguration)
        {
            _logger = logger;
            _clientConfiguration = clientConfiguration;
            _emailClient = emailClient;
        }

        public async Task ConsumeMessagesAsync(Messaging.Models.Notification notification, CancellationToken cancellationToken)
        {
            notification.Parameters.TryGetValue("CustomerEmail", out var customerEmail);
            notification.Parameters.TryGetValue("CustomerName", out var customerName);

            if (customerEmail is null || customerName is null) 
            {
                throw new ArgumentException("Reciever Name and Email are not defined for Notification");
            }
            var tickets = (notification.Content as List<Ticket>);

            if (tickets is null) 
            {
                throw new ArgumentException("Content is not defined for Notification");
            }
            var messageBody = HtmlEmailTemplateBuilder.GenerateCheckedOutTicketEmailBody(tickets, notification.Id.ToString());
            var message = new HtmlEmailMessageBuilder()
                .AddSender(_clientConfiguration.SenderEmail, _clientConfiguration.SenderEmail)
                .AddReceiver(customerEmail, customerName)
                .SetSubject($"Your tickets, No. {notification.Id} succesfully paid")
                .SetBody(messageBody)
                .Create();

            await _emailClient.SendEmailAsync(message, cancellationToken);
        }
    }
}
