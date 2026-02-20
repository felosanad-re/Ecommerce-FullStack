using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.StockTransactions;

namespace Talabat.Repositaries.Data.Configurations.StockTransactions
{
    public class StockTransactionConfig : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.Property(S => S.DateTime).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(S => S.Type)
                .HasConversion<string>();
        }
    }
}
