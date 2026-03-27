# VillaBooking

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-Deployed-0078D4?logo=microsoftazure&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)

A scalable, production-grade RESTful API and MVC web application for vacation rental property management. Designed with real-world architecture patterns, secure JWT authentication, versioned endpoints, and automated CI/CD pipelines deploying directly to Azure.

### Live Demo

| Application | URL |
|-------------|-----|
| API | [villabooking-api.azurewebsites.net](https://villabooking-api.azurewebsites.net) |
| Web App | [villabooking-app.azurewebsites.net](https://villabooking-app.azurewebsites.net) |

---

## Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Features](#-features)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Deployment](#-deployment)
- [Future Improvements](#-future-improvements)
- [Author](#-author)

---

## 📋 Overview

VillaBooking addresses the core challenges of vacation rental property management:

- **Versioned REST API** with advanced filtering, pagination, and sorting capabilities
- **MVC web application** consuming the API for streamlined administrative workflows
- **Secure authentication** with JWT tokens and role-based access control
- **Image management** with validation and cloud-ready storage

Built to handle production workloads with maintainable, testable code.

---

## 🛠 Tech Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET 10.0 |
| **API** | ASP.NET Core Web API |
| **Web UI** | ASP.NET Core MVC |
| **ORM** | Entity Framework Core 10.0 |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Core Identity + JWT Bearer |
| **Object Mapping** | AutoMapper |
| **API Versioning** | Asp.Versioning.Mvc |
| **API Documentation** | Scalar (OpenAPI) |
| **CI/CD** | GitHub Actions |
| **Hosting** | Azure Web Apps |

---

## ✨ Features

### API
- **RESTful endpoints** with full CRUD operations for villas and amenities
- **API versioning** (v1 for core operations, v2 for advanced querying)
- **JWT authentication** with configurable token expiration
- **Role-based authorization** (Admin, Customer)
- **Image upload** with type and size validation (JPG, PNG, max 5MB)
- **Pagination headers** for client-side navigation
- **Standardized responses** with consistent error handling

### Web Application
- **Property management dashboard** with complete CRUD workflows
- **Secure authentication** with session-based token management
- **AJAX-powered pagination** for seamless data browsing
- **Image preview and upload** with client-side validation
- **Responsive layout** built with Bootstrap

---

## 🏗 Architecture

This solution implements an **N-Tier Layered Architecture** with strict separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    VillaBooking.Web                         │
│                  (MVC - Presentation)                       │
│         Controllers → Services → API Client                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ HTTP/JWT
┌─────────────────────────────────────────────────────────────┐
│                    VillaBooking.API                         │
│              (REST API - Business Logic)                    │
│         Controllers → Services → DbContext                  │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      SQL Server                             │
│                      (Database)                             │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    VillaBooking.DTO                         │
│            (Shared Data Transfer Objects)                   │
└─────────────────────────────────────────────────────────────┘
```

### Why This Architecture?

- **Independent scaling** — API and Web tiers deploy and scale separately
- **Shared contracts** — DTOs enforce consistency across projects
- **Testable services** — Business logic decoupled from HTTP layer
- **API-first design** — Ready for mobile apps or third-party consumers

---

## 📁 Project Structure

```
VillaBooking/
├── .github/workflows/          # CI/CD pipelines for Azure deployment
│
├── VillaBooking.API/           # REST API project
│   ├── Controllers/            # API endpoints (v1 and v2)
│   ├── Data/
│   │   ├── Contexts/           # EF Core DbContext
│   │   ├── Configurations/     # Entity type configurations
│   │   └── Migrations/         # Database migrations
│   ├── Models/                 # Domain entities
│   ├── Services/               # Business logic (Auth, Image)
│   └── Profiles/               # AutoMapper mappings
│
├── VillaBooking.DTO/           # Shared data transfer objects
│   ├── Auth/                   # Login, Register, Token DTOs
│   ├── Villa/                  # Villa DTOs
│   ├── VillaAmenity/           # Amenity DTOs
│   └── Responses/              # APIResponse wrapper
│
├── VillaBooking.Web/           # MVC web application
│   ├── Controllers/            # MVC controllers
│   ├── Views/                  # Razor views
│   ├── Services/               # API client services
│   ├── Models/                 # View models
│   └── Extensions/             # Helper extensions
│
└── VillaBooking.slnx           # Solution file
```

### Key Design Decisions

| Folder | Purpose |
|--------|---------|
| `VillaBooking.DTO` | Isolated DTO project prevents circular dependencies between API and Web |
| `Controllers/v1` vs `v2` | URL-based versioning maintains backward compatibility |
| `Services/` | Interface-driven services enable DI and unit testing |
| `Data/Configurations/` | Fluent API keeps entity classes focused on domain logic |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full instance)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/VillaBooking.git
   cd VillaBooking
   ```

2. **Configure the database connection**

   Update `appsettings.json` in both `VillaBooking.API` and `VillaBooking.Web`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VillaBooking;Trusted_Connection=True;"
     }
   }
   ```

3. **Configure JWT settings** (API only)
   ```json
   {
     "JwtSettings": {
       "Secret": "your-256-bit-secret-key-here"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   cd VillaBooking.API
   dotnet ef database update
   ```

5. **Start the API**
   ```bash
   dotnet run --project VillaBooking.API
   ```

6. **Start the Web application** (in a new terminal)
   ```bash
   dotnet run --project VillaBooking.Web
   ```

### Default URLs

| Application | URL |
|-------------|-----|
| API | `https://localhost:7071` |
| API Documentation | `https://localhost:7071/scalar` |
| Web Application | `https://localhost:7010` |

---

## 📡 API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Authenticate and receive JWT |

### Villas (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/v2/villa` | List villas with filtering, sorting, pagination |
| `GET` | `/api/v2/villa/{id}` | Retrieve villa by ID |
| `POST` | `/api/v2/villa` | Create villa (multipart/form-data) |
| `PUT` | `/api/v2/villa/{id}` | Update villa |
| `DELETE` | `/api/v2/villa/{id}` | Delete villa |

**Query Parameters (GET /api/v2/villa):**

| Parameter | Type | Description |
|-----------|------|-------------|
| `name` | string | Filter by name (contains) |
| `minOccupancy` | int | Minimum occupancy |
| `maxOccupancy` | int | Maximum occupancy |
| `minRate` | double | Minimum rate |
| `maxRate` | double | Maximum rate |
| `sortBy` | string | Field to sort by |
| `sortOrder` | string | `asc` or `desc` |
| `page` | int | Page number (default: 1) |
| `pageSize` | int | Items per page (default: 10) |

### Villa Amenities (v2)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/v2/villa-amenities` | List all amenities |
| `GET` | `/api/v2/villa-amenities/{id}` | Retrieve amenity by ID |
| `POST` | `/api/v2/villa-amenities` | Create amenity |
| `PUT` | `/api/v2/villa-amenities/{id}` | Update amenity |
| `DELETE` | `/api/v2/villa-amenities/{id}` | Delete amenity |

### Response Format

All endpoints return a consistent structure:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Villas retrieved successfully",
  "data": [...],
  "errors": null,
  "timestamp": "2026-03-27T10:30:00Z"
}
```

---

## ☁️ Deployment

Automated CI/CD via GitHub Actions deploys to Azure on every push to `master`:

- **API** → `villabooking-api.azurewebsites.net`
- **Web** → `villabooking-app.azurewebsites.net`

Pipelines use Azure OIDC for secure, keyless authentication.

---

## 🔮 Future Improvements

- **Booking engine** — Reservation system with availability calendar and conflict detection
- **Payment processing** — Stripe integration for secure guest transactions
- **Distributed caching** — Redis layer to reduce database load on high-traffic endpoints
- **Test suite** — Unit and integration tests with code coverage reporting
- **Rate limiting & throttling** — Protect API from abuse and ensure fair usage
- **Transactional emails** — Booking confirmations, reminders, and cancellation notices via SendGrid
- **Analytics dashboard** — Occupancy rates, revenue tracking, and booking trends

---

## 👤 Author

**Beshoy Gamal**
.NET Backend Developer

[![GitHub](https://img.shields.io/badge/GitHub-Profile-181717?logo=github&logoColor=white)](https://github.com/beshoy-gamal-waheb)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-0A66C2?logo=linkedin&logoColor=white)](https://linkedin.com/in/beshoy-gamal)
[![Website](https://img.shields.io/badge/Website-Portfolio-0EA5E9?logo=google-chrome&logoColor=white)](https://beshoygamal.dev)

---

## 📄 License

This project is licensed under the MIT License.
