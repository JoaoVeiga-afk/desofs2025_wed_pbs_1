using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ShopTex.Controllers;

[ApiController]
[Route("Error")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    /// Handles the exception and returns a response with the error.
    public ActionResult<ErrorController> Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context.Error;
        var response = HandleException(exception);
        return response;
    }


    private ActionResult<ErrorController> HandleException(Exception exception)
    {
        ObjectResult response;
        switch (exception)
        {
            case ArgumentNullException:
                response = BadRequest(exception.Message);
                break;
            case ArgumentException:
                response = BadRequest(exception.Message);
                break;
            default:
                response = StatusCode(503, exception.InnerException.Message);
                break;
        }

        return response;
    }
}