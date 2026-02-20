
namespace Felo.Talabat.Api.Errors
{
    public class ResponseApi
    {
        // بعمل فيه صيغه ثابته لكل اشكال الايورس
        public int StatusCode { get; set; }
        public List<string>? Message { get; set; }

        public ResponseApi(int statusCode, List<string>? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? new List<string> { GetDefaultMessageStatusCode(statusCode) }; // fun Get Default message status code
        }

        private string? GetDefaultMessageStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A Bad Request",
                401 => "Autorized, you are not",
                404 => "Not found Product",
                500 => "Server Error",
                _ => null // Any Error Come Null
            };
        }
    }
}
