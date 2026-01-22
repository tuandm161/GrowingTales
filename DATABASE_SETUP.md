# 🗄️ Hướng dẫn Setup Database - Siêu Chi Tiết

## 🎯 Tổng quan

Dự án này đã có sẵn:
- ✅ Database models (User, Story, Payment, etc.)
- ✅ Migration files đã tạo
- ✅ Code tự động tạo database khi chạy lần đầu

**Bạn chỉ cần chạy 1 lệnh!**

---

## 🚀 CÁCH NHANH NHẤT (Khuyến khích)

### Bước 1: Mở PowerShell
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
```

### Bước 2: Chạy lệnh tạo database
```powershell
dotnet ef database update
```

**Chờ khoảng 5-10 giây...**

### Kết quả:
```
Done.
```

✅ **Xong! Database đã được tạo tại `Server\growingtales.db`**

---

## 🔍 Kiểm tra Database đã tạo thành công

### Cách 1: Kiểm tra file tồn tại
```powershell
# Vẫn trong thư mục Server
dir *.db

# Sẽ thấy:
# growingtales.db (khoảng vài KB đến vài MB)
```

### Cách 2: Chạy app và test
```powershell
dotnet run
```

Mở browser: http://localhost:5002

- Đăng ký tài khoản mới
- Nếu thành công → Database hoạt động!

---

## 📊 Xem dữ liệu trong Database

### Cách 1: Dùng DB Browser for SQLite (Khuyến khích)

**Bước 1: Download công cụ**
- Truy cập: https://sqlitebrowser.org/dl/
- Tải "DB Browser for SQLite" (Windows .msi)
- Cài đặt

**Bước 2: Mở database**
1. Mở DB Browser
2. File → Open Database
3. Chọn file: `D:\CSharp\WebAPI\GrowingTales\Server\growingtales.db`
4. Bạn sẽ thấy tất cả bảng:
   - Users
   - Stories
   - StoryPages
   - Subscriptions
   - Payments
   - ActivityLogs
   - Notifications
   - SystemSettings
   - Contacts

**Bước 3: Xem dữ liệu**
- Tab "Browse Data"
- Chọn bảng muốn xem (vd: Users)
- Sẽ thấy tất cả records

### Cách 2: Dùng Visual Studio Code

**Bước 1: Cài extension**
- Mở VS Code
- Extensions (Ctrl+Shift+X)
- Tìm "SQLite Viewer"
- Install

**Bước 2: Mở database**
- Trong VS Code, mở file `Server/growingtales.db`
- Hoặc chuột phải file → Open Database
- Sẽ thấy cấu trúc database

### Cách 3: Dùng Command Line

```powershell
# Cài SQLite command line tool
winget install SQLite.SQLite

# Mở database
cd D:\CSharp\WebAPI\GrowingTales\Server
sqlite3 growingtales.db

# Xem các bảng
.tables

# Xem dữ liệu
SELECT * FROM Users;
SELECT * FROM Stories;
SELECT * FROM SystemSettings;

# Thoát
.quit
```

---

## 🔧 Quản lý Database

### Xem các bảng đã tạo
```sql
-- Mở DB Browser hoặc sqlite3
SELECT name FROM sqlite_master WHERE type='table';
```

Kết quả:
```
Users
Stories
StoryPages
Subscriptions
Payments
ActivityLogs
Notifications
SystemSettings
Contacts
__EFMigrationsHistory
```

### Xem structure của 1 bảng
```sql
PRAGMA table_info(Users);
```

### Đếm số records
```sql
SELECT COUNT(*) FROM Users;
SELECT COUNT(*) FROM Stories;
SELECT COUNT(*) FROM Payments;
```

### Xóa tất cả data (cẩn thận!)
```sql
DELETE FROM Stories;
DELETE FROM Users;
-- Hoặc xóa file .db và chạy lại migration
```

---

## 🔄 Nếu muốn tạo lại Database từ đầu

### Bước 1: Xóa database cũ
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server

# Dừng app nếu đang chạy (Ctrl+C)

# Xóa database files
Remove-Item growingtales.db -Force
Remove-Item growingtales.db-shm -Force -ErrorAction SilentlyContinue
Remove-Item growingtales.db-wal -Force -ErrorAction SilentlyContinue
```

### Bước 2: Tạo lại
```powershell
dotnet ef database update
```

### Bước 3: Verify
```powershell
# Chạy app
dotnet run

# Hoặc xem file
dir *.db
```

✅ **Database mới đã được tạo!**

---

## 📝 Tạo Migration mới (Khi thay đổi Models)

Nếu bạn thêm/sửa Models sau này:

### Bước 1: Tạo migration
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server

# Tạo migration với tên mô tả
dotnet ef migrations add TenMigration

# VD:
dotnet ef migrations add AddAvatarToUser
dotnet ef migrations add AddRevenueReport
```

### Bước 2: Xem migration đã tạo
```powershell
# Migration file sẽ xuất hiện trong Server\Migrations\
dir Migrations\*.cs
```

### Bước 3: Apply migration
```powershell
dotnet ef database update
```

### Bước 4: Rollback migration (nếu sai)
```powershell
# Xóa migration cuối cùng
dotnet ef migrations remove

# Hoặc quay về migration trước
dotnet ef database update TenMigrationTruocDo
```

---

## 🌐 Setup Database cho Azure

### Option 1: Dùng SQLite (Đơn giản - Khuyến khích cho bắt đầu)

**Ưu điểm:**
- ✅ Miễn phí
- ✅ Không cần setup phức tạp
- ✅ Đủ cho < 100 users đồng thời

**Nhược điểm:**
- ❌ Không scale tốt cho traffic cao
- ❌ Backup thủ công

**Setup:**

Đã có sẵn trong hướng dẫn Azure! Database sẽ được lưu trong Azure File Share.

Không cần làm gì thêm - app sẽ tự động tạo database khi deploy lần đầu.

### Option 2: Dùng Azure SQL Database (Mạnh mẽ - Cho production lớn)

**Ưu điểm:**
- ✅ Scale tốt
- ✅ Backup tự động
- ✅ High availability
- ✅ Nhiều tính năng enterprise

**Nhược điểm:**
- ❌ Tốn phí (~$5/tháng cho tier Basic)
- ❌ Setup phức tạp hơn

**Setup:**

Xem hướng dẫn chi tiết trong: **[AZURE_SQL_SETUP.md](AZURE_SQL_SETUP.md)**

---

## 🧪 Test Database

### Test 1: Tạo user
```powershell
# Chạy app
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet run
```

1. Mở http://localhost:5002
2. Đăng ký tài khoản: email `test@test.com`, password `123456`
3. Nếu thành công → Database hoạt động!

### Test 2: Kiểm tra data đã lưu
```powershell
# Mở DB Browser
# Hoặc dùng sqlite3
sqlite3 growingtales.db

SELECT * FROM Users;
# Sẽ thấy user vừa tạo!
```

### Test 3: Tạo truyện
1. Tạo 1 truyện
2. Kiểm tra:
```sql
SELECT * FROM Stories;
SELECT * FROM StoryPages;
```

---

## ❓ FAQ về Database

**Q: Database file ở đâu?**
A: `D:\CSharp\WebAPI\GrowingTales\Server\growingtales.db`

**Q: Có cần tạo bảng thủ công không?**
A: KHÔNG! EF Core tự động tạo tất cả bảng từ Models.

**Q: Làm sao backup database?**
A: 
```powershell
# Copy file .db
Copy-Item growingtales.db -Destination growingtales.backup.db

# Hoặc export ra SQL
sqlite3 growingtales.db .dump > backup.sql
```

**Q: Làm sao restore backup?**
A:
```powershell
# Xóa database hiện tại
Remove-Item growingtales.db

# Copy backup
Copy-Item growingtales.backup.db -Destination growingtales.db
```

**Q: Database có bị mất khi deploy lên Azure không?**
A: KHÔNG! Nếu dùng Azure File Share, data sẽ được lưu vĩnh viễn.

**Q: Có thể đổi từ SQLite sang SQL Server sau không?**
A: CÓ! Xem hướng dẫn trong [AZURE_SQL_SETUP.md](AZURE_SQL_SETUP.md)

---

## 🆘 Troubleshooting

### Lỗi: "No DbContext was found"
```powershell
# Đảm bảo ở đúng thư mục Server
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet ef database update
```

### Lỗi: "Build failed"
```powershell
# Build project trước
dotnet build
# Sau đó chạy lại
dotnet ef database update
```

### Lỗi: "Unable to create database"
```powershell
# Xóa database cũ nếu bị corrupt
Remove-Item growingtales.db -Force
Remove-Item Migrations -Recurse -Force

# Tạo migration mới
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Lỗi: "Table already exists"
```powershell
# Xóa database và tạo lại
Remove-Item growingtales.db
dotnet ef database update
```

---

## 📋 Tóm tắt - Chỉ cần nhớ 3 lệnh

```powershell
# 1. Tạo migration (chỉ khi thay đổi Models)
dotnet ef migrations add TenMigration

# 2. Tạo/update database
dotnet ef database update

# 3. Xem migration hiện có
dotnet ef migrations list
```

---

**Vậy thôi! Siêu đơn giản đúng không? 😊**
