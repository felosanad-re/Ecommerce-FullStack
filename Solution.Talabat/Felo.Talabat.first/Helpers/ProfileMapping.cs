using AutoMapper;
using Felo.Talabat.Api.Helpers.Resolvers;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Felo.Talabat.Api.ModelDto.Products;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.RequestModels;
using Talabat.Core.RequestModels.BrandRequests;
using Talabat.Core.RequestModels.CategoriesRequests;

namespace Felo.Talabat.Api.Helpers
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, s => s.MapFrom(P => P.Brand!.Name))
                .ForMember(d => d.Category, s => s.MapFrom(P => P.Category!.Name))
                .ForMember(d => d.PictureUrl, s => s.MapFrom<ProductWithPictureResolver>());

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DelivaryMethod, o => o.MapFrom(s => s.DelivaryMethod!.ShortName))
                .ForMember(d => d.DelivaryMethodCost, o => o.MapFrom(s => s.DelivaryMethod!.Cost))
                .ReverseMap();

            CreateMap<OrderItems, OrderItemsDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderWithPictureResolver>());

            CreateMap<UpdateProductRequest, Product>();
            CreateMap<AddProductRequest, Product>();

            CreateMap<BrandRequest, Brand>();

            CreateMap<AddCategoryRequest, Category>();
        }
    }
}
