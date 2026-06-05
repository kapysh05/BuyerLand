namespace BuyersMarket.Domain.Enums;

public enum UserRole
{
    Customer = 1,
    Buyer = 2
}

public enum TenderStatus
{
    Open = 1,
    InReview = 2,
    Assigned = 3,
    Closed = 4,
    Cancelled = 5
}

public enum OfferStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Withdrawn = 4
}

public enum DealStatus
{
    Created = 1,
    InProgress = 2,
    Delivered = 3,
    Completed = 4,
    Disputed = 5,
    Cancelled = 6
}

public enum PaymentStatus
{
    NotPaid = 1,
    Pending = 2,
    Paid = 3,
    Refunded = 4
}
