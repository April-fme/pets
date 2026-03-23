# Pets Project - Copilot Instructions

## 🎯 Project Overview
**Pet Health Monitoring System** - A comprehensive platform for managing pet health data, monitoring vital signs from IoT collars, recording feeding/waste logs, managing veterinary appointments, and generating intelligent health alerts.

## 🏗️ Project Structure
```
pets/
├── frontend/                 # React.js application (localhost:3000)
│   ├── src/
│   │   ├── components/       # React components (ActivityChart, ActivityChartAPI)
│   │   ├── services/         # API services (api.js, petService.js)
│   │   ├── App.js            # Main app component
│   │   └── index.js          # React entry point
│   └── package.json
├── PetsAPI/                  # ASP.NET Core Web API (.NET 10)
│   ├── Controllers/          # API endpoints (Pets, CollarData, DailyLogs, Alerts, Appointments)
│   ├── Models/               # Entity models
│   ├── Services/             # Business logic (AlertService, CollarSimulatorService)
│   ├── Migrations/           # EF Core database migrations
│   ├── Program.cs            # App configuration & startup
│   ├── appsettings.json      # Production settings
│   └── appsettings.Development.json  # Development settings
├── pets.sln                  # Solution file
└── ReadMe.md                 # Project requirements
```

## 🛠️ Tech Stack
- **Frontend**: React 19.2.4, Chart.js 4.5.1, Axios, TailwindCSS
- **Backend**: ASP.NET Core (.NET 10), Entity Framework Core
- **Database**: Microsoft SQL Server (PetHealthDB on 192.168.207.52)
- **Authentication**: JWT (planned)
- **Visualization**: chart.js for trend charts

## 📊 Database Configuration
- **Server**: 192.168.207.52
- **Database**: PetHealthDB
- **Credentials**: reyi / ReyifmE
- **Connection String**: Already configured in appsettings.json & appsettings.Development.json

### Core Tables
- **Pets**: Pet basic info (ID, Name, Species, Birthday, WeightGoal)
- **CollarData**: IoT collar sensor data (Temperature, HeartRate, ActivityLevel, SleepQuality)
- **DailyLogs**: Feeding & waste records (FoodAmount, WaterIntake, StoolStatus, UrineStatus)
- **Appointments**: Veterinary appointments (ClinicName, VetName, AppointmentDate, Reason, Status)
- **Alerts**: Health anomaly alerts (Type, Severity, Message, IsResolved)

## 🚀 Building & Running

### Backend (ASP.NET Core API)
```bash
cd PetsAPI

# Restore dependencies
dotnet restore

# Apply database migrations (ensure DB is up-to-date)
dotnet ef database update

# Run the API server
dotnet run
# Runs on: https://localhost:5001 or http://localhost:5000
```

### Frontend (React)
```bash
cd frontend

# Install dependencies (if needed)
npm install

# Start development server
npm start
# Runs on: http://localhost:3000

# Build for production
npm build
```

## 🔌 API Configuration
- **Frontend API Base URL**: http://localhost:3001 (configured in api.js)
- **CORS**: Configured in Program.cs to allow localhost:3000
- **Response Format**: camelCase (configured via JsonSerializerOptions)

## 👨‍💻 Development Conventions

### Code Style
- Controllers: RESTful design, return appropriate HTTP status codes
- Services: Encapsulate business logic, implement interfaces (IAlertService)
- Frontend: Use functional components with hooks
- Component naming: PascalCase for components, camelCase for functions/variables

### Database Changes
- Use Entity Framework Core migrations: `dotnet ef migrations add <MigrationName>`
- Always apply migrations before running: `dotnet ef database update`
- Connection strings in appsettings files should use parameterized queries

### API Changes
- Keep controllers focused on HTTP handling
- Move business logic to Services layer
- Update ActivityChartAPI.js or api.js when adding new API endpoints
- Use axios for API calls in frontend

### Security
- Token validation middleware for protected endpoints
- SQL injection prevention: No raw SQL queries (use EF Core)
- CORS validation in Program.cs

## 📝 Common Commands Reference

| Command | Purpose |
|---------|---------|
| `dotnet build` | Compile C# backend |
| `dotnet run` | Run backend API |
| `dotnet ef migrations add {name}` | Create new DB migration |
| `dotnet ef database update` | Apply pending migrations |
| `npm start` | Run frontend dev server |
| `npm build` | Build frontend for production |

## 🔄 Data Flow Pattern
1. **Frontend** (React) → calls API via axios (api.js/petService.js)
2. **API** (Controllers) → delegates to Services for business logic
3. **Services** → queries database via Entity Framework (DbContext)
4. **Database** (SQL Server) → returns data
5. **API** → response serialized to camelCase JSON
6. **Frontend** → displays data using Chart.js or React components

## 📌 Key Files to Know
- [PetsController.cs](../PetsAPI/Controllers/PetsController.cs) - Pet management API
- [ActivityChartAPI.js](../frontend/src/components/ActivityChartAPI.js) - Activity chart with real DB data
- [PetsDbContext.cs](../PetsAPI/Data/PetsDbContext.cs) - EF Core DbContext
- [appsettings.Development.json](../PetsAPI/appsettings.Development.json) - Dev DB config
- [api.js](../frontend/src/services/api.js) - Frontend API service

## ⚠️ Important Notes
- Always ensure database migrations are applied before running the backend
- Frontend runs on port 3000, backend API expected on port 3001
- Database connection uses SQL authentication (reyi account)
- Chart.js is configured for real-time data visualization
- Test all API endpoints with mock data disabled to verify DB integration
