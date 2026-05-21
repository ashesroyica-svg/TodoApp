# project-spec.md — ICA Todo Application: Complete Build Prompt

## Master Prompt for Claude Code

You are a Senior Full Stack Software Architect and Senior .NET Developer. Build a complete, production-ready **ICA Employee Todo Application** from scratch using the specifications below. Every file must be fully implemented — no stubs, no placeholders.

---

## What to Build

A full-stack internal task management application for ICA Employees consisting of:

1. **ASP.NET Core 8 REST API** — Clean Architecture, JWT auth, EF Core + MySQL
2. **Angular 18 SPA** — Bootstrap 5.3, Reactive Forms, Dark/Light theme, loading spinner
3. **MySQL Database** — Code-first schema via EF Core migrations

---

## Solution Structure

Create a Visual Studio solution file `IcaTodo.sln` at the root with these projects:

```
IcaTodo/
├── IcaTodo.sln
├── CLAUDE.md
├── SPEC.md
├── project-spec.md
└── src/
    ├── Core/
    │   ├── Application/
    │   │   ├── Application.csproj
    │   │   ├── DTOs/
    │   │   │   ├── Auth/
    │   │   │   │   ├── RegisterRequestDto.cs
    │   │   │   │   ├── LoginRequestDto.cs
    │   │   │   │   └── LoginResponseDto.cs
    │   │   │   └── Todo/
    │   │   │       ├── CreateTaskDto.cs
    │   │   │       ├── TaskResponseDto.cs
    │   │   │       ├── UpdateTaskStatusDto.cs
    │   │   │       └── PaginatedResultDto.cs
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
    │       ├── Domain.csproj
    │       └── Entities/
    │           ├── User.cs
    │           └── TaskItem.cs
    ├── Infrastructure/
    │   ├── Infrastructure/
    │   │   ├── Infrastructure.csproj
    │   │   └── Helpers/
    │   │       └── JwtHelper.cs
    │   └── Persistence/
    │       ├── Persistence.csproj
    │       ├── Context/
    │       │   └── AppDbContext.cs
    │       └── Repositories/
    │           ├── UserRepository.cs
    │           └── TaskRepository.cs
    └── Presentation/
        ├── Todo.API/
        │   ├── Todo.API.csproj
        │   ├── Program.cs
        │   ├── appsettings.json
        │   ├── appsettings.Development.json
        │   ├── Controllers/
        │   │   ├── AuthController.cs
        │   │   └── TaskController.cs
        │   └── Middleware/
        │       └── ExceptionMiddleware.cs
        └── Todo.UI/                           ← Angular 18 project
            ├── angular.json
            ├── package.json
            ├── tsconfig.json
            ├── tsconfig.app.json
            └── src/
                ├── index.html
                ├── main.ts
                ├── styles.scss
                └── app/
                    ├── app.component.ts
                    ├── app.component.html
                    ├── app.component.scss
                    ├── app.routes.ts
                    ├── app.config.ts
                    ├── core/
                    │   ├── guards/
                    │   │   └── auth.guard.ts
                    │   ├── interceptors/
                    │   │   ├── auth.interceptor.ts
                    │   │   └── loading.interceptor.ts
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
                    │   │   ├── login/
                    │   │   │   ├── login.component.ts
                    │   │   │   ├── login.component.html
                    │   │   │   └── login.component.scss
                    │   │   └── register/
                    │   │       ├── register.component.ts
                    │   │       ├── register.component.html
                    │   │       └── register.component.scss
                    │   └── todo/
                    │       ├── todo.component.ts
                    │       ├── todo.component.html
                    │       └── todo.component.scss
                    └── layouts/
                        ├── navbar/
                        │   ├── navbar.component.ts
                        │   ├── navbar.component.html
                        │   └── navbar.component.scss
                        └── spinner/
                            ├── spinner.component.ts
                            ├── spinner.component.html
                            └── spinner.component.scss
```

---

## Step-by-Step Implementation Instructions

### STEP 1: Domain Layer (`src/Core/Domain`)

Create `Domain.csproj` targeting `net8.0`.

**`User.cs`** — Entity with properties: `Id (int PK)`, `Username (string)`, `Email (string)`, `PasswordHash (string)`, `CreatedDate (DateTime)`, `UpdatedDate (DateTime?)`, `IsActive (bool, default true)`, navigation: `ICollection<TaskItem> Tasks`.

**`TaskItem.cs`** — Entity with properties: `Id (int PK)`, `Task (string)`, `IsCompleted (bool, default false)`, `IsDeleted (bool, default false)`, `CreatedDate (DateTime)`, `UpdatedDate (DateTime?)`, `CompletedDate (DateTime?)`, `UserId (int FK)`, navigation: `User User`.

---

### STEP 2: Application Layer (`src/Core/Application`)

Create `Application.csproj` targeting `net8.0`, referencing Domain project.

**`ApiResponse<T>`** — Generic wrapper with: `bool Success`, `string Message`, `T? Data`, `List<string>? Errors`. Add static factory methods: `Ok(T data, string message)`, `Fail(string message, List<string>? errors = null)`.

**Auth DTOs:**
- `RegisterRequestDto`: `Username (Required, 2-100 chars)`, `Email (Required, EmailAddress)`, `Password (Required, MinLength 8, RegEx for uppercase+number)`, `ConfirmPassword (Required, Compare Password)`
- `LoginRequestDto`: `Email (Required, EmailAddress)`, `Password (Required)`
- `LoginResponseDto`: `Token (string)`, `Username (string)`, `Email (string)`, `UserId (int)`

**Todo DTOs:**
- `CreateTaskDto`: `Task (Required, 1-500 chars)`
- `TaskResponseDto`: `Id`, `Task`, `IsCompleted`, `CreatedDate`, `CompletedDate?`
- `UpdateTaskStatusDto`: `IsCompleted (bool)`
- `PaginatedResultDto<T>`: `Items (List<T>)`, `TotalCount`, `Page`, `PageSize`, `TotalPages`

**Repository Interfaces:**
- `IUserRepository`: `GetByEmailAsync(string email)`, `CreateAsync(User user)`, `EmailExistsAsync(string email)`
- `ITaskRepository`: `GetPagedAsync(int userId, int page, int pageSize, string? search)`, `GetTotalCountAsync(int userId, string? search)`, `CreateAsync(TaskItem task)`, `GetByIdAsync(int id, int userId)`, `UpdateAsync(TaskItem task)`, `SoftDeleteAsync(int id, int userId)`

**Service Interfaces:**
- `IAuthService`: `RegisterAsync(RegisterRequestDto dto)`, `LoginAsync(LoginRequestDto dto)`
- `ITaskService`: `GetTasksAsync(int userId, int page, int pageSize, string? search)`, `CreateTaskAsync(int userId, CreateTaskDto dto)`, `UpdateTaskStatusAsync(int userId, int taskId, UpdateTaskStatusDto dto)`, `DeleteTaskAsync(int userId, int taskId)`

**Service Implementations:**

`AuthService.cs`:
- `RegisterAsync`: validate email uniqueness, hash password with `BCrypt.Net.BCrypt.HashPassword(password, 12)`, save user, return success ApiResponse
- `LoginAsync`: find user by email, verify with `BCrypt.Net.BCrypt.Verify(password, hash)`, generate JWT via `JwtHelper`, return `LoginResponseDto` with token

`TaskService.cs`:
- All methods filter by `UserId` and `IsDeleted == false`
- `GetTasksAsync`: returns `PaginatedResultDto<TaskResponseDto>` with correct pagination math
- `CreateTaskAsync`: creates new `TaskItem`, sets `CreatedDate = DateTime.UtcNow`
- `UpdateTaskStatusAsync`: updates `IsCompleted`, sets `CompletedDate = UtcNow` if completing, nulls it if unchecking, sets `UpdatedDate`
- `DeleteTaskAsync`: sets `IsDeleted = true`, sets `UpdatedDate = UtcNow`

---

### STEP 3: Persistence Layer (`src/Infrastructure/Persistence`)

Create `Persistence.csproj` targeting `net8.0`, referencing Domain + Application. Add NuGet: `Pomelo.EntityFrameworkCore.MySql`, `Microsoft.EntityFrameworkCore`.

**`AppDbContext.cs`**:
```csharp
// DbSet<User> Users, DbSet<TaskItem> Tasks
// OnModelCreating:
//   - TBL_User table, unique index on Email
//   - TBL_Task table, index on (UserId, IsDeleted)
//   - All DateTime columns use datetime(6)
//   - String column lengths specified explicitly
```

**`UserRepository.cs`** — Implements `IUserRepository`. All methods async.

**`TaskRepository.cs`** — Implements `ITaskRepository`. 
- `GetPagedAsync`: filter by `userId`, `!IsDeleted`, optional `Contains(search)` on Task field, ordered by `CreatedDate DESC`, skip/take for pagination
- `GetTotalCountAsync`: same filters, return `CountAsync()`

---

### STEP 4: Infrastructure Layer (`src/Infrastructure/Infrastructure`)

Create `Infrastructure.csproj` referencing Domain + Application.

**`JwtHelper.cs`**:
```csharp
// Constructor: IConfiguration config
// Method: GenerateToken(User user) -> string
// Claims: NameIdentifier (userId), Email, Name (username)
// Signing: HmacSha256 with secret from config["JwtSettings:SecretKey"]
// Expiry: from config["JwtSettings:ExpiryInMinutes"]
// Issuer/Audience from config
```

---

### STEP 5: API Layer (`src/Presentation/Todo.API`)

Create `Todo.API.csproj` targeting `net8.0`, referencing all projects. Add NuGet: `Microsoft.AspNetCore.Authentication.JwtBearer`, `BCrypt.Net-Next`, `Swashbuckle.AspNetCore`.

**`ExceptionMiddleware.cs`** — Catches all unhandled exceptions, logs them, returns `ApiResponse<object>` with `success: false` and generic error message. Never expose stack traces.

**`AuthController.cs`** — Route: `/api/auth`
- `POST /register` → `IAuthService.RegisterAsync` → returns `201 Created` on success
- `POST /login` → `IAuthService.LoginAsync` → returns `200 OK` with token

**`TaskController.cs`** — Route: `/api/tasks`, decorated with `[Authorize]`
- `GET /` with query params `page (default 1)`, `pageSize (default 8)`, `search (optional)` → paginated results
- `POST /` → create task, return `201 Created`
- `PATCH /{id}/status` → update IsCompleted
- `DELETE /{id}` → soft delete

In every TaskController action, extract `UserId` from JWT claims using:
```csharp
int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
```

**`Program.cs`** — Complete startup configuration:
```csharp
// Services registered:
// - AddDbContext with UseMySql (Pomelo), ServerVersion.AutoDetect
// - AddAuthentication(JwtBearer) with ValidateIssuer/Audience/Lifetime/SigningKey
// - AddScoped for all repositories and services
// - AddCors for http://localhost:4200
// - AddSwaggerGen with JWT Bearer auth scheme
// - AddControllers with model validation

// Middleware pipeline:
// - UseExceptionMiddleware (custom)
// - UseSwagger / UseSwaggerUI (development only)
// - UseCors
// - UseAuthentication
// - UseAuthorization
// - MapControllers
```

**`appsettings.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=IcaTodoDB;Uid=root;Pwd=YourPassword123!;"
  },
  "JwtSettings": {
    "SecretKey": "IcaTodoAppSuperSecretKey2024!MustBe32CharsMin",
    "Issuer": "IcaTodoApp",
    "Audience": "IcaTodoUsers",
    "ExpiryInMinutes": 1440
  },
  "AllowedHosts": "*",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

### STEP 6: Angular Frontend (`src/Presentation/Todo.UI`)

Bootstrap a complete Angular 18 standalone application.

**`package.json`** — Include: `@angular/core@18`, `@angular/forms@18`, `@angular/router@18`, `@angular/common@18`, `bootstrap@5.3`, `bootstrap-icons` (for sun/moon/trash icons), `rxjs@7`.

**Models (`core/models/`):**

```typescript
// api-response.model.ts
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: string[];
}

// user.model.ts
export interface User {
  userId: number;
  username: string;
  email: string;
  token: string;
}

// task.model.ts
export interface Task {
  id: number;
  task: string;
  isCompleted: boolean;
  createdDate: string;
  completedDate?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
```

**`loading.service.ts`** — `BehaviorSubject<boolean>(false)`, expose as `loading$` observable, methods `show()` / `hide()`.

**`theme.service.ts`** — Toggle between `light`/`dark`, persist to `localStorage('ica-theme')`, apply `document.documentElement.setAttribute('data-bs-theme', theme)` on init and toggle.

**`auth.service.ts`**:
- `register(dto)` → POST `/api/auth/register`
- `login(dto)` → POST `/api/auth/login` → on success save to `localStorage('ica_jwt_token')` and `localStorage('ica_user')`
- `logout()` → clear both localStorage keys
- `isLoggedIn()` → check token presence
- `getToken()` → return token from localStorage

**`task.service.ts`**:
- `getTasks(page, pageSize, search)` → GET `/api/tasks?page=&pageSize=&search=`
- `createTask(dto)` → POST `/api/tasks`
- `updateTaskStatus(id, isCompleted)` → PATCH `/api/tasks/${id}/status`
- `deleteTask(id)` → DELETE `/api/tasks/${id}`

**`auth.interceptor.ts`** — Clone request, add `Authorization: Bearer <token>` header if token exists in localStorage.

**`loading.interceptor.ts`** — Call `loadingService.show()` before request, `loadingService.hide()` in `finalize()`.

**`auth.guard.ts`** — Functional guard: redirect to `/auth/login` if `!authService.isLoggedIn()`.

**`spinner.component`** — Overlay div with Bootstrap spinner, subscribes to `loading$`, shown via `*ngIf` (or `@if`). Full-screen semi-transparent backdrop.

**`navbar.component`**:
- Left: ICA logo (use a placeholder `<span>` with "ICA" styled as a blue badge) + app title "ICA Todo"
- Right: Dark/Light theme toggle button (sun/moon icon from Bootstrap Icons) + Logout button
- On logout: call `authService.logout()`, navigate to `/auth/login`

**`login.component`**:
- Full-page vertically and horizontally centered layout (`min-vh-100 d-flex align-items-center justify-content-center`)
- Bootstrap Card (`card shadow-lg`, `card-body p-5`, `max-width: 420px`)
- Card header: "Welcome Back" title + "ICA Employee Portal" subtitle
- Reactive form with Email and Password fields
- Show `is-invalid` class + error message when field touched and invalid
- Login button: full width, disabled while loading or form invalid
- Link to Register page below button
- On success: navigate to `/todo`
- On error: show Bootstrap alert with error message

**`register.component`**:
- Same centered card layout as login (`max-width: 480px`)
- Card header: "Create Account" + "Join ICA Todo"
- Reactive form: Username, Email, Password, Confirm Password
- Custom validator: `passwordMatchValidator` — compare `password` and `confirmPassword` fields
- Show validation errors inline (touched + invalid)
- Register button: full-width, disabled while loading/invalid
- Link to Login below
- On success: navigate to `/auth/login` with success message

**`todo.component`**:

HTML layout:
```
<app-navbar>
<main class="container py-4">
  <!-- Add Task Card -->
  <div class="card shadow-sm mb-4">
    <div class="card-body">
      <div class="input-group">
        <input type="text" [formControl]="taskControl" placeholder="What needs to be done?" class="form-control form-control-lg">
        <button (click)="addTask()" class="btn btn-primary">Add Task</button>
      </div>
    </div>
  </div>

  <!-- Search -->
  <div class="mb-3">
    <input type="text" [formControl]="searchControl" placeholder="Search tasks..." class="form-control">
  </div>

  <!-- Task Cards -->
  <div *ngFor="let task of tasks">
    <div class="card shadow-sm mb-2" [class.completed-task]="task.isCompleted">
      <div class="card-body d-flex align-items-center gap-3">
        <input type="checkbox" [checked]="task.isCompleted" (change)="toggleStatus(task)">
        <span [class.text-decoration-line-through]="task.isCompleted" [class.text-danger]="task.isCompleted">
          {{ task.task }}
        </span>
        <button class="btn btn-outline-danger btn-sm ms-auto" (click)="deleteTask(task.id)">
          <i class="bi bi-trash"></i>
        </button>
      </div>
    </div>
  </div>

  <!-- Pagination -->
  <nav>
    <ul class="pagination justify-content-center">
      <!-- Previous, page numbers, Next -->
    </ul>
  </nav>
</main>
```

TypeScript behavior:
- `ngOnInit`: call `loadTasks(1)`
- `loadTasks(page)`: call `taskService.getTasks(page, 8, searchQuery)`, update `tasks` array and pagination state
- `searchControl.valueChanges.pipe(debounceTime(400), distinctUntilChanged())`: call `loadTasks(1)` with new query
- `addTask()`: validate `taskControl` not empty, call `taskService.createTask()`, reload page 1, clear input
- `toggleStatus(task)`: call `taskService.updateTaskStatus(task.id, !task.isCompleted)`, update task in array
- `deleteTask(id)`: call `taskService.deleteTask(id)`, remove from `tasks` array
- `goToPage(n)`: bounds check, call `loadTasks(n)`

**`app.routes.ts`**:
```typescript
export const routes: Routes = [
  { path: '', redirectTo: 'todo', pathMatch: 'full' },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'todo', component: TodoComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'todo' }
];
```

**`app.config.ts`**:
```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, loadingInterceptor]))
  ]
};
```

**`index.html`** — Include Bootstrap 5.3 CSS from CDN, Bootstrap Icons CSS from CDN, set `<html lang="en">`.

**`styles.scss`** — Global styles:
```scss
// Import bootstrap
@import 'bootstrap/scss/bootstrap';

// ICA brand colors
:root {
  --ica-primary: #003087;
  --ica-accent: #e31837;
}

// Completed task styling
.completed-task span {
  text-decoration: line-through;
  color: #dc3545 !important;
}

// Spinner overlay
.spinner-overlay {
  position: fixed;
  top: 0; left: 0;
  width: 100%; height: 100%;
  background: rgba(0,0,0,0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
```

---

## Environment Configuration

**`environments/environment.ts`**:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:7001'
};
```

---

## EF Core Migrations Command

After all backend files are created, run:
```bash
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/Persistence \
  --startup-project src/Presentation/Todo.API \
  --output-dir Migrations

dotnet ef database update \
  --project src/Infrastructure/Persistence \
  --startup-project src/Presentation/Todo.API
```

---

## Final Checklist

Before considering the implementation complete, verify:

### Backend
- [ ] All 4 `.csproj` files reference correct dependencies
- [ ] `IcaTodo.sln` includes all projects
- [ ] `AppDbContext` uses `TBL_User` and `TBL_Task` table names
- [ ] All repository methods are async
- [ ] JWT claims include `NameIdentifier` (userId), `Email`, `Name`
- [ ] All controller actions return `ApiResponse<T>`
- [ ] `[Authorize]` on `TaskController`
- [ ] UserId from JWT claims in every task action
- [ ] BCrypt work factor 12
- [ ] Soft delete on all task deletions
- [ ] CORS allows `http://localhost:4200`
- [ ] Swagger configured with JWT auth

### Frontend
- [ ] All components are standalone
- [ ] Auth interceptor attaches Bearer token
- [ ] Loading interceptor wraps every HTTP call
- [ ] AuthGuard protects `/todo` route
- [ ] Login saves token + user to localStorage on success
- [ ] Logout clears localStorage + redirects to login
- [ ] Search debounced 400ms
- [ ] Pagination shows max 8 tasks per page
- [ ] Completed tasks show strikethrough red text
- [ ] Dark/Light theme toggle works and persists
- [ ] Form validation shows inline errors
- [ ] Loading spinner shows on API calls

### Database
- [ ] MySQL connection string in `appsettings.json`
- [ ] EF Core migrations created
- [ ] Unique index on `TBL_User.Email`
- [ ] Composite index on `TBL_Task (UserId, IsDeleted)`
- [ ] FK from `TBL_Task.UserId` to `TBL_User.Id` with CASCADE DELETE

---

## Running the Application

### Backend
```bash
cd src/Presentation/Todo.API
dotnet run
# Swagger UI: https://localhost:7001/swagger
```

### Frontend
```bash
cd src/Presentation/Todo.UI
npm install
ng serve --open
# App: http://localhost:4200
```
