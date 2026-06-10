# PhoneShop Management System - Separated FE/BE ASP.NET Core

## 1. PHƯƠNG PHÁP NỘP BÀI (SUBMISSION)
- **PPTx**: File slide thuyết trình kiến trúc và demo.
- **Project Code**: Folder đã nén (zip) bao gồm cả BE và FE.
- **Guide / Manual**: Nội dung bên dưới.

## 2. KIẾN TRÚC HỆ THỐNG (ARCHITECTURE)
Hệ thống được thiết kế theo mô hình **N-Layers** (Client-Server):
- **PhoneShopApp.FE.Web** (Client/Presentation): ASP.NET MVC gọi API.
- **PhoneShopApp.BE.Api** (Server/Presentation): Web API cung cấp dữ liệu.
- **PhoneShopApp.BE.Services** (Business Logic): Xử lý nghiệp vụ và áp dụng Design Patterns.
- **PhoneShopApp.BE.Infrastructure** (Data Access): EF Core mapping MySQL.
- **PhoneShopApp.BE.Core** (Core): Entities, DTOs, Enums.

## 3. DESIGN PATTERNS ĐÃ ÁP DỤNG
- **Creational (Singleton)**: `JwtTokenGeneratorSingleton` dùng để tạo mã JWT duy nhất cho hệ thống.
- **Creational (Factory Method)**: `PhoneSearchFactoryMethod` dùng để tạo các chiến lược tìm kiếm điện thoại (Model, Brand, All) dựa trên tham số đầu vào.
- **Structural (Facade)**: `PhoneShopFacade` gom các service phức tạp (Phones, Users) thành một đầu mối duy nhất cho Controller sử dụng.
- **Behavioral (State)**: `PhoneReviewState` xử lý chuyển trạng thái điện thoại (Pending → Approved/Rejected).
- **Behavioral (Iterator)**: `PhoneIterator` dùng để duyệt danh sách điện thoại một cách linh hoạt trong PhoneService.

## 4. HƯỚNG DẪN CÀI ĐẶT & CHẠY (GUIDE / MANUAL)

### Yêu cầu:
- .NET SDK 9.0+
- MySQL Server 8.0+

### Bước 1: Cấu hình Database
1. Mở MySQL và chạy script [`data.sql`](data.sql) để tạo database và bảng:

```sql
-- Script đã bao gồm CREATE DATABASE, CREATE TABLE và INSERT dữ liệu mẫu
source data.sql;
```

2. Database sẽ được tạo với tên `phone_shop_app`.
3. Chuỗi kết nối đã được cấu hình sẵn trong [`BE/PhoneShopApp.BE.Api/appsettings.json`](BE/PhoneShopApp.BE.Api/appsettings.json):

```json
"MySql": "Server=localhost;Port=3306;Database=phone_shop_app;Uid=root;Pwd=1234;"
```

Nếu MySQL của bạn có user/password khác, sửa lại cho đúng.

### Bước 2: Chạy Backend (API)
Mở terminal tại thư mục gốc và chạy:

```
dotnet run --project BE/PhoneShopApp.BE.Api
```

BE chạy tại: **http://localhost:5201**

### Bước 3: Chạy Frontend (MVC)
Mở terminal mới và chạy:

```
dotnet run --project FE/PhoneShopApp.FE.Web
```

FE chạy tại: **http://localhost:5298**

### Bước 4: Truy cập ứng dụng
Mở trình duyệt, vào **http://localhost:5298** và đăng nhập bằng:
- **Username**: `admin`
- **Password**: `admin123`

## 5. CÁC CHỨC NĂNG CHÍNH
- **Login**: Đăng nhập Admin (admin/admin123) — JWT được lưu trong Session, FE tự động gắn Bearer token.
- **User Management**: Xem danh sách users, tạo user mới (chỉ Admin).
- **Phone Management**: Xem, Thêm, Sửa, Xóa điện thoại (CRUD đầy đủ, chỉ Admin với CUD).
- **Phone Search**: Tìm kiếm điện thoại theo Model, Brand, hoặc tất cả các trường (Factory Method + Iterator Pattern).
- **Phone Status Review**: Phê duyệt / Từ chối điện thoại (State Pattern, chỉ Admin).

### API Endpoints (BE)
| Method | URL | Auth | Mô tả |
|--------|-----|------|-------|
| GET | `/api/phones?keyword=&searchBy=` | JWT | Tìm kiếm / liệt kê điện thoại |
| POST | `/api/phones` | Admin | Thêm điện thoại mới |
| PUT | `/api/phones/{id}` | Admin | Sửa thông tin điện thoại |
| PATCH | `/api/phones/{id}/status` | Admin | Đổi trạng thái (Approve/Reject) |
| DELETE | `/api/phones/{id}` | Admin | Xóa điện thoại |

## 6. LƯU Ý KỸ THUẬT
- Mọi logic Design Patterns đều được đánh dấu bằng comment rõ ràng trong code.
- **BE chạy port 5201**, **FE chạy port 5298**.
- FE gọi API Backend thông qua `BackendApiClient` (đã cấu hình trong [`FE/PhoneShopApp.FE.Web/appsettings.json`](FE/PhoneShopApp.FE.Web/appsettings.json)).
- Sử dụng MySQL (Pomelo.EntityFrameworkCore.MySql) - không dùng SQL Server.
- Dữ liệu mẫu gồm 20 điện thoại từ nhiều hãng: Apple, Samsung, Google, Sony, OnePlus, Xiaomi, Nothing, Asus, Oppo, Vivo, Huawei, Motorola, Honor.
- JWT Token được cấu hình với thời hạn 180 phút, lưu trong Session phía FE.
- Môi trường Development tự động seed dữ liệu Admin và danh sách điện thoại mẫu khi chạy lần đầu.
