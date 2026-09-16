using BuyersMarket.Application.Tenders.Commands;
using BuyersMarket.Application.Tenders.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

/// <summary>
/// Тендеры.
/// </summary>
[ApiController]
[Authorize]
[Route("api/Tenders")]
[Produces("application/json")]
public class TendersController : ControllerBase
{
    private readonly IMediator _mediator;
    public TendersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Создание тендера. Доступно только пользователям с ролью Customer.</summary>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(CreateTenderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateTenderCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        if (!result.IsSuccess) return result.ToActionResult();
        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
