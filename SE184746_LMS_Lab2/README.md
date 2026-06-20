# PRN232 LMS Lab 2 — SE184746

Learning Management System REST API  
**Stack**: ASP.NET Core 8 · Entity Framework Core · SQL Server · Docker  
**Architecture**: 3-layer (API / Services / Repositories)

---

## Quick Start — Docker Compose (recommended)

```powershell
docker compose up --build
```

| URL | Description |
|-----|-------------|
| `http://localhost:8080` | Swagger UI (root URL) |
| `http://localhost:8080/api/v1/auth/login` | Login endpoint |

Stop: `docker compose down`

---

## Quick Start — Local development

```powershell
# Ensure SQL Server is running on localhost,1433 (sa / 12345)
dotnet run --project PRN232.LMS.API
```

Swagger UI: `https://localhost:7xxx` (see console output)

---

## Default credentials (seeded on first run)

| Username | Password  | Role  |
|----------|-----------|-------|
| admin    | Admin@123 | Admin |
| user     | User@123  | User  |

---

## Project structure

| Project | Role |
|---------|------|
| `PRN232.LMS.API` | Controllers, middleware, Swagger, validators, AutoMapper (API layer) |
| `PRN232.LMS.Services` | Business logic, service interfaces/implementations, business models |
| `PRN232.LMS.Repositories` | EF Core DbContext, entities, migrations, repositories, seed data |

---

## API endpoints

All endpoints require a JWT Bearer token (except `POST /api/v1/auth/login`).

### Authentication

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/v1/auth/login` | None | Login – returns access token + refresh token |
| POST | `/api/v1/auth/refresh-token` | None | Exchange refresh token for a new token pair |

### Students (v1)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/students` | User | List students (search, sort, page, size, fields, expand) |
| GET | `/api/v1/students/{id}` | User | Get student by ID (supports `?expand=enrollments`) |
| POST | `/api/v1/students` | User | Create student (StudentCode optional) |
| PUT | `/api/v1/students/{id}` | User | Update student |
| DELETE | `/api/v1/students/{id}` | User | Delete student |

### Students (v2) — StudentCode required

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v2/students` | User | List students V2 (supports `X-Request-Id` header) |
| GET | `/api/v2/students/{id}` | User | Get student by ID V2 |
| POST | `/api/v2/students` | Admin | Create student (StudentCode **required**) |
| PUT | `/api/v2/students/{id}` | Admin | Update student (StudentCode **required**) |
| DELETE | `/api/v2/students/{id}` | Admin | Delete student |

### Enrollments

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/enrollments` | User | List enrollments (expand=student,course) |
| GET | `/api/v1/enrollments/{id}` | User | Get enrollment by ID |
| POST | `/api/v1/enrollments` | User | Create enrollment |
| PUT | `/api/v1/enrollments/{id}` | User | Update enrollment |
| PATCH | `/api/v1/enrollments/{id}/status` | User | Patch enrollment status only |
| DELETE | `/api/v1/enrollments/{id}` | User | Delete enrollment |

### Courses

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/courses` | User | List courses (expand=semester,subject,enrollments) |
| GET | `/api/v1/courses/{id}` | User | Get course by ID |
| GET | `/api/v1/courses/{id}/enrollments` | User | Get enrollments of a course (nested resource) |
| GET | `/api/v1/courses/{id}/students` | User | Get students of a course (nested resource) |
| POST | `/api/v1/courses` | User | Create course |
| PUT | `/api/v1/courses/{id}` | User | Update course |
| DELETE | `/api/v1/courses/{id}` | User | Delete course |

### Semesters

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/semesters` | User | List semesters (expand=courses) |
| GET | `/api/v1/semesters/{id}` | User | Get semester by ID |
| POST | `/api/v1/semesters` | User | Create semester |
| PUT | `/api/v1/semesters/{id}` | User | Update semester |
| DELETE | `/api/v1/semesters/{id}` | User | Delete semester |

### Subjects

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/subjects` | User | List subjects (expand=courses) |
| GET | `/api/v1/subjects/{id}` | User | Get subject by ID |
| POST | `/api/v1/subjects` | User | Create subject |
| PUT | `/api/v1/subjects/{id}` | User | Update subject |
| DELETE | `/api/v1/subjects/{id}` | User | Delete subject |

### Users (Admin only)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/v1/users` | Admin | List users |
| GET | `/api/v1/users/{id}` | Admin | Get user by ID |
| POST | `/api/v1/users` | Admin | Create user |
| PUT | `/api/v1/users/{id}` | Admin | Update user |
| DELETE | `/api/v1/users/{id}` | Admin | Delete user |

---

## List query parameters

All list endpoints support:

| Parameter | Description | Example |
|-----------|-------------|---------|
| `search` | Full-text filter | `?search=john` |
| `sort` | Sort field(s), prefix `-` for descending | `?sort=-dateOfBirth,fullName` |
| `page` | Page number (default 1) | `?page=2` |
| `size` | Page size 1–100 (default 10) | `?size=20` |
| `fields` | Comma-separated field selection | `?fields=studentId,fullName,email` |
| `expand` | Comma-separated related resources | `?expand=enrollments,semester` |

---

## Response format

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": { ... },
  "errors": null
}
```

### HTTP status codes

| Code | Usage |
|------|--------|
| 200 | Successful GET / PUT / PATCH |
| 201 | Successful POST |
| 204 | Successful DELETE |
| 400 | Validation error |
| 401 | Unauthenticated |
| 403 | Forbidden (insufficient role) |
| 404 | Resource not found |
| 406 | Not Acceptable (unsupported media type) |
| 500 | Unexpected server error |

---

## API Versioning

Versioning is supported via three mechanisms:
- **URL segment** (default): `/api/v1/...`, `/api/v2/...`
- **Header**: `X-Api-Version: 2.0`
- **Media type**: `Accept: application/json;x-api-version=2.0`

---

## Content negotiation

- `Accept: application/json` → JSON response (default)
- `Accept: application/xml` → XML response
- Other types → `406 Not Acceptable`

---

## Seed data

Automatically applied on first startup via `DatabaseInitializer`:

- 5 semesters
- 10 subjects  
- 50 students
- 20 courses
- 500 enrollments
- 2 default users (admin / user)
