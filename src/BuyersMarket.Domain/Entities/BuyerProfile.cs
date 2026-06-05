using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

public class BuyerProfile : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public decimal Rating { get; set; } = 0;
    public int CompletedDealsCount { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public User User { get; set; } = null!;
}
