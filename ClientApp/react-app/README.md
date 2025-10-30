# 📖 GrowingTales - React Frontend

**GrowingTales** là ứng dụng tạo câu chuyện kỷ niệm cho trẻ em bằng AI. Chuyển giọng nói hoặc văn bản thành những câu chuyện đẹp, ý nghĩa với sự hỗ trợ của Gemini AI.

## ✨ Tính năng

### 🎤 Hai cách nhập liệu
- **Nhập văn bản**: Viết ý tưởng câu chuyện
- **Ghi âm**: Kể câu chuyện bằng giọng nói của bạn

### 🤖 AI-Powered
- Sử dụng **Google Gemini API** để:
  - Chuyển giọng nói thành văn bản
  - Tạo câu chuyện từ ý tưởng
  - Tạo hình ảnh minh họa cho từng trang

### 📚 Quản lý truyện
- Xem danh sách tất cả câu chuyện đã tạo
- Đọc truyện với giao diện đẹp như sách thật
- In truyện để lưu giữ
- Xóa truyện không cần thiết

## 🚀 Hướng dẫn cài đặt & chạy

### Yêu cầu
- **Node.js 18+** và **npm**
- Backend API đang chạy tại `http://localhost:5002`

### Bước 1: Cài đặt dependencies
```bash
npm install
```

### Bước 2: Chạy development server
```bash
npm run dev
```

Frontend sẽ chạy tại: **http://localhost:5173**

### Bước 3: Build cho production
```bash
npm run build
```

Output sẽ ở thư mục `dist/`

## 🛠️ Công nghệ sử dụng

- **React 18** - UI Library
- **TypeScript** - Type safety
- **Vite** - Build tool (siêu nhanh!)
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **CSS3** - Styling với gradients & animations

## 📁 Cấu trúc Project

```
src/
├── config/
│   └── environment.ts          # API configuration
├── models/
│   └── story.model.ts          # TypeScript interfaces
├── services/
│   └── story.service.ts        # API service
├── pages/
│   ├── CreateStory.tsx         # Tạo truyện
│   ├── CreateStory.css
│   ├── StoryList.tsx           # Danh sách truyện
│   ├── StoryList.css
│   ├── StoryViewer.tsx         # Xem truyện
│   └── StoryViewer.css
├── App.tsx                     # Main app with routing
├── App.css
├── main.tsx                    # Entry point
└── index.css
```

## 🎯 Cách sử dụng

### 1. Tạo truyện mới (/)
**Chọn phương thức nhập:**
- 📝 **Văn bản**: Nhập ý tưởng câu chuyện
- 🎤 **Ghi âm**: Click "Bắt đầu ghi âm" và kể câu chuyện

**Nhập thông tin:**
- Tên bé (bắt buộc)
- Tuổi
- Chủ đề (tùy chọn)
- Bối cảnh thêm (tùy chọn)
- Số trang (3-10 trang)

**Tạo truyện:**
- Click "Tạo Câu Chuyện"
- Đợi AI xử lý (15-30 giây)
- Tự động chuyển đến trang xem truyện

### 2. Xem danh sách truyện (/stories)
- Hiển thị tất cả truyện đã tạo
- Click vào card để đọc truyện
- Click 🗑️ để xóa truyện

### 3. Đọc truyện (/story/:id)
- Xem từng trang như đọc sách thật
- Dùng nút "← ➡" hoặc click vào dots để chuyển trang
- Click 🖨️ "In truyện" để in ra giấy

## ⚙️ Configuration

Cấu hình API URL trong `src/config/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5002/api'
};
```

## 🔧 Scripts

- `npm run dev` - Chạy development server
- `npm run build` - Build cho production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## 📝 So sánh với Angular version

### Ưu điểm của React version:
- ✅ **Đơn giản hơn**: Ít boilerplate code
- ✅ **Nhanh hơn**: Vite build cực nhanh
- ✅ **Phổ biến hơn**: Community lớn, nhiều tài liệu
- ✅ **Flexible hơn**: Ít opinionated, dễ customize

### Khác biệt:
| Feature | Angular | React |
|---------|---------|-------|
| State Management | Signals | useState hooks |
| Routing | Angular Router | React Router |
| HTTP Client | HttpClient | Axios |
| Two-way binding | [(ngModel)] | Controlled inputs |
| Styling | Component CSS | Component CSS |
| Build tool | Angular CLI | Vite |

## 🎨 Features nổi bật

### Responsive Design
- Tương thích với mobile, tablet, desktop
- Breakpoints tại 768px

### Smooth Animations
- Page transitions
- Hover effects
- Loading spinners
- Floating animations

### User Experience
- Loading states
- Error handling
- Success messages
- Confirmation dialogs

## 🚀 Deployment

### Vercel (Recommended)
```bash
npm run build
# Deploy dist/ folder to Vercel
```

### Netlify
```bash
npm run build
# Deploy dist/ folder to Netlify
```

### Docker
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build
CMD ["npm", "run", "preview"]
```

## 📄 License

Distributed under the MIT License.

---

Made with ❤️ for children's memories

**Converted from Angular to React** 🎉
