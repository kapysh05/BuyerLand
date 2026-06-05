using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BuyersMarket.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    // TODO: следующая итерация
}
