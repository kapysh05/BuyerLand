using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> b)
    {
        b.ToTable("Currencies");
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).IsRequired().HasMaxLength(8);
        b.Property(x => x.Name).IsRequired().HasMaxLength(64);
        b.Property(x => x.Symbol).HasMaxLength(8);
        b.HasIndex(x => x.Code).IsUnique();

        b.HasData(
            new Currency { Id = SeedIds.CurrencyKzt, Code = "KZT", Name = "Казахстанский тенге", Symbol = "₸" },
            new Currency { Id = SeedIds.CurrencyUsd, Code = "USD", Name = "US Dollar", Symbol = "$" },
            new Currency { Id = SeedIds.CurrencyEur, Code = "EUR", Name = "Euro", Symbol = "€" },
            new Currency { Id = SeedIds.CurrencyRub, Code = "RUB", Name = "Российский рубль", Symbol = "₽" }
        );
    }
}
