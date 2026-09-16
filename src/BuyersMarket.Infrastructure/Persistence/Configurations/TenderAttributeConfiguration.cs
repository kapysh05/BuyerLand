using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class TenderAttributeConfiguration : IEntityTypeConfiguration<TenderAttribute>
{
    public void Configure(EntityTypeBuilder<TenderAttribute> b)
    {
        b.ToTable("TenderAttributes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Key).IsRequired().HasMaxLength(64);
        b.Property(x => x.Value).IsRequired().HasMaxLength(256);
        b.HasIndex(x => x.TenderId);
    }
}
