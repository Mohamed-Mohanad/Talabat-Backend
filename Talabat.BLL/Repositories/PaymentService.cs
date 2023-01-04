using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.BLL.Interfaces;
using Talabat.BLL.OrderSpecifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;
using Product = Talabat.DAL.Entities.Product;

namespace Talabat.BLL.Repositories
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IBasketRepository basketRepository
            ,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _configuration = configuration;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);

            if (basket == null) return null;
            
            var shippingPrice = 0m;
            
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
            }

            foreach(var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                
                if (item.Price != productItem.Price)
                    item.Price = productItem.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions 
                { 
                    Amount = (long)basket.Items.Sum(x => x.Quantity * (x.Price * 100)) + ((long)shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>{ "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(x => x.Quantity * (x.Price * 100)) + ((long)shippingPrice * 100),
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            basket.ShippingPrice= shippingPrice;
            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFalid(string paymentIntentId)
        {
            var spec = new OrderWithItemsAndDeleviryMethodSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order is null) return null;

            order.Status = OrderStatus.PaymentFaild;
            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderWithItemsAndDeleviryMethodSpecification(paymentIntentId);
            
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order is null) return null;

            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}
