# 🏡 RentalScrapper

A modern, containerized .NET solution for scraping real estate listings from [immobilienscout24.at](https://www.immobilienscout24.at/) and processing them asynchronously using RabbitMQ and SQL Server. Built for scalability, reliability, and fun! 🚀

> **⚠️ This is a working MVP. Data may be parsed incorrectly or incompletely. Use at your own risk!**

---

## 📦 Features

- 🔍 **Scrape** real estate listings for Austria, Vienna, and Salzburg
- 🐇 **Queue** messages with RabbitMQ for async processing
- 🗄️ **Store** data in SQL Server
- 🛠️ **Worker** service for background processing
- 🐳 **Docker Compose** for easy local development

---

## 🏗️ Architecture

```
+-------------------+      +-------------------+      +-------------------+
|   ImScoutAT Main  | ---> |     RabbitMQ      | ---> |  ImScoutAtWorker  |
| (Scraper Service) |      |   (Message Bus)   |      | (Background Proc) |
+-------------------+      +-------------------+      +-------------------+
         |                                                    |
         |                                                    v
         |-------------------------------------------->  SQL Server
```

---

## 🚀 Quick Start

### 1. Clone the repo

```bash
git clone https://github.com/yourusername/RentalScrapper.git
cd RentalScrapper
```

### 2. Start everything with Docker Compose

```bash
docker-compose up --build
```

This will spin up:

- SQL Server (`db`)
- RabbitMQ with management UI (`rabbitmq`)
- Scraper services for Austria, Vienna, Salzburg (`imscoutat-main`, `imscoutat-wien`, `imscoutat-salzburg`)
- Worker service (`imscoutat-worker`)

### 3. Access RabbitMQ Management

- URL: [http://localhost:15672](http://localhost:15672)
- Default user/pass: `guest` / `guest`

### 4. Access SQL Server

- Host: `localhost`
- Port: `1433`
- User: `sa`
- Password: `Z!uperPuperPassword123`

---

## 🧑‍💻 Usage Examples

### Scraping Listings

The scraper services will automatically start scraping and sending messages to RabbitMQ. You can customize the target region by changing the `SCRAPE_URL` environment variable in `docker-compose.yml`.

### Sending a Message (C# Example)

```csharp
var client = new MessageClient(logger);
await client.Send(new RabbitMQMessage { /* ... */ }, CancellationToken.None);
```

### Consuming Messages (Worker Example)

```csharp
var worker = new RabbitMQClient();
await worker.ReceiveMessages();
```

---

## ⚙️ Configuration

All configuration is managed via environment variables and `SystemInfo` class. See `docker-compose.yml` for examples.

---

## 📝 Project Structure

- `ImScoutAT/` - Scraper service
- `ImScoutAtWorker/` - Worker/background processor
- `docker-compose.yml` - Multi-service orchestration
- `README.md` - This file!

---

## 🐞 Troubleshooting

- **RabbitMQ not healthy?** Wait a few seconds, it may be initializing.
- **SQL Server connection issues?** Ensure port 1433 is free and Docker has enough memory.
- **Scraper not working?** Check logs with `docker-compose logs -f`.

---

## ❤️ Contributing

PRs and issues welcome! Please open an issue or submit a pull request.

---

## 📄 License

MIT

---

Happy Scraping! 🏠✨
