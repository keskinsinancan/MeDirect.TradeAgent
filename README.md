# 🚀 MeDirect.TradeAgent

A **scalable trading management platform** for MeDirect, designed with **Clean Architecture**, **Domain-Driven Design (DDD)**, and **asynchronous event-driven communication** using **RabbitMQ**.  

---

## 🏗️ Architecture Overview

```plantuml
@startuml
package "API Layer" {
  [API Controllers] --> [Application Services]
  [Background Workers] --> [RabbitMQ Publisher]
}

package "Application Layer" {
  [Application Services] --> [Repositories <<interface>>]
  [Application Services] --> [Message Bus <<interface>>]
}

package "Domain Layer" {
  [Entities] --> [Domain Events]
  [Value Objects]
}

package "Infrastructure Layer" {
  [EF Core Repository] -up-|> [Repositories <<interface>>]
  [RabbitMQ Integration] -up-|> [Message Bus <<interface>>]
}

package "Consumer Service" {
  [RabbitMQ Subscriber] --> [Application Services]
}

package "Logging" {
  [Serilog] --> [Redis Log Store]
  [API Log Endpoint] --> [Redis Log Store]
}

[Domain Events] --> [RabbitMQ Publisher]
[RabbitMQ Subscriber] --> [Application Services]
@enduml
```

✨ **Highlights**  
- Core **business rules** live in the **Domain Layer**  
- **Application Layer** orchestrates workflows  
- **Infrastructure Layer** → EF Core + RabbitMQ + Redis  
- **API Layer** exposes endpoints & publishes events  
- **Consumer Service** processes messages asynchronously  
- **Logging Layer** centralizes logs in Redis  

---

## 📦 Messaging Flow

```plantuml
@startuml
actor User
participant API
participant "Outbox Table" as Outbox
participant RabbitMQ
participant Consumer

User -> API : Execute Trade
API -> Outbox : Save Trade + Event
API -> RabbitMQ : Publish Event (async)
RabbitMQ -> Consumer : Deliver Event
Consumer -> Consumer : Process Trade Event
@enduml
```

---

## 🌐 Deployment View

```plantuml
@startuml
node "Docker Host" {
  node "API Service" {
    [TradeAgent.API]
  }
  node "Consumer Service" {
    [TradeAgent.Consumer]
  }
  node "Logging Service" {
    [TradeAgent.Logging]
  }
  database "PostgreSQL"
  queue "RabbitMQ"
  storage "Redis"
}

[TradeAgent.API] --> PostgreSQL
[TradeAgent.API] --> RabbitMQ
[TradeAgent.API] --> Redis
[TradeAgent.Consumer] --> RabbitMQ
[TradeAgent.Consumer] --> Redis
[TradeAgent.Logging] --> Redis
@enduml
```

---

## ✨ Key Features

- 🧼 Clean, maintainable, and testable architecture  
- 🏛️ Rich domain models & domain events (DDD)  
- 📨 Asynchronous event publishing & consumption via RabbitMQ  
- 📦 Outbox pattern for reliable delivery  
- 🪵 Centralized distributed logging with Redis + Serilog  
- 🐳 Containerized deployment with Docker Compose  
- ✅ Unit + integration tests  
- ⚡ CI/CD with GitHub Actions  

---

## 🧱 SOLID Principles

- **SRP 🧩** – One responsibility per class  
- **OCP ➕** – Open for extension, closed for modification  
- **LSP 🔄** – Subtypes replace supertypes safely  
- **ISP ✂️** – Small, focused interfaces  
- **DIP 🔌** – High-level modules depend on abstractions  

---

## 🐳 Containerization

`docker-compose.yaml` provisions:  
- 🗄 PostgreSQL + PGAdmin  
- 📨 RabbitMQ + Management UI  
- 🔴 Redis for logging  
- 🌐 API + Consumer services  

Start all services:  

```bash
docker compose up --build
```

---

## 🔥 Logging Architecture

- 📊 **Serilog** → structured logs  
- 🪣 Logs stored in **Redis**  
- 🌐 Logs retrievable from `/api/demologs`  

---

## 🧪 Testing

- 🧩 **Unit Tests** → business logic & services  
- 🔗 **Integration Tests** → API, messaging, persistence  

---

## ⚡ CI/CD with GitHub Actions

- ✅ Builds & tests on each push/PR  
- 🚦 Prevents regressions & ensures quality  

---

## 📂 Projects

- `TradeAgent.Domain` 🏛️ – Entities, Value Objects, Domain Events  
- `TradeAgent.Application` 🎯 – DTOs, Services, Abstractions  
- `TradeAgent.Infrastructure` 🔧 – EF Core, RabbitMQ, Redis  
- `TradeAgent.API` 🌐 – Web API + Background Workers  
- `TradeAgent.Consumer` 📥 – RabbitMQ Consumer Service  
- `TradeAgent.Logging` 🪵 – Centralized Logging  
- `TradeAgent.Tests` ✅ – Unit & Integration Tests  

---

## 📜 License

Licensed under the **MIT License**.
