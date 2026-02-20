using System.ComponentModel.DataAnnotations;

namespace Felo.Talabat.Api.ModelDto.IdentityDtos
{
    public class CheckCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
