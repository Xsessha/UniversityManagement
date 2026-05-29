# Lumina University Management System — Презентація проєкту

## Що це і навіщо

Lumina — веб-застосунок для управління університетом на ASP.NET Core 9. Замінює розрізнені Excel-таблиці та паперові журнали єдиною платформою з рольовим доступом, real-time сповіщеннями та автоматичною звітністю.

---

## Технологічний стек

| Шар | Технологія | Версія |
|-----|-----------|--------|
| Backend | ASP.NET Core MVC | 9.0 |
| Мова | C# | 13 |
| ORM | Entity Framework Core | 9.0 |
| БД (dev) | SQLite | — |
| БД (prod) | SQL Server | — |
| Автентифікація | ASP.NET Core Identity | 9.0 |
| Real-time | SignalR | 9.0 |
| Тестування | xUnit + Moq + FluentAssertions | 2.9 / 4.20 / 8.x |

---

## Архітектура — Clean Architecture

```
UniversityManagement.Core        ← Доменний шар
    Models, Interfaces, DTOs, Enums

UniversityManagement.Data        ← Інфраструктурний шар
    DbContext, Repositories, Configurations, Seed

UniversityManagement.Patterns    ← Патерни проєктування
    Factory, Strategy, Observer, Builder, Composite, Visitor

UniversityManagement.Services    ← Бізнес-логіка
    13 сервісів

UniversityManagement.Web         ← Презентаційний шар
    16 контролерів, Views, Middleware, SignalR Hub

UniversityManagement.Tests       ← Тести
    26 unit-тестів
```

Залежності спрямовані строго всередину: Web → Services → Data → Core. Жоден внутрішній шар не знає про зовнішній.

---

## ООП — Конструктори та деструктори

### Конструктори

У C# конструктор ініціалізує об'єкт при його створенні. У проєкті кожен сервіс отримує залежності через **ін'єкцію конструктора** — це головний механізм Dependency Injection:

```csharp
// Конструктор з одною залежністю
public class StudentService
{
    private readonly IRepository<Student> _repository;

    public StudentService(IRepository<Student> repository)
    {
        _repository = repository;  // залежність вводиться ззовні
    }
}

// Конструктор з багатьма залежностями (DashboardService)
public class DashboardService
{
    private readonly IRepository<Student>  _studentRepository;
    private readonly IRepository<Teacher>  _teacherRepository;
    private readonly IRepository<Course>   _courseRepository;
    private readonly UniversityDbContext   _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(
        IRepository<Student> studentRepository,
        IRepository<Teacher> teacherRepository,
        IRepository<Course>  courseRepository,
        UniversityDbContext  context,
        UserManager<ApplicationUser> userManager)
    {
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _courseRepository  = courseRepository;
        _context           = context;
        _userManager       = userManager;
    }
}
```

Ініціалізатори властивостей у моделях — спрощена форма конструктора:

```csharp
public class Student
{
    public string FirstName { get; set; } = string.Empty;  // значення за замовч.
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public List<Grade> Grades { get; set; } = new();       // порожня колекція
}
```

### Деструктори та IDisposable

У .NET деструктор (`~ClassName()`) викликається збирачем сміття. Для детермінованого звільнення ресурсів використовується патерн `IDisposable` + `using`. У проєкті `UniversityDbContext` успадковує від `DbContext`, який реалізує `IDisposable` — EF Core автоматично закриває з'єднання з БД при виході зі scope:

```csharp
// DI-контейнер реєструє DbContext як Scoped —
// автоматично Dispose() наприкінці кожного HTTP-запиту
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlite(connectionString));

// У тестах — явне using для детермінованого Dispose
using var context = new UniversityDbContext(options);
context.Students.Add(student);
await context.SaveChangesAsync();
// context.Dispose() викликається автоматично наприкінці блоку
```

---

## ООП — Наслідування

### Ланцюжок наслідування Identity

```
IdentityUser  (ASP.NET Core Identity)
    └── ApplicationUser  (наш клас)
            + FirstName : string
            + LastName  : string
            + Role      : UserRole
```

```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName  { get; set; } = string.Empty;
    public UserRole Role    { get; set; }
}
```

Завдяки наслідуванню `ApplicationUser` отримує всю функціональність Identity (хешування паролів, токени, ролі) і додає доменні властивості.

### Наслідування репозиторіїв

```
GenericRepository<T>  (базова реалізація)
    ├── StudentRepository  (може перевизначати методи)
    ├── TeacherRepository
    └── CourseRepository
```

```csharp
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly UniversityDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(UniversityDbContext context)
    {
        _context = context;
        _dbSet   = context.Set<T>();
    }

    public virtual async Task<List<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);
}
```

### Наслідування Builder

```
ReportBuilder  (абстрактний)
    ├── PdfReportBuilder
    └── ExcelReportBuilder
```

```csharp
public abstract class ReportBuilder
{
    protected Report report = new();
    public abstract void BuildTitle();
    public abstract void BuildContent();
    public Report GetReport() => report;
}

public class PdfReportBuilder : ReportBuilder
{
    public override void BuildTitle()   => report.Title   = "PDF Report";
    public override void BuildContent() => report.Content = "PDF content";
}
```

---

## ООП — Поліморфізм

Поліморфізм — здатність використовувати об'єкти різних класів через посилання на базовий тип.

### Поліморфізм через інтерфейси

```csharp
// Один інтерфейс — різні реалізації
IRatingStrategy strategy;

strategy = new ExamRatingStrategy();
double r1 = strategy.Calculate(student);   // student.Rating + 15

strategy = new AttendanceRatingStrategy();
double r2 = strategy.Calculate(student);   // student.Rating * 0.9

strategy = new ScholarshipRatingStrategy();
double r3 = strategy.Calculate(student);   // student.Rating * 1.2
// Виклик однаковий — результат різний залежно від типу
```

### Поліморфізм через Visitor

```csharp
IVisitor visitor;

visitor = new RatingVisitor();
visitor.Visit(student);      // student.Rating += 10

visitor = new AttendanceVisitor();
visitor.Visit(student);      // перевірка відвідуваності

visitor = new StatisticsVisitor();
visitor.Visit(student);      // вивід статистики
```

### Поліморфізм IRepository<T>

```csharp
IRepository<Student> repo;

// В production — реальна БД
repo = new GenericRepository<Student>(dbContext);

// У тестах — in-memory fake
repo = new FakeStudentRepository();

// Сервіс не знає різниці — поліморфізм
var students = await repo.GetAllAsync();
```

---

## Інтерфейси

Інтерфейс визначає контракт без реалізації. У проєкті 6 ключових інтерфейсів:

```csharp
// 1. Доступ до даних — узагальнений CRUD
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// 2. Бізнес-логіка сервісів
public interface IService<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// 3. Стратегія рейтингу
public interface IRatingStrategy
{
    double Calculate(Student student);
}

// 4. Підписник Observer
public interface IObserver
{
    void Update(string message);
}

// 5. Visitor — операції над ієрархією
public interface IVisitor
{
    void Visit(Student student);
    void Visit(Teacher teacher);
    void Visit(Course course);
    void Visit(Group group);
    void Visit(Faculty faculty);
}

// 6. Composite — компонент ієрархії
public interface IUniversityComponent
{
    string Name { get; }
    void Display(int depth = 0);
}
```

---

## Абстрактні класи

Абстрактний клас — база з частковою реалізацією, не може бути створений напряму:

```csharp
public abstract class ReportBuilder
{
    protected Report report = new();       // спільний стан

    public abstract void BuildTitle();     // обов'язково перевизначити
    public abstract void BuildContent();   // обов'язково перевизначити

    public Report GetReport() => report;   // готова реалізація — не перевизначається
}
```

Різниця між інтерфейсом і абстрактним класом у проєкті:

| | Інтерфейс | Абстрактний клас |
|---|-----------|-----------------|
| Стан | Немає | Є (`protected Report report`) |
| Реалізація методів | Немає | Часткова (`GetReport()`) |
| Множинне успадкування | Так | Ні |
| Приклад у проєкті | `IRepository<T>` | `ReportBuilder` |

---

## Перерахування (Enums)

```csharp
// Роль користувача
public enum UserRole
{
    Admin,    // повний доступ
    Teacher,  // управління курсами, оцінками
    Student   // перегляд власних даних
}

// Тип заняття
public enum LessonType
{
    Lecture, Seminar, Laboratory,
    Practice, Exam, Consultation, Workshop
}

// Статус студента
public enum StudentStatus
{
    Active, AcademicLeave, Suspended, Graduated
}

// Статус відвідуваності
public enum AttendanceStatus
{
    Present, Absent, Late
}
```

Перерахування використовуються в атрибутах авторизації:

```csharp
[Authorize(Roles = "Admin,Teacher")]
public async Task<IActionResult> Index() { ... }

[Authorize(Roles = "Admin")]
public IActionResult Create() { ... }
```

---

## SOLID — 5 принципів

### S — Single Responsibility

Кожен клас має одну відповідальність:

```
StudentService     → тільки логіка студентів
AttendanceService  → тільки відвідуваність
RatingService      → тільки розрахунок рейтингу
ReportService      → тільки звіти
StatisticsService  → тільки статистика
```

### O — Open/Closed

Відкритий для розширення, закритий для модифікації:

```csharp
// Новий тип заняття = новий клас, без зміни LessonFactoryProvider
public class WebinarFactory : ILessonFactory
{
    public Lesson CreateLesson() =>
        new Lesson { Topic = "Webinar", Type = LessonType.Lecture };
}
// Додаємо лише один рядок у switch:
// "webinar" => new WebinarFactory()
```

### L — Liskov Substitution

`GenericRepository<Student>` може бути підставлений скрізь, де очікується `IRepository<Student>`, без порушення коректності. `FakeStudentRepository` у тестах — теж коректна заміна.

### I — Interface Segregation

Жоден клас не реалізує зайвих методів. `IRatingStrategy` містить лише `Calculate()`. `IObserver` — лише `Update()`. Якби всі методи були в одному інтерфейсі — порушення ISP.

### D — Dependency Inversion

```csharp


//  Дотримання DIP — залежність від абстракції
public class StudentService
{
    private readonly IRepository<Student> _repository;

    public StudentService(IRepository<Student> repository)
    {
        _repository = repository;  // конкретика вводиться ззовні
    }
}
```

---

## Патерни проєктування — 7 патернів GoF

### 1. Factory Method (Породжувальний)

**Проблема:** Три типи занять — Lecture, Seminar, Laboratory. Логіка створення не повинна бути розкидана по контролерах.

```csharp
public interface ILessonFactory
{
    Lesson CreateLesson();
}

public class LectureFactory : ILessonFactory
{
    public Lesson CreateLesson() =>
        new Lesson { Topic = "Lecture", Type = LessonType.Lecture };
}

// Провайдер вибирає фабрику за рядком
public static ILessonFactory GetFactory(string type) => type switch
{
    "lecture" => new LectureFactory(),
    "seminar"  => new SeminarFactory(),
    "lab"      => new LaboratoryFactory(),
    _          => new LectureFactory()
};
```

**Перевага:** Додавання нового типу = 1 новий клас + 1 рядок у switch.

---

### 2. Strategy (Поведінковий)

**Проблема:** Різні алгоритми рейтингу — за іспитом, відвідуваністю, стипендією.

```csharp
public class RatingService
{
    private IRatingStrategy _strategy = null!;

    public void SetStrategy(IRatingStrategy strategy) => _strategy = strategy;
    public double Calculate(Student student) => _strategy.Calculate(student);
}

// Exam: +15 до рейтингу
// Attendance: ×0.9
// Scholarship: ×1.2
```

**Перевага:** Алгоритм змінюється в runtime без зміни сервісу.

---

### 3. Observer (Поведінковий)

**Проблема:** Викладач надсилає повідомлення — студенти мають отримати його миттєво без жорсткої залежності.

```csharp
public class TeacherSubject
{
    private List<IObserver> observers = new();
    public void Attach(IObserver o) => observers.Add(o);
    public void Notify(string msg)  => observers.ForEach(o => o.Update(msg));
}

// NotificationObserver → SignalR Hub → браузер студента
```

---

### 4. Builder (Породжувальний)

**Проблема:** Звіти PDF і Excel — однаковий процес побудови, різні деталі.

```csharp
// Director
var builder = new PdfReportBuilder();
builder.BuildTitle();
builder.BuildContent();
Report report = builder.GetReport();
// report.Title = "PDF Report"
```

---

### 5. Composite (Структурний)

**Проблема:** Факультет → Групи → Студенти — деревоподібна ієрархія, операції мають застосовуватись до будь-якого рівня.

```
FacultyComposite "Комп'ютерних наук"
    GroupComposite "КН-21"
        StudentLeaf "Ksenia Olkhovska"
        StudentLeaf "Ivan Ivanov"
    GroupComposite "КН-22"
        StudentLeaf "..."
```

---

### 6. Visitor (Поведінковий)

**Проблема:** Різні операції над ієрархією — рейтинг, відвідуваність, статистика, звіт — без зміни доменних класів.

```csharp
// Одна ієрархія — чотири різних операції
new RatingVisitor().Visit(faculty);      // оновлює рейтинги
new AttendanceVisitor().Visit(faculty);  // перевіряє відвідуваність
new StatisticsVisitor().Visit(faculty);  // виводить статистику
new ReportVisitor().Visit(faculty);      // генерує звіт
```

---

### 7. Repository (Структурний)

**Проблема:** Сервіси не повинні залежати від EF Core напряму — це унеможливлює тестування.

```csharp
public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly UniversityDbContext _context;
    private readonly DbSet<T> _dbSet;

    // В тестах підміняється FakeRepository або Mock
    // У production — реальний EF Core + SQLite/SQL Server
}
```

---

## Generics та LINQ

### Generic Repository — один клас для 11 сутностей

```csharp
// Замість 11 окремих репозиторіїв — один параметричний
services.AddScoped<IRepository<Student>,  GenericRepository<Student>>();
services.AddScoped<IRepository<Teacher>,  GenericRepository<Teacher>>();
services.AddScoped<IRepository<Course>,   GenericRepository<Course>>();
// ... і ще 8
```

### LINQ у системі

```csharp
// Фільтрація + Include (JOIN)
var students = await _context.Students
    .Include(s => s.Group).ThenInclude(g => g!.Faculty)
    .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
    .ToListAsync();

// Агрегація
double avg = students.Any() ? students.Average(s => s.Rating) : 0;

// Групування
var byGroup = students.GroupBy(s => s.GroupId)
    .Select(g => new { GroupId = g.Key, Count = g.Count() });

// Проекція
var topStudents = students
    .OrderByDescending(s => s.Rating)
    .Take(10).ToList();
```

---

## Dependency Injection

Весь DI налаштований в `ServiceExtensions.cs` і реєструється одним рядком:

```csharp
// Program.cs
builder.Services.AddBusinessServices();

// ServiceExtensions.cs — метод-розширення
public static IServiceCollection AddBusinessServices(
    this IServiceCollection services)
{
    // 13 сервісів
    services.AddScoped<StudentService>();
    services.AddScoped<TeacherService>();
    services.AddScoped<RatingService>();
    // ...

    // 11 репозиторіїв
    services.AddScoped<IRepository<Student>, GenericRepository<Student>>();
    services.AddScoped<IRepository<Teacher>, GenericRepository<Teacher>>();
    // ...

    return services;
}
```

**Scoped lifetime** — новий екземпляр на кожен HTTP-запит, автоматичний `Dispose()` наприкінці.

---

## Автентифікація та авторизація

```csharp
// ASP.NET Core Identity — хешування паролів, токени
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<UniversityDbContext>();

// Рольова авторизація на рівні дій
[Authorize(Roles = "Admin")]
public IActionResult Create() { ... }

[Authorize(Roles = "Admin,Teacher")]
public async Task<IActionResult> Index() { ... }

[Authorize(Roles = "Admin,Teacher,Student")]
public async Task<IActionResult> Details(int id) { ... }
```

**Тестові облікові записи** (seed при запуску):

| Email | Пароль | Роль |
|-------|--------|------|
| admin@lumina.edu | Admin123! | Admin |
| teacher@lumina.edu | Teacher123! | Teacher |
| student@lumina.edu | Student123! | Student |

---

## SignalR — Real-time сповіщення

```csharp
// Hub
public class NotificationHub : Hub { }

// Реєстрація
app.MapHub<NotificationHub>("/notificationHub");

// Observer надсилає через hub
public class NotificationObserver : IObserver
{
    public void Update(string message)
    {
        // Clients.All.SendAsync("ReceiveNotification", message)
    }
}
```

Студент отримує сповіщення від викладача **без перезавантаження сторінки**.

---

## Обробка помилок

```csharp
// ExceptionMiddleware — перехоплює будь-який виняток
public async Task InvokeAsync(HttpContext context)
{
    try
    {
        await _next(context);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync($"Error: {ex.Message}");
    }
}

// 404 → /Home/StatusCode/404
app.UseStatusCodePagesWithRedirects("/Home/StatusCode/{0}");
```

---

## Unit-тести — 25 тестів, 100% проходження



| Файл | Тести | Що перевіряється |
|------|-------|-----------------|
| StudentTests | 1 | Модель Student |
| StudentServiceTests | 7 | GetAll, GetById, Add, Delete |
| StudentsControllerTests | 4 | Index, Details, Edit |
| FactoryTests | 5 | Всі 3 фабрики + Provider |
| AuthenticationServiceTests | 5 | Валідація паролів (Theory) |
| RoleAccessTests | 1 | Рольова модель |

```csharp
// Приклад: Mock ізолює від бази даних
var mock = new Mock<IRepository<Student>>();
mock.Setup(x => x.GetAllAsync()).ReturnsAsync(students);
var service = new StudentService(mock.Object);

// Перевірка поведінки
mock.Verify(x => x.AddAsync(student), Times.Once);
mock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
```

