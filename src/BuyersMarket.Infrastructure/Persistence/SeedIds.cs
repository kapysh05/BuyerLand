namespace BuyersMarket.Infrastructure.Persistence;

/// <summary>
/// Фиксированные Guid'ы для seed-данных. Обязаны быть стабильными между
/// миграциями, иначе HasData будет генерировать апдейты на каждое изменение.
/// </summary>
internal static class SeedIds
{
    // Currencies
    public static readonly Guid CurrencyKzt = new("11111111-0000-0000-0000-000000000001");
    public static readonly Guid CurrencyUsd = new("11111111-0000-0000-0000-000000000002");
    public static readonly Guid CurrencyEur = new("11111111-0000-0000-0000-000000000003");
    public static readonly Guid CurrencyRub = new("11111111-0000-0000-0000-000000000004");

    // Brands
    public static readonly Guid BrandLacoste = new("22222222-0000-0000-0000-000000000001");
    public static readonly Guid BrandCalvinKlein = new("22222222-0000-0000-0000-000000000002");
    public static readonly Guid BrandStussy = new("22222222-0000-0000-0000-000000000003");
    public static readonly Guid BrandNike = new("22222222-0000-0000-0000-000000000004");
    public static readonly Guid BrandAdidas = new("22222222-0000-0000-0000-000000000005");
    public static readonly Guid BrandApple = new("22222222-0000-0000-0000-000000000006");
    public static readonly Guid BrandSamsung = new("22222222-0000-0000-0000-000000000007");

    // Categories — Clothing tree
    public static readonly Guid CategoryClothing = new("33333333-0000-0000-0000-000000000001");
    public static readonly Guid CategoryClothingTops = new("33333333-0000-0000-0000-000000000002");
    public static readonly Guid CategoryClothingTShirts = new("33333333-0000-0000-0000-000000000003");
    public static readonly Guid CategoryClothingHoodies = new("33333333-0000-0000-0000-000000000004");

    // Categories — Electronics tree
    public static readonly Guid CategoryElectronics = new("33333333-0000-0000-0000-000000000010");
    public static readonly Guid CategoryElectronicsSmartphones = new("33333333-0000-0000-0000-000000000011");
    public static readonly Guid CategoryElectronicsLaptops = new("33333333-0000-0000-0000-000000000012");

    // Categories — Footwear tree
    public static readonly Guid CategoryFootwear = new("33333333-0000-0000-0000-000000000020");
    public static readonly Guid CategoryFootwearSneakers = new("33333333-0000-0000-0000-000000000021");
}
