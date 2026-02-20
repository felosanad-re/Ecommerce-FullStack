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
            if(!string.IsNullOrEmpty(source.Product.PictureUrl))
                return $"{_config["BasePictureUrl"]}/{source.Product.PictureUrl}";
            return string.Empty;
        }
    }

}
