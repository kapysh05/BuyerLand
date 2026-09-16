using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<BuyerProfile> BuyerProfiles { get; }
    DbSet<Tender> Tenders { get; }
    DbSet<TenderImage> TenderImages { get; }
    DbSet<TenderAttribute> TenderAttributes { get; }
    DbSet<Offer> Offers { get; }
    DbSet<Deal> Deals { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<Category> Categories { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<Brand> Brands { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
