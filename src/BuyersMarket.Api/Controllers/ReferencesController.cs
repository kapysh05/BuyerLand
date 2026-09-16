using BuyersMarket.Application.Tenders.DTOs;
using BuyersMarket.Application.Tenders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

/// <summary>
/// Справочники: категории, валюты, бренды.
/// Доступны аутентифицированным пользователям для наполнения форм.
/// </summary>
[ApiController]
[Authorize]
[Route("api")]
[Produces("application/json")]
public class ReferencesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReferencesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Все категории (плоский список с ParentId для построения дерева на фронте).</summary>
    [HttpGet("Categories")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Categories(CancellationToken ct)
        => (await _mediator.Send(new GetCategoriesQuery(), ct)).ToActionResult();

    /// <summary>Все валюты.</summary>
    [HttpGet("Currencies")]
    [ProducesResponseType(typeof(IReadOnlyList<CurrencyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Currencies(CancellationToken ct)
        => (await _mediator.Send(new GetCurrenciesQuery(), ct)).ToActionResult();

    /// <summary>Все бренды.</summary>
    [HttpGet("Brands")]
    [ProducesResponseType(typeof(IReadOnlyList<BrandDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Brands(CancellationToken ct)
        => (await _mediator.Send(new GetBrandsQuery(), ct)).ToActionResult();
}
