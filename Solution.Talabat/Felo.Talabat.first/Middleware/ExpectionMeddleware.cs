using Felo.Talabat.Api.Errors;
using System.Net;
using System.Text.Json;

namespace Talabat.APIs.Middleware
{
    // First Middleware
    public class ExpectionMeddleware
    {
        private readonly ILogger<ExpectionMeddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        
        public ExpectionMeddleware(RequestDelegate next,IWebHostEnvironment env, ILogger<ExpectionMeddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        // Create Fun -> ByConvision
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Take Action In Request
                await _next.Invoke(httpContext); // Go To Next Middleware
                // Take Action in Response
            }
            catch (Exception ex)
            {
                // 1. Log Error
                _logger.LogError(ex.Message);

                // Adjust Header
                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                // Create Class Content Body Message [ApiExpectionResponse]
                var response = _env.IsDevelopment() ?
                    new ApiExpectionResponse((int)HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message }, ex.StackTrace.ToString())
                        : 
                    new ApiExpectionResponse((int)HttpStatusCode.InternalServerError);
                // علشان احل مشكله الكاميل كيس علشان الفرونت ايند يقدر انه يستخدمها في الجيسون
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
