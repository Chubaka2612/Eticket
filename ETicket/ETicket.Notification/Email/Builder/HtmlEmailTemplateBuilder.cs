
using ETicket.Messaging.Models;
using System.Text;
using ETicket.Utils;

namespace ETicket.Notification.Email.Builder
{
    public class HtmlEmailTemplateBuilder
    {
        public static string GenerateCheckedOutTicketEmailBody(List<Ticket> ticketsInfoList, string orderNo)
        {
            var template = new StringBuilder(GetEmailTemplate("CheckedOutTicketEmailTemplate"));
            var ticketsInfoSection = GenerateTicketsInfoSection(ticketsInfoList);
            Parameter.SubstituteParameter(template, Parameter.OrderNumber, orderNo.ToString());
            Parameter.SubstituteParameter(template, Parameter.TicketsSection, ticketsInfoSection);
            Parameter.SubstituteParameter(template, Parameter.Current_Year, DateTime.UtcNow.ToString("yyyy"));
            return template.ToString();
        }

        private static string GetEmailTemplate(string templateName)
        {
            var assembly = typeof(HtmlEmailTemplateBuilder).Assembly;
            return assembly.GetManifestedResourceContent($"{assembly.GetName().Name}.Templates.{templateName}.html");
        }

        private static string GenerateTicketsInfoSection(List<Ticket> ticketsInfoList)
        {
            if (ticketsInfoList.Count == 0)
            {
                return string.Empty;
            }
            var ticketsSection = new StringBuilder();
            foreach (var ticket in ticketsInfoList)
            {
                var ticketSection = @$"<div class='ticket-info'>
						<table>
							<tr>
								<th>Event Title</th>
								<td>{ticket.EventTitle}</td>
							</tr>
							<tr>
								<th>Event Date and Time</th>
								<td>{ticket.EventDateAndTime}</td>
							</tr>
							<tr>
								<th>Venue</th>
								<td>{ticket.Venue}</td>
							</tr>
							<tr>
								<th>Row Number</th>
								<td>{ticket.RowNumber}</td>
							</tr>
							<tr>
								<th>Seat Number</th>
								<td>{ticket.SeatNumber}</td>
							</tr>
							<tr>
								<th>Ticket Price</th>
								<td>{ticket.TicketPrice.ToString("#.##")}</td>
							</tr>
						</table>
					</div>";
                ticketsSection.Append(ticketSection);
            }
            return ticketsSection.ToString();
        }

        private sealed class Parameter
        {
            private Parameter(string value)
            {
                this.Value = value;
            }

            public static Parameter OrderNumber => new Parameter("order_number");

            public static Parameter TicketsSection => new Parameter("tickets_section");

            public static Parameter Current_Year => new Parameter("current_year");

            private string Value { get; }

            public static void SubstituteParameter(StringBuilder source, Parameter parameter, string value)
            {
                source.Replace("@{param:" + parameter.Value + "}", value);
            }
        }
    }
}
