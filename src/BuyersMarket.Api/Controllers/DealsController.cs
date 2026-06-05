using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/deals")]
public class DealsController : ControllerBase
{
    // TODO: следующая итерация
}
