using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IApplicationDbContext db, IPasswordHasher hasher, IJwtService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var emailNorm = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == emailNorm, ct);

        var invalid = Error.Unauthorized("auth.invalid_credentials", "Invalid email or password.");
        if (user is null || !user.IsActive)
            return Result<AuthResponseDto>.Failure(invalid);

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure(invalid);

        var refresh = _jwt.GenerateRefreshToken(user.Id);
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync(ct);

        var access = _jwt.GenerateAccessToken(user);
        var accessExpiresAt = _jwt.GetAccessTokenExpiry();

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            user.Id, user.Email, user.DisplayName, user.Role,
            access, refresh.Token, accessExpiresAt));
    }
}
