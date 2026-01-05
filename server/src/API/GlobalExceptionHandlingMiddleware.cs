using API;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;


public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
           
            await _next.Invoke(context);
            //var claims = context.User.Identities.First().Claims;
            //if(claims.Where(c=>c.Type.Contains("Ban")).Any())
            //{
            //    string status = claims.Where(c => c.Type == "Ban").First()?.Value;
            //    if (status.Contains("Banned"))
            //    {
            //        await HandleException(context, new NotValidUserException("User is banned"));
            //    }
            //}

            
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception exception)
    {
        ApiResponse apiResponse = new();

        switch (exception)
        {

            case UsernameIsTakenException usernameIsTakenException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                apiResponse.IsSuccess = false;
                apiResponse.Message = usernameIsTakenException.Message;
                apiResponse.Result = null;
                break;
            case MailNotConfirmedException mailNotConfirmedException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                apiResponse.IsSuccess = false;
                apiResponse.Message = mailNotConfirmedException.Message;
                apiResponse.Result = null;
                break;
            case RestrictedException restrictedException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                apiResponse.IsSuccess = false;
                apiResponse.Message = restrictedException.Message;
                apiResponse.Result = null;
                break;
            case InvalidArgumentException invalidArgumentException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                apiResponse.IsSuccess = false;
                apiResponse.Message = invalidArgumentException.Message;
                apiResponse.Result = null;
                break;
            case NotValidUserException notValidUserException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);
                apiResponse.IsSuccess = false;
                apiResponse.Message = notValidUserException.Message;
                apiResponse.Result = null;
                break;
            case NotFoundException notFoundException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.NotFound);
                apiResponse.IsSuccess = false;
                apiResponse.Message = notFoundException.Message;
                apiResponse.Result = null;
                break;
            case
                ArgumentException argumentException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
                apiResponse.IsSuccess = false;
                apiResponse.Message = argumentException.Message;
                apiResponse.Result = null;
                break;
            case
                UnauthorizedAccessException unauthorizedAccessException:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                apiResponse.IsSuccess = false;
                apiResponse.Message = unauthorizedAccessException.Message;
                apiResponse.Result = null;
                break;
            case
                Exception ex:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
                apiResponse.IsSuccess = false;
                apiResponse.Message = ex.Message;
                apiResponse.Result = null;
                break;
            default:
                apiResponse.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);
                apiResponse.IsSuccess = false;
                apiResponse.Message = "An unexpected error occurred.";
                apiResponse.Result = null;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = apiResponse.StatusCode;

        return context.Response.WriteAsJsonAsync(apiResponse);
    }
}
