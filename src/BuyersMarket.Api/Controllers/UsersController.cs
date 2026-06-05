using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Users.Commands;
using BuyersMarket.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

/// <summary>
/// Профили пользователей.
/// </summary>
[ApiController]
[Authorize]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Возвращает профиль текущего аутентифицированного пользователя.</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(CancellationToken ct)
        => (await _mediator.Send(new GetMeQuery(), ct)).ToActionResult();

    /// <summary>Профиль пользователя по идентификатору.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => (await _mediator.Send(new GetUserByIdQuery(id), ct)).ToActionResult();

    /// <summary>Обновление профиля текущего пользователя (DisplayName, PhoneNumber, AvatarUrl).</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileCommand cmd, CancellationToken ct)
        => (await _mediator.Send(cmd, ct)).ToActionResult();
}
