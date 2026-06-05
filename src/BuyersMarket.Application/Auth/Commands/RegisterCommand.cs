using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Entities;
using BuyersMarket.Domain.Enums;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName,
    string PhoneNumber,
    UserRole Role
) : IRequest<Result<AuthResponseDto>>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        RuleFor(x => x.DisplayName).NotEmpty().Length(2, 50);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+7\d{10}$")
            .WithMessage("PhoneNumber must be in format +7XXXXXXXXXX.");
        RuleFor(x => x.Role).Must(r => r == UserRole.Customer || r == UserRole.Buyer)
            .WithMessage("Role must be Customer or Buyer.");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly IDateTimeService _clock;

    public RegisterCommandHandler(IApplicationDbContext db, IPasswordHasher hasher, IJwtService jwt, IDateTimeService clock)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _clock = clock;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var emailNorm = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == emailNorm, ct);
        if (exists)
            return Result<AuthResponseDto>.Failure(Error.Conflict("auth.email_taken", "Email is already in use."));

        var now = _clock.UtcNow;
        var user = new User
        {
            Email = emailNorm,
            PasswordHash = _hasher.Hash(request.Password),
            DisplayName = request.DisplayName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Role = request.Role,
            IsActive = true,
            CreatedAt = now
        };
        _db.Users.Add(user);

        if (request.Role == UserRole.Buyer)
        {
            _db.BuyerProfiles.Add(new BuyerProfile
            {
                UserId = user.Id,
                CreatedAt = now
            });
        }

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
