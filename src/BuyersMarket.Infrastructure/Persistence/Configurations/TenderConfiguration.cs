using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class TenderConfiguration : IEntityTypeConfiguration<Tender>
{
    public void Configure(EntityTypeBuilder<Tender> b)
    {
        b.ToTable("Tenders");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).IsRequired().HasMaxLength(4000);
        b.Property(x => x.BudgetMin).HasPrecision(18, 2);
        b.Property(x => x.BudgetMax).HasPrecision(18, 2);
        b.Property(x => x.Quantity).IsRequired();
        b.Property(x => x.ReferenceUrl).HasMaxLength(1024);
        b.Property(x => x.PreferredCountry).HasMaxLength(64);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.CategoryId);
        b.HasIndex(x => x.CurrencyId);
        b.HasIndex(x => x.BrandId);

        b.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(x => x.TenderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Attributes)
            .WithOne(x => x.Tender)
            .HasForeignKey(x => x.TenderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Offers)
            .WithOne()
            .HasForeignKey(x => x.TenderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TenderImageConfiguration : IEntityTypeConfiguration<TenderImage>
{
    public void Configure(EntityTypeBuilder<TenderImage> b)
    {
        b.ToTable("TenderImages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Url).IsRequired().HasMaxLength(1024);
        b.HasIndex(x => x.TenderId);
    }
}

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.ToTable("Offers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Price).HasPrecision(18, 2);
        b.Property(x => x.Comment).HasMaxLength(2000);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.HasIndex(x => x.TenderId);
        b.HasIndex(x => x.BuyerId);
    }
}

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> b)
    {
        b.ToTable("Deals");
        b.HasKey(x => x.Id);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.PaymentStatus).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.HasIndex(x => x.TenderId);
        b.HasIndex(x => x.OfferId);
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.BuyerId);
    }
}

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> b)
    {
        b.ToTable("Conversations");
        b.HasKey(x => x.Id);
        b.Property(x => x.CreatedAt).IsRequired();
        b.HasIndex(x => x.DealId);
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.BuyerId);

        b.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.ToTable("Messages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).IsRequired().HasMaxLength(4000);
        b.Property(x => x.SentAt).IsRequired();
        b.HasIndex(x => x.ConversationId);
    }
}
