# BuyersMarket API

> Бэкенд маркетплейса байеров: заказчик публикует **тендер** на нужный товар → байеры
> откликаются **офферами** с ценой → заказчик принимает оффер → из него рождается
> **сделка** и чат между сторонами.

**.NET 8** · **PostgreSQL 16** · Clean Architecture · CQRS (MediatR) · JWT с ротацией refresh · OpenAPI

|  |  |
|---|---|
| **Запуск** | `docker compose up -d` → `dotnet ef database update` → `dotnet run` — [подробно](#запуск) |
| **Swagger UI** | `http://localhost:5088/swagger` |
| **Контракт для фронта** | [`contracts/swagger.json`](./contracts/swagger.json) — OpenAPI 3.0, лежит в репозитории |
| **ТЗ** | [`backend-spec.md`](./backend-spec.md) |

---

## Что реализовано

**11 эндпоинтов.** Схема БД готова целиком — 13 таблиц, включая домены, у которых
бизнес-логики пока нет.

| Домен | Эндпоинты | Статус |
|---|---|---|
| **Auth** | `POST /api/Auth/` → `Register` · `Login` · `Refresh` · `Logout` | ✅ готов |
| **Users** | `GET /api/Users/Me` · `GET /api/Users/{id}` · `PUT /api/Users/Me` | ✅ готов |
| **Справочники** | `GET /api/` → `Categories` · `Currencies` · `Brands` | ✅ готов |
| **Tenders** | `POST /api/Tenders` | 🟡 только создание |
| **Offers** | — | ⬜ таблицы есть, логики нет |
| **Deals** | — | ⬜ таблицы есть, логики нет |
| **Chat** | хаб SignalR смонтирован на `/hubs/chat` | ⬜ каркас |

Что умеет Auth: регистрация с ролью `Customer` или `Buyer` (байеру сразу заводится
профиль), вход, ротация пары токенов при `/Refresh` с отзывом старого, отзыв при
`/Logout`. Пароли — BCrypt, access — HS256 на 15 минут, refresh — 64 случайных байта
на 7 дней.

Что умеет создание тендера: 14 полей, ссылки на справочники категорий, валют и брендов,
до 10 картинок и до 20 произвольных атрибутов ключ-значение, доступ только роли
`Customer`, в ответе — развёрнутые объекты заказчика и справочников, чтобы фронту
не ходить за ними повторно.

---

## Архитектура

```
BuyersMarket.Domain          сущности, перечисления, Result<T>, Error — не зависит ни от чего
        ↑
BuyersMarket.Application     CQRS: команды, запросы, хэндлеры, DTO, валидаторы, интерфейсы
        ↑
BuyersMarket.Infrastructure  EF Core, миграции, JwtService, PasswordHasher
BuyersMarket.Api             контроллеры, middleware, Program.cs, Swagger
```

Зависимости идут только вверх: `Domain ← Application ← Infrastructure / Api`.

Три решения, которые определяют, как читается остальной код:

- **Ошибки — значения, а не исключения.** Доменная ошибка это `Result<T>` с типизированным
  `Error`; в HTTP-коды она разворачивается в одном месте — `ResultExtensions`. Исключения
  ловит middleware и отдаёт `ProblemDetails` с `traceId`, наружу детали уходят только
  в Development.
- **Валидация живёт в pipeline.** `ValidationBehavior` прогоняет FluentValidation до
  хэндлера и превращает провал в `Result.Failure`. Контроллеры и хэндлеры про валидацию
  не знают.
- **Репозиториев поверх EF нет.** Хэндлеры работают с `IApplicationDbContext` напрямую —
  `DbContext` уже и есть Unit of Work.

Структура репозитория:

```
├── src/
│   ├── BuyersMarket.Domain/          сущности, перечисления, Result<T>, Error
│   ├── BuyersMarket.Application/     CQRS, DTO, валидаторы, интерфейсы
│   ├── BuyersMarket.Infrastructure/  EF Core, миграции, сервисы
│   └── BuyersMarket.Api/             контроллеры, middleware, Program.cs
├── contracts/swagger.json            OpenAPI — источник типов для фронта
├── scripts/export-swagger.ps1        регенерация контракта
└── backend-spec.md                   ТЗ
```

---

## Запуск

### Что понадобится

| Инструмент | Проверить или поставить |
|---|---|
| .NET SDK 8.0+ | `dotnet --version` |
| Docker | для Postgres — либо свой PostgreSQL 14+ на `localhost:5432` |
| dotnet-ef | `dotnet tool install --global dotnet-ef --version 8.*` |

### 1. Поднять Postgres

```bash
docker compose up -d
```

Поднимет `postgres:16-alpine`: база `buyersmarket`, пользователь и пароль `postgres`,
порт `5432`, данные в volume `pgdata`. Ровно эта строка подключения уже прописана
в `appsettings.json` — для локальной разработки настраивать ничего не нужно.

Убедиться, что контейнер поднялся здоровым:

```bash
docker compose ps
```

### 2. Применить миграции

```bash
dotnet ef database update --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api
```

Создаст 13 таблиц и зальёт сиды: 4 валюты, 7 брендов, 9 категорий в трёх деревьях.

На старте приложения миграции **не** применяются — накатывать нужно явно, этой командой.

### 3. Запустить API

```bash
dotnet run --project src/BuyersMarket.Api
```

Браузер откроется сам на **http://localhost:5088/swagger** (порт задан
в `Properties/launchSettings.json`).

### 4. Проверить, что всё живо

Сценарий целиком проходится в Swagger UI:

1. **`POST /api/Auth/Register`** — роль `Customer`, телефон строго `+7XXXXXXXXXX`,
   пароль от 8 символов, минимум одна буква и одна цифра. В ответе — `accessToken`.
2. Кнопка 🔒 **Authorize** наверху страницы → вставить `accessToken`
   (само слово `Bearer` добавлять не нужно).
3. **`GET /api/Users/Me`** — вернётся твой профиль.
4. **`GET /api/Categories`** и **`GET /api/Currencies`** — скопировать по одному `id`.
5. **`POST /api/Tenders`** — создать тендер с этими `categoryId` и `currencyId`.
   Ожидаем `201` и развёрнутый объект тендера.
6. **`POST /api/Auth/Refresh`** — пара токенов обновится, старый refresh отзовётся.
7. **`POST /api/Auth/Logout`** — refresh отозван, сценарий закрыт.

### Если что-то не завелось

| Симптом | Причина |
|---|---|
| `Connection refused` от Npgsql | Postgres не поднялся: `docker compose ps`, затем `docker compose logs postgres` |
| `relation "Users" does not exist` | Не применены миграции — вернись к шагу 2 |
| `dotnet ef: command not found` | Поставь глобальный tool: `dotnet tool install --global dotnet-ef --version 8.*` |
| `401` везде, кроме Auth | Не нажат **Authorize**, либо access протух — он живёт 15 минут, сделай `/Refresh` |
| Порт `5088` занят | `dotnet run --project src/BuyersMarket.Api --urls http://localhost:5090` |

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

## Миграции EF Core

Создать новую:

```bash
dotnet ef migrations add ИмяМиграции --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api --output-dir Persistence/Migrations
```

Откатить последнюю, если ещё не применял:

```bash
dotnet ef migrations remove --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api
```

Применить:

```bash
dotnet ef database update --project src/BuyersMarket.Infrastructure --startup-project src/BuyersMarket.Api
```

Идентификаторы сид-данных зафиксированы константами в `Persistence/SeedIds.cs`. Менять
их нельзя: `HasData` сверяет строки по ключу, и плавающие Guid будут генерировать
лишние `UPDATE` в каждой следующей миграции.

---

## Контракт для фронта

[`contracts/swagger.json`](./contracts/swagger.json) — OpenAPI 3.0. Файл коммитится,
чтобы фронт собирал типы, не поднимая бэкенд у себя:

```bash
npx openapi-typescript ./contracts/swagger.json -o ./src/api/types.ts
```

Подробности — [`contracts/README.md`](./contracts/README.md).

После изменения контроллеров или DTO контракт нужно перегенерировать:

```bash
dotnet build
powershell -ExecutionPolicy Bypass -File ./scripts/export-swagger.ps1
git add contracts/swagger.json
```

---

## Конфигурация

ASP.NET Core читает `__` как разделитель уровней вложенности.

| Ключ | Назначение |
|---|---|
| `ConnectionStrings__Default` | Строка подключения к Postgres |
| `Jwt__Secret` | Секрет подписи JWT — минимум 32 случайных символа |
| `Jwt__Issuer` / `Jwt__Audience` | Issuer и audience токена |
| `Jwt__AccessTokenMinutes` | TTL access-токена, по умолчанию 15 |
| `Jwt__RefreshTokenDays` | TTL refresh-токена, по умолчанию 7 |
| `Swagger__BasicAuth__Username` / `__Password` | Доступ к `/swagger*` вне Development |
| `ASPNETCORE_ENVIRONMENT` | `Development` / `Staging` / `Production` |
| `ASPNETCORE_URLS` | Адрес слушателя |

Значения в `appsettings.json` рассчитаны на локальный Docker-Postgres и содержат
**плейсхолдерный** JWT-секрет. Для не-локальных сред секреты задаются переменными
окружения, локально удобнее user-secrets:

```bash
cd src/BuyersMarket.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=…;Database=…;Username=…;Password=…"
dotnet user-secrets set "Jwt:Secret" "сюда 48+ случайных байт в base64"
```

### Swagger вне Development

В Staging и Production `/swagger*` закрыт HTTP Basic Auth — креды берутся
из `Swagger__BasicAuth__Username|Password`. Если они не заданы, middleware
пропускает запросы без проверки, как в Development.
Реализация — [`SwaggerBasicAuthMiddleware`](./src/BuyersMarket.Api/Middleware/SwaggerBasicAuthMiddleware.cs).

---

## Безопасность

Сделано:

- BCrypt для паролей.
- Access 15 минут плюс refresh 7 дней с ротацией при `/Refresh` и отзывом при `/Logout`;
  активность refresh-токена проверяется в SQL, а не в памяти.
- Обобщённая ошибка «invalid credentials» — не раскрывает, что именно неверно.
- Гонка при регистрации одного email ловится по `UNIQUE` и отдаёт `409`, а не `500`.
- `ProblemDetails` на все ошибки; детали исключений наружу уходят только в Development,
  в остальных средах — `traceId` и запись в логе.
- `CryptographicOperations.FixedTimeEquals` при проверке Basic Auth.

Нужно сделать до боевого деплоя:

- [ ] **CORS не настроен вообще** — браузерный фронт упрётся сразу, даже локально.
- [ ] Заменить `Jwt:Secret` на случайный 48+ байт, убрать плейсхолдер из `appsettings.json`.
- [ ] HTTPS — терминация на nginx, Caddy или Cloudflare.
- [ ] Rate limiter на `/api/Auth/*` (`AddRateLimiter` из .NET 8).
- [ ] Поднять BCrypt work factor до 12.
- [ ] Бэкап БД.

---

## Дальше по плану

- [ ] **Tenders** — список с фильтрами и пагинацией, получение по id, редактирование,
      смена статуса. Сейчас тендер можно создать, но нечем показать.
- [ ] **Offers** — отклики байеров, принятие и отклонение.
- [ ] **Deals** — сделка из принятого оффера, статусы, оплата.
- [ ] **Chat** — наполнить SignalR-хаб, сообщения с привязкой к сделке.
- [ ] **Тесты** — тестового проекта пока нет; `IDateTimeService` уже заведён под мок.

---

## Лицензия

Private / Proprietary.
