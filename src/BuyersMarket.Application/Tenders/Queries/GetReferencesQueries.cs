using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Application.Tenders.DTOs;
using BuyersMarket.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Tenders.Queries;

public record GetCategoriesQuery() : IRequest<Result<IReadOnlyList<CategoryDto>>>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetCategoriesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var list = await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.ParentId))
            .ToListAsync(ct);
        return Result<IReadOnlyList<CategoryDto>>.Success(list);
    }
}

public record GetCurrenciesQuery() : IRequest<Result<IReadOnlyList<CurrencyDto>>>;

public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, Result<IReadOnlyList<CurrencyDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetCurrenciesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<CurrencyDto>>> Handle(GetCurrenciesQuery request, CancellationToken ct)
    {
        var list = await _db.Currencies
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyDto(c.Id, c.Code, c.Name, c.Symbol))
            .ToListAsync(ct);
        return Result<IReadOnlyList<CurrencyDto>>.Success(list);
    }
}

public record GetBrandsQuery() : IRequest<Result<IReadOnlyList<BrandDto>>>;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandDto>>>
{
    private readonly IApplicationDbContext _db;
    public GetBrandsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken ct)
    {
        var list = await _db.Brands
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto(b.Id, b.Name, b.LogoUrl))
            .ToListAsync(ct);
        return Result<IReadOnlyList<BrandDto>>.Success(list);
    }
}
