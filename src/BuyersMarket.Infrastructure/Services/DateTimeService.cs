using BuyersMarket.Application.Common.Interfaces;

namespace BuyersMarket.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
