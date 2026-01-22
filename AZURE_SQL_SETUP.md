# Hướng dẫn sử dụng Azure SQL Database (Thay vì SQLite)

Nếu bạn muốn database mạnh mẽ và có thể scale, hãy dùng Azure SQL Database thay vì SQLite.

## Khi nào nên dùng Azure SQL?

✅ **Nên dùng khi:**
- Có nhiều hơn 100 users đồng thời
- Cần backup tự động
- Cần scale database độc lập với app
- Website có traffic cao

❌ **Chưa cần dùng khi:**
- Mới bắt đầu,ít users
- Muốn tiết kiệm chi phí
- Database < 1GB

---

## Bước 1: Tạo Azure SQL Database

### 1.1. Trong Azure Portal
1. Tìm **"SQL databases"** → Create
2. **Basics:**
   - Resource group: `growingtales-rg`
   - Database name: `growingtales-db`
   - Server: Nhấn **"Create new"**
     - Server name: `growingtales-sqlserver` (phải unique)
     - Location: `Southeast Asia`
     - Authentication: `Use SQL authentication`
     - Server admin login: `sqladmin`
     - Password: Tạo mật khẩu mạnh (ít nhất 12 ký tự, có chữ hoa, số, ký tự đặc biệt)
     - **GHI NHỚ MẬT KHẨU NÀY!**
   - Compute + storage: Nhấn **"Configure database"**
     - **DTU-based**: Basic (5 DTU) - ~$5/tháng
     - Hoặc **vCore-based**: General Purpose, 2 vCores - ~$200/tháng
     - Khuyến khích: **Basic** cho bắt đầu

3. **Networking:**
   - Connectivity method: `Public endpoint`
   - Firewall rules:
     - ✅ Allow Azure services and resources to access this server
     - ✅ Add current client IP address

4. **Additional settings:**
   - Data source: `None` (blank database)
   - Collation: `SQL_Latin1_General_CP1_CI_AS`

5. Nhấn **Review + create** → **Create**
6. Đợi 3-5 phút

---

## Bước 2: Cấu hình Firewall

### 2.1. Cho phép Azure services
1. Vào SQL Server (không phải Database): `growingtales-sqlserver`
2. Bên trái: **Networking**
3. Tab **"Public access"**
4. ✅ Tick: "Allow Azure services and resources to access this server"
5. Save

### 2.2. Cho phép IP của bạn (để quản lý)
1. Vẫn trong **Networking**
2. Phần **"Firewall rules"**
3. Nhấn **"+ Add a firewall rule"**
4. Rule name: `MyComputer`
5. Start IP: (IP hiện tại của bạn - tự động điền)
6. End IP: (giống Start IP)
7. Save

---

## Bước 3: Lấy Connection String

### 3.1. Trong Azure Portal
1. Vào SQL Database: `growingtales-db`
2. Bên trái: **Connection strings**
3. Copy **ADO.NET** connection string:

```
Server=tcp:growingtales-sqlserver.database.windows.net,1433;Initial Catalog=growingtales-db;Persist Security Info=False;User ID=sqladmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### 3.2. Thay `{your_password}` bằng mật khẩu thật
```
Server=tcp:growingtales-sqlserver.database.windows.net,1433;Initial Catalog=growingtales-db;Persist Security Info=False;User ID=sqladmin;Password=MatKhauCuaBan123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

---

## Bước 4: Cấu hình App Service

### 4.1. Thêm Connection String vào Azure
1. Vào App Service: `growingtales-app`
2. Configuration → **Connection strings** (tab riêng, không phải Application settings)
3. Nhấn **"+ New connection string"**
4. Điền:
   - **Name**: `DefaultConnection`
   - **Value**: (Paste connection string vừa copy ở trên)
   - **Type**: `SQLServer`
5. Save

### 4.2. Xóa mount SQLite (nếu có)
1. Configuration → Path mappings
2. Xóa mount `database` (nếu có)
3. Save

---

## Bước 5: Cập nhật Code

### 5.1. Cài package SQL Server
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 5.2. Update Program.cs

Tìm dòng:
```csharp
options.UseSqlite(...)
```

Thay bằng:
```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString?.Contains("database.windows.net") == true)
{
    // Azure SQL
    options.UseSqlServer(connectionString);
}
else
{
    // SQLite
    options.UseSqlite(connectionString ?? "Data Source=growingtales.db");
}
```

### 5.3. Deploy lại
```powershell
.\deploy-to-azure.ps1
```

---

## Bước 6: Chạy Migration

### Option 1: Tự động (Khuyến khích)

Code đã có sẵn trong `Program.cs`:
```csharp
db.Database.EnsureCreated();
```

App sẽ tự động tạo tables khi chạy lần đầu.

### Option 2: Chạy từ Kudu

1. Vào App Service → Advanced Tools
2. Nhấn "Go →"
3. Debug console → PowerShell
4. Chạy:
```powershell
cd site\wwwroot
dotnet ef database update
```

### Option 3: Chạy từ local

```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server

# Set connection string tạm thời
$env:ConnectionStrings__DefaultConnection="Server=tcp:growingtales-sqlserver.database.windows.net,1433;..."

# Run migration
dotnet ef database update

# Unset
Remove-Item Env:\ConnectionStrings__DefaultConnection
```

---

## Bước 7: Test

1. Mở website: `https://growingtales-app.azurewebsites.net`
2. Đăng ký tài khoản mới
3. Tạo truyện
4. Kiểm tra database:
   - Vào Azure Portal → SQL Database → Query editor
   - Login với `sqladmin` và password
   - Chạy query:
     ```sql
     SELECT * FROM Users;
     SELECT * FROM Stories;
     ```
   - Sẽ thấy data vừa tạo!

---

## So sánh SQLite vs Azure SQL

| Tính năng | SQLite | Azure SQL |
|-----------|--------|-----------|
| **Chi phí** | $0 | ~$5-200/tháng |
| **Setup** | Dễ | Phức tạp hơn |
| **Performance** | OK cho < 100 users | Tốt cho mọi scale |
| **Backup** | Thủ công | Tự động hàng ngày |
| **Concurrent writes** | Hạn chế | Tốt |
| **Max size** | Unlimited (nhưng chậm nếu > 1GB) | Unlimited |
| **Khuyến khích** | Dev & small apps | Production apps |

---

## Chi phí Azure SQL

| Tier | DTU/vCore | Storage | Chi phí/tháng |
|------|-----------|---------|---------------|
| **Basic** | 5 DTU | 2GB | ~$5 |
| **Standard S0** | 10 DTU | 250GB | ~$15 |
| **Standard S1** | 20 DTU | 250GB | ~$30 |
| **General Purpose** | 2 vCores | 32GB | ~$200 |

**Khuyến khích**: Bắt đầu với **Basic** ($5/tháng)

---

## Rollback về SQLite

Nếu muốn quay lại dùng SQLite:

1. **Xóa Connection String trong App Service**
   - Configuration → Connection strings → Xóa `DefaultConnection`

2. **Mount lại File Share**
   - Configuration → Path mappings → Add storage mount

3. **Deploy lại code cũ**
   ```powershell
   git checkout <commit-truoc-khi-doi-sang-sql>
   .\deploy-to-azure.ps1
   ```

---

## Tips

1. **Enable auto-backup**
   - SQL Database → Backups
   - Configure retention period

2. **Monitor performance**
   - Query Performance Insight
   - Identify slow queries

3. **Use connection pooling**
   - Đã có sẵn trong EF Core
   - Giảm số connections

4. **Optimize costs**
   - Pause database khi không dùng (DTU-based only)
   - Scale down vào ban đêm

---

**Chúc bạn thành công với Azure SQL! 🚀**
