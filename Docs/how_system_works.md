# Як працює система — Lumina University Management System

## Запуск системи

При виконанні `dotnet run` відбувається наступна послідовність:

```
Program.cs запускається
    │
    ├─ Реєстрація DbContext (SQLite або SQL Server)
    ├─ Реєстрація ASP.NET Core Identity
    ├─ builder.Services.AddBusinessServices()
    │       → 13 сервісів + 11 репозиторіїв у DI-контейнер
    ├─ builder.Services.AddSignalR()
    │
    ├─ app.Build()
    │
    ├─ dbContext.Database.EnsureCreatedAsync()
    │       → створює таблиці якщо не існують
    ├─ SeedIdentityAsync()
    │       → створює ролі Admin/Teacher/Student
    │       → створює 3 тестових користувачі
    ├─ SeedData.SeedAsync()
    │       → заповнює факультети, групи, студентів, курси
    │
    └─ app.Run() → сервер слухає запити
```

---

## Повний шлях HTTP-запиту

```
Браузер
    │  GET/POST запит
    ▼
ExceptionMiddleware.InvokeAsync()
    │  try { await _next(context) }
    ▼
app.UseAuthentication()
    │  читає cookie/токен → встановлює User.Identity
    ▼
app.UseAuthorization()
    │  перевіряє [Authorize(Roles="...")] на контролері
    │  якщо немає прав → redirect /Account/Login
    ▼
Router (MapControllerRoute)
    │  "{controller}/{action}/{id?}"
    ▼
Controller.Action()
    │  DI вводить залежності через конструктор
    ▼
Service Layer
    │  бізнес-логіка
    ▼
IRepository<T>
    │  GenericRepository<T> → EF Core
    ▼
UniversityDbContext
    │  LINQ → SQL-запит
    ▼
База даних (SQLite / SQL Server)
    │  повертає дані
    ▼
Controller → return View(model)
    ▼
Razor View (.cshtml)
    │  рендер HTML
    ▼
HTTP Response → Браузер
```

---

## Модуль 1: Автентифікація

### Вхід у систему

```
1. Користувач вводить email + пароль → POST /Account/Login
2. AccountController → SignInManager.PasswordSignInAsync(email, password)
3. Identity перевіряє хеш пароля у таблиці AspNetUsers
4. Успіх → встановлює auth cookie → Redirect /Dashboard
5. Провал → повертає форму з помилкою "Invalid credentials"
```

### Що відбувається при кожному запиті після входу

```
Cookie надходить разом із запитом
    → UseAuthentication() розшифровує cookie
    → встановлює ClaimsPrincipal з роллю (Admin/Teacher/Student)
    → UseAuthorization() перевіряє [Authorize(Roles="...")] на action
```

### Seed-ініціалізація ролей і користувачів

```csharp
// При кожному запуску перевіряється і доповнюється
foreach (var role in Enum.GetNames<UserRole>())  // Admin, Teacher, Student
{
    if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
}
// Створення тестових користувачів якщо не існують
```

---

## Модуль 2: Dashboard

### Логіка персоналізованого Dashboard

```
DashboardController.Index()
    │
    ├─ отримує email поточного користувача з User.Identity
    ▼
DashboardService.GetDashboardDataAsync(userEmail)
    │
    ├─ UserManager.FindByEmailAsync(email) → ApplicationUser
    │
    ├─ якщо Role == Student:
    │       → знаходить Student по email
    │       → вибирає його оцінки, відвідуваність, заняття групи
    │       → повертає DashboardViewModel з особистими даними
    │
    ├─ якщо Role == Teacher:
    │       → знаходить Teacher по email
    │       → вибирає його курси, групи, заняття
    │       → повертає DashboardViewModel з даними викладача
    │
    └─ якщо Role == Admin:
            → агрегує загальну статистику по університету
            → середній рейтинг, відсоток відвідуваності, сповіщення
```

### DashboardViewModel — що містить

```
TotalStudents    — кількість студентів
TotalTeachers    — кількість викладачів
TotalCourses     — кількість курсів
TotalGroups      — кількість груп
AverageRating    — середній рейтинг
AttendanceRate   — % присутності
TodayLessons     — заняття сьогодні
RecentNotifications — останні 5 сповіщень
CurrentStudent   — об'єкт студента (для ролі Student)
CurrentTeacher   — об'єкт викладача (для ролі Teacher)
```

---

## Модуль 3: Управління студентами

### Створення студента

```
Admin → POST /Students/Create (Student + password)
    │
    ├─ Валідація: GroupId > 0, group існує в БД
    ├─ Валідація: password не порожній
    ├─ Валідація: ModelState.IsValid
    │
    ├─ UserManager.FindByEmailAsync() → перевірка унікальності email
    │
    ├─ UserManager.CreateAsync(ApplicationUser, password)
    │       → хешує пароль, зберігає в AspNetUsers
    │
    ├─ RoleManager.RoleExistsAsync("Student")
    │       → якщо немає — створює роль
    │
    ├─ UserManager.AddToRoleAsync(user, "Student")
    │       → запис у AspNetUserRoles
    │
    ├─ _context.Students.Add(student)
    ├─ _context.SaveChangesAsync()
    │       → новий рядок у таблиці Students
    │
    └─ Redirect → /Students/Index
```

### Перегляд деталей студента

```
GET /Students/Details/{id}
    │
    └─ _context.Students
        .Include(s => s.Group)
            .ThenInclude(g => g.Faculty)
        .Include(s => s.Grades)
            .ThenInclude(g => g.Course)
        .Include(s => s.Attendances)
            .ThenInclude(a => a.Lesson)
        .FirstOrDefaultAsync(s => s.Id == id)
            → один SQL JOIN-запит для всіх пов'язаних даних

    ├─ якщо student == null → return NotFound()
    │
    └─ Розклад групи:
        _context.Schedules
            .Where(s => s.GroupId == student.GroupId)
            .Include(s => s.Lesson).ThenInclude(l => l.Course)
                .ThenInclude(c => c.Teacher)
            .OrderBy(s => s.Lesson.Date)
```

---

## Модуль 4: Сервісний шар

### Як сервіс отримує залежності

```
HTTP Request → StudentsController
    │
    DI-контейнер перевіряє конструктор:
    StudentsController(StudentService, UniversityDbContext, UserManager, RoleManager)
    │
    ├─ StudentService(IRepository<Student>)
    │       → GenericRepository<Student>(UniversityDbContext)
    │               → UniversityDbContext(DbContextOptions)
    │
    ├─ UniversityDbContext — той самий Scoped екземпляр
    ├─ UserManager<ApplicationUser> — з Identity
    └─ RoleManager<IdentityRole> — з Identity
```

### Ланцюжок виклику StudentService

```
Controller.DeleteConfirmed(id)
    │
    └─ StudentService.DeleteStudentAsync(id)
            │
            ├─ IRepository<Student>.GetByIdAsync(id)
            │       → GenericRepository → _dbSet.FindAsync(id)
            │               → SELECT * FROM Students WHERE Id = id
            │
            ├─ якщо student != null:
            │       IRepository<Student>.DeleteAsync(id)
            │           → _dbSet.Remove(entity)
            │           → _context.SaveChangesAsync()
            │               → DELETE FROM Students WHERE Id = id
            │
            └─ якщо student == null: нічого не робить (guard clause)
```

---

## Модуль 5: Патерни в дії

### Factory — створення заняття

```
Admin → POST /Schedule/AddLesson (type = "lecture")
    │
    ├─ LessonFactoryProvider.GetFactory("lecture")
    │       → повертає new LectureFactory()
    │
    ├─ LectureFactory.CreateLesson()
    │       → new Lesson { Topic = "Lecture", Type = LessonType.Lecture }
    │
    └─ _context.Lessons.Add(lesson) → SaveChangesAsync()
```

### Strategy — розрахунок рейтингу

```
Admin → POST /Ratings/Calculate (studentId, strategy = "exam")
    │
    ├─ RatingService.SetStrategy(new ExamRatingStrategy())
    │
    ├─ RatingService.Calculate(student)
    │       → ExamRatingStrategy.Calculate(student)
    │               → return student.Rating + 15
    │
    └─ student.Rating = newRating → SaveChangesAsync()
```

### Observer → SignalR — сповіщення

```
Teacher → POST /Notifications/Send (message, groupId)
    │
    ├─ TeacherSubject.Notify(message)
    │       → перебирає всіх IObserver
    │
    ├─ NotificationObserver.Update(message)
    │       → NotificationHub.Clients.All
    │               → SendAsync("ReceiveNotification", message)
    │
    └─ JavaScript у браузері студента отримує push
            → відображає сповіщення без перезавантаження
```

### Builder — генерація звіту

```
Admin → POST /Reports/Generate (type = "PDF")
    │
    ├─ ReportService отримує тип
    ├─ new PdfReportBuilder()
    │
    ├─ builder.BuildTitle()   → report.Title = "PDF Report"
    ├─ builder.BuildContent() → report.Content = "PDF content"
    ├─ builder.GetReport()    → повний об'єкт Report
    │
    └─ _repository.AddAsync(report) → збереження в БД
```

### Visitor — обхід ієрархії

```
Admin → GET /Attendance/Faculty (facultyId)
    │
    ├─ new AttendanceVisitor()
    │
    ├─ visitor.Visit(faculty)
    │       → для кожної group: visitor.Visit(group)
    │               → для кожного student: visitor.Visit(student)
    │                       → перевіряє відвідуваність студента
    │
    └─ повертає агреговані результати
```

---

## Модуль 6: Відвідуваність

```
Teacher → POST /Attendance/Create
    │  { StudentId, LessonId, Status, Date }
    │
    └─ AttendanceService.MarkAttendanceAsync(attendance)
            │
            └─ IRepository<Attendance>.AddAsync(attendance)
                    → INSERT INTO Attendances VALUES (...)

Статуси: Present (0), Absent (1), Late (2)

Статистика:
    AttendanceRate = count(Present) * 100.0 / count(all)
```

---

## Модуль 7: Сповіщення

```
Таблиця Notifications:
    Id, Title, Message, CreatedAt, IsRead, StudentId

Lifecycle сповіщення:
1. Teacher надсилає → TeacherSubject.Notify()
2. Observer → SignalR push до браузера
3. Одночасно → запис у Notifications (IsRead = false)
4. Student бачить лічильник непрочитаних
5. Student відкриває → IsRead = true → оновлення в БД
```

---

## Модуль 8: Обробка помилок

```
Будь-який виняток у будь-якому контролері
    │
    └─ ExceptionMiddleware.InvokeAsync()
            │  catch (Exception ex)
            ├─ Response.StatusCode = 500
            └─ WriteAsync($"Error: {ex.Message}")

HTTP 404 (не знайдено):
    app.UseStatusCodePagesWithRedirects("/Home/StatusCode/{0}")
    → redirect /Home/StatusCode/404
    → HomeController.StatusCode(404) → View

Контролерні guard clauses:
    var student = await _context.Students.FindAsync(id);
    if (student == null) return NotFound();  // → 404
```

---

## Конфігурація системи

```
appsettings.json
    │
    ├─ DatabaseProvider: "Sqlite" або "SqlServer"
    │       → вибирає рядок підключення
    │       → вибирає EF Core провайдер
    │
    ├─ AutoCreateDatabase: true/false
    │       → true = EnsureCreatedAsync() при запуску
    │
    └─ SeedMockData: true/false
            → true = SeedData.SeedAsync() при запуску

Перемикання між БД — нульові зміни в коді:
    SQLite  → для розробки та демо
    SqlServer → для production
```

---

## Lifecycle одного HTTP-запиту (повна картина)

```
GET /Students/Details/42

1. DNS → IP → TCP з'єднання → Kestrel
2. ExceptionMiddleware: обгортає в try/catch
3. UseStaticFiles: /css, /js → обслуговує напряму (не цей шлях)
4. UseRouting: /Students/Details/42 → StudentsController.Details
5. UseSession: перевіряє сесію
6. UseAuthentication: cookie → ClaimsPrincipal (user = "ksenia@lumina.edu", role = "Student")
7. UseAuthorization: [Authorize(Roles="Admin,Teacher,Student")] → OK
8. StudentsController.Details(42):
   a. DI: вводить StudentService, UniversityDbContext, UserManager, RoleManager
   b. EF Core: SELECT Students JOIN Group JOIN Faculty JOIN Grades JOIN Attendances WHERE Id=42
   c. EF Core: SELECT Schedules WHERE GroupId=student.GroupId
   d. ViewBag.StudentSchedule = schedule
   e. return View(student)
9. Razor View: рендер HTML з моделлю student
10. HTTP 200 + HTML → Kestrel → браузер
11. Scoped DbContext: Dispose() → закриття з'єднання з БД
```

---

## Масштабування

Для переходу від SQLite до SQL Server — лише зміна конфігурації:

```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=prod-server;Database=LuminaDb;..."
  }
}
```

Для додавання нового модуля (наприклад, Scholarships):

```
1. Core/Models/Scholarship.cs          ← доменна модель
2. Data/Context: DbSet<Scholarship>    ← таблиця
3. Services/ScholarshipService.cs      ← бізнес-логіка
4. Web/Controllers/ScholarshipsController.cs ← HTTP endpoint
5. ServiceExtensions.cs: +2 рядки DI  ← реєстрація
6. Tests/ScholarshipServiceTests.cs    ← тести
```

Жодна існуюча частина системи не змінюється — Open/Closed Principle.