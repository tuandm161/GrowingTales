# 🎯 BẮT ĐẦU TỪ ĐÂY - Hướng dẫn toàn bộ từ A-Z

**Dành cho người hoàn toàn mới với Azure** ⭐

---

## ✅ Phần 1: KIỂM TRA DATABASE (2 phút)

### Bạn đang lo lắng về database? Đừng lo!

Chạy lệnh này:
```powershell
cd D:\CSharp\WebAPI\GrowingTales
.\check-database.ps1
```

Nếu thấy:
```
Status: ✅ SẴN SÀNG
```

→ **Database đã OK! Chuyển sang Phần 2**

Nếu thấy lỗi → Chạy lệnh này:
```powershell
cd Server
dotnet ef database update
```

---

## ✅ Phần 2: TEST LOCAL (5 phút)

Đảm bảo app chạy tốt trên máy bạn trước khi deploy.

### Bước 1: Chạy app
```powershell
cd D:\CSharp\WebAPI\GrowingTales\Server
dotnet run
```

### Bước 2: Mở browser
- URL: http://localhost:5002

### Bước 3: Test nhanh
1. Trang chủ có hiển thị không? → ✅
2. Click "Đăng ký" → Tạo tài khoản → ✅
3. Click "Tạo truyện" → Điền form:
   - Tên: `Bé An`
   - Tuổi: `5`
   - Chủ đề: `Dũng cảm`
   - Nội dung: `Chú thỏ nhỏ dũng cảm`
   - Nhấn "Tạo truyện"
4. Đợi 20-30 giây → Truyện hiển thị → ✅

**Nếu tất cả ✅ → Tắt app (Ctrl+C) và chuyển Phần 3**

---

## ✅ Phần 3: LẤY API KEYS (10-15 phút)

### 3.1. Gemini API Key (BẮT BUỘC)

**Bước 1:** Vào https://aistudio.google.com/apikey

**Bước 2:** Đăng nhập Google Account

**Bước 3:** Nhấn **"Create API Key"**

**Bước 4:** Copy key (dạng: `AIzaSy...`)

**Bước 5:** Lưu vào Notepad, ghi nhớ!

### 3.2. WhomeAI Key (BẮT BUỘC)

**Bước 1:** Vào https://whome.so

**Bước 2:** Sign Up → Tạo tài khoản

**Bước 3:** Vào Dashboard → API Keys

**Bước 4:** Copy key (dạng: `whm_...`)

**Bước 5:** Lưu vào Notepad!

### 3.3. VNPay (TÙY CHỌN - Có thể bỏ qua)

Nếu muốn có tính năng thanh toán thật:
- Đăng ký: https://vnpay.vn
- Hoặc dùng sandbox: https://sandbox.vnpayment.vn/

**Có thể bỏ qua bước này!** App vẫn chạy bình thường.

---

## ✅ Phần 4: TẠO AZURE RESOURCES (5 phút)

### Bước 1: Cài Azure CLI

```powershell
# Mở PowerShell với quyền Administrator
# Chuột phải PowerShell → Run as Administrator

winget install -e --id Microsoft.AzureCLI
```

**Hoặc tải từ:** https://aka.ms/installazurecliwindows

### Bước 2: Đăng nhập Azure

```powershell
# Đóng PowerShell cũ, mở PowerShell mới
az login
```

- Browser tự động mở
- Đăng nhập Microsoft Account (đã đăng ký Azure)
- Đóng browser
- PowerShell hiện thông tin subscription

### Bước 3: Chạy script setup

```powershell
cd D:\CSharp\WebAPI\GrowingTales
.\setup-azure-resources.ps1
```

**Script sẽ tự động tạo:**
- Resource Group
- App Service Plan (Free tier)
- Web App
- Storage Account
- File Share

**Đợi 2-3 phút...**

**Kết quả:**
```
✅ SETUP HOÀN TẤT!
🌐 URL: https://growingtales-app.azurewebsites.net
```

---

## ✅ Phần 5: CÁI ĐẶT API KEYS TRÊN AZURE (5 phút)

### Bước 1: Mở Azure Portal
- Vào: https://portal.azure.com

### Bước 2: Tìm App Service
- Thanh tìm kiếm ở trên
- Gõ: `growingtales-app`
- Click vào kết quả

### Bước 3: Vào Configuration
- Menu bên trái
- Click **"Configuration"**
- Tab **"Application settings"**

### Bước 4: Thêm API keys

Click **"+ New application setting"** và thêm:

**Setting 1:**
```
Name: GeminiAPI__ApiKey
Value: AIzaSy... (paste key của bạn)
```
→ Click OK

**Setting 2:**
```
Name: WhomeAI__ApiKey
Value: whm_... (paste key của bạn)
```
→ Click OK

**Setting 3:**
```
Name: ASPNETCORE_ENVIRONMENT
Value: Production
```
→ Click OK

### Bước 5: Save và Restart
- Nhấn **"Save"** ở trên cùng
- Nhấn **"Continue"**
- Đợi 30 giây cho app restart

---

## ✅ Phần 6: MOUNT FILE SHARE (3 phút)

Để database có thể lưu trữ dữ liệu lâu dài.

### Vẫn trong Configuration

**Bước 1:** Chọn tab **"Path mappings"**

**Bước 2:** Scroll xuống **"Azure storage mounts"**

**Bước 3:** Click **"+ New Azure Storage Mount"**

**Bước 4:** Điền thông tin:
```
Name: database
Configuration option: Basic
Storage accounts: growingtalesstorage
Storage type: Azure Files
Share name: growingtales-data
Mount path: /data
Read only: ☐ (BỎ TÍCH - để app ghi được)
```

**Bước 5:** Click **OK**

**Bước 6:** Click **"Save"** ở trên → **"Continue"**

---

## ✅ Phần 7: DEPLOY CODE (5 phút)

### Chạy 1 lệnh duy nhất:

```powershell
cd D:\CSharp\WebAPI\GrowingTales
.\deploy-to-azure.ps1
```

**Script sẽ:**
- Build project
- Tạo file zip
- Upload lên Azure
- Deploy

**Đợi 3-5 phút...**

**Kết quả:**
```
🎉 DEPLOY THÀNH CÔNG!
🌐 URL: https://growingtales-app.azurewebsites.net
```

---

## ✅ Phần 8: TEST WEBSITE TRÊN AZURE (5 phút)

### Bước 1: Mở website
```powershell
start https://growingtales-app.azurewebsites.net
```

**Lần đầu có thể lâu 30-60 giây** (app đang khởi động)

### Bước 2: Test các chức năng

1. **Trang chủ hiển thị** → ✅
2. **Click "Đăng ký"**
   - Email: `user@example.com`
   - Tên: `Test User`
   - Mật khẩu: `Test123!`
   - Đăng ký → ✅

3. **Đăng nhập thành công** → ✅

4. **Click "Tạo truyện"**
   - Tên bé: `Bé An`
   - Tuổi: `5`
   - Chủ đề: `Dũng cảm`
   - Nội dung: `Một chú thỏ nhỏ học cách vượt qua nỗi sợ hãi`
   - Click "Tạo truyện"
   - **Đợi 30-40 giây** (AI đang tạo)
   - Truyện hiển thị với hình ảnh → ✅

5. **Click nút "Đọc"** (🔊)
   - Nghe AI đọc truyện → ✅

6. **Click "Chia sẻ"**
   - Copy link
   - Mở tab ẩn danh
   - Paste link
   - Truyện hiển thị → ✅

**Nếu tất cả ✅ → THÀNH CÔNG! 🎉**

---

## 🆘 Nếu có lỗi

### Lỗi: "Application Error" hoặc trang trắng

**Giải pháp 1: Xem logs**
```powershell
az webapp log tail --name growingtales-app --resource-group growingtales-rg
```

**Giải pháp 2: Restart app**
```powershell
az webapp restart --name growingtales-app --resource-group growingtales-rg
```

**Giải pháp 3: Kiểm tra API keys**
- Vào Azure Portal → App Service → Configuration
- Đảm bảo `GeminiAPI__ApiKey` và `WhomeAI__ApiKey` đã điền đúng
- Nhấn Save

### Lỗi: "Failed to create story" hoặc tạo truyện lâu quá

**Nguyên nhân:** API keys chưa đúng hoặc hết quota

**Kiểm tra:**
1. API key Gemini có đúng không?
2. API key WhomeAI có đúng không?
3. Xem logs để biết lỗi cụ thể

### Lỗi: Database-related errors

**Giải pháp:**
1. Kiểm tra File Share đã mount chưa (Phần 6)
2. Restart app
3. Nếu vẫn lỗi, xem logs chi tiết

---

## 🎉 CHECKLIST HOÀN THÀNH

In ra và tích dần:

- [ ] ✅ Phần 1: Database OK (chạy `.\check-database.ps1`)
- [ ] ✅ Phần 2: App chạy local OK
- [ ] ✅ Phần 3: Có API keys (Gemini + WhomeAI)
- [ ] ✅ Phần 4: Azure resources đã tạo (chạy `.\setup-azure-resources.ps1`)
- [ ] ✅ Phần 5: API keys đã cấu hình trong Azure Portal
- [ ] ✅ Phần 6: File Share đã mount
- [ ] ✅ Phần 7: Code đã deploy (chạy `.\deploy-to-azure.ps1`)
- [ ] ✅ Phần 8: Website hoạt động, test thành công

**Nếu tất cả ✅ → XIN CHÚC MỪNG! Website của bạn đã LIVE! 🚀**

---

## 📞 Cần giúp thêm?

1. **Database:** Đọc [DATABASE_SETUP.md](DATABASE_SETUP.md)
2. **Deploy:** Đọc [HUONG_DAN_DEPLOY_CHO_NGUOI_MOI.md](HUONG_DAN_DEPLOY_CHO_NGUOI_MOI.md)
3. **Verify:** Đọc [VERIFY_DATABASE.md](VERIFY_DATABASE.md)

---

## 💡 Lưu ý quan trọng

1. ✅ **Database LOCAL đã có sẵn** - File `Server/growingtales.db`
2. ✅ **Khi deploy Azure** - Database TỰ ĐỘNG tạo khi app chạy lần đầu
3. ✅ **Migration TỰ ĐỘNG** - Code tự chạy migrations, không cần lệnh thủ công
4. ✅ **Không mất data** - Dữ liệu lưu trong Azure File Share, bền vững
5. ✅ **Miễn phí hoàn toàn** - Nếu dùng Free tier F1

### Bạn KHÔNG CẦN:
- ❌ Tạo database thủ công
- ❌ Chạy `dotnet ef database update` trên Azure
- ❌ Vào Kudu Console để setup database
- ❌ Lo lắng về migrations

### Bạn CHỈ CẦN:
- ✅ Deploy code
- ✅ Mount File Share (Phần 6)
- ✅ Đợi app khởi động
- ✅ Database tự động sẵn sàng!

---

**Bắt đầu thôi! Theo từng bước, rất dễ! 💪**
