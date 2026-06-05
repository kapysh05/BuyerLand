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
    DbSet<Offer> Offers { get; }
    DbSet<Deal> Deals { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
