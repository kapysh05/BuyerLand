using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

/// <summary>
/// Иерархический справочник категорий: Одежда → Верх → Футболки.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
}
