using AutoMapper;
using Felo.Talabat.Api.Helpers.Resolvers;
using Felo.Talabat.Api.ModelDto.AdminModels;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Felo.Talabat.Api.ModelDto.Products;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.RequestModels;
using Talabat.Core.RequestModels.BrandRequests;
using Talabat.Core.RequestModels.CategoriesRequests;
using Talabat.Core.RequestModels.Orders;
using Talabat.Core.RequestModels.Products;

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

            // For Export
            CreateMap<Product, ProductExportToReturn>()
                .ForMember(d => d.BrandId, o => o.MapFrom(s => s.Brand.Id))
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Category.Id))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s=> s.PictureUrl));

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DelivaryMethod, o => o.MapFrom(s => s.DelivaryMethod!.ShortName))
                .ForMember(d => d.DelivaryMethodCost, o => o.MapFrom(s => s.DelivaryMethod!.Cost))
                .ReverseMap();


            CreateMap<OrderItems, OrderItemsDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderWithPictureResolver>());

            // For Export
            CreateMap<Order, OrderExportToReturn>()
                .ForMember(d => d.DelivaryMethodId, o => o.MapFrom(s => s.DelivaryMethod.Id))
                .ForMember(d => d.DelivaryMethodName, o => o.MapFrom(s => s.DelivaryMethod.ShortName))
                .ForMember(d => d.AddressShiper, o => o.MapFrom(s => s.AddressShiper));

            CreateMap<OrderItems, OrderItemsExportToReturn>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

            CreateMap<AddProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>();

            CreateMap<BrandRequest, Brand>();

            CreateMap<AddCategoryRequest, Category>();

            CreateMap<ApplicationUser, ApplicationUserToReturn>();
            CreateMap<Order, OrderStatusToReturnDto>();
        }
    }
}
