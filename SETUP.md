# 🚀 Setup Guide - GrowingTales

## 📋 Prerequisites

- **.NET 10.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** và **npm** - [Download](https://nodejs.org/)
- **Gemini API Key** (free) - [Get here](https://aistudio.google.com/app/apikey)

---

## ⚙️ Configuration

### 1. Backend Configuration

**Bước 1: Copy file config mẫu**
```bash
cd Server
cp appsettings.json.example appsettings.json
```

**Bước 2: Thêm Gemini API Key**

Mở `Server/appsettings.json` và thay `YOUR_GEMINI_API_KEY_HERE` bằng API key thật:

```json
{
  "Gemini": {
    "ApiKey": "AIzaSy..."
  }
}
```

**⚠️ LƯU Ý:** File `appsettings.json` đã được thêm vào `.gitignore` để không commit API key lên Git!

---

## 🏃 Running the Application

### Option 1: Chạy cả 2 (Backend + Frontend)

**Terminal 1 - Backend:**
```bash
cd Server
dotnet run
```
→ Backend chạy tại: `http://localhost:5002`

**Terminal 2 - Frontend (React):**
```bash
cd ClientApp/react-app
npm install    # Chỉ lần đầu
npm run dev
```
→ Frontend chạy tại: `http://localhost:5173`

### Option 2: Chỉ chạy Backend (API only)

```bash
cd Server
dotnet run
```

Test API: `http://localhost:5002/api/story`

---

## 📁 Project Structure

```
GrowingTalesTest/
│
├── Server/                          # .NET Backend
│   ├── Controllers/
│   │   └── StoryController.cs      # API endpoints
│   ├── Services/
│   │   ├── GeminiService.cs        # Gemini AI integration
│   │   └── StoryStorageService.cs  # In-memory storage
│   ├── Models/
│   │   ├── Story.cs
│   │   └── StoryRequest.cs
│   ├── appsettings.json            # Config (gitignored)
│   └── appsettings.json.example    # Template
│
└── ClientApp/
    ├── react-app/                   # React Frontend (NEW)
    │   ├── src/
    │   │   ├── pages/               # Main components
    │   │   ├── services/            # API services
    │   │   └── models/              # TypeScript types
    │   └── package.json
    │
    └── [angular files]              # Angular (OLD, có thể xóa)
```

---

## 🔑 Getting Gemini API Key

1. Truy cập: https://aistudio.google.com/app/apikey
2. Đăng nhập với Google account
3. Click **"Create API Key"**
4. Copy key và paste vào `Server/appsettings.json`

**Free tier:**
- 15 requests/minute
- 1500 requests/day
- Đủ cho development!

---

## 🧪 Testing

### Test Backend API

```bash
# Health check
curl http://localhost:5002/api/weatherforecast

# Get all stories
curl http://localhost:5002/api/story

# Create story (PowerShell)
$body = @{
    inputText = "Một câu chuyện về con thỏ"
    childName = "Minh"
    childAge = 5
    theme = "Động vật"
    pageCount = 3
    language = "vi"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5002/api/story/generate-from-text" -Method POST -Body $body -ContentType "application/json"
```

---

## 🐛 Troubleshooting

### Backend không chạy

**Lỗi:** "Gemini API key chưa được cấu hình"
- **Giải pháp:** Check `Server/appsettings.json` có đúng API key không

**Lỗi:** Port 5002 đã được dùng
- **Giải pháp:** 
  ```bash
  # Windows
  netstat -ano | findstr :5002
  taskkill /PID <PID> /F
  ```

### Frontend không chạy

**Lỗi:** "Cannot find module"
- **Giải pháp:** 
  ```bash
  cd ClientApp/react-app
  rm -rf node_modules package-lock.json
  npm install
  ```

**Lỗi:** CORS
- **Giải pháp:** Đảm bảo backend đang chạy và CORS đã enable

---

## 📦 Production Build

### Backend
```bash
cd Server
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd ClientApp/react-app
npm run build
```
Output: `ClientApp/react-app/dist/`

---

## 🔐 Security Notes

**⚠️ QUAN TRỌNG:**

1. **KHÔNG commit API key** vào Git
2. **KHÔNG share** `appsettings.json` file
3. **SỬ DỤNG** environment variables cho production
4. **THÊM** `appsettings.json` vào `.gitignore` (đã có)

**Production setup:**
```bash
# Set environment variable
export Gemini__ApiKey="your-api-key"

# Hoặc dùng Azure Key Vault / AWS Secrets Manager
```

---

## 📚 API Documentation

### Endpoints

#### Story Management

**POST** `/api/story/generate-from-text`
- Tạo truyện từ văn bản
- Body: `StoryRequest`

**POST** `/api/story/generate-from-audio`
- Tạo truyện từ audio
- Body: `AudioStoryRequest`

**GET** `/api/story`
- Lấy tất cả truyện

**GET** `/api/story/{id}`
- Lấy truyện theo ID

**GET** `/api/story/by-child/{childName}`
- Lấy truyện theo tên bé

**DELETE** `/api/story/{id}`
- Xóa truyện

---

## 🤝 Contributing

1. Fork project
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 📄 License

MIT License

---

## 🆘 Support

Có vấn đề? Tạo issue tại: [GitHub Issues](your-repo-url/issues)

---

Made with ❤️ for children's memories

