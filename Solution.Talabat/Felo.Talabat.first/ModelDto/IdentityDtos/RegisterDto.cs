using System.ComponentModel.DataAnnotations;

namespace Felo.Talabat.Api.ModelDto.IdentityDtos
{
    public class RegisterDto // Request
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string? Address { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
