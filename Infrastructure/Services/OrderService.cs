using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
  public  class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IPaymentService _paymentService;
        private readonly StoreContext _storeContext;

        public OrderService( IBasketRepository basketRepo,
                            IPaymentService paymentService, StoreContext storeContext)
        {
            _basketRepo = basketRepo;
            _paymentService = paymentService;
            _storeContext = storeContext;
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
                // _unitOfWork.Repository<Product>().GetByIdAsync(item.Id)
                var productItem = await _storeContext.Products.FindAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id,productItem.Name,productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            // ret delivery method from repo
            //_unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId)
            var deliveryMethod = await _storeContext.DeliveryMethods.FindAsync(deliveryMethodId);
            // calculate subtotal
            var subTotal = items.Sum(item => item.Price);
            // check to see if order exists (same paymentIntentId)
            var spec = new OrderByPaymentIntentIdWithItemsSpecification(basket.PaymentIntentId);
            // _unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
            var existingOrder = await _storeContext.Orders.FirstOrDefaultAsync(x => x.PaymentIntentId == basket.PaymentIntentId) ;
          if (existingOrder != null)
          {
              //_unitOfWork.Repository<Order>().Delete(existingOrder);
              _storeContext.Remove(existingOrder);
              await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
          }
            // create order
            var order = new Order(items , buyerEmail, shippingAddress, deliveryMethod, subTotal,basket.PaymentIntentId);
            // save t db
             /*_unitOfWork.Repository<Order>().Add(order)*/; // nothing saved to database at this point
            await _storeContext.Orders.AddAsync(order);
             /* await _unitOfWork.Complete()*/; // if error , it will throw an error 
             await _storeContext.SaveChangesAsync();
            // return order
            return order;
        }

        
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            // _unitOfWork.Repository<Order>().ListAsync(spec)
            return await  _storeContext.Orders.Where(x => x.BuyerEmail == buyerEmail).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Order> GetOrderByOrderIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            //_unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
            return await _storeContext.Orders.FirstOrDefaultAsync(x => x.Id == id && x.BuyerEmail == buyerEmail);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            //return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
            return await _storeContext.DeliveryMethods.ToListAsync();
        }
    }
}
