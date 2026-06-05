using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

public class Conversation : BaseEntity, IAuditable
{
    public Guid? DealId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BuyerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; }
}
