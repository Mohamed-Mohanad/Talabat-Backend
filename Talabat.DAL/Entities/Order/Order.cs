namespace Talabat.DAL.Entities.Order
{
    public class Order : BaseEntity
    {
        public Order()
        {
        }

        public Order(string? buyerEmail, ShippingAddress shipToAddress, DeliveryMethod deliveryMethod, 
                    IReadOnlyList<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShipToAddress = shipToAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string? BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public ShippingAddress ShipToAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public IReadOnlyList<OrderItem> Items { get; set; }
        public decimal SubTotal { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal GetTotal() 
            => SubTotal + DeliveryMethod.Cost;
    }
}
