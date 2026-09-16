using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Deals")]
public class DealsController : ControllerBase
{
    // TODO: следующая итерация
}
