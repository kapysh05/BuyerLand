using BuyersMarket.Application.Auth.DTOs;
using BuyersMarket.Application.Common.Interfaces;
using BuyersMarket.Application.Tenders.DTOs;
using BuyersMarket.Domain.Common;
using BuyersMarket.Domain.Entities;
using BuyersMarket.Domain.Enums;
using BuyersMarket.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuyersMarket.Application.Tenders.Commands;

/// <summary>
/// Создаёт тендер от имени текущего заказчика. CustomerId берётся из ICurrentUserService.
/// </summary>
public record CreateTenderCommand(
    string Title,
    Guid CategoryId,
    string Description,
    decimal BudgetMin,
    decimal BudgetMax,
    Guid CurrencyId,
    int Quantity,
    Guid? BrandId,
    string? ReferenceUrl,
    DateTime? DesiredByDate,
    string? PreferredCountry,
    IReadOnlyList<string>? ImageUrls,
    IReadOnlyList<TenderAttributeDto>? Attributes
) : IRequest<Result<CreateTenderResponseDto>>;

public class CreateTenderCommandValidator : AbstractValidator<CreateTenderCommand>
{
    public CreateTenderCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(5, 120);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.CurrencyId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().Length(15, 4000);
        RuleFor(x => x.BudgetMin).GreaterThan(0);
        RuleFor(x => x.BudgetMax).GreaterThanOrEqualTo(x => x.BudgetMin)
            .WithMessage("BudgetMax must be greater than or equal to BudgetMin.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);

        RuleFor(x => x.ReferenceUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrWhiteSpace(x.ReferenceUrl))
            .WithMessage("ReferenceUrl must be a valid URL.");

        RuleFor(x => x.DesiredByDate)
            .Must(d => d!.Value > DateTime.UtcNow).When(x => x.DesiredByDate.HasValue)
            .WithMessage("DesiredByDate must be in the future.");

        RuleFor(x => x.ImageUrls)
            .Must(list => list!.Count <= 10).When(x => x.ImageUrls is not null)
            .WithMessage("No more than 10 images allowed.");
        RuleForEach(x => x.ImageUrls)
            .Must(BeValidUrl).When(x => x.ImageUrls is not null)
            .WithMessage("Each image URL must be valid.");

        RuleFor(x => x.Attributes)
            .Must(list => list!.Count <= 20).When(x => x.Attributes is not null)
            .WithMessage("No more than 20 attributes allowed.");
        RuleForEach(x => x.Attributes)
            .ChildRules(a =>
            {
                a.RuleFor(x => x.Key).NotEmpty().MaximumLength(64);
                a.RuleFor(x => x.Value).NotEmpty().MaximumLength(256);
            })
            .When(x => x.Attributes is not null);
    }

    private static bool BeValidUrl(string? url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri)
           && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}

public class CreateTenderCommandHandler : IRequestHandler<CreateTenderCommand, Result<CreateTenderResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _current;
    private readonly IDateTimeService _clock;

    public CreateTenderCommandHandler(IApplicationDbContext db, ICurrentUserService current, IDateTimeService clock)
    {
        _db = db;
        _current = current;
        _clock = clock;
    }

    public async Task<Result<CreateTenderResponseDto>> Handle(CreateTenderCommand request, CancellationToken ct)
    {
        if (_current.UserId is null)
            return Result<CreateTenderResponseDto>.Failure(
                Error.Unauthorized("auth.unauthenticated", "Not authenticated."));

        if (_current.Role != UserRole.Customer)
            return Result<CreateTenderResponseDto>.Failure(
                Error.Forbidden("tenders.forbidden", "Only customers can create tenders."));

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct);
        if (category is null)
            return Result<CreateTenderResponseDto>.Failure(
                Error.NotFound("tenders.category_not_found", "Category not found."));

        var currency = await _db.Currencies.FirstOrDefaultAsync(c => c.Id == request.CurrencyId, ct);
        if (currency is null)
            return Result<CreateTenderResponseDto>.Failure(
                Error.NotFound("tenders.currency_not_found", "Currency not found."));

        Brand? brand = null;
        if (request.BrandId.HasValue)
        {
            brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == request.BrandId.Value, ct);
            if (brand is null)
                return Result<CreateTenderResponseDto>.Failure(
                    Error.NotFound("tenders.brand_not_found", "Brand not found."));
        }

        var customer = await _db.Users.FirstOrDefaultAsync(u => u.Id == _current.UserId, ct);
        if (customer is null)
            return Result<CreateTenderResponseDto>.Failure(
                Error.NotFound("users.not_found", "Current user not found."));

        var now = _clock.UtcNow;
        var tender = new Tender
        {
            CustomerId = customer.Id,
            Title = request.Title.Trim(),
            CategoryId = category.Id,
            Description = request.Description.Trim(),
            BudgetMin = request.BudgetMin,
            BudgetMax = request.BudgetMax,
            CurrencyId = currency.Id,
            Quantity = request.Quantity,
            BrandId = brand?.Id,
            ReferenceUrl = request.ReferenceUrl?.Trim(),
            DesiredByDate = request.DesiredByDate,
            PreferredCountry = request.PreferredCountry?.Trim(),
            Status = TenderStatus.Open,
            CreatedAt = now
        };

        if (request.ImageUrls is { Count: > 0 })
        {
            foreach (var url in request.ImageUrls)
                tender.Images.Add(new TenderImage { TenderId = tender.Id, Url = url.Trim() });
        }

        if (request.Attributes is { Count: > 0 })
        {
            foreach (var a in request.Attributes)
                tender.Attributes.Add(new TenderAttribute
                {
                    TenderId = tender.Id,
                    Key = a.Key.Trim(),
                    Value = a.Value.Trim()
                });
        }

        _db.Tenders.Add(tender);
        await _db.SaveChangesAsync(ct);

        var dto = new CreateTenderResponseDto(
            tender.Id,
            tender.Title,
            tender.Description,
            tender.BudgetMin,
            tender.BudgetMax,
            tender.Quantity,
            tender.ReferenceUrl,
            tender.DesiredByDate,
            tender.PreferredCountry,
            tender.Status,
            new UserDto(customer.Id, customer.Email, customer.DisplayName, customer.PhoneNumber,
                customer.Role, customer.AvatarUrl, customer.CreatedAt),
            new CategoryDto(category.Id, category.Name, category.ParentId),
            new CurrencyDto(currency.Id, currency.Code, currency.Name, currency.Symbol),
            brand is null ? null : new BrandDto(brand.Id, brand.Name, brand.LogoUrl),
            tender.Images.Select(i => i.Url).ToList(),
            tender.Attributes.Select(a => new TenderAttributeDto(a.Key, a.Value)).ToList(),
            tender.CreatedAt);

        return Result<CreateTenderResponseDto>.Success(dto);
    }
}
