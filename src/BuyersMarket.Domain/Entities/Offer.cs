using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Domain.Entities;

public class Offer : BaseEntity, IAuditable
{
    public Guid TenderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Price { get; set; }
    public string? Comment { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
