using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.API.Errors;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;

namespace Talabat.API.Controllers
{
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string _wbSecret = "whsec_412e4a57256ec29341d26a225343fce45c03b0c482d167b524c2251256a37be9";

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            
            if(basket == null)
                return BadRequest(new ApiResponse(400, "error with your basket !!"));

            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _wbSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeded");
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Faild", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFalid(intent.Id);
                    _logger.LogInformation("Payment Faild", order.Id);
                    break;
            }

            return new EmptyResult(); 
        }
    }
}
