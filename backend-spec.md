# ТЗ: Бэкенд маркетплейса байеров

> **Цель документа.** Технические контракты и структура backend-проекта для генерации через Claude Code.
> **Принцип работы.** Архитектура закладывается под все 4 домена сразу (Auth/Users, Tender/Offer, Deal, Chat), но **реализуется пошагово**. Эта итерация: **Auth + Users**. Остальные домены описаны на уровне сущностей и интерфейсов как «скелет» — их методы помечены `// TODO: следующая итерация` и не реализуются сейчас.
> **Платежи.** В этой версии **не реализуются**. У сущности `Deal` есть статус оплаты как поле перечисления — без какого-либо платёжного слоя, интеграций и денежной логики.

---

## 1. Технологический стек

- **.NET 8**, ASP.NET Core 8 (Web API)
- **Clean Architecture** (4 слоя: Domain, Application, Infrastructure, API/Presentation)
- **MediatR** — CQRS (Commands/Queries + Handlers)
- **PostgreSQL** + **EF Core 8** (Npgsql provider)
- **JWT** (access + refresh) для аутентификации
- **FluentValidation** — валидация команд
- **SignalR** — чат (скелет, реализация позже)
- **Serilog** — логирование
- **Swagger / OpenAPI** — документация API

---

## 2. Структура решения (Clean Architecture)

```
BuyersMarket.sln
│
├── src/
│   ├── BuyersMarket.Domain/            # Сущности, enum'ы, доменные ошибки. Зависимостей нет.
│   │   ├── Common/                     # BaseEntity, IAuditable, Result<T>
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Errors/
│   │
│   ├── BuyersMarket.Application/        # CQRS, интерфейсы, DTO, валидаторы. Зависит только от Domain.
│   │   ├── Common/
│   │   │   ├── Interfaces/              # IApplicationDbContext, IJwtService, ICurrentUserService...
│   │   │   ├── Behaviors/               # ValidationBehavior, LoggingBehavior (MediatR pipeline)
│   │   │   └── Mappings/
│   │   ├── Auth/                        # ← РЕАЛИЗУЕМ В ЭТОЙ ИТЕРАЦИИ
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── DTOs/
│   │   ├── Users/                       # ← РЕАЛИЗУЕМ В ЭТОЙ ИТЕРАЦИИ
│   │   ├── Tenders/                     # скелет
│   │   ├── Offers/                      # скелет
│   │   ├── Deals/                       # скелет
│   │   └── Chat/                        # скелет
│   │
│   ├── BuyersMarket.Infrastructure/     # EF Core, реализации интерфейсов, JWT, хэширование. Зависит от Application.
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/          # IEntityTypeConfiguration<T>
│   │   │   └── Migrations/
│   │   ├── Services/                    # JwtService, PasswordHasher, DateTimeService...
│   │   └── DependencyInjection.cs
│   │
│   └── BuyersMarket.Api/                # Контроллеры, middleware, DI, конфиг. Точка входа.
│       ├── Controllers/
│       ├── Hubs/                        # ChatHub (скелет)
│       ├── Middleware/                  # ExceptionHandlingMiddleware
│       ├── Program.cs
│       └── appsettings.json
│
└── tests/
    ├── BuyersMarket.UnitTests/
    └── BuyersMarket.IntegrationTests/
```

**Правило зависимостей:** Domain ← Application ← Infrastructure / Api. Domain ни от чего не зависит. Application зависит только от Domain. Инфраструктура и API реализуют интерфейсы из Application.

---

## 3. Доменные сущности (Domain)

Все наследуют `BaseEntity` (Guid Id) и, где нужно, `IAuditable` (CreatedAt, UpdatedAt).

### 3.1 Common

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

// Result-паттерн для возврата из Application без исключений в бизнес-логике
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }
    // фабрики Success(T value) / Failure(Error error)
}
```

### 3.2 Enums

```csharp
public enum UserRole
{
    Customer = 1,   // заказчик / услугополучатель
    Buyer    = 2    // байер / исполнитель
}

public enum TenderStatus
{
    Open      = 1,  // принимает офферы
    InReview  = 2,  // заказчик выбирает оффер
    Assigned  = 3,  // оффер принят, идёт сделка
    Closed    = 4,
    Cancelled = 5
}

public enum OfferStatus
{
    Pending   = 1,
    Accepted  = 2,
    Rejected  = 3,
    Withdrawn = 4
}

public enum DealStatus
{
    Created    = 1,
    InProgress = 2,
    Delivered  = 3,
    Completed  = 4,
    Disputed   = 5,
    Cancelled  = 6
}

// Поле-заглушка. Платёжной логики в этой версии НЕТ.
public enum PaymentStatus
{
    NotPaid   = 1,
    Pending   = 2,
    Paid      = 3,
    Refunded  = 4
}
```

### 3.3 User (реализуем)

```csharp
public class User : BaseEntity, IAuditable
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string DisplayName { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // навигация
    public BuyerProfile? BuyerProfile { get; set; }   // только если Role == Buyer
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
    public User User { get; set; }
}

// Профиль байера — расширенные данные для рейтинга/топа. В этой итерации создаётся
// пустым при регистрации с ролью Buyer; логика рейтинга — позже.
public class BuyerProfile : BaseEntity, IAuditable
{
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public decimal Rating { get; set; } = 0;          // средняя оценка
    public int CompletedDealsCount { get; set; } = 0;
    public int ReviewsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public User User { get; set; }
}
```

### 3.4 Скелет остальных доменов (НЕ реализуем методы, только сущности + конфигурации EF)

```csharp
// --- Tenders ---
public class Tender : BaseEntity, IAuditable
{
    public Guid CustomerId { get; set; }              // User с ролью Customer
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal BudgetMin { get; set; }
    public decimal BudgetMax { get; set; }
    public string Currency { get; set; } = "KZT";
    public TenderStatus Status { get; set; } = TenderStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<TenderImage> Images { get; set; } = new List<TenderImage>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}

public class TenderImage : BaseEntity
{
    public Guid TenderId { get; set; }
    public string Url { get; set; }
}

// --- Offers ---
public class Offer : BaseEntity, IAuditable
{
    public Guid TenderId { get; set; }
    public Guid BuyerId { get; set; }                 // User с ролью Buyer
    public decimal Price { get; set; }
    public string? Comment { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// --- Deals ---
public class Deal : BaseEntity, IAuditable
{
    public Guid TenderId { get; set; }
    public Guid OfferId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Created;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid; // заглушка
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// --- Chat ---
public class Conversation : BaseEntity, IAuditable
{
    public Guid? DealId { get; set; }                 // привязка к сделке (опц.)
    public Guid CustomerId { get; set; }
    public Guid BuyerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; }
}
```

> Для скелета: создать EF-конфигурации и таблицы (миграции), чтобы схема БД была целостной. **Команды/Queries/Handlers для этих доменов не пишем** — только зарегистрировать пустые папки. Это даст возможность подключать домены по одному без переделки схемы.

---

## 4. Интерфейсы Application (Common/Interfaces)

```csharp
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<BuyerProfile> BuyerProfiles { get; }
    // скелет:
    DbSet<Tender> Tenders { get; }
    DbSet<TenderImage> TenderImages { get; }
    DbSet<Offer> Offers { get; }
    DbSet<Deal> Deals { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
    Guid? ValidateAccessToken(string token);   // вернёт userId или null
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}

public interface IDateTimeService
{
    DateTime UtcNow { get; }
}
```

---

## 5. ИТЕРАЦИЯ 1 — Auth (контракты)

CQRS через MediatR. Каждая команда возвращает `Result<T>`. Валидация — FluentValidation.

### 5.1 DTOs

```csharp
public record AuthResponseDto(
    Guid UserId,
    string Email,
    string DisplayName,
    UserRole Role,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string PhoneNumber,
    UserRole Role,
    string? AvatarUrl,
    DateTime CreatedAt
);
```

### 5.2 Commands / Queries

```csharp
// Регистрация. Если Role == Buyer — создаётся пустой BuyerProfile.
public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName,
    string PhoneNumber,
    UserRole Role
) : IRequest<Result<AuthResponseDto>>;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponseDto>>;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<Result<AuthResponseDto>>;

public record LogoutCommand(
    string RefreshToken
) : IRequest<Result<bool>>;
```

### 5.3 Правила валидации (FluentValidation)

- `Email` — непустой, валидный формат, уникальный (проверка в хэндлере).
- `Password` — мин. 8 символов, хотя бы одна буква и одна цифра.
- `DisplayName` — 2–50 символов.
- `PhoneNumber` — формат `+7XXXXXXXXXX` (KZ), непустой.
- `Role` — допустимо только `Customer` или `Buyer` (не дефолт 0).

### 5.4 Логика хэндлеров (кратко)

- **RegisterHandler:** проверить уникальность email → захэшировать пароль → создать `User` → если `Buyer`, создать `BuyerProfile` → сгенерировать access+refresh → сохранить refresh → вернуть `AuthResponseDto`.
- **LoginHandler:** найти по email → проверить пароль и `IsActive` → выдать токены → вернуть DTO. Ошибки — обобщённые («неверный логин или пароль»), без указания, что именно не так.
- **RefreshTokenHandler:** найти refresh → проверить `IsActive` → отозвать старый (`RevokedAt`), выдать новый (ротация) → вернуть DTO.
- **LogoutHandler:** найти refresh → проставить `RevokedAt`.

---

## 6. ИТЕРАЦИЯ 1 — Users (контракты)

```csharp
public record GetMeQuery() : IRequest<Result<UserDto>>;            // по ICurrentUserService.UserId

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;

public record UpdateProfileCommand(
    string DisplayName,
    string PhoneNumber,
    string? AvatarUrl
) : IRequest<Result<UserDto>>;
```

- `GetMe` / `UpdateProfile` — только для аутентифицированного пользователя (берём Id из `ICurrentUserService`, не из тела запроса).
- `UpdateProfile` обновляет только свои данные; смена email и роли — вне этой итерации.

---

## 7. API-слой (контроллеры этой итерации)

Тонкие контроллеры: принимают request → шлют команду через MediatR → маппят `Result<T>` в HTTP-ответ (200/400/401/404 + проблема через `ProblemDetails`).

```
POST   /api/auth/register     → RegisterCommand        [AllowAnonymous]
POST   /api/auth/login        → LoginCommand           [AllowAnonymous]
POST   /api/auth/refresh      → RefreshTokenCommand    [AllowAnonymous]
POST   /api/auth/logout       → LogoutCommand          [Authorize]

GET    /api/users/me          → GetMeQuery             [Authorize]
GET    /api/users/{id}        → GetUserByIdQuery       [Authorize]
PUT    /api/users/me          → UpdateProfileCommand   [Authorize]
```

**Скелет (контроллеры создать пустыми с `// TODO`, без действий):** `TendersController`, `OffersController`, `DealsController`, `ChatHub`.

---

## 8. Инфраструктура и конфигурация

- `ApplicationDbContext : DbContext, IApplicationDbContext` — все DbSet'ы, применение конфигураций через `ApplyConfigurationsFromAssembly`.
- EF-конфигурации (`IEntityTypeConfiguration<T>`) для **всех** сущностей (включая скелет) — чтобы схема БД создалась целиком одной миграцией `InitialCreate`.
- `JwtService` — HS256, секрет/issuer/audience/lifetimes из `appsettings`. Access ~15 мин, refresh ~7 дней.
- `PasswordHasher` — BCrypt (пакет `BCrypt.Net-Next`).
- MediatR pipeline: `ValidationBehavior` (прогон FluentValidation), `LoggingBehavior`.
- `ExceptionHandlingMiddleware` — единый перехват, маппинг в `ProblemDetails`.
- Индексы БД: `User.Email` (unique), `RefreshToken.Token` (unique), `Offer.TenderId`, `Deal.TenderId`, `Message.ConversationId`.

### appsettings (ключи)

```json
{
  "ConnectionStrings": { "Default": "Host=localhost;Port=5432;Database=buyersmarket;Username=postgres;Password=postgres" },
  "Jwt": {
    "Secret": "<32+ символов>",
    "Issuer": "BuyersMarket",
    "Audience": "BuyersMarketClient",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  }
}
```

---

## 9. Порядок генерации для Claude Code

1. Создать solution и 4 проекта со ссылками (правило зависимостей из §2).
2. Подключить NuGet-пакеты (MediatR, EF Core + Npgsql, FluentValidation, BCrypt.Net-Next, Serilog, JWT Bearer, Swashbuckle).
3. **Domain:** все сущности, enum'ы, `BaseEntity`, `Result<T>`, `Error`.
4. **Infrastructure/Persistence:** `ApplicationDbContext` + EF-конфигурации для всех сущностей → миграция `InitialCreate`.
5. **Application/Common:** интерфейсы (§4), MediatR-behaviors.
6. **Infrastructure/Services:** `JwtService`, `PasswordHasher`, `CurrentUserService`, `DateTimeService` + `DependencyInjection`.
7. **Application/Auth:** DTO, команды, валидаторы, хэндлеры (§5).
8. **Application/Users:** запросы/команды + хэндлеры (§6).
9. **Api:** контроллеры Auth и Users, middleware, `Program.cs`, JWT-аутентификация, Swagger.
10. Контроллеры/хаб остальных доменов — пустые заглушки с `// TODO`.

**Definition of Done итерации 1:** проект собирается, миграция применяется, через Swagger проходит сценарий: register → login → refresh → get me → update profile → logout. Домены Tender/Offer/Deal/Chat присутствуют в схеме БД, но без бизнес-логики.
