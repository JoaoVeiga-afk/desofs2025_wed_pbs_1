using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;

namespace ShopTex.Config;

public sealed class ProblemDetailsFilter : IResultFilter
{
    private readonly ProblemDetailsFactory _factory;
    public ProblemDetailsFilter(ProblemDetailsFactory factory) => _factory = factory;

    public void OnResultExecuting(ResultExecutingContext ctx)
    {
        ctx.Result = ctx.Result switch
        {
            BadRequestObjectResult br when br.Value is not ProblemDetails
                => Wrap(br.Value, 400, ctx),
            BadRequestResult
                => Wrap(null, 400, ctx),

            UnauthorizedObjectResult ur when ur.Value is not ProblemDetails
                => Wrap(ur.Value, 401, ctx),
            UnauthorizedResult
                => Wrap(null, 401, ctx),

            NotFoundObjectResult nf when nf.Value is not ProblemDetails
                => Wrap(nf.Value, 404, ctx),
            NotFoundResult
                => Wrap(null, 404, ctx),

            ObjectResult obj when obj.StatusCode is >= 400 and <= 599 && obj.Value is not ProblemDetails
                => Wrap(obj.Value, obj.StatusCode ?? 500, ctx),

            StatusCodeResult sc when sc.StatusCode is >= 400 and <= 599
                => Wrap(null, sc.StatusCode, ctx),

            OkObjectResult ok when ok.Value is string str
                => new OkObjectResult(new { message = str }),

            _ => ctx.Result
        };
    }

    public void OnResultExecuted(ResultExecutedContext ctx)
    {

    }

    private static string? GetDetail(object? val)
    {
        if (val == null) return null;

        if (val is string s) return s;

        var type = val.GetType();
        var prop = type.GetProperty("Message") ?? type.GetProperty("Detail");
        if (prop is { PropertyType: { } pt } && pt == typeof(string))
            return prop.GetValue(val) as string;

        return JsonSerializer.Serialize(val);
    }

    private ObjectResult Wrap(object? val, int status, ResultExecutingContext c)
    {
        var problem = _factory.CreateProblemDetails(
            c.HttpContext,
            statusCode: status,
            detail: GetDetail(val));

        problem.Extensions.Remove("traceId");

        return new ObjectResult(problem) { StatusCode = status };
    }
}
