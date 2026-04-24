using Felo.Talabat.Api.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Stripe;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services.Contract.AuthServices;
using Talabat.Repositaries.Data;
using Talabat.Repositaries.Data.Hubs;
using File = System.IO.File;

namespace Felo.Talabat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                #region Configuration

                builder.Services.AddControllers()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();


                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.AddUserSecrets<Program>(optional: true);
                }

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

                builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
                        ?? throw new InvalidOperationException("Redis connection string is missing.");

                    var redisUri = new Uri(redisConnectionString);
                    var configuration = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        ConnectRetry = 3,
                        ConnectTimeout = 10000,
                        SyncTimeout = 10000,
                        AsyncTimeout = 10000,
                        KeepAlive = 60,
                        Ssl = redisUri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase),
                        SslHost = redisUri.Host,
                        User = string.IsNullOrWhiteSpace(redisUri.UserInfo.Split(':')[0])
                            ? null
                            : redisUri.UserInfo.Split(':')[0],
                        Password = redisUri.UserInfo.Contains(':')
                            ? redisUri.UserInfo[(redisUri.UserInfo.IndexOf(':') + 1)..]
                            : null,
                    };

                    configuration.EndPoints.Add(redisUri.Host, redisUri.Port);
                    return ConnectionMultiplexer.Connect(configuration);
                });

                // Add SignelR
                builder.Services.AddSignalR();

                // Add Identity Services
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 5;
                }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

                // Add All Identity Dependency Injection
                builder.Services.AddIdentityServices(builder.Configuration);

                // Add All Application Services Dependency
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
                            "http://shoppingfast.runasp.net",
                            "https://shoppingfast.runasp.net"
                        )
                         .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                    });
                });
                #endregion
                StripeConfiguration.ApiKey = builder.Configuration["StripeSitting:SecretKey"];
                var app = builder.Build();

                #region Update DataBase And Applied Migrations
                // Add Scope
                var scope = app.Services.CreateScope();
                // Add Service
                var services = scope.ServiceProvider;
                // Create Object from ShopDbContext Implicitly
                var _context = services.GetRequiredService<ShopDbContext>();
                // Create Object from ApplicationDbContext Implicitly
                var _ApplicationContext = services.GetRequiredService<ApplicationDbContext>();
                // Create Object From ILogger Factory Implicitly
                var logger = services.GetRequiredService<ILoggerFactory>();
                // Create New Object DbInitialization Implicitly
                var _dbInitialization = services.GetRequiredService<IDbInitialization>();

                // Apply and Update Database
                try
                {
                    await _ApplicationContext.Database.MigrateAsync();
                    await _context.Database.MigrateAsync();
                    await _dbInitialization.CreateInitializationAsync();
                    // Create New Object ShopDbContextSeed Implicitly
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
                #region Midllweare
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
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

                app.MapHub<HubSignalR>("/orderHub"); // Angular Listing

                #endregion

                app.Run();
            }
            catch (Exception ex)
            {
                WriteStartupFailure(ex);
                throw;
            }
        }

        private static void WriteStartupFailure(Exception ex)
        {
            var message = $"""
            [{DateTime.UtcNow:O}] Application startup failed.
            {FlattenException(ex)}

            """;

            foreach (var logPath in GetStartupLogPaths())
            {
                try
                {
                    var directory = Path.GetDirectoryName(logPath);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.AppendAllText(logPath, message);
                }
                catch
                {
                    // Try the next writable location.
                }
            }

            try
            {
                Console.Error.WriteLine(message);
            }
            catch
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static IEnumerable<string> GetStartupLogPaths()
        {
            var paths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "startup-error.log"),
                Path.Combine(AppContext.BaseDirectory, "logs", "startup-error.log"),
                Path.Combine(Path.GetTempPath(), "AdminPanel.Apis", "startup-error.log")
            };

            return paths.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static string FlattenException(Exception ex)
        {
            var lines = new List<string>();
            var current = ex;
            var level = 0;

            while (current != null)
            {
                lines.Add($"Level {level}: {current.GetType().FullName}");
                lines.Add($"Message: {current.Message}");
                lines.Add(current.StackTrace ?? "No stack trace available.");
                lines.Add(string.Empty);
                current = current.InnerException!;
                level++;
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}

