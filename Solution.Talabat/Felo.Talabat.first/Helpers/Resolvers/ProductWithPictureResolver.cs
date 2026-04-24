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

            if (source.PictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                source.PictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(source.PictureUrl, UriKind.Absolute, out var absoluteUri) &&
                    (absoluteUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                     absoluteUri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)))
                {
                    return $"{baseUrl.TrimEnd('/')}/{absoluteUri.AbsolutePath.TrimStart('/')}";
                }

                return source.PictureUrl;
            }

            return $"{baseUrl.TrimEnd('/')}/{source.PictureUrl.TrimStart('/')}";
        }
    }
}
