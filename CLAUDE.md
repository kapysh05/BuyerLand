# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Команды

```bash
# Сборка. Держится на нуле предупреждений — не оставляй новые.
dotnet build BuyersMarket.slnx

# Локальный Postgres (строка подключения уже в appsettings.json)
docker compose up -d

# Миграции. На старте приложения НЕ применяются — только вручную.
dotnet ef database update --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api
dotnet ef migrations add ИмяМиграции --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api --output-dir Persistence/Migrations

# Запуск API → http://localhost:5088/swagger
dotnet run --project src/BuyersMarket.Api
```

`dotnet ef` — глобальный tool, в манифест не вынесен: `dotnet tool install --global dotnet-ef --version 8.*`

### Регенерация контракта — обязательна

После любой правки контроллеров, команд или DTO:

```bash
dotnet build
powershell -ExecutionPolicy Bypass -File ./scripts/export-swagger.ps1
```

Скрипт поднимает API на порту 5099, выкачивает `/swagger/v1/swagger.json` в `contracts/swagger.json` и гасит процесс. Файл коммитится: фронт генерирует из него TS-типы, не поднимая бэкенд. Забыть регенерацию — значит сломать фронту сборку типов.

### Тестов нет

Тестового проекта в решении не существует. Проверка изменений — сборка плюс ручной прогон сценария в Swagger UI (расписан в README, раздел «Запуск»). Не выдумывай команды запуска тестов и не утверждай, что тесты прошли.

## Архитектура

### Clean Architecture: слои и правило зависимостей

```
  Api              контроллеры, middleware, Program.cs, композиция DI
  Infrastructure   EF Core, миграции, JwtService, PasswordHasher, часы
        │  реализуют интерфейсы, объявленные слоем ниже
        ▼
  Application      команды, запросы, хэндлеры, DTO, валидаторы,
                   интерфейсы внешнего мира
        │
        ▼
  Domain           сущности, перечисления, Result<T>, Error, Errors
```

- **Domain** — ядро. Не ссылается ни на один проект и ни на один NuGet-пакет: в его `.csproj` нет ни единого `PackageReference`. Никакого EF, ASP.NET и атрибутов персистентности. Единственное исключение — `[NotMapped]` на `RefreshToken.IsActive`, чтобы EF не пытался мапить вычисляемое свойство.
- **Application** — сценарии использования. Знает только Domain. Здесь CQRS-слайсы, валидаторы, DTO и **интерфейсы того, что реализует Infrastructure**: `IApplicationDbContext`, `IJwtService`, `IPasswordHasher`, `ICurrentUserService`, `IDateTimeService`.
- **Infrastructure** — реализации этих интерфейсов: `ApplicationDbContext`, конфигурации EF, миграции, `JwtService`, `PasswordHasher`, `CurrentUserService`, `DateTimeService`.
- **Api** — тонкий транспорт: контроллеры, middleware, `Program.cs`, настройка Swagger и JWT.

Правило зависимостей не нарушается ни в одну сторону:

- `Domain` не ссылается ни на что.
- `Application` ссылается только на `Domain`. **Не добавляй сюда ссылку на Infrastructure.** Нужен хэндлеру внешний сервис — заведи интерфейс в `Application/Common/Interfaces/` и реализуй его в Infrastructure.
- `Infrastructure` ссылается на `Application`, потому что реализует её интерфейсы. Стрелка идёт снизу вверх — это и есть инверсия зависимостей, а не нарушение.
- `Api` ссылается на обе и не ходит в БД мимо `IApplicationDbContext`: в контроллерах нет `DbContext`, только `IMediator`.

### Куда класть новый код

| Что добавляешь | Куда |
|---|---|
| Сущность | `Domain/Entities/` + конфигурация в `Infrastructure/Persistence/Configurations/` + `DbSet` в **двух** местах + миграция |
| Сценарий, то есть эндпоинт | `Application/<Домен>/Commands` или `Queries`, одним файлом: команда, валидатор, хэндлер |
| Форма ответа | `Application/<Домен>/DTOs/<Домен>Dtos.cs` |
| Код ошибки | `Domain/Errors/Errors.cs` |
| Внешний сервис: почта, хранилище, платежи | интерфейс в `Application/Common/Interfaces/`, реализация в `Infrastructure/Services/`, регистрация в `Infrastructure/DependencyInjection.cs` |
| Сквозное поведение: аудит, транзакции, кеш | `IPipelineBehavior` в `Application/Common/Behaviors/` плюс `AddOpenBehavior` в `Application/DependencyInjection.cs` |
| HTTP-специфика: заголовки, статусы, cookie | только `Api` |

Регистрация зависимостей собрана в двух методах расширения — `AddApplication()` и `AddInfrastructure(config)`. `Program.cs` их только вызывает; не размазывай регистрации по нему.

MediatR и FluentValidation сканируют сборку Application целиком, поэтому новый хэндлер или валидатор подхватывается сам — вручную регистрировать не нужно. То же с конфигурациями EF: `ApplyConfigurationsFromAssembly`.

### Осознанные компромиссы

Это не недочёты — не «исправляй» их без запроса:

- `IApplicationDbContext` торчит наружу типами EF (`DbSet<T>`). Плата за отказ от репозиториев: `DbContext` уже Unit of Work, обёртка добавила бы слой без выгоды.
- В `Application` есть прямая ссылка на `Npgsql` — ради `PostgresException` в `DbExceptionHelpers`, чтобы отличать нарушение `UNIQUE` от прочих сбоёв. Ссылка намеренно ограничена этим случаем, остальную инфраструктуру сюда не тащи.
- Сущности Domain анемичные: публичные сеттеры, логика живёт в хэндлерах. Богатая модель проекту пока не нужна.

### Ошибки — значения, не исключения

Хэндлеры возвращают `Result<T>` с типизированным `Error` (`Domain/Errors/`). В HTTP-код ошибка разворачивается в одном месте — `Api/Controllers/ResultExtensions.cs`, по `ErrorType`. Исключения ловит `ExceptionHandlingMiddleware` и отдаёт `ProblemDetails` с `traceId`; наружу детали уходят только в Development.

Не бросай исключения для доменных ситуаций и не расширяй маппинг статусов в контроллерах.

**Каждый `IRequest` обязан возвращать `Result<T>`.** `ValidationBehavior` через рефлексию превращает провал валидации в `Result.Failure`; если `TResponse` не `Result<>`, он бросит `ValidationException` — и вместо 400 клиент получит 500.

Новые коды ошибок добавляй в реестр `Domain/Errors/Errors.cs`, а не инлайном через `Error.Conflict(...)`.

### Один слайс — один файл

`Application/<Домен>/Commands/<Имя>Command.cs` содержит сразу три типа: `record XCommand`, `XCommandValidator`, `XCommandHandler`. Так устроены все существующие команды — не разноси по отдельным файлам.

DTO живут одним файлом на домен (`Auth/DTOs/AuthDtos.cs`, `Tenders/DTOs/TenderDtos.cs`), записи с XML-комментариями на каждый параметр.

### Команды MediatR — это и есть тело запроса

Контроллеры принимают команду напрямую: `[FromBody] CreateTenderCommand cmd` и сводятся к одной строке `=> (await _mediator.Send(cmd, ct)).ToActionResult();`. Отдельных request-DTO нет.

Следствие: **любая правка полей команды меняет публичный контракт API.** Правишь команду — регенерируй swagger.

### Идентичность — только из токена

`ICurrentUserService` (UserId / Role / IsAuthenticated) — единственный источник. В командах нет и не должно быть полей вроде `CustomerId`: клиент не может назначить автора. Роль проверяется дважды — атрибутом `[Authorize(Roles = ...)]` и в хэндлере, чтобы команда оставалась безопасной при вызове в обход HTTP.

Время в хэндлерах — через `IDateTimeService`, не `DateTime.UtcNow`. Если время нужно несколько раз за хэндлер, возьми его один раз в локальную переменную.

### Доступ к БД

`IApplicationDbContext` — единственная абстракция, репозиториев нет: `DbContext` уже Unit of Work.

**Новая сущность требует двух регистраций `DbSet`:** в `Application/Common/Interfaces/IApplicationDbContext.cs` и в `Infrastructure/Persistence/ApplicationDbContext.cs`. Забыть вторую — получить ошибку только в рантайме.

Конфигурации EF подхватываются через `ApplyConfigurationsFromAssembly`, поэтому **имя файла не совпадает с сущностью**. Ищи по имени класса, а не по файлу:

- `TenderConfiguration.cs` → ещё и `TenderImage`, `Offer`, `Deal`, `Conversation`, `Message`
- `UserConfiguration.cs` → ещё и `RefreshToken`, `BuyerProfile`

Гонки по уникальным индексам обрабатывай через `DbExceptionHelpers.IsUniqueViolation` — паттерн задан в `RegisterCommand`: предпроверка плюс `catch` на `DbUpdateException`.

### Вычисляемые свойства не транслируются в SQL

`RefreshToken.IsActive` помечен `[NotMapped]` и годится только для проверок в памяти. Для запросов есть спецификация `RefreshToken.IsActiveExpr(now)`, которую EF разворачивает в SQL. Фильтрация активности в памяти после `FirstOrDefaultAsync` — уже исправленный баг, не возвращай его.

### Сиды

Guid сид-данных зафиксированы константами в `Infrastructure/Persistence/SeedIds.cs`. Менять их нельзя: `HasData` сверяет строки по ключу, и новый Guid породит лишние `UPDATE`/`DELETE` в каждой следующей миграции.

### PascalCase везде

Маршруты (`api/Auth/Register`, `api/Users/Me`) и имена таблиц (`Users`, `RefreshTokens`, `TenderImages`) — PascalCase. Приведено миграцией `RenameTablesToPascalCase`, не вводи lowercase обратно.

### XML-комментарии — часть контракта

`GenerateDocumentationFile` включён в `Api` и `Application`, `Program.cs` подгружает XML обоих проектов в Swagger. Комментарии `<summary>` на контроллерах, командах и DTO попадают в `contracts/swagger.json` и служат фронту документацией. Это не украшение — пиши их для всего публичного.

## Ветки

`feature/*` → `test` (тест) → `predeploy` (препрод) → `master` (прод).

В `master` и `predeploy` не пушить напрямую — только merge снизу. Рабочая ветка отводится от `master`. Подробности и правила — раздел «Ветки и среды» в README.

CI/CD нет, модель поддерживается руками.

## Текущее состояние

Реализовано: Auth (4 эндпоинта), Users (3), справочники категорий/валют/брендов (3), создание тендера (1). Схема БД готова целиком — 13 таблиц.

Без бизнес-логики, только таблицы и пустые заглушки: `Offers`, `Deals`, чат (`ChatHub` смонтирован на `/hubs/chat`).

Известные дыры, которые не надо «открывать заново»:

- **CORS не настроен вообще** — ни `AddCors`, ни `UseCors`. Браузерный фронт упрётся сразу.
- У тендеров есть только создание: ни списка, ни получения по id, ни фильтров.
- `JwtService.GetAccessTokenExpiry()` возвращает значение из мутабельного поля, заполненного предыдущим вызовом `GenerateAccessToken`. Работает, пока на запрос генерится один токен.
