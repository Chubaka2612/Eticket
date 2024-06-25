using System.Net.Mail;

namespace ETicket.Notification.Email.Client
{
    public interface IEmailClient
    {
        Task SendEmailAsync(MailMessage message, CancellationToken cancellationToken);
    }
}
