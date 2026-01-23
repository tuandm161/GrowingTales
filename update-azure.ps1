# Script deploy code mới lên Azure nhanh
# Chỉ cần chạy: .\update-azure.ps1

Write-Host "🚀 Deploying code mới lên Azure..." -ForegroundColor Cyan

Push-Location "$PSScriptRoot\Server"

# Build
Write-Host "📦 Building..." -ForegroundColor Yellow
dotnet publish -c Release -o publish --no-self-contained -r linux-x64

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build thất bại!" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Zip
Write-Host "📦 Creating zip..." -ForegroundColor Yellow
Remove-Item publish.zip -Force -ErrorAction SilentlyContinue
Compress-Archive -Path publish/* -DestinationPath publish.zip -Force

# Deploy
Write-Host "☁️ Deploying lên Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip `
    --resource-group growingtales-rg `
    --name growingtales-app `
    --src publish.zip

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Deploy thành công!" -ForegroundColor Green
    Write-Host "🌐 https://growingtales.online" -ForegroundColor Cyan
}
else {
    Write-Host "❌ Deploy thất bại!" -ForegroundColor Red
}

# Cleanup
Remove-Item publish -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item publish.zip -Force -ErrorAction SilentlyContinue
Pop-Location
