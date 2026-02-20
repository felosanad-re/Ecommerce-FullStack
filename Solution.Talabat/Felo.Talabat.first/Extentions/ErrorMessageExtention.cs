using Felo.Talabat.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Felo.Talabat.Api.Extentions
{
    public static class ErrorMessageExtention
    {
        public static IServiceCollection AddError(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ((actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                .SelectMany(P => P.Value.Errors)
                                                .Select(E => E.ErrorMessage);
                    var response = new ResponceValidationError()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                });
            });

            return services;
        }
    }
}
