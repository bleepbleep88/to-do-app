# Todo App

A full-stack todo application built with ASP.NET Core Web API backend and Next.js React frontend.

## Features

- User authentication with JWT tokens
- Create, read, update, delete todos
- Subtask management
- Drag-and-drop reordering
- Responsive design with Tailwind CSS
- Real-time optimistic updates

## Quick Start with Docker

### Prerequisites
- Docker and Docker Compose installed

### Run the Application

```bash
# Clone the repository and navigate to the root directory
cd to-do-app

# Start both API and frontend
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

The application will be available at:
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **API Swagger**: http://localhost:5000/swagger

### Stopping the Application

```bash
docker-compose down
```

## Development Setup

### Backend (ASP.NET Core)
```bash
cd planner-backend/TodoApp.API
dotnet restore
dotnet run
```

### Frontend (Next.js)
```bash
cd planner-frontend/my-app
npm install
npm run dev
```

## Project Structure

```
├── planner-backend/          # ASP.NET Core Web API
│   ├── TodoApp.API/         # API Controllers and startup
│   ├── TodoApp.Application/ # Business logic and CQRS
│   ├── TodoApp.Domain/      # Domain entities and enums
│   └── TodoApp.Infrastructure/ # Data access and services
├── planner-frontend/my-app/ # Next.js React frontend
├── docker-compose.yml       # Docker orchestration
└── README.md
```

## API Endpoints

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login  
- `GET /api/todos` - Get user's todos
- `POST /api/todos` - Create new todo
- `PUT /api/todos/{id}` - Update todo
- `DELETE /api/todos/{id}` - Delete todo
- `POST /api/todos/reorder` - Reorder todos
- `POST /api/todos/{todoId}/subtasks` - Create subtask
- `PUT /api/todos/{todoId}/subtasks/{id}` - Update subtask
- `DELETE /api/todos/{todoId}/subtasks/{id}` - Delete subtask
- `POST /api/todos/{todoId}/subtasks/reorder` - Reorder subtasks

## Technologies

**Backend:**
- ASP.NET Core 8.0
- Entity Framework Core (In-Memory)
- MediatR (CQRS pattern)
- JWT Authentication
- Serilog
- AutoMapper

**Frontend:**
- Next.js 15
- React 18
- TypeScript
- Tailwind CSS
- Redux Toolkit
- RTK Query
- @dnd-kit (drag and drop)

## License

MIT License