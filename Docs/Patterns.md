# Патерни проєктування — Lumina University Management System

## Огляд

У проєкті застосовано 7 патернів проєктування з каталогу GoF (Gang of Four). Кожен обраний з конкретної причини — для вирішення реальної проблеми в доменній логіці університету, а не для демонстрації патернів заради патернів.


## 1. Factory Method — Створення занять

### Проблема
Заняття в університеті бувають трьох типів: лекція, семінар, лабораторна. Кожен тип може мати різну логіку ініціалізації, валідацію, тривалість. Якщо використовувати `new Lesson { Type = ... }` скрізь у коді — зміна логіки створення потребуватиме правок у десятках місць.

### Рішення
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

public class LessonFactoryProvider
{
    public static ILessonFactory GetFactory(string type) => type switch
    {
        "lecture"  => new LectureFactory(),
        "seminar"  => new SeminarFactory(),
        "lab"      => new LaboratoryFactory(),
        _          => new LectureFactory()
    };
}
```

### Чому саме Factory Method
- Інкапсулює логіку створення об'єктів — контролер не знає деталей
- Додавання нового типу заняття (наприклад, `Webinar`) = новий клас + один рядок у `LessonFactoryProvider`, без змін у контролерах
- Легко тестується ізольовано: `new LectureFactory().CreateLesson()` не потребує моків

### Альтернативи що відкинуті
- **Simple Factory** — не дає поліморфізму і важко розширюється
- **Abstract Factory** — надлишково для одного типу продукту (`Lesson`)



## 2. Strategy — Розрахунок рейтингу

### Проблема
Університет використовує різні алгоритми підрахунку рейтингу студента: за результатами іспиту, за відвідуваністю, для стипендійного конкурсу. Алгоритми змінюються незалежно від логіки збереження результату.

### Рішення
```csharp
public interface IRatingStrategy
{
    double Calculate(Student student);
}

public class ExamRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student) => student.Rating + 15;
}

public class AttendanceRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student) => student.Rating * 0.9;
}

public class ScholarshipRatingStrategy : IRatingStrategy
{
    public double Calculate(Student student) => student.Rating * 1.2;
}
```

### Чому саме Strategy
- Алгоритм можна змінювати в runtime — адміністратор вибирає спосіб підрахунку
- Кожна стратегія тестується окремо, ізольовано
- Нова формула = новий клас без зміни `RatingService`
- Відповідає принципу Open/Closed: сервіс закритий для модифікації, відкритий для розширення

### Альтернативи що відкинуті
- **if/switch у сервісі** — порушує OCP, важко підтримувати при додаванні нових формул


## 3. Observer — Сповіщення

### Проблема
Коли викладач змінює оцінку або надсилає повідомлення, студенти мають отримати сповіщення. Якщо `TeacherService` напряму викликає `NotificationService`, виникає жорстка залежність і складне тестування.

### Рішення
```csharp
public class TeacherSubject
{
    private List<IObserver> observers = new();

    public void Attach(IObserver observer) => observers.Add(observer);

    public void Notify(string message)
    {
        foreach (var o in observers)
            o.Update(message);
    }
}

public class NotificationObserver : IObserver
{
    public void Update(string message) { /* SignalR push */ }
}
```

### Чому саме Observer
- Розв'язує `TeacherSubject` від конкретних одержувачів сповіщень
- Легко додати новий тип реакції (наприклад, email) — реалізуй `IObserver`, зареєструй
- Природна інтеграція з SignalR: `NotificationHub` підписується як observer
- Відповідає принципу Dependency Inversion

### Альтернативи що відкинуті
- **Event/Delegate C#** — вбудований механізм, але Observer явніший і більш extensible для multiple subscribers з різною логікою


## 4. Builder — Генерація звітів

### Проблема
Звіти бувають двох форматів: PDF та Excel. Процес побудови звіту однаковий (заголовок + контент + метадані), але кінцевий результат і деталі рендерингу відрізняються.

### Рішення
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

public class ExcelReportBuilder : ReportBuilder
{
    public override void BuildTitle()   => report.Title   = "Excel Report";
    public override void BuildContent() => report.Content = "Excel content";
}
```

### Чому саме Builder
- Відокремлює алгоритм побудови від деталей представлення
- Director (ReportService) керує послідовністю кроків, Builder — деталями
- Легко додати `WordReportBuilder` без змін у сервісі
- Усуває "telescoping constructor" — проблему великої кількості параметрів при створенні складного об'єкта `Report`

### Альтернативи що відкинуті
- **Фабричний метод** — не підходить, бо акцент на покроковій побудові, а не на одному акті створення


## 5. Composite — Ієрархія університету

### Проблема
Університет має деревоподібну структуру: Факультет → Групи → Студенти. Операції (відображення, обхід, підрахунок) мають однаково застосовуватись до будь-якого рівня ієрархії.

### Рішення
```csharp
public interface IUniversityComponent
{
    string Name { get; }
    void Display(int depth = 0);
}

public class FacultyComposite : IUniversityComponent
{
    public List<IUniversityComponent> Children { get; set; } = new();
    public void Add(IUniversityComponent c) => Children.Add(c);
    public void Display(int depth = 0)
    {
        Console.WriteLine(new string('-', depth) + " Faculty: " + Name);
        foreach (var child in Children) child.Display(depth + 2);
    }
}

public class StudentLeaf : IUniversityComponent
{
    public void Display(int depth = 0) =>
        Console.WriteLine(new string('-', depth) + " Student: " + Name);
}
```

### Чому саме Composite
- Клієнтський код (контролер, visitor) не розрізняє листок і композит — єдиний інтерфейс
- Рекурсивний обхід дерева природньо випливає зі структури
- Нові рівні ієрархії (наприклад, `DepartmentComposite`) додаються без змін у клієнті

### Альтернативи що відкинуті
- **Рекурсивні методи в доменних класах** — змішує відображальну логіку з бізнес-логікою, порушує SRP


## 6. Visitor — Операції над ієрархією

### Проблема
Потрібно виконувати різні операції над деревом Composite: перевірка відвідуваності, оновлення рейтингу, генерація статистики, побудова звіту. Додавання кожної операції безпосередньо в класи ієрархії призведе до їх роздуття.

### Рішення
```csharp
public interface IVisitor
{
    void Visit(Student student);
    void Visit(Group group);
    void Visit(Faculty faculty);
}

public class RatingVisitor : IVisitor
{
    public void Visit(Student student) => student.Rating += 10;
    public void Visit(Group group)     => group.Students.ForEach(s => Visit(s));
    public void Visit(Faculty faculty) => faculty.Groups.ForEach(g => Visit(g));
}
```

### Чому саме Visitor
- Додавання нової операції = новий клас `IVisitor`, без змін у доменних класах
- Відокремлює алгоритм від структури даних
- Природне поєднання з Composite: обхід дерева + дія на кожному вузлі
- Усі операції (рейтинг, відвідуваність, статистика, звіт) ізольовані та тестуються окремо

### Альтернативи що відкинуті
- **Методи в доменних класах** — порушує SRP, кожна нова операція змінює стабільні класи
- **Switch по типу** — порушує OCP, не масштабується


## 7. Repository — Абстракція доступу до даних

### Проблема
Сервіси не повинні напряму залежати від Entity Framework. Це унеможливлює тестування (немає можливості підставити fake/mock) і прив'язує бізнес-логіку до конкретної ORM.

### Рішення
```csharp
public interface IRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class StudentService
{
    private readonly IRepository<Student> _repository;

    public StudentService(IRepository<Student> repository)
    {
        _repository = repository;
    }
}
```

### Чому саме Repository
- Повна заміна EF на будь-яку іншу ORM або in-memory сховище без змін у сервісах
- Unit-тести використовують `Mock<IRepository<Student>>` — жодної реальної БД
- Централізує запити — логіка вибірки не розкидана по контролерах
- Відповідає Dependency Inversion Principle

### Альтернативи що відкинуті
- **Прямі виклики DbContext у сервісах** — важко тестувати, жорстка залежність від EF


## Взаємодія патернів

```
HTTP Request
    │
    ▼
Controller
    │
    ├─► Factory → створює Lesson потрібного типу
    │
    ├─► Service (використовує Repository для даних)
    │       │
    │       ├─► Strategy → вибирає алгоритм рейтингу
    │       │
    │       └─► Builder → будує звіт крок за кроком
    │
    ├─► Composite → обходить ієрархію факультет/група/студент
    │       │
    │       └─► Visitor → виконує операцію на кожному вузлі
    │
    └─► Observer → сповіщає підписників про зміни → SignalR → браузер
```

Патерни не конкурують між собою — вони закривають різні аспекти системи: **Factory** вирішує створення, **Strategy** — алгоритми, **Observer** — комунікацію, **Builder** — складну побудову, **Composite + Visitor** — обхід ієрархій, **Repository** — ізоляцію даних.