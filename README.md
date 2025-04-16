# Uptime Monitoring Project

This project is a full-stack uptime monitoring application that tracks the availability and latency of registered services. It utilizes ASP.NET Core for backend API development, Redis for efficient state management, and Apache Kafka for handling asynchronous service status updates.

---

## 🚀 Getting Started

### Prerequisites

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Running the Project

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd uptime-monitoring-service
   ```

2. Build and start all services using Docker Compose:
   ```bash
   docker-compose up --build
   ```

3. Access the services:
   - API: http://localhost:5130
   - Dashboard: http://localhost:5131
   - Kafka: localhost:9092
   - Redis: localhost:6379
   - Zookeeper: localhost:2181

4. To stop all services:
   ```bash
   docker-compose down
   ```

### Development

- The API service runs on port 5130
- The Dashboard service runs on port 5131
- Services communicate internally using Docker's internal network
- Environment variables are configured in docker-compose.yml

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
