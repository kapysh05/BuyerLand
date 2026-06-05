using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeService _clock;

    public LogoutCommandHandler(IApplicationDbContext db, IDateTimeService clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken ct)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct);
        if (token is null)
            return Result<bool>.Failure(Error.NotFound("auth.refresh_not_found", "Refresh token not found."));

        if (token.RevokedAt is null)
        {
            token.RevokedAt = _clock.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        return Result<bool>.Success(true);
    }
}
