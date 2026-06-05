using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var err = result.Error!;
        var status = err.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = err.Code,
            Detail = err.Message
        };
        return new ObjectResult(problem) { StatusCode = status };
    }
}
