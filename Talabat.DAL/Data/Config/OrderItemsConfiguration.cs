using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Order;

namespace Talabat.DAL.Data.Config
{
    public class OrderItemsConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(O => O.ItemOrdered, NP => {
                NP.WithOwner();
            });
            builder.Property(I => I.Price).HasColumnType("decimal(18,2)");
        }
    }
}
