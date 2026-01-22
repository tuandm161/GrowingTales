# Hướng Dẫn Cấu Hình Email để Gửi Email Đặt Lại Mật Khẩu

## Tổng Quan

Để gửi email đặt lại mật khẩu, bạn cần cấu hình SMTP server. Có nhiều lựa chọn:

1. **Gmail** (Dễ nhất, miễn phí)
2. **SendGrid** (Cloud service, 100 emails/ngày miễn phí)
3. **Mailgun** (Cloud service)
4. **SMTP Server riêng**

---

## Cách 1: Sử dụng Gmail (Khuyến nghị cho Development)

### Bước 1: Tạo App Password cho Gmail

1. Đăng nhập vào tài khoản Google của bạn
2. Vào [Google Account Security](https://myaccount.google.com/security)
3. Bật **2-Step Verification** (nếu chưa bật)
4. Vào **App passwords** (Mật khẩu ứng dụng)
5. Chọn app: **Mail** và device: **Other (Custom name)**
6. Nhập tên: "GrowingTales"
7. Nhấn **Generate**
8. **Copy mật khẩu** (16 ký tự, không có khoảng trắng)

### Bước 2: Cấu hình trong appsettings.json

Mở `Server/appsettings.json` và cập nhật:

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-16-char-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "GrowingTales",
    "EnableSsl": true,
    "BaseUrl": "https://localhost:5001"
  },
  "AppSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}
```

**Lưu ý:**
- `SmtpUsername`: Email Gmail của bạn
- `SmtpPassword`: App Password (16 ký tự) vừa tạo, KHÔNG phải mật khẩu Gmail
- `BaseUrl`: URL của website (thay đổi khi deploy)

---

## Cách 2: Sử dụng SendGrid (Khuyến nghị cho Production)

### Bước 1: Tạo tài khoản SendGrid

1. Đăng ký tại [SendGrid](https://sendgrid.com/) (miễn phí 100 emails/ngày)
2. Xác thực email
3. Vào **Settings** → **API Keys**
4. Tạo API Key mới:
   - Name: "GrowingTales"
   - Permissions: **Full Access** hoặc chỉ **Mail Send**
5. **Copy API Key** (chỉ hiển thị 1 lần)

### Bước 2: Verify Sender Identity

1. Vào **Settings** → **Sender Authentication**
2. Chọn **Verify a Single Sender**
3. Điền thông tin và verify email

### Bước 3: Cấu hình trong appsettings.json

```json
{
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SmtpUsername": "apikey",
    "SmtpPassword": "your-sendgrid-api-key",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "GrowingTales",
    "EnableSsl": true,
    "BaseUrl": "https://yourdomain.com"
  }
}
```

---

## Cách 3: Sử dụng SMTP Server khác

### Cấu hình cho các provider phổ biến:

#### Outlook/Hotmail:
```json
{
  "Email": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@outlook.com",
    "SmtpPassword": "your-password",
    "EnableSsl": true
  }
}
```

#### Yahoo:
```json
{
  "Email": {
    "SmtpHost": "smtp.mail.yahoo.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@yahoo.com",
    "SmtpPassword": "your-app-password",
    "EnableSsl": true
  }
}
```

#### Zoho:
```json
{
  "Email": {
    "SmtpHost": "smtp.zoho.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@zoho.com",
    "SmtpPassword": "your-password",
    "EnableSsl": true
  }
}
```

---

## Cấu hình BaseUrl

`BaseUrl` là URL của website, dùng để tạo link reset password trong email.

### Development:
```json
{
  "AppSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}
```

### Production:
```json
{
  "AppSettings": {
    "BaseUrl": "https://yourdomain.com"
  }
}
```

Hoặc trên Azure:
```json
{
  "AppSettings": {
    "BaseUrl": "https://growingtales-app.azurewebsites.net"
  }
}
```

---

## Kiểm Tra Cấu Hình

Sau khi cấu hình, restart server và thử:

1. Vào `/Account/ForgotPassword`
2. Nhập email
3. Kiểm tra hộp thư (cả spam folder)

Nếu email không được gửi, kiểm tra:
- Logs trong console
- Cấu hình SMTP có đúng không
- Firewall có chặn port 587/465 không
- App Password (Gmail) có đúng không

---

## Bảo Mật

⚠️ **QUAN TRỌNG:**

1. **KHÔNG commit** `appsettings.json` có chứa mật khẩu thật vào Git
2. Sử dụng `appsettings.Development.json` cho local
3. Sử dụng **Environment Variables** hoặc **Azure App Settings** cho production
4. Sử dụng **App Password** thay vì mật khẩu chính

### Sử dụng Environment Variables:

```powershell
# Windows
$env:Email__SmtpPassword="your-password"

# Linux/Mac
export Email__SmtpPassword="your-password"
```

### Hoặc trong Azure App Settings:
```
Email__SmtpPassword = your-password
```

---

## Troubleshooting

### Lỗi: "The SMTP server requires a secure connection"
- Đảm bảo `EnableSsl: true`
- Thử port 465 thay vì 587

### Lỗi: "Authentication failed"
- Kiểm tra lại username/password
- Với Gmail: đảm bảo dùng App Password, không phải mật khẩu chính
- Đảm bảo đã bật "Less secure app access" (nếu không dùng App Password)

### Email không đến
- Kiểm tra thư mục Spam
- Kiểm tra email có đúng không
- Xem logs để biết email có được gửi không

---

## Tài Liệu Tham Khảo

- [Gmail App Passwords](https://support.google.com/accounts/answer/185833)
- [SendGrid Documentation](https://docs.sendgrid.com/)
- [ASP.NET Core Email](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/email)
