# 📚 LibraryStore

> **Bookstore E-Commerce Backend API**  
> A production-ready, reusable backend template for a local bookstore — built with ASP.NET Core 8, Clean Architecture, and real-world external service integrations.

---

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features](#features)
- [Data Model](#data-model)
- [Order Management](#order-management)
- [Telegram Bot Integration](#telegram-bot-integration)
- [Cloudinary Integration](#cloudinary-integration)
- [Security](#security)
- [Getting Started](#getting-started)
- [Deployment](#deployment)
- [Known Limitations & Scope](#known-limitations--scope)
- [Roadmap](#roadmap)

---

## Overview

**LibraryStore** is a backend REST API for managing a bookstore catalog, administrator accounts, product images, store configuration, and customer orders. It is built as both a practical portfolio project and a foundation for a real local business — designed to be reusable and adaptable for other shops.

**Key goals:**
- Deliver a fully functional backend for a local bookstore.
- Apply Clean Architecture for long-term maintainability.
- Integrate external services: Cloudinary for image storage, Telegram for real-time order management.
- Serve as a reusable template for similar e-commerce backends.

> The frontend is a separate project. This repository covers the backend API only.

---

## Tech Stack

| Technology | Purpose |
|---|---|
| **C# / .NET 8** | Main language and application platform |
| **ASP.NET Core Web API** | HTTP API layer |
| **Entity Framework Core** | ORM and database access |
| **SQL Server** | Relational database |
| **MediatR** | CQRS command/query dispatch |
| **FluentValidation** | Request validation pipeline |
| **ASP.NET Core Identity** | Administrator identity management |
| **JWT Bearer** | Stateless API authentication |
| **Cloudinary** | Cloud-based product image storage |
| **Telegram Bot API** | Real-time order notifications and actions |

---

## Architecture

LibraryStore follows **Clean Architecture**, organized into four layers with strict dependency rules:

```
LibraryStore/
├── Domain/          # Core business entities and rules
├── Application/     # Use cases, CQRS handlers, validation
├── Infrastructure/  # EF Core, SQL Server, Cloudinary, Telegram
└── WebApi/          # Controllers, DI, auth, middleware
```

### Layer Responsibilities

**Domain**  
Core business entities, relationships, and domain rules. No dependency on the database, HTTP, or any external service.

**Application**  
All application use cases and workflows:
- MediatR commands and queries
- Command/query handlers
- FluentValidation validators
- Application interfaces and abstractions
- Pipeline behaviors including validation

**Infrastructure**  
Implementations that communicate with the outside world:
- EF Core `DbContext` and entity configurations
- SQL Server persistence
- Implementations of Application abstractions
- Cloudinary image upload/delete integration
- Telegram Bot webhook and callback handling

**WebApi**  
The HTTP entry point:
- Thin controllers that dispatch to MediatR
- Dependency injection composition root
- Authentication and authorization configuration
- Middleware and HTTP pipeline setup

### Request Flow

```
HTTP Request
  → Controller
  → MediatR Command / Query
  → Validation Pipeline Behavior
  → Handler
  → Application Abstraction
  → Infrastructure Implementation
  → SQL Server / Cloudinary / Telegram
```

Controllers are intentionally thin. All business and use-case logic lives in Application handlers.

---

## Features

### Product Management

Full CRUD support for bookstore products, including availability toggling, pricing, and multi-category assignment.

| Field | Description |
|---|---|
| `Id` | GUID identifier (from base entity) |
| `Name` | Product name (max 100 chars) |
| `Description` | Product description (max 400 chars) |
| `Price` | Decimal price, precision `(18,2)` |
| `OldPrice` | Optional previous price for sale display |
| `IsAvailable` | Controls whether a product can be ordered |

### Category Management

Products are organized into categories with the following rules:
- Name is required (max 100 chars).
- Description max 400 chars.
- Category names must be unique among active (non-deleted) categories.
- Soft deletion is used instead of hard deletes.

### Many-to-Many Product–Category Relationship

A product can belong to multiple categories and a category can hold multiple products. This is managed through a `ProductCategory` join entity with a composite key `(ProductId, CategoryId)`.

### Product Images (Cloudinary)

Each product supports **up to three active images**, stored externally via Cloudinary.

| Field | Description |
|---|---|
| `Url` | Cloudinary-hosted secure image URL |
| `PublicId` | Cloudinary identifier used for management and deletion |
| `IsPrimary` | Flags the product's main display image |
| `DisplayOrder` | Backend-controlled display ordering |

### Search & Pagination

Product listing supports search and paginated responses. Confirm exact query parameters, sortable fields, and response schema from the API endpoints.

### Store Settings

A dedicated Store Settings feature allows store-level configuration to be managed through the API.

### Administrator Authentication

- ASP.NET Core Identity for administrator account management.
- JWT Bearer tokens for protecting administrative endpoints.
- `/me`-style endpoint returning the current administrator's profile.
- **V1 note:** logout is client-side token deletion. Server-side token revocation and refresh tokens are not part of the current implementation.

---

## Data Model

```
Category
   |
   | many-to-many (via ProductCategory)
   |
Product ──── ProductImage (up to 3 per product)

Order
   |
   | one-to-many
   |
OrderItem (product name + price snapshot + quantity)

Administrator ──── ASP.NET Core Identity tables
Store Settings ──── store-level configuration
```

### Base Entity

All main entities inherit from a shared base:

| Property | Purpose |
|---|---|
| `Id` | `Guid` identifier |
| `CreatedAt` | UTC creation timestamp |
| `UpdatedAt` | Nullable UTC update timestamp |
| `IsDeleted` | Soft-delete flag (default `false`) |

> `ProductCategory` uses a composite key and does not inherit from `BaseEntity`.

### Soft Deletion

Relevant entities use soft deletion and EF Core global query filters, so deleted records are excluded from queries automatically.

---

## Order Management

### Guest Checkout

No customer account is required. A customer submits:
- Name and phone number
- Delivery address
- Ordered products and quantities

**Payment method:** Cash on Delivery (COD).

### Checkout Workflow

1. Receive customer and order-item data.
2. Validate the request (FluentValidation pipeline).
3. Retrieve the requested products.
4. Check that every product is available; reject the entire order if any product is not.
5. Calculate the order total from prices and quantities.
6. Create the `Order` and `OrderItem` records.
7. Generate a sequential order number.
8. Persist the order in SQL Server.
9. Send a Telegram notification to the store.

### Order Numbers

```
ORD-000001
ORD-000002
ORD-000003
```

### Order Statuses

| Status | Meaning |
|---|---|
| `Pending` | Order created, awaiting store confirmation |
| `Confirmed` | Accepted by the store |
| `Cancelled` | Cancelled by the store |

### Order Item Snapshots

Each `OrderItem` stores a snapshot of the product name and price at the time of ordering. Historical orders remain accurate even if catalog data changes later.

---

## Telegram Bot Integration

The Telegram integration gives the store real-time order awareness and the ability to manage orders directly from Telegram.

### New Order Notification

When an order is placed, a Telegram message is sent containing:
- Order number
- Customer name and phone
- Ordered products and quantities
- Order total

The message includes **Confirm** and **Cancel** inline buttons.

### Callback Workflow

1. An administrator presses an inline button in Telegram.
2. Telegram sends a callback update to the configured webhook.
3. The backend receives and validates the webhook secret token.
4. The callback is processed as an order status update command.
5. The order status is updated in SQL Server.
6. The original Telegram message may be updated to reflect the result.

**Webhook endpoint:**
```http
POST /api/telegram/webhook
```

---

## Cloudinary Integration

Product images are stored externally on Cloudinary rather than on the application server.

### Upload Flow

1. API receives an image upload request.
2. Backend validates the file (type, size limits).
3. Backend uploads the image to Cloudinary.
4. Cloudinary returns a secure URL and `PublicId`.
5. Backend stores both in SQL Server.

### Delete Flow

1. Backend retrieves the image's `PublicId`.
2. Backend requests deletion from Cloudinary.
3. Backend deletes the image record from SQL Server.

> ⚠️ Cloudinary and SQL Server operations are not atomic. A production-hardened implementation should handle cases where one succeeds and the other fails (retry logic, orphaned asset cleanup).

---

## Security

- Administrative endpoints are protected by JWT Bearer authentication and authorization policies.
- JWT signing keys must be strong and kept outside source control.
- Cloudinary and Telegram credentials are managed via environment variables or .NET User Secrets — never committed to Git.
- Telegram webhook requests are validated using a configured secret token.
- Uploaded images are validated and subject to size/type limits.
- Internal exception details are not exposed in production responses.
- HTTPS is required in deployed environments.
- SQL Server and hosting resources follow least-privilege access principles.

### If Credentials Are Exposed

Removing a secret from the latest commit is insufficient if it was pushed to a remote. **Rotate immediately:**
- JWT signing key
- Database password
- Cloudinary API secret
- Telegram bot token
- Telegram webhook secret

Then review Git history and audit repository access.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or accessible server)
- Visual Studio 2022 / Visual Studio Code / Rider
- Cloudinary account and credentials
- Telegram bot token and webhook secret

### Clone the Repository

```bash
git clone <repository-url>
cd LibraryStore
```

### Configure Secrets

Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables to supply the following configuration:

| Configuration | Description |
|---|---|
| SQL Server connection string | Database access |
| JWT signing key + token settings | Authentication |
| Cloudinary cloud name, API key, API secret | Image storage |
| Telegram bot token | Bot integration |
| Telegram webhook secret token | Webhook validation |

> ⚠️ **Never commit real credentials to Git.**

### Apply Database Migrations

```bash
dotnet ef database update
```

Run from the correct startup or migrations project. Specify `--project` and `--startup-project` flags if the solution separates them.

### Build and Run

```bash
dotnet restore
dotnet build
dotnet run
```

### API Documentation

If Swagger/OpenAPI is configured, the development Swagger UI is available at:

```
https://localhost:<port>/swagger
```

Use it to explore all routes, request schemas, and response contracts.

---

## Deployment

A typical production deployment topology:

```
Customer Browser
      │
      ▼
Frontend Hosting
      │  HTTPS API requests
      ▼
Reverse Proxy (e.g. Nginx)
      │
      ▼
ASP.NET Core Web API
      │
      ├──▶ SQL Server
      ├──▶ Cloudinary
      └──▶ Telegram Bot API
```

**Deployment checklist:**
1. Choose a VPS or cloud hosting provider.
2. Configure the server and firewall.
3. Install or containerize the .NET API.
4. Provision SQL Server with secure access.
5. Supply environment variables and secrets.
6. Configure a domain and HTTPS certificate (e.g. Let's Encrypt).
7. Configure a reverse proxy (Nginx or similar).
8. Register the Telegram webhook with the public HTTPS endpoint.
9. Configure automated database backups and test restoration.
10. Add structured logging and health monitoring.

> Docker, CI/CD pipelines, and production monitoring are planned but not yet fully implemented. See [Roadmap](#roadmap).

---

## Known Limitations & Scope

| Area | Current Scope |
|---|---|
| Customer accounts | Guest checkout; no customer registration |
| Payment | Cash on Delivery only |
| Delivery fee | Handled outside the backend |
| Delivery area | Intended for Minya |
| Inventory | No stock-quantity management |
| Order statuses | Pending, Confirmed, Cancelled |
| Auth logout | Client-side token deletion; no server-side revocation |
| Refresh tokens | Not included in V1 |
| Images | Up to three active images per product via Cloudinary |
| Frontend | Separate project |
| Deployment | Production deployment not yet fully documented |

---

## Roadmap

### Backend & Reliability
- [ ] Unit and integration test coverage
- [ ] Structured logging and health check endpoints
- [ ] Rate limiting for public endpoints
- [ ] Consistent API response envelopes and versioning
- [ ] Idempotency protection for checkout
- [ ] Resilient Cloudinary/Telegram failure handling
- [ ] Background job processing for notifications and cleanup

### Authentication & Authorization
- [ ] Refresh token workflow
- [ ] Server-side token revocation
- [ ] Granular administrator roles and permissions

### E-Commerce
- [ ] Stock quantity and inventory management
- [ ] Delivery fee rules and service area configuration
- [ ] Online payment integration
- [ ] Order history and reporting
- [ ] Discounts and promotions
- [ ] Optional customer accounts

### DevOps & Operations
- [ ] Dockerized development and production deployment
- [ ] CI/CD pipeline
- [ ] Automated database backups and restore testing
- [ ] Monitoring and alerting
- [ ] Full deployment documentation

---

## License

This project is for portfolio and educational purposes. Add a license file if you intend to open-source or redistribute this codebase.
