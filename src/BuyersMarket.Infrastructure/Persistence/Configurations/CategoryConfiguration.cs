using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        b.HasIndex(x => x.ParentId);

        b.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasData(
            // Clothing
            new Category { Id = SeedIds.CategoryClothing,         Name = "Одежда" },
            new Category { Id = SeedIds.CategoryClothingTops,     Name = "Верх",     ParentId = SeedIds.CategoryClothing },
            new Category { Id = SeedIds.CategoryClothingTShirts,  Name = "Футболки", ParentId = SeedIds.CategoryClothingTops },
            new Category { Id = SeedIds.CategoryClothingHoodies,  Name = "Худи",     ParentId = SeedIds.CategoryClothingTops },
            // Electronics
            new Category { Id = SeedIds.CategoryElectronics,            Name = "Электроника" },
            new Category { Id = SeedIds.CategoryElectronicsSmartphones, Name = "Смартфоны", ParentId = SeedIds.CategoryElectronics },
            new Category { Id = SeedIds.CategoryElectronicsLaptops,     Name = "Ноутбуки",  ParentId = SeedIds.CategoryElectronics },
            // Footwear
            new Category { Id = SeedIds.CategoryFootwear,         Name = "Обувь" },
            new Category { Id = SeedIds.CategoryFootwearSneakers, Name = "Кроссовки", ParentId = SeedIds.CategoryFootwear }
        );
    }
}
