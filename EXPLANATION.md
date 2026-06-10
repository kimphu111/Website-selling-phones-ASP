# PhoneShop Management System — Giải Thích Chi Tiết Kiến Trúc & Luồng Hoạt Động

## Mục lục
1. [Tổng quan kiến trúc](#1-tổng-quan-kiến-trúc)
2. [Cấu trúc project](#2-cấu-trúc-project)
3. [Luồng hoạt động tổng thể (Flow Diagram)](#3-luồng-hoạt-động-tổng-thể)
4. [Authentication & Authorization (JWT)](#4-authentication--authorization)
5. [Giải thích từng Design Pattern](#5-giải-thích-từng-design-pattern)
6. [Luồng CRUD Phone chi tiết](#6-luồng-crud-phone-chi-tiết)
7. [Cách FE giao tiếp với BE](#7-cách-fe-giao-tiếp-với-be)
8. [Database Schema](#8-database-schema)

---

## 1. Tổng quan kiến trúc

Dự án được thiết kế theo mô hình **N-Layers Architecture** (Client-Server), tách biệt hoàn toàn Frontend (FE) và Backend (BE).

### Sơ đồ kiến trúc

```
┌─────────────────────────────────────────────────────────────┐
│  🌐 Client (Port 5298)                                      │
│  ┌─────────────────────────────────┐                        │
│  │  📱 PhoneShopApp.FE.Web          │                        │
│  │  ASP.NET Core MVC (Razor Views)  │                        │
│  │  Controllers → BackendApiClient  │                        │
│  └────────────┬────────────────────┘                        │
└───────────────┼─────────────────────────────────────────────┘
                │  HTTP REST API (JSON)
                │  JWT Bearer Token
                ▼
┌─────────────────────────────────────────────────────────────┐
│  ☁️ Server (Port 5201)                                       │
│  ┌─────────────────────────────────┐                        │
│  │  🎯 PhoneShopApp.BE.Api          │                        │
│  │  Web API Controllers            │                        │
│  │  [Authorize] JWT Authentication │                        │
│  └────────────┬────────────────────┘                        │
│               ▼                                              │
│  ┌─────────────────────────────────┐                        │
│  │  🧩 PhoneShopFacade (Facade)    │                        │
│  └────────────┬────────────────────┘                        │
│               ▼                                              │
│  ┌─────────────────────────────────┐                        │
│  │  ⚙️ PhoneShopApp.BE.Services    │                        │
│  │  + Singleton (JWT)              │                        │
│  │  + Factory Method (Search)      │                        │
│  │  + State (Status Review)        │                        │
│  │  + Iterator (Phone Collection)  │                        │
│  └────────────┬────────────────────┘                        │
│               ▼                                              │
│  ┌─────────────────────────────────┐                        │
│  │  🗄️ PhoneShopApp.BE.Infrastructure│                      │
│  │  EF Core + Pomelo MySQL          │                        │
│  │  Repositories → DbContext        │                        │
│  └────────────┬────────────────────┘                        │
└───────────────┼─────────────────────────────────────────────┘
                │  SQL
                ▼
         ┌──────────────┐
         │  💾 MySQL     │
         │ phone_shop_app│
         └──────────────┘
```

### Các Layer:

| Layer | Vai trò | Công nghệ |
|-------|---------|-----------|
| **FE.Web** | Giao diện người dùng, gọi API qua HTTP | ASP.NET Core MVC (Razor Views) |
| **BE.Api** | Đầu vào API, nhận request từ FE, trả JSON | ASP.NET Core Web API, JWT Auth |
| **BE.Services** | Logic nghiệp vụ, Design Patterns | C# Classes, Dependency Injection |
| **BE.Infrastructure** | Kết nối DB, Repository, DbContext | EF Core + Pomelo MySQL |
| **BE.Core** | Định nghĩa chung: Entity, DTO, Enum | C# Class Library |

---

## 2. Cấu trúc project

```
📁 ProjectTIen/
├── 📄 PhoneShopApp.sln              # Solution file (dùng để mở VS)
├── 📄 README.md                     # Hướng dẫn cài đặt
├── 📄 EXPLANATION.md                # File này — giải thích chi tiết
├── 📄 data.sql                      # Script tạo database MySQL
│
├── 📁 BE/
│   ├── 📁 PhoneShopApp.BE.Api/              # 🎯 Web API
│   │   ├── Program.cs                       # Entry point, DI, auth config
│   │   ├── appsettings.json                 # Connection string, JWT config
│   │   ├── Properties/launchSettings.json   # Port 5201
│   │   └── Controllers/
│   │       ├── AuthController.cs            # POST /api/auth/login
│   │       ├── PhonesController.cs          # CRUD: GET/POST/PUT/PATCH/DELETE
│   │       └── UsersController.cs           # GET /api/users
│   │
│   ├── 📁 PhoneShopApp.BE.Core/              # 📦 Core
│   │   ├── Entities/
│   │   │   ├── Phone.cs                     # Entity Phone
│   │   │   └── AppUser.cs                   # Entity User
│   │   ├── Enums/
│   │   │   └── PhoneStatus.cs               # Pending, Approved, Rejected
│   │   ├── DTOs/                            # Data Transfer Objects
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── LoginResponseDto.cs
│   │   │   ├── CreatePhoneDto.cs
│   │   │   ├── UpdatePhoneDto.cs
│   │   │   ├── UpdatePhoneStatusDto.cs
│   │   │   ├── PhoneDto.cs
│   │   │   ├── CreateUserDto.cs
│   │   │   └── UserDto.cs
│   │   └── Interfaces/Repositories/
│   │       ├── IPhoneRepository.cs          # Interface cho PhoneRepo
│   │       └── IAppUserRepository.cs        # Interface cho UserRepo
│   │
│   ├── 📁 PhoneShopApp.BE.Infrastructure/    # 🗄️ Data Access
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs              # DbContext (EF Core)y

│   │   │   └── DbSeeder.cs                 # Seed dữ liệu mẫu
│   │   ├── Repositories/
│   │   │   ├── PhoneRepository.cs           # Implement IPhoneRepository
│   │   │   └── AppUserRepository.cs         # Implement IAppUserRepository
│   │   └── DependencyInjection/
│   │       └── ServiceCollectionExtensions.cs # Đăng ký DI cho Infrastructure
│   │
│   └── 📁 PhoneShopApp.BE.Services/          # ⚙️ Business Logic
│       ├── Interfaces/
│       │   ├── IAuthService.cs
│       │   ├── IUserService.cs
│       │   ├── IPhoneService.cs
│       │   └── IPhoneShopFacade.cs          # Interface Facade Pattern
│       ├── Implementations/
│       │   ├── AuthService.cs               # Xử lý login
│       │   ├── PasswordHasher.cs            # Hash password SHA256
│       │   ├── UserService.cs               # CRUD User
│       │   └── PhoneService.cs              # CRUD Phone + Patterns
│       ├── Patterns/
│       │   ├── Singleton/
│       │   │   └── JwtTokenGeneratorSingleton.cs
│       │   ├── Factory/
│       │   │   ├── IPhoneSearchStrategy.cs
│       │   │   ├── ModelSearchStrategy.cs
│       │   │   ├── BrandSearchStrategy.cs
│       │   │   ├── AllFieldSearchStrategy.cs
│       │   │   └── PhoneSearchFactoryMethod.cs
│       │   ├── Facade/
│       │   │   └── PhoneShopFacade.cs
│       │   ├── State/
│       │   │   ├── IPhoneReviewState.cs
│       │   │   ├── PendingPhoneState.cs
│       │   │   ├── ApprovedPhoneState.cs
│       │   │   ├── RejectedPhoneState.cs
│       │   │   └── PhoneReviewStateContext.cs
│       │   └── Iterator/
│       │       ├── IPhoneIterator.cs
│       │       └── PhoneCollection.cs
│       └── DependencyInjection/
│           └── ServiceCollectionExtensions.cs # Đăng ký DI cho Services
│
└── 📁 FE/
    └── 📁 PhoneShopApp.FE.Web/              # 🌐 MVC Frontend
        ├── Program.cs                       # Entry point, DI, Session
        ├── appsettings.json                 # BackendApi.BaseUrl
        ├── Properties/launchSettings.json   # Port 5298
        ├── Controllers/
        │   ├── AuthController.cs            # Login/Logout
        │   ├── HomeController.cs            # Trang chủ
        │   ├── PhonesController.cs          # CRUD Phone (gọi BE API)
        │   └── UsersController.cs           # Quản lý User
        ├── Models/
        │   ├── Auth/
        │   │   ├── LoginViewModel.cs
        │   │   └── LoginResponseViewModel.cs
        │   ├── Phones/
        │   │   ├── PhoneViewModel.cs
        │   │   └── CreatePhoneViewModel.cs
        │   └── Users/
        │       └── UserViewModel.cs
        ├── Services/
        │   └── BackendApiClient.cs          # 🌉 CẦU NỐI FE-BE
        └── Views/
            ├── Auth/Login.cshtml
            ├── Phones/Index.cshtml
            ├── Users/Index.cshtml
            └── Shared/_Layout.cshtml
```

---

## 3. Luồng hoạt động tổng thể

### 3a. Luồng Đăng nhập

```
👤 Người dùng         🌐 FE Browser        🎬 FE Controller        🌉 BackendApiClient        🎯 BE API Controller        🔐 AuthService        🏭 JwtTokenGenerator (Singleton)        🗄️ UserRepository        💾 MySQL
    │                    │                      │                         │                            │                        │                    │                               │                      │
    │ 1. Nhập user/pass  │                      │                         │                            │                        │                    │                               │                      │
    │───────────────────>│                      │                         │                            │                        │                    │                               │                      │
    │                    │ 2. POST /Auth/Login   │                         │                            │                        │                    │                               │                      │
    │                    │─────────────────────>│                         │                            │                        │                    │                               │                      │
    │                    │                      │ 3. LoginAsync(user,pass) │                            │                        │                    │                               │                      │
    │                    │                      │────────────────────────>│                            │                        │                    │                               │                      │
    │                    │                      │                         │ 4. POST /api/auth/login     │                        │                    │                               │                      │
    │                    │                      │                         │───────────────────────────>│                        │                    │                               │                      │
    │                    │                      │                         │                            │ 5. LoginAsync(request)  │                    │                               │                      │
    │                    │                      │                         │                            │───────────────────────>│                    │                               │                      │
    │                    │                      │                         │                            │                        │ 6. GetByUsername()  │                               │                      │
    │                    │                      │                         │                            │                        │───────────────────>│                               │                      │
    │                    │                      │                         │                            │                        │                    │ 7. SELECT * FROM users        │                      │
    │                    │                      │                         │                            │                        │                    │──────────────────────────────>│                      │
    │                    │                      │                         │                            │                        │                    │<──────────────────────────────│ 8. User data        │
    │                    │                      │                         │                            │                        │<───────────────────│                               │                      │
    │                    │                      │                         │                            │                        │                    │                               │                      │
    │                    │                      │                         │                            │                        │ 9. GenerateToken() │                               │                      │
    │                    │                      │                         │                            │                        │───────────────────>│                               │                      │
    │                    │                      │                         │                            │                        │<───────────────────│ 10. { Token, Role }          │                      │
    │                    │                      │                         │                            │<───────────────────────│                    │                               │                      │
    │                    │                      │                         │ 11. HTTP 200 + JSON         │                        │                    │                               │                      │
    │                    │                      │                         │<───────────────────────────│                        │                    │                               │                      │
    │                    │                      │ 12. LoginResponseVM      │                            │                        │                    │                               │                      │
    │                    │                      │<────────────────────────│                            │                        │                    │                               │                      │
    │                    │                      │ 13. Lưu Token+Role vào Session                        │                        │                    │                               │                      │
    │                    │ 14. Redirect /Phones │                         │                            │                        │                    │                               │                      │
    │                    │<─────────────────────│                         │                            │                        │                    │                               │                      │
    │  15. Xem danh sách │                      │                         │                            │                        │                    │                               │                      │
    │<───────────────────│                      │                         │                            │                        │                    │                               │                      │
```

### 3b. Luồng Xem danh sách Phone (có Search + Iterator)

```
🌐 FE Browser        🎬 FE PhonesController        🌉 BackendApiClient        🎯 BE PhonesController        🧩 PhoneShopFacade        ⚙️ PhoneService        🏭 SearchFactory        🔄 Iterator        🗄️ PhoneRepository        💾 MySQL
    │                      │                            │                          │                           │                       │                    │                  │                   │                      │
    │ 1. GET /Phones       │                            │                          │                           │                       │                    │                  │                   │                      │
    │─────────────────────>│                            │                          │                           │                       │                    │                  │                   │                      │
    │                      │ 2. Lấy token từ Session    │                          │                           │                       │                    │                  │                   │                      │
    │                      │ 3. SearchPhonesAsync()     │                          │                           │                       │                    │                  │                   │                      │
    │                      │───────────────────────────>│                          │                           │                       │                    │                  │                   │                      │
    │                      │                            │ 4. GET /api/phones      │                           │                       │                    │                  │                   │                      │
    │                      │                            │  + Authorization: Bearer │                           │                       │                    │                  │                   │                      │
    │                      │                            │─────────────────────────>│                           │                       │                    │                  │                   │                      │
    │                      │                            │                          │ 5. SearchPhonesAsync()    │                       │                    │                  │                   │                      │
    │                      │                            │                          │─────────────────────────>│                       │                    │                  │                   │                      │
    │                      │                            │                          │                           │ 6. SearchAsync()      │                    │                  │                   │                      │
    │                      │                            │                          │                           │─────────────────────>│                    │                  │                   │                      │
    │                      │                            │                          │                           │                       │ 7. ListAsync()     │                  │                   │                      │
    │                      │                            │                          │                           │                       │─────────────────────────────────────────>│                      │
    │                      │                            │                          │                           │                       │                    │                  │                   │ 8. SELECT * FROM phones  │
    │                      │                            │                          │                           │                       │                    │                  │                   │──────────────────────>│
    │                      │                            │                          │                           │                       │                    │                  │                   │<──────────────────────│ 9. List<Phone>  │
    │                      │                            │                          │                           │                       │<──────────────────────────────────────────│                      │
    │                      │                            │                          │                           │                       │                    │                  │                   │                      │
    │                      │                            │                          │                           │                       │ 10. Nếu có keyword │                  │                   │                      │
    │                      │                            │                          │                           │                       │─────> searchBy=?  │                  │                   │                      │
    │                      │                            │                          │                           │                       │      │              │                  │                   │                      │
    │                      │                            │                          │                           │                       │      ├─ "model" → ModelSearchStrategy      │                      │
    │                      │                            │                          │                           │                       │      ├─ "brand" → BrandSearchStrategy     │                      │
    │                      │                            │                          │                           │                       │      └─ else → AllFieldSearchStrategy    │                      │
    │                      │                            │                          │                           │                       │                    │                  │                   │                      │
    │                      │                            │                          │                           │                       │ 11. strategy.Apply() lọc danh sách     │                      │
    │                      │                            │                          │                           │                       │                    │                  │                   │                      │
    │                      │                            │                          │                           │                       │ 12. Tạo PhoneCollection + Iterator     │                      │
    │                      │                            │                          │                           │                       │─────────────────────────────────>│                   │                      │
    │                      │                            │                          │                           │                       │                    │                  │                   │                      │
    │                      │                            │                          │                           │                       │ 13. while(it.HasNext()) { it.Next() }  │                      │
    │                      │                            │                          │                           │                       │<─────────────────────────────────│                   │                      │
    │                      │                            │                          │                           │                       │ 14. Map(Phone → PhoneDto)             │                      │
    │                      │                            │                          │                           │<─────────────────────│                    │                  │                   │                      │
    │                      │                            │                          │ 15. List<PhoneDto>         │                       │                    │                  │                   │                      │
    │                      │                            │                          │<─────────────────────────│                       │                    │                  │                   │                      │
    │                      │                            │ 16. HTTP 200 + JSON      │                           │                       │                    │                  │                   │                      │
    │                      │                            │<─────────────────────────│                           │                       │                    │                  │                   │                      │
    │                      │ 17. List<PhoneViewModel>    │                          │                           │                       │                    │                  │                   │                      │
    │                      │<───────────────────────────│                          │                           │                       │                    │                  │                   │                      │
    │ 18. Render View      │                            │                          │                           │                       │                    │                  │                   │                      │
    │<─────────────────────│                            │                          │                           │                       │                    │                  │                   │                      │
```

### 3c. Luồng Phê duyệt/Từ chối Phone (State Pattern)

```
👑 Admin          🌐 FE Browser        🎬 PhonesController        🌉 BackendApiClient        🎯 API PhonesController        🧩 Facade        ⚙️ PhoneService        🔄 State Context        🗄️ PhoneRepository        💾 MySQL
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │ 1. Click Approve │                      │                           │                            │                     │                │                    │                      │                    │
    │────────────────>│                      │                           │                            │                     │                │                    │                      │                    │
    │                 │ 2. POST /UpdateStatus │                           │                            │                     │                │                    │                      │                    │
    │                 │  id=5, status=approve │                           │                            │                     │                │                    │                      │                    │
    │                 │─────────────────────>│                           │                            │                     │                │                    │                      │                    │
    │                 │                      │ 3. UpdatePhoneStatusAsync  │                            │                     │                │                    │                      │                    │
    │                 │                      │───────────────────────────>│                            │                     │                │                    │                      │                    │
    │                 │                      │                           │ 4. PATCH /api/phones/5/status│                     │                │                    │                      │                    │
    │                 │                      │                           │  Body: {"action":"approve"}  │                     │                │                    │                      │                    │
    │                 │                      │                           │───────────────────────────>│                     │                │                    │                      │                    │
    │                 │                      │                           │                            │ 5. UpdatePhoneStatus │                │                    │                      │                    │
    │                 │                      │                           │                            │────────────────────>│                │                    │                      │                    │
    │                 │                      │                           │                            │                     │ 6. UpdateStatus  │                    │                      │                    │
    │                 │                      │                           │                            │                     │───────────────>│                    │                      │                    │
    │                 │                      │                           │                            │                     │                │ 7. GetByIdAsync(5) │                      │                    │
    │                 │                      │                           │                            │                     │                │─────────────────────────────────────────>│                    │
    │                 │                      │                           │                            │                     │                │                    │                      │ 8. SELECT phone    │
    │                 │                      │                           │                            │                     │                │                    │                      │───────────────────>│
    │                 │                      │                           │                            │                     │                │                    │                      │<───────────────────│                    │
    │                 │                      │                           │                            │                     │                │<──────────────────────────────────────────│                    │
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │                 │                      │                           │                            │                     │                │ 9. new PhoneReviewStateContext(Pending)   │                    │
    │                 │                      │                           │                            │                     │                │───────────────────>│                      │                    │
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │                 │                      │                           │                            │                     │                │10. ApplyAction(phone, "approve")          │                    │
    │                 │                      │                           │                            │                     │                │───────────────────>│                      │                    │
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │                 │                      │                           │                            │                     │                │                    │ ⚡ _state.Approve(phone)                  │                    │
    │                 │                      │                           │                            │                     │                │                    │  → phone.Status = Approved               │                    │
    │                 │                      │                           │                            │                     │                │                    │ ⚡ _state = new ApprovedPhoneState()      │                    │
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │                 │                      │                           │                            │                     │                │<───────────────────│                      │                    │
    │                 │                      │                           │                            │                     │                │                    │                      │                    │
    │                 │                      │                           │                            │                     │                │11. UpdateAsync(phone)                     │                    │
    │                 │                      │                           │                            │                     │                │─────────────────────────────────────────>│                    │
    │                 │                      │                           │                            │                     │                │                    │                      │12. UPDATE phones   │
    │                 │                      │                           │                            │                     │                │                    │                      │  SET status='Approved'                    │
    │                 │                      │                           │                            │                     │                │                    │                      │───────────────────>│
    │                 │                      │                           │                            │                     │                │                    │                      │<───────────────────│                    │
    │                 │                      │                           │                            │                     │                │<──────────────────────────────────────────│                    │
    │                 │                      │                           │                            │                     │<───────────────│                    │                      │                    │
    │                 │                      │                           │ 13. HTTP 200                │                     │                │                    │                      │                    │
    │                 │                      │                           │<───────────────────────────│                     │                │                    │                      │                    │
    │                 │                      │ 14. true                  │                            │                     │                │                    │                      │                    │
    │                 │                      │<───────────────────────────│                            │                     │                │                    │                      │                    │
    │                 │ 15. Redirect /Phones  │                           │                            │                     │                │                    │                      │                    │
    │                 │<─────────────────────│                           │                            │                     │                │                    │                      │                    │
    │ 16. Thấy Approved│                      │                           │                            │                     │                │                    │                      │                    │
    │<────────────────│                      │                           │                            │                     │                │                    │                      │                    │
```

---

## 4. Authentication & Authorization

### 4.1 Cơ chế JWT

```
                      JWT Token
┌────────────────────────────────────────────────────┐
│  HEADER                                             │
│  { "alg": "HS256", "typ": "JWT" }                  │
├────────────────────────────────────────────────────┤
│  PAYLOAD (Claims)                                   │
│  ├─ ClaimTypes.NameIdentifier → User.Id             │
│  ├─ ClaimTypes.Name          → User.Username        │
│  └─ ClaimTypes.Role          → User.Role            │
├────────────────────────────────────────────────────┤
│  SIGNATURE                                          │
│  HMACSHA256( base64(Header) + "." + base64(Payload) │
│             + Jwt:Key )                             │
└────────────────────────────────────────────────────┘
```

### 4.2 Cơ chế hoạt động

| Bước | Mô tả | File |
|------|-------|------|
| 1 | User nhập username/password, FE gửi POST `/api/auth/login` | [`AuthController.cs`](BE/PhoneShopApp.BE.Api/Controllers/AuthController.cs) (BE) |
| 2 | BE kiểm tra user + hash password, gọi Singleton tạo JWT | [`AuthService.cs`](BE/PhoneShopApp.BE.Services/Implementations/AuthService.cs) |
| 3 | Singleton `JwtTokenGeneratorSingleton` tạo JWT với 3 claims: userId, username, role | [`JwtTokenGeneratorSingleton.cs`](BE/PhoneShopApp.BE.Services/Patterns/Singleton/JwtTokenGeneratorSingleton.cs) |
| 4 | FE nhận JWT, lưu vào **Session** (không phải LocalStorage) | [`AuthController.cs`](FE/PhoneShopApp.FE.Web/Controllers/AuthController.cs) (FE) |
| 5 | Mỗi request FE → BE, gắn header `Authorization: Bearer {token}` | [`BackendApiClient.cs`](FE/PhoneShopApp.FE.Web/Services/BackendApiClient.cs) |
| 6 | BE middleware `JwtBearer` verify token, set `User.Identity` | [`Program.cs`](BE/PhoneShopApp.BE.Api/Program.cs) (BE) |
| 7 | `[Authorize]` check token hợp lệ, `[Authorize(Roles="Admin")]` check role trong claim | [`PhonesController.cs`](BE/PhoneShopApp.BE.Api/Controllers/PhonesController.cs) (BE) |

### 4.3 Code minh họa

**JWT Config trong** [`Program.cs`](BE/PhoneShopApp.BE.Api/Program.cs):
```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
```

**Singleton** [`JwtTokenGeneratorSingleton.cs`](BE/PhoneShopApp.BE.Services/Patterns/Singleton/JwtTokenGeneratorSingleton.cs):
```csharp
// Chỉ có 1 instance duy nhất trong toàn ứng dụng
public sealed class JwtTokenGeneratorSingleton
{
    private static readonly Lazy<JwtTokenGeneratorSingleton> LazyInstance =
        new(() => new JwtTokenGeneratorSingleton());
    
    public static JwtTokenGeneratorSingleton Instance => LazyInstance.Value;
    private JwtTokenGeneratorSingleton() { } // private constructor

    public LoginResponseDto GenerateToken(AppUser user, IConfiguration configuration)
    {
        // Tạo claims: userId, username, role
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };
        // ... tạo JWT + trả về Token + Role + ExpiresAt
    }
}
```

---

## 5. Giải thích từng Design Pattern

### 5.1 🔶 Singleton — JwtTokenGeneratorSingleton

**File**: [`JwtTokenGeneratorSingleton.cs`](BE/PhoneShopApp.BE.Services/Patterns/Singleton/JwtTokenGeneratorSingleton.cs)

**Mục đích**: Đảm bảo chỉ có **một đối tượng duy nhất** để tạo JWT token.

**Cách hoạt động**:
```
┌────────────────────────────────────────┐
│  JwtTokenGeneratorSingleton             │
│  ────────────────────────              │
│  -instance: Lazy<Singleton> (static)    │
│  ────────────────────────              │
│  +Instance: Singleton (static property) │
│  ────────────────────────              │
│  +GenerateToken(user, config): DTO     │
│  ────────────────────────              │
│  -JwtTokenGeneratorSingleton()         │
│   (private constructor)                │
└────────────────────────────────────────┘
         ▲
         │ chỉ có 1 instance
         │
   AuthService ──gọi──> Instance.GenerateToken()
```

- **Constructor là `private`** → không ai tạo instance mới được
- **`Lazy<T>`** → thread-safe, chỉ tạo instance khi lần đầu gọi `Instance`
- **Stateless** → không cần nhiều instance, 1 cái dùng chung là đủ

### 5.2 🔶 Factory Method — PhoneSearchFactoryMethod

**Các file**: 
- [`IPhoneSearchStrategy.cs`](BE/PhoneShopApp.BE.Services/Patterns/Factory/IPhoneSearchStrategy.cs) — Interface
- [`ModelSearchStrategy.cs`](BE/PhoneShopApp.BE.Services/Patterns/Factory/ModelSearchStrategy.cs) — Tìm theo model
- [`BrandSearchStrategy.cs`](BE/PhoneShopApp.BE.Services/Patterns/Factory/BrandSearchStrategy.cs) — Tìm theo brand
- [`AllFieldSearchStrategy.cs`](BE/PhoneShopApp.BE.Services/Patterns/Factory/AllFieldSearchStrategy.cs) — Tìm tất cả
- [`PhoneSearchFactoryMethod.cs`](BE/PhoneShopApp.BE.Services/Patterns/Factory/PhoneSearchFactoryMethod.cs) — Factory

**Sơ đồ**:
```
                    ┌────────────────────────┐
                    │  <<interface>>          │
                    │  IPhoneSearchStrategy   │
                    │  ────────────────────  │
                    │  +Apply(phones, keyword)│
                    └───────────┬────────────┘
                                │ implements
            ┌───────────────────┼───────────────────┐
            ▼                   ▼                   ▼
┌────────────────────┐ ┌────────────────┐ ┌────────────────────┐
│ ModelSearchStrategy│ │BrandSearchStrategy││AllFieldSearchStrategy│
│ ───────────────── │ │────────────────│ │──────────────────── │
│ phone.Model        │ │ phone.Brand     │ │ Tất cả các field    │
│ .Contains(kw)      │ │ .Contains(kw)   │ │ (Model/Brand/Specs) │
└────────────────────┘ └────────────────┘ └────────────────────┘
                    ▲                   ▲                   ▲
                    │                   │                   │
                    └───────────────────┼───────────────────┘
                                        │
                        ┌───────────────┴──────────────┐
                        │  PhoneSearchFactoryMethod     │
                        │  ──────────────────────────  │
                        │  +Create(searchBy): Strategy  │
                        │    "model" → ModelSearch      │
                        │    "brand" → BrandSearch      │
                        │    else    → AllFieldSearch   │
                        └──────────────────────────────┘
```

**Code Factory**:
```csharp
public class PhoneSearchFactoryMethod
{
    public virtual IPhoneSearchStrategy Create(string? searchBy)
    {
        return searchBy?.ToLowerInvariant() switch
        {
            "model" => new ModelSearchStrategy(),
            "brand" => new BrandSearchStrategy(),
            _ => new AllFieldSearchStrategy()
        };
    }
}
```

**Cách dùng trong** [`PhoneService.cs`](BE/PhoneShopApp.BE.Services/Implementations/PhoneService.cs):
```csharp
// Factory Method Pattern: tạo strategy phù hợp
var strategy = _searchFactory.Create(searchBy);

// Strategy Pattern: áp dụng chiến lược lọc
phones = strategy.Apply(phones, keyword).ToList();
```

**Tại sao dùng Factory Method?** — Cho phép thêm chiến lược tìm kiếm mới (VD: search theo giá) mà không cần sửa code của `PhoneService` (nguyên lý Open/Closed).

### 5.3 🔶 Facade — PhoneShopFacade

**File**: [`PhoneShopFacade.cs`](BE/PhoneShopApp.BE.Services/Patterns/Facade/PhoneShopFacade.cs)

**Sơ đồ**:
```
┌─────────────────────────────────────────────────────────────────┐
│                     🎯 PhonesController                          │
│  Chỉ inject 1 dependency: IPhoneShopFacade                      │
│  Gọi: facade.SearchPhonesAsync(), facade.CreatePhoneAsync(), ...│
└──────────────────────────┬──────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────────┐
│  🧩 PhoneShopFacade (Facade)                                    │
│  ──────────────────────────────────────────                    │
│  - userService: IUserService              │                    │
│  - phoneService: IPhoneService            │                    │
│  ──────────────────────────────────────────                    │
│  +SearchPhonesAsync()   → phoneService.SearchAsync()            │
│  +CreatePhoneAsync()    → phoneService.CreateAsync()            │
│  +UpdatePhoneAsync()    → phoneService.UpdateAsync()            │
│  +UpdatePhoneStatusAsync() → phoneService.UpdateStatusAsync()   │
│  +DeletePhoneAsync()    → phoneService.DeleteAsync()            │
│  +GetUsersAsync()       → userService.ListAsync()               │
│  +CreateUserAsync()     → userService.CreateAsync()             │
└──────────┬──────────────────────────────────┬───────────────────┘
           │                                  │
           ▼                                  ▼
┌──────────────────────┐       ┌──────────────────────┐
│  📱 PhoneService      │       │  👤 UserService       │
│  SearchAsync()        │       │  ListAsync()          │
│  CreateAsync()        │       │  CreateAsync()        │
│  UpdateAsync()        │       └──────────────────────┘
│  UpdateStatusAsync()  │
│  DeleteAsync()        │
└──────────────────────┘
```

**Lợi ích**:
1. Controller chỉ cần inject **1 dependency** thay vì nhiều
2. Nếu sau này thêm logic (logging, caching, transaction), chỉ cần sửa Facade
3. Giấu complexity của business logic khỏi Controller

### 5.4 🔶 State — PhoneReviewState

**Các file**:
- [`IPhoneReviewState.cs`](BE/PhoneShopApp.BE.Services/Patterns/State/IPhoneReviewState.cs) — Interface
- [`PendingPhoneState.cs`](BE/PhoneShopApp.BE.Services/Patterns/State/PendingPhoneState.cs)
- [`ApprovedPhoneState.cs`](BE/PhoneShopApp.BE.Services/Patterns/State/ApprovedPhoneState.cs)
- [`RejectedPhoneState.cs`](BE/PhoneShopApp.BE.Services/Patterns/State/RejectedPhoneState.cs)
- [`PhoneReviewStateContext.cs`](BE/PhoneShopApp.BE.Services/Patterns/State/PhoneReviewStateContext.cs) — Context

**Sơ đồ State Machine**:
```
                    ┌──────────┐
                    │  Pending  │  ← Trạng thái khởi tạo
                    └─────┬────┘
                          │
              ┌───────────┼───────────┐
              ▼                       ▼
        ┌──────────┐           ┌──────────┐
        │ Approved │           │ Rejected │  ← Terminal states
        └──────────┘           └──────────┘
  (Không thể đổi)       (Không thể đổi)
```

**Cách mỗi State xử lý**:
```
              PendingPhoneState                ApprovedPhoneState              RejectedPhoneState
         ┌──────────────────────┐        ┌──────────────────────┐       ┌──────────────────────┐
Approve  │ ✅ phone.Status =    │        │ ❌ throw Exception   │       │ ❌ throw Exception   │
         │   PhoneStatus.Approved│       │  "Already Approved"  │       │  "Already Rejected"  │
         └──────────────────────┘        └──────────────────────┘       └──────────────────────┘
         ┌──────────────────────┐        ┌──────────────────────┐       ┌──────────────────────┐
Reject   │ ✅ phone.Status =    │        │ ❌ throw Exception   │       │ ❌ throw Exception   │
         │   PhoneStatus.Rejected│       │  "Already Approved"  │       │  "Already Rejected"  │
         └──────────────────────┘        └──────────────────────┘       └──────────────────────┘
```

**Code Context**:
```csharp
public class PhoneReviewStateContext
{
    private IPhoneReviewState _state;

    public PhoneReviewStateContext(PhoneStatus status)
    {
        // Khởi tạo state dựa trên status hiện tại
        _state = status switch
        {
            PhoneStatus.Pending  => new PendingPhoneState(),
            PhoneStatus.Approved => new ApprovedPhoneState(),
            PhoneStatus.Rejected => new RejectedPhoneState(),
            _ => new PendingPhoneState()
        };
    }

    public void ApplyAction(Phone phone, string action)
    {
        if (action == "approve")
        {
            _state.Approve(phone);     // Gọi state hiện tại
            _state = new ApprovedPhoneState(); // Chuyển state
        }
        else if (action == "reject")
        {
            _state.Reject(phone);
            _state = new RejectedPhoneState();
        }
    }
}
```

**Tại sao dùng State?** — Kiểm soát chặt chẽ workflow nghiệp vụ: không thể approve 2 lần, không thể reject điện thoại đã approved.

### 5.5 🔶 Iterator — PhoneCollection / PhoneIterator

**Files**: [`IPhoneIterator.cs`](BE/PhoneShopApp.BE.Services/Patterns/Iterator/IPhoneIterator.cs), [`PhoneCollection.cs`](BE/PhoneShopApp.BE.Services/Patterns/Iterator/PhoneCollection.cs)

**Sơ đồ**:
```
┌──────────────────────────────────────────────────┐
│  Interface: IIterator<T>                         │
│  ────────────────────────                        │
│  +HasNext(): bool  → Còn phần tử tiếp theo?      │
│  +Next(): T        → Lấy phần tử tiếp theo       │
└──────────────────────────────────────────────────┘
                        ▲ implements
              ┌─────────┴──────────┐
              │  PhoneIterator      │
              │  ──────────────────│
              │  -_phones: List     │
              │  -_position: int    │
              │  ──────────────────│
              │  +HasNext()         │
              │  +Next()            │
              └────────────────────┘
                        ▲ created by
              ┌─────────┴──────────┐
              │  PhoneCollection    │
              │  ──────────────────│
              │  -_phones: List     │
              │  ──────────────────│
              │  +CreateIterator()  │
              └────────────────────┘
```

**Code**:
```csharp
// Iterator
public class PhoneIterator : IIterator<Phone>
{
    private readonly List<Phone> _phones;
    private int _position = 0;

    public bool HasNext() => _position < _phones.Count;
    public Phone Next() => _phones[_position++];
}

// Collection (Aggregate)
public class PhoneCollection : IPhoneCollection
{
    private readonly List<Phone> _phones;
    public IIterator<Phone> CreateIterator() => new PhoneIterator(_phones);
}
```

**Cách dùng trong** [`PhoneService.cs`](BE/PhoneShopApp.BE.Services/Implementations/PhoneService.cs):
```csharp
// Sử dụng Iterator Pattern để duyệt danh sách (yêu cầu của thầy)
var collection = new PhoneCollection(phones);
var iterator = collection.CreateIterator();
var result = new List<PhoneDto>();

while (iterator.HasNext())
{
    result.Add(Map(iterator.Next())); // Map entity → DTO
}

return result;
```

**Tại sao dùng Iterator?** — Duyệt danh sách một cách thống nhất, không phụ thuộc vào cấu trúc dữ liệu bên trong. Nếu đổi từ `List<Phone>` sang `Phone[]` hoặc `IEnumerable`, code duyệt vẫn không đổi.

---

## 6. Luồng CRUD Phone chi tiết

### 6.1 Bảng API Endpoints

| Phương thức | URL | Auth | FE Action | BE Action | Mô tả |
|------------|-----|------|-----------|-----------|-------|
| **GET** | `/api/phones?keyword=&searchBy=` | JWT | `Index()` | `Search()` | Xem danh sách + tìm kiếm |
| **POST** | `/api/phones` | Admin | `Create()` | `Create()` | Thêm mới điện thoại |
| **PUT** | `/api/phones/{id}` | Admin | `Edit()` | `Update()` | Sửa thông tin điện thoại |
| **PATCH** | `/api/phones/{id}/status` | Admin | `UpdateStatus()` | `UpdateStatus()` | Duyệt (approve) / Từ chối (reject) |
| **DELETE** | `/api/phones/{id}` | Admin | `Delete()` | `Delete()` | Xóa điện thoại |

### 6.2 Luồng dữ liệu qua các layer (Thêm Phone)

```
🌐 FE                          ☁️ BE                                     💾 DB
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│  Form    │───>│ FE       │───>│ BE       │───>│ Facade   │───>│ Service  │───>│ Repo     │───>│ MySQL    │
│ Input    │    │Controller│    │Controller│    │(Facade)  │    │(Service) │    │(EF Core) │    │          │
└──────────┘    └──────────┘    └──────────┘    └──────────┘    └──────────┘    └──────────┘    └──────────┘
Model,Brand       POST              POST            DTO              Entity          AddAsync      INSERT
Specs          /Phones/Create   /api/phones    CreatePhoneDto      new Phone()     SaveChanges    INTO phones
```

### 6.3 Chi tiết từng file quan trọng

**1. FE View** → [`FE/PhoneShopApp.FE.Web/Views/Phones/Index.cshtml`](FE/PhoneShopApp.FE.Web/Views/Phones/Index.cshtml)
- Modal "Add New Phone" → POST đến `PhonesController.Create()`
- Modal "Edit" → POST đến `PhonesController.Edit()`
- Nút Approve/Reject → POST đến `PhonesController.UpdateStatus()`
- Nút Delete → POST đến `PhonesController.Delete()`

**2. FE Controller** → [`FE/PhoneShopApp.FE.Web/Controllers/PhonesController.cs`](FE/PhoneShopApp.FE.Web/Controllers/PhonesController.cs)
- Lấy JWT token từ Session
- `ViewData["IsAdmin"]` để ẩn/hiện nút CRUD theo role
- Mỗi action gọi `BackendApiClient` tương ứng

**3. FE API Client** → [`FE/PhoneShopApp.FE.Web/Services/BackendApiClient.cs`](FE/PhoneShopApp.FE.Web/Services/BackendApiClient.cs)
- Dùng `IHttpClientFactory` để tạo HTTP client
- Tự động gắn `Authorization: Bearer {token}` vào header
- Serialize request → JSON, deserialize response → ViewModel

**4. BE Controller** → [`BE/PhoneShopApp.BE.Api/Controllers/PhonesController.cs`](BE/PhoneShopApp.BE.Api/Controllers/PhonesController.cs)
- `[Authorize]` → mọi endpoint đều cần JWT token
- `[Authorize(Roles = "Admin")]` → chỉ Admin mới được Create/Update/Delete
- Chỉ inject `IPhoneShopFacade` (1 dependency duy nhất)

**5. Facade** → [`BE/PhoneShopApp.BE.Services/Patterns/Facade/PhoneShopFacade.cs`](BE/PhoneShopApp.BE.Services/Patterns/Facade/PhoneShopFacade.cs)
- Chuyển tiếp request từ Controller xuống Service
- Nếu cần thêm logging/caching, chỉ cần sửa ở đây

**6. PhoneService** → [`BE/PhoneShopApp.BE.Services/Implementations/PhoneService.cs`](BE/PhoneShopApp.BE.Services/Implementations/PhoneService.cs)
- `SearchAsync()`: lấy DS → lọc (Factory) → duyệt (Iterator) → Map → trả về DTO
- `CreateAsync()`: DTO → Entity → Repository.Add → Map → trả về DTO
- `UpdateAsync()`: Lấy entity → cập nhật fields → Repository.Update → trả về
- `UpdateStatusAsync()`: Lấy entity → State Pattern → Repository.Update → trả về
- `DeleteAsync()`: Repository.Delete

**7. Repository** → [`BE/PhoneShopApp.BE.Infrastructure/Repositories/PhoneRepository.cs`](BE/PhoneShopApp.BE.Infrastructure/Repositories/PhoneRepository.cs)
- `ListAsync()`: `ORDER BY id DESC`
- `GetByIdAsync()`: `FirstOrDefault(id)`
- `AddAsync()`: `Add()` + `SaveChangesAsync()`
- `UpdateAsync()`: `Update()` + `SaveChangesAsync()`
- `DeleteAsync()`: `Find(id)` → `Remove()` → `SaveChangesAsync()`

---

## 7. Cách FE giao tiếp với BE

### 7.1 Cấu hình kết nối

**FE** [`appsettings.json`](FE/PhoneShopApp.FE.Web/appsettings.json):
```json
{
  "BackendApi": {
    "BaseUrl": "http://localhost:5201"
  }
}
```

**FE** [`Program.cs`](FE/PhoneShopApp.FE.Web/Program.cs):
```csharp
// Đăng ký HttpClient với tên "BackendApi" và BaseAddress là URL của BE
builder.Services.AddHttpClient("BackendApi", client =>
{
    var baseUrl = builder.Configuration["BackendApi:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(baseUrl);
});
```

### 7.2 Cách gọi API từ FE

Mỗi request từ FE đến BE đều qua [`BackendApiClient`](FE/PhoneShopApp.FE.Web/Services/BackendApiClient.cs):

```csharp
// 1. Lấy HTTP client từ factory (URL đã được cấu hình sẵn)
var client = _httpClientFactory.CreateClient("BackendApi");

// 2. Gắn JWT token vào Authorization header
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

// 3. Serialize dữ liệu thành JSON
var payload = JsonSerializer.Serialize(model);
var content = new StringContent(payload, Encoding.UTF8, "application/json");

// 4. Gọi API (POST, GET, PUT, PATCH, DELETE)
var response = await client.PostAsync("/api/phones", content, cancellationToken);

// 5. Kiểm tra kết quả (true/false)
return response.IsSuccessStatusCode;
```

### 7.3 Session Management

FE dùng **Session** để lưu trạng thái đăng nhập:

| Session Key | Giá trị | Mục đích |
|-------------|---------|----------|
| `jwt_token` | JWT token string | Gắn vào header mỗi request |
| `role` | "Admin" hoặc "User" | `ViewData["IsAdmin"]` để ẩn/hiện button |
| `jwt_expiry` | DateTime ISO 8601 | Kiểm tra token còn hạn |

**Login** ([`AuthController.cs`](FE/PhoneShopApp.FE.Web/Controllers/AuthController.cs)):
```csharp
HttpContext.Session.SetString("jwt_token", loginResult.Token);
HttpContext.Session.SetString("role", loginResult.Role);
```

**Logout**:
```csharp
HttpContext.Session.Clear();
return RedirectToAction("Login");
```

---

## 8. Database Schema

```sql
-- Tạo database
CREATE DATABASE phone_shop_app;
USE phone_shop_app;

-- Bảng users
CREATE TABLE users (
  id            INT AUTO_INCREMENT PRIMARY KEY,
  username      VARCHAR(100) NOT NULL UNIQUE,
  full_name     VARCHAR(200) NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  role          VARCHAR(50) NOT NULL
);

-- Bảng phones
CREATE TABLE phones (
  id            INT AUTO_INCREMENT PRIMARY KEY,
  model         VARCHAR(200) NOT NULL,
  brand         VARCHAR(200) NOT NULL,
  specifications VARCHAR(1000),
  status        VARCHAR(50) NOT NULL DEFAULT 'Pending'
);
```

**Lưu ý**: Enum `PhoneStatus` (Pending = 0, Approved = 1, Rejected = 2) được EF Core map thành **VARCHAR** trong MySQL thông qua `.HasConversion<string>()`.

---

## Tổng kết

Dự án này là một **ứng dụng ASP.NET Core hoàn chỉnh** với:

| Khía cạnh | Chi tiết |
|-----------|----------|
| **Kiến trúc** | N-Layers: FE (MVC) → BE (API) → Services (Logic) → Infrastructure (Data) → MySQL |
| **FE-BE tách biệt** | Giao tiếp qua REST API + JSON, port 5201 (BE) và 5298 (FE) |
| **Authentication** | JWT với Claims-based Authorization (Role Admin/User) |
| **Design Patterns** | Singleton (JWT), Factory Method (Search), Facade (API), State (Status), Iterator (Collection) |
| **Database** | MySQL với EF Core (Pomelo), seed data tự động khi chạy lần đầu |
| **CRUD** | Create, Read (Search), Update, Delete, UpdateStatus |
| **DI (Dependency Injection)** | Toàn bộ service/repository được inject qua constructor |


 Đã hoàn tất — File EXPLANATION.md giải thích chi tiết toàn bộ dự án
Tôi đã tạo file EXPLANATION.md với 8 phần, bao gồm:

📖 Nội dung file giải thích
Phần	Nội dung
1. Tổng quan kiến trúc	Sơ đồ N-Layers: FE (MVC) → BE (API) → Services → Infrastructure → MySQL
2. Cấu trúc project	Tree view đầy đủ từng file + chú thích vai trò
3. Luồng hoạt động	3 flow diagram dạng text: Login, Search + Iterator, Status Review + State Pattern — đi từ User click → FE → BE → DB → response về
4. Authentication & JWT	Cách JWT được tạo (Singleton), lưu (Session), gửi (Bearer header), xác thực (JwtBearer middleware)
5. 5 Design Patterns	Mỗi pattern có sơ đồ + code + tại sao dùng: Singleton, Factory Method, Facade, State, Iterator
6. CRUD Phone	Bảng API endpoints + luồng dữ liệu qua từng layer + chi tiết từng file
7. FE-BE giao tiếp	Cấu hình HttpClient, cách BackendApiClient gọi API, Session management
8. Database Schema	SQL CREATE TABLE cho users + phones
🎯 Tóm tắt cốt lõi để bạn hiểu bản chất:
1. FE chạy port 5298, BE chạy port 5201 — hoàn toàn độc lập.

2. FE không truy vấn DB, chỉ gọi REST API qua BackendApiClient với JWT trong header.

3. BE có 5 layer, dữ liệu đi qua đúng thứ tự:

Controller → Facade → Service → Repository → DbContext → MySQL

4. 5 Design Patterns nằm trong BE.Services:

Singleton: 1 instance JWT generator cho cả app
Factory Method: Chọn strategy search theo searchBy param
Facade: Controller chỉ gọi 1 class thay vì nhiều service
State: Phone chỉ cho approve/reject khi đang Pending
Iterator: Duyệt danh sách phone bằng HasNext()/Next()
5. Admin mới được Create/Update/Delete/Approve/Reject — check bằng [Authorize(Roles = "Admin")]