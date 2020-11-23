using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
namespace Core.Interfaces
{
  public  interface IOrderService
  {
      Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);

      Task<IReadOnlyList<Order>> GetOrdersFroUserAsync(string buyerEmail);

      Task<Order> GetOrderByOrderIdAsync(int id,string buyerEmail);

      Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
  }
}
