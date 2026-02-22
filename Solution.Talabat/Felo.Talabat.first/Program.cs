using Felo.Talabat.Api.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stripe;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services.Contract.AuthServices;
using Talabat.Repositaries.Data;

namespace Felo.Talabat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configuration

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Add secret file
            builder.Configuration
                .AddJsonFile("C:\\Users\\Act\\AppData\\Roaming\\Microsoft\\UserSecrets\\ac7b37e3-e66e-42fc-b593-ce3d528b9b78\\secrets.json",
                             optional: true,   // خليه optional علشان ميفشلش لو مش موجود
                             reloadOnChange: builder.Environment.IsDevelopment());

            // Add ShopDbContext
            builder.Services.AddDbContext<ShopDbContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add Identity DataBase
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            // Add Redis Connection

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var connStr = builder.Configuration.GetConnectionString("Redis");

                options.Configuration = connStr;

                options.InstanceName = "FastShop_";  // مهم عشان ما يتداخلش مع apps تانية
            });

            // Add Identity Services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Add All Identity Dependancy Injection
            builder.Services.AddIdentityServices(builder.Configuration);

            // Add All Application Services Dependancy
            builder.Services.AddApplicationServices();

            // Add Error Response Model
            builder.Services.AddError();

            builder.Services.AddCors(action =>
            {
                action.AddPolicy("AllowAngular", options =>
                {
                    options.WithOrigins(
                        "http://localhost:4200",
                        "https://localhost:4200",
                        "http://shop-web.runasp.net", // deploy in monester
                        "https://shop-web.runasp.net" // deploy in monester
                    )
                     .AllowAnyHeader().AllowAnyMethod().AllowCredentials(); ;
                });
            });
            #endregion
            StripeConfiguration.ApiKey = builder.Configuration["StripeSitting:SecretKey"];
            var app = builder.Build();

            #region Update DataBase And Applyed Migrations
            // Add Scope
            var scope = app.Services.CreateScope();
            // Add Service
            var services = scope.ServiceProvider;
            // Create Object from ShopDbContext Implicitaly
            var _context = services.GetRequiredService<ShopDbContext>();
            // Create Object from ApplicationDbContext Implicitaly
            var _ApplicationContext = services.GetRequiredService<ApplicationDbContext>();
            // Create Object From Ilooger Factory Implicitaly
            var logger = services.GetRequiredService<ILoggerFactory>();
            // Create New Object DbInitialization Implicitaly
            var _dbInitialization = services.GetRequiredService<IDbInitialization>();
            
            // Apply and Update Database
            try
            {
                await _ApplicationContext.Database.MigrateAsync();
                await _context.Database.MigrateAsync();
                await _dbInitialization.CreateInitializationAsync();
                // Create New Object ShopDbContextSeed Implicitaly
                await ShopDbContextSeed.SeedAsync(_context);
            }
            catch (Exception ex)
            {
                var _logger = logger.CreateLogger<Program>();
                _logger.LogError(ex, "Error in database");
            }
            #endregion
            //builder.Logging.AddFile("logs/app-{Date}.log");
            // Configure the HTTP request pipeline.
            #region Middlwears 
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();   // ده بيظهر stack trace كامل
            }
            else
            {
                // حتى في Production، خلينا نشوف الخطأ مؤقتاً
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            await context.Response.WriteAsync("Error: " + error.Error.Message + "\n" + error.Error.StackTrace);
                        }
                        else
                        {
                            await context.Response.WriteAsync("Unknown server error");
                        }
                    });
                });
            }
            app.UseRouting();

            app.UseCors("AllowAngular");
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
