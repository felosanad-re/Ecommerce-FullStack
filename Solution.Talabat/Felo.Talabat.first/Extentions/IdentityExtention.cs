using Felo.Talabat.Api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Services.Contract.AuthServices;
using Talabat.Repositaries;
using Talabat.Services.AuthServices;

namespace Felo.Talabat.Api.Extentions
{
    public static class IdentityExtention
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration _configuration)
        {
            // Allow Dependancy For EmailSender
            services.AddTransient<IEmailSender, EmailSender>();

            // Add dependance Injection For DbInitialization
            services.AddScoped(typeof(IDbInitialization), typeof(DbInitialization));

            // Add dependance Injection For AuthService
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:audience"],
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(_configuration["JWT:expires"])),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]))
                };
            });
            return services;
        }
    }
}
