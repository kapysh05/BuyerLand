using BuyersMarket.Domain.Common;

namespace BuyersMarket.Domain.Entities;

/// <summary>
/// Справочник брендов (Lacoste, Calvin Klein, Stussy ...).
/// Опционален при создании тендера.
/// </summary>
public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
