# ✅ Kiểm Tra Database - Tổng Hợp Đầy Đủ

## 🎯 Mục tiêu: Đảm bảo database hoạt động đúng trên Azure với mount path `/data`

---

## ✅ 1. Connection String

### File: `Server/appsettings.Production.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=/data/growingtales.db"
}
```

✅ **ĐÚNG** - Dùng `/data` (không phải `/home/data`)

---

## ✅ 2. Code trong Program.cs

### Dòng 20-22: Cấu hình DbContext
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=growingtales.db")
```

✅ **ĐÚNG** - Đọc từ `appsettings.Production.json` khi Production

### Dòng 58-77: Auto Migration
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Checking database and running migrations...");
        db.Database.Migrate(); // ← Tự động chạy migrations
        logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        db.Database.EnsureCreated(); // Fallback
        logger.LogWarning("Used EnsureCreated as fallback");
    }
}
```

✅ **ĐÚNG** - Tự động chạy migrations khi app khởi động

---

## ✅ 3. AppDbContext

### File: `Server/Data/AppDbContext.cs`

- ✅ Có đầy đủ 9 DbSet (Users, Stories, Payments, etc.)
- ✅ Có cấu hình relationships
- ✅ Có indexes cho performance
- ✅ Có seed data cho SystemSettings

✅ **ĐÚNG** - Cấu hình đầy đủ

---

## ✅ 4. Luồng Database trên Azure

### Khi deploy lần đầu:

```
1. Code được upload lên Azure
   ↓
2. App Service khởi động
   ↓
3. Program.cs chạy (dòng 58-77)
   ↓
4. Tạo scope và lấy AppDbContext
   ↓
5. Đọc ConnectionString từ appsettings.Production.json
   → "Data Source=/data/growingtales.db"
   ↓
6. Kiểm tra file /data/growingtales.db có chưa?
   ├─ Chưa có → Tạo mới
   └─ Có rồi → Skip
   ↓
7. Chạy db.Database.Migrate()
   ↓
8. EF Core kiểm tra __EFMigrationsHistory table
   ├─ Chưa có → Tạo table này
   └─ Có rồi → Check migrations đã apply chưa
   ↓
9. Apply migration "InitialCreate"
   - Tạo 9 bảng (Users, Stories, Payments, etc.)
   - Tạo indexes
   - Insert seed data (12 SystemSettings records)
   ↓
10. Log: "Database migrations completed successfully"
   ↓
11. App tiếp tục khởi động
   ↓
12. Website sẵn sàng!
```

### Khi có migration mới:

```
1. Deploy code mới (có migration mới)
   ↓
2. App khởi động
   ↓
3. db.Database.Migrate() chạy
   ↓
4. EF Core check __EFMigrationsHistory
   ↓
5. Tìm migration chưa apply
   ↓
6. Apply migration mới
   ↓
7. Database được update
```

---

## ✅ 5. Mount Path trên Azure

### Cấu hình trong Azure Portal:

```
Name: database
Storage account: growingtalesstorage
Share name: growingtales-data
Mount path: /data          ← ĐÚNG!
Read only: NO (cho phép ghi)
```

### Connection String phải khớp:

```
appsettings.Production.json: "Data Source=/data/growingtales.db"
Azure Mount path: /data
```

✅ **KHỚP!**

---

## ✅ 6. Kiểm tra Logic

### Local Development:
- Connection: `growingtales.db` (relative path)
- Database tạo tại: `Server/growingtales.db`
- ✅ OK

### Azure Production:
- Connection: `/data/growingtales.db` (absolute path)
- Database tạo tại: `/data/growingtales.db` (trong File Share)
- ✅ OK

### Fallback:
- Nếu migration fail → `EnsureCreated()` (dòng 74)
- Đảm bảo database vẫn được tạo
- ✅ OK

---

## ✅ 7. Checklist Verify

Sau khi deploy, kiểm tra:

- [ ] **Connection String đúng:**
  ```json
  "Data Source=/data/growingtales.db"
  ```

- [ ] **Mount path đúng:**
  ```
  /data (không phải /home/data)
  ```

- [ ] **Logs có dòng:**
  ```
  "Checking database and running migrations..."
  "Database migrations completed successfully"
  ```

- [ ] **Test chức năng:**
  - Đăng ký user → Thành công
  - Tạo truyện → Thành công
  - Xem truyện → Thành công

- [ ] **File database tồn tại:**
  - Azure Portal → Storage Account → File Share
  - Browse → Thấy `growingtales.db`

---

## 🚨 Các lỗi có thể gặp

### Lỗi 1: "Unable to open database file"

**Nguyên nhân:** Mount path không khớp với Connection String

**Kiểm tra:**
- Connection String: `/data/growingtales.db` ✅
- Mount path: `/data` ✅
- **Phải khớp nhau!**

### Lỗi 2: "Database is locked"

**Nguyên nhân:** Nhiều process cùng truy cập database

**Giải pháp:**
- SQLite hỗ trợ concurrent reads tốt
- Chỉ 1 write tại 1 thời điểm (OK cho web app)
- Nếu lỗi → Restart app

### Lỗi 3: "Migration failed"

**Nguyên nhân:** Migration files thiếu hoặc corrupt

**Giải pháp:**
- Đảm bảo folder `Migrations/` được deploy
- Không thêm vào `.gitignore`
- Redeploy nếu cần

---

## 📊 So sánh Path

| Môi trường | Connection String | Mount Path | Kết quả |
|------------|-------------------|------------|---------|
| **Local** | `growingtales.db` | N/A | ✅ OK |
| **Azure (đúng)** | `/data/growingtales.db` | `/data` | ✅ OK |
| **Azure (sai)** | `/data/growingtales.db` | `/home/data` | ❌ Lỗi! |
| **Azure (sai)** | `/home/data/growingtales.db` | `/data` | ❌ Lỗi! |

→ **Phải khớp nhau!**

---

## ✅ Kết luận

### Connection String: ✅ ĐÚNG
```json
"Data Source=/data/growingtales.db"
```

### Code Logic: ✅ ĐÚNG
- Tự động đọc config
- Tự động chạy migrations
- Có fallback

### Luồng Database: ✅ ĐÚNG
- Tự động tạo khi deploy
- Tự động apply migrations
- Lưu trong File Share bền vững

### Mount Path: ✅ CẦN ĐẢM BẢO
- Azure Portal: Mount `/data`
- Connection String: `/data/growingtales.db`
- **Phải khớp!**

---

## 🎯 Action Items cho bạn

1. ✅ **appsettings.Production.json** - Đã đúng `/data`
2. ✅ **Code** - Đã đúng, tự động
3. ⚠️ **Azure Portal** - Cần mount tại `/data` (không phải `/home/data`)

**Khi mount File Share trên Azure:**
- Mount path: `/data` ✅
- Connection String: `/data/growingtales.db` ✅
- **Khớp nhau!** ✅

---

**Tất cả đã OK! Chỉ cần mount đúng path `/data` trên Azure! 🎉**
