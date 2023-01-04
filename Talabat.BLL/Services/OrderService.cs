using Talabat.BLL.Interfaces;
using Talabat.BLL.OrderSpecifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;

namespace Talabat.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, ShippingAddress shippingAddress)
        {
            //1. Get Basket from Basket Repo
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            //2. Get Selected Items at Basket from product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrder = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                var orderItem = new OrderItem(productItemOrder, product.Price, item.Quantity);
                items.Add(orderItem);
            }
            //3. Get Delivery Method From DeliveryMethodId Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            //4. Calculate Subtotal
            var subTotal = items.Sum(i => i.Price * i.Quantity);

            //check to see if order exists => TODO 
            var spec = new OrderWithItemsAndDeleviryMethodSpecification(basket.PaymentIntentId);
            var existOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if(existOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }
            
            //5. Create Order
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, items, subTotal, basket.PaymentIntentId);

            _unitOfWork.Repository<Order>().Create(order);

            //6. savr to Datbase
            var result = await _unitOfWork.Complete();

            if(result <= 0) return null;

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        public async Task<Order> GetOrderByIdAsync(int orderId, string buyerEmail)
        {
            var specification = new OrderWithItemsAndDeleviryMethodSpecification(orderId, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(specification);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var specification = new OrderWithItemsAndDeleviryMethodSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().GetAllWithSpec(specification);
        }

    }
}
