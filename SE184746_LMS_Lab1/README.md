# PRN232 LMS Lab 1 — SE184746

Learning Management System REST API (.NET 8, 3-layer architecture, SQL Server, Docker).

## Structure

| Project | Role |
|---------|------|
| `PRN232.LMS.API` | Controllers, Swagger, middleware |
| `PRN232.LMS.Services` | Business logic, models, mappers |
| `PRN232.LMS.Repositories` | EF Core, entities, migrations, `DatabaseInitializer`, seed SQL |

## Run with Docker Compose (lab deployment)

```powershell
docker compose up --build
```

- Swagger: http://localhost:8080/swagger
- API: http://localhost:8080/api/students?page=1&size=10

Stop: `docker compose down`

## Run locally

```powershell
dotnet run --project PRN232.LMS.API
```

Update `PRN232.LMS.API/appsettings.json` if SQL Server is not on `localhost,1433`.

## API endpoints

| Method | Endpoint |
|--------|----------|
| GET | `/api/students`, `/api/students/{id}` |
| POST | `/api/students` |
| PUT | `/api/students/{id}` |
| DELETE | `/api/students/{id}` |
| GET | `/api/enrollments`, `/api/enrollments/{id}` |
| POST | `/api/enrollments` |
| PUT | `/api/enrollments/{id}` |
| DELETE | `/api/enrollments/{id}` |
| GET | `/api/semesters`, `/api/semesters/{id}` |
| POST | `/api/semesters` |
| PUT | `/api/semesters/{id}` |
| DELETE | `/api/semesters/{id}` |
| GET | `/api/subjects`, `/api/subjects/{id}` |
| POST | `/api/subjects` |
| PUT | `/api/subjects/{id}` |
| DELETE | `/api/subjects/{id}` |
| GET | `/api/courses`, `/api/courses/{id}` |
| GET | `/api/courses/{id}/enrollments` (enrollments of a course; supports search, sort, page, size, fields, expand) |
| POST | `/api/courses` |
| PUT | `/api/courses/{id}` |
| DELETE | `/api/courses/{id}` |

### List query parameters

`search`, `sort`, `page`, `size`, `fields`, `expand`

Example:

```http
GET /api/enrollments?search=active&sort=-enrollDate&page=1&size=20&fields=enrollmentId,status&expand=student,course
```

### Response wrapper

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {},
  "errors": null
}
```

### HTTP status codes

| Code | Usage |
|------|--------|
| 200 | Successful GET, PUT, DELETE |
| 201 | Successful POST |
| 400 | Validation error |
| 404 | Resource not found |
| 500 | Unexpected server error |

## Seed data

5 semesters, 50 students, 10 subjects, 20 courses, 500 enrollments (on first run via `Repositories/Data/DatabaseInitializer`).
