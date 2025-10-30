# ============================================
# Clean Project for Git Commit
# ============================================

Write-Host "`n🧹 Cleaning GrowingTales Project for Git..." -ForegroundColor Cyan

# Backend Cleanup
Write-Host "`n📦 Cleaning Backend..." -ForegroundColor Yellow
Remove-Item -Recurse -Force Server/bin -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force Server/obj -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force Server/.vs -ErrorAction SilentlyContinue
Write-Host "  ✅ Backend cleaned" -ForegroundColor Green

# Frontend React Cleanup
Write-Host "`n⚛️  Cleaning React Frontend..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ClientApp/react-app/node_modules -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force ClientApp/react-app/dist -ErrorAction SilentlyContinue
Write-Host "  ✅ React cleaned" -ForegroundColor Green

# Frontend Angular Cleanup (optional)
if (Test-Path "ClientApp/src") {
    Write-Host "`n🅰️  Cleaning Angular Frontend (if exists)..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force ClientApp/node_modules -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force ClientApp/dist -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force ClientApp/.angular -ErrorAction SilentlyContinue
    Write-Host "  ✅ Angular cleaned" -ForegroundColor Green
}

# VS/VSCode Cleanup
Write-Host "`n🔧 Cleaning IDE files..." -ForegroundColor Yellow
Remove-Item -Recurse -Force .vs -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .vscode -ErrorAction SilentlyContinue
Write-Host "  ✅ IDE files cleaned" -ForegroundColor Green

Write-Host "`n✨ Project cleaned successfully!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "  1. Review BEFORE_COMMIT_CHECKLIST.md" -ForegroundColor White
Write-Host "  2. Check git status: git status" -ForegroundColor White
Write-Host "  3. Add files: git add ." -ForegroundColor White
Write-Host "  4. Commit: git commit -m 'your message'" -ForegroundColor White
Write-Host "  5. Push: git push" -ForegroundColor White
Write-Host ""

