using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace API.Filters;

/// <summary>
/// Action filter to validate model state and return standardized error response
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var errorMessage = string.Join("; ", errors);

            var response = new ApiResponse(
                errorMessage,
                false,
                null,
                (int)HttpStatusCode.BadRequest
            );

            context.Result = new BadRequestObjectResult(response);
        }
    }
}
