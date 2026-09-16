using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BuyerProfile> BuyerProfiles => Set<BuyerProfile>();
    public DbSet<Tender> Tenders => Set<Tender>();
    public DbSet<TenderImage> TenderImages => Set<TenderImage>();
    public DbSet<TenderAttribute> TenderAttributes => Set<TenderAttribute>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Brand> Brands => Set<Brand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default) => base.SaveChangesAsync(ct);
}
