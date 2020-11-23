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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo)
        {
            _unitOfWork = unitOfWork;
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
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id,productItem.Name,productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            // ret delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            // calculate subtotal
            var subTotal = items.Sum(item => item.Price);
            // create order
            var order = new Order(items , buyerEmail, shippingAddress, deliveryMethod, subTotal);
            // save t db
             _unitOfWork.Repository<Order>().Add(order); // nothing saved to database at this point
              await _unitOfWork.Complete(); // if error , it will throw an error 
            // delete basket
            await _basketRepo.DeleteBasketAsync(basketId);
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
