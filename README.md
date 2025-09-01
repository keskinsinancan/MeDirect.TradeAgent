# MeDirect.TradeAgent

A scalable trading management platform for MeDirect, designed with a clean architecture, Domain-Driven Design (DDD), and asynchronous event-driven communication using RabbitMQ.

## Architecture Overview

- **Domain Layer:**  
  Contains core business entities, value objects, and domain events, encapsulating business rules and logic. This layer models the core of the business and enforces invariants.

- **Application Layer:**  
  Handles application logic, data transfer objects (DTOs), service orchestration, and defines abstractions for repositories and messaging. Coordinates use cases and business workflows.

- **Infrastructure Layer:**  
  Implements data access (using Entity Framework Core), messaging (RabbitMQ integration), and other technical concerns. Fulfills contracts defined in the application layer.

- **API Layer:**  
  Exposes HTTP endpoints and background workers for processing and publishing events. Handles incoming requests and orchestrates application services.

- **Consumer:**  
  A separate service that listens to RabbitMQ messages and processes them asynchronously, enabling decoupled and scalable event handling.

## Domain-Driven Design (DDD) in This Project

This project follows DDD principles to ensure that business logic is central, consistent, and decoupled from infrastructure and application concerns.

- **Rich Domain Model:**  
  The `TradeAgent.Domain` project contains entities (e.g., `Trade`), value objects, and domain events (e.g., `TradeExecutedEvent`). Business rules are enforced within these models.

- **Domain Events:**  
  Significant business actions raise domain events, which are handled asynchronously to enable integration and eventual consistency.

- **Separation of Concerns:**  
  DDD encourages a clear separation between business logic and technical details, resulting in a modular, maintainable, and testable codebase.

- **Outbox Pattern:**  
  Ensures reliable event delivery by storing domain events in an outbox table and publishing them asynchronously, maintaining consistency between the database and message broker.

## Key Features

- Clean, maintainable, and testable architecture
- DDD approach with rich domain models and domain events
- Asynchronous event publishing and consumption via RabbitMQ
- Outbox pattern for reliable event delivery
- Modular design for easy extension and scaling

## Messaging Flow

1. **Trade Execution:**  
   When a trade is executed, a domain event is raised in the domain layer.
2. **Event Publishing:**  
   The event is persisted and published to RabbitMQ by the API's background worker.
3. **Event Consumption:**  
   The Consumer service listens to the queue, processes incoming events, and performs further actions.

## Configuration

- **RabbitMQ:**  
  Connection settings are managed via `appsettings.json` in both API and Consumer projects.
- **Database:**  
  Uses Entity Framework Core for data persistence.

## Getting Started

1. **Clone the repository**
2. **Configure RabbitMQ and database settings** in `appsettings.json` files.
3. docker compose to fire up PostgreSql Server, PG Admin, RabbitMq server and RabbitMq admin panel 
4. **Run the launch profile "Api + Consumer"** to start the HTTP server and background workers.
5. See the messages are consumed by the consumer in the console logs.

## Projects

- `TradeAgent.Domain` – Business entities, value objects, and domain events
- `TradeAgent.Application` – DTOs, services, and abstractions
- `TradeAgent.Infrastructure` – Data and messaging implementations
- `TradeAgent.API` – Web API and background workers
- `TradeAgent.Consumer` – RabbitMQ event consumer

## License

This project is licensed under the MIT License.