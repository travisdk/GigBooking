# GigBooking API

A robust, production-grade REST API backend built for managing music gigs, artists, and venues. The project is designed with a strong focus on maintainability, testability, and enterprise patterns, implementing **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS**.

---

## 🏗️ Architecture & Design Patterns

The solution strictly separates concerns across multiple layers:

* **Domain Layer:** Core business logic, entities (`Artist`, `Venue`, `Gig`), value objects (`Location`), domain events, and explicit domain error definitions using a functional `Result<T>` pattern.
* **Application Layer:** Use cases implemented via **MediatR** commands and queries, validation pipelines, and application DTOs.
* **Infrastructure Layer:** Database persistence via **Entity Framework Core**, database context configurations, repositories, and unit of work implementations supporting soft-delete capabilities (`IsActive`).
* **WebAPI Layer:** Controllers/Endpoints, request/response contracts, route definitions, and global error handling.

---

## 🚀 Tech Stack

* **.NET** (C#)
* **Entity Framework Core** (SQL Server persistence)
* **MediatR** (CQRS implementation)
* **xUnit** (Testing framework)
* **Testcontainers** (Integration testing with real containerized databases)
* **FluentAssertions / Built-in Assertions**

---

## 🧪 Testing Strategy

The repository includes a comprehensive test suite divided into:
1. **Unit Tests:** Fast, isolated tests verifying domain logic and handler behaviors.
2. **Integration Tests (`BaseEndpointTests` & `CustomWebApplicationFactory`):** End-to-end API tests running against isolated SQL Server instances via Testcontainers. These tests verify HTTP status codes (`204`, `400`, `404`), error contracts, and database persistence/rollback mechanics.

---

## 📋 Core Domain Entities

* **Artist:** Manages artist profiles with support for active/inactive lifecycles (soft-delete constraints).
* **Venue:** Features a complex `Location` Value Object (Street, Postal Code, City, Country Code) and soft-delete support.
* **Gig:** Coordinates events linking Artists and Venues, enforcing business rules around active relations.

---

## 🛠️ Getting Started

### Prerequisites
* .NET SDK installed
* Docker installed (required for running integration tests via Testcontainers)

### Running Locally
1. Clone the repository:
   ```bash
   git clone [https://github.com/travisdk/GigBooking.git](https://github.com/travisdk/GigBooking.git)
   cd GigBooking
