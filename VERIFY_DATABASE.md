# ✅ Hướng dẫn Verify Database - Siêu Đơn Giản

## 🎯 Mục tiêu: Đảm bảo database hoạt động tốt

---

## ⚡ Kiểm tra Nhanh (30 giây)

Chạy script tự động:

```powershell
cd D:\CSharp\WebAPI\GrowingTales
.\check-database.ps1
```

Nếu thấy:
```
Status: ✅ SẴN SÀNG
```

→ **Hoàn hảo! Không cần làm gì thêm!**

---

## 🔍 Kiểm tra Chi Tiết

### Bước 1: Kiểm tra file database có tồn tại không

```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
dir *.db
```

**Kết quả mong đợi:**
```
growingtales.db    (khoảng 200-500 KB)
```

✅ Nếu thấy file → Database đã được tạo!

❌ Nếu không thấy → Chạy lệnh tạo:
```powershell
dotnet ef database update
```

---

### Bước 2: Xem các bảng trong database

**Cách 1: Download DB Browser (Khuyến khích - Dễ nhất)**

1. Tải về: https://sqlitebrowser.org/dl/
2. Chọn **"DB Browser for SQLite - Standard installer for 64-bit Windows"**
3. Cài đặt (Next → Next → Install)
4. Mở DB Browser
5. File → Open Database
6. Chọn: `D:\CSharp\WebAPI\GrowingTales\Server\growingtales.db`

**Bạn sẽ thấy:**

```
📁 Tables
├── ActivityLogs       (Nhật ký hoạt động)
├── Contacts          (Liên hệ từ khách)
├── Notifications     (Thông báo)
├── Payments          (💰 Doanh thu)
├── Stories           (Truyện)
├── StoryPages        (Trang truyện)
├── Subscriptions     (Gói đăng ký)
├── SystemSettings    (Cấu hình)
└── Users             (Người dùng)
```

**Cách 2: Dùng VS Code**

1. Mở VS Code
2. Extensions → Tìm **"SQLite Viewer"**
3. Install extension
4. Trong VS Code, mở file `Server/growingtales.db`
5. Sẽ thấy cấu trúc database bên phải

---

### Bước 3: Test bằng cách tạo user thử

```powershell
# Chạy app
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet run
```

Mở browser: http://localhost:5002

1. Click **"Đăng ký"**
2. Nhập:
   - Email: `test@test.com`
   - Tên: `Test User`
   - Mật khẩu: `123456`
   - Xác nhận: `123456`
3. Nhấn **"Đăng ký"**

**Nếu thành công:**
- Bạn sẽ thấy trang chủ
- Có tên user ở góc phải

**Kiểm tra trong database:**
- Mở DB Browser
- Tab "Browse Data"
- Table: `Users`
- Sẽ thấy user `test@test.com` vừa tạo!

---

### Bước 4: Xem dữ liệu sample

Trong DB Browser, chọn bảng `SystemSettings`:

Bạn sẽ thấy các cấu hình mặc định:
```
site_name = "GrowingTales"
premium_price = "150000"
free_stories_limit = "3"
...
```

→ **Dữ liệu mặc định đã được seed!**

---

## 🛠️ Các lệnh hữu ích

### Xem tất cả migrations
```powershell
cd Server
dotnet ef migrations list
```

### Tạo migration mới (khi sửa Models)
```powershell
dotnet ef migrations add TenMigration
dotnet ef database update
```

### Xóa và tạo lại database
```powershell
# Xóa
Remove-Item growingtales.db, growingtales.db-*

# Tạo lại
dotnet ef database update
```

### Backup database
```powershell
# Backup
Copy-Item growingtales.db growingtales.backup.$(Get-Date -Format 'yyyyMMdd').db

# Restore
Copy-Item growingtales.backup.20260120.db growingtales.db
```

---

## ❓ Câu hỏi thường gặp

**Q: Tôi phải tạo bảng thủ công không?**
A: **KHÔNG!** Entity Framework tự động tạo tất cả bảng từ Models.

**Q: Migration là gì?**
A: Migration = "Bản thiết kế" để tạo/sửa database. Giống như Git cho database.

**Q: Khi nào cần chạy `dotnet ef database update`?**
A: Chỉ khi:
- Lần đầu setup project
- Có migration mới (sau khi pull code mới từ Git)
- Muốn tạo lại database

**Q: Database có bị mất khi tắt máy không?**
A: KHÔNG! File `.db` được lưu trên ổ cứng.

**Q: Khi deploy lên Azure, database có tự động tạo không?**
A: CÓ! Code trong `Program.cs` có:
```csharp
db.Database.EnsureCreated();
```
App sẽ tự động tạo database lần đầu chạy.

---

## 🚨 Lỗi thường gặp

### Lỗi: "A database operation failed while processing the request"

**Nguyên nhân:** Database bị corrupt hoặc migration chưa chạy

**Giải pháp:**
```powershell
cd Server
Remove-Item growingtales.db
dotnet ef database update
```

### Lỗi: "SQLite Error 5: database is locked"

**Nguyên nhân:** App đang chạy và đang dùng database

**Giải pháp:**
- Tắt app (Ctrl+C)
- Đóng DB Browser nếu đang mở
- Thử lại

### Lỗi: "No DbContext was found"

**Nguyên nhân:** Đang ở sai thư mục

**Giải pháp:**
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet ef database update
```

---

## 🎓 Tóm tắt cho người mới

Database của bạn:
- ✅ Đã được tạo tự động
- ✅ Có đầy đủ 9 bảng
- ✅ Có dữ liệu mẫu (SystemSettings)
- ✅ Sẵn sàng để dùng

**Bạn không cần làm gì thêm!**

Khi deploy lên Azure:
- Database sẽ tự động được tạo
- Dữ liệu sẽ được lưu trong Azure File Share
- Bền vững và an toàn

---

**Yên tâm deploy đi bạn! Database đã OK! 🎉**
