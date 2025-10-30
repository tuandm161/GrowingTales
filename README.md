# 📖 GrowingTales - AI-Powered Children's Storybook Generator

**GrowingTales** là ứng dụng tạo câu chuyện kỷ niệm cho trẻ em bằng AI. Chuyển giọng nói hoặc văn bản thành những câu chuyện đẹp, ý nghĩa với sự hỗ trợ của Google Gemini AI.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![React](https://img.shields.io/badge/React-18-blue)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-blue)

---

## ✨ Tính năng

### 🎤 Hai cách nhập liệu
- **📝 Nhập văn bản**: Viết ý tưởng câu chuyện
- **🎙️ Ghi âm**: Kể câu chuyện bằng giọng nói của bạn

### 🤖 AI-Powered
- **Google Gemini 2.5 Pro** để:
  - Chuyển giọng nói thành văn bản
  - Tạo câu chuyện từ ý tưởng
  - Tạo mô tả hình ảnh cho từng trang

### 📚 Quản lý truyện
- Xem danh sách tất cả câu chuyện đã tạo
- Đọc truyện với giao diện đẹp như sách thật
- In truyện để lưu giữ
- Xóa truyện không cần thiết

### 🎨 Giao diện thân thiện
- Responsive, hoạt động tốt trên mobile
- Màu sắc sinh động, phù hợp với trẻ em
- Animation mượt mà
- Modern UI/UX

---

## 🚀 Quick Start

### Prerequisites
- **.NET 10.0 SDK**
- **Node.js 18+**
- **Gemini API Key** (free)

### Setup (3 phút)

```bash
# 1. Clone repository
git clone <your-repo-url>
cd GrowingTalesTest

# 2. Backend setup
cd Server
cp appsettings.json.example appsettings.json
# Thêm Gemini API key vào appsettings.json

# 3. Frontend setup
cd ../ClientApp/react-app
npm install

# 4. Run!
# Terminal 1 - Backend
cd Server
dotnet run

# Terminal 2 - Frontend
cd ClientApp/react-app
npm run dev
```

**Xem chi tiết:** [SETUP.md](SETUP.md)

---

## 📁 Project Structure

```
GrowingTalesTest/
├── Server/                      # .NET 10 Backend
│   ├── Controllers/            # API endpoints
│   ├── Services/
│   │   ├── GeminiService.cs   # Gemini AI integration
│   │   └── StoryStorageService.cs
│   ├── Models/
│   └── Program.cs
│
└── ClientApp/
    └── react-app/              # React 18 + Vite Frontend
        ├── src/
        │   ├── pages/          # Main components
        │   │   ├── CreateStory.tsx
        │   │   ├── StoryList.tsx
        │   │   └── StoryViewer.tsx
        │   ├── services/       # API integration
        │   └── models/         # TypeScript types
        └── package.json
```

---

## 🛠️ Tech Stack

### Backend
- **.NET 10.0** - Modern C# framework
- **ASP.NET Core Web API** - RESTful API
- **Google Gemini 2.5 Pro** - AI text generation
- **In-memory storage** - Simple data storage

### Frontend
- **React 18** - UI library
- **TypeScript 5.9** - Type safety
- **Vite 7** - Lightning-fast build tool
- **React Router** - Client-side routing
- **Axios** - HTTP client

---

## 🎯 Features

| Feature | Description | Status |
|---------|-------------|--------|
| Text to Story | Tạo truyện từ văn bản | ✅ |
| Voice to Story | Tạo truyện từ giọng nói | ✅ |
| Story List | Danh sách truyện | ✅ |
| Story Viewer | Xem truyện page by page | ✅ |
| Print Story | In truyện ra giấy | ✅ |
| Delete Story | Xóa truyện | ✅ |
| Responsive UI | Mobile-friendly | ✅ |
| Image Generation | AI tạo ảnh | 🚧 (Placeholder) |

---

## 📸 Screenshots

### Create Story
![Create Story](docs/screenshots/create-story.png)

### Story Viewer
![Story Viewer](docs/screenshots/story-viewer.png)

### Story List
![Story List](docs/screenshots/story-list.png)

---

## 🔑 Getting Gemini API Key

1. Truy cập: [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Đăng nhập với Google account
3. Click "Create API Key"
4. Copy key và thêm vào `Server/appsettings.json`

**Free tier:**
- 15 requests/minute
- 1500 requests/day
- Đủ cho development!

---

## 🧪 API Endpoints

### Story Management

```
POST   /api/story/generate-from-text      # Tạo truyện từ văn bản
POST   /api/story/generate-from-audio     # Tạo truyện từ audio
GET    /api/story                         # Lấy tất cả truyện
GET    /api/story/{id}                    # Lấy 1 truyện
GET    /api/story/by-child/{childName}    # Lấy truyện theo tên bé
DELETE /api/story/{id}                    # Xóa truyện
```

**Example Request:**
```bash
POST http://localhost:5002/api/story/generate-from-text
Content-Type: application/json

{
  "inputText": "Một câu chuyện về con thỏ dũng cảm",
  "childName": "Minh",
  "childAge": 5,
  "theme": "Phiêu lưu",
  "pageCount": 5,
  "language": "vi"
}
```

---

## 🚀 Deployment

### Backend (Azure App Service)
```bash
cd Server
dotnet publish -c Release -o ./publish
# Deploy to Azure
```

### Frontend (Vercel/Netlify)
```bash
cd ClientApp/react-app
npm run build
# Deploy dist/ folder
```

---

## 🤝 Contributing

Contributions are welcome! 

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

**Before committing:** Read [BEFORE_COMMIT_CHECKLIST.md](BEFORE_COMMIT_CHECKLIST.md)

---

## 📝 Documentation

- [Setup Guide](SETUP.md) - Hướng dẫn cài đặt chi tiết
- [Migration Guide](ClientApp/MIGRATION_GUIDE.md) - Angular to React migration
- [Commit Checklist](BEFORE_COMMIT_CHECKLIST.md) - Trước khi commit lên Git

---

## 🐛 Known Issues

- [ ] Images are placeholders (Unsplash integration removed)
- [ ] Stories stored in memory (lost on restart)
- [ ] No user authentication
- [ ] No database

---

## 🎯 Roadmap

- [ ] **AI Image Generation** - Integrate real image generation API
- [ ] **Database** - PostgreSQL or MongoDB
- [ ] **User Authentication** - Login/Register
- [ ] **Export PDF** - Download story as PDF
- [ ] **Share Stories** - Share via link
- [ ] **Multi-language** - Support English, Vietnamese
- [ ] **Voice Synthesis** - AI reads story aloud
- [ ] **Cloud Storage** - Save images to cloud

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

---

## 🙏 Acknowledgments

- **Google Gemini AI** - Powerful text generation
- **Lorem Picsum** - Placeholder images
- **React Community** - Amazing ecosystem
- **.NET Community** - Excellent documentation

---

## ⭐ Show your support

Give a ⭐️ if this project helped you!

---

Made with ❤️ for children's memories

**Status:** 🟢 Active Development
