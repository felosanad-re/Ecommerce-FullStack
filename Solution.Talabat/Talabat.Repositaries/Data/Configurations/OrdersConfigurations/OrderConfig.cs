using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Repositaries.Data.Configurations.OrdersConfigurations
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(O => O.AddressShiper, Address => Address.WithOwner());

            builder.Property(O => O.SubTotal).HasColumnType("decimal(18, 2)");
            
            builder.HasOne(O => O.DelivaryMethod)
                .WithMany()
                .HasForeignKey(O => O.DelivaryMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(O => O.OrderStatus)
                .HasConversion(
                    OS => OS.ToString(),
                    OS => Enum.Parse<OrderStatus>(OS)
                );
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

