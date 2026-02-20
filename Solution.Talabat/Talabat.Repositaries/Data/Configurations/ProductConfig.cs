using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entites.Products;

namespace Talabat.Repositaries.Data.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.Id).UseIdentityColumn(1, 1);
            builder.Property(P => P.Price).HasColumnType("decimal(18, 2)");

            builder.HasOne(P => P.Brand)
                .WithMany()
                .HasForeignKey(P => P.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(P => P.Category)
                .WithMany()
                .HasForeignKey(P => P.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(P => P.StockTransactions)
                .WithOne(T => T.Product)
                .HasForeignKey(T => T.ProductId);
        }
    }
}
