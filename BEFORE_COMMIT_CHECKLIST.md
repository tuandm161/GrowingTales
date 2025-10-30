# ✅ Checklist trước khi commit lên Git

## 🔐 Bảo mật (QUAN TRỌNG!)

- [ ] **Đã xóa API keys khỏi code**
  - [ ] Check `Server/appsettings.json` - Phải dùng placeholder
  - [ ] Check `Server/appsettings.Development.json` - Không commit file này
  - [ ] Check all `.cs` files - Không hard-code API key
  
- [ ] **File `.gitignore` đã được tạo**
  - [ ] `appsettings.json` trong `.gitignore`
  - [ ] `node_modules/` trong `.gitignore`
  - [ ] `bin/`, `obj/` trong `.gitignore`

- [ ] **Template files đã tạo**
  - [ ] `appsettings.json.example` có sẵn
  - [ ] README có hướng dẫn setup

---

## 🗑️ Files CẦN XÓA trước khi commit

### Backend
```bash
# Xóa build outputs
rm -rf Server/bin/
rm -rf Server/obj/

# Xóa user files
rm -rf Server/.vs/
rm -rf .vs/
```

### Frontend (Angular - nếu không dùng)
```bash
# Nếu chỉ dùng React, xóa Angular:
rm -rf ClientApp/src/
rm -rf ClientApp/node_modules/
rm ClientApp/angular.json
rm ClientApp/package.json
rm ClientApp/package-lock.json
rm ClientApp/tsconfig*.json
```

### Frontend (React)
```bash
# Xóa node_modules
rm -rf ClientApp/react-app/node_modules/

# Xóa build output
rm -rf ClientApp/react-app/dist/
```

---

## 📝 Files NÊN COMMIT

### ✅ Backend
- ✅ `Server/*.cs` (all source files)
- ✅ `Server/Server.csproj`
- ✅ `Server/Program.cs`
- ✅ `Server/appsettings.json.example` (template)
- ✅ `Server/Controllers/`
- ✅ `Server/Services/`
- ✅ `Server/Models/`
- ❌ `Server/appsettings.json` (có API key - đã ignore)
- ❌ `Server/bin/`, `Server/obj/` (đã ignore)

### ✅ Frontend
- ✅ `ClientApp/react-app/src/` (all source)
- ✅ `ClientApp/react-app/public/`
- ✅ `ClientApp/react-app/package.json`
- ✅ `ClientApp/react-app/vite.config.ts`
- ✅ `ClientApp/react-app/tsconfig.json`
- ✅ `ClientApp/react-app/README.md`
- ❌ `ClientApp/react-app/node_modules/` (đã ignore)
- ❌ `ClientApp/react-app/dist/` (đã ignore)

### ✅ Root
- ✅ `.gitignore`
- ✅ `README.md`
- ✅ `SETUP.md`
- ✅ `BEFORE_COMMIT_CHECKLIST.md` (file này)
- ✅ `ClientApp/MIGRATION_GUIDE.md` (nếu muốn giữ)

---

## 🔍 Kiểm tra lại

### 1. Scan toàn bộ code tìm sensitive data

```bash
# Tìm API keys
grep -r "AIzaSy" . --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj

# Tìm passwords
grep -r "password" . --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj

# Tìm secrets
grep -r "secret" . --exclude-dir=node_modules --exclude-dir=bin --exclude-dir=obj
```

### 2. Test .gitignore đang hoạt động

```bash
# Check những file sẽ được commit
git status

# Đảm bảo KHÔNG thấy:
# - Server/bin/
# - Server/obj/
# - Server/appsettings.json
# - ClientApp/react-app/node_modules/
# - ClientApp/react-app/dist/
```

### 3. Verify appsettings.json

```bash
cat Server/appsettings.json.example
```

**Phải thấy:**
```json
{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
  }
}
```

**KHÔNG được có API key thật!**

---

## 📦 Commands để clean project

### Option 1: Clean từng thứ
```bash
# Backend
cd Server
dotnet clean
rm -rf bin/ obj/ .vs/

# Frontend React
cd ../ClientApp/react-app
rm -rf node_modules/ dist/

# Frontend Angular (nếu xóa)
cd ../
rm -rf node_modules/ dist/ .angular/
```

### Option 2: Script tự động

**PowerShell (Windows):**
```powershell
# Clean backend
Remove-Item -Recurse -Force Server/bin, Server/obj, Server/.vs -ErrorAction SilentlyContinue

# Clean frontend
Remove-Item -Recurse -Force ClientApp/react-app/node_modules, ClientApp/react-app/dist -ErrorAction SilentlyContinue

# Clean Angular (nếu không dùng)
Remove-Item -Recurse -Force ClientApp/node_modules, ClientApp/dist, ClientApp/.angular -ErrorAction SilentlyContinue

Write-Host "✅ Cleaned!" -ForegroundColor Green
```

**Bash (Linux/Mac):**
```bash
#!/bin/bash

# Clean backend
rm -rf Server/bin Server/obj Server/.vs

# Clean frontend
rm -rf ClientApp/react-app/node_modules ClientApp/react-app/dist

# Clean Angular (nếu không dùng)
rm -rf ClientApp/node_modules ClientApp/dist ClientApp/.angular

echo "✅ Cleaned!"
```

---

## 🚀 Git Commands

### First time setup

```bash
# 1. Init git (nếu chưa có)
git init

# 2. Add all files
git add .

# 3. Check status
git status

# 4. Commit
git commit -m "Initial commit: GrowingTales - AI Story Generator"

# 5. Add remote
git remote add origin https://github.com/username/GrowingTales.git

# 6. Push
git push -u origin main
```

### Update sau này

```bash
git add .
git commit -m "Update: description"
git push
```

---

## ⚠️ LƯU Ý CUỐI CÙNG

### ❌ TUYỆT ĐỐI KHÔNG commit:

1. **API Keys** - Gemini API key
2. **Passwords** - Bất kỳ password nào
3. **Connection Strings** - Database credentials
4. **User Secrets** - Bất kỳ thông tin cá nhân
5. **node_modules/** - Thư viện frontend
6. **bin/, obj/** - Build outputs
7. **.vs/, .vscode/** - IDE settings

### ✅ NÊN commit:

1. **Source code** - `.cs`, `.tsx`, `.ts` files
2. **Config templates** - `.example` files
3. **Documentation** - README, SETUP guides
4. **Package configs** - `package.json`, `.csproj`
5. **.gitignore** - File này rất quan trọng!

---

## 📋 Final Check

Trước khi `git push`, chạy:

```bash
# 1. Check git status
git status

# 2. Không được thấy:
# - appsettings.json
# - bin/
# - obj/
# - node_modules/

# 3. Search API keys trong staged files
git diff --cached | grep "AIzaSy"

# 4. Nếu tìm thấy → ABORT và xóa ngay!
git reset HEAD
```

---

## 🎉 Done!

Nếu tất cả checklist đều ✅ → An toàn để push lên Git!

```bash
git push origin main
```

**Chúc mừng! Project của bạn đã sẵn sàng chia sẻ! 🚀**

