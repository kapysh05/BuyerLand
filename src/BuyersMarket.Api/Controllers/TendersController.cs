using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tenders")]
public class TendersController : ControllerBase
{
    // TODO: следующая итерация
}
