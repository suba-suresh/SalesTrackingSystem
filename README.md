# 📊 Sales Tracking System

C# | .NET | WPF | Entity Framework | MVVM

A high-quality desktop business application built using C# and .NET, designed with clean architecture, performance awareness, and backend-first principles.

This project demonstrates how enterprise-grade .NET applications are structured, how business logic flows from UI to database, and how systems can later evolve into high-throughput distributed services.

# 🎯 Purpose of This Project 

 This project was built to demonstrate:

- Strong C# and .NET fundamentals

- Clean separation of concerns (MVVM)

- Efficient data access via Entity Framework

- Business logic implementation with performance awareness

- Debugging, validation, and maintainability

- A foundation that can scale into APIs and distributed systems

The focus is not just UI, it is how data flows, how logic is structured, and how the system behaves under load.

# 🛠️ Technology Stack

- Language: C#

- Framework: .NET (WPF)

- UI: WPF (XAML)

- Architecture: MVVM (Model–View–ViewModel)

- ORM: Entity Framework 6 (Code First)

- Database: SQL Server

- Collections: ObservableCollection

- Data Binding: INotifyPropertyChanged

- Command Handling: ICommand / RelayCommand

- Reporting: FlowDocument (WPF Printing)

- LINQ: LINQ to Entities / LINQ to Objects

# 🧱 Architectural Overview (MVVM)
```
 UI (View - XAML)
        ↓
Commands (ICommand)
        ↓
ViewModel (Business Logic + State)
        ↓
Entity Framework (DbContext)
        ↓
SQL Server Database
```
Why MVVM?

- Decouples UI from business logic

- Enables testability and maintainability

- Mirrors how backend services separate controllers, services, and data layers

# 📦 Core Functional Modules
✔ Sales Management

- Add, update, delete sales records

- Automatic total calculation

- Daily, weekly, weekend, and monthly aggregation

- Optimised LINQ queries for filtering by date ranges

- ObservableCollection ensures UI updates without refresh overhead

✔ Salary Management

- Employee salary calculation based on:

- Hourly rate

- Hours worked

- Weekly or monthly payment cycles

- Supports historical salary records

- Prevents duplicate employee entries

- Prints structured salary reports

✔ Expense Management

- Track operational expenses

- Date-based filtering

- Stored as normalized entities

# 🧠 Core C# & .NET Concepts Demonstrated

🔹 C# Fundamentals

- Classes and objects

- Encapsulation using properties

- Interfaces (INotifyPropertyChanged, ICommand)

- LINQ (Where, Sum, GroupBy)

- Exception handling and validation

- Enums and business-driven state modeling

🔹 .NET Runtime Concepts

- CLR execution model

- Garbage Collection (managed memory)

- Change tracking in Entity Framework

- Deferred execution vs immediate execution in LINQ

- IDisposable pattern awareness (DbContext lifecycle)

🗄️ Database Design & Data Access

- Entity Framework Code First

- Strongly typed domain models

Relationships:

- One Employee → Many Salary Records

- LINQ queries translated into SQL

- Efficient filtering by date ranges for reports

- Avoids loading unnecessary data (performance-aware queries)

⚡ Performance & Throughput Awareness 

- Although this is a desktop application, the design reflects high-throughput backend thinking:

- Avoids UI thread blocking

- Uses ObservableCollection instead of reloading grids

- Uses date-range filtering instead of full table scans

Logic structured so it can move to:

- ASP.NET Core Web API

- Microservices

- Distributed data stores

# 🧪 Debugging & Diagnostics

Breakpoints placed in:

- Command execution

- Business logic methods

- LINQ aggregation logic

- Debugging data-binding issues using:

- INotifyPropertyChanged

- Output window binding errors

- Diagnosing performance bottlenecks in LINQ queries

- Verifying EF SQL translation via query inspection

# 🖨️ Reporting & Printing

- Implemented using WPF FlowDocument

- Structured tables with totals

Demonstrates:

- Document generation

- Separation of reporting logic

- Presentation-layer formatting

# 🚀 How to Run the Application

Prerequisites
- Visual Studio 2019+

- .NET Framework installed

- SQL Server (LocalDB or Express)

Steps:

``` Clone the repository: git clone https://github.com/your-username/sales-tracking-system.git ```


- Open solution in Visual Studio

- Update the connection string in App.config

- Run the application

- Database is created automatically via EF Code First
  
# 🔄 Scalability & Future Evolution

This application is intentionally designed to evolve into:

- ASP.NET Core Web API

- REST-based backend services

- Distributed caching

- Cloud-hosted SQL (Azure)

- CI/CD pipelines

- The ViewModel logic maps cleanly to service-layer logic in APIs.


# ⭐ Final Note

This project is not just a CRUD app.

It is a foundation-level system that demonstrates:

- Engineering discipline

- Backend thinking

- Production awareness

- Readiness for high-scale environments.

# 👨‍💻 Author

Suba Suresh

Aspiring C# / .NET Software Engineer
Focused on backend systems, performance-aware design, and scalable architectures


