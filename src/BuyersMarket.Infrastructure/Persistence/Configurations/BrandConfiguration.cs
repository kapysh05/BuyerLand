using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> b)
    {
        b.ToTable("Brands");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        b.Property(x => x.LogoUrl).HasMaxLength(512);
        b.HasIndex(x => x.Name).IsUnique();

        b.HasData(
            new Brand { Id = SeedIds.BrandLacoste,      Name = "Lacoste" },
            new Brand { Id = SeedIds.BrandCalvinKlein,  Name = "Calvin Klein" },
            new Brand { Id = SeedIds.BrandStussy,       Name = "Stussy" },
            new Brand { Id = SeedIds.BrandNike,         Name = "Nike" },
            new Brand { Id = SeedIds.BrandAdidas,       Name = "Adidas" },
            new Brand { Id = SeedIds.BrandApple,        Name = "Apple" },
            new Brand { Id = SeedIds.BrandSamsung,      Name = "Samsung" }
        );
    }
}
