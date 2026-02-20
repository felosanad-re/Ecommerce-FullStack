using System.ComponentModel.DataAnnotations;

namespace Felo.Talabat.Api.ModelDto.IdentityDtos
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string ResetToken { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password Not Matched")]
        public string ConfairmPassword { get; set; }
    }
}
