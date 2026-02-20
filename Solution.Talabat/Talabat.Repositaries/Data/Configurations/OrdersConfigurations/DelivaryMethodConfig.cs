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
    public class DelivaryMethodConfig : IEntityTypeConfiguration<DelivaryMethod>
    {
        public void Configure(EntityTypeBuilder<DelivaryMethod> builder)
        {
            builder.Property(D => D.Cost).HasColumnType("decimal(18, 2)");
        }
    }
}
