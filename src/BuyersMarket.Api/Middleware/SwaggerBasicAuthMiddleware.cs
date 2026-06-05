using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace BuyersMarket.Api.Middleware;

/// <summary>
/// Защищает /swagger и /swagger/v1/swagger.json через HTTP Basic Auth.
/// Логин/пароль читаются из конфигурации Swagger:BasicAuth:Username|Password.
/// Если креды не заданы — middleware no-op (доступ свободный, как в Development).
/// </summary>
public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _user;
    private readonly string? _pass;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _user = config["Swagger:BasicAuth:Username"];
        _pass = config["Swagger:BasicAuth:Password"];
    }

    public async Task Invoke(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value ?? string.Empty;
        var isSwagger = path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);

        if (!isSwagger || string.IsNullOrEmpty(_user) || string.IsNullOrEmpty(_pass))
        {
            await _next(ctx);
            return;
        }

        var header = ctx.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(header) && header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var encoded = AuthenticationHeaderValue.Parse(header).Parameter ?? string.Empty;
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var idx = decoded.IndexOf(':');
                if (idx > 0)
                {
                    var user = decoded[..idx];
                    var pass = decoded[(idx + 1)..];
                    if (FixedTimeEquals(user, _user) && FixedTimeEquals(pass, _pass))
                    {
                        await _next(ctx);
                        return;
                    }
                }
            }
            catch
            {
                // fallthrough → 401
            }
        }

        ctx.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\", charset=\"UTF-8\"";
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var ab = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        if (ab.Length != bb.Length) return false;
        return CryptographicOperations.FixedTimeEquals(ab, bb);
    }
}
