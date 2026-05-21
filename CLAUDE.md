# CLAUDE.md — Claude Code Instructions for ICA Todo Application

## Project Context

This is a full-stack ICA Employee Todo Application built with:
- **Backend:** ASP.NET Core 8 Web API (Clean Architecture)
- **Frontend:** Angular 18 + Bootstrap 5.3
- **Database:** MySQL 8+ via EF Core (Code-First)
- **Auth:** JWT Bearer tokens

Read `SPEC.md` and `project-spec.md` for full requirements before generating any code.

---

## Code Generation Rules

### General

- Always generate **complete, runnable files** — no placeholders, no `// TODO` comments
- Follow the exact folder structure defined in `SPEC.md`
- Every file must compile/run without modification
- Use **UTF-8 encoding** for all files
- Add XML doc comments on all public C# methods and classes
- Add JSDoc comments on all public TypeScript methods

### C# / .NET Conventions

- Target framework: `net8.0`
- Nullable reference types: **enabled** (`<Nullable>enable</Nullable>`)
- Implicit usings: **enabled**
- Use `var` for local variables where type is obvious
- Use `async/await` for all I/O operations
- Controller actions must return `Task<IActionResult>`
- Use `ApiResponse<T>` wrapper for **all** API responses
- Use `[Required]`, `[EmailAddress]`, `[MinLength]` data annotations on all DTOs
- Use `ILogger<T>` in all services and controllers
- Repository methods must be async and cancellable (`CancellationToken ct = default`)
- `AppDbContext` table names: `TBL_User`, `TBL_Task`
- Use `HasColumnType("varchar(255)")` etc. in EF fluent configuration

**Naming Conventions:**
- Interfaces: `IServiceName`, `IRepositoryName`
- DTOs: `EntityActionDto` (e.g., `RegisterRequestDto`, `TaskResponseDto`)
- Services: `EntityService` (e.g., `AuthService`)
- Repositories: `EntityRepository`
- Controllers: `EntityController`

### TypeScript / Angular Conventions

- Use **standalone components** (Angular 18 style — no NgModules)
- Use `inject()` function instead of constructor injection where possible
- Use `signal()` and `computed()` where appropriate
- Reactive Forms only — no template-driven forms
- Services use `providedIn: 'root'`
- All HTTP calls return typed `Observable<ApiResponse<T>>`
- Unsubscribe using `takeUntilDestroyed()` or `async` pipe
- Use `environment.ts` for API base URL: `http://localhost:7001`
- Theme toggle must persist to `localStorage`
- Spinner must show/hide on **every** HTTP request via interceptor

**Naming Conventions:**
- Components: `feature.component.ts`
- Services: `feature.service.ts`
- Guards: `feature.guard.ts`
- Interceptors: `feature.interceptor.ts`
- Models: `feature.model.ts`
- Use `camelCase` for variables, `PascalCase` for classes/interfaces

### CSS / Styling Conventions

- Use Bootstrap 5.3 utility classes as the primary styling approach
- Component-scoped SCSS for custom overrides
- Dark mode: apply `data-bs-theme="dark"` on `<html>` tag
- ICA brand color: `#003087` (navy blue) as primary accent
- Completed task style: `text-decoration: line-through; color: #dc3545;`
- Navbar: always use `bg-primary` with `data-bs-theme="dark"` navbar variant
- Cards: `shadow-sm` on all task cards
- Loading spinner: centered fullscreen overlay with semi-transparent backdrop

---

## File Generation Order

Generate files in this order to avoid dependency issues:

### Backend Order
1. `Domain/Entities/User.cs`
2. `Domain/Entities/TaskItem.cs`
3. `Application/Wrappers/ApiResponse.cs`
4. `Application/DTOs/Auth/*.cs`
5. `Application/DTOs/Todo/*.cs`
6. `Application/RepositoryInterfaces/IUserRepository.cs`
7. `Application/RepositoryInterfaces/ITaskRepository.cs`
8. `Application/ServiceInterfaces/IAuthService.cs`
9. `Application/ServiceInterfaces/ITaskService.cs`
10. `Persistence/Context/AppDbContext.cs`
11. `Persistence/Repositories/UserRepository.cs`
12. `Persistence/Repositories/TaskRepository.cs`
13. `Infrastructure/Helpers/JwtHelper.cs`
14. `Application/Services/AuthService.cs`
15. `Application/Services/TaskService.cs`
16. `Todo.API/Controllers/AuthController.cs`
17. `Todo.API/Controllers/TaskController.cs`
18. `Todo.API/Middleware/ExceptionMiddleware.cs`
19. `Todo.API/Program.cs`
20. `Todo.API/appsettings.json`
21. `.csproj` files for each project
22. `IcaTodo.sln`

### Frontend Order
1. `environments/environment.ts`
2. `core/models/*.ts`
3. `core/services/loading.service.ts`
4. `core/services/theme.service.ts`
5. `core/services/auth.service.ts`
6. `core/services/task.service.ts`
7. `core/interceptors/auth.interceptor.ts`
8. `core/interceptors/loading.interceptor.ts`
9. `core/guards/auth.guard.ts`
10. `layouts/spinner/spinner.component.*`
11. `layouts/navbar/navbar.component.*`
12. `views/auth/login/login.component.*`
13. `views/auth/register/register.component.*`
14. `views/todo/todo.component.*`
15. `app.routes.ts`
16. `app.config.ts`
17. `app.component.*`
18. `index.html`
19. `main.ts`
20. `angular.json`, `package.json`, `tsconfig.json`

---

## Key Implementation Details

### JWT Token Extraction in Backend

The UserId must **always** be extracted from the JWT claim inside the controller — never from the request body:

```csharp
private int GetUserIdFromToken()
{
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null) throw new UnauthorizedAccessException();
    return int.Parse(userIdClaim.Value);
}
```

### Soft Delete Pattern

Never hard-delete tasks. Always set `IsDeleted = true` and filter in all queries:

```csharp
// Repository — always filter deleted tasks
.Where(t => t.UserId == userId && !t.IsDeleted)
```

### Search with Debounce (Frontend)

```typescript
// In TodoComponent
searchControl = new FormControl('');

ngOnInit() {
  this.searchControl.valueChanges.pipe(
    debounceTime(400),
    distinctUntilChanged(),
    takeUntilDestroyed(this.destroyRef)
  ).subscribe(query => {
    this.currentPage = 1;
    this.loadTasks(query ?? '');
  });
}
```

### Auth Interceptor Pattern

```typescript
// Attach token to every outgoing request
intercept(req: HttpRequest<unknown>, next: HttpHandler) {
  const token = localStorage.getItem('ica_jwt_token');
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  return next.handle(req);
}
```

### BCrypt Usage (Backend)

```csharp
// Hash on register
string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

// Verify on login
bool valid = BCrypt.Net.BCrypt.Verify(password, storedHash);
```

### EF Core Configuration

```csharp
// AppDbContext — table naming
modelBuilder.Entity<User>().ToTable("TBL_User");
modelBuilder.Entity<TaskItem>().ToTable("TBL_Task");

// Indexes for performance
modelBuilder.Entity<User>()
    .HasIndex(u => u.Email).IsUnique();
modelBuilder.Entity<TaskItem>()
    .HasIndex(t => new { t.UserId, t.IsDeleted });
```

---

## Error Handling

### Backend

- Global exception middleware catches all unhandled exceptions
- Returns `ApiResponse<object>` with `success: false` and error message
- Never expose stack traces in production
- Log all exceptions with `ILogger`

### Frontend

- HTTP errors caught in service layer via `catchError`
- Display user-friendly error messages (not raw API errors)
- 401 responses: clear localStorage and redirect to `/auth/login`
- Network errors: show "Unable to connect to server" toast/alert

---

## CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader());
});
```

---

## EF Core Migrations

```bash
# Run from solution root
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/Persistence \
  --startup-project src/Presentation/Todo.API

dotnet ef database update \
  --project src/Infrastructure/Persistence \
  --startup-project src/Presentation/Todo.API
```

---

## Do Not

- Do NOT use `ViewBag`, `ViewData`, or Razor views anywhere
- Do NOT use `NgModules` — Angular 18 standalone components only
- Do NOT store passwords in plain text — always BCrypt hash
- Do NOT trust UserId from request body — always use JWT claims
- Do NOT hard-delete any task records — use soft delete
- Do NOT use `any` type in TypeScript — always use proper interfaces
- Do NOT add `console.log` debug statements in production code
- Do NOT use `HttpClient` directly in components — always use services
- Do NOT skip form validation on either frontend or backend
