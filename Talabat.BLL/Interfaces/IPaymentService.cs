using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;

namespace Talabat.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFalid(string paymentIntentId);
    }
}
