# BuyersMarket — бэкенд маркетплейса байеров

REST API на ASP.NET Core 8 + PostgreSQL + JWT. Clean Architecture, CQRS через MediatR.

> Полное ТЗ — [`backend-spec.md`](./backend-spec.md). Текущая итерация: **Auth + Users**.
> Остальные домены (Tender / Offer / Deal / Chat) присутствуют в схеме БД, но без бизнес-логики.

---

## Стек

- **.NET 8** + ASP.NET Core 8 Web API
- **PostgreSQL** + EF Core 8 (Npgsql)
- **MediatR** — CQRS
- **FluentValidation** — валидация команд
- **JWT** (HS256) — access + refresh с ротацией
- **BCrypt.Net-Next** — хэширование паролей
- **Serilog** — логирование
- **Swagger / OpenAPI** — документация + контракт для фронта
- **SignalR** — каркас чата (на будущее)

---

## Структура

```
BuyerLand/
├── src/
│   ├── BuyersMarket.Domain/            # Сущности, enum'ы, Result<T>, Error
│   ├── BuyersMarket.Application/       # CQRS (commands/queries/handlers), DTO, валидаторы, интерфейсы
│   ├── BuyersMarket.Infrastructure/    # EF Core (DbContext, миграции), JwtService, PasswordHasher
│   └── BuyersMarket.Api/               # Контроллеры, middleware, Program.cs, Swagger
├── contracts/
│   ├── swagger.json                    # OpenAPI 3.0 — источник типов для фронта
│   └── README.md                       # Как фронту генерировать TS-типы
├── scripts/
│   └── export-swagger.ps1              # Регенерация swagger.json
├── backend-spec.md                     # ТЗ
├── BuyersMarket.slnx                   # Solution
└── dotnet-tools.json                   # Манифест локальных tool'ов (Swashbuckle CLI)
```

Зависимости слоёв: **Domain ← Application ← Infrastructure / Api**.
Domain ни от чего не зависит. Application — только от Domain.

---

## Ветки и среды

| Ветка | Среда | Назначение |
|---|---|---|
| `master` | **прод** | Продакшн. Попадает только то, что прошло препрод. |
| `predeploy` | **препрод** | Кандидат на релиз: накапливаем задачи, прогоняем, потом пачкой в прод. |
| `test` | **тест** | Интеграция задач — первая среда после ветки разработчика. |

Поток задачи: `feature/*` → `test` → `predeploy` → `master`.

Правила:

- В `master` и `predeploy` не пушим напрямую — только merge снизу.
- Рабочая ветка отводится от `master` (актуальный прод), а не от `test`:
  иначе в задачу утекают чужие неподтверждённые изменения.
- `test` — песочница: туда можно слить несколько задач параллельно.
  В `predeploy` уезжает только то, что подтверждено на тесте.
- Миграции EF накатываются на среду до выката кода, который их требует.

CI/CD пока нет — модель поддерживается руками. Ветки стоит защитить
в GitHub (Settings → Branches): запретить force-push и прямой push
в `master` и `predeploy`.

---

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- PostgreSQL 14+ (локально или managed — Neon / Supabase / Azure / Yandex Cloud)
- [PowerShell 5.1+](https://learn.microsoft.com/powershell/) (Windows) или pwsh (Linux/macOS) — для скриптов

Опционально: Docker (для локального Postgres в контейнере).

---

## Быстрый старт

### 1. Клонирование и восстановление

```powershell
git clone <repo-url> BuyerLand
cd BuyerLand
dotnet restore
dotnet tool restore   # ставит локальный Swashbuckle.AspNetCore.Cli
```

### 2. Postgres

Любой из вариантов:

**Локально через Docker** (рекомендуется для dev):
```powershell
docker run -d --name buyersmarket-pg `
    -e POSTGRES_DB=buyersmarket `
    -e POSTGRES_USER=postgres `
    -e POSTGRES_PASSWORD=postgres `
    -p 5432:5432 `
    -v pgdata:/var/lib/postgresql/data `
    postgres:16-alpine
```

**Managed (Neon / Azure / т.п.):** получи connection string, положи в user-secrets (см. ниже).

### 3. Секреты (никогда не коммитим)

JWT secret и connection string не лежат в `appsettings.json` в проде. Локально используй **user-secrets**:

```powershell
cd src/BuyersMarket.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=buyersmarket;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:Secret" "$([Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(48)))"
cd ../..
```

В `appsettings.json` остаются плейсхолдеры — этого достаточно для прохождения сборки.

### 4. Миграция БД

```powershell
dotnet ef database update `
    --project src/BuyersMarket.Infrastructure `
    --startup-project src/BuyersMarket.Api
```

### 5. Запуск

```powershell
dotnet run --project src/BuyersMarket.Api
```

Открывай Swagger UI: `http://localhost:5088/swagger`
(порт указан в `src/BuyersMarket.Api/Properties/launchSettings.json`)

---

## DoD итерации 1 — сценарий

В Swagger UI прогоняется по порядку:

1. `POST /api/auth/register` → получаешь `accessToken` + `refreshToken`.
2. Жмёшь 🔒 **Authorize** наверху, вставляешь access (без префикса `Bearer`).
3. `POST /api/auth/login` — повторный вход.
4. `POST /api/auth/refresh` — обновление пары (старый refresh отзывается).
5. `GET /api/users/me` — твой профиль.
6. `PUT /api/users/me` — обновление DisplayName/PhoneNumber/AvatarUrl.
7. `POST /api/auth/logout` — отзыв refresh.

---

## Миграции EF Core

Создать новую миграцию:
```powershell
dotnet ef migrations add <Name> `
    --project src/BuyersMarket.Infrastructure `
    --startup-project src/BuyersMarket.Api `
    --output-dir Persistence/Migrations
```

Откатить последнюю (до применения):
```powershell
dotnet ef migrations remove `
    --project src/BuyersMarket.Infrastructure `
    --startup-project src/BuyersMarket.Api
```

Применить:
```powershell
dotnet ef database update `
    --project src/BuyersMarket.Infrastructure `
    --startup-project src/BuyersMarket.Api
```

---

## Контракт для фронта

В [`contracts/swagger.json`](./contracts/swagger.json) лежит OpenAPI 3.0 спецификация. Фронт генерирует из неё TS-типы:

```bash
npx openapi-typescript ./contracts/swagger.json -o ./src/api/types.ts
```

Подробности — [`contracts/README.md`](./contracts/README.md).

**После изменения контроллеров / DTO** обнови файл:
```powershell
dotnet build
powershell -ExecutionPolicy Bypass -File ./scripts/export-swagger.ps1
git add contracts/swagger.json
```

---

## Swagger в проде

В Production эндпоинты `/swagger*` закрыты HTTP **Basic Auth**. Креды задаются env-переменными:

```bash
export Swagger__BasicAuth__Username=admin
export Swagger__BasicAuth__Password='strong-password-here'
```

Если креды не заданы (Development по умолчанию) — Swagger открыт без авторизации.
Реализация — [`SwaggerBasicAuthMiddleware`](./src/BuyersMarket.Api/Middleware/SwaggerBasicAuthMiddleware.cs).

---

## Конфигурация (env-переменные)

ASP.NET Core читает `__` как разделитель уровней:

| Ключ | Назначение |
|---|---|
| `ConnectionStrings__Default` | Строка подключения к Postgres |
| `Jwt__Secret` | Секрет подписи JWT (≥ 32 символа случайных) |
| `Jwt__Issuer` | Issuer токена |
| `Jwt__Audience` | Audience токена |
| `Jwt__AccessTokenMinutes` | TTL access (по умолчанию 15) |
| `Jwt__RefreshTokenDays` | TTL refresh (по умолчанию 7) |
| `Swagger__BasicAuth__Username` | Логин для `/swagger*` в проде |
| `Swagger__BasicAuth__Password` | Пароль для `/swagger*` в проде |
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Staging` / `Production` |
| `ASPNETCORE_URLS` | Адрес слушателя (по умолчанию из `launchSettings.json`) |

---

## Безопасность

Уже реализовано:
- BCrypt для паролей (work factor по умолчанию 11).
- JWT access (15 мин) + refresh (7 дней) с ротацией при `/refresh` и отзывом при `/logout`.
- Обобщённая ошибка «invalid credentials» (не раскрывает, что именно неверно).
- `ProblemDetails` для всех ошибок.
- `CryptographicOperations.FixedTimeEquals` для проверки Basic Auth.

Перед prod-деплоем:
- [ ] Заменить `Jwt:Secret` на случайный 48+ байт.
- [ ] HTTPS (за nginx / Caddy / Cloudflare с TLS-терминацией).
- [ ] Rate-limiter на `/api/auth/*` (см. `AddRateLimiter` в .NET 8).
- [ ] CORS под реальный origin фронта (без `AllowAnyOrigin` + `AllowCredentials`).
- [ ] BCrypt work factor поднять до 12 (`BCrypt.HashPassword(pwd, 12)`).
- [ ] Логи Serilog — не писать в файл пароли/токены (сейчас не пишем).
- [ ] Backup БД (managed Postgres делает сам).

---

## Дальнейшие итерации (по ТЗ)

- [ ] **Tenders** — CRUD тендеров, поиск, статусы.
- [ ] **Offers** — отклики байеров, принятие/отклонение.
- [ ] **Deals** — создание сделки из принятого оффера, статусы.
- [ ] **Chat** — SignalR-хаб, сообщения с привязкой к сделке.

Бизнес-логика этих доменов пока **не реализована**, но сущности и таблицы созданы — миграция `InitialCreate` содержит всю схему.

---

## Лицензия

Private / Proprietary.
