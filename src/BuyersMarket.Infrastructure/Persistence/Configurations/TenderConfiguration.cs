using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class TenderConfiguration : IEntityTypeConfiguration<Tender>
{
    public void Configure(EntityTypeBuilder<Tender> b)
    {
        b.ToTable("tenders");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).IsRequired();
        b.Property(x => x.BudgetMin).HasPrecision(18, 2);
        b.Property(x => x.BudgetMax).HasPrecision(18, 2);
        b.Property(x => x.Currency).IsRequired().HasMaxLength(8);
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.HasIndex(x => x.CustomerId);

        b.HasMany(x => x.Images)
            .WithOne()
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
        b.ToTable("tender_images");
        b.HasKey(x => x.Id);
        b.Property(x => x.Url).IsRequired().HasMaxLength(1024);
        b.HasIndex(x => x.TenderId);
    }
}

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> b)
    {
        b.ToTable("offers");
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
        b.ToTable("deals");
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
        b.ToTable("conversations");
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
        b.ToTable("messages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).IsRequired().HasMaxLength(4000);
        b.Property(x => x.SentAt).IsRequired();
        b.HasIndex(x => x.ConversationId);
    }
}
