using System.Collections.Generic;
using System.Threading.Tasks;
using API.UnitTests.Helper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace API.UnitTests.ServiceTests
{
   public class OrderServiceTests : TestBase
    {
        private readonly StoreContext _storeContext;
        private readonly Mock<IBasketRepository> _basketRepoMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        public OrderServiceTests()
        {
            _basketRepoMock    = new Mock<IBasketRepository>();
            _paymentServiceMock =new Mock<IPaymentService>();
            _storeContext = GetStoreContext();
        }

        [Fact]
        public async Task Create_Order_Should_Return_New_Order()
        {
            // Arrange
            var buyerEmail = "a@b.com";
            var productItem = new Product()
            {
                Name = "product", Description = "", Price = 1, PictureUrl = "url"
            };

            await _storeContext.Products.AddAsync(productItem);
            var deliveryMethod = new DeliveryMethod()
            {
                ShortName = "express",
                DeliveryTime = "1",
                Description = "fast",
                Price = 4
            };
            await _storeContext.DeliveryMethods.AddAsync(deliveryMethod);

            await _storeContext.SaveChangesAsync();
            var fakeCustomerBasket = new CustomerBasket
            {
                Id = "12",
                Items = new List<BasketItem>()
                {
                    new BasketItem()
                        {Id = 1, Brand = "vs", PictureUrl = "", Price = 1, ProductName = "c#", Quantity = 1}
                }
            };
            _basketRepoMock.Setup(b => b.GetBasketAsync(It.IsAny<string>())).ReturnsAsync(fakeCustomerBasket);
            var orderService = new OrderService(_basketRepoMock.Object, _paymentServiceMock.Object, _storeContext);

            // Act
            var result =
                await orderService.CreateOrderAsync(buyerEmail, deliveryMethod.Id, It.IsAny<string>(), new Address());
            // Assert
            Assert.Equal(buyerEmail, result.BuyerEmail);
        }
    }
}
