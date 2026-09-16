# API contracts

`swagger.json` — машиночитаемая OpenAPI 3.0 спецификация всех эндпоинтов и DTO.
Файл коммитится в репозиторий: фронт собирает типы прямо из него, без необходимости
поднимать API на своей машине.

## Регенерация (бэкенд-разработчик)

После изменения контроллеров / DTO:

```powershell
dotnet build
powershell -ExecutionPolicy Bypass -File ./scripts/export-swagger.ps1
git add contracts/swagger.json
git commit -m "chore: regenerate swagger"
```

Скрипт поднимает API на временном порту 5099, выкачивает `/swagger/v1/swagger.json`,
кладёт в `contracts/swagger.json` и гасит процесс.

## Генерация TypeScript-типов на фронте

Самый легковесный вариант — только типы, без runtime-клиента
([openapi-typescript](https://github.com/openapi-ts/openapi-typescript)):

```bash
npm i -D openapi-typescript
npx openapi-typescript ../backend/contracts/swagger.json -o ./src/api/types.ts
```

Использование:

```ts
import type { components, paths } from './api/types';

type AuthResponse = components['schemas']['AuthResponseDto'];
type RegisterBody = paths['/api/Auth/Register']['post']['requestBody']['content']['application/json'];
```

Если нужен полноценный typed-клиент (методы, не только типы) — возьми
[openapi-fetch](https://github.com/openapi-ts/openapi-typescript/tree/main/packages/openapi-fetch)
или [orval](https://orval.dev/) (RTK Query / React Query / Axios адаптеры).

## Альтернатива — фронт тянет схему с живого API

В dev-окружении фронт может ходить напрямую на `/swagger/v1/swagger.json`
запущенного бэкенда. В проде эндпоинт **закрыт Basic Auth** —
поэтому коммит файла в репо предпочтительнее.
