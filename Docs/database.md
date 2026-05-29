# Як працює база даних — Lumina University Management System

## Технологія

Система використовує **Entity Framework Core 9** — ORM (Object-Relational Mapper), що перетворює C#-об'єкти на SQL-таблиці та навпаки. Підтримуються два провайдери:

| Провайдер | Використання | Конфігурація |
|-----------|-------------|-------------|
| SQLite | Розробка, демо | `DatabaseProvider: "Sqlite"` |
| SQL Server | Production | `DatabaseProvider: "SqlServer"` |

Перемикання між ними — без змін у коді, лише в `appsettings.json`.

---

## UniversityDbContext — серце БД

`UniversityDbContext` успадковує від `IdentityDbContext<ApplicationUser>`, що автоматично додає всі таблиці ASP.NET Identity:

```csharp
public class UniversityDbContext : IdentityDbContext<ApplicationUser>
{
    // Доменні таблиці
    public DbSet<Student>      Students      => Set<Student>();
    public DbSet<Teacher>      Teachers      => Set<Teacher>();
    public DbSet<Course>       Courses       => Set<Course>();
    public DbSet<Faculty>      Faculties     => Set<Faculty>();
    public DbSet<Group>        Groups        => Set<Group>();
    public DbSet<Lesson>       Lessons       => Set<Lesson>();
    public DbSet<Grade>        Grades        => Set<Grade>();
    public DbSet<Attendance>   Attendances   => Set<Attendance>();
    public DbSet<Schedule>     Schedules     => Set<Schedule>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report>       Reports       => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);  // Identity таблиці
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UniversityDbContext).Assembly);  // Fluent API конфігурації
    }
}
```

---

## Повна схема таблиць

### Таблиці Identity (автоматично від IdentityDbContext)

```
AspNetUsers          — облікові записи (ApplicationUser)
AspNetRoles          — ролі (Admin, Teacher, Student)
AspNetUserRoles      — зв'язок користувач-роль
AspNetUserClaims     — claims користувача
AspNetRoleClaims     — claims ролі
AspNetUserLogins     — зовнішні провайдери входу
AspNetUserTokens     — токени (2FA, скидання паролю)
```

### Доменні таблиці

```
Faculties            — факультети
Groups               — групи студентів
Students             — студенти
Teachers             — викладачі
Courses              — навчальні дисципліни
Lessons              — конкретні заняття
Grades               — оцінки
Attendances          — відвідуваність
Schedules            — розклад
Notifications        — сповіщення
Reports              — згенеровані звіти
```

---

## Структура кожної таблиці

### Faculties
```
Id         INT  PK AUTO
Name       NVARCHAR(255)  NOT NULL
Dean       NVARCHAR(255)
```

### Groups
```
Id         INT  PK AUTO
Name       NVARCHAR(255)  NOT NULL
FacultyId  INT  FK → Faculties.Id
```

### Students
```
Id         INT  PK AUTO
FirstName  NVARCHAR(100)  NOT NULL      ← HasMaxLength(100)
LastName   NVARCHAR(100)  NOT NULL      ← HasMaxLength(100)
Email      NVARCHAR(100)  NOT NULL      ← HasMaxLength(100)
BirthDate  DATETIME
Status     INT  DEFAULT 0              ← enum: 0=Active, 1=AcademicLeave...
Rating     FLOAT  DEFAULT 0            ← HasDefaultValue(0)
GroupId    INT  FK → Groups.Id
```

### Teachers
```
Id         INT  PK AUTO
FirstName  NVARCHAR(255)
LastName   NVARCHAR(255)
Email      NVARCHAR(255)
Department NVARCHAR(255)
```

### Courses
```
Id          INT  PK AUTO
Name        NVARCHAR(100)  NOT NULL     ← HasMaxLength(100)
Credits     INT  NOT NULL
Description NVARCHAR(MAX)
TeacherId   INT  FK → Teachers.Id      ← HasOne(c => c.Teacher)
```

### Lessons
```
Id       INT  PK AUTO
Topic    NVARCHAR(255)
Type     INT                           ← enum: 0=Lecture, 1=Seminar...
Date     DATETIME
CourseId INT  FK → Courses.Id
```

### Grades
```
Id        INT  PK AUTO
Value     INT
Date      DATETIME
Comment   NVARCHAR(MAX)
StudentId INT  FK → Students.Id
CourseId  INT  FK → Courses.Id
```

### Attendances
```
Id        INT  PK AUTO
Status    INT                          ← enum: 0=Present, 1=Absent, 2=Late
Date      DATETIME
StudentId INT  FK → Students.Id
LessonId  INT  FK → Lessons.Id
```

### Schedules
```
Id        INT  PK AUTO
DayOfWeek NVARCHAR(50)
StartTime NVARCHAR(10)
EndTime   NVARCHAR(10)
GroupId   INT  FK → Groups.Id
LessonId  INT  FK → Lessons.Id
```

### Notifications
```
Id        INT  PK AUTO
Title     NVARCHAR(255)
Message   NVARCHAR(MAX)
CreatedAt DATETIME  DEFAULT NOW
IsRead    BIT  DEFAULT 0
StudentId INT  FK → Students.Id
```

### Reports
```
Id          INT  PK AUTO
Title       NVARCHAR(255)
Description NVARCHAR(MAX)
Content     NVARCHAR(MAX)
CreatedAt   DATETIME  DEFAULT NOW
GeneratedBy NVARCHAR(255)
ReportType  NVARCHAR(50)
FilePath    NVARCHAR(500)
```

---

## Зв'язки між таблицями

```
Faculty ──< Group ──< Student ──< Grade >── Course >── Teacher
                  │                    └─< Attendance >── Lesson >── Course
                  └──< Schedule >── Lesson
                  └──< Course (many-to-many через CourseGroup)

Student ──< Notification
```

### ERD (Entity-Relationship Diagram)

```
[Faculty] 1 ──── * [Group] 1 ──── * [Student]
                                        │
                          ┌─────────────┼─────────────┐
                          │             │             │
                        [Grade]  [Attendance]  [Notification]
                          │             │
                       [Course]      [Lesson]
                          │             │
                       [Teacher]     [Course]
                          │
                        [Schedule] ──── [Group]
                                  ──── [Lesson]
```

---

## Fluent API конфігурації

EF Core дозволяє налаштовувати таблиці через Fluent API в окремих класах конфігурацій:

### StudentConfiguration
```csharp
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);                        // PRIMARY KEY

        builder.Property(s => s.FirstName)
            .IsRequired()                                  // NOT NULL
            .HasMaxLength(100);                            // NVARCHAR(100)

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Rating)
            .HasDefaultValue(0);                           // DEFAULT 0
    }
}
```

### CourseConfiguration
```csharp
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Credits)
            .IsRequired();

        builder.HasOne(c => c.Teacher);                   // FK → Teachers.Id
    }
}
```

Всі конфігурації підключаються автоматично через:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(UniversityDbContext).Assembly);
```

---

## GenericRepository<T> — як EF Core виконує запити

```csharp
public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly UniversityDbContext _context;
    private readonly DbSet<T> _dbSet;

    // DbSet<T> — таблиця в БД, типізована для конкретної сутності
    public GenericRepository(UniversityDbContext context)
    {
        _context = context;
        _dbSet   = context.Set<T>();  // Students, Teachers, Courses...
    }

    // SELECT * FROM [таблиця]
    public async Task<List<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    // SELECT * FROM [таблиця] WHERE Id = @id
    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    // INSERT INTO [таблиця] VALUES (...)
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();   // транзакція фіксується тут
    }

    // UPDATE [таблиця] SET ... WHERE Id = @id
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    // DELETE FROM [таблиця] WHERE Id = @id
    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
```

---

## Розширений репозиторій з Include (навігація)

У `ServiceExtensions.cs` реалізовано розширений варіант репозиторія що автоматично підвантажує пов'язані сутності:

```csharp
private IQueryable<T> BuildQuery()
{
    var query = _context.Set<T>().AsQueryable();
    var entityType = _context.Model.FindEntityType(typeof(T));

    // Автоматично додає Include для всіх навігаційних властивостей
    foreach (var navigation in entityType.GetNavigations())
    {
        query = query.Include(navigation.Name);
    }

    return query.AsSplitQuery();  // окремий SQL для кожного Include
}
```

`AsSplitQuery()` — замість одного великого JOIN генерується кілька менших запитів. Це вирішує проблему "cartesian explosion" при множинних Include.

---

## Як EF Core перетворює LINQ на SQL

### Приклад з контролера

```csharp
// C# код
var students = await _context.Students
    .Include(s => s.Group)
    .ThenInclude(g => g!.Faculty)
    .OrderBy(s => s.LastName)
    .ThenBy(s => s.FirstName)
    .ToListAsync();
```

```sql
-- Згенерований SQL (Split Query)
SELECT s.*, g.*, f.*
FROM Students s
LEFT JOIN Groups g ON s.GroupId = g.Id
LEFT JOIN Faculties f ON g.FacultyId = f.Id
ORDER BY s.LastName, s.FirstName
```

### LINQ фільтрація

```csharp
// C#
var schedule = await _context.Schedules
    .Where(s => s.GroupId == student.GroupId)
    .Include(s => s.Lesson)
    .OrderBy(s => s.Lesson!.Date)
    .ToListAsync();
```

```sql
-- SQL
SELECT sc.*, l.*
FROM Schedules sc
LEFT JOIN Lessons l ON sc.LessonId = l.Id
WHERE sc.GroupId = @p0
ORDER BY l.Date
```

---

## Транзакції та SaveChangesAsync

EF Core накопичує зміни в пам'яті (Change Tracker) і записує їх у БД лише при `SaveChangesAsync()`:

```csharp
// Нічого не записується до SaveChangesAsync
_context.Students.Add(student);          // стан: Added (в пам'яті)
await _context.SaveChangesAsync();       // тут виконується INSERT

_context.Students.Update(student);       // стан: Modified
await _context.SaveChangesAsync();       // тут виконується UPDATE

_context.Students.Remove(student);       // стан: Deleted
await _context.SaveChangesAsync();       // тут виконується DELETE
```

Кожен виклик `SaveChangesAsync()` у GenericRepository виконується в **неявній транзакції** — або всі зміни зберігаються, або жодна.

---

## Створення БД при запуску

```csharp
// Program.cs
await dbContext.Database.EnsureCreatedAsync();
```

`EnsureCreatedAsync()` перевіряє чи існує БД і таблиці. Якщо ні — створює їх на основі конфігурацій з `OnModelCreating`. Це не міграції — таблиці створюються одразу в фінальному вигляді.

---

## Seed — початкові дані

```csharp
// SeedData.SeedAsync(dbContext)
if (!await dbContext.Faculties.AnyAsync())
{
    // Додає факультети, групи, студентів, викладачів, курси
    // лише якщо таблиці порожні
}
```

Seed-дані заповнюються один раз при першому запуску і не дублюються при наступних.

---

## Scoped Lifetime DbContext

```
HTTP Request 1 → новий DbContext (з'єднання 1)
                    операції з БД...
                 Dispose() → закрити з'єднання 1

HTTP Request 2 → новий DbContext (з'єднання 2)
                    операції з БД...
                 Dispose() → закрити з'єднання 2
```

Кожен HTTP-запит отримує власний `DbContext`. Це забезпечує:
- ізоляцію між запитами
- автоматичне закриття з'єднань
- коректну роботу Change Tracker

```csharp
// Реєстрація як Scoped
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlite(connectionString));
// Scoped — за замовчуванням для DbContext
```

---

## Перемикання SQLite ↔ SQL Server

```csharp
// Program.cs — вся логіка вибору провайдера
var databaseProvider = builder.Configuration
    .GetValue<string>("DatabaseProvider") ?? "Sqlite";

builder.Services.AddDbContext<UniversityDbContext>(options =>
{
    if (databaseProvider.Equals("Sqlite", ...))
    {
        options.UseSqlite(connectionString);  // файл university.db
        return;
    }
    options.UseSqlServer(connectionString);   // SQL Server
});
```

LINQ-запити, репозиторії та сервіси залишаються незмінними — EF Core перекладає їх у правильний SQL-діалект автоматично.

---

## Підсумок: шлях даних

```
C# об'єкт (Student)
    │
    ▼ _context.Students.Add(student)
EF Core Change Tracker
    │  стан: Added
    ▼ SaveChangesAsync()
EF Core SQL Generator
    │  INSERT INTO Students (FirstName, LastName, ...) VALUES (@p0, @p1, ...)
    ▼
SQLite / SQL Server
    │  виконує SQL, повертає Id
    ▼
EF Core оновлює student.Id
    │
    ▼
C# код продовжується з student.Id = 42
```