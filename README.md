# Payroll Management System (Hệ thống Quản lý Lương)

Payroll Management System là một hệ thống ứng dụng quản lý nhân sự và tiền lương toàn diện được xây dựng trên nền tảng **.NET 8** ứng dụng mô hình **Clean Architecture** và mẫu thiết kế **CQRS**. Ứng dụng cung cấp giải pháp trọn vẹn để quản lý hồ sơ nhân viên, tổ chức phòng ban, hệ thống ngạch bậc lương, và quy trình đánh giá năng lực minh bạch.

---

## Các tính năng chính

* **Quản lý Hồ sơ & Nhân sự:**
    * Quản lý thông tin chi tiết nhân viên, CCCD, chức vụ, phòng ban và ngày vào làm.
    * Hệ thống luân chuyển nhân sự giữa các phòng ban hoặc thay đổi chức danh.
* **Hệ thống Đánh giá Năng lực (Competency Evaluation):**
    * Tự động quản lý các "Kỳ đánh giá" theo các giai đoạn (Khởi tạo, Đang đánh giá, Đã chốt).
    * Thiết lập Khung năng lực, Tiêu chí năng lực, và Mức quy đổi điểm thưởng/hệ số.
    * Luồng duyệt phiếu: Nhân viên Tự đánh giá -> Quản lý Đánh giá, chấm điểm -> Tự động tính toán Hệ số P2.
* **Xác thực và Phân quyền (Auth & Access Control):**
    * Xác thực người dùng an toàn bằng **JWT Token** (hỗ trợ Access Token và Refresh Token).
    * Mã hóa mật khẩu an toàn với **BCrypt**.
    * Phân quyền linh hoạt theo Vai trò (Role).
* **Quản lý Hệ thống Danh mục:**
    * Quản lý động danh sách Phòng ban, Chức vụ.
    * Xây dựng hệ thống cấu trúc lương chi tiết: Ngạch lương -> Bậc lương -> Hệ số lương.
* **Giao diện Web tương tác (Client SPA):**
    * Giao diện hiện đại, responsive, và thân thiện với người dùng cuối, hỗ trợ Dark Mode.
    * Xử lý hiển thị trạng thái (Enum) đồng bộ và logic màu sắc trực quan qua các thẻ Tag.

---

## Công nghệ & Kiến trúc

Dự án được cấu trúc theo **Clean Architecture** và **CQRS Pattern** cho phần Backend để đảm bảo tính độc lập, dễ bảo trì và mở rộng, kết hợp với Single Page Application (SPA) cho phần Frontend.

### 1. Backend (API)
* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Cơ sở dữ liệu:** PostgreSQL (qua Entity Framework Core)
* **Thư viện/Công cụ được sử dụng:**
    * **Entity Framework Core (PostgreSQL):** ORM thao tác với cơ sở dữ liệu.
    * **MediatR:** Triển khai CQRS (Command/Query Responsibility Segregation).
    * **FluentValidation:** Validate dữ liệu đầu vào tự động thông qua Pipeline Behavior.
    * **BCrypt:** Mã hóa mật khẩu.
* **Testing:** xUnit & Moq (Framework phục vụ Unit Test và Integration Test).

### 2. Frontend (Client)
* **Framework:** ReactJS 19 (với Vite)
* **Ngôn ngữ:** TypeScript
* **UI/UX:** Tailwind CSS, Ant Design (antd), Material-UI (MUI).
* **Thư viện/Công cụ được sử dụng:**
    * **React Hook Form & Zod:** Quản lý form và validation dữ liệu.
    * **Axios:** Gọi API Backend (xử lý HTTP requests, tự động đính kèm và làm mới JWT Token).
    * **Framer Motion:** Xử lý các hiệu ứng animation chuyển cảnh mượt mà.

---

## Cấu trúc dự án

```text
PayrollManagementSystem.sln
├── backend/
│   ├── src/
│   │   ├── PayrollManagementSystem.API/            # Controllers, Middleware, API Entrypoint
│   │   ├── PayrollManagementSystem.Application/    # CQRS (Commands/Queries), DTOs, Validators
│   │   ├── PayrollManagementSystem.Domain/         # Core Models (User, Employee, Salary,...), Enums
│   │   └── PayrollManagementSystem.Infrastructure/ # DbContext (PostgreSQL), Repositories, JWT
│   └── tests/
│       ├── PayrollManagementSystem.IntegrationTests/ # Test tích hợp API
│       └── PayrollManagementSystem.UnitTests/        # Test chức năng từng thành phần
└── frontend/                                         # Web Client Frontend (ReactJS 19 + Vite)
    ├── src/
    │   ├── features/                                 # Các module chức năng (users, employees,...)
    │   ├── types/                                    # Kiểu dữ liệu TypeScript dùng chung
    │   └── ...
```

## Hướng dẫn cài đặt và chạy ứng dụng

### 1. Yêu cầu hệ thống
* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [PostgreSQL](https://www.postgresql.org/download/)
* [Node.js](https://nodejs.org/) (phiên bản 18+ để chạy Frontend).
* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) hoặc [Visual Studio Code](https://code.visualstudio.com/).

### 2. Cài đặt Backend (API)
2.1 **Cấu hình môi trường**
* Mở file `appsettings.Development.json` trong project `PayrollManagementSystem.API` và cập nhật chuỗi kết nối Database cho phù hợp với môi trường của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1:5433;Database=payroll_management_system_db;Username=postgres;Password=<DB_PASSWORD>"
  },
  "JwtSettings": {
    "SecretKey": "<YOUR_SECRET_KEY>",
    "Issuer": "PayrollManagementSystem.API",
    "Audience": "PayrollManagementSystem.Client",
    "ExpirationMinutes": 10,
    "RefreshTokenExpirationDays": 7
  }
}
```

2.2 **Cập nhật Database (Migration)**
* Mở terminal tại thư mục root của dự án Backend và chạy lệnh sau để khởi tạo cơ sở dữ liệu:

```bash
cd backend/src/PayrollManagementSystem.API

# Cập nhật Database dựa trên các file Migrations đã có sẵn
dotnet ef database update --project ../PayrollManagementSystem.Infrastructure --startup-project .
```

2.3 **Khởi chạy ứng dụng**
* Chạy lệnh sau để khởi động API:

```bash
dotnet run --project .
```

* **API sẽ khởi chạy và cho phép xem tài liệu thông qua Swagger (nếu được cấu hình).**

### 3. Cài đặt Frontend (Client)
3.1 **Cấu hình biến môi trường**
* Kiểm tra hoặc tạo file `.env` trong thư mục `frontend` và đảm bảo biến môi trường đang trỏ tới đúng cổng của API (thường là http://localhost:5000 hoặc tùy cấu hình ASP.NET Core của bạn):
```bash
VITE_API_URL=http://localhost:5000
```

3.2 **Cài đặt dependencies và chạy Web UI**
* Mở một cửa sổ terminal tại thư mục `frontend` và chạy lần lượt các lệnh sau:
```bash
# 1. Di chuyển vào thư mục chứa mã nguồn frontend
cd frontend

# 2. Tải và cài đặt tất cả các thư viện (dependencies) cần thiết
npm install

# 3. Khởi chạy giao diện
npm run dev
```
* **Giao diện sẽ khởi chạy tại: http://localhost:5173 (cổng mặc định của Vite)**
