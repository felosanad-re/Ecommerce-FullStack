using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entites.Orders;


namespace Talabat.Repositaries.Data.Configurations.OrdersConfigurations
{
    public class OrderItemsConfig : IEntityTypeConfiguration<OrderItems>
    {
        public void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            builder.Property(O => O.Price).HasColumnType("decimal(18, 2)");
            builder.OwnsOne(O => O.Product, Items => Items.WithOwner());
        }
    }
}
