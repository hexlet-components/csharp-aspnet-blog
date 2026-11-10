# csharp-aspnet-blog

[![CI](https://github.com/hexlet-components/csharp-aspnet-blog/actions/workflows/main.yml/badge.svg)](https://github.com/hexlet-components/csharp-aspnet-blog/actions/workflows/main.yml)
[![Check](https://github.com/hexlet-components/csharp-aspnet-blog/actions/workflows/check.yml/badge.svg)](https://github.com/hexlet-components/csharp-aspnet-blog/actions/workflows/check.yml)

Демо-приложение блога на ASP.NET Core Web API: пользователи, посты, комментарии к постам, вход по JWT. Это C#-аналог Java-репозитория [java-spring-blog](https://github.com/hexlet-components/java-spring-blog). Нужно для курсов C# Хекслета. На него ссылается урок «Создание собственных типов» курса «C#: Классы»: он показывает, что классы реального приложения делятся на категории, и каждая категория здесь лежит в своей папке.

Студент открывает репозиторий по ссылке из урока и читает код, поэтому пути и имена классов в уроках считаются частью компонента. Переименование или перенос класса ломает ссылки уроков, а смена стека или структуры папок делает текст урока неверным. Перед такой правкой найди уроки, которые ссылаются на файл.

## Устройство

Весь код приложения лежит в `src/CSharpAspNetBlog/`, одна папка на категорию классов:

| Папка | Категория | Пример |
| --- | --- | --- |
| `Models/` | Сущности | `User`, `Post`, `PostComment` |
| `Dtos/` | Объекты передачи данных (`record`) | `PostDto`, `PostCreateDto` |
| `Options/` | Конфигурация | `JwtOptions` |
| `Data/` | База и начальные данные | `BlogDbContext`, `DataInitializer` |
| `Controllers/` | HTTP-контроллеры | `PostsController` |
| `Exceptions/` | Исключения приложения | `ResourceNotFoundException` |
| `Handlers/` | Перевод исключений в HTTP-ответы | `GlobalExceptionHandler` |
| `Mappers/` | Преобразование сущностей и DTO вручную | `PostMapper` |
| `Repositories/` | Доступ к данным | `PostRepository` |
| `Services/` | Логика приложения | `PostService`, `UserService` |
| `Auth/` | Выпуск токенов и текущий пользователь | `JwtTokenService`, `CurrentUserService` |
| `Specifications/` | Условия выборки | `PostCommentSpecification` |

Мапперы написаны руками, без генераторов: студент видит каждое присваивание. Тесты лежат в `tests/CSharpAspNetBlog.Tests/`: xUnit, `WebApplicationFactory`, AwesomeAssertions и Bogus для данных.

## Запуск

```sh
mise run setup
mise run dev
# http://localhost:5128
# username: hexlet@example.com
# password: qwerty

mise run test
```

`make <задача>` вызывает ту же задачу mise. При первом старте приложение создаёт SQLite-базу `blog.db` рядом с запуском и наполняет её тестовыми данными.

Вход и запросы с токеном:

```sh
TOKEN=$(curl -s -X POST http://localhost:5128/api/login \
  -H 'Content-Type: application/json' \
  -d '{"username":"hexlet@example.com","password":"qwerty"}')

curl -H "Authorization: Bearer $TOKEN" http://localhost:5128/api/posts
```

Все адреса, кроме `/api/login`, требуют токен:

- `POST /api/login`: токен строкой.
- `/api/users`: список, показ, создание, обновление.
- `/api/posts`: список, показ, создание, обновление и удаление. Менять и удалять пост может только его автор, чужой запрос получает 403.
- `/api/posts/{postId}/comments`: список страницами по 10 (`?page=2`, `?createdAtGt=...`), показ, создание, обновление и удаление. Менять и удалять комментарий может только его автор.
- `GET /api/posts_comments`: все комментарии с фильтрами `postId`, `authorId`, `createdAtGt`.
- `GET /openapi/v1.json`: описание API в формате OpenAPI, без токена.

Секрет подписи токена в `appsettings.json` учебный. Свой задают переменной `Jwt__SecretKey`.

## Проверки

`mise run check` гоняет тот же набор, что CI: разметку, опечатки, ссылки, workflows, сборку со строгими анализаторами .NET SDK, `dotnet format --verify-no-changes` и OSV-Scanner по `packages.lock.json`.

---

[![Hexlet Ltd. logo](https://raw.githubusercontent.com/Hexlet/assets/master/images/hexlet_logo128.png)](https://hexlet.io/?utm_source=github&utm_medium=link&utm_campaign=csharp-aspnet-blog)

This repository is created and maintained by the team and the community of Hexlet, an educational project. [Read more about Hexlet](https://hexlet.io/pages/about?utm_source=github&utm_medium=link&utm_campaign=csharp-aspnet-blog).
