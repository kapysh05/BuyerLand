using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _db;

    public GetUserByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure(Error.NotFound("users.not_found", "User not found."));

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email, user.DisplayName, user.PhoneNumber,
            user.Role, user.AvatarUrl, user.CreatedAt));
    }
}
