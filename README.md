# 📖 GrowingTales - Tạo truyện cho bé với AI

Ứng dụng web ASP.NET Core MVC giúp phụ huynh tạo truyện tranh được cá nhân hóa cho con yêu bằng công nghệ AI.

## ✨ Tính năng

### 🎯 Tạo truyện
- ✍️ Nhập văn bản hoặc 🎤 ghi âm giọng nói
- 🤖 AI tự động tạo nội dung phù hợp với độ tuổi
- 🎨 Tạo hình ảnh minh họa tự động
- 🌈 Nhiều chủ đề: Dũng cảm, Tình bạn, Phiêu lưu, Khoa học...

### 📚 Quản lý truyện
- 💾 Lưu trữ và quản lý thư viện truyện
- ⭐ Đánh dấu yêu thích
- 🔍 Tìm kiếm và lọc theo chủ đề, tên bé
- ✏️ Chỉnh sửa thông tin truyện
- 🗑️ Xóa truyện

### 🎭 Tính năng nâng cao
- 🔊 Text-to-Speech - Đọc truyện tự động
- 🔗 Chia sẻ truyện qua link
- 📄 Xuất PDF (sắp có)
- 🖼️ Tạo lại hình ảnh nếu không hài lòng

### 💎 Gói đăng ký
- 🆓 **Free**: 3 truyện miễn phí, tối đa 5 trang/truyện
- ⭐ **Premium** (150.000đ/tháng): Không giới hạn, 15 trang/truyện, không watermark

## 🛠️ Công nghệ sử dụng

### Backend
- ASP.NET Core 8.0 MVC
- Entity Framework Core
- SQLite / Azure SQL Database
- Cookie Authentication

### AI Services
- Google Gemini API - Tạo nội dung truyện
- WhomeAI - Tạo hình ảnh

### Payment Gateway
- VNPay - Thanh toán online

### Frontend
- Razor Views
- Bootstrap 5
- Bootstrap Icons
- Web Speech API (Text-to-Speech)

## 📦 Cấu trúc Database

### Bảng chính
- **Users** - Người dùng (Role, Status, Subscription)
- **Stories** - Truyện (Title, Theme, ViewCount)
- **StoryPages** - Trang truyện (Content, Image)
- **Subscriptions** - Gói đăng ký
- **Payments** - Giao dịch thanh toán (theo dõi doanh thu)
- **ActivityLogs** - Nhật ký hoạt động
- **Notifications** - Thông báo người dùng
- **SystemSettings** - Cấu hình hệ thống
- **Contacts** - Liên hệ/phản hồi

## 🚀 Cài đặt Local

### Yêu cầu
- .NET 8.0 SDK trở lên
- Visual Studio 2022 hoặc VS Code

### Các bước

1. **Clone repository**
```powershell
git clone <repo-url>
cd GrowingTales
```

2. **Cấu hình API Keys**

Tạo file `Server/appsettings.Development.json`:
```json
{
  "GeminiAPI": {
    "ApiKey": "YOUR_GEMINI_API_KEY"
  },
  "WhomeAI": {
    "ApiKey": "YOUR_WHOME_API_KEY"
  },
  "VnPay": {
    "TmnCode": "YOUR_VNPAY_CODE",
    "HashSecret": "YOUR_VNPAY_SECRET"
  }
}
```

3. **Chạy ứng dụng**
```powershell
cd Server
dotnet restore
dotnet run
```

4. **Truy cập**
- URL: http://localhost:5002

## 🌐 Deploy lên Azure

### Cách nhanh nhất (Khuyến khích)

```powershell
# 1. Setup Azure resources
.\setup-azure-resources.ps1

# 2. Cấu hình API keys trong Azure Portal

# 3. Deploy
.\deploy-to-azure.ps1
```

### Hướng dẫn chi tiết
Xem file: **[AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md)**

Hoặc hướng dẫn nhanh: **[DEPLOY_QUICKSTART.md](DEPLOY_QUICKSTART.md)**

## 📊 Admin Features (Sắp có)

- 📈 Dashboard thống kê doanh thu
- 👥 Quản lý người dùng
- 💰 Báo cáo thanh toán
- 📧 Xem liên hệ/phản hồi
- ⚙️ Cấu hình hệ thống

## 🔐 API Keys cần thiết

### 1. Google Gemini API
- Đăng ký: https://makersuite.google.com/app/apikey
- Free: 60 requests/minute
- Dùng model: `gemini-2.0-flash-exp`

### 2. WhomeAI
- Đăng ký: https://whome.so
- Dùng cho tạo hình ảnh
- Model: `flux-dev`

### 3. VNPay (Tuỳ chọn)
- Đăng ký doanh nghiệp: https://vnpay.vn
- Sandbox mode có sẵn trong config để test

## 📝 License

Private project - All rights reserved

## 👨‍💻 Phát triển bởi

GrowingTales Team

---

**Made with ❤️ for Vietnamese children**
