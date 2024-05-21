

namespace ETicket.Tests.Abstractions
{
    public class AbstractApiTest: AbstractTest
    {
        protected const string BASE_ROUTE = "api/eticket";
        protected const string BASE_EVENTS_ROUTE = BASE_ROUTE + "/events";
        protected const string BASE_ORDERS_ROUTE = BASE_ROUTE + "/orders/carts";
        protected const string BASE_PAYMENTS_ROUTE = BASE_ROUTE + "/payments";
    }
}
