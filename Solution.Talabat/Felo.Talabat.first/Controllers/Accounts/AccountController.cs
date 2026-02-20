using Felo.Talabat.Api.ModelDto.IdentityDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Entites.Identity;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.AuthServices;

namespace Felo.Talabat.Api.Controllers.Accounts
{

    public class AccountController : BaseController
    {
        #region Services
        private readonly IRedisRepo<ApplicationUserOtp> _redisOtp;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IRedisRepo<ApplicationUserOtp> redisOtp,
            IAuthService authService)
        {
            _redisOtp = redisOtp;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _authService = authService;
        }

        #endregion

        #region Register

        [HttpPost("Register")] // Post: /api/Account/Register
        public async Task<ActionResult<RegisterToReturnDto>> Register([FromBody] RegisterDto registerDto)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Address = registerDto.Address
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Send Email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = $"{Request.Scheme}://{Request.Host}/api/Account/Confairm?userId={user.Id}&token={token}";

                await _emailSender.SendEmailAsync(user.Email, "Confairm Your Account"
                    , $"<h1>Please Confairm Your Account By Click <a href='{link}'>here</a></h1>");
                await _userManager.AddToRoleAsync(user, SD.CUSTOMER);
                return Ok(new RegisterToReturnDto()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = await _authService.CreateTokenAsync(user, _userManager)
                });
            }
            else
            {
                return BadRequest(new
                {
                    error = result.Errors.Select(E => E.Description)
                });
            }
        }
        #endregion

        #region Confirm

        [HttpGet("ConfirmAccount")] // Get: /api/Account/ConfirmAccount
        public async Task<IActionResult> ConfirmAccount([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return BadRequest(new
            {
                error = "Invalid Email Confirmation Request"
            });
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest(new
            {
                error = "Invalid Token"
            });
            return Ok("Confairm Email Succeeded");
        }
        #endregion

        #region Login

        [HttpPost("Login")] // Post: /api/Account/Login
        public async Task<ActionResult<LoginToReturnDto>> Login([FromBody] LoginDto loginDto) 
        { 
            // Check On Email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) 
            {
                return NotFound(new
                {
                    error = "This Email Not Found"
                });
            }
            var flag = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (flag)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, false);
                if (result.IsNotAllowed) return Unauthorized(new
                {
                    error = "You Not Allow To Access Here"
                });
                if (result.IsLockedOut) return BadRequest(new
                {
                    error = "Your Account Is Locked"
                });
                if (result.Succeeded) return Ok(new LoginToReturnDto()
                {
                    UserName = user.UserName!,
                    Token = await _authService.CreateTokenAsync(user, _userManager)
                });
            }
            return BadRequest(new
            {
                error = "this Password Not Correct"
            });
        }

        #endregion

        #region Reset Password

        #region Step: 1 Send Otp
        [HttpPost("ForgetPassword")] // Post: /api/Account/ForgetPassword
        public async Task<ActionResult<CheckEmailToReturnDto>> ForgetPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null) return NotFound(new
            {
                error = "Email Not Found"
            });
            // Create Otp 4 Digits
            var code = new Random().Next(1000, 9999);

            // Save Token In Redis Memory
            await _redisOtp.SetCacheAsync(new ApplicationUserOtp()
            {
                Code = code.ToString(),
                Id = user.Email!, // key
                Email = user.Email!,
            });
            // Send Otp Message
            await _emailSender.SendEmailAsync(user.Email!, "Reset Password Code",
                $"<h1>Please Use This Code {code}</h1>");

            return Ok(new CheckEmailToReturnDto()
            {
                UserId = user.Id,
            });
        }
        #endregion

        #region Check On Otp

        [HttpPost("CheckCode")] // Post: /api/Account/CheckCode
        public async Task<ActionResult> CheckCode([FromBody] CheckCodeDto checkCodeDto)
        {
            var getCode = await _redisOtp.GetCacheAsync(checkCodeDto.Email);
            if (getCode == null) return BadRequest(new
            {
                error = "Code Or Invaild"
            });
            if (getCode.Code != checkCodeDto.Code) return BadRequest(new
            {
                error = "This Code Incorrect Or Is't Expaired "
            });

            // Generate reset token
            var resetToken = Guid.NewGuid().ToString();

            // Delete OTP (one-time use)
            await _redisOtp.RemoveCacheAsync(checkCodeDto.Email);
            await _redisOtp.SetCacheAsync(new ApplicationUserOtp()
            {
                Id = $"reset:{checkCodeDto.Email}",
                Code = resetToken,
            });
            return Ok(new
            {
                message = "You Can Change Your Password",
                resetToken
            });
        }

        #endregion

        #region ResetPassword
        [HttpPost("ResetPassword")] // Post: /api/Account/ResetPassword
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            // Get resetToken
            var resetToken = await _redisOtp.GetCacheAsync($"reset:{resetPasswordDto.Email}");
            // resetToken is expaird or no
            if (resetToken is null) return BadRequest(new { error = "Reset token expired or invalid" });
            // resetToken compare with resetToken in redis
            if (resetToken.Code != resetPasswordDto.ResetToken) return BadRequest(new { error = "Invalid reset token" });
            // Find User
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user is null) return BadRequest(new {error = "User Not Found"});

            var dummyToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, dummyToken, resetPasswordDto.NewPassword);
            if(result is null) return BadRequest(new {error = result!.Errors});

            // Remove Code
            await _redisOtp.RemoveCacheAsync($"reset:{resetPasswordDto.Email}");
            return Ok(new
            {
                message = "Password changed successfully"
            });
        }
        #endregion

        #endregion
    }
}
