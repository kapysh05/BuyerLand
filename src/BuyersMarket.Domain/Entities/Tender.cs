using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Domain.Entities;

public class Tender : BaseEntity, IAuditable
{
    public Guid CustomerId { get; set; }                  // User с ролью Customer
    public string Title { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal BudgetMin { get; set; }
    public decimal BudgetMax { get; set; }
    public Guid CurrencyId { get; set; }                  // FK на справочник валют
    public int Quantity { get; set; } = 1;
    public Guid? BrandId { get; set; }                    // опционально
    public string? ReferenceUrl { get; set; }
    public DateTime? DesiredByDate { get; set; }
    public string? PreferredCountry { get; set; }
    public TenderStatus Status { get; set; } = TenderStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // навигация
    public User Customer { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
    public Brand? Brand { get; set; }
    public ICollection<TenderImage> Images { get; set; } = new List<TenderImage>();
    public ICollection<TenderAttribute> Attributes { get; set; } = new List<TenderAttribute>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}

public class TenderImage : BaseEntity
{
    public Guid TenderId { get; set; }
    public string Url { get; set; } = string.Empty;
}
