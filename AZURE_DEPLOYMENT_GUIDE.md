# Hướng dẫn Deploy GrowingTales lên Azure

## Bước 1: Chuẩn bị tài khoản Azure

### 1.1. Tạo tài khoản Azure (nếu chưa có)
1. Truy cập: https://azure.microsoft.com/free/
2. Nhấn "Start free" hoặc "Try Azure for free"
3. Đăng nhập bằng Microsoft Account
4. Điền thông tin cá nhân và thẻ tín dụng (để xác minh, không tính phí trong 30 ngày đầu)
5. Bạn sẽ nhận được **$200 credit** để sử dụng trong 30 ngày đầu

### 1.2. Cài đặt Azure CLI (tuỳ chọn nhưng khuyến khích)
```powershell
# Tải và cài đặt từ: https://aka.ms/installazurecliwindows
# Hoặc dùng winget
winget install -e --id Microsoft.AzureCLI

# Kiểm tra đã cài thành công
az --version

# Đăng nhập
az login
```

## Bước 2: Chuẩn bị dự án

### 2.1. Cập nhật appsettings cho Production
File `appsettings.Production.json` đã được tạo sẵn với các cấu hình phù hợp cho Azure.

### 2.2. Thêm file `.dockerignore` và `.deployment` (đã tạo)

### 2.3. Build và test local
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

## Bước 3: Tạo Resources trên Azure Portal

### 3.1. Đăng nhập Azure Portal
- Truy cập: https://portal.azure.com
- Đăng nhập với tài khoản đã tạo

### 3.2. Tạo Resource Group
1. Tìm "Resource groups" trong thanh tìm kiếm
2. Nhấn "+ Create"
3. Điền thông tin:
   - **Subscription**: Chọn subscription của bạn (Free Trial)
   - **Resource group name**: `growingtales-rg`
   - **Region**: `Southeast Asia` (gần Việt Nam nhất)
4. Nhấn "Review + create" → "Create"

### 3.3. Tạo App Service Plan
1. Tìm "App Service plans" trong thanh tìm kiếm
2. Nhấn "+ Create"
3. Điền thông tin:
   - **Resource Group**: Chọn `growingtales-rg`
   - **Name**: `growingtales-plan`
   - **Operating System**: `Windows` hoặc `Linux` (khuyến khích Linux)
   - **Region**: `Southeast Asia`
   - **Pricing tier**: Nhấn "Explore pricing plans"
     - Chọn **F1 (Free)** cho test, hoặc **B1 (Basic)** cho production (~$13/tháng)
4. Nhấn "Review + create" → "Create"

### 3.4. Tạo Web App (App Service)
1. Tìm "App Services" trong thanh tìm kiếm
2. Nhấn "+ Create"
3. Điền thông tin:

**Basics:**
   - **Resource Group**: `growingtales-rg`
   - **Name**: `growingtales-app` (phải unique, sẽ thành growingtales-app.azurewebsites.net)
   - **Publish**: `Code`
   - **Runtime stack**: `.NET 8 (LTS)` hoặc `.NET 9`
   - **Operating System**: `Windows` hoặc `Linux`
   - **Region**: `Southeast Asia`
   - **App Service Plan**: Chọn `growingtales-plan` đã tạo

**Deployment (Optional):**
   - Bỏ qua hoặc cấu hình GitHub Actions sau

**Networking:**
   - Giữ mặc định

**Monitoring:**
   - **Enable Application Insights**: `Yes` (để theo dõi logs)
   - Tạo mới hoặc chọn existing

4. Nhấn "Review + create" → "Create"
5. Đợi 2-3 phút để Azure tạo resources

### 3.5. Tạo Azure SQL Database (Tuỳ chọn - thay SQLite)

**Lưu ý:** Nếu muốn dùng SQLite trên Azure, bỏ qua bước này và xem phần "Sử dụng SQLite trên Azure" bên dưới.

Để có database bền vững hơn, nên dùng Azure SQL:

1. Tìm "SQL databases" trong thanh tìm kiếm
2. Nhấn "+ Create"
3. Điền thông tin:
   - **Resource Group**: `growingtales-rg`
   - **Database name**: `growingtales-db`
   - **Server**: Nhấn "Create new"
     - **Server name**: `growingtales-server` (phải unique)
     - **Location**: `Southeast Asia`
     - **Authentication**: `SQL authentication`
     - **Server admin login**: `sqladmin`
     - **Password**: Tạo mật khẩu mạnh (ghi nhớ lại)
   - **Compute + storage**: Nhấn "Configure database"
     - Chọn **Basic** (5 DTU, ~$5/tháng) cho test
4. Trong tab "Networking":
   - **Connectivity method**: `Public endpoint`
   - **Allow Azure services**: `Yes`
   - **Add current client IP**: `Yes`
5. Nhấn "Review + create" → "Create"

## Bước 4: Deploy từ Visual Studio Code

### 4.1. Cài đặt Azure Tools Extension
1. Mở VS Code
2. Nhấn Extensions (Ctrl+Shift+X)
3. Tìm và cài "Azure App Service"
4. Đăng nhập Azure: Nhấn Azure icon bên trái → Sign in

### 4.2. Deploy
1. Nhấn chuột phải vào thư mục `Server`
2. Chọn "Deploy to Web App..."
3. Chọn subscription của bạn
4. Chọn Web App: `growingtales-app`
5. Xác nhận deploy
6. Đợi 3-5 phút để upload và deploy

## Bước 5: Deploy từ Azure CLI (Cách thay thế)

```powershell
# Đăng nhập
az login

# Build và publish
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet publish -c Release -o ./publish

# Tạo file zip
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force

# Deploy lên Azure
az webapp deployment source config-zip `
  --resource-group growingtales-rg `
  --name growingtales-app `
  --src ./publish.zip

# Xem logs
az webapp log tail --resource-group growingtales-rg --name growingtales-app
```

## Bước 6: Cấu hình Environment Variables

### 6.1. Cấu hình trên Azure Portal
1. Vào App Service: `growingtales-app`
2. Bên trái, chọn "Configuration" trong phần "Settings"
3. Nhấn tab "Application settings"
4. Thêm các settings sau (nhấn "+ New application setting"):

**Cơ bản:**
```
ASPNETCORE_ENVIRONMENT = Production
```

**Gemini API:**
```
GeminiAPI__ApiKey = AIzaSy... (API key của bạn)
GeminiAPI__Model = gemini-2.0-flash-exp
```

**WhomeAI (Image Generation):**
```
WhomeAI__ApiKey = whm_... (API key của bạn)
WhomeAI__BaseUrl = https://api.whome.so
```

**VNPay:**
```
VnPay__TmnCode = XXXXXXXX
VnPay__HashSecret = XXXXXXXX
VnPay__Url = https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
```

**Database (nếu dùng Azure SQL):**
```
ConnectionStrings__DefaultConnection = Server=tcp:growingtales-server.database.windows.net,1433;Initial Catalog=growingtales-db;Persist Security Info=False;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

4. Nhấn "Save" ở trên cùng
5. Nhấn "Continue" để restart app

### 6.2. Hoặc dùng Azure CLI
```powershell
# Set multiple settings at once
az webapp config appsettings set `
  --resource-group growingtales-rg `
  --name growingtales-app `
  --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    "GeminiAPI__ApiKey=YOUR_GEMINI_KEY" `
    "WhomeAI__ApiKey=YOUR_WHOME_KEY"
```

## Bước 7: Sử dụng SQLite trên Azure (Khuyến khích cho bắt đầu)

SQLite có thể chạy trên Azure App Service, nhưng cần lưu ý:

### 7.1. Cấu hình File System
1. Vào App Service → Configuration
2. Thêm setting:
```
WEBSITE_LOCAL_CACHE_OPTION = Always
WEBSITE_LOCAL_CACHE_SIZEINMB = 1000
```

### 7.2. Sử dụng Azure File Share (Khuyến khích)
1. Tạo Storage Account:
   - Tìm "Storage accounts" → Create
   - Name: `growingtalesstorage`
   - Region: `Southeast Asia`
   - Performance: `Standard`
   - Create

2. Tạo File Share:
   - Vào Storage Account vừa tạo
   - File shares → + File share
   - Name: `growingtales-data`
   - Create

3. Mount vào App Service:
   - Vào App Service → Configuration → Path mappings
   - Nhấn "+ New Azure Storage Mount"
   - Name: `database`
   - Storage accounts: Chọn `growingtalesstorage`
   - Share name: `growingtales-data`
   - Mount path: `/data`
   - Save

4. Cập nhật Connection String:
```
ConnectionStrings__DefaultConnection = Data Source=/data/growingtales.db
```

## Bước 8: Database Migration (TỰ ĐỘNG - Không cần làm gì!)

### ✅ Database tự động được tạo khi deploy!

Code trong `Program.cs` đã có sẵn:
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Tự động chạy migrations
}
```

**Nghĩa là:**
- Khi deploy lần đầu → Database tự động tạo
- Khi có migration mới → Tự động apply
- **BẠN KHÔNG CẦN** chạy lệnh thủ công!

### Kiểm tra database đã tạo thành công

1. Vào App Service → Log stream
2. Tìm dòng log:
```
Checking database and running migrations...
Database migrations completed successfully
```

3. Hoặc test bằng cách đăng ký user mới trên website
   - Nếu thành công → Database hoạt động!

### Nếu muốn chạy migration thủ công (Không khuyến khích)

**Lưu ý:** Azure Kudu không có `dotnet ef` tool sẵn, cần cài thủ công:

```powershell
# Trong Kudu Console
cd site\wwwroot
dotnet tool install --global dotnet-ef
export PATH="$PATH:/root/.dotnet/tools"  # Linux
# Hoặc set PATH=%PATH%;C:\local\UserProfile\.dotnet\tools  # Windows
dotnet ef database update
```

**Nhưng không cần thiết vì đã tự động rồi!**

## Bước 9: Cấu hình Domain và SSL

### 9.1. Domain mặc định
- URL mặc định: `https://growingtales-app.azurewebsites.net`
- SSL certificate tự động có sẵn

### 9.2. Custom Domain (tuỳ chọn)
1. Mua domain từ các nhà cung cấp (GoDaddy, Namecheap, etc.)
2. Vào App Service → Custom domains
3. Nhấn "+ Add custom domain"
4. Nhập domain của bạn (vd: growingtales.vn)
5. Làm theo hướng dẫn để thêm DNS records
6. Validate và bind domain

### 9.3. SSL Certificate cho Custom Domain
1. Sau khi bind domain thành công
2. Vào App Service → Certificates
3. Chọn "Managed Certificate" (Free) hoặc upload certificate của bạn
4. Bind certificate với domain

## Bước 10: Monitoring và Logs

### 10.1. Xem Logs trực tiếp
1. Vào App Service → Log stream
2. Hoặc dùng CLI:
```powershell
az webapp log tail --resource-group growingtales-rg --name growingtales-app
```

### 10.2. Application Insights
1. Vào App Service → Application Insights
2. Xem metrics: Response time, Failed requests, Server response time
3. Xem exceptions và errors

### 10.3. Download Logs
1. Vào App Service → Advanced Tools (Kudu)
2. Tools → Diagnostic dump
3. Hoặc vào `https://growingtales-app.scm.azurewebsites.net/api/logs/docker`

## Bước 11: Scaling và Performance

### 11.1. Scale Up (Tăng tài nguyên)
1. Vào App Service → Scale up (App Service plan)
2. Chọn tier phù hợp:
   - F1: Free (1 GB RAM, 60 mins/day)
   - B1: Basic (~$13/month, 1.75 GB RAM, Always on)
   - S1: Standard (~$70/month, 1.75 GB RAM, Auto-scale)
   - P1V2: Premium (~$146/month, 3.5 GB RAM, High performance)

### 11.2. Scale Out (Tăng số instance)
1. Vào App Service → Scale out (App Service plan)
2. Chọn số instance (cần tier S1 trở lên)

### 11.3. Enable "Always On"
1. Vào App Service → Configuration → General settings
2. Always on: `On` (cần tier B1 trở lên)
3. Save

## Bước 12: CI/CD với GitHub Actions (Tuỳ chọn)

### 12.1. Kết nối GitHub
1. Vào App Service → Deployment Center
2. Chọn "GitHub"
3. Authorize và chọn repository
4. Chọn branch: `main`
5. Azure sẽ tự động tạo GitHub Actions workflow

### 12.2. File workflow đã được tạo tại `.github/workflows/azure-webapps-dotnet-core.yml`

Từ giờ, mỗi khi push code lên GitHub, ứng dụng sẽ tự động build và deploy!

## Bước 13: Troubleshooting

### Lỗi thường gặp:

**1. "Application Error" khi truy cập web:**
- Kiểm tra logs: App Service → Log stream
- Kiểm tra Environment variables đã cấu hình đúng chưa
- Kiểm tra database connection string

**2. "HTTP Error 500.30 - ASP.NET Core app failed to start":**
- Check .NET version trên Azure khớp với project
- Kiểm tra startup logs trong Application Insights

**3. Database connection failed:**
- Nếu dùng Azure SQL: Check firewall rules, cho phép Azure services
- Nếu dùng SQLite: Check file permissions và mount path

**4. API keys không hoạt động:**
- Kiểm tra Configuration → Application settings
- Đảm bảo format đúng (dùng `__` thay vì `:`)

**5. Ứng dụng chạy chậm:**
- Upgrade App Service Plan
- Enable Application Insights để phân tích performance
- Optimize database queries

## Chi phí dự kiến

### Option 1: Free Tier (Test)
- App Service: F1 Free
- Database: SQLite (free)
- Storage: 1GB free
- **Tổng: $0/tháng**
- Giới hạn: 60 mins CPU/day, sleep sau 20 mins không hoạt động

### Option 2: Basic (Production nhỏ)
- App Service: B1 (~$13/month)
- Azure SQL: Basic (~$5/month)
- Storage: Standard (~$0.05/month)
- Application Insights: Free tier (5GB/month)
- **Tổng: ~$18-20/tháng**

### Option 3: Standard (Production)
- App Service: S1 (~$70/month)
- Azure SQL: Standard S0 (~$15/month)
- Storage: Standard (~$0.50/month)
- **Tổng: ~$85-90/tháng**

## Các lệnh hữu ích

```powershell
# Restart app
az webapp restart --name growingtales-app --resource-group growingtales-rg

# View logs
az webapp log tail --name growingtales-app --resource-group growingtales-rg

# List all resources
az resource list --resource-group growingtales-rg --output table

# Delete everything (cẩn thận!)
az group delete --name growingtales-rg --yes --no-wait
```

## Tài liệu tham khảo

- Azure App Service: https://docs.microsoft.com/azure/app-service/
- Azure SQL Database: https://docs.microsoft.com/azure/azure-sql/
- GitHub Actions: https://docs.github.com/actions
- ASP.NET Core deployment: https://docs.microsoft.com/aspnet/core/host-and-deploy/azure-apps/

## Hỗ trợ

Nếu gặp vấn đề, check:
1. Application Insights logs
2. Kudu diagnostic logs
3. Azure Status: https://status.azure.com/
4. Stack Overflow với tag `azure-app-service`

---

**Chúc bạn deploy thành công! 🚀**
