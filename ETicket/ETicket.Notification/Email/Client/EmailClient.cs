using ETicket.Notification.Configuration;
using System.Net;
using System.Net.Mail;
using Polly;
using mailslurp.Api;

namespace ETicket.Notification.Email.Client
{
    public class EmailClient : IEmailClient
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailClientConfiguration _emailClientConfiguration;
        private readonly ILogger<EmailClient> _logger;

        public EmailClient(EmailClientConfiguration emailClientConfiguration, ILogger<EmailClient> logger)
        {
            _emailClientConfiguration = emailClientConfiguration;
            _logger = logger;

            var config = new mailslurp.Client.Configuration();
            config.ApiKey.Add("x-api-key", emailClientConfiguration.ApiKey);
            var inboxController = new InboxControllerApi(config);
            var accessDetails = inboxController.GetImapSmtpAccess(Guid.Parse(emailClientConfiguration.InboxId));
            _smtpClient = new SmtpClient(accessDetails.SmtpServerHost)
            {
                Port = accessDetails.SmtpServerPort,
                Credentials = new NetworkCredential(userName: accessDetails.SmtpUsername, password: accessDetails.SmtpPassword),
                EnableSsl = false
            };
        }

        public Task SendEmailAsync(MailMessage message, CancellationToken cancellationToken)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 4, retryAttempt =>
                {
                    _logger.LogWarning("An error occured during the email sending. Trying to reconnect.");
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                });

            return retryPolicy.ExecuteAsync(() => _smtpClient.SendMailAsync(message, cancellationToken));
        }
    }
}
