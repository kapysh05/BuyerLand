using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Domain.Entities;

public class Deal : BaseEntity, IAuditable
{
    public Guid TenderId { get; set; }
    public Guid OfferId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Created;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
