# Script test dự án trước khi deploy lên Azure
# Giúp phát hiện lỗi sớm

Write-Host "🧪 Bắt đầu test dự án trước khi deploy..." -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0
$WarningCount = 0

# Test 1: Check .NET SDK
Write-Host "📌 Test 1: Kiểm tra .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($dotnetVersion) {
    Write-Host "✅ .NET SDK version: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "❌ .NET SDK chưa được cài đặt!" -ForegroundColor Red
    $ErrorCount++
}
Write-Host ""

# Test 2: Check project structure
Write-Host "📌 Test 2: Kiểm tra cấu trúc project..." -ForegroundColor Yellow
$requiredFiles = @(
    "Server\Server.csproj",
    "Server\Program.cs",
    "Server\appsettings.json",
    "Server\appsettings.Production.json"
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Thiếu file: $file" -ForegroundColor Red
        $ErrorCount++
    }
}
Write-Host ""

# Test 3: Build project
Write-Host "📌 Test 3: Build project..." -ForegroundColor Yellow
Push-Location "Server"
dotnet build --configuration Release --no-restore --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build thành công" -ForegroundColor Green
} else {
    Write-Host "❌ Build thất bại - Kiểm tra lỗi ở trên" -ForegroundColor Red
    $ErrorCount++
}
Pop-Location
Write-Host ""

# Test 4: Check appsettings.Production.json
Write-Host "📌 Test 4: Kiểm tra appsettings.Production.json..." -ForegroundColor Yellow
$prodSettings = Get-Content "Server\appsettings.Production.json" -Raw | ConvertFrom-Json
$checks = @(
    @{ Key = "GeminiAPI.ApiKey"; Path = $prodSettings.GeminiAPI.ApiKey; Name = "Gemini API Key" },
    @{ Key = "WhomeAI.ApiKey"; Path = $prodSettings.WhomeAI.ApiKey; Name = "WhomeAI API Key" }
)

foreach ($check in $checks) {
    if ([string]::IsNullOrEmpty($check.Path)) {
        Write-Host "⚠️  $($check.Name) chưa được cấu hình trong file" -ForegroundColor Yellow
        Write-Host "   → Nhớ cấu hình trong Azure Portal: Configuration > Application settings" -ForegroundColor Gray
        $WarningCount++
    } else {
        Write-Host "✅ $($check.Name) đã có trong file" -ForegroundColor Green
    }
}
Write-Host ""

# Test 5: Check database migration
Write-Host "📌 Test 5: Kiểm tra database migrations..." -ForegroundColor Yellow
$migrations = Get-ChildItem "Server\Migrations\*.cs" -ErrorAction SilentlyContinue
if ($migrations.Count -gt 0) {
    Write-Host "✅ Có $($migrations.Count) migration files" -ForegroundColor Green
} else {
    Write-Host "⚠️  Không tìm thấy migration files" -ForegroundColor Yellow
    $WarningCount++
}
Write-Host ""

# Test 6: Check Views
Write-Host "📌 Test 6: Kiểm tra Razor Views..." -ForegroundColor Yellow
$requiredViews = @(
    "Server\Views\_ViewImports.cshtml",
    "Server\Views\_ViewStart.cshtml",
    "Server\Views\Shared\_Layout.cshtml",
    "Server\Views\Home\Index.cshtml",
    "Server\Views\Account\Login.cshtml",
    "Server\Views\Account\Register.cshtml",
    "Server\Views\Story\Create.cshtml",
    "Server\Views\Story\Index.cshtml",
    "Server\Views\Story\Details.cshtml",
    "Server\Views\Subscription\Pricing.cshtml"
)

$missingViews = 0
foreach ($view in $requiredViews) {
    if (-not (Test-Path $view)) {
        Write-Host "❌ Thiếu: $view" -ForegroundColor Red
        $missingViews++
    }
}

if ($missingViews -eq 0) {
    Write-Host "✅ Tất cả views cần thiết đều có ($($requiredViews.Count) files)" -ForegroundColor Green
} else {
    Write-Host "❌ Thiếu $missingViews views" -ForegroundColor Red
    $ErrorCount++
}
Write-Host ""

# Test 7: Check wwwroot
Write-Host "📌 Test 7: Kiểm tra static files..." -ForegroundColor Yellow
$staticFiles = @(
    "Server\wwwroot\css\site.css",
    "Server\wwwroot\js\site.js"
)

foreach ($file in $staticFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Thiếu: $file" -ForegroundColor Red
        $ErrorCount++
    }
}
Write-Host ""

# Test 8: Check sensitive files NOT in git
Write-Host "📌 Test 8: Kiểm tra files nhạy cảm..." -ForegroundColor Yellow
$sensitiveFiles = @(
    "Server\growingtales.db",
    "Server\appsettings.Development.json"
)

foreach ($file in $sensitiveFiles) {
    if (Test-Path $file) {
        # Check if in .gitignore
        $gitignore = Get-Content ".gitignore" -Raw
        $fileName = Split-Path $file -Leaf
        if ($gitignore -match $fileName) {
            Write-Host "✅ $fileName đã trong .gitignore" -ForegroundColor Green
        } else {
            Write-Host "⚠️  $fileName nên thêm vào .gitignore" -ForegroundColor Yellow
            $WarningCount++
        }
    }
}
Write-Host ""

# Test 9: Check deployment files
Write-Host "📌 Test 9: Kiểm tra deployment files..." -ForegroundColor Yellow
$deployFiles = @(
    "Server\.deployment",
    "Server\web.config",
    "deploy-to-azure.ps1",
    "setup-azure-resources.ps1"
)

foreach ($file in $deployFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Thiếu: $file (không bắt buộc)" -ForegroundColor Yellow
        $WarningCount++
    }
}
Write-Host ""

# Test 10: Try run local
Write-Host "📌 Test 10: Test chạy local (10 giây)..." -ForegroundColor Yellow
Push-Location "Server"

# Start app in background
$job = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run 2>&1
} -ArgumentList (Get-Location).Path

# Wait for app to start
Start-Sleep -Seconds 8

# Check if running
$output = Receive-Job $job
if ($output -match "Now listening on") {
    Write-Host "✅ App có thể chạy local" -ForegroundColor Green
    
    # Try to access
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5002" -TimeoutSec 5 -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ Homepage trả về HTTP 200" -ForegroundColor Green
        }
    } catch {
        Write-Host "⚠️  Homepage không load được (có thể do chưa config API keys)" -ForegroundColor Yellow
        $WarningCount++
    }
} else {
    Write-Host "❌ App không chạy được local" -ForegroundColor Red
    Write-Host "Lỗi:" -ForegroundColor Yellow
    Write-Host $output -ForegroundColor Gray
    $ErrorCount++
}

# Stop app
Stop-Job $job -ErrorAction SilentlyContinue
Remove-Job $job -Force -ErrorAction SilentlyContinue
Pop-Location
Write-Host ""

# Summary
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 KẾT QUẢ TEST" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

if ($ErrorCount -eq 0 -and $WarningCount -eq 0) {
    Write-Host "🎉 HOÀN HẢO! Dự án sẵn sàng để deploy!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Các bước tiếp theo:" -ForegroundColor Yellow
    Write-Host "1. Chạy: .\setup-azure-resources.ps1" -ForegroundColor White
    Write-Host "2. Cấu hình API keys trong Azure Portal" -ForegroundColor White
    Write-Host "3. Chạy: .\deploy-to-azure.ps1" -ForegroundColor White
} elseif ($ErrorCount -eq 0) {
    Write-Host "⚠️  CÓ $WarningCount WARNINGS nhưng có thể deploy" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Kiểm tra lại các warnings ở trên." -ForegroundColor White
    Write-Host "Nếu OK, có thể tiếp tục deploy." -ForegroundColor White
} else {
    Write-Host "❌ CÓ $ErrorCount ERRORS - Cần sửa trước khi deploy!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng sửa các lỗi ở trên trước khi deploy." -ForegroundColor White
}

Write-Host ""
Write-Host "Tổng kết:" -ForegroundColor Cyan
Write-Host "  • Errors: $ErrorCount" -ForegroundColor $(if ($ErrorCount -eq 0) { "Green" } else { "Red" })
Write-Host "  • Warnings: $WarningCount" -ForegroundColor $(if ($WarningCount -eq 0) { "Green" } else { "Yellow" })
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
