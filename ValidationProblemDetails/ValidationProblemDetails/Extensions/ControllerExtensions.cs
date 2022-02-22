using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc
{
    internal static class ControllerExtensions
    {
        public static ModelStateDictionary AddErrorWithCode(this ModelStateDictionary stateDictionary, string key, string message, string errorCode)
        {
            stateDictionary.AddModelError(key, $"{errorCode}|{message}");
            return stateDictionary;
        }

        public static IActionResult ValidationErrorResult(this ControllerBase controllerBase)
        {
            var apiBehaviorOptions = controllerBase.HttpContext.RequestServices.GetService(typeof(IOptions<ApiBehaviorOptions>)) as IOptions<ApiBehaviorOptions>;

            if (apiBehaviorOptions == null)
                return new BadRequestObjectResult(new ValidationProblemDetails(controllerBase.ModelState));

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(controllerBase.ControllerContext);
        }
    }
}
