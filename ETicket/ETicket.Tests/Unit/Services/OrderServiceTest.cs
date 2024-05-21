using ETicket.Bll.Services;
using ETicket.Bll.Services.Cart;
using ETicket.Db.Dal;
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using ETicket.Db.Domain.Enums;
using ETicket.Tests.Abstractions;
using ETicket.Tests.Unit.DataFixtures;
using ETicket.Tests.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ETicket.Tests.Unit.Services
{
    [TestFixture]
    public class OrderServiceTest : AbstractTest
    {
        private ETicketDbDataFixture _eventsSeedDataFixture;
        private IUnitOfWork _unitOfWork;
        private OrderService _orderService;
        private ETicketDbContext _context;
        CartStorage _cartStorage;

        [SetUp]
        public void Setup()
        {
            _eventsSeedDataFixture = new ETicketDbDataFixture();
            _context = _eventsSeedDataFixture
                .BuildInMemoryDatabase("OrderServiceTests")
                .GetDataContext;
            _cartStorage = new CartStorage();
            _unitOfWork = new ETicketUnitOfWork(_context);
            _orderService = new OrderService(_cartStorage, _unitOfWork);
        }

        [Test]
        public void GetCartItems_ShouldReturCartItems()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartItem = TestDataFactory.GetStubBusinessOrderItem1();
            var cart = _cartStorage.GetOrCreate(cartId);
            cart.TryAddItem(new CartItem(cartItem.EventId, cartItem.SeatId, cartItem.PriceId, cartItem.UserId));

            // Act
            var result = _orderService.GetCartItems(cartId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().EventId, Is.EqualTo(cartItem.EventId));
                Assert.That(result.First().SeatId, Is.EqualTo(cartItem.SeatId));
                Assert.That(result.First().PriceId, Is.EqualTo(cartItem.PriceId));
                Assert.That(result.First().UserId, Is.EqualTo(cartItem.UserId));
            });
        }

        [Test]
        public void AddItemToCart_ShouldCreateCartAndAddItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var orderItem = TestDataFactory.GetStubBusinessOrderItem1();

            // Act
            var result = _orderService.AddItemToCart(cartId, orderItem);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(cartId));
            Assert.That(result.TotalAmount, Is.EqualTo(1));
        }

        [Test]
        public void AddItemToExistingCart_ShouldGetExistingCartAndAddNewItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var orderItem1 = TestDataFactory.GetStubBusinessOrderItem1();
            var orderItem2 = TestDataFactory.GetStubBusinessOrderItem2();

            // Act
            _orderService.AddItemToCart(cartId, orderItem1);
            var result = _orderService.AddItemToCart(cartId, orderItem2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(cartId));
            Assert.That(result.TotalAmount, Is.EqualTo(2));
        }

        [Test]
        public void AddExistingItemToCart_ShouldThrowArgumentException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var orderItem1 = TestDataFactory.GetStubBusinessOrderItem1();
            var orderItem2 = TestDataFactory.GetStubBusinessOrderItem1();

            // Act
            _orderService.AddItemToCart(cartId, orderItem1);

            //Assert
            Assert.Throws<ArgumentException>(() => _orderService.AddItemToCart(cartId, orderItem2));
        }

        [Test]
        public void DeleteItemFromCart_ShouldDeleteCartItem()
        {
            // Arrange
            var orderItem = TestDataFactory.GetStubBusinessOrderItem1();
            var cartId = Guid.NewGuid();
            var cart = _cartStorage.GetOrCreate(cartId);
            cart.TryAddItem(new CartItem(orderItem.EventId, orderItem.SeatId, orderItem.PriceId, orderItem.UserId));

            // Act
            _orderService.DeleteItemFromCart(cartId, orderItem.EventId, orderItem.SeatId);

            // Assert
            Assert.Throws<ArgumentException>(() => _orderService.GetCartItems(cartId));
        }

        [Test]
        public async Task BookCartSeats_ShouldUpdateSeatsStatus()
        {
            //Arrange
            var cartId = Guid.NewGuid();
            var orderItem1 = TestDataFactory.GetStubBusinessOrderItem1();
            var orderItem2 = TestDataFactory.GetStubBusinessOrderItem2();
            var cancellationToken = CancellationToken.None;

            // Act
            _orderService.AddItemToCart(cartId, orderItem1);
            _orderService.AddItemToCart(cartId, orderItem2);

            var seats = new List<Seat>() { TestDataFactory.GetStubRow1Seat1(), TestDataFactory.GetStubRow1Seat2() };
            _eventsSeedDataFixture.AddSeats(seats);

            var result = await _orderService.BookCartSeatsAsync(cartId, cancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null); //paymentId
                //all seats have 'Booked' status
                Assert.That(_context.Seats.First(seat => seat.Id == seats.First().Id).SeatStatusId, Is.EqualTo(SeatStatusOption.Booked));
                Assert.That(_context.Seats.First(seat => seat.Id == seats.Last().Id).SeatStatusId, Is.EqualTo(SeatStatusOption.Booked));
                Assert.That(_context.Payments.First(payment => payment.Id == result).PaymentStatusId, Is.EqualTo(PaymentStatusOption.New));//payment has 'New' status
            });
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _eventsSeedDataFixture.Dispose();
        }
    }
}
