using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IDateTimeService _clock;

    public RefreshTokenCommandHandler(IApplicationDbContext db, IJwtService jwt, IDateTimeService clock)
    {
        _db = db;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var token = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct);

        if (token is null || !token.IsActive)
            return Result<AuthResponseDto>.Failure(
                Error.Unauthorized("auth.invalid_refresh", "Invalid or expired refresh token."));

        token.RevokedAt = _clock.UtcNow;

        var newRefresh = _jwt.GenerateRefreshToken(token.UserId);
        _db.RefreshTokens.Add(newRefresh);
        await _db.SaveChangesAsync(ct);

        var access = _jwt.GenerateAccessToken(token.User);
        var accessExpiresAt = _jwt.GetAccessTokenExpiry();

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            token.User.Id, token.User.Email, token.User.DisplayName, token.User.Role,
            access, newRefresh.Token, accessExpiresAt));
    }
}
