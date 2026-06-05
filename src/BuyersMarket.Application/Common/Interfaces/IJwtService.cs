using BuyersMarket.Domain.Entities;

namespace BuyersMarket.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    DateTime GetAccessTokenExpiry();
    RefreshToken GenerateRefreshToken(Guid userId);
    Guid? ValidateAccessToken(string token);
}
