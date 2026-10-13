
# LibraryStore — E-Commerce Store Backend API

A reusable bookstore e-commerce backend built with **ASP.NET Core 8**, **Entity Framework Core**, and **SQL Server**.

LibraryStore demonstrates practical backend engineering concepts, including Clean Architecture, CQRS, request validation, authentication, order management, and integrations with external APIs.

> **Project scope:** This repository contains the backend API. The frontend can consume its endpoints but is maintained separately.

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
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Security](#security)
- [Future Improvements](#future-improvements)

## Overview

LibraryStore is a reusable backend template for an online bookstore. It provides APIs for managing products, categories, product images, store settings, and customer orders.

The project integrates with:

- **Telegram Bot API** — sends order notifications to the administrator and provides buttons to confirm or cancel orders.
- **Cloudinary** — uploads, stores, and deletes product images.

The goal is to build a maintainable backend with clear separation of concerns and practical integrations with external services.

## Features

- Product and category management.
- Many-to-many relationship between products and categories.
- Product image management with a maximum of three active images per product.
- Primary image selection and server-controlled display ordering.
- Cloudinary image upload and deletion.
- Product search and pagination.
- Store settings management.
- Guest checkout without customer registration.
- Automatic order numbering.
- Order item snapshots to preserve product names and prices at purchase time.
- Order statuses: `Pending`, `Confirmed`, and `Cancelled`.
- Telegram notifications with inline Confirm and Cancel buttons.
- Telegram webhook integration for updating order status.
- ASP.NET Core Identity and JWT authentication.
- Role-based authorization for administrative endpoints.
- FluentValidation and centralized exception handling.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# / .NET 8 | Application platform |
| ASP.NET Core Web API | HTTP API and controllers |
| Entity Framework Core | Database access and ORM |
| SQL Server | Relational database |
| ASP.NET Core Identity | User and role management |
| JWT Bearer Authentication | Protecting authenticated API requests |
| MediatR | Request and handler dispatch |
| FluentValidation | Request validation |
| Cloudinary | Product image storage and delivery |
| Telegram Bot API | Order notifications and administrator actions |
| Swagger / OpenAPI | API documentation and testing |

## Architecture

The solution is organized into four main layers:

```text
LibraryStore/
├── LibraryStore.Domain/
├── LibraryStore.Application/
├── LibraryStore.Infrastructure/
└── LibraryStore.WebApi/
```

### 1. Domain Layer

Contains the core business entities and domain-level abstractions, including products, categories, product images, orders, and order items.

The Domain layer should not depend on database infrastructure, controllers, or external API implementations.

### 2. Application Layer

Contains the application's use cases and request-handling logic.

Examples include:

- Creating and updating products.
- Creating orders.
- Retrieving orders.
- Updating order status.
- Validating incoming requests.

MediatR handlers organize the execution of these use cases. Interfaces allow the Application layer to use infrastructure capabilities without depending directly on their implementations.

### 3. Infrastructure Layer

Implements persistence and external services, including:

- Entity Framework Core and SQL Server.
- Cloudinary image operations.
- Telegram Bot API integration.
- External-service configuration and dependency injection.

### 4. WebApi Layer

Provides the HTTP entry point through:

- Controllers and routes.
- Authentication and authorization middleware.
- Dependency injection composition.
- Swagger configuration.
- Global exception middleware.

This separation improves maintainability and makes it easier to change infrastructure without spreading those changes across the entire application.

## Key Concepts

### CQRS and MediatR

The project organizes application operations into commands and queries.

- **Commands** change application state, such as creating a product or confirming an order.
- **Queries** retrieve information, such as getting product details or listing orders.
- **Handlers** contain the logic that executes each request.
- **MediatR** dispatches requests to their corresponding handlers.

This is a CQRS-style organization. It does not mean the application uses separate read and write databases.

### FluentValidation Pipeline

FluentValidation defines rules for incoming requests, such as required fields, valid quantities, and supported image formats.

A MediatR validation pipeline runs validators before the corresponding handler executes. This keeps validation logic out of controllers and helps maintain consistent request handling.

### Entity Framework Core Configurations

Entity configurations define database rules such as:

- Required fields and maximum lengths.
- Decimal precision.
- Unique indexes.
- Entity relationships.
- Database constraints.
- Global query filters for soft deletion.

### Soft Deletion

Instead of immediately removing certain records from the database, the application can mark them as deleted using `IsDeleted`.

Global query filters can exclude soft-deleted records from normal queries.

For product images, the application also deletes the corresponding Cloudinary asset when the image is deleted from the system.

### Many-to-Many Relationships

Products and categories use an explicit `ProductCategory` join entity. A product can belong to multiple categories, and a category can contain multiple products.

### Order Snapshots

Order items store the product name and unit price at the time the order is created. Historical order details therefore remain meaningful if the product's name or current price changes later.

### Server-Side Business Rules

The backend calculates order totals and assigns important values such as the initial order status and order number. Clients should not be trusted to submit authoritative prices or totals.

## Telegram Bot Integration

One of the project's main features is integration with the **Telegram Bot API**.

The integration lets an administrator receive new-order notifications and manage order status directly from Telegram.

### New Order Notification

When a customer places an order:

1. The API validates the request.
2. The backend checks that the requested products exist and are available.
3. The backend calculates item totals and the final order total using server-side prices.
4. The order is saved in SQL Server with `Pending` status.
5. The Telegram bot sends a notification to the administrator.

The notification includes order information such as:

- Order number.
- Customer name and phone.
- Delivery address.
- Ordered products and quantities.
- Order total.

### Confirm and Cancel Buttons

The Telegram message includes inline buttons:

- **Confirm Order**
- **Cancel Order**

When the administrator presses one of these buttons:

1. Telegram sends a callback update to the configured webhook.
2. The backend receives the update through its Telegram webhook endpoint.
3. The backend validates the update and checks the authorized administrator.
4. The callback identifies the order and requested action.
5. The corresponding application command updates the order status in the database.

This means the Telegram bot is not independently managing order data. It sends the action to the backend, and the database remains the source of truth.

### Webhook Security

The webhook should validate Telegram's configured secret token and restrict administrative actions to the intended Telegram user.

The bot token and webhook secret must be stored securely and must never be committed to source control.

For production, the webhook must use a publicly reachable HTTPS endpoint.

## Cloudinary Integration

LibraryStore uses **Cloudinary** to manage product image files without storing the actual image binaries in SQL Server.

### Image Upload Flow

1. An administrator submits an image using `multipart/form-data`.
2. The backend validates the file's size and allowed image type.
3. The Cloudinary service uploads the image.
4. Cloudinary returns a secure image URL and a `PublicId`.
5. The backend stores the URL and `PublicId` in the database.

The image URL is used to display the image, while the `PublicId` is used to identify the asset for management and deletion.

### Image Deletion Flow

When a product image is deleted:

1. The backend uses the stored `PublicId` to request deletion from Cloudinary.
2. The database record is marked as deleted according to the application's soft-delete approach.

The `PublicId` should be stored explicitly rather than reconstructed from the image URL.

### Image Management Rules

- A product can have at most three active images.
- One image can be designated as the primary image.
- Display ordering is controlled by the backend.
- The application validates supported image formats and file size.
- Cloudinary stores and delivers the image assets.

### Reliability Consideration

Cloudinary operations and database updates do not form one automatic atomic transaction. If an upload succeeds but saving the database record fails, the uploaded asset may need cleanup. A production implementation should consider error handling, logging, and retry strategies for these cases.

## Authentication and Authorization

The project uses **ASP.NET Core Identity** and **JWT Bearer Authentication**.

- Identity manages administrator accounts and roles.
- Login issues a JWT after successful authentication.
- The API validates the token on protected requests.
- Role-based authorization restricts administrative operations.
- The `/me` endpoint can return information about the currently authenticated user.

Customers use guest checkout and do not need to create an account.

### JWT Logout Consideration

With stateless JWT authentication, client-side logout typically removes the stored token and redirects the user to the login page. This does not invalidate a copied token before it expires.

Token revocation or refresh-token management can be added later if the application's security requirements justify it.

## Order Management

### Guest Checkout

Customers can place orders without creating an account. The checkout request includes customer contact information, a delivery address, and the requested items.

The backend:

- Validates customer fields and item quantities.
- Checks product existence and availability.
- Calculates prices and totals on the server.
- Stores product name and price snapshots.
- Generates an order number.
- Creates the order with `Pending` status.
- Sends a Telegram notification.

### Order Statuses

| Status | Meaning |
|---|---|
| `Pending` | The order has been created and awaits an administrator's decision. |
| `Confirmed` | The administrator has accepted the order. |
| `Cancelled` | The administrator has cancelled the order. |

Administrators can manage orders through protected API endpoints or Telegram's inline buttons.

### Current Business Scope

- Payment method: Cash on Delivery.
- Delivery is intended for Minya.
- Delivery fees are handled outside the backend.
- Product availability is represented by an availability flag; inventory quantities are not tracked.
- The store supports a single branch.
- Customer accounts, payment gateways, coupons, and complex inventory management are outside the current scope.

## Getting Started

### Prerequisites

Install:

- .NET 8 SDK.
- SQL Server.
- Entity Framework Core CLI tools.
- Cloudinary credentials for image upload.
- Telegram bot credentials if you want to test order notifications and callbacks.

### 1. Clone the Repository

```bash
git clone <YOUR_REPOSITORY_URL>
cd LibraryStore
```

Open the solution in Visual Studio or your preferred .NET IDE. Adjust the directory if your repository uses a different name.

### 2. Configure Application Secrets

Configure the connection string, JWT settings, Cloudinary credentials, and Telegram settings. See [Configuration](#configuration).

### 3. Restore Dependencies

Run from the solution directory:

```bash
dotnet restore
```

### 4. Apply Database Migrations

If migrations are located in `LibraryStore.Infrastructure` and `LibraryStore.WebApi` is the startup project, a typical command is:

```bash
dotnet ef database update \
  --project LibraryStore.Infrastructure \
  --startup-project LibraryStore.WebApi
```

Adjust `--project` to match the project that actually contains your migrations.

If no initial migration exists, create one using the correct migrations and startup projects before applying the database update.

### 5. Run the API

```bash
dotnet run --project LibraryStore.WebApi
```

Use the URL printed in the console. If Swagger is enabled in Development, open `/swagger` on that URL to explore the API.

### 6. Test the Telegram Webhook (Optional)

For local testing, Telegram needs a public HTTPS URL that can forward requests to your development machine. A tunnel can be used during development. In production, configure the deployed HTTPS URL and protect the webhook with the configured secret.

## Configuration

The exact configuration section names depend on the options classes and configuration bindings in the current code. The following is an **illustrative template only**; adapt it to the project's actual configuration.

```json
{
  "ConnectionStrings": {
    "cs": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_A_LONG_RANDOM_SECRET",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  },
  "Cloudinary": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "DO_NOT_COMMIT_THIS_SECRET"
  },
  "Telegram": {
    "BotToken": "DO_NOT_COMMIT_YOUR_BOT_TOKEN",
    "WebhookSecret": "DO_NOT_COMMIT_YOUR_WEBHOOK_SECRET"
  }
}
```

The JWT, Telegram, and connection-string property names above may differ from the current project. Match them to your actual settings classes.

### Secret Management

Use .NET User Secrets during local development, or environment variables and a secret manager in deployment.

Nested environment-variable keys commonly use double underscores:

```text
ConnectionStrings__cs
Jwt__Key
Cloudinary__ApiSecret
Telegram__BotToken
Telegram__WebhookSecret
```

Use a strong random JWT signing key. If any real credentials were committed to GitHub or shared logs, rotate them. Removing a secret from the latest version of a file does not remove it from Git history.

## API Endpoints

The following routes are representative examples. **Check the route attributes in the current code or Swagger UI to confirm the exact paths.**

| Area | Example method and route | Access |
|---|---|---|
| Authentication | `POST /api/auth/register` | Public registration endpoint for administrator setup |
| Authentication | `POST /api/auth/login` | Public |
| Authentication | `GET /api/auth/me` | Authenticated |
| Products | `GET /api/products` | Public |
| Products | `GET /api/products/{id}` | Public |
| Categories | `GET /api/categories` | Public |
| Guest checkout | `POST /api/order` | Public |
| Admin orders | Configured admin order-list route | Admin |
| Admin order details | Configured admin order-details route | Admin |
| Store settings | Configured public GET route | Public |
| Store settings | Configured update route | Admin |
| Product images | Configured multipart upload route | Admin |
| Telegram webhook | `POST /api/telegram/webhook` | Validated Telegram webhook |

Use Swagger/OpenAPI in Development to inspect the actual routes exposed by the application.

### Uploading Product Images

Image upload requests use `multipart/form-data`, not JSON, because the request contains a file. The request typically contains the product identifier, image file, and supported primary-image option. Display order should be assigned by the backend.

## Security Notes

- Never commit real connection strings, JWT signing keys, Cloudinary API secrets, Telegram bot tokens, or webhook secrets.
- Rotate any credentials that have been exposed.
- Protect administrative write endpoints with authentication and the appropriate role.
- Validate Telegram webhook secrets and restrict callback actions to the authorized administrator.
- Validate uploaded files by size and allowed image type; production hardening should not rely on the submitted content type alone.
- Use HTTPS in deployment.
- Return generic errors to clients and log detailed exceptions securely on the server.
- Back up the database and test restoring it before using the application for real orders.
- Review CORS, logging, database permissions, and secret storage before public deployment.

## Future Improvements

These are possible future enhancements, not claims that they are already implemented:

- Unit and integration tests.
- More robust cleanup and retry handling for Cloudinary and Telegram failures.
- Logging and monitoring for webhook delivery and external API failures.
- Docker-based deployment, reverse proxy, HTTPS, and automated backups.
- CI/CD pipeline.
- Refresh-token support or another token-revocation strategy if required.
- Inventory quantities and stock reservation.
- Rate limiting and additional production hardening based on real traffic and threat assessment.

## Project Status

LibraryStore is a backend-focused portfolio project demonstrating ASP.NET Core architecture, database modeling, authentication, order management, and third-party API integrations.

**Built with C# and ASP.NET Core 8.**
