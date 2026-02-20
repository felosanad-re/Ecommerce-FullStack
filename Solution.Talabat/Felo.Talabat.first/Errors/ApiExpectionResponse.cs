namespace Felo.Talabat.Api.Errors
{
    public class ApiExpectionResponse : ResponseApi
    {
        // عملته علشان اهندل مشكله الايرور سيرفر
        public string? Details { get; set; }

        public ApiExpectionResponse(int statusCode, List<string>? message = null, string? details =null)
            : base(statusCode, message)
        {
            Details = details;
        }
    }
}
