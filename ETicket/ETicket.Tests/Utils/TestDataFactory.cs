using ETicket.Bll.Models;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ETicket.Tests.Utils
{
    public class TestDataFactory
    {

        public static Event GetStubEvent1(List<Venue> venues = default)
        {
            return Builder<Event>
                  .Build()
                  .With(model => model.Name = "Test Event 1")
                  .With(model => model.Id = 1L)
                  .With(model => model.Date = DateTime.Now.AddDays(1))
                  .With(model => model.CreatedAt = DateTime.Now)
                  .With(model => model.Venues = venues)
                  .GetInstance();
        }

        public static Event GetStubEvent2(List<Venue> venues = default)
        {
            return Builder<Event>
                  .Build()
                  .With(model => model.Name = "Test Event 2")
                  .With(model => model.Id = 2L)
                  .With(model => model.Date = DateTime.Now.AddDays(2))
                  .With(model => model.CreatedAt = DateTime.Now)
                  .With(model => model.Venues = venues)
                  .GetInstance();
        }


        public static BusinessSeat GetStubSeat1()
        {
            return Builder<BusinessSeat>
                  .Build()
                  .With(model => model.SeatStatus = SeatStatusOption.Available)
                  .With(model => model.Id = 1L)
                  .With(model => model.Number = 1)
                  .With(model => model.RowId = 1)
                  .GetInstance();
        }

        public static BusinessSeat GetStubSeat2()
        {
            return Builder<BusinessSeat>
                  .Build()
                  .With(model => model.SeatStatus = SeatStatusOption.Available)
                  .With(model => model.Id = 2L)
                  .With(model => model.Number = 2)
                  .With(model => model.RowId = 2)
                  .GetInstance();
        }

        public static Venue GetStubVenue1()
        {
            return Builder<Venue>
                  .Build()
                  .With(model => model.Id = 1L)
                  .With(model => model.City = "Warsaw")
                  .With(model => model.Address = "Novi Sviat")
                  .With(model => model.Country = "Poland")
                  .With(model => model.Name = "Arena")
                  .GetInstance();
        }

        public static Section GetStubSection1()
        {
            return Builder<Section>
                  .Build()
                  .With(model => model.Id = 1L)
                  .With(model => model.ManifestId = 1L)
                  .With(model => model.Name = "VIP Lounge")
                  .GetInstance();
        }

        public static Row GetStubRow1()
        {
            return Builder<Row>
                  .Build()
                  .With(model => model.Id = 1L)
                  .With(model => model.SectionId = 1L)
                  .With(model => model.Number = 1)
                  .GetInstance();
        }

        public static Seat GetStubRow1Seat1()
        {
            return Builder<Seat>
                  .Build()
                  .With(model => model.Id = 1L)
                  .With(model => model.RowId = 1L)
                  .With(model => model.Number = 1)
                  .GetInstance();
        }

        public static Seat GetStubRow1Seat2()
        {
            return Builder<Seat>
                  .Build()
                  .With(model => model.Id = 2L)
                  .With(model => model.RowId = 1L)
                  .With(model => model.Number = 2)
                  .GetInstance();
        }

        public static Manifest GetStubManifest1()
        {
            return Builder<Manifest>
                  .Build()
                  .With(model => model.Id = 1L)
                  .With(model => model.VenueId = 1L)
                  .GetInstance();
        }

        public static List<BusinessSeat> GetStubSeats()
        {
            return new List<BusinessSeat>
            {
               GetStubSeat1(),
               GetStubSeat2()
            };
        }

        public static List<Event> GetStubEvents()
        {
            return new List<Event>
           {
               GetStubEvent1(),
               GetStubEvent2()
           };

        }

        public static BusinessOrderItem GetStubBusinessOrderItem1()
        {
            return Builder<BusinessOrderItem>
                .Build()
                .With(model => model.PriceId = 1L)
                .With(model => model.SeatId = 1L)
                .With(model => model.UserId = 1L)
                .With(model => model.EventId = 1L)
                .GetInstance();
        }

        public static BusinessOrderItem GetStubBusinessOrderItem2()
        {
            return Builder<BusinessOrderItem>
                .Build()
                .With(model => model.PriceId = 1L)
                .With(model => model.SeatId = 2L)
                .With(model => model.UserId = 1L)
                .With(model => model.EventId = 1L)
                .GetInstance();
        }
    }
}
