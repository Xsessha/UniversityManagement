# University Management System

## Purpose
This project is an ASP.NET Core MVC university portal for managing students, teachers, courses, groups, schedules, attendance, grades, notifications, and reports.

## Role-based workflow

### 1. Student workflow
1. Student opens the registration page and fills in first name, last name, email, password, confirmation password, and selects the Student role.
2. The system creates the account, stores the user in the database, assigns the Student role, creates the student profile, and detects the related faculty, group, and course context.
3. The platform generates the student schedule, creates starter notifications, and prepares the personal dashboard.
4. The student automatically signs in and lands on the Student Dashboard.
5. The dashboard shows statistics such as average grade, attendance percentage, number of subjects, notifications, and current rating.
6. The schedule is generated automatically for the group, and the student can view subject, teacher, room, time, and lesson type.
7. The student can review grades, GPA, attendance, rating, and real-time notifications from the dashboard.
8. The system updates the dashboard automatically when attendance, grades, or schedule details change.

### 2. Teacher workflow
1. Teacher creates an account and selects the Teacher role.
2. The system creates the teacher profile, assigns courses and groups, generates the teacher schedule, and opens the Teacher Dashboard.
3. The teacher can manage attendance, grades, comments, and schedule updates for assigned classes.
4. The dashboard displays today’s lessons, rooms, groups, and schedule details.
5. The teacher can mark attendance as present, absent, or late, and the student receives the update automatically.
6. The teacher can review GPA, attendance statistics, and performance charts for the assigned groups.
7. The teacher can export reports and send notifications about schedule changes or academic updates.

### 3. Administrator workflow
1. Admin manages faculties, groups, courses, teachers, students, and schedules from the central Admin Dashboard.
2. Admin can create, edit, block, and move students between groups; assign courses to teachers; manage faculty structure; and trigger schedule generation.
3. Admin monitors university-wide statistics, reports, attendance analytics, GPA analytics, and notifications.
4. Admin controls roles, user permissions, authorization, and university-wide automation.

### 4. Role interaction and automation
- Student ↔ Teacher: teacher updates grades and attendance; student receives live updates.
- Teacher ↔ Administrator: admin assigns courses and groups; teachers receive schedule updates.
- Student ↔ Administrator: admin manages groups and academic structure; students receive system notifications.
- After every meaningful change, the system updates the database, recalculates dashboard statistics, sends notifications, and keeps the platform synchronized.

## Automation expectations
- Database updates on every meaningful change.
- Dashboard statistics are recalculated after grade, attendance, or schedule updates.
- Notifications are sent in real time for academic and schedule events.
- Schedule and analytics logic should feel like a real university information system rather than a simple CRUD application.

## Role map
- Admin: full management and reporting
- Teacher: lesson, attendance, and grading workflow
- Student: personal schedule, attendance, grades, and notifications

## Main modules
- Authentication and roles: AccountController, Identity, ApplicationUser
- Students / Teachers / Courses / Groups / Faculties: CRUD and details views
- Schedule / Attendance / Grades / Reports / Notifications: role-based workflows
- Dashboard: statistics and quick access

## Suggested daily usage
- Start from /Account/Login
- Create or verify users in the admin panel
- Assign courses and groups before generating lessons
- Review attendance and grades after each lesson
- Use the dashboard for current activity and alerts

## Technical notes
- ASP.NET Core MVC + Razor Views
- Entity Framework Core + SQLite/SQL Server
- Identity for authentication and authorization
- SignalR for notifications

## Run locally
1. dotnet restore
2. dotnet build
3. dotnet run --project UniversityManagement.Web

## Current status
The application builds successfully and the main runtime flow is ready for role-based usage.
