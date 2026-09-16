using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuyersMarket.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
        b.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(32);
        b.Property(x => x.DisplayName).IsRequired().HasMaxLength(64);
        b.Property(x => x.Role).IsRequired();
        b.Property(x => x.IsActive).IsRequired();
        b.Property(x => x.AvatarUrl).HasMaxLength(512);
        b.Property(x => x.CreatedAt).IsRequired();

        b.HasOne(x => x.BuyerProfile)
            .WithOne(x => x.User)
            .HasForeignKey<BuyerProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.Token).IsRequired().HasMaxLength(512);
        b.HasIndex(x => x.Token).IsUnique();
        b.Property(x => x.ExpiresAt).IsRequired();
        b.Property(x => x.CreatedAt).IsRequired();
        b.Ignore(x => x.IsActive);
    }
}

public class BuyerProfileConfiguration : IEntityTypeConfiguration<BuyerProfile>
{
    public void Configure(EntityTypeBuilder<BuyerProfile> b)
    {
        b.ToTable("BuyerProfiles");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.UserId).IsUnique();
        b.Property(x => x.Bio).HasMaxLength(2000);
        b.Property(x => x.Rating).HasPrecision(5, 2);
        b.Property(x => x.CreatedAt).IsRequired();
    }
}
