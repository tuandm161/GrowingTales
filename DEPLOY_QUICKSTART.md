# Hướng dẫn Deploy nhanh lên Azure

## Cách 1: Sử dụng Scripts tự động (Khuyến khích cho người mới)

### Bước 1: Cài đặt Azure CLI
```powershell
# Tải và cài đặt Azure CLI
winget install -e --id Microsoft.AzureCLI

# Hoặc tải từ: https://aka.ms/installazurecliwindows
```

### Bước 2: Đăng nhập Azure
```powershell
az login
```

### Bước 3: Tạo resources trên Azure
```powershell
# Chạy script setup (tạo App Service, Storage, etc.)
.\setup-azure-resources.ps1

# Hoặc với tùy chỉnh:
.\setup-azure-resources.ps1 -AppName "ten-app-cua-ban" -Sku "B1"
```

### Bước 4: Cấu hình API Keys
1. Vào Azure Portal: https://portal.azure.com
2. Tìm App Service của bạn (vd: `growingtales-app`)
3. Vào **Configuration** → **Application settings**
4. Thêm các settings:
   - `GeminiAPI__ApiKey` = Gemini API key của bạn
   - `WhomeAI__ApiKey` = WhomeAI key của bạn
   - `VnPay__TmnCode` = VNPay TmnCode (nếu có)
   - `VnPay__HashSecret` = VNPay HashSecret (nếu có)
5. Nhấn **Save**

### Bước 5: Deploy code
```powershell
.\deploy-to-azure.ps1

# Hoặc với tên app khác:
.\deploy-to-azure.ps1 -AppName "ten-app-cua-ban"
```

### Bước 6: Xem logs và test
```powershell
# Xem logs realtime
az webapp log tail --name growingtales-app --resource-group growingtales-rg

# Mở browser
start https://growingtales-app.azurewebsites.net
```

---

## Cách 2: Deploy thủ công qua Azure Portal

### Bước 1: Tạo Web App
1. Vào https://portal.azure.com
2. Tìm "App Services" → Create
3. Điền:
   - **Name**: `growingtales-app`
   - **Runtime**: `.NET 8`
   - **Region**: `Southeast Asia`
   - **Pricing**: Chọn `F1 Free` hoặc `B1 Basic`
4. Create

### Bước 2: Deploy từ VS Code
1. Cài Extension: **Azure App Service**
2. Mở thư mục `Server`
3. Chuột phải → **Deploy to Web App**
4. Chọn App Service vừa tạo
5. Đợi upload xong

### Bước 3: Cấu hình
- Làm theo **Bước 4** của Cách 1 (cấu hình API keys)

---

## Cách 3: CI/CD với GitHub Actions (Tự động)

### Bước 1: Push code lên GitHub
```powershell
git add .
git commit -m "Prepare for Azure deployment"
git push
```

### Bước 2: Lấy Publish Profile từ Azure
1. Vào App Service → Overview
2. Nhấn "Download publish profile"
3. Mở file `.PublishSettings` vừa tải
4. Copy toàn bộ nội dung

### Bước 3: Thêm Secret vào GitHub
1. Vào GitHub repository → Settings → Secrets and variables → Actions
2. Nhấn "New repository secret"
3. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
4. Value: Paste nội dung publish profile
5. Add secret

### Bước 4: Trigger deployment
File workflow đã có sẵn tại `.github/workflows/azure-webapps-dotnet-core.yml`

Từ giờ, mỗi khi push code lên branch `main`, GitHub Actions sẽ tự động:
- Build project
- Run tests
- Deploy lên Azure

---

## Checklist sau khi deploy

- [ ] Truy cập được URL: `https://your-app.azurewebsites.net`
- [ ] Trang chủ hiển thị đúng
- [ ] Đăng ký tài khoản được
- [ ] Đăng nhập được
- [ ] Tạo truyện được (test cả text và audio)
- [ ] Xem truyện, chuyển trang hoạt động
- [ ] Text-to-Speech hoạt động
- [ ] Share link hoạt động
- [ ] Upgrade Premium hoạt động (nếu đã cấu hình VNPay)

---

## Troubleshooting

### Lỗi: "Application Error"
```powershell
# Xem logs để biết lỗi cụ thể
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

### Lỗi: Database connection
- Kiểm tra Connection String trong Configuration
- Nếu dùng SQLite: Đảm bảo đã mount File Share
- Nếu dùng Azure SQL: Check firewall rules

### App chạy chậm hoặc timeout
- Upgrade App Service Plan từ F1 → B1
- Enable "Always On" trong Configuration

### API keys không hoạt động
- Kiểm tra format: dùng `__` thay vì `:`
- VD: `GeminiAPI__ApiKey` chứ không phải `GeminiAPI:ApiKey`

---

## Chi phí ước tính

| Tier | App Service | Storage | Tổng/tháng |
|------|-------------|---------|------------|
| **Free** | F1 (Free) | 1GB (Free) | **$0** |
| **Basic** | B1 ($13) | Standard ($0.05) | **~$13** |
| **Standard** | S1 ($70) | Standard ($0.50) | **~$70** |

**Khuyến nghị:**
- Test: Dùng F1 Free
- Production: Dùng B1 Basic (~$13/month)

---

## Liên hệ hỗ trợ

Nếu gặp vấn đề:
1. Xem logs trong Azure Portal
2. Check Application Insights
3. Đọc docs: https://docs.microsoft.com/azure/app-service/

**Chúc bạn deploy thành công! 🎉**
