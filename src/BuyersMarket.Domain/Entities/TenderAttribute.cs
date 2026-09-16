using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

/// <summary>
/// Гибкие атрибуты тендера ключ-значение ("Размер": "M", "Цвет": "Белый").
/// На MVP может оставаться пустым; задел под умные формы по категориям.
/// </summary>
public class TenderAttribute : BaseEntity
{
    public Guid TenderId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public Tender Tender { get; set; } = null!;
}
