using AutoMapper;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Talabat.Core.Entites.Orders;

namespace Felo.Talabat.Api.Helpers.Resolvers
{
    public class OrderWithPictureResolver : IValueResolver<OrderItems, OrderItemsDto, string>
    {
        private readonly IConfiguration _config;

        public OrderWithPictureResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(OrderItems source, OrderItemsDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Product.PictureUrl))
                return string.Empty;

            if (source.Product.PictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                source.Product.PictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(source.Product.PictureUrl, UriKind.Absolute, out var absoluteUri) &&
                    (absoluteUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                     absoluteUri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)))
                {
                    var baseUrlFromConfig = _config["BasePictureUrl"] ?? string.Empty;
                    return $"{baseUrlFromConfig.TrimEnd('/')}/{absoluteUri.AbsolutePath.TrimStart('/')}";
                }

                return source.Product.PictureUrl;
            }

            var baseUrl = _config["BasePictureUrl"] ?? string.Empty;
            return $"{baseUrl.TrimEnd('/')}/{source.Product.PictureUrl.TrimStart('/')}";
        }
    }

}
