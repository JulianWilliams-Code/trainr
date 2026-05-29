using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Trainr.Application.Common;

namespace Trainr.API.Filters;

/// <summary>
/// Intercepts invalid ModelState before it reaches the controller and
/// returns a consistent ApiResponse shape instead of the default ProblemDetails.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Fail("Validation failed.", errors));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
