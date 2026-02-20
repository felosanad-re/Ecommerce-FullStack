using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services.Contract.AuthServices;

namespace Talabat.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            // Climes
            var userClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var userRole = await userManager.GetRolesAsync(user);
            foreach (var role in userRole)
                userClaim.Add(new Claim(ClaimTypes.Role, role));
            // Create Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            // Create Token Object 
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:issuer"],
                    audience: _configuration["JWT:audience"],
                    userClaim,
                    expires: DateTime.UtcNow.AddDays(double.Parse( _configuration["JWT:expires"]!)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
