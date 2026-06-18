# LKE Web API — Like-Kind Exchange Platform

A production ASP.NET Web API (MVC 5 / .NET Framework 4.5.2) backend for managing **Like-Kind Exchange (1031 Exchange)** transactions, asset tracking, financial reporting, and user administration. Deployed to **Microsoft Azure App Service** with **Azure SQL Database** and **Azure Blob Storage**.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Deployment](#deployment)

---

## Project Overview

The LKE (Like-Kind Exchange) platform enables tax advisors and clients to manage IRS Section 1031 property exchange transactions. It handles:

- Tracking relinquished and replacement properties within 45-day and 180-day IRS deadlines
- Generating financial reports (gain recognition, depreciation, sales/purchase summaries)
- Role-based user access for administrators, clients, and read-only users
- Excel report generation and archival to Azure Blob Storage

---

## Technology Stack

| Layer | Technology |
|---|---|
| API Framework | ASP.NET Web API 2 (MVC 5) |
| Language | C# / .NET Framework 4.5.2 |
| Authentication | ASP.NET Identity + OAuth 2.0 (OWIN) |
| Data Access | Entity Framework 6 (Database-First) |
| Database | Azure SQL Database (SQL Server) |
| File Storage | Azure Blob Storage |
| Report Generation | EPPlus (Excel .xlsx) + DotNetZip |
| Logging | log4net |
| Cloud Platform | Microsoft Azure (App Service) |
| Dependency Management | NuGet |

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Client Applications                 │
│         (Angular SPA on Azure Static Hosting)        │
└──────────────────────┬──────────────────────────────┘
                       │ HTTPS / REST JSON
┌──────────────────────▼──────────────────────────────┐
│              LKEWebAPI  (ASP.NET Web API 2)          │
│                                                      │
│  Controllers      Models          Providers          │
│  ├─ Administration  ├─ UserLogin    └─ OAuth (OWIN)  │
│  ├─ UserLogin       ├─ Reports                       │
│  ├─ Reports         ├─ AssetMaint                    │
│  ├─ AssetMaint      └─ Identity                      │
│  ├─ Pinnacle                                         │
│  └─ 45DayID                                          │
└────────────┬────────────────────┬───────────────────┘
             │ Entity Framework 6 │ Direct ADO.NET
┌────────────▼──────────┐  ┌──────▼──────────────────┐
│     LKE_DAL           │  │   Azure Blob Storage     │
│  (Data Access Layer)  │  │  (Report file archive)   │
│                       │  └─────────────────────────┘
│  Repositories         │
│  ├─ RAssetMaintance   │
│  ├─ RDMEntity         │
│  └─ RTransImports     │
│                       │
│  Utilities            │
│  ├─ BlobUtilities     │
│  ├─ SqlConnectionFact │
│  └─ DataTypesUtil     │
└──────────┬────────────┘
           │
┌──────────▼────────────┐
│   Azure SQL Database  │
│  ├─ LKE_Login (auth)  │
│  └─ LKE_Reports (data)│
└───────────────────────┘
```

---

## Project Structure

```
LKE-Rebuild/
│
├── LKEWebAPI.sln                        # Visual Studio solution
│
├── LKEWebAPI/                           # Web API project
│   ├── App_Start/
│   │   ├── WebApiConfig.cs              # Route & CORS configuration
│   │   ├── IdentityConfig.cs            # ASP.NET Identity setup
│   │   ├── Startup.Auth.cs              # OWIN OAuth pipeline
│   │   └── BundleConfig.cs
│   │
│   ├── Controllers/
│   │   ├── AdministrationController.cs  # User management (CRUD, roles, email)
│   │   ├── UserLoginController.cs       # Authentication & session handling
│   │   ├── ReportsController.cs         # Excel report generation & Blob upload
│   │   ├── AssetMaintananceController.cs# Asset/property data management
│   │   ├── PinnacleController.cs        # Pinnacle data integration
│   │   ├── _45DayIDController.cs        # 45-day IRS deadline tracking
│   │   ├── AccountController.cs         # ASP.NET Identity account actions
│   │   └── HomeController.cs
│   │
│   ├── Models/
│   │   ├── UserLogin.cs                 # Login request/response models
│   │   ├── ReportsGenerator.cs          # Report request models
│   │   ├── AssetMaintananceModel.cs     # Asset data models
│   │   ├── IdentityModels.cs            # EF Identity user models
│   │   └── LoginCredentials.cs
│   │
│   ├── Providers/
│   │   └── ApplicationOAuthProvider.cs  # Custom OAuth token provider
│   │
│   ├── Results/
│   │   └── ChallengeResult.cs           # Authentication challenge helper
│   │
│   ├── Resources/                       # Excel report templates (.xlsx)
│   │   ├── SalesTransactionReport_Template.xlsx
│   │   ├── GainRecognizedReport_Template.xlsx
│   │   ├── DepreciationReport_Template.xlsx
│   │   └── ...
│   │
│   ├── Areas/HelpPage/                  # Auto-generated API documentation
│   ├── Startup.cs                       # OWIN startup entry point
│   ├── Global.asax.cs                   # Application lifecycle hooks
│   └── Web.config                       # App configuration (no real credentials)
│
├── LKE_DAL/                             # Data Access Layer (class library)
│   ├── Repositories/
│   │   ├── RepositoryBase.cs            # Generic repository pattern base
│   │   ├── RAssetMaintance.cs           # Asset maintenance queries
│   │   ├── RDMEntity.cs                 # Entity/company data queries
│   │   └── RTransImports.cs             # Transaction import queries
│   │
│   ├── Models/
│   │   ├── WebUserModel.cs              # User view models
│   │   ├── ReportDataSourceModel.cs     # Report data binding models
│   │   ├── 45DayIDDeadlineModel.cs      # IRS 45-day deadline model
│   │   ├── AssetMaintananceModel.cs
│   │   └── ...
│   │
│   ├── Utilities/
│   │   ├── BlobUtilities.cs             # Azure Blob Storage helpers
│   │   ├── SqlConnectionFactory.cs      # IDbConnection factory
│   │   ├── DataTypesUtilities.cs        # Type conversion helpers
│   │   └── User.cs                      # User session utilities
│   │
│   ├── Logging/
│   │   └── Logger.cs                    # log4net wrapper
│   │
│   ├── Partial Classes/
│   │   └── ChartDetail.cs               # EF entity partial class extensions
│   │
│   ├── LKE_LoginDataModel.edmx          # Entity Framework data model (EDMX)
│   └── App.Config                       # DAL configuration (no real credentials)
│
└── .gitignore
```

---

## Key Features

### User & Role Management
- Create, edit, deactivate users with role assignments (Admin, Read-Only, Client)
- Email notifications on account creation with temporary credentials
- Encrypted password storage via 3DES

### Financial Report Generation
- **Sales Transaction Report** — detailed property sale records
- **Purchase Transaction Report** — replacement property purchases
- **Gain Recognition Report** — realized/deferred gain calculations
- **Sales & Purchase Summary** — aggregated activity view
- **Make/Model Report** — asset categorization breakdown
- **Depreciation Report** — asset depreciation schedules
- **45-Day ID Reports** — IRS deadline tracking for pending identifications
- Reports generated as `.xlsx` using EPPlus, zipped, and archived to Azure Blob Storage

### 45-Day / 180-Day IRS Deadline Tracking
- Dashboard of assets approaching or past IRS exchange deadlines
- Replacement asset identification management
- Automated deadline calculations from closing dates

### Azure Integration
- **Azure SQL Database** — multi-tenant data store
- **Azure Blob Storage** — report archive with SAS-authenticated download URLs
- **Azure App Service** — deployed with staging and production slots

---

## API Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/api/UserLogin/Login` | Authenticate user, return token |
| POST | `/api/Administration/AddEditUser` | Create or update a user |
| GET | `/api/Administration/GetAllUsers` | List all users |
| POST | `/api/Reports/GenerateReport` | Generate and archive an Excel report |
| GET | `/api/Reports/DownloadReport` | Get Blob Storage download URL |
| GET | `/api/AssetMaintanance/GetAssets` | Retrieve asset list |
| POST | `/api/AssetMaintanance/SaveAsset` | Save asset record |
| GET | `/api/_45DayID/GetPendingIDs` | Pending 45-day identifications |
| GET | `/api/Pinnacle/GetData` | Pinnacle system data feed |

> Full interactive documentation is available at `/Help` when running locally.

---

## Getting Started

### Prerequisites
- Visual Studio 2017 or later
- .NET Framework 4.5.2
- SQL Server or Azure SQL Database access
- Azure Storage Account (for report archiving)
- NuGet package restore enabled

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/tahseenmohd/lke-web-api.git
   cd lke-web-api
   ```

2. **Restore NuGet packages**
   Open `LKEWebAPI.sln` in Visual Studio → right-click solution → *Restore NuGet Packages*

3. **Configure connections** (see [Configuration](#configuration))

4. **Build and run**
   Press `F5` in Visual Studio — the API starts on `http://localhost:60789/`

---

## Configuration

Copy the placeholder values in `Web.config` and `LKE_DAL/App.Config` and replace them with real values from your Azure portal. **Never commit real credentials.**

| Key | Description |
|---|---|
| `YOUR_SERVER` | Azure SQL server hostname |
| `YOUR_USER` | SQL login username |
| `YOUR_PASSWORD` | SQL login password |
| `YOUR_STORAGE_ACCOUNT` | Azure Storage account name |
| `YOUR_STORAGE_KEY` | Azure Storage account key |
| `adminEmail` | Admin notification email |
| `adminPassword` | Admin default password |

For local development, use **Azure Key Vault** or **environment variables** rather than editing `Web.config` directly.

---

## Deployment

The solution includes publish profiles for Azure App Service under `LKEWebAPI/Properties/PublishProfiles/`:

- `lke-new-api` — Production
- `lke-new-api-staging` — Staging slot
- `lkeapi-testing` — Test environment

Deploy via Visual Studio: right-click project → *Publish* → select profile.

---

## Author

Developed by **Tahseen Mohammed**  
GitHub: [github.com/tahseenmohd](https://github.com/tahseenmohd)
