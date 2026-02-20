using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entites.Identity;
using Talabat.Core.Services.Contract.AuthServices;

namespace Talabat.Repositaries
{
    public class DbInitialization : IDbInitialization
    {
        #region Services
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialization(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion
        public async Task CreateInitializationAsync()
        {
            string[] rolesNames = ["SUPER_ADMIN", "ADMIN", "CUSTOMER"];
            if (!_roleManager.Roles.Any())
            {
                foreach (var roleName in rolesNames)
                {
                    // Check if the role already exists
                    if (!await _roleManager.RoleExistsAsync(roleName))
                        // Create the role if it doesn't exist
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            // Create SuperAdmin Account
            var user = new ApplicationUser()
            {
                FirstName = "Felo",
                LastName = "Sanad",
                UserName = "SuperAdmin",
                Email = "SuperAdmin@Talabat.com",
                Address = "Elemam El Gazaly Impapa Egypt",
            };
            var userEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userEmail is null)
            {
                var result = await _userManager.CreateAsync(user, "Admin123$");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        throw new Exception(error.Description);
                    }
                }
            await _userManager.AddToRoleAsync(user, "SUPER_ADMIN");
            }
        }
    }
}
