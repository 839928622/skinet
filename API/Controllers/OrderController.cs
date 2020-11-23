using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;

namespace API.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _autoMapper;

        public OrdersController(IOrderService orderService,IMapper autoMapper)
        {
            _orderService = orderService;
            _autoMapper = autoMapper;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var shippingAddress = _autoMapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId,
                shippingAddress);

            if (order == null) return BadRequest(new APiResponse(400, "Problem occurred during  creating order"));

            return Ok(order);
        }
    }
}
