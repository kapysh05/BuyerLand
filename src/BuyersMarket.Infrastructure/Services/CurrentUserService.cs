using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace BuyersMarket.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var sub = Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = Principal?.FindFirst(ClaimTypes.Role)?.Value
                ?? Principal?.FindFirst("role")?.Value;
            return Enum.TryParse<UserRole>(role, out var r) ? r : null;
        }
    }

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
}
