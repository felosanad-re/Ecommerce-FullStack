using AutoMapper;
using Felo.Talabat.Api.ModelDto.AdminModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Core;
using Talabat.Core.Entites.Identity;

namespace Felo.Talabat.Api.Controllers.Admin
{
    [Authorize]
    [Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
    public class AdminUserController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public AdminUserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        #region GetAllAccount

        [HttpGet("Accounts")] // Get: /api/AdminUser/Accounts
        public async Task<ActionResult<ApplicationUserToReturn>> GetUSers()
        {
            var users = await _userManager.Users.AsQueryable().ToListAsync();
            var data = _mapper.Map<List<ApplicationUserToReturn>>(users);
            return Ok(data);
        }
        #endregion

        #region Delete User
        [HttpDelete("DeleteUser")] // Delete: /api/AdminUser/DeleteUser
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return BadRequest();

            var result = await _userManager.DeleteAsync(user);
            if (result is null) return BadRequest(result.Errors);
            return Ok(new
            {
                Message = "User Deleted SuccessFully"
            });
        }
        #endregion

        #region LockAccount
        [HttpPost("LockAccount")] // Post: /api/AdminUser/LockAccount
        public async Task<ActionResult> Lock(string id, double days)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return BadRequest(new
            {
                Message = "User Not Found"
            });

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(days));

            return Ok(new
            {
                Message = "User Lock Succseeded",
                LockDays = $"Account Lock For: {days} days"
            });
        }
        #endregion

        #region
        [HttpPost("ActiveAccount")] // Post: /api/AdminUser/ActiveAccount
        public async Task<ActionResult> Active(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return BadRequest(new
            {
                Message = "User not found"
            });

            await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok(new
            {
                Message = "User Active Now"
            });
        }
        #endregion
    }
}
