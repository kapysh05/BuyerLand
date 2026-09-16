using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Domain.Enums;

namespace BuyersMarket.Application.Tenders.DTOs;

/// <summary>Гибкий атрибут тендера (ключ-значение).</summary>
public record TenderAttributeDto(string Key, string Value);

/// <summary>Краткое представление категории.</summary>
public record CategoryDto(Guid Id, string Name, Guid? ParentId);

/// <summary>Краткое представление валюты.</summary>
public record CurrencyDto(Guid Id, string Code, string Name, string? Symbol);

/// <summary>Краткое представление бренда.</summary>
public record BrandDto(Guid Id, string Name, string? LogoUrl);

/// <summary>
/// Ответ при создании тендера. Содержит вложенные объекты заказчика, категории,
/// валюты и (опционально) бренда — фронту не нужно делать дополнительные запросы.
/// </summary>
public record CreateTenderResponseDto(
    Guid Id,
    string Title,
    string Description,
    decimal BudgetMin,
    decimal BudgetMax,
    int Quantity,
    string? ReferenceUrl,
    DateTime? DesiredByDate,
    string? PreferredCountry,
    TenderStatus Status,
    UserDto Customer,
    CategoryDto Category,
    CurrencyDto Currency,
    BrandDto? Brand,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyList<TenderAttributeDto> Attributes,
    DateTime CreatedAt
);
