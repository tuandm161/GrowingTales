# ✅ Checklist Deploy GrowingTales lên Azure

Dùng file này để theo dõi tiến độ deployment của bạn.

## Giai đoạn 1: Chuẩn bị (10-15 phút)

- [ ] Tạo tài khoản Azure (nếu chưa có)
- [ ] Nhận $200 credit miễn phí
- [ ] Cài đặt Azure CLI: `winget install -e --id Microsoft.AzureCLI`
- [ ] Đăng nhập Azure: `az login`
- [ ] Verify login thành công: `az account show`

## Giai đoạn 2: Lấy API Keys (15-20 phút)

### Gemini API
- [ ] Truy cập: https://makersuite.google.com/app/apikey
- [ ] Tạo API key mới
- [ ] Copy và lưu lại key: `AIzaSy...`
- [ ] Test key bằng cách tạo 1 truyện local

### WhomeAI API
- [ ] Đăng ký tài khoản: https://whome.so
- [ ] Lấy API key từ dashboard
- [ ] Copy và lưu lại key: `whm_...`
- [ ] Test key (optional)

### VNPay (Tuỳ chọn - có thể bỏ qua)
- [ ] Đăng ký tài khoản sandbox: https://sandbox.vnpayment.vn/
- [ ] Lấy TMN Code
- [ ] Lấy Hash Secret
- [ ] Hoặc dùng config mặc định để test

## Giai đoạn 3: Tạo Azure Resources (10-15 phút)

### Option A: Dùng Script tự động (Khuyến khích)
- [ ] Chạy: `.\setup-azure-resources.ps1`
- [ ] Đợi script chạy xong (khoảng 2-3 phút)
- [ ] Kiểm tra kết quả trong Azure Portal

### Option B: Tạo thủ công trong Azure Portal
- [ ] Tạo Resource Group: `growingtales-rg`
- [ ] Tạo App Service Plan: `growingtales-plan`
  - [ ] Chọn tier: `F1 Free` (test) hoặc `B1 Basic` (production)
  - [ ] Region: `Southeast Asia`
- [ ] Tạo Web App: `growingtales-app`
  - [ ] Runtime: `.NET 8`
  - [ ] OS: `Linux` (khuyến khích)
- [ ] Tạo Storage Account (cho SQLite): `growingtalesstorage`
- [ ] Tạo File Share: `growingtales-data` (5GB)

## Giai đoạn 4: Cấu hình Web App (5-10 phút)

- [ ] Vào Azure Portal → App Service → Configuration
- [ ] Tab "Application settings", thêm:
  - [ ] `ASPNETCORE_ENVIRONMENT` = `Production`
  - [ ] `GeminiAPI__ApiKey` = `<your-gemini-key>`
  - [ ] `WhomeAI__ApiKey` = `<your-whome-key>`
  - [ ] `VnPay__TmnCode` = `<your-code>` (nếu có)
  - [ ] `VnPay__HashSecret` = `<your-secret>` (nếu có)
  - [ ] `WEBSITE_TIME_ZONE` = `SE Asia Standard Time`
- [ ] Nhấn **Save** và **Continue** để restart

### Cấu hình Storage (cho SQLite)
- [ ] Vào Configuration → Path mappings
- [ ] Nhấn "+ New Azure Storage Mount"
- [ ] Điền:
  - [ ] Name: `database`
  - [ ] Storage account: `growingtalesstorage`
  - [ ] Share name: `growingtales-data`
  - [ ] Mount path: `/data`
- [ ] Save

### Cấu hình nâng cao (Optional)
- [ ] Tab "General settings":
  - [ ] Always On: `On` (cần tier B1+)
  - [ ] HTTP version: `2.0`
  - [ ] ARR affinity: `Off`
- [ ] Save

## Giai đoạn 5: Deploy Code (5-10 phút)

### Option A: Dùng Script (Nhanh nhất)
- [ ] Chạy: `.\deploy-to-azure.ps1`
- [ ] Đợi upload và deploy (2-5 phút)
- [ ] Kiểm tra thông báo thành công

### Option B: VS Code Extension
- [ ] Cài Extension: "Azure App Service"
- [ ] Đăng nhập Azure trong VS Code
- [ ] Chuột phải folder `Server` → Deploy to Web App
- [ ] Chọn `growingtales-app`
- [ ] Đợi deploy xong

### Option C: Azure CLI thủ công
```powershell
- [ ] cd Server
- [ ] dotnet publish -c Release -o ./publish
- [ ] Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force
- [ ] az webapp deployment source config-zip --resource-group growingtales-rg --name growingtales-app --src ./publish.zip
```

## Giai đoạn 6: Kiểm tra & Test (10-15 phút)

### Kiểm tra deploy
- [ ] Vào Azure Portal → App Service → Overview
- [ ] Status hiển thị "Running"
- [ ] Default domain: `https://growingtales-app.azurewebsites.net`

### Xem logs
- [ ] Vào App Service → Log stream
- [ ] Hoặc dùng CLI: `az webapp log tail --name growingtales-app --resource-group growingtales-rg`
- [ ] Kiểm tra không có lỗi nghiêm trọng

### Test ứng dụng
- [ ] Mở URL: `https://growingtales-app.azurewebsites.net`
- [ ] Trang chủ load thành công
- [ ] Test đăng ký tài khoản mới
- [ ] Test đăng nhập
- [ ] Test tạo truyện (text)
  - [ ] Nhập thông tin bé
  - [ ] Chọn chủ đề
  - [ ] Tạo truyện
  - [ ] Truyện hiển thị đúng
- [ ] Test tạo truyện (audio) - Optional
- [ ] Test xem truyện
  - [ ] Chuyển trang hoạt động
  - [ ] Text-to-Speech hoạt động
  - [ ] Nút yêu thích hoạt động
- [ ] Test chia sẻ truyện
  - [ ] Tạo link chia sẻ
  - [ ] Mở link trong tab ẩn danh
  - [ ] Truyện hiển thị đúng
- [ ] Test trang bảng giá
- [ ] Test nâng cấp Premium (nếu đã config VNPay)

### Kiểm tra performance
- [ ] Trang load < 3 giây
- [ ] Không có lỗi console
- [ ] Hình ảnh hiển thị đúng
- [ ] Mobile responsive

## Giai đoạn 7: Tối ưu hóa (Optional)

### Performance
- [ ] Enable Application Insights
- [ ] Kiểm tra performance metrics
- [ ] Optimize database queries nếu cần
- [ ] Enable caching nếu cần

### Security
- [ ] Đổi tất cả default passwords
- [ ] Enable HTTPS only
- [ ] Kiểm tra CORS settings
- [ ] Review authentication flow

### Monitoring
- [ ] Setup alerts trong Application Insights
- [ ] Configure email notifications
- [ ] Monitor error rates

## Giai đoạn 8: CI/CD (Optional - 15-20 phút)

- [ ] Push code lên GitHub
- [ ] Download Publish Profile từ Azure
- [ ] Thêm secret `AZURE_WEBAPP_PUBLISH_PROFILE` vào GitHub
- [ ] Verify workflow file: `.github/workflows/azure-webapps-dotnet-core.yml`
- [ ] Test auto-deploy bằng cách push code mới

## Giai đoạn 9: Custom Domain & SSL (Optional)

- [ ] Mua domain (vd: growingtales.vn)
- [ ] Vào App Service → Custom domains
- [ ] Add custom domain
- [ ] Cấu hình DNS records
- [ ] Validate domain
- [ ] Enable SSL certificate (Free managed certificate)
- [ ] Bind SSL với domain
- [ ] Test HTTPS

## Giai đoạn 10: Go Live! 🎉

- [ ] Thông báo cho users
- [ ] Share link trên social media
- [ ] Monitor logs trong 24h đầu
- [ ] Sẵn sàng xử lý issues
- [ ] Celebrate! 🎊

---

## 📞 Hỗ trợ

Nếu gặp vấn đề ở bất kỳ bước nào:

1. **Check logs**:
   ```powershell
   az webapp log tail --name growingtales-app --resource-group growingtales-rg
   ```

2. **Restart app**:
   ```powershell
   az webapp restart --name growingtales-app --resource-group growingtales-rg
   ```

3. **Xem Application Insights**:
   - Vào Azure Portal → Application Insights → Failures

4. **Kiểm tra docs**:
   - [AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md) - Chi tiết đầy đủ
   - [DEPLOY_QUICKSTART.md](DEPLOY_QUICKSTART.md) - Hướng dẫn nhanh

---

## 💰 Chi phí dự kiến

| Giai đoạn | Tier | Chi phí/tháng |
|-----------|------|---------------|
| **Test** | F1 Free | $0 |
| **Production nhỏ** | B1 Basic | ~$13 |
| **Production** | S1 Standard | ~$70 |

**Lưu ý**: Free tier có giới hạn 60 mins CPU/day và sleep sau 20 mins idle.

---

**Chúc bạn deploy thành công! 🚀**
