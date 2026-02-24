namespace Felo.Talabat.Api.ModelDto.AdminModels
{
    public class ApplicationUserToReturn
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
        public string LockoutEnabled { get; set; }
        public string LockoutEnd { get; set; }
    }
}
