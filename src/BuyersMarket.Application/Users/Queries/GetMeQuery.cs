using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Users.Queries;

public record GetMeQuery() : IRequest<Result<UserDto>>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;

    public GetMeQueryHandler(IApplicationDbContext db, ICurrentUserService current)
    {
        _db = db;
        _current = current;
    }

    public async Task<Result<UserDto>> Handle(GetMeQuery request, CancellationToken ct)
    {
        if (_current.UserId is null)
            return Result<UserDto>.Failure(Error.Unauthorized("auth.unauthenticated", "Not authenticated."));

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _current.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("users.not_found", "User not found."));

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email, user.DisplayName, user.PhoneNumber,
            user.Role, user.AvatarUrl, user.CreatedAt));
    }
}
