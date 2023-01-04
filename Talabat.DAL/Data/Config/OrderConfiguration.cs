using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.DAL.Entities.Order;

namespace Talabat.DAL.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(O => O.ShipToAddress, NP => {
                NP.WithOwner();
            });
            builder.Property(O => O.Status)
                .HasConversion(
                    orderStatus => orderStatus.ToString(),
                    orderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus)
                );
            builder.HasMany(O => O.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
