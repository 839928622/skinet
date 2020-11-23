using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace Infrastructure.Services
{
  public  class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<DeliveryMethod> _dmRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IBasketRepository _basketRepo;

        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<DeliveryMethod> dmRepo,
                            IGenericRepository<Product> productRepo, IBasketRepository basketRepo)
        {
            _orderRepo = orderRepo;
            _dmRepo = dmRepo;
            _productRepo = productRepo;
            _basketRepo = basketRepo;
        }
        /// <inheritdoc />
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);
            // get items from the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _productRepo.GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id,productItem.Name,productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            // ret delivery method from repo
            var deliveryMethod = await  _dmRepo.GetByIdAsync(deliveryMethodId);
            // calculate subtotal
            var subTotal = items.Sum(item => item.Price);
            // create order
            var order = new Order(items , buyerEmail, shippingAddress, deliveryMethod, subTotal);
            // save t db

            // return order
            return order;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Order>> GetOrdersFroUserAsync(string buyerEmail)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Order> GetOrderByOrderIdAsync(int id, string buyerEmail)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
