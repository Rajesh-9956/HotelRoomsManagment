# 🛏️ Room Management System (Fullstack ASP.NET  MVC + API)

A full-stack Room Management system built using **ASP.NET Core **, leveraging a clean architecture that separates concerns between API services and the MVC client. It allows Admins to fully manage rooms and beds while regular users have read-only access.

---

## 🔧 Tech Stack

- **Backend**: ASP.NET Core Web API  
- **Frontend**: ASP.NET Core MVC 
- **Authentication**: ASP.NET Core Identity  
- **Authorization**: Role-based (`Admin`, `User`)  
- **Tokens**: JWT + Refresh Token (stored in **cookies**)  
- **Database**: SQL Server (LocalDB) using EF Core  
- **Design Patterns**: Clean separation, Generic API Responses  
- **Extra Features**: Pagination, Centralized Error Handling

---

## 🛡️ Authentication & Roles

The system uses **JWT authentication with refresh tokens**, where access tokens are issued on login, and **refresh tokens are securely stored in HttpOnly cookies**.

### Roles:
- 🔑 `Admin`  
  - Full CRUD operations on Rooms and Beds  
  - Can view users and manage the system

- 👤 `User`  
  - Can only **view** rooms and beds (read-only access)

---

## 📦 Features

✅ JWT + Refresh Token (cookie-based)

✅ ASP.NET Core Identity for user management

✅ Role-based Access Control (Admin/User)

✅ Generic API Responses for consistent output

✅ EF Core Relationships (Room ➝ Beds)

✅ Pagination Support

✅ Centralized Error Handling Middleware

✅ Secure MVC UI for both admin and user workflows

---
## 🚀 Getting Started

### 🖥️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server LocalDB installed and running

---

### 📥 Clone & Run

```bash
# Clone the repository
git clone https://github.com/aymanyassien/RoomManagementSystem.git

# Navigate into the project folder
cd RoomManagementSystem

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run

```
---
## 📂 Project Structure

```plaintext
RoomManagementSolution/           # Solution root folder
├── RoomManagement.API/          # ASP.NET Core Web API project (Backend services)
│   ├── Controllers/             # API endpoints
│   ├── DTOs/                    # Data Transfer Objects for requests/responses
│   ├── Middleware/              # Custom error handling, JWT config
│   ├── Services/                # Business logic and service layer
│   └── Program.cs               # App bootstrap (entry point)
│
├── RoomManagement.MVC/          # ASP.NET MVC project (Frontend)
│   ├── Controllers/             # MVC controllers for views
│   ├── Views/                   # Razor Views for Admin/User interfaces
│   ├── wwwroot/                 # Static files (CSS, JS)
│   └── Startup.cs               # MVC app configuration
│
└── RoomManagement.Domain/       # Domain layer (shared between API & MVC)
    ├── Entities/                # Core models like Room, Bed, ApplicationUser
    ├── Enums/                   # Role enums or constants
    └── Interfaces/              # Optional: Repository/service interfaces
```
---
## App in Action

🎥 [Download Demo Video](https://drive.google.com/drive/folders/1ydYs_Lhc_3q1mA54-g8jTeVL_UgK0kjq)

<br><br>

<img width="1440" alt="api_1" src="https://github.com/user-attachments/assets/95b2336d-c6e2-4500-890a-70b025deaef8" />


<img width="1440" alt="api_2" src="https://github.com/user-attachments/assets/9516bc8a-3df0-4f09-b88a-1bd916b4d9ba" />



