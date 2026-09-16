using System.Reflection;
using System.Text;
using BuyersMarket.Api.Hubs;
using BuyersMarket.Api.Middleware;
using BuyersMarket.Application;
using BuyersMarket.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecret = jwtSection["Secret"] ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BuyersMarket API",
        Version = "v1",
        Description = """
            Маркетплейс байеров: заказчик публикует тендер — байеры откликаются офферами с ценой — из принятого оффера рождается сделка.

            **Реализовано:** Auth (регистрация, вход, ротация refresh-токенов), Users (профили), справочники (категории, валюты, бренды), создание тендера.

            **Есть в схеме БД, но без бизнес-логики:** Offers, Deals, чат.
            """
    });

    // XML-комменты из всех проектов (API + Application для DTO/команд).
    var baseDir = AppContext.BaseDirectory;
    foreach (var xml in Directory.EnumerateFiles(baseDir, "BuyersMarket.*.xml"))
        c.IncludeXmlComments(xml, includeControllerXmlComments: true);

    c.SupportNonNullableReferenceTypes();
    c.UseAllOfToExtendReferenceSchemas();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Введи только сам токен — \"Bearer \" префикс добавится сам.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger включён в любом окружении. В Development доступ свободный (креды не заданы),
// в Staging/Production — закрыт Basic Auth, креды из конфигурации Swagger:BasicAuth:*.
app.UseMiddleware<SwaggerBasicAuthMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BuyersMarket API v1");
    c.DocumentTitle = "BuyersMarket API";
});

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();

public partial class Program { }
