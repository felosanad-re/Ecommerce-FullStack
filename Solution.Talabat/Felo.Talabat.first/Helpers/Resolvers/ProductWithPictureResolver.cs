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
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return $"{_configuration["BasePictureUrl"]}/{source.PictureUrl}";
            return string.Empty;
        }
    }
}
