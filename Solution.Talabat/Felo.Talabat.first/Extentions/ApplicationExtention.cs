using Felo.Talabat.Api.Helpers;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.CartServices;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Services.Contract.PaymentsService;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.UnitOfWork;
using Talabat.Repositaries.Data;
using Talabat.Repositaries.Data.UnitOfWorks;
using Talabat.Services.CartServices;
using Talabat.Services.OrderServices;
using Talabat.Services.PaymentServices;
using Talabat.Services.ProductServices;

namespace Felo.Talabat.Api.Extentions
{
    public static class ApplicationExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            // Allow IPaymentService
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            // Allow IOrderBuilder
            services.AddScoped(typeof(IOrderBuilder), typeof(OrderBuilder));
            // Allow Order Service
            services.AddScoped(typeof(IOrderServices), typeof(OrderService));
            // Allow ICartService
            services.AddScoped(typeof(ICartService), typeof(CartService));
            // Allow dependancy for AutoMapper
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(ProfileMapping));
            // Allow dependancy for Product Services
            services.AddScoped(typeof(IProductService), typeof(ProductService));

            // Allow dependancy for Brand Service
            services.AddScoped(typeof(IBrandService), typeof(BrandService));

            // Allow dependancy for Category Service 
            services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

            // Allow dependancy for IUnit Of Work
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            // Allow dependance For Redis
            services.AddScoped(typeof(IRedisRepo<>), typeof(RedisRepo<>));

            return services;
        }
    }
}
