using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Domain.Entities;

public class Tender : BaseEntity, IAuditable
{
    public Guid CustomerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BudgetMin { get; set; }
    public decimal BudgetMax { get; set; }
    public string Currency { get; set; } = "KZT";
    public TenderStatus Status { get; set; } = TenderStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<TenderImage> Images { get; set; } = new List<TenderImage>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}

public class TenderImage : BaseEntity
{
    public Guid TenderId { get; set; }
    public string Url { get; set; } = string.Empty;
}
