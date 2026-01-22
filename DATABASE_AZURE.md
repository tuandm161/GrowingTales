# 🗄️ Database trên Azure - Giải đáp mọi thắc mắc

## 🎯 Câu hỏi của bạn: "Chưa tạo database và chưa liên kết"

### ✅ Trả lời ngắn gọn:

**KHÔNG CẦN LO!** Database sẽ **TỰ ĐỘNG TẠO** khi bạn deploy lên Azure!

---

## 🤖 Cách hoạt động (Tự động 100%)

### Bước 1: Bạn deploy code lên Azure
```powershell
.\deploy-to-azure.ps1
```

### Bước 2: App khởi động lần đầu

Code trong `Program.cs` sẽ tự động chạy:

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // ← TỰ ĐỘNG tạo database!
}
```

### Bước 3: Database được tạo

- ✅ Tạo file database: `/data/growingtales.db`
- ✅ Tạo tất cả 9 bảng (Users, Stories, Payments, etc.)
- ✅ Insert dữ liệu mặc định (SystemSettings)
- ✅ Sẵn sàng sử dụng!

**BẠN KHÔNG CẦN LÀM GÌ CẢ!** 🎉

---

## 📍 Database được lưu ở đâu trên Azure?

### Local (máy bạn):
```
D:\CSharp\WebAPI\GrowingTales\Server\growingtales.db
```

### Azure (sau khi mount File Share):
```
/data/growingtales.db
```

**Dữ liệu lưu trong Azure File Share:**
- Storage Account: `growingtalesstorage`
- File Share: `growingtales-data`
- Path: `/data/`

→ **Bền vững, không bị mất khi restart app!**

---

## ✅ Checklist Database trên Azure

Sau khi deploy, kiểm tra:

### 1. Xem logs để confirm
```powershell
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

Tìm dòng:
```
Checking database and running migrations...
Database migrations completed successfully
```

→ **Database đã được tạo!**

### 2. Test bằng cách đăng ký user

1. Mở: `https://growingtales-app.azurewebsites.net`
2. Click "Đăng ký"
3. Tạo tài khoản mới
4. Nếu thành công → **Database hoạt động!**

### 3. Kiểm tra File Share (Optional)

1. Vào Azure Portal
2. Tìm Storage Account: `growingtalesstorage`
3. File shares → `growingtales-data`
4. Browse → Sẽ thấy file `growingtales.db`

---

## 🔧 Nếu cần chạy Migration thủ công (Không khuyến khích)

### Cách 1: Cài dotnet-ef tool trên Azure Kudu

```bash
# Vào Kudu Console: https://growingtales-app.scm.azurewebsites.net
cd site/wwwroot

# Cài dotnet-ef tool (chỉ cần 1 lần)
dotnet tool install --global dotnet-ef

# Add to PATH (Linux)
export PATH="$PATH:/root/.dotnet/tools"

# Hoặc Windows
set PATH=%PATH%;C:\local\UserProfile\.dotnet\tools

# Chạy migration
dotnet ef database update
```

### Cách 2: Chạy từ local kết nối đến Azure SQL (Nếu dùng Azure SQL)

```powershell
# Set connection string tạm
$env:ConnectionStrings__DefaultConnection="Server=tcp:..."

# Chạy migration
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet ef database update

# Xóa env variable
Remove-Item Env:\ConnectionStrings__DefaultConnection
```

### Cách 3: Dùng Azure Data Studio (Nếu dùng Azure SQL)

1. Download Azure Data Studio
2. Connect đến Azure SQL Database
3. Run SQL scripts từ migration files

---

## ❓ FAQ về Database trên Azure

**Q: Tôi có phải tạo database thủ công trên Azure không?**
A: **KHÔNG!** App tự động tạo khi chạy lần đầu.

**Q: Migration files có cần deploy lên Azure không?**
A: **CÓ!** Migration files trong folder `Migrations/` phải được deploy cùng code.

**Q: Làm sao biết database đã được tạo?**
A: 
1. Xem logs (có dòng "Database migrations completed")
2. Test đăng ký user - nếu OK → Database OK
3. Vào File Share xem có file `.db`

**Q: Database có bị mất khi restart app không?**
A: **KHÔNG!** Data lưu trong File Share, rất an toàn.

**Q: Làm sao backup database trên Azure?**
A:
1. Vào Storage Account → File shares → `growingtales-data`
2. Download file `growingtales.db`
3. Hoặc dùng Azure CLI:
```powershell
az storage file download --account-name growingtalesstorage --share-name growingtales-data --path growingtales.db --dest ./backup.db
```

**Q: Có thể xem data trong database trên Azure không?**
A: **CÓ!**
1. Download file `.db` từ File Share
2. Mở bằng DB Browser trên máy bạn
3. Hoặc connect bằng Azure Data Studio

---

## 🚨 Troubleshooting

### Lỗi: "A database operation failed"

**Nguyên nhân:** 
- File Share chưa mount
- Quyền ghi file bị từ chối

**Giải pháp:**
1. Kiểm tra File Share đã mount chưa (Configuration → Path mappings)
2. Ensure mount path = `/data`
3. Read only = `NO` (phải cho phép ghi)
4. Restart app

### Lỗi: "Unable to open database file"

**Nguyên nhân:** Path không đúng

**Kiểm tra:**
- `appsettings.Production.json` có: `"Data Source=/data/growingtales.db"`
- File Share mount path: `/data`

**Giải pháp:**
- Đảm bảo 2 path này khớp nhau
- Restart app

### Lỗi trong logs: "Migration failed"

**Nguyên nhân:** 
- Migration files bị thiếu
- Database bị lock

**Giải pháp:**
1. Redeploy để đảm bảo migration files được upload
2. Restart app
3. Check logs chi tiết

---

## 💡 Best Practices

### 1. Luôn deploy Migration files
Đảm bảo folder `Migrations/` được deploy lên Azure:
- Không thêm `Migrations/` vào `.gitignore`
- Không thêm `Migrations/` vào `.dockerignore`

### 2. Monitor logs sau deploy
```powershell
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

Tìm các dòng:
- ✅ "Checking database and running migrations..."
- ✅ "Database migrations completed successfully"

### 3. Backup định kỳ
```powershell
# Tạo script backup tự động
# Download .db file mỗi tuần
```

### 4. Test sau mỗi lần deploy
- Đăng ký user mới
- Tạo truyện mới
- Check không có lỗi

---

## 🎓 Tóm tắt - Điều bạn cần nhớ

1. ✅ **Database TỰ ĐỘNG tạo** khi deploy lần đầu
2. ✅ **Migration TỰ ĐỘNG chạy** mỗi khi app khởi động
3. ✅ **Dữ liệu LƯU BỀN VỮNG** trong Azure File Share
4. ✅ **KHÔNG CẦN chạy lệnh thủ công** trong Kudu Console

**Chỉ cần:**
- Deploy code: `.\deploy-to-azure.ps1`
- Đợi app khởi động
- Database tự động OK!

---

## 📊 So sánh với các cách khác

| Cách | Độ khó | Tự động | Khuyến khích |
|------|--------|---------|--------------|
| **Auto Migration (Code)** | ⭐ Dễ | ✅ Hoàn toàn | ✅ YES |
| Chạy thủ công Kudu | ⭐⭐⭐ Khó | ❌ Không | ❌ NO |
| Chạy từ local | ⭐⭐ Trung bình | ❌ Không | ❌ NO |

→ **Dùng Auto Migration (đã có sẵn trong code)!**

---

**Yên tâm deploy! Database sẽ tự động OK! 🚀**
