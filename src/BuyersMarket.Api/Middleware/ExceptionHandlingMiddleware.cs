using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = ex.Message
            };
            ctx.Response.StatusCode = problem.Status.Value;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
