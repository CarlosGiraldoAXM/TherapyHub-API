# Project Rules – .NET API Architecture

These rules are MANDATORY.
All generated or modified code MUST strictly follow this architecture.

---

## 1. General Architecture

- ALWAYS use a layered architecture
- Controllers MUST be thin
- Controllers MUST NOT contain business logic
- Controllers MUST NOT access DbContext directly
- ALL business logic MUST live in Services
- ALL data access MUST be done through Repositories
- Entity Framework DbContext MUST ONLY be used inside repositories
- Use async/await everywhere
- NEVER block async code (.Result, .Wait)

---

## 2. Folder Structure (MANDATORY)

The project MUST follow this structure:

Controllers/
DTOs/
├─ Common/
├─ Requests/
│ └─ {Feature}/
└─ Responses/
└─ {Feature}/
Models/
Services/
├─ IServices/
└─ Implementations/
Repositorio/
├─ IRepositorio/
├─ UnitOfWork.cs
└─ Repository.cs
Validators/
AutoMapper/
Configuration/


DO NOT create folders outside this structure without justification.

---

## 3. DTO Rules

### 3.1 DTO Organization

- DTOs MUST be separated by responsibility:
  - Requests → input models
  - Responses → output models
  - Common → shared DTOs
- NEVER reuse Entity models as DTOs
- NEVER expose database entities in API responses

### 3.2 API Response Standard

- ALL controller responses MUST use `ApiResponse<T>`
- Controllers MUST NOT return raw objects
- Use:
  - SuccessResponse
  - ErrorResponse
  - NotFoundResponse
- ApiResponse MUST include:
  - Success
  - Message
  - Data
  - Errors
  - Timestamp
  - StatusCode

---

## 4. Services (Domain Services)

- Each feature MUST have:
  - One interface in `Services/IServices`
  - One implementation in `Services/Implementations`
- Services MUST:
  - Contain ALL business rules
  - Coordinate multiple repositories if needed
  - Handle domain validations
- Services MUST NOT:
  - Use DbContext directly
  - Return entities
- Services MUST return DTOs only

---

## 5. Repository Pattern

### 5.1 Generic Repository

- ALL repositories MUST inherit from `Repository<T>`
- `Repository<T>` MUST provide:
  - CRUD operations
  - Async methods
  - Pagination support
- DO NOT duplicate generic CRUD logic in feature repositories

### 5.2 Feature Repositories

- Feature repositories MUST:
  - Inherit from `IRepository<T>`
  - Implement only feature-specific queries
- Naming convention:
  - Interface: `I{Feature}Repositorio`
  - Implementation: `{Feature}Repositorio`

---

## 6. Unit of Work

- ALL write operations spanning multiple repositories MUST use UnitOfWork
- UnitOfWork MUST:
  - Manage transactions
  - Expose repositories via properties
  - Control SaveChanges
- Services MUST:
  - Begin transactions explicitly when needed
  - Commit on success
  - Rollback on failure
- Controllers MUST NOT use UnitOfWork

---

## 7. Entity Framework Rules

- Use EF Core only through repositories
- Prefer LINQ over raw SQL
- Raw SQL is FORBIDDEN unless strictly necessary
- Always paginate large queries
- Use soft delete when applicable
- Avoid lazy loading unless explicitly required

---

## 8. Validation (FluentValidation)

- ALL request DTOs MUST be validated using FluentValidation
- DataAnnotations are NOT allowed for business validation
- Validators MUST:
  - Live in `Validators/{Feature}`
  - Validate business rules and data consistency
- Controllers MUST NOT manually validate models
- Validation errors MUST be returned using ApiResponse

---

## 9. AutoMapper

- ALL entity ↔ DTO mapping MUST use AutoMapper
- Mapping logic MUST NOT be done manually in services
- Each feature MUST have its own Profile
- Profiles MUST live in `AutoMapper/`

---

## 10. Exception Handling

- Services MUST throw exceptions for business errors
- Controllers MUST:
  - Catch exceptions
  - Log errors
  - Return standardized ApiResponse errors
- Do NOT swallow exceptions silently
- Logging is MANDATORY for:
  - Errors
  - Warnings
  - Important business events

---

## 11. Controllers Rules

- Controllers MUST:
  - Receive DTOs
  - Call Services
  - Return ApiResponse<T>
- Controllers MUST NOT:
  - Contain business logic
  - Use repositories directly
- Controllers SHOULD:
  - Be secured using Authorization attributes
  - Declare ProducesResponseType

---

## 12. Naming Conventions

- Async methods MUST end with `Async`
- Interfaces MUST start with `I`
- DTOs MUST end with:
  - RequestDto
  - ResponseDto
- Services MUST end with `Service`
- Repositories MUST end with `Repositorio`

---

## 13. Logging

- Services MUST use ILogger<T>
- Log:
  - Entry and exit of important operations
  - Warnings for business validations
  - Errors with full exception details

---

## 14. Golden Rule

If there is doubt:
- FOLLOW EXISTING PROJECT PATTERNS
- DO NOT introduce new architectural styles
- CONSISTENCY is more important than creativity



