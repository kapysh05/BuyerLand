using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Offers")]
public class OffersController : ControllerBase
{
    // TODO: следующая итерация
}
