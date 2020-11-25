using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _autoMapper;

        public PaymentController(IPaymentService paymentService, IMapper autoMapper)
        {
            _paymentService = paymentService;
            _autoMapper = autoMapper;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            return Ok(await _paymentService.CreateOrUpdatePaymentIntent(basketId));
        }
    }
}
