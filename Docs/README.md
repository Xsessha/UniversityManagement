# Lumina University Management System

Lumina — веб-застосунок для управління університетом, побудований на ASP.NET Core 9 з використанням MVC-архітектури, Identity для автентифікації та Entity Framework Core для роботи з базою даних.

## Технологічний стек

- **Backend:** ASP.NET Core 9, C# 13
- **ORM:** Entity Framework Core 9 (SQLite / SQL Server)
- **Автентифікація:** ASP.NET Core Identity
- **Real-time:** SignalR
- **Тестування:** xUnit, FluentAssertions, Moq
- **Патерни:** Factory, Strategy, Observer, Builder, Composite, Visitor, Repository

## Структура проєкту

```
UniversityManagement/
├── UniversityManagement.Core        # Моделі, інтерфейси, DTO, енуми
├── UniversityManagement.Data        # DbContext, репозиторії, конфігурації, seed
├── UniversityManagement.Patterns    # Реалізація патернів проєктування
├── UniversityManagement.Services    # Бізнес-логіка
├── UniversityManagement.Web         # MVC контролери, Views, Middleware
└── UniversityManagement.Tests       # Unit-тести
```

## Швидкий старт

### Вимоги
- .NET 9 SDK
- SQLite (за замовчуванням) або SQL Server

### Запуск

```bash
git clone <repo-url>
cd UniversityManagement
dotnet restore
dotnet run --project UniversityManagement.Web
```

Відкрий браузер: `https://localhost:5001`

### Тестові облікові записи

| Роль          | Email                    | Пароль       |
|---------------|--------------------------|--------------|
| Адміністратор | admin@lumina.edu         | Admin123!    |
| Викладач      | teacher@lumina.edu       | Teacher123!  |
| Студент       | student@lumina.edu       | Student123!  |

### Запуск тестів

```bash
cd UniversityManagement.Tests
dotnet test
```

## Основні можливості

- Управління студентами, викладачами, курсами, групами, факультетами
- Розклад занять
- Журнал відвідуваності
- Система оцінювання
- Генерація звітів (PDF, Excel)
- Сповіщення в реальному часі через SignalR
- Рольова модель доступу: Admin, Teacher, Student

## База даних

За замовчуванням використовується SQLite. Щоб перейти на SQL Server, зміни `appsettings.json`:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=...;Database=LuminaDb;..."
  }
}
```

База створюється автоматично при першому запуску (`EnsureCreatedAsync`). Тестові дані заповнюються через `SeedData`.
## Демонстрація 
![Результат](image\1.png)
![Результат](image\2.png)
![Результат](image\3.png)
![Результат](image\4.png)
![Результат](image\5.png)
![Результат](image\6.png)
![Результат](image\7.png)
![Результат](image\8.png)
![Результат](image\9.png)
![Результат](image\10.png)
![Результат](image\11.png)
![Результат](image\12.png)
![Результат](image\13.png)
![Результат](image\14.png)
![Результат](image\15.png)
![Результат](image\16.png)
![Результат](image\17.png)
![Результат](image\18.png)
![Результат](image\19.png)
![Результат](image\20.png)
![Результат](image\21.png)
![Результат](image\22.png)
![Результат](image\23.png)