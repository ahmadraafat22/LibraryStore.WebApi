# LibraryStore — E-Commerce Store Backend API

A reusable bookstore e-commerce backend built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server**.

LibraryStore demonstrates practical backend engineering: Clean/Layered Architecture, CQRS with MediatR, centralized request validation, JWT authentication, guest checkout order management, and integrations with external APIs (Telegram, Cloudinary).

> **Project scope:** This repository contains the backend API only. The frontend consumes these endpoints but is maintained in a separate project.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Key Concepts](#key-concepts)
- [Telegram Bot Integration](#telegram-bot-integration)
- [Cloudinary Integration](#cloudinary-integration)
- [Authentication and Authorization](#authentication-and-authorization)
- [Order Management](#order-management)
- [Offers / Discounts](#offers--discounts)
- [Store Settings](#store-settings)
- [Admin Dashboard](#admin-dashboard)
- [CORS Policy](#cors-policy)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Current Business Scope (V1)](#current-business-scope-v1)
- [Roadmap (V2)](#roadmap-v2)
- [Project Status](#project-status)

## Overview

LibraryStore is a reusable backend template for an online bookstore. It provides APIs for managing products, categories, product images, store settings, and guest customer orders.

The project integrates with:

- **Telegram Bot API** — sends order notifications to the administrator with inline buttons to confirm or cancel orders directly from Telegram.
- **Cloudinary** — uploads, stores, and deletes product images without storing binaries in SQL Server.

The goal is a maintainable backend with clear separation of concerns and real-world integrations, built independently as a freelance/portfolio project.

## Features

- Product and category management with a many-to-many relationship.
- Product image management: maximum of three active images per product.
- Primary image selection with server-controlled display ordering.
- Cloudinary image upload and deletion.
- Product search and pagination.
- "New Arrivals" endpoint (latest products, fixed page size).
- Store settings management (contact info and social links).
- Guest checkout — no customer registration required.
- Automatic, sequence-based order numbering.
- Order item snapshots to preserve product name and price at purchase time.
- Order statuses: `Pending`, `Confirmed`, `Cancelled`.
- Telegram notifications with inline Confirm/Cancel buttons; message is edited after the action is taken.
- Telegram webhook integration, secured with a secret token.
- Admin dashboard with sales and order statistics by period.
- ASP.NET Core Identity and JWT authentication with role-based authorization.
- FluentValidation pipeline and centralized global exception handling.
- Soft deletion with global query filters across business entities.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# / .NET 8 | Application platform |
| ASP.NET Core Web API | HTTP API and controllers |
| Entity Framework Core | Database access and ORM |
| SQL Server | Relational database |
| ASP.NET Core Identity | User and role management |
| JWT Bearer Authentication | Protecting authenticated API requests |
| MediatR | CQRS-style request/handler dispatch |
| FluentValidation | Request validation pipeline |
| Cloudinary | Product image storage and delivery |
| Telegram Bot API | Order notifications and administrator actions |
| Swagger / OpenAPI | API documentation and testing |

## Architecture

The solution follows Clean/Layered Architecture, split into four projects:

```text
LibraryStore/
├── LibraryStore.Domain/
│   ├── Entities/
│   └── Interfaces/
├── LibraryStore.Application/
│   ├── Features/
│   └── Common/
│       ├── Behaviors/
│       ├── Exceptions/
│       └── DTOs/
├── LibraryStore.Infrastructure/
│   ├── Persistence/
│   └── Services/
└── LibraryStore.WebApi/
    ├── Controllers/
    │   └── Admin/
    ├── Middleware/
    └── Program.cs
```

### 1. Domain Layer

Core business entities and abstractions — products, categories, product images, orders, and order items — plus a shared `BaseEntity`:

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```

The Domain layer has no dependency on EF Core, controllers, or external API implementations.

### 2. Application Layer

Use cases and request-handling logic, organized as MediatR Commands and Queries (creating/updating products, creating orders, retrieving orders, updating order status, etc.). The Application layer depends on abstractions (e.g. `ILibraryDbContext`) rather than concrete infrastructure.

### 3. Infrastructure Layer

Implements persistence and external services:

- EF Core + SQL Server (`LibraryDbContext : IdentityDbContext<ApplicationUser>, ILibraryDbContext`).
- Cloudinary image operations.
- Telegram Bot API client and webhook handling.
- External-service configuration and dependency injection.

### 4. WebApi Layer

- Controllers and routes, with Public and Admin controllers kept separate for clarity and security.
- Authentication/authorization middleware.
- Dependency injection composition and Swagger configuration.
- Global exception middleware.

### Architecture Decisions

- Controllers stay thin; business logic lives in Application handlers.
- **No Unit of Work + Generic Repository pattern.** Application handlers work directly against `ILibraryDbContext`, with MediatR/CQRS organizing the use cases instead.
- EF Core configurations use `IEntityTypeConfiguration<T>`, loaded via `ApplyConfigurationsFromAssembly()`.
- Public vs. Admin endpoints are physically separated into different controllers/routes.

This separation improves maintainability and makes it easier to change infrastructure without spreading changes across the whole application.

## Key Concepts

### CQRS and MediatR

- **Commands** change state (e.g. create a product, confirm an order).
- **Queries** retrieve data (e.g. get product details, list orders).
- **Handlers** contain the logic for each request; **MediatR** dispatches requests to their handler.

This is a CQRS-style organization of code, not separate read/write databases.

### FluentValidation Pipeline

FluentValidation defines rules for incoming requests (required fields, valid quantities, supported image formats, etc.). A MediatR pipeline behavior runs the relevant validator before the handler executes, keeping validation out of controllers.

### Entity Framework Core Configurations

Entity configurations define required fields and max lengths, decimal precision, unique indexes, relationships, database constraints, and global query filters for soft deletion.

### Soft Deletion

Business entities use `IsDeleted` instead of hard deletes. Global query filters exclude soft-deleted records automatically (filtered index syntax: `[IsDeleted] = 0`). For product images, deleting a record also triggers deletion of the corresponding Cloudinary asset.

### Many-to-Many Relationships

Products and categories use an explicit `ProductCategory` join entity (composite key `ProductId + CategoryId`, not derived from `BaseEntity`). A product can belong to multiple categories, and a category can contain multiple products.

### Order Snapshots

Order items store `ProductName`, `UnitPrice`, and `LineTotal` at the time of purchase, so historical orders stay accurate even if a product's name or current price changes later.

### Server-Side Business Rules

The backend — never the client — calculates line totals and the order total, assigns the initial order status, and generates the order number. The client-side cart (kept in frontend LocalStorage) is only a convenience; the server re-validates product existence, availability, quantity, and current price on checkout. If any item in the cart is unavailable, the entire checkout is rejected.

## Telegram Bot Integration

### New Order Notification

When a customer places an order:

1. The API validates the request.
2. The backend checks that requested products exist and are available.
3. The backend calculates item totals and the order total using server-side prices.
4. The order is saved with `Pending` status and a generated order number.
5. The Telegram bot sends a notification to the administrator with order number, customer name and phone, delivery address, ordered products/quantities, and the order total.

### Confirm and Cancel Buttons

The Telegram message includes inline **Confirm Order** and **Cancel Order** buttons, with callback data `confirm:{orderId}` / `cancel:{orderId}`.

When the administrator presses one:

1. Telegram sends a callback update to the configured webhook (`POST /api/telegram/webhook`).
2. The backend validates the webhook secret and the authorized administrator's Telegram user ID.
3. The callback identifies the order and the requested action.
4. The corresponding `ConfirmOrderCommand` / `CancelOrderCommand` updates the order status in the database.
5. The original Telegram message is edited to remove the buttons and show the new status label (falls back to sending a new message if the edit fails).

The bot never manages order data independently — the database remains the source of truth; Telegram only triggers backend commands.

### Webhook Security

- The secret is validated via the `X-Telegram-Bot-Api-Secret-Token` header.
- `allowedUpdates` is restricted to `CallbackQuery` only.
- The bot token and webhook secret are stored outside source control (e.g. `appsettings.Development.json`, excluded from git, or environment variables/user secrets in other environments) and should be rotated if ever exposed.
- Production requires a publicly reachable HTTPS endpoint for the webhook.

## Cloudinary Integration

### Image Upload Flow

1. An administrator submits an image via `multipart/form-data`.
2. The backend validates file size and allowed image type.
3. The Cloudinary service uploads the image.
4. Cloudinary returns a secure image URL and a `PublicId`.
5. The backend stores both the URL and `PublicId` in the database — the `PublicId` is stored explicitly rather than reconstructed from the URL.

### Image Deletion Flow

1. The stored `PublicId` is used to request deletion from Cloudinary.
2. The database record is marked deleted following the application's soft-delete approach.

### Image Management Rules

- Maximum of three **active** images per product (soft-deleted images don't count toward the limit).
- One image can be designated primary; the primary image uses `DisplayOrder = 0`, others use `1`, `2`, `3`.
- Display ordering is controlled entirely by the backend, not the client.
- Supported image formats and file size are validated server-side.

### Reliability Consideration

Cloudinary operations and the database update are not one automatic atomic transaction. If an upload succeeds but the database save fails, the uploaded asset may need manual cleanup — production hardening should add retry/cleanup handling and logging for these cases.

## Authentication and Authorization

- **ASP.NET Core Identity** manages administrator accounts and roles (`ApplicationUser` extends `IdentityUser` with `FirstName`/`LastName`, unique normalized email).
- Login issues a **JWT** after successful authentication; the token carries `NameIdentifier`, `Name`, `Email`, `JTI`, and role claims, all using `DateTime.UtcNow`.
- JWT expiry is configured via `JWT:ExpiryMinutes` in `appsettings.json` (recommended: 120 minutes), with `ClockSkew = TimeSpan.Zero`.
- Role-based authorization restricts administrative operations.
- `GET /api/auth/me` returns the currently authenticated user's info.
- Guest customers never log in — checkout does not require an account.

### Middleware Order

```csharp
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
```

### JWT Logout Consideration

Logout is **frontend-only** in V1: the client removes the stored token and redirects to login. This does not invalidate a copied token before it expires. Refresh-token support and proper revocation are deferred to V2.

## Order Management

### Guest Checkout

Customers check out without an account by submitting contact info, a delivery address, and the requested items. The backend:

- Validates customer fields and item quantities.
- Checks product existence and availability (rejects the whole checkout if any item is unavailable).
- Calculates prices and totals server-side.
- Stores product name/price snapshots.
- Generates a sequential order number.
- Creates the order as `Pending`.
- Sends a Telegram notification to the admin.

### Order Statuses

| Status | Meaning |
|---|---|
| `Pending` | Order created, awaiting an administrator's decision. |
| `Confirmed` | Administrator accepted the order. |
| `Cancelled` | Administrator cancelled the order. |

Transitions are restricted to `Pending → Confirmed` or `Pending → Cancelled`; any other transition raises a `BusinessRuleException`. Admins manage status through protected API endpoints or Telegram's inline buttons.

### Order Numbering

Order numbers follow the format `ORD-000001`, generated via a SQL Server **sequence** and an `ICodeGeneratorService` / `CodeGeneratorService` implementation, guaranteeing uniqueness without relying on client input.

## Offers / Discounts

There is no separate `Offer` entity. A product is considered on offer whenever `OldPrice > Price`; the frontend is responsible for displaying the discount based on these two fields.

## Store Settings

Single-row settings entity: `StoreName`, `Phone`, `Address`, `Facebook`, `Instagram`, `WhatsApp`, `TikTok`. Seeded via `HasData`.

> ⚠️ Do not modify the seeded `HasData` values directly in a new migration — doing so can overwrite values an administrator has already edited through the app.

- Public: `GET /api/store-settings`
- Admin: `PUT /api/admin/store-settings`

## Admin Dashboard

`GET /api/admin/dashboard/stats?period=Today|Week|Month` returns:

- Total sales (from `Confirmed` orders only)
- Total orders
- Confirmed / Pending / Cancelled order counts
- Available products count
- Best-selling products (top 10, from `Confirmed` orders only)

No charts in V1 — numeric stats only.

## CORS Policy

Configured in `Program.cs` under the policy name `"FrontendPolicy"`, with allowed origins read from `Cors:AllowedOrigins` in `appsettings.json`. `UseCors` must be registered **before** `UseAuthentication`/`UseAuthorization`.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Entity Framework Core CLI tools
- Cloudinary credentials (for image upload)
- Telegram bot credentials (to test notifications and callbacks)

### 1. Clone the Repository

```bash
git clone <YOUR_REPOSITORY_URL>
cd LibraryStore
```

Open the solution in Visual Studio or your preferred .NET IDE.

### 2. Configure Application Secrets

Set the connection string, JWT settings, Cloudinary credentials, and Telegram settings — see [Configuration](#configuration).

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

```bash
dotnet ef database update \
  --project LibraryStore.Infrastructure \
  --startup-project LibraryStore.WebApi
```

Adjust `--project` if migrations live elsewhere. Create an initial migration first if one doesn't exist yet.

### 5. Run the API

```bash
dotnet run --project LibraryStore.WebApi
```

If Swagger is enabled in Development, open `/swagger` on the printed URL to explore the API.

### 6. Test the Telegram Webhook (Optional)

Local testing needs a public HTTPS URL forwarding to your dev machine (e.g. via a tunneling tool such as ngrok: `Kestrel → http://localhost:5002 → ngrok → https://xxx.ngrok-free.dev/api/telegram/webhook`). In production, configure the real deployed HTTPS URL and keep the webhook secret protected.

## Configuration

Illustrative template — match property names to the actual options classes/bindings in the code:

```json
{
  "ConnectionStrings": {
    "cs": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_A_LONG_RANDOM_SECRET",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE",
    "ExpiryMinutes": 120
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "DO_NOT_COMMIT_THIS_SECRET"
  },
  "Telegram": {
    "BotToken": "DO_NOT_COMMIT_YOUR_BOT_TOKEN",
    "WebhookSecret": "DO_NOT_COMMIT_YOUR_WEBHOOK_SECRET"
  },
  "Cors": {
    "AllowedOrigins": ["https://your-frontend-domain.com"]
  }
}
```

### Secret Management

Use .NET User Secrets locally, or environment variables / a secret manager in deployment. Nested keys use double underscores:

```text
ConnectionStrings__cs
Jwt__Key
Cloudinary__ApiSecret
Telegram__BotToken
Telegram__WebhookSecret
```

Use a strong random JWT signing key, and rotate any credential that was ever committed or shared — removing a secret from the latest file version does not remove it from git history.

## API Endpoints

Representative routes — confirm exact paths against the controllers or Swagger UI.

| Area | Example method and route | Access |
|---|---|---|
| Authentication | `POST /api/auth/register` | Public (admin setup) |
| Authentication | `POST /api/auth/login` | Public |
| Authentication | `GET /api/auth/me` | Authenticated |
| Products | `GET /api/products` | Public |
| Products | `GET /api/products/{id}` | Public |
| Products | `GET /api/products/new-arrivals` | Public (latest 6, static) |
| Categories | `GET /api/categories` | Public |
| Guest checkout | `POST /api/order` | Public |
| Admin products | `GET/POST/PUT/DELETE /api/admin/products[/{id}]` | Admin |
| Admin categories | `POST/PUT/DELETE /api/admin/categories[/{id}]` | Admin |
| Admin orders | `GET /api/admin/orders`, `GET /api/admin/orders/{id}` | Admin |
| Admin order status | Confirm/Cancel order routes | Admin |
| Admin dashboard | `GET /api/admin/dashboard/stats?period=` | Admin |
| Store settings | `GET /api/store-settings` | Public |
| Store settings | `PUT /api/admin/store-settings` | Admin |
| Product images | Multipart upload route | Admin |
| Telegram webhook | `POST /api/telegram/webhook` | Validated Telegram webhook |

Use Swagger/OpenAPI in Development to inspect the actual routes exposed.

### Uploading Product Images

Image upload requests use `multipart/form-data` (not JSON) since they include a file — typically the product ID, image file, and a primary-image flag. Display order is assigned by the backend, never the client.

## Security Notes

- Never commit real connection strings, JWT signing keys, Cloudinary API secrets, Telegram bot tokens, or webhook secrets.
- Rotate any credential that has been exposed.
- Protect administrative write endpoints with authentication and the correct role.
- Validate the Telegram webhook secret and restrict callback actions to the authorized administrator's Telegram user ID.
- Validate uploaded files by size and type; don't rely solely on the submitted content type.
- Use HTTPS in deployment.
- Return generic errors to clients; log detailed exceptions securely server-side.
- Back up the database and test restores before handling real orders.
- Review CORS, logging, database permissions, and secret storage before public deployment.

## Current Business Scope (V1)

- Payment method: Cash on Delivery only (no payment gateway).
- Delivery limited to Minya; delivery fees are handled outside the backend.
- Product availability is a boolean flag — no stock/inventory quantity tracking, no max-quantity rule.
- Single branch store.
- No customer accounts, order history, coupons, complex offers, or multi-branch support.
- No caching, rate limiting, background jobs, or distributed systems components.

## Roadmap (V2)

- Deployment (Azure / Render / Railway)
- Refactor shared DTOs
- Automated testing (unit + integration)
- Refresh-token support / proper token revocation
- Rate limiting and further production hardening
- Caching, background jobs (e.g. Hangfire), RabbitMQ
- Docker-based deployment, reverse proxy, HTTPS automation, backups
- CI/CD pipeline
- Stock quantities and reservation
- Broader monitoring/logging for webhook and external API failures

## Project Status

LibraryStore is an actively developed, backend-focused portfolio/freelance project demonstrating ASP.NET Core architecture, database modeling, authentication, order management, and third-party API integrations (Telegram, Cloudinary).

**Built with C# and ASP.NET Core 8.**
