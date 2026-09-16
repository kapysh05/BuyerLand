namespace BuyersMarket.Application.Tenders.DTOs;

/// <summary>Краткое представление категории.</summary>
public record CategoryDto(Guid Id, string Name, Guid? ParentId);

/// <summary>Краткое представление валюты.</summary>
public record CurrencyDto(Guid Id, string Code, string Name, string? Symbol);

/// <summary>Краткое представление бренда.</summary>
public record BrandDto(Guid Id, string Name, string? LogoUrl);
