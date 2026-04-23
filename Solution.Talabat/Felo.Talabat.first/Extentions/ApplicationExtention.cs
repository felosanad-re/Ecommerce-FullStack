using Felo.Talabat.Api.Helpers;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.AttachmentService;
using Talabat.Core.Services.Contract.CartServices;
using Talabat.Core.Services.Contract.ExportServices;
using Talabat.Core.Services.Contract.HubServices;
using Talabat.Core.Services.Contract.NotificationsServices;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Services.Contract.PaymentsService;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.UnitOfWork;
using Talabat.Repositaries.Data;
using Talabat.Repositaries.Data.UnitOfWorks;
using Talabat.Services.AttachmentServices;
using Talabat.Services.CartServices;
using Talabat.Services.ExportServices;
using Talabat.Services.HubServices;
using Talabat.Services.NotificationServices;
using Talabat.Services.OrderServices;
using Talabat.Services.PaymentServices;
using Talabat.Services.ProductServices;

namespace Felo.Talabat.Api.Extentions
{
    public static class ApplicationExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add Localization
            services.AddLocalization(o => o.ResourcesPath = "Localization");
            // Add Export
            services.AddScoped<IExportService, ExportService>();
            //Allow NotificationServices
            services.AddScoped(typeof(INotificationService), typeof(NotificationService));
            //Allow IOrderTrackingHub
            services.AddScoped(typeof(IOrderTracingServiceHub), typeof(OrderServiceTracking));
            //Allow Attachments
            services.AddScoped(typeof(IAttachmentService), typeof(AttachmentService));
            // Allow IPaymentService
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            // Allow IOrderBuilder
            services.AddScoped(typeof(IOrderBuilder), typeof(OrderBuilder));
            // Allow Order Service
            services.AddScoped(typeof(IOrderServices), typeof(OrderService));
            // Allow ICartService
            services.AddScoped(typeof(ICartService), typeof(CartService));
            // Allow dependency for AutoMapper
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(typeof(ProfileMapping));
            // Allow dependency for Product Services
            services.AddScoped(typeof(IProductService), typeof(ProductService));

            // Allow dependency for Brand Service
            services.AddScoped(typeof(IBrandService), typeof(BrandService));

            // Allow dependency for Category Service 
            services.AddScoped(typeof(ICategoryService), typeof(CategoryService));

            // Allow dependency for IUnit Of Work
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            // Allow dependency For Redis
            services.AddScoped(typeof(IRedisRepo<>), typeof(RedisRepo<>));

            return services;
        }
    }
}
