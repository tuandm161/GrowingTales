# Script bật logging cho Azure App Service

param(
    [string]$AppName = "growingtales-app",
    [string]$ResourceGroup = "growingtales-rg"
)

Write-Host "🔧 Bật logging cho Azure App Service..." -ForegroundColor Cyan
Write-Host ""

# Kiểm tra Azure CLI
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure CLI chưa được cài đặt!" -ForegroundColor Red
    Write-Host "   Cài đặt: winget install -e --id Microsoft.AzureCLI" -ForegroundColor Yellow
    exit 1
}

# Kiểm tra đã login chưa
Write-Host "🔍 Kiểm tra Azure login..." -ForegroundColor Yellow
$account = az account show 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue
if (-not $account) {
    Write-Host "⚠️  Chưa đăng nhập Azure. Đang đăng nhập..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Đăng nhập thất bại!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ Đã đăng nhập Azure" -ForegroundColor Green
Write-Host ""

# Bật Application Logging
Write-Host "📝 Bật Application Logging..." -ForegroundColor Yellow
az webapp log config `
    --name $AppName `
    --resource-group $ResourceGroup `
    --application-logging filesystem `
    --level information `
    --web-server-logging filesystem `
    --detailed-error-messages true `
    --failed-request-tracing true

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Đã bật logging thành công!" -ForegroundColor Green
} else {
    Write-Host "❌ Có lỗi khi bật logging!" -ForegroundColor Red
    Write-Host "   Kiểm tra tên App Service và Resource Group" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "✅ HOÀN TẤT!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

# Restart app để áp dụng thay đổi
Write-Host "🔄 Đang restart app để áp dụng thay đổi..." -ForegroundColor Yellow
az webapp restart --name $AppName --resource-group $ResourceGroup

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ App đã được restart" -ForegroundColor Green
} else {
    Write-Host "⚠️  Không thể restart app tự động" -ForegroundColor Yellow
    Write-Host "   Vui lòng restart thủ công qua Azure Portal" -ForegroundColor Gray
}

Write-Host ""
Write-Host "📋 CÁCH XEM LOGS:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1️⃣  Qua Azure Portal (Dễ nhất):" -ForegroundColor Yellow
Write-Host "   → App Service → Log stream" -ForegroundColor White
Write-Host ""
Write-Host "2️⃣  Qua Azure CLI (Real-time):" -ForegroundColor Yellow
Write-Host "   az webapp log tail --name $AppName --resource-group $ResourceGroup" -ForegroundColor White
Write-Host ""
Write-Host "3️⃣  Download logs:" -ForegroundColor Yellow
Write-Host "   az webapp log download --name $AppName --resource-group $ResourceGroup --log-file logs.zip" -ForegroundColor White
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Sau khi restart, đợi 30-60 giây rồi xem Log Stream!" -ForegroundColor Yellow
Write-Host ""
