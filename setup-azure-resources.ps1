# Script tạo tất cả Azure resources cần thiết cho GrowingTales
# Chạy script này TRƯỚC KHI deploy

param(
    [string]$ResourceGroup = "growingtales-rg",
    [string]$AppName = "growingtales-app",
    [string]$PlanName = "growingtales-plan",
    [string]$StorageAccount = "growingtalesstorage",
    [string]$FileShare = "growingtales-data",
    [string]$Location = "southeastasia",
    [string]$Sku = "B1"  # F1 (Free), B1 (Basic), S1 (Standard), P1V2 (Premium)
)

Write-Host "🚀 Bắt đầu setup Azure resources..." -ForegroundColor Cyan
Write-Host ""

# Check Azure CLI
$azVersion = az --version 2>$null
if (-not $azVersion) {
    Write-Host "❌ Azure CLI chưa được cài đặt!" -ForegroundColor Red
    Write-Host "Cài đặt từ: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    exit 1
}

# Login check
Write-Host "📌 Kiểm tra đăng nhập..." -ForegroundColor Yellow
$account = az account show 2>$null | ConvertFrom-Json
if (-not $account) {
    Write-Host "⚠️  Đang mở trình duyệt để đăng nhập..." -ForegroundColor Yellow
    az login
}
Write-Host "✅ Subscription: $($account.name)" -ForegroundColor Green
Write-Host ""

# Create Resource Group
Write-Host "📌 Tạo Resource Group: $ResourceGroup" -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location --output none
Write-Host "✅ Resource Group đã sẵn sàng" -ForegroundColor Green
Write-Host ""

# Create App Service Plan
Write-Host "📌 Tạo App Service Plan: $PlanName ($Sku)" -ForegroundColor Yellow
az appservice plan create `
    --name $PlanName `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku $Sku `
    --is-linux `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ App Service Plan đã tạo" -ForegroundColor Green
} else {
    Write-Host "⚠️  App Service Plan có thể đã tồn tại" -ForegroundColor Yellow
}
Write-Host ""

# Create Web App
Write-Host "📌 Tạo Web App: $AppName" -ForegroundColor Yellow
az webapp create `
    --name $AppName `
    --resource-group $ResourceGroup `
    --plan $PlanName `
    --runtime "DOTNETCORE:10.0" `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Web App đã tạo" -ForegroundColor Green
} else {
    Write-Host "⚠️  Web App có thể đã tồn tại hoặc tên đã được sử dụng" -ForegroundColor Yellow
    Write-Host "Thử tên khác: ${AppName}-$(Get-Random -Maximum 9999)" -ForegroundColor Gray
}
Write-Host ""

# Create Storage Account
Write-Host "📌 Tạo Storage Account: $StorageAccount" -ForegroundColor Yellow
az storage account create `
    --name $StorageAccount `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku Standard_LRS `
    --output none

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Storage Account đã tạo" -ForegroundColor Green
} else {
    Write-Host "⚠️  Storage Account có thể đã tồn tại hoặc tên đã được sử dụng" -ForegroundColor Yellow
}
Write-Host ""

# Get Storage Account Key
Write-Host "📌 Lấy Storage Account Key..." -ForegroundColor Yellow
$storageKey = az storage account keys list `
    --account-name $StorageAccount `
    --resource-group $ResourceGroup `
    --query "[0].value" `
    --output tsv

if ($storageKey) {
    Write-Host "✅ Đã lấy Storage Key" -ForegroundColor Green
} else {
    Write-Host "⚠️  Không lấy được Storage Key" -ForegroundColor Yellow
}
Write-Host ""

# Create File Share
if ($storageKey) {
    Write-Host "📌 Tạo File Share: $FileShare" -ForegroundColor Yellow
    az storage share create `
        --name $FileShare `
        --account-name $StorageAccount `
        --account-key $storageKey `
        --quota 5 `
        --output none
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ File Share đã tạo (5GB)" -ForegroundColor Green
    } else {
        Write-Host "⚠️  File Share có thể đã tồn tại" -ForegroundColor Yellow
    }
    Write-Host ""
}

# Configure Web App Settings
Write-Host "📌 Cấu hình Web App..." -ForegroundColor Yellow

# Enable detailed errors
az webapp config set `
    --name $AppName `
    --resource-group $ResourceGroup `
    --startup-file "" `
    --output none

# Configure app settings
az webapp config appsettings set `
    --name $AppName `
    --resource-group $ResourceGroup `
    --settings `
        ASPNETCORE_ENVIRONMENT=Production `
        WEBSITE_TIME_ZONE="SE Asia Standard Time" `
        WEBSITE_LOCAL_CACHE_OPTION=Always `
    --output none

Write-Host "✅ Đã cấu hình cơ bản" -ForegroundColor Green
Write-Host ""

# Mount storage (if created)
if ($storageKey) {
    Write-Host "📌 Mount File Share vào Web App..." -ForegroundColor Yellow
    
    # Note: Azure CLI không hỗ trợ storage mount trực tiếp, phải dùng Portal hoặc ARM template
    Write-Host "⚠️  Vui lòng mount storage thủ công trong Azure Portal:" -ForegroundColor Yellow
    Write-Host "   1. Vào App Service → Configuration → Path mappings" -ForegroundColor Gray
    Write-Host "   2. Nhấn '+ New Azure Storage Mount'" -ForegroundColor Gray
    Write-Host "   3. Điền:" -ForegroundColor Gray
    Write-Host "      - Name: database" -ForegroundColor Gray
    Write-Host "      - Storage account: $StorageAccount" -ForegroundColor Gray
    Write-Host "      - Share name: $FileShare" -ForegroundColor Gray
    Write-Host "      - Mount path: /data" -ForegroundColor Gray
    Write-Host ""
}

# Summary
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host "✅ SETUP HOÀN TẤT!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Thông tin resources đã tạo:" -ForegroundColor Yellow
Write-Host "   • Resource Group: $ResourceGroup" -ForegroundColor White
Write-Host "   • App Service Plan: $PlanName ($Sku)" -ForegroundColor White
Write-Host "   • Web App: $AppName" -ForegroundColor White
Write-Host "   • Storage Account: $StorageAccount" -ForegroundColor White
Write-Host "   • File Share: $FileShare" -ForegroundColor White
Write-Host ""
Write-Host "🌐 URL: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Các bước tiếp theo:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1️⃣  Cấu hình API Keys trong Azure Portal:" -ForegroundColor White
Write-Host "   Vào: App Service → Configuration → Application settings" -ForegroundColor Gray
Write-Host "   Thêm các settings:" -ForegroundColor Gray
Write-Host "   - GeminiAPI__ApiKey = <your-gemini-key>" -ForegroundColor Cyan
Write-Host "   - WhomeAI__ApiKey = <your-whome-key>" -ForegroundColor Cyan
Write-Host "   - VnPay__TmnCode = <your-vnpay-code>" -ForegroundColor Cyan
Write-Host "   - VnPay__HashSecret = <your-vnpay-secret>" -ForegroundColor Cyan
Write-Host ""
Write-Host "2️⃣  Mount File Share (nếu chưa):" -ForegroundColor White
Write-Host "   Theo hướng dẫn ở trên" -ForegroundColor Gray
Write-Host ""
Write-Host "3️⃣  Deploy code:" -ForegroundColor White
Write-Host "   .\deploy-to-azure.ps1" -ForegroundColor Cyan
Write-Host ""
Write-Host "4️⃣  Xem logs:" -ForegroundColor White
Write-Host "   az webapp log tail --name $AppName --resource-group $ResourceGroup" -ForegroundColor Cyan
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Green

Pop-Location
