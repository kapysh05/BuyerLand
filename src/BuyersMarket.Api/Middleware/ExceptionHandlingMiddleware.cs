using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Middleware;

/// <summary>
/// Глобальный перехват необработанных исключений → 500 + ProblemDetails.
/// В Production наружу уходит обобщённый Detail и traceId; реальное сообщение
/// и стек только в логах под тем же traceId.
/// В Development можно отдавать ex.Message для удобства отладки.
/// Доменные ошибки идут через Result-pattern и сюда не попадают.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            var traceId = ctx.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            if (ctx.Response.HasStarted)
            {
                // Если ответ уже начал отправляться — заголовки/тело перезаписать нельзя.
                return;
            }

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred."
            };
            problem.Extensions["traceId"] = traceId;

            ctx.Response.Clear();
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(problem, ctx.RequestAborted);
        }
    }
}
