# Developer Guide — Lumina University Management System

## Архітектура

Проєкт побудований за принципом **Clean Architecture** з чітким розділенням відповідальностей між шарами:

```
UniversityManagement.Core        ← Доменний шар (моделі, інтерфейси, DTO, енуми)
UniversityManagement.Data        ← Інфраструктурний шар (EF Core, репозиторії)
UniversityManagement.Patterns    ← Патерни проєктування
UniversityManagement.Services    ← Бізнес-логіка
UniversityManagement.Web         ← Презентаційний шар (MVC, контролери, views)
UniversityManagement.Tests       ← Unit-тести
```

Залежності спрямовані лише всередину: Web → Services → Data → Core. Жоден внутрішній шар не залежить від зовнішнього.

---

## Налаштування середовища розробки

### Вимоги
- .NET 9 SDK
- Visual Studio 2022+ або VS Code з C# Dev Kit
- Git

### Перший запуск

```bash
git clone <repo-url>
cd UniversityManagement
dotnet restore
dotnet run --project UniversityManagement.Web
```

База даних SQLite створюється автоматично в `UniversityManagement.Web/university.db`. Seed-дані заповнюються при першому запуску через `SeedData.SeedAsync()`.

### Перемикання на SQL Server

У `appsettings.json`:
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=LuminaDb;Trusted_Connection=True;"
  }
}
```

---

## Структура коду

### Core — доменний шар

`Models/` — доменні сутності без залежностей від фреймворків:
- `Student`, `Teacher`, `Course`, `Group`, `Faculty`
- `Lesson`, `Grade`, `Attendance`, `Schedule`
- `Notification`, `Report`, `ApplicationUser`

`Interfaces/` — контракти для репозиторіїв та стратегій:
- `IRepository<T>` — CRUD-контракт для будь-якої сутності
- `IRatingStrategy` — контракт для стратегій рейтингу
- `IObserver`, `ISubject`, `IVisitor` — контракти патернів

`Enums/` — перерахування:
- `UserRole`: Admin, Teacher, Student
- `LessonType`: Lecture, Seminar, Laboratory
- `StudentStatus`: Active, Inactive, Graduated
- `AttendanceStatus`: Present, Absent, Late

### Data — інфраструктурний шар

`Context/UniversityDbContext.cs` — EF Core контекст, успадковує `IdentityDbContext<ApplicationUser>`.

`Repositories/` — реалізація `IRepository<T>` через EF Core.

`Configurations/` — Fluent API конфігурації зв'язків між таблицями.

`Seed/SeedData.cs` — заповнення тестовими даними при старті.

### Patterns — патерни проєктування

| Папка       | Патерн    | Призначення                                    |
|-------------|-----------|------------------------------------------------|
| `Factory/`  | Factory   | Створення занять за типом                      |
| `Strategy/` | Strategy  | Алгоритми розрахунку рейтингу                  |
| `Observer/` | Observer  | Система сповіщень                              |
| `Builder/`  | Builder   | Побудова PDF та Excel звітів                   |
| `Composite/`| Composite | Ієрархія факультет → група → студент           |
| `Visitor/`  | Visitor   | Операції над ієрархією (рейтинг, відвідуваність)|

### Services — бізнес-логіка

Кожен сервіс залежить від `IRepository<T>` через Dependency Injection:

```csharp
public class StudentService
{
    private readonly IRepository<Student> _repository;

    public StudentService(IRepository<Student> repository)
    {
        _repository = repository;
    }
}
```

Реєстрація сервісів — `Web/Extensions/ServiceExtensions.cs`.

### Web — презентаційний шар

`Controllers/` — MVC контролери. Кожен контролер захищений `[Authorize]`, окремі дії — роллю через `[Authorize(Roles = "Admin")]`.

`Middleware/ExceptionMiddleware.cs` — глобальний обробник помилок. Логує виключення та повертає стандартну відповідь.

`Hubs/NotificationHub.cs` — SignalR хаб для real-time сповіщень.

`Views/` — Razor Views, організовані за контролерами.

---

## Dependency Injection

Всі сервіси реєструються в `ServiceExtensions.cs`:

```csharp
services.AddScoped<StudentService>();
services.AddScoped<TeacherService>();
services.AddScoped<CourseService>();
services.AddScoped<RatingService>();
services.AddScoped<IRepository<Student>, StudentRepository>();
// ...
```

---

## Тестування

### Запуск тестів

```bash
cd UniversityManagement.Tests
dotnet test
```

### Структура тестів

| Файл                          | Що тестується                                    |
|-------------------------------|--------------------------------------------------|
| `StudentTests.cs`             | Модель Student — властивості                     |
| `StudentServiceTests.cs`      | StudentService — GetAll, GetById, Add, Delete    |
| `StudentsControllerTests.cs`  | StudentsController — Index, Details, Edit        |
| `FactoryTests.cs`             | LectureFactory, SeminarFactory, LaboratoryFactory|
| `AuthenticationServiceTests.cs`| Валідація паролів                               |
| `RoleAccessTests.cs`          | Рольова модель                                  |

### Підходи до тестування

**Mock репозиторія** для сервісних тестів:
```csharp
var mock = new Mock<IRepository<Student>>();
mock.Setup(x => x.GetAllAsync()).ReturnsAsync(students);
var service = new StudentService(mock.Object);
```

**InMemory Database** для контролерних тестів:
```csharp
var options = new DbContextOptionsBuilder<UniversityDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;
```

**Важливо — FluentAssertions 8.x:** для nullable reference types використовуй `Assert.NotNull()` / `Assert.IsType<T>()` замість `.Should().NotBeNull()`.

---

## Додавання нового модуля

Приклад: додати модуль `Scholarship` (стипендії).

1. **Core** — додай модель `Scholarship.cs` в `Models/`, DTO в `DTO/`
2. **Data** — додай `DbSet<Scholarship>` в `UniversityDbContext`, створи `ScholarshipRepository`
3. **Services** — створи `ScholarshipService` з `IRepository<Scholarship>`
4. **Web** — створи `ScholarshipsController` та Views
5. **Tests** — створи `ScholarshipServiceTests.cs`
6. **DI** — зареєструй сервіс у `ServiceExtensions.cs`

---

## Конфігурація

`appsettings.json` ключові параметри:

| Ключ                  | Значення за замовчуванням | Опис                              |
|-----------------------|---------------------------|-----------------------------------|
| `DatabaseProvider`    | `"Sqlite"`                | `"Sqlite"` або `"SqlServer"`      |
| `AutoCreateDatabase`  | `false`                   | `true` — авто-створення БД        |
| `SeedMockData`        | `true`                    | `false` — не заповнювати тестовими даними |