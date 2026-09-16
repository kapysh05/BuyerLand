using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyersMarket.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/Tenders")]
public class TendersController : ControllerBase
{
    // TODO: следующая итерация
}
