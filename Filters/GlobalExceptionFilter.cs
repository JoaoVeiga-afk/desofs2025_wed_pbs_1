using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ShopTex.Domain.Shared;

namespace ShopTex.Filters;

public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _factory;
    public GlobalExceptionFilter(ProblemDetailsFactory factory) => _factory = factory;

    public void OnException(ExceptionContext ctx)
    {
        int status = ctx.Exception switch
        {
            BusinessRuleValidationException => 400,
            UnauthorizedAccessException     => 401,
            _                               => 500
        };

        var problem = _factory.CreateProblemDetails(
            ctx.HttpContext,
            statusCode: status,
            title:      ctx.Exception.GetType().Name,
            detail:     ctx.Exception.Message);

        problem.Extensions.Remove("traceId");   

        ctx.Result = new ObjectResult(problem) { StatusCode = status };
        ctx.ExceptionHandled = true;
    }
}