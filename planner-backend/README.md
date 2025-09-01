# Todo List API - Clean Architecture Implementation

This is a modern .NET Core 8.0 Web API for a todo list application built following Clean Architecture principles with CQRS pattern.

## 🏗️ Architecture

The solution follows Clean Architecture with the following layers:

- **Domain Layer** (`TodoApp.Domain`): Core business entities and enums
- **Application Layer** (`TodoApp.Application`): Business logic, CQRS commands/queries, and interfaces
- **Infrastructure Layer** (`TodoApp.Infrastructure`): Data access, services, and external dependencies
- **API Layer** (`PlannerAPI`): Controllers and API configuration

## 🚀 Features

### ✅ Requirements Implemented

- **Clean Architecture**: Proper separation of concerns with dependency inversion
- **CQRS with MediatR**: Commands and queries separated using MediatR pattern
- **Entity Framework Core**: In-memory database for demo purposes
- **Authentication**: Simple JWT-based authentication middleware
- **Logging**: Structured logging with Serilog
- **In-Memory Caching**: Microsoft.Extensions.Caching.Memory
- **CORS**: Configured for frontend integration
- **Production-Ready Features**:
  - Error handling and validation
  - Proper HTTP status codes
  - Swagger/OpenAPI documentation
  - Health check endpoint

### 📋 API Endpoints

#### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

#### Todos
- `GET /api/todos` - Get all user's todos
- `GET /api/todos/{id}` - Get specific todo
- `POST /api/todos` - Create new todo
- `PUT /api/todos/{id}` - Update todo
- `DELETE /api/todos/{id}` - Soft delete todo

#### System
- `GET /api/health` - Health check

## 🛠️ Technology Stack

- **.NET 8.0** - Framework
- **Entity Framework Core 9.0** - ORM with In-Memory database
- **MediatR 13.0** - CQRS pattern implementation
- **Serilog 9.0** - Structured logging
- **AutoMapper 15.0** - Object-to-object mapping
- **FluentValidation 12.0** - Input validation
- **JWT Bearer Tokens** - Authentication
- **Swagger/OpenAPI** - API documentation

## 🔧 Setup & Running

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Navigate to the solution directory:
   ```bash
   cd planner-backend
   ```
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```
5. Run the API:
   ```bash
   cd PlannerAPI/PlannerAPI
   dotnet run
   ```

The API will be available at: `http://localhost:5247`

### Swagger Documentation
Visit `http://localhost:5247/swagger` for interactive API documentation.

## 📊 Database

Uses **Entity Framework Core In-Memory Database** for demonstration purposes. The database is seeded on application startup and data persists only during the application lifecycle.

### Entities
- **User**: Username, Email, Password (hashed), FirstName, LastName
- **Todo**: Title, Description, Status (Pending/InProgress/Completed/Cancelled), Priority (Low/Medium/High/Critical), DueDate

## 🔐 Authentication

Simple JWT-based authentication system:
- Registration creates new user with hashed password
- Login validates credentials and returns JWT token
- Protected endpoints require `Authorization: Bearer <token>` header
- Tokens expire after 24 hours

## 🧪 Testing Examples

### Register User
```bash
curl -X POST "http://localhost:5247/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com", 
    "password": "password123",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### Login
```bash
curl -X POST "http://localhost:5247/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "password123"
  }'
```

### Create Todo
```bash
curl -X POST "http://localhost:5247/api/todos" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Complete project",
    "description": "Finish the todo API implementation",
    "priority": 2,
    "dueDate": "2025-09-05T00:00:00Z"
  }'
```

## 🔮 Future Enhancements

### Scalability Considerations
1. **Database**: Replace in-memory DB with persistent storage (SQL Server, PostgreSQL)
2. **Caching**: Implement Redis for distributed caching
3. **Authentication**: Add OAuth 2.0, refresh tokens, password reset
4. **Validation**: Expand FluentValidation rules
5. **Performance**: Add pagination, sorting, filtering
6. **Security**: HTTPS enforcement, rate limiting, input sanitization
7. **Monitoring**: Application Insights, metrics, health checks
8. **Testing**: Unit tests, integration tests
9. **CI/CD**: Docker containerization, automated deployment
10. **Documentation**: Comprehensive API documentation

### Additional Features
- **Todo Categories/Labels**: Organize todos by category
- **Due Date Notifications**: Email/SMS reminders
- **File Attachments**: Allow file uploads for todos
- **Collaboration**: Share todos with other users
- **Search**: Full-text search capabilities
- **Analytics**: Usage statistics and reporting
- **Mobile API**: Optimized endpoints for mobile apps

## 🎯 Architecture Benefits

1. **Separation of Concerns**: Each layer has clear responsibilities
2. **Testability**: Business logic is isolated and easily testable
3. **Maintainability**: Clean code structure makes changes easier
4. **Scalability**: Architecture supports growth and additional features
5. **Flexibility**: Easy to swap implementations (e.g., database providers)

## 📝 Design Decisions

### Trade-offs Made
1. **Simple Authentication**: Used basic JWT instead of OAuth for demo simplicity
2. **In-Memory Database**: Chose convenience over persistence for demonstration
3. **Minimal Validation**: Basic validation implemented, could be expanded
4. **Exception Handling**: Global error handling at controller level

### Assumptions
1. Single-tenant application (no multi-tenancy)
2. Users can only access their own todos
3. Soft delete for todos (not permanent deletion)
4. UTC timestamps for all date operations
5. Simple password hashing (production would use bcrypt/Argon2)

This implementation demonstrates modern .NET development practices with clean architecture, making it ready for production with additional security and performance enhancements.