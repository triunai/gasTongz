# ⚡️ GasTongz Backend



Welcome to GasTongz, a POS & Inventory Management System designed for handling gas tongs (or similar products) across multiple shops in Malaysia. This repository contains the backend services, built with ASP.NET Core, MSSQL, and Dapper, following Clean Architecture principles and incorporating Domain-Driven Design (DDD) concepts.

  

## ✨ Overview



- **Purpose**: Manage gas cylinder inventory by shop, handle sales (cash & QR), generate reports, and easily scale as the business grows.
- **Tech Stack**:
    - **ASP.NET Core (C#)** for API and application layer
    - **Microsoft SQL Server (MSSQL)** for database
    - **Dapper** for lightweight data access (and future EF Core integration)
    - **Clean Architecture** + **Domain-Driven Design (DDD)** to ensure maintainability and separation of concerns

  

## 🚀 Key Features



1. **Multi-Shop Inventory**  
    Track how many units of each product (e.g., Gas Tongs) exist in each shop.
2. **Sales & POS**
    - Supports **cash** and **QR-based** (Touch 'n Go, DuitNow) payments
    - Receipt image upload for manual OCR
    - Auto-deduction of inventory after every sale
3. **Reporting**
    - Generate daily, weekly, and monthly sales reports
    - Payment breakdown (cash vs. QR)
    - Low-stock alerts (configurable thresholds)
4. **User Roles**
    - **Admin**: Full control over CRUD operations and financial reports
    - **Viewer**: (Future) Read-only access 🏗️ Architecture The backend follows a vertical slice implementation within a Clean Architecture framework. This ensures that features remain modular, scalable, and maintainable.

```shell
src/
├── Domain/
│   ├── Entities/         # Core domain objects (e.g., Shop, Product, Inventory)
│   ├── ValueObjects/     # Immutable types (e.g., Receipt, PaymentMethod)
│   ├── Enums/            # Enumerations (e.g., PaymentStatus, TongStatus)
│   └── Events/           # Domain events (if event-driven DDD is applied)
├── Application/
│   ├── DTOs/             # Data Transfer Objects (request/response models)
│   ├── Interfaces/       # Abstractions (e.g., IInventoryService)
├── Infrastructure/
│   ├── Repositories/
│   ├── Commands/         # CQRS commands with FluentVal (e.g., ProcessSaleCommand)
│   ├── Queries/          # CQRS queries with FluentVal(e.g., GenerateReportQuery)    Dapper-based implementations (e.g.,         InventoryRepository)
│   ├── ExternalServices/ # OCR and payment integration placeholders
│   ├── Persistence/      # EF Core contexts (if added)
│   └── Logging/          # Serilog or other logging configurations
├── API/
│   ├── Controllers/      # ASP.NET Core API controllers
│   ├── Filters/          # Exception handling, request filters
│   └── Startup/          # Program.cs and appsettings configuration
└── Shared/
    ├── Helpers/          # Utility classes (e.g., common data processing)
    ├── Security/         # Security-related utilities
    └── Services/         # Cross-cutting concerns (e.g., health checks, virus scans)
Note: Folder structure is modular to accommodate additional features and third-party integrations.

```

  

## 📂 Repository Setup


We maintain **two separate repositories**:

1. **Frontend**
    - Holds the UI code (React or Angular).
    - Communicates with this backend via REST APIs.
2. **Backend** (`gasTongz`)
    - ASP.NET Core + MSSQL + Dapper
    - **Clean Architecture** & **DDD** structure

  

## 📝 Getting Started



### 1. Clone the Repo



```shell
git clone https://github.com/triunai/gasTongz.git
cd gasTongz
```

### 2. Configure Database



1. Install or have access to SQL Server (local or remote).
2. Create a new database, e.g. GasTongzDB.
3. In the appsettings.json (or environment variables), set your MSSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=GasTongzDB;User Id=sa;Password=YOUR_SECRET;"
  }
}

```

### 3. Apply Migrations or Run SQL Script



1. If purely using Dapper, run the schema script (e.g., gasTongz-schema.sql) in your SQL client (SSMS, Azure Data Studio).
2. If using EF Core, generate and run migrations:

```shell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the Application



```shell
dotnet build
dotnet run
```

Default URL: [https://localhost:5001](https://localhost:5001/) or [http://localhost:5000](http://localhost:5000/)

or

Visit /swagger if you have Swagger set up, or test endpoints via Postman

  

## 🌐 API Endpoints (Sample)



- GET /api/shops
    - Returns a list of shops.
- POST /api/inventory
    - Adds or updates inventory for a specific shop and product.
- POST /api/sales
    - Processes a new sale, adjusts stock, and logs the transaction. (Complete endpoint list to be detailed in future docs or Swagger.)

  

## 🎨 Frontend Repo



- Name: gasTongz-frontend
- Stack: React or Angular (recommend one for maintainability)
- Structure:

```shell
src/
  ├── components/
  ├── pages/
  ├── services/
  └── ...
```

  

## ☑️ Roadmap



OCR Integration: Tesseract or IronOCR for receipt image text extraction. Payment Gateway: Real Touch 'n Go / DuitNow integration. Reporting Enhancements: Charts, exporting to PDF/Excel, etc. RBAC & Security: Expanded roles, encryption, and fine-grained permissions.  

## 🤝 Contributing



Fork this repo & create a branch for your feature. Submit a Pull Request with descriptive commit messages. Code reviews focus on maintainability, test coverage, and domain consistency.  

## 📄 License



Licensed under the MIT License. Dont play2