using BuyersMarket.Application.Auth.Commands;
using BuyersMarket.Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

/// <summary>
/// Регистрация, вход, обновление и отзыв токенов.
/// </summary>
[ApiController]
[Route("api/Auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Регистрация нового пользователя (Customer или Buyer).</summary>
    /// <remarks>Если роль Buyer — параллельно создаётся пустой BuyerProfile.</remarks>
    [HttpPost("Register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand cmd, CancellationToken ct)
        => (await _mediator.Send(cmd, ct)).ToActionResult();

    /// <summary>Вход по email и паролю. Выдаёт пару access + refresh.</summary>
    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
        => (await _mediator.Send(cmd, ct)).ToActionResult();

    /// <summary>Обновление пары токенов по refresh-токену. Старый refresh отзывается (ротация).</summary>
    [HttpPost("Refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand cmd, CancellationToken ct)
        => (await _mediator.Send(cmd, ct)).ToActionResult();

    /// <summary>Отзыв текущего refresh-токена (logout).</summary>
    [HttpPost("Logout")]
    [Authorize]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand cmd, CancellationToken ct)
        => (await _mediator.Send(cmd, ct)).ToActionResult();
}
