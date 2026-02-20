namespace Talabat.Core.Entites.Identity
{
    public class ApplicationUserOtp : BaseModelRedis
    {
        public string Code { get; set; }
        public string Email { get; set; }
    }
}
