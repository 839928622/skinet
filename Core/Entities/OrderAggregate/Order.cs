using System;
using System.Collections.Generic;

namespace Core.Entities.OrderAggregate
{
  public  class Order : BaseEntity
    {
        public Order()
        {
            
        }
        public Order(
            IReadOnlyList<OrderItem> orderItems,
            string buyerEmail, 
            Address shipAddress,
            DeliveryMethod deliveryMethod, 
            decimal subtotal
            )
        {
            OrderItems = orderItems;
            BuyerEmail = buyerEmail;
            ShipAddress = shipAddress;
            DeliveryMethod = deliveryMethod;
            Subtotal = subtotal;
           
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public Address ShipAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
        public decimal Subtotal { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }

        public decimal GetTotal()
        {
            return Subtotal + DeliveryMethod.Price;
        }
    }
}
