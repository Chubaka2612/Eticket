namespace ETicket.Notification.Configuration;

public class EmailClientConfiguration
{
    public string ApiKey { get; set; }

    public string InboxId { get; set; }

    public string SenderEmail { get; set; }

    public string RecieverEmail { get; set; }
}
