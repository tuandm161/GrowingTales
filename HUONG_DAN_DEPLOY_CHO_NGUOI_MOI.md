# 🎓 Hướng dẫn Deploy cho người mới bắt đầu với Azure

Đây là hướng dẫn **siêu chi tiết** cho bạn lần đầu deploy lên Azure. Tôi sẽ giải thích từng bước một cách đơn giản nhất.

---

## 🎯 Tổng quan: Bạn sẽ làm gì?

1. ✅ Tạo tài khoản Azure (Free, nhận $200 credit)
2. ✅ Tạo "App Service" = nơi chứa website của bạn
3. ✅ Upload code lên Azure
4. ✅ Cấu hình API keys
5. ✅ Truy cập website qua internet

**Thời gian:** Khoảng 30-45 phút cho lần đầu tiên

---

## Bước 1: Tạo tài khoản Azure (5-10 phút)

### 1.1. Truy cập trang đăng ký
- Mở browser, vào: https://azure.microsoft.com/free/
- Nhấn nút **"Bắt đầu miễn phí"** hoặc **"Start free"**

### 1.2. Đăng nhập Microsoft Account
- Nếu đã có email @outlook.com, @hotmail.com → dùng luôn
- Nếu chưa có → Tạo tài khoản mới (free)

### 1.3. Điền thông tin
Azure sẽ yêu cầu:
- ✅ Thông tin cá nhân (tên, địa chỉ)
- ✅ Số điện thoại (để xác minh)
- ✅ Thẻ tín dụng/debit (để xác minh danh tính - **KHÔNG bị trừ tiền**)

**Lưu ý quan trọng:**
- Azure sẽ giữ ~$1 để verify, sau đó hoàn lại
- 30 ngày đầu: $200 credit miễn phí
- Sau 30 ngày: Chỉ trừ tiền nếu bạn upgrade lên Pay-As-You-Go
- Nếu không upgrade, tài khoản sẽ bị khóa nhưng **KHÔNG bị trừ tiền**

### 1.4. Hoàn tất đăng ký
- Nhấn qua các bước
- Đợi Azure tạo subscription (~1-2 phút)
- Bạn sẽ thấy Azure Portal: https://portal.azure.com

✅ **Xong bước 1!**

---

## Bước 2: Cài Azure CLI (5 phút)

### 2.1. Download Azure CLI
- Mở PowerShell **với quyền Administrator** (chuột phải → Run as Administrator)
- Chạy lệnh:

```powershell
winget install -e --id Microsoft.AzureCLI
```

**Nếu lỗi "winget not found":**
- Tải trực tiếp từ: https://aka.ms/installazurecliwindows
- Chạy file `.msi` vừa tải
- Next → Next → Install

### 2.2. Kiểm tra đã cài thành công
```powershell
# Đóng PowerShell cũ, mở PowerShell mới
az --version
```

Nếu thấy hiện phiên bản (vd: `azure-cli 2.xx.x`) → **Thành công!**

### 2.3. Đăng nhập Azure
```powershell
az login
```

- Browser sẽ tự động mở
- Đăng nhập bằng tài khoản Azure vừa tạo
- Sau khi đăng nhập xong, đóng browser
- PowerShell sẽ hiện danh sách subscriptions

✅ **Xong bước 2!**

---

## Bước 3: Chạy Script Setup (3-5 phút)

Đây là bước **siêu dễ** - script sẽ tự động tạo tất cả resources cần thiết.

### 3.1. Mở PowerShell
```powershell
# Di chuyển vào thư mục project
cd D:\CSharp\WebAPI\GrowingTales

# Chạy script setup
.\setup-azure-resources.ps1
```

### 3.2. Script sẽ tự động:
- ✅ Tạo Resource Group
- ✅ Tạo App Service Plan
- ✅ Tạo Web App
- ✅ Tạo Storage Account
- ✅ Tạo File Share

**Đợi khoảng 2-3 phút**

### 3.3. Kết quả
Bạn sẽ thấy:
```
✅ SETUP HOÀN TẤT!
🌐 URL: https://growingtales-app.azurewebsites.net
```

✅ **Xong bước 3!**

---

## Bước 4: Cấu hình API Keys trong Azure Portal (5-10 phút)

### 4.1. Mở Azure Portal
- Vào: https://portal.azure.com
- Đăng nhập (nếu chưa)

### 4.2. Tìm App Service
- Trong thanh tìm kiếm ở trên, gõ: `growingtales-app`
- Nhấn vào App Service xuất hiện

### 4.3. Vào Configuration
- Bên trái, tìm mục **"Configuration"** (trong phần Settings)
- Nhấn vào
- Chọn tab **"Application settings"**

### 4.4. Thêm các settings

Nhấn nút **"+ New application setting"**, thêm từng cái:

**Setting 1:**
- Name: `GeminiAPI__ApiKey`
- Value: `AIzaSy...` (API key Gemini của bạn)
- OK

**Setting 2:**
- Name: `WhomeAI__ApiKey`
- Value: `whm_...` (API key WhomeAI của bạn)
- OK

**Setting 3:**
- Name: `ASPNETCORE_ENVIRONMENT`
- Value: `Production`
- OK

**Setting 4 (nếu có VNPay):**
- Name: `VnPay__TmnCode`
- Value: `<your-code>`
- OK

**Setting 5 (nếu có VNPay):**
- Name: `VnPay__HashSecret`
- Value: `<your-secret>`
- OK

### 4.5. Save
- Nhấn nút **"Save"** ở trên cùng
- Nhấn **"Continue"** để restart app
- Đợi khoảng 30 giây

✅ **Xong bước 4!**

---

## Bước 5: Mount File Share (5 phút)

Bước này để website có thể lưu database SQLite.

### 5.1. Vẫn trong App Service
- Bên trái, tìm **"Configuration"**
- Chọn tab **"Path mappings"**
- Scroll xuống phần **"Azure storage mounts"**

### 5.2. Thêm storage mount
- Nhấn **"+ New Azure Storage Mount"**
- Điền:
  - **Name**: `database`
  - **Configuration option**: `Basic`
  - **Storage accounts**: Chọn `growingtalesstorage`
  - **Storage type**: `Azure Files`
  - **Share name**: Chọn `growingtales-data`
  - **Mount path**: `/data`
  - **Read only**: Bỏ tích (để app có thể ghi database)
- Nhấn **OK**

### 5.3. Save
- Nhấn **"Save"** ở trên
- Nhấn **"Continue"**

✅ **Xong bước 5!**

---

## Bước 6: Deploy Code (5-10 phút)

### 6.1. Chạy script deploy
Mở PowerShell:
```powershell
cd D:\CSharp\WebAPI\GrowingTales
.\deploy-to-azure.ps1
```

### 6.2. Đợi upload
- Script sẽ build project
- Tạo file zip
- Upload lên Azure
- **Đợi khoảng 3-5 phút**

### 6.3. Kết quả
```
🎉 DEPLOY THÀNH CÔNG!
🌐 URL: https://growingtales-app.azurewebsites.net
```

✅ **Xong bước 6!**

---

## Bước 7: Test Website (10 phút)

### 7.1. Mở website
```powershell
start https://growingtales-app.azurewebsites.net
```

**Lần đầu có thể hơi lâu (30-60 giây)** vì app đang khởi động.

### 7.2. Test các chức năng
- [ ] Trang chủ hiển thị đúng
- [ ] Click "Đăng ký" → Tạo tài khoản mới → **Thành công**
- [ ] Đăng nhập với tài khoản vừa tạo → **Thành công**
- [ ] Click "Tạo truyện"
  - [ ] Nhập tên bé: `Bé An`
  - [ ] Tuổi: `5`
  - [ ] Chủ đề: `Dũng cảm`
  - [ ] Nội dung: `Một chú thỏ nhỏ học cách vượt qua nỗi sợ hãi`
  - [ ] Nhấn "Tạo truyện"
  - [ ] Đợi 20-40 giây
  - [ ] **Truyện được tạo với text và hình ảnh!**
- [ ] Click nút "Đọc" (🔊) → Nghe AI đọc truyện
- [ ] Click "Chia sẻ" → Copy link → Mở tab ẩn danh → **Link hoạt động!**

### 7.3. Nếu có lỗi
```powershell
# Xem logs để biết lỗi gì
az webapp log tail --name growingtales-app --resource-group growingtales-rg

# Restart app
az webapp restart --name growingtales-app --resource-group growingtales-rg
```

✅ **Hoàn thành!**

---

## 🎉 Chúc mừng! Website của bạn đã live!

URL của bạn: `https://growingtales-app.azurewebsites.net`

### Điều chỉnh tên App (nếu muốn)

Nếu muốn đổi tên app (vd: `truyen-be-yeu`):

```powershell
.\setup-azure-resources.ps1 -AppName "truyen-be-yeu"
.\deploy-to-azure.ps1 -AppName "truyen-be-yeu"
```

URL mới: `https://truyen-be-yeu.azurewebsites.net`

---

## 📚 Tài liệu thêm

- **Hướng dẫn đầy đủ**: [AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md)
- **Checklist**: [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

---

## ❓ FAQ

**Q: Có mất phí không?**
A: Nếu dùng tier Free (F1) → Hoàn toàn miễn phí. Nếu dùng B1 Basic → ~$13/tháng.

**Q: Làm sao biết đang dùng bao nhiêu tiền?**
A: Vào Azure Portal → Cost Management → Cost analysis

**Q: Làm sao tắt để không tốn tiền?**
A: 
```powershell
# Xóa toàn bộ resources
az group delete --name growingtales-rg --yes
```

**Q: Website bị chậm?**
A: Nâng cấp từ F1 → B1 hoặc S1, và bật "Always On"

**Q: Làm sao update code sau này?**
A: Chỉ cần chạy lại:
```powershell
.\deploy-to-azure.ps1
```

**Q: Muốn dùng tên miền riêng (vd: truyen.vn)?**
A: Mua domain → Vào App Service → Custom domains → Add domain → Làm theo hướng dẫn

---

## 💡 Tips

1. **Luôn test local trước khi deploy**
   ```powershell
   cd Server
   dotnet run
   # Test tại http://localhost:5002
   ```

2. **Monitor logs sau khi deploy**
   ```powershell
   az webapp log tail --name growingtales-app --resource-group growingtales-rg
   ```

3. **Backup database định kỳ**
   - Download file `growingtales.db` từ File Share
   - Hoặc export data ra SQL

4. **Sử dụng CI/CD để tự động deploy**
   - Mỗi lần push code lên GitHub → Tự động deploy
   - Tiết kiệm thời gian!

---

## 🆘 Cần giúp đỡ?

### Nếu script báo lỗi:

**Lỗi: "Azure CLI not found"**
→ Chưa cài Azure CLI, xem lại Bước 2

**Lỗi: "Not logged in"**
→ Chạy `az login` để đăng nhập

**Lỗi: "App name already taken"**
→ Tên app đã có người dùng, đổi tên khác:
```powershell
.\setup-azure-resources.ps1 -AppName "ten-moi-cua-ban"
```

**Lỗi: "Quota exceeded"**
→ Tài khoản free chỉ cho tạo 10 App Services. Xóa app cũ đi.

### Nếu website báo lỗi:

**"Application Error" khi mở web**
→ Chưa cấu hình API keys, xem lại Bước 4

**"HTTP 500 Error"**
→ Lỗi server, xem logs:
```powershell
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

**Trang trắng hoặc loading mãi**
→ Restart app:
```powershell
az webapp restart --name growingtales-app --resource-group growingtales-rg
```

---

## 📞 Liên hệ

Nếu vẫn gặp vấn đề:
1. Screenshot lỗi
2. Copy logs
3. Gửi vào group/forum hỗ trợ

---

**Chúc bạn thành công! Đừng ngại thử - Azure Free tier rất an toàn! 💪**
