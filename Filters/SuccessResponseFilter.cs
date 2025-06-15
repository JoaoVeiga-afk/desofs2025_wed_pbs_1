using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace ShopTex.Filters
{
    public sealed class SuccessResponseFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult result &&
                result.StatusCode is >= 200 and < 300 &&
                result.Value is not null &&
                result.Value is not ProblemDetails)
            {
                context.Result = new ObjectResult(new
                {
                    code = result.StatusCode,
                    status = "success",
                    data = result.Value is IEnumerable<object> or Array
                        ? result.Value
                        : new[] { result.Value }
                })
                {
                    StatusCode = result.StatusCode
                };
            }

            else if (context.Result is OkResult)
            {
                context.Result = new ObjectResult(new
                {
                    code = 200,
                    status = "success",
                    data = new object[0]
                })
                {
                    StatusCode = 200
                };
            }

            else if (context.Result is CreatedResult created && created.Value is not null)
            {
                context.Result = new ObjectResult(new
                {
                    code = 201,
                    status = "success",
                    data = new[] { created.Value }
                })
                {
                    StatusCode = 201
                };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context) { }
    }
}