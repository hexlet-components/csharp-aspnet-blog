# AGENTS.md

Репозиторий создан из шаблона `hexlet-components/component-template`. Общий слой проверок взят из [dev-quality-gates](https://github.com/mokevnin/dev-quality-gates): mise, hk, commitlint, rumdl, typos, lychee, jactionlint и betterleaks. Проверки стека .NET добавлены сверху в `hk.pkl`.

## Правила для агентов

- Перед тем как считать правку законченной, запусти `mise run check` и `mise run test`: они же идут в CI.
- Стек: .NET 10, ASP.NET Core на контроллерах, EF Core и SQLite. Версии пакетов живут в `Directory.Packages.props`, свойства сборки в `Directory.Build.props`.
- Файлы `packages.lock.json` генерирует `dotnet restore`, руками их не правят. После смены версии пакета выполни `dotnet restore --force-evaluate` и закоммить лок.
- Правила анализаторов и `dotnet format` не отключаются, чтобы правка прошла. Исключение узкое (путь и правило), с причиной в `.editorconfig`, и решает его человек.
- На пути и имена классов ссылается урок «Создание собственных типов» курса «C#: Классы» в `hexlet-exercise-kit`. Перед переносом или переименованием класса найди такие ссылки и поправь урок.
- Категории классов видны по папкам `src/CSharpAspnetBlog/`. Новый класс кладётся в папку своей категории, новая папка заводится только для новой категории.
