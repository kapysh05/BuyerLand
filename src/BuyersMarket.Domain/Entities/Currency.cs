using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

/// <summary>
/// Справочник валют. Код — ISO 4217 (KZT, USD, EUR, RUB).
/// </summary>
public class Currency : BaseEntity
{
    public string Code { get; set; } = string.Empty;     // "KZT"
    public string Name { get; set; } = string.Empty;     // "Тенге"
    public string? Symbol { get; set; }                  // "₸"
}
