# SPEC.md — ICA Todo Application Technical Specification

## Project Overview

**Application Name:** ICA Todo Application  
**Purpose:** Internal task tracking tool for ICA Employees to record and manage daily tasks  
**Architecture:** Clean Architecture (Backend) + Feature-based (Frontend)  
**Target Runtime:** .NET 8 SDK, Angular 18, MySQL 8+

---

## Architecture Overview

```
ICA-Todo/
├── src/
│   ├── Core/
│   │   ├── Application/          # Use cases, DTOs, interfaces, services
│   │   └── Domain/               # Entities, domain logic
│   ├── Infrastructure/
│   │   ├── Infrastructure/       # Third-party integrations, helpers
│   │   └── Persistence/          # EF Core DbContext, repositories, migrations
│   └── Presentation/
│       ├── Todo.API/             # ASP.NET Core 8 Web API
│       └── Todo.UI/              # Angular 18 SPA
```

---

## Backend Specification

### Technology Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | MySQL 8+ (Pomelo.EntityFrameworkCore.MySql) |
| Authentication | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) |
| Password Hashing | BCrypt.Net-Next |
| Architecture | Clean Architecture |
| Patterns | Repository Pattern, Service Layer, Dependency Injection |

### NuGet Packages

```xml
<!-- Todo.API -->
Microsoft.AspNetCore.Authentication.JwtBearer (8.x)
Microsoft.EntityFrameworkCore.Design (8.x)
Swashbuckle.AspNetCore (6.x)

<!-- Persistence -->
Pomelo.EntityFrameworkCore.MySql (8.x)
Microsoft.EntityFrameworkCore (8.x)

<!-- Infrastructure -->
BCrypt.Net-Next (4.x)
System.IdentityModel.Tokens.Jwt (7.x)
```

### Project Structure

```
src/
├── Core/
│   ├── Application/
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── RegisterRequestDto.cs
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   └── LoginResponseDto.cs
│   │   │   └── Todo/
│   │   │       ├── CreateTaskDto.cs
│   │   │       ├── TaskResponseDto.cs
│   │   │       └── UpdateTaskStatusDto.cs
│   │   ├── RepositoryInterfaces/
│   │   │   ├── IUserRepository.cs
│   │   │   └── ITaskRepository.cs
│   │   ├── ServiceInterfaces/
│   │   │   ├── IAuthService.cs
│   │   │   └── ITaskService.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   └── TaskService.cs
│   │   └── Wrappers/
│   │       └── ApiResponse.cs
│   └── Domain/
│       └── Entities/
│           ├── User.cs
│           └── TaskItem.cs
├── Infrastructure/
│   ├── Infrastructure/
│   │   └── Helpers/
│   │       └── JwtHelper.cs
│   └── Persistence/
│       ├── Context/
│       │   └── AppDbContext.cs
│       ├── Repositories/
│       │   ├── UserRepository.cs
│       │   └── TaskRepository.cs
│       └── Migrations/
└── Presentation/
    └── Todo.API/
        ├── Controllers/
        │   ├── AuthController.cs
        │   └── TaskController.cs
        ├── Middleware/
        │   └── ExceptionMiddleware.cs
        ├── Program.cs
        └── appsettings.json
```

### Domain Entities

#### User (TBL_User)

```csharp
public class User
{
    public int Id { get; set; }                        // PK, Auto-increment
    public string Username { get; set; }               // NOT NULL, VARCHAR(100)
    public string Email { get; set; }                  // NOT NULL, UNIQUE, VARCHAR(255)
    public string PasswordHash { get; set; }           // NOT NULL, VARCHAR(255) BCrypt hash
    public DateTime CreatedDate { get; set; }          // DEFAULT UTC_NOW
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<TaskItem> Tasks { get; set; }
}
```

#### TaskItem (TBL_Task)

```csharp
public class TaskItem
{
    public int Id { get; set; }                        // PK, Auto-increment
    public string Task { get; set; }                   // NOT NULL, VARCHAR(500)
    public bool IsCompleted { get; set; } = false;
    public bool IsDeleted { get; set; } = false;       // Soft delete flag
    public DateTime CreatedDate { get; set; }          // DEFAULT UTC_NOW
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int UserId { get; set; }                    // FK -> TBL_User
    public User User { get; set; }
}
```

### API Endpoints

#### Auth Controller — `/api/auth`

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| POST | `/api/auth/register` | Register new user | No |
| POST | `/api/auth/login` | Login and get JWT | No |

#### Task Controller — `/api/tasks`

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| GET | `/api/tasks` | Get paginated tasks | Yes |
| GET | `/api/tasks/search?q={query}&page={n}` | Search tasks | Yes |
| POST | `/api/tasks` | Create new task | Yes |
| PATCH | `/api/tasks/{id}/status` | Update task status | Yes |
| DELETE | `/api/tasks/{id}` | Soft delete task | Yes |

### API Response Wrapper

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

**Example responses:**

```json
// Success
{ "success": true, "message": "Login successful", "data": { "token": "eyJ..." } }

// Error
{ "success": false, "message": "Validation failed", "errors": ["Email is required"] }
```

### JWT Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=IcaTodoDB;Uid=root;Pwd=yourpassword;"
  },
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-minimum-32-characters",
    "Issuer": "IcaTodoApp",
    "Audience": "IcaTodoUsers",
    "ExpiryInMinutes": 1440
  },
  "AllowedHosts": "*"
}
```

### Validation Rules

**Register:**
- Username: Required, 2–100 chars
- Email: Required, valid email format, unique
- Password: Required, min 8 chars, at least one number and one uppercase letter
- ConfirmPassword: Must match Password

**Login:**
- Email: Required, valid email format
- Password: Required

**Create Task:**
- Task: Required, 1–500 chars

### Pagination

All list endpoints return:
```json
{
  "success": true,
  "message": "Tasks retrieved",
  "data": {
    "items": [...],
    "totalCount": 45,
    "page": 1,
    "pageSize": 8,
    "totalPages": 6
  }
}
```

Default page size: **8 tasks per page**

---

## Frontend Specification

### Technology Stack

| Component | Technology |
|---|---|
| Framework | Angular 18 |
| UI Library | Bootstrap 5.3 |
| Forms | Angular Reactive Forms |
| HTTP | Angular HttpClient |
| Routing | Angular Router |
| Language | TypeScript 5+ |
| State | Services + BehaviorSubject |

### Folder Structure

```
src/app/
├── core/
│   ├── guards/
│   │   └── auth.guard.ts
│   ├── interceptors/
│   │   ├── auth.interceptor.ts        # Attach JWT to requests
│   │   └── loading.interceptor.ts     # Trigger spinner
│   ├── services/
│   │   ├── auth.service.ts
│   │   ├── task.service.ts
│   │   ├── theme.service.ts
│   │   └── loading.service.ts
│   └── models/
│       ├── user.model.ts
│       ├── task.model.ts
│       └── api-response.model.ts
├── views/
│   ├── auth/
│   │   ├── register/
│   │   │   ├── register.component.ts
│   │   │   ├── register.component.html
│   │   │   └── register.component.scss
│   │   └── login/
│   │       ├── login.component.ts
│   │       ├── login.component.html
│   │       └── login.component.scss
│   └── todo/
│       ├── todo.component.ts
│       ├── todo.component.html
│       └── todo.component.scss
├── layouts/
│   ├── navbar/
│   │   ├── navbar.component.ts
│   │   ├── navbar.component.html
│   │   └── navbar.component.scss
│   └── spinner/
│       ├── spinner.component.ts
│       ├── spinner.component.html
│       └── spinner.component.scss
├── app.component.ts
├── app.component.html
├── app.routes.ts
└── app.config.ts
```

### Routing Table

| Route | Component | Guard |
|---|---|---|
| `/` | Redirect to `/todo` | AuthGuard |
| `/auth/login` | LoginComponent | — |
| `/auth/register` | RegisterComponent | — |
| `/todo` | TodoComponent | AuthGuard |

### Theme System

- Light/Dark toggle stored in `localStorage` key: `ica-theme`
- ThemeService applies `data-bs-theme="dark"` or `"light"` to `<html>` element
- Toggle button in Navbar (sun/moon icon)

### Local Storage Keys

| Key | Value |
|---|---|
| `ica_jwt_token` | JWT token string |
| `ica_user` | Serialized user object (id, username, email) |
| `ica-theme` | `"light"` or `"dark"` |

### Loading Spinner

- Global overlay spinner via `LoadingService` (BehaviorSubject)
- `LoadingInterceptor` sets loading state on every HTTP request
- Spinner component subscribes and shows/hides accordingly

### Form Validation Behavior

- Show error messages only after field is touched or form is submitted
- Disable submit button while form is invalid or loading
- Show Bootstrap `is-invalid` class on invalid touched fields

### Todo Page Behavior

- On load: fetch page 1 (8 tasks)
- Search input: debounce 400ms, resets to page 1
- Checkbox toggle: immediately calls PATCH API, updates UI
- Completed tasks: text shown with `text-decoration: line-through; color: red`
- Delete: calls DELETE API, removes card from list
- Pagination: show up to 8 tasks per page, show page numbers

---

## Database Schema (EF Core Code-First)

### MySQL Tables

```sql
-- TBL_User
CREATE TABLE `TBL_User` (
  `Id`           INT NOT NULL AUTO_INCREMENT,
  `Username`     VARCHAR(100) NOT NULL,
  `Email`        VARCHAR(255) NOT NULL,
  `PasswordHash` VARCHAR(255) NOT NULL,
  `CreatedDate`  DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedDate`  DATETIME(6) NULL,
  `IsActive`     TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `IX_TBL_User_Email` (`Email`)
);

-- TBL_Task
CREATE TABLE `TBL_Task` (
  `Id`            INT NOT NULL AUTO_INCREMENT,
  `Task`          VARCHAR(500) NOT NULL,
  `IsCompleted`   TINYINT(1) NOT NULL DEFAULT 0,
  `IsDeleted`     TINYINT(1) NOT NULL DEFAULT 0,
  `CreatedDate`   DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `UpdatedDate`   DATETIME(6) NULL,
  `CompletedDate` DATETIME(6) NULL,
  `UserId`        INT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_TBL_Task_UserId` (`UserId`),
  INDEX `IX_TBL_Task_IsDeleted_UserId` (`IsDeleted`, `UserId`),
  CONSTRAINT `FK_TBL_Task_TBL_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `TBL_User` (`Id`) ON DELETE CASCADE
);
```

---

## Security Requirements

- Passwords hashed with BCrypt (work factor: 12)
- JWT tokens expire in 24 hours (configurable)
- All task endpoints protected by `[Authorize]` attribute
- UserId extracted from JWT claims — never trusted from request body
- Soft delete (IsDeleted flag) — no hard deletes on tasks
- Input validation on both client and server side
- CORS configured for Angular dev server (`http://localhost:4200`)

---

## Environment Setup

### Backend

```bash
cd src/Presentation/Todo.API
dotnet restore
dotnet ef database update
dotnet run
# API runs on https://localhost:7001
```

### Frontend

```bash
cd src/Presentation/Todo.UI
npm install
ng serve
# App runs on http://localhost:4200
```
