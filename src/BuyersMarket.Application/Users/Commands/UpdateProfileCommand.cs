using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Users.Commands;

public record UpdateProfileCommand(
    string DisplayName,
    string PhoneNumber,
    string? AvatarUrl
) : IRequest<Result<UserDto>>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().Length(2, 50);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+7\d{10}$")
            .WithMessage("PhoneNumber must be in format +7XXXXXXXXXX.");
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly IDateTimeService _clock;

    public UpdateProfileCommandHandler(IApplicationDbContext db, ICurrentUserService current, IDateTimeService clock)
    {
        _db = db;
        _current = current;
        _clock = clock;
    }

    public async Task<Result<UserDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        if (_current.UserId is null)
            return Result<UserDto>.Failure(Error.Unauthorized("auth.unauthenticated", "Not authenticated."));

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _current.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("users.not_found", "User not found."));

        user.DisplayName = request.DisplayName.Trim();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = _clock.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email, user.DisplayName, user.PhoneNumber,
            user.Role, user.AvatarUrl, user.CreatedAt));
    }
}
