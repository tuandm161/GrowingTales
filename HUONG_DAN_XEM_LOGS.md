# 📋 Hướng Dẫn Xem Logs Trên Azure

## 🚨 Vấn đề: "Xem log không thấy gì sau khi deploy"

---

## ✅ Giải pháp: BẬT LOGGING TRÊN AZURE

### Bước 1: Bật Application Logging trong Azure Portal

1. **Mở Azure Portal**
   - Vào: https://portal.azure.com
   - Đăng nhập

2. **Tìm App Service**
   - Thanh tìm kiếm: `growingtales-app`
   - Click vào App Service

3. **Vào Configuration**
   - Menu bên trái → **"Configuration"**
   - Tab **"General settings"** (hoặc scroll xuống)

4. **Bật Logging**
   
   Tìm phần **"Logging"** và bật:
   
   - ✅ **Application logging (Filesystem)**: `On`
   - ✅ **Level**: `Information` (hoặc `Verbose` để xem nhiều hơn)
   - ✅ **Web server logging**: `On`
   - ✅ **Detailed error messages**: `On`
   - ✅ **Failed request tracing**: `On` (tùy chọn)

5. **Save**
   - Nhấn **"Save"** ở trên cùng
   - Nhấn **"Continue"** để restart app
   - Đợi 30-60 giây

---

## ✅ Cách 1: Xem Logs Qua Azure Portal (Dễ nhất)

### Log Stream (Real-time)

1. **Vào App Service** → `growingtales-app`
2. Menu bên trái → **"Log stream"**
3. **Logs sẽ hiện real-time!**

**Bạn sẽ thấy:**
```
[2026-01-18 10:30:15] Checking database and running migrations...
[2026-01-18 10:30:16] Database migrations completed successfully
[2026-01-18 10:30:17] Now listening on: http://0.0.0.0:8080
```

---

## ✅ Cách 2: Xem Logs Qua Azure CLI (PowerShell)

### Bật Log Stream qua CLI

```powershell
# Bật Application Logging
az webapp log config --name growingtales-app --resource-group growingtales-rg `
    --application-logging filesystem `
    --level information `
    --web-server-logging filesystem

# Xem logs real-time
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

**Lệnh này sẽ hiển thị logs real-time!**

---

## ✅ Cách 3: Download Logs File

### Qua Azure Portal

1. **App Service** → **"Advanced Tools"** (Kudu)
2. Click **"Go →"**
3. Menu trên → **"Tools"** → **"Diagnostic dump"**
4. Download logs

### Qua Azure CLI

```powershell
# Download logs
az webapp log download --name growingtales-app --resource-group growingtales-rg `
    --log-file logs.zip
```

---

## ✅ Cách 4: Xem Logs Qua Kudu Console

1. **App Service** → **"Advanced Tools"** → **"Go →"**
2. Menu trên → **"Debug console"** → **"CMD"** (hoặc **"PowerShell"**)
3. Điều hướng:
   ```
   cd LogFiles
   cd Application
   ```
4. Xem file logs:
   ```
   type LoggingErrors.txt
   type LoggingOutput.txt
   ```

---

## 🔍 Tìm Logs Database Migration

Sau khi bật logging, bạn sẽ thấy logs như này:

```
[2026-01-18 10:30:15] info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 8.0.0 initialized 'AppDbContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: None
[2026-01-18 10:30:15] info: Program[0]
      Checking database and running migrations...
[2026-01-18 10:30:16] info: Microsoft.EntityFrameworkCore.Migrations[20100]
      Applying migration '20260120040039_InitialCreate'.
[2026-01-18 10:30:17] info: Program[0]
      Database migrations completed successfully
[2026-01-18 10:30:18] info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:8080
```

---

## 🚨 Nếu Vẫn Không Thấy Logs

### Kiểm tra 1: App có đang chạy không?

```powershell
# Kiểm tra status
az webapp show --name growingtales-app --resource-group growingtales-rg --query state
```

Nếu thấy `"Stopped"` → Restart:
```powershell
az webapp start --name growingtales-app --resource-group growingtales-rg
```

### Kiểm tra 2: Restart App để logs xuất hiện

```powershell
az webapp restart --name growingtales-app --resource-group growingtales-rg
```

Sau đó đợi 30 giây và xem lại Log Stream.

### Kiểm tra 3: Kiểm tra Log Level

Đảm bảo Log Level = `Information` hoặc `Verbose`:
- Azure Portal → Configuration → General settings
- Application logging level: `Information`

### Kiểm tra 4: Test bằng cách truy cập website

1. Mở: `https://growingtales-app.azurewebsites.net`
2. Thực hiện một action (vd: đăng ký user)
3. Xem Log Stream → Sẽ thấy logs mới

---

## 📊 Script Tự Động Bật Logging

Tạo file `enable-logging.ps1`:

```powershell
# Enable logging cho Azure App Service

Write-Host "🔧 Bật logging cho Azure App Service..." -ForegroundColor Cyan

$appName = "growingtales-app"
$resourceGroup = "growingtales-rg"

# Bật Application Logging
Write-Host "📝 Bật Application Logging..." -ForegroundColor Yellow
az webapp log config `
    --name $appName `
    --resource-group $resourceGroup `
    --application-logging filesystem `
    --level information `
    --web-server-logging filesystem `
    --detailed-error-messages true `
    --failed-request-tracing true

Write-Host "✅ Đã bật logging!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Xem logs real-time:" -ForegroundColor Cyan
Write-Host "   az webapp log tail --name $appName --resource-group $resourceGroup" -ForegroundColor Gray
Write-Host ""
Write-Host "🌐 Hoặc xem qua Azure Portal:" -ForegroundColor Cyan
Write-Host "   App Service → Log stream" -ForegroundColor Gray
```

---

## 🎯 Checklist: Đảm Bảo Logs Hoạt Động

- [ ] ✅ Application logging (Filesystem) = `On`
- [ ] ✅ Log Level = `Information` hoặc `Verbose`
- [ ] ✅ Web server logging = `On`
- [ ] ✅ Detailed error messages = `On`
- [ ] ✅ Đã Save và Restart app
- [ ] ✅ Đã đợi 30-60 giây sau restart
- [ ] ✅ Đã mở Log Stream hoặc chạy `az webapp log tail`

---

## 💡 Tips

1. **Log Stream tốt nhất cho real-time**
   - Azure Portal → Log stream
   - Hoặc: `az webapp log tail`

2. **Logs được lưu 24-48 giờ**
   - Sau đó tự động xóa
   - Nếu cần lưu lâu hơn → Download logs

3. **Verbose logs = Nhiều thông tin hơn**
   - Nhưng tốn nhiều storage hơn
   - Dùng khi debug

4. **Application Insights (Nâng cao)**
   - Tích hợp Application Insights để xem logs tốt hơn
   - Có dashboard và analytics

---

## 🔗 Xem Thêm

- **Azure Portal Log Stream**: App Service → Log stream
- **CLI Logs**: `az webapp log tail`
- **Kudu Console**: App Service → Advanced Tools → Go →

---

**Sau khi bật logging, bạn sẽ thấy tất cả logs! 📊**
