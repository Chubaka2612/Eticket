
using System.Net.Mail;

namespace ETicket.Notification.Email.Builder
{
    public class HtmlEmailMessageBuilder
    {
        private MailMessage _message;
        private string _body;

        public HtmlEmailMessageBuilder()
        {
            _message = new MailMessage();
        }

        public HtmlEmailMessageBuilder AddSender(string mail, string name)
        {
            _message.From = new MailAddress(mail, name);
            _message.Sender = new MailAddress(mail, name);

            return this;
        }

        public HtmlEmailMessageBuilder AddReceiver(string mail, string name)
        {
            _message.To.Add(new MailAddress(mail, name));           
            return this;
        }

        public HtmlEmailMessageBuilder SetSubject(string subject)
        {
            _message.Subject = subject;
            return this;
        }

        public HtmlEmailMessageBuilder SetBody(string body)
        {
            _body = body;
            return this;
        }

        public MailMessage Create()
        {
            _message.IsBodyHtml = true;
            _message.Body = _body;
            return _message;
        }
    }
}
