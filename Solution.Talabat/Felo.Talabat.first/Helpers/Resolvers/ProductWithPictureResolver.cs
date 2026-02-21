using AutoMapper;
using Felo.Talabat.Api.ModelDto.Products;
using Talabat.Core.Entites.Products;

namespace Felo.Talabat.Api.Helpers.Resolvers
{
    public class ProductWithPictureResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductWithPictureResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return string.Empty;

            var baseUrl = _configuration["BasePictureUrl"] ?? string.Empty;

            // If Image Have Full Pash
            if (source.PictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                source.PictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return source.PictureUrl;
            }

            // غير كده → نسبي → ضيف الـ base
            return $"{baseUrl.TrimEnd('/')}/{source.PictureUrl.TrimStart('/')}";
        }
    }
}
