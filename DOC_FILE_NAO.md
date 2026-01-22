# 📚 Đọc file nào? - Hướng dẫn chọn tài liệu phù hợp

---

## 🎯 Dựa vào câu hỏi của bạn

### ❓ "Database đã tạo chưa? Liên kết chưa?"
→ Đọc: **[TOM_TAT_DATABASE.md](TOM_TAT_DATABASE.md)** ⭐⭐⭐

**TL;DR:** Database đã OK, tự động tạo khi deploy, không lo!

---

### ❓ "Làm sao deploy lên Azure? Tôi mới lần đầu"
→ Đọc: **[BAT_DAU_TU_DAY.md](BAT_DAU_TU_DAY.md)** ⭐⭐⭐

**TL;DR:** 8 bước đơn giản, có script tự động, 30-45 phút xong.

---

### ❓ "Tôi cần checklist để đánh dấu tiến độ"
→ Đọc: **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** ⭐⭐

**TL;DR:** Tick từng ô khi hoàn thành, không bỏ sót.

---

### ❓ "Làm sao quản lý database?"
→ Đọc: **[DATABASE_SETUP.md](DATABASE_SETUP.md)** ⭐⭐

**TL;DR:** Migrations, backup, restore, xem data.

---

### ❓ "Làm sao verify database OK?"
→ Đọc: **[VERIFY_DATABASE.md](VERIFY_DATABASE.md)** ⭐

**TL;DR:** Chạy `.\check-database.ps1`

---

### ❓ "Database trên Azure hoạt động như thế nào?"
→ Đọc: **[DATABASE_AZURE.md](DATABASE_AZURE.md)** ⭐

**TL;DR:** Tự động tạo, lưu trong File Share, không mất data.

---

### ❓ "Hướng dẫn deploy chi tiết, đầy đủ nhất"
→ Đọc: **[AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md)** ⭐⭐⭐

**TL;DR:** 432 dòng, mọi chi tiết về Azure.

---

### ❓ "Tôi muốn dùng Azure SQL thay vì SQLite"
→ Đọc: **[AZURE_SQL_SETUP.md](AZURE_SQL_SETUP.md)** ⭐

**TL;DR:** Tạo SQL Server, configure, migration.

---

## 🚀 Đường đi nhanh nhất (Khuyến khích)

Bạn có **30-45 phút**? Làm theo thứ tự:

### Bước 1: Kiểm tra (2 phút)
```powershell
.\check-database.ps1
```
→ Đảm bảo database OK

### Bước 2: Đọc hướng dẫn (5 phút)
Đọc: **[BAT_DAU_TU_DAY.md](BAT_DAU_TU_DAY.md)**
→ Hiểu toàn bộ flow

### Bước 3: Cài Azure CLI (5 phút)
```powershell
winget install -e --id Microsoft.AzureCLI
az login
```

### Bước 4: Lấy API keys (10 phút)
- Gemini: https://aistudio.google.com/apikey
- WhomeAI: https://whome.so
- Lưu vào Notepad

### Bước 5: Setup Azure (5 phút)
```powershell
.\setup-azure-resources.ps1
```

### Bước 6: Cấu hình (5 phút)
- Vào Azure Portal
- App Service → Configuration
- Thêm API keys
- Mount File Share

### Bước 7: Deploy (5 phút)
```powershell
.\deploy-to-azure.ps1
```

### Bước 8: Test (5 phút)
- Mở website
- Đăng ký, tạo truyện
- ✅ THÀNH CÔNG!

---

## 📖 Bảng so sánh tài liệu

| File | Độ dài | Độ khó | Dành cho | Ưu tiên |
|------|--------|--------|----------|---------|
| **TOM_TAT_DATABASE.md** | Ngắn | ⭐ Dễ | Lo database | ⭐⭐⭐ |
| **BAT_DAU_TU_DAY.md** | Trung | ⭐ Dễ | Bắt đầu deploy | ⭐⭐⭐ |
| **DATABASE_TRUYEN_MIENG.md** | Ngắn | ⭐ Dễ | Giải thích database | ⭐⭐ |
| **DEPLOYMENT_CHECKLIST.md** | Trung | ⭐ Dễ | Theo dõi tiến độ | ⭐⭐ |
| **VERIFY_DATABASE.md** | Trung | ⭐⭐ Trung | Verify database | ⭐ |
| **DATABASE_SETUP.md** | Dài | ⭐⭐ Trung | Quản lý DB | ⭐ |
| **DATABASE_AZURE.md** | Trung | ⭐⭐ Trung | DB trên Azure | ⭐ |
| **AZURE_DEPLOYMENT_GUIDE.md** | Dài | ⭐⭐⭐ Khó | Chi tiết đầy đủ | ⭐ |
| **DEPLOY_QUICKSTART.md** | Trung | ⭐⭐ Trung | Tham khảo nhanh | ⭐ |
| **AZURE_SQL_SETUP.md** | Dài | ⭐⭐⭐ Khó | Nâng cao | ☆ |

---

## 🎯 Khuyến nghị

### Nếu bạn mới 100%:
1. **[TOM_TAT_DATABASE.md](TOM_TAT_DATABASE.md)** - Đọc trước để hiểu database
2. **[BAT_DAU_TU_DAY.md](BAT_DAU_TU_DAY.md)** - Theo từng bước deploy
3. **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** - Tick dần khi làm

### Nếu bạn đã biết cơ bản:
1. **[DEPLOY_QUICKSTART.md](DEPLOY_QUICKSTART.md)** - Xem nhanh các lệnh
2. **[AZURE_DEPLOYMENT_GUIDE.md](AZURE_DEPLOYMENT_GUIDE.md)** - Tham khảo chi tiết

### Nếu bạn gặp lỗi:
1. Xem logs: `az webapp log tail ...`
2. Đọc phần Troubleshooting trong **[BAT_DAU_TU_DAY.md](BAT_DAU_TU_DAY.md)**
3. Check **[DATABASE_AZURE.md](DATABASE_AZURE.md)** nếu lỗi database

---

## 🛠️ Scripts có sẵn

| Script | Công dụng | Khi nào dùng |
|--------|-----------|--------------|
| `check-database.ps1` | Kiểm tra DB | Trước khi deploy |
| `test-before-deploy.ps1` | Test toàn bộ | Trước khi deploy |
| `setup-azure-resources.ps1` | Tạo resources | 1 lần đầu tiên |
| `deploy-to-azure.ps1` | Deploy code | Mỗi khi update |

---

## 💡 Mẹo

- **Bắt đầu:** Đọc file ngắn trước (TOM_TAT, BAT_DAU)
- **Gặp lỗi:** Đọc file chi tiết (AZURE_DEPLOYMENT_GUIDE)
- **Muốn hiểu sâu:** Đọc hết tất cả

---

**Chúc bạn thành công! Bắt đầu từ [TOM_TAT_DATABASE.md](TOM_TAT_DATABASE.md) nhé! 📖**
