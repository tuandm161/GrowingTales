# Script kiểm tra database của GrowingTales

Write-Host "🔍 Kiểm tra Database GrowingTales..." -ForegroundColor Cyan
Write-Host ""

$dbPath = "Server\growingtales.db"

# Check 1: File tồn tại
Write-Host "📌 Kiểm tra file database..." -ForegroundColor Yellow
if (Test-Path $dbPath) {
    $dbFile = Get-Item $dbPath
    $sizeMB = [math]::Round($dbFile.Length / 1MB, 2)
    Write-Host "✅ Database tồn tại: $dbPath" -ForegroundColor Green
    Write-Host "   Kích thước: $($dbFile.Length) bytes (~$sizeMB MB)" -ForegroundColor Gray
    Write-Host "   Tạo lúc: $($dbFile.CreationTime)" -ForegroundColor Gray
    Write-Host "   Sửa lần cuối: $($dbFile.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "❌ Database chưa được tạo!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Chạy lệnh sau để tạo database:" -ForegroundColor Yellow
    Write-Host "  cd Server" -ForegroundColor Cyan
    Write-Host "  dotnet ef database update" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}
Write-Host ""

# Check 2: Migrations
Write-Host "📌 Kiểm tra migrations..." -ForegroundColor Yellow
Push-Location "Server"

$migrations = dotnet ef migrations list 2>&1 | Select-String -Pattern "^\d{14}_"
if ($migrations) {
    Write-Host "✅ Migrations đã apply:" -ForegroundColor Green
    foreach ($migration in $migrations) {
        Write-Host "   • $migration" -ForegroundColor Gray
    }
} else {
    Write-Host "⚠️  Không tìm thấy migrations" -ForegroundColor Yellow
}
Pop-Location
Write-Host ""

# Check 3: Connection string
Write-Host "📌 Kiểm tra connection string..." -ForegroundColor Yellow
$appsettings = Get-Content "Server\appsettings.json" -Raw | ConvertFrom-Json
$connString = $appsettings.ConnectionStrings.DefaultConnection
if ($connString) {
    Write-Host "✅ Connection string: $connString" -ForegroundColor Green
} else {
    Write-Host "⚠️  Connection string không tìm thấy" -ForegroundColor Yellow
}
Write-Host ""

# Check 4: Test connection bằng app
Write-Host "📌 Test connection bằng app..." -ForegroundColor Yellow
Write-Host "   Đang start app (10 giây)..." -ForegroundColor Gray

Push-Location "Server"
$testJob = Start-Job -ScriptBlock {
    param($path)
    Set-Location $path
    dotnet run 2>&1
} -ArgumentList (Get-Location).Path

Start-Sleep -Seconds 8

$output = Receive-Job $testJob
if ($output -match "Now listening on") {
    Write-Host "✅ App có thể kết nối database và chạy được!" -ForegroundColor Green
    
    # Try to query
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5002" -TimeoutSec 5 -ErrorAction Stop
        Write-Host "✅ Homepage trả về HTTP $($response.StatusCode)" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  App chạy nhưng homepage có lỗi" -ForegroundColor Yellow
        Write-Host "   Lỗi: $($_.Exception.Message)" -ForegroundColor Gray
    }
} else {
    Write-Host "❌ App không khởi động được" -ForegroundColor Red
    Write-Host "   Kiểm tra logs để biết chi tiết" -ForegroundColor Gray
}

Stop-Job $testJob -ErrorAction SilentlyContinue
Remove-Job $testJob -Force -ErrorAction SilentlyContinue
Pop-Location
Write-Host ""

# Summary
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 TÓM TẮT" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "Database path: $dbPath" -ForegroundColor White
if (Test-Path $dbPath) {
    Write-Host "Status: ✅ SẴN SÀNG" -ForegroundColor Green
} else {
    Write-Host "Status: ❌ CHƯA TẠO" -ForegroundColor Red
}
Write-Host ""

Write-Host "Các bảng trong database:" -ForegroundColor Yellow
Write-Host "  • Users - Người dùng" -ForegroundColor White
Write-Host "  • Stories - Truyện" -ForegroundColor White
Write-Host "  • StoryPages - Trang truyện" -ForegroundColor White
Write-Host "  • Subscriptions - Gói đăng ký" -ForegroundColor White
Write-Host "  • Payments - Thanh toán (doanh thu)" -ForegroundColor White
Write-Host "  • ActivityLogs - Nhật ký hoạt động" -ForegroundColor White
Write-Host "  • Notifications - Thông báo" -ForegroundColor White
Write-Host "  • SystemSettings - Cấu hình hệ thống" -ForegroundColor White
Write-Host "  • Contacts - Liên hệ" -ForegroundColor White
Write-Host ""

Write-Host "📖 Hướng dẫn xem data:" -ForegroundColor Yellow
Write-Host "  1. Download DB Browser: https://sqlitebrowser.org/dl/" -ForegroundColor White
Write-Host "  2. Mở file: $dbPath" -ForegroundColor White
Write-Host "  3. Tab 'Browse Data' để xem records" -ForegroundColor White
Write-Host ""

Write-Host "🚀 Deploy lên Azure:" -ForegroundColor Yellow
Write-Host "  Database sẽ tự động được tạo khi deploy lần đầu!" -ForegroundColor White
Write-Host "  Không cần làm gì thêm." -ForegroundColor White
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
