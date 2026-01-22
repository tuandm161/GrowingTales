# Script deploy GrowingTales lên Azure
# Yêu cầu: Đã cài Azure CLI và đã login (az login)

param(
    [string]$ResourceGroup = "growingtales-rg",
    [string]$AppName = "growingtales-app",
    [string]$Location = "southeastasia"
)

Write-Host "🚀 Bắt đầu deploy GrowingTales lên Azure..." -ForegroundColor Cyan
Write-Host ""

# Check Azure CLI
Write-Host "📌 Kiểm tra Azure CLI..." -ForegroundColor Yellow
$azVersion = az --version 2>$null
if (-not $azVersion) {
    Write-Host "❌ Azure CLI chưa được cài đặt!" -ForegroundColor Red
    Write-Host "Vui lòng cài đặt từ: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    exit 1
}
Write-Host "✅ Azure CLI đã cài đặt" -ForegroundColor Green

# Check login
Write-Host ""
Write-Host "📌 Kiểm tra đăng nhập Azure..." -ForegroundColor Yellow
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "⚠️  Chưa đăng nhập Azure. Đang mở trình duyệt..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Đăng nhập thất bại!" -ForegroundColor Red
        exit 1
    }
}
Write-Host "✅ Đã đăng nhập: $($account.user.name)" -ForegroundColor Green

# Build project
Write-Host ""
Write-Host "📌 Build project..." -ForegroundColor Yellow
Push-Location "$PSScriptRoot\Server"
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build thất bại!" -ForegroundColor Red
    Pop-Location
    exit 1
}
Write-Host "✅ Build thành công" -ForegroundColor Green

# Publish
Write-Host ""
Write-Host "📌 Publish project..." -ForegroundColor Yellow
dotnet publish --configuration Release --output ./publish
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Publish thất bại!" -ForegroundColor Red
    Pop-Location
    exit 1
}
Write-Host "✅ Publish thành công" -ForegroundColor Green

# Create zip
Write-Host ""
Write-Host "📌 Tạo file zip..." -ForegroundColor Yellow
if (Test-Path "./publish.zip") {
    Remove-Item "./publish.zip" -Force
}
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip -Force
Write-Host "✅ Đã tạo publish.zip" -ForegroundColor Green

# Check if Resource Group exists
Write-Host ""
Write-Host "📌 Kiểm tra Resource Group..." -ForegroundColor Yellow
$rgExists = az group exists --name $ResourceGroup
if ($rgExists -eq "false") {
    Write-Host "⚠️  Resource Group '$ResourceGroup' chưa tồn tại. Đang tạo..." -ForegroundColor Yellow
    az group create --name $ResourceGroup --location $Location
    Write-Host "✅ Đã tạo Resource Group" -ForegroundColor Green
} else {
    Write-Host "✅ Resource Group đã tồn tại" -ForegroundColor Green
}

# Check if App Service exists
Write-Host ""
Write-Host "📌 Kiểm tra App Service..." -ForegroundColor Yellow
$webAppExists = az webapp show --name $AppName --resource-group $ResourceGroup 2>$null
if (-not $webAppExists) {
    Write-Host "❌ App Service '$AppName' chưa tồn tại!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng tạo App Service trước:" -ForegroundColor Yellow
    Write-Host "1. Vào Azure Portal: https://portal.azure.com" -ForegroundColor White
    Write-Host "2. Tạo App Service với tên: $AppName" -ForegroundColor White
    Write-Host "3. Hoặc dùng lệnh:" -ForegroundColor White
    Write-Host ""
    Write-Host "   az appservice plan create --name ${AppName}-plan --resource-group $ResourceGroup --sku B1 --is-linux" -ForegroundColor Cyan
    Write-Host "   az webapp create --name $AppName --resource-group $ResourceGroup --plan ${AppName}-plan --runtime `"DOTNETCORE:8.0`"" -ForegroundColor Cyan
    Write-Host ""
    Pop-Location
    exit 1
}
Write-Host "✅ App Service đã tồn tại" -ForegroundColor Green

# Deploy
Write-Host ""
Write-Host "📌 Deploying lên Azure..." -ForegroundColor Yellow
Write-Host "⏳ Đang upload... (có thể mất 2-5 phút)" -ForegroundColor Gray

az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $AppName `
    --src ./publish.zip

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Deploy thất bại!" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host "✅ Deploy thành công!" -ForegroundColor Green

# Cleanup
Write-Host ""
Write-Host "📌 Dọn dẹp..." -ForegroundColor Yellow
Remove-Item ./publish -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item ./publish.zip -Force -ErrorAction SilentlyContinue
Pop-Location
Write-Host "✅ Đã dọn dẹp" -ForegroundColor Green

# Show results
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host "🎉 DEPLOY THÀNH CÔNG!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 URL: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Các bước tiếp theo:" -ForegroundColor Yellow
Write-Host "1. Cấu hình Environment Variables trong Azure Portal" -ForegroundColor White
Write-Host "   - Vào App Service → Configuration → Application settings" -ForegroundColor Gray
Write-Host "   - Thêm: GeminiAPI__ApiKey, WhomeAI__ApiKey, VnPay__TmnCode, etc." -ForegroundColor Gray
Write-Host ""
Write-Host "2. Kiểm tra logs:" -ForegroundColor White
Write-Host "   az webapp log tail --name $AppName --resource-group $ResourceGroup" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. Restart app sau khi cấu hình:" -ForegroundColor White
Write-Host "   az webapp restart --name $AppName --resource-group $ResourceGroup" -ForegroundColor Cyan
Write-Host ""
Write-Host "4. Mở browser và test: https://$AppName.azurewebsites.net" -ForegroundColor White
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
