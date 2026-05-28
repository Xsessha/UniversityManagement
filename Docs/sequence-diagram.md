# Sequence Діаграми — Lumina University Management System

## 1. Автентифікація користувача

```mermaid
sequenceDiagram
    actor User
    participant Browser
    participant AccountController
    participant Identity as ASP.NET Identity
    participant DB as UniversityDbContext

    User->>Browser: Вводить email та пароль
    Browser->>AccountController: POST /Account/Login
    AccountController->>Identity: SignInManager.PasswordSignInAsync()
    Identity->>DB: SELECT * FROM AspNetUsers WHERE Email = ?
    DB-->>Identity: ApplicationUser
    Identity-->>AccountController: SignInResult.Succeeded
    AccountController-->>Browser: Redirect → /Dashboard
    Browser-->>User: Головна сторінка
```

---

## 2. Створення студента адміністратором

```mermaid
sequenceDiagram
    actor Admin
    participant StudentsController
    participant Identity as UserManager
    participant StudentService
    participant DB as UniversityDbContext

    Admin->>StudentsController: POST /Students/Create (Student + password)
    StudentsController->>DB: AnyAsync(Groups, GroupId)
    DB-->>StudentsController: true
    StudentsController->>Identity: FindByEmailAsync(email)
    Identity-->>StudentsController: null (не існує)
    StudentsController->>Identity: CreateAsync(ApplicationUser, password)
    Identity-->>StudentsController: IdentityResult.Succeeded
    StudentsController->>Identity: AddToRoleAsync(user, "Student")
    StudentsController->>DB: Students.Add(student)
    StudentsController->>DB: SaveChangesAsync()
    DB-->>StudentsController: OK
    StudentsController-->>Admin: Redirect → /Students/Index
```

---

## 3. Генерація звіту (Builder Pattern)

```mermaid
sequenceDiagram
    actor Admin
    participant ReportsController
    participant ReportService
    participant PdfBuilder as PdfReportBuilder
    participant ExcelBuilder as ExcelReportBuilder
    participant DB as UniversityDbContext

    Admin->>ReportsController: POST /Reports/Generate (type="PDF")
    ReportsController->>ReportService: GenerateReport("PDF")
    ReportService->>PdfBuilder: new PdfReportBuilder()
    ReportService->>PdfBuilder: BuildTitle()
    ReportService->>PdfBuilder: BuildContent()
    ReportService->>PdfBuilder: GetReport()
    PdfBuilder-->>ReportService: Report { Title, Content }
    ReportService->>DB: Reports.Add(report)
    ReportService->>DB: SaveChangesAsync()
    DB-->>ReportService: OK
    ReportService-->>ReportsController: Report
    ReportsController-->>Admin: View(report)
```

---

## 4. Розрахунок рейтингу студента (Strategy Pattern)

```mermaid
sequenceDiagram
    actor Teacher
    participant RatingsController
    participant RatingService
    participant Strategy as IRatingStrategy
    participant DB as UniversityDbContext

    Teacher->>RatingsController: POST /Ratings/Calculate (studentId, strategy="Exam")
    RatingsController->>RatingService: SetStrategy(new ExamRatingStrategy())
    RatingsController->>RatingService: Calculate(student)
    RatingService->>Strategy: Calculate(student)
    Strategy-->>RatingService: student.Rating + 15
    RatingService-->>RatingsController: newRating
    RatingsController->>DB: UPDATE Students SET Rating = ?
    DB-->>RatingsController: OK
    RatingsController-->>Teacher: View з оновленим рейтингом
```

---

## 5. Сповіщення в реальному часі (Observer + SignalR)

```mermaid
sequenceDiagram
    actor Teacher
    participant NotificationsController
    participant TeacherSubject
    participant NotificationObserver
    participant SignalR as NotificationHub
    participant Browser as Student Browser

    Teacher->>NotificationsController: POST /Notifications/Send (message, groupId)
    NotificationsController->>TeacherSubject: Notify(message)
    TeacherSubject->>NotificationObserver: Update(message)
    NotificationObserver->>SignalR: Clients.All.SendAsync("ReceiveNotification")
    SignalR-->>Browser: Push сповіщення
    Browser-->>Teacher: Студент бачить сповіщення миттєво
```

---

## 6. Відвідуваність (Visitor Pattern)

```mermaid
sequenceDiagram
    actor Admin
    participant AttendanceController
    participant AttendanceVisitor
    participant FacultyComposite
    participant GroupComposite
    participant StudentLeaf

    Admin->>AttendanceController: GET /Attendance/Faculty (facultyId)
    AttendanceController->>AttendanceVisitor: new AttendanceVisitor()
    AttendanceController->>FacultyComposite: Accept(visitor)
    FacultyComposite->>AttendanceVisitor: Visit(faculty)
    AttendanceVisitor->>GroupComposite: Visit(group) [для кожної групи]
    GroupComposite->>AttendanceVisitor: Visit(student) [для кожного студента]
    AttendanceVisitor->>StudentLeaf: перевіряє відвідуваність
    AttendanceVisitor-->>AttendanceController: результати
    AttendanceController-->>Admin: View зі статистикою
```

---

## 7. Створення заняття (Factory Pattern)

```mermaid
sequenceDiagram
    actor Admin
    participant ScheduleController
    participant LessonFactoryProvider
    participant LectureFactory
    participant DB as UniversityDbContext

    Admin->>ScheduleController: POST /Schedule/AddLesson (type="lecture")
    ScheduleController->>LessonFactoryProvider: GetFactory("lecture")
    LessonFactoryProvider-->>ScheduleController: LectureFactory
    ScheduleController->>LectureFactory: CreateLesson()
    LectureFactory-->>ScheduleController: Lesson { Topic="Lecture", Type=Lecture }
    ScheduleController->>DB: Lessons.Add(lesson)
    ScheduleController->>DB: SaveChangesAsync()
    DB-->>ScheduleController: OK
    ScheduleController-->>Admin: Redirect → /Schedule
```