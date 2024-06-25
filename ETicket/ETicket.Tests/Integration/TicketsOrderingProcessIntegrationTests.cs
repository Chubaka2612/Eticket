using ETicket.Bll.Models;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using ETicket.Tests.Abstractions;
using ETicket.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ETicket.Tests.Integration
{
    public class TicketsOrderingProcessIntegrationTests : AbstractApiTest
    {
        InMemoryWebFactory webFactory;
        HttpClient httpClient;

        [Test]
        [TestCaseSource(nameof(PaymentOptionSource))]
        public async Task PlaceOrder_PaymentShouldBe(KeyValuePair<PaymentStatusOption, SeatStatusOption> paymentSet)
        {
            var expectedPaymentStatus = paymentSet.Key;
            var expectedSeatStatus = paymentSet.Value;

            webFactory = new InMemoryWebFactory($"TicketsOrdering{expectedPaymentStatus}ProcessIntegrationTests");
            httpClient = webFactory.CreateNoRedirectClient();

          
            var testEvent = TestDataFactory.GetStubEvent1();
            var testVenue = TestDataFactory.GetStubVenue1();
            var testSection = TestDataFactory.GetStubSection1();
            var testSeat1 = TestDataFactory.GetStubRow1Seat1();
            var testSeat2 = TestDataFactory.GetStubRow1Seat2();
            
            // Arrange
            using (var scope = webFactory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                db.Repository<Venue>().Add(testVenue);
                db.Repository<Manifest>().Add(TestDataFactory.GetStubManifest1());
                db.Repository<Event>().Add(TestDataFactory.GetStubEvent1(new List<Venue>() { testVenue }));
                db.Repository<Section>().Add(testSection);
                db.Repository<Row>().Add(TestDataFactory.GetStubRow1());
                
                db.Repository<Seat>().Add(testSeat1);
                db.Repository<Seat>().Add(testSeat2);

                await db.SaveChangesAsync();
            }

            // Act: create cart
            var cartId = Guid.NewGuid();
            var orderItem1 = TestDataFactory.GetStubBusinessOrderItem1();
            var orderItem2 = TestDataFactory.GetStubBusinessOrderItem2();

            // Act: place order items to cart
            await httpClient.PostAsync($"{BASE_ORDERS_ROUTE}/{cartId}", CommonUtil.SerializeToHttpContent(orderItem1));
            await httpClient.PostAsync($"{BASE_ORDERS_ROUTE}/{cartId}", CommonUtil.SerializeToHttpContent(orderItem2));

            //Act: book order items
            var response = await httpClient.PutAsync($"{BASE_ORDERS_ROUTE}/{cartId}/book", content:null);
            var paymentId = long.Parse(await response.Content.ReadAsStringAsync());
            
            //Act: complete payment
            var url = expectedPaymentStatus == PaymentStatusOption.Completed ? $"{BASE_PAYMENTS_ROUTE}/{paymentId}/complete" : $"{BASE_PAYMENTS_ROUTE}/{paymentId}/failed";
            await httpClient.PostAsync(url, content:null);

            //Assert: payment state
             response = await httpClient.GetAsync($"{BASE_PAYMENTS_ROUTE}/{paymentId}");
            var json = await response.Content.ReadAsStringAsync();
            var paymentState = CommonUtil.PopulateFromJson<PaymentState>(json);
            Assert.That(paymentState.Status, Is.EqualTo(expectedPaymentStatus)); //verify payment has appropriate status

            //Assert: seats status
            response = await httpClient.GetAsync($"{BASE_EVENTS_ROUTE}/{testEvent.Id}/venues/{testVenue.Id}/sections/{testSection.Id}/seats");

            json = await response.Content.ReadAsStringAsync();
            var seats = CommonUtil.PopulateFromJson<List<BusinessSeat>>(json);
            var seatIds = new List<Seat>() { testSeat1, testSeat2}.Select(item => item.Id).ToList();
            var bookedPreviouslySeats =  seats
                    .Where(seat => seatIds.Contains(seat.Id));

            Assert.That(bookedPreviouslySeats.Select(s => s.SeatStatus), Is.All.EqualTo(expectedSeatStatus));
        }

        protected static IEnumerable<TestCaseData> PaymentOptionSource()
        {
            var sourceList = new Dictionary<PaymentStatusOption, SeatStatusOption>
            {
               { PaymentStatusOption.Completed, SeatStatusOption.Sold},
               { PaymentStatusOption.Failed, SeatStatusOption.Available},
            };

            for (int i = 0; i < sourceList.Count; i++)
            {
                TestCaseData tcd = new(sourceList.ElementAt(i));
                tcd.SetName("{m}[" + sourceList.Keys.ToList()[i] + "]");
               
                yield return tcd;
            }
        }
    }
}
