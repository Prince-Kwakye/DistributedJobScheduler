# Distributed Job Scheduler

## 🚀 Overview
The **Distributed Job Scheduler** is a scalable and efficient job scheduling system built with **ASP.NET Core** and **Entity Framework Core**. It allows users to manage background jobs with priority-based execution, dependency handling, and retry mechanisms in a distributed environment.

---

## 🔥 Features

- ✅ **Job Management** – Create, update, delete, and retrieve jobs.
- ✅ **Priority-Based Execution** – Schedules jobs based on priority and creation time.
- ✅ **Dependency Handling** – Ensures jobs execute only after dependent jobs are completed.
- ✅ **Retry Mechanism** – Jobs that fail are retried up to a configurable maximum.
- ✅ **User Authentication** – Secure authentication and authorization using **ASP.NET Identity**.
- ✅ **Worker Nodes** – Multiple worker nodes can process jobs for scalability.
- ✅ **Job Results Logging** – Logs execution details for tracking and debugging.

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core Web API (C#)
- **Database:** Entity Framework Core with SQL Server
- **Authentication:** ASP.NET Identity
- **ORM:** Entity Framework Core
- **Dependency Injection:** Built-in .NET DI
- **Logging & Monitoring:** Integrated logging for tracking job execution

---

## 📦 Installation & Setup

### Prerequisites:
- .NET 8.0 SDK or later
- SQL Server (LocalDB, Docker, or Cloud instance)

### Steps:

1️⃣ Clone the repository:
   ```sh
   git clone https://github.com/Cybertron618/DistributedJobScheduler.git
   cd DistributedJobScheduler
   ```

2️⃣ Install dependencies:
   ```sh
   dotnet restore
   ```

3️⃣ Configure the database connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your_server;Database=JobSchedulerDB;Trusted_Connection=True;"
   }
   ```

4️⃣ Apply database migrations:
   ```sh
   dotnet ef database update
   ```

5️⃣ Run the application:
   ```sh
   dotnet run
   ```

---

## 🚀 How It Works

1. **Users submit jobs** via the API.
2. **Jobs are stored** in the database with their priority and dependencies.
3. **Worker nodes fetch pending jobs** and execute them in order of priority.
4. **Completed job results** are logged and stored for reference.

---

## 🏗️ API Endpoints

### 🎯 Job Management
- `GET /api/jobs/{id}` – Retrieve a job by ID
- `POST /api/jobs` – Create a new job
- `PUT /api/jobs/{id}` – Update a job
- `DELETE /api/jobs/{id}` – Delete a job

### 📌 Job Execution
- `GET /api/jobs/pending` – Retrieve pending jobs
- `GET /api/jobs/next` – Retrieve the next job to execute

### 🔐 Authentication
- `POST /api/auth/register` – Register a new user
- `POST /api/auth/login` – Authenticate and obtain a token

---

## 🤝 Contributing

Contributions are welcome!

---

## 📜 License

This project is licensed under the **MIT License**

---

## ✨ Authors & Credits

- **Cybertron618** – [GitHub Profile](https://github.com/Cybertron618)

Feel free to reach out for any questions or feature suggestions! 🚀

