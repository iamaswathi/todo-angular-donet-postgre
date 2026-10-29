# Todo App (.NET + PostgreSQL + Angular)

Tested layout for Apple Silicon (arm64). Needs: .NET SDK 10, Node 20+/22, Docker Desktop.

## Run it
```bash
# 1. Database (native arm64 Postgres)
docker compose up -d db

# 2. Backend  ->  http://localhost:5080
cd backend/TodoApi && dotnet run

# 3. Frontend (new terminal, from repo root)
./setup-frontend.sh            # one-time: generates Angular project + applies our code
cd frontend && npm start       # -> http://localhost:4200
```

## Run backend tests
```bash
dotnet test backend/TodoApi.Tests
```

## API
GET/POST `/api/todos`, GET/PUT/DELETE `/api/todos/{id}`, GET `/health`
