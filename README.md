# Uptime Monitoring Project

This project is a full-stack uptime monitoring application that tracks the availability and latency of registered services. It utilizes ASP.NET Core for backend API development, Redis for efficient state management, and Apache Kafka for handling asynchronous service status updates.

---

## 🧠 Functionality

- **Service Registration**: Add URLs that need to be monitored.
- **Health Checks**: Periodically ping services and log their response time and status.
- **Data Pipeline**: Health check results are sent to Kafka topics and processed asynchronously.
- **Status Dashboard**: Redis caches real-time service statuses for fast access by the frontend/UI (not shown in this repo).

---

## 🖼️ Screenshot

![Screenshot Placeholder](https://via.placeholder.com/600x400.png?text=Service+Uptime+Dashboard)

---

## 🏗️ Architecture

```
                +-------------------+
                |    ASP.NET API    |
                +-------------------+
                         |
         Registers       | Publishes
        +-------------> Kafka <----------------+
        |                |                     |
   REST Client      (Health Checks)      Kafka Consumer
        |                |                     |
        |          +-----v------+       +------v------+
        |          |  CheckJob  |-----> | Redis Cache |
        |          +------------+       +-------------+
        |                                       |
        +---------------------------------------+
                            API Queries Status
```

---

## 📚 What I Learned

Through this project, I gained practical experience with distributed system components:

- **Apache Kafka**: I used Kafka as a distributed message broker to publish and consume service health status updates. This allowed me to decouple the service monitoring logic from the data processing pipeline.
- **Redis**: I leveraged Redis as a fast in-memory cache to store and retrieve the latest status of each monitored service, ensuring real-time performance for any frontend or monitoring dashboard.
- **ASP.NET Core**: I used ASP.NET Core Web API to expose REST endpoints for registering services and querying their current status, allowing for easy integration with clients or monitoring interfaces.
