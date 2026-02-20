using System.ComponentModel.DataAnnotations;

namespace Felo.Talabat.Api.ModelDto.IdentityDtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
