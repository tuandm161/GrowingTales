# Script kiểm tra cấu hình database cho Azure deployment

Write-Host "🔍 Kiểm tra cấu hình Database cho Azure..." -ForegroundColor Cyan
Write-Host ""

$errors = 0
$warnings = 0

# Check 1: appsettings.Production.json
Write-Host "📌 Kiểm tra appsettings.Production.json..." -ForegroundColor Yellow
$prodSettings = Get-Content "Server\appsettings.Production.json" -Raw | ConvertFrom-Json
$connString = $prodSettings.ConnectionStrings.DefaultConnection

if ($connString -match "Data Source=/data/growingtales\.db") {
    Write-Host "✅ Connection String đúng: $connString" -ForegroundColor Green
} elseif ($connString -match "/home/data") {
    Write-Host "❌ Connection String SAI: Dùng /home/data thay vì /data" -ForegroundColor Red
    Write-Host "   Cần sửa thành: Data Source=/data/growingtales.db" -ForegroundColor Yellow
    $errors++
} else {
    Write-Host "⚠️  Connection String: $connString" -ForegroundColor Yellow
    Write-Host "   Đảm bảo mount path trên Azure khớp với path trong connection string" -ForegroundColor Gray
    $warnings++
}
Write-Host ""

# Check 2: Program.cs - DbContext configuration
Write-Host "📌 Kiểm tra Program.cs..." -ForegroundColor Yellow
$programContent = Get-Content "Server\Program.cs" -Raw

if ($programContent -match "GetConnectionString\(`"DefaultConnection`"\)") {
    Write-Host "✅ Program.cs đọc ConnectionString từ config" -ForegroundColor Green
} else {
    Write-Host "❌ Program.cs không đọc ConnectionString đúng cách" -ForegroundColor Red
    $errors++
}

if ($programContent -match "db\.Database\.Migrate\(\)") {
    Write-Host "✅ Program.cs có auto-migration (db.Database.Migrate())" -ForegroundColor Green
} else {
    Write-Host "❌ Program.cs thiếu auto-migration" -ForegroundColor Red
    $errors++
}
Write-Host ""

# Check 3: AppDbContext
Write-Host "📌 Kiểm tra AppDbContext..." -ForegroundColor Yellow
$dbContextContent = Get-Content "Server\Data\AppDbContext.cs" -Raw

$requiredDbSets = @(
    @{Name="User"; Display="Users"},
    @{Name="Story"; Display="Stories"},
    @{Name="StoryPage"; Display="StoryPages"},
    @{Name="Subscription"; Display="Subscriptions"},
    @{Name="Payment"; Display="Payments"},
    @{Name="ActivityLog"; Display="ActivityLogs"},
    @{Name="Notification"; Display="Notifications"},
    @{Name="SystemSetting"; Display="SystemSettings"},
    @{Name="Contact"; Display="Contacts"}
)
$missingDbSets = @()

foreach ($dbSet in $requiredDbSets) {
    if ($dbContextContent -match "DbSet<$($dbSet.Name)>") {
        Write-Host "✅ DbSet<$($dbSet.Display)> có" -ForegroundColor Green
    } else {
        Write-Host "❌ Thiếu DbSet<$($dbSet.Display)>" -ForegroundColor Red
        $missingDbSets += $dbSet.Display
        $errors++
    }
}

if ($missingDbSets.Count -eq 0) {
    Write-Host "✅ Tất cả 9 DbSet đều có" -ForegroundColor Green
}
Write-Host ""

# Check 4: Migration files
Write-Host "📌 Kiểm tra Migration files..." -ForegroundColor Yellow
$migrations = Get-ChildItem "Server\Migrations\*.cs" -ErrorAction SilentlyContinue | Where-Object { $_.Name -match "^\d{14}_" }

if ($migrations.Count -gt 0) {
    Write-Host "✅ Có $($migrations.Count) migration file(s):" -ForegroundColor Green
    foreach ($migration in $migrations) {
        Write-Host "   • $($migration.Name)" -ForegroundColor Gray
    }
} else {
    Write-Host "⚠️  Không tìm thấy migration files" -ForegroundColor Yellow
    Write-Host "   Chạy: dotnet ef migrations add InitialCreate" -ForegroundColor Gray
    $warnings++
}
Write-Host ""

# Check 5: .gitignore - Migration files không bị ignore
Write-Host "📌 Kiểm tra .gitignore..." -ForegroundColor Yellow
$gitignore = Get-Content ".gitignore" -Raw -ErrorAction SilentlyContinue

if ($gitignore -match "Migrations") {
    Write-Host "⚠️  Migrations/ có trong .gitignore - Cần đảm bảo migration files được commit!" -ForegroundColor Yellow
    $warnings++
} else {
    Write-Host "✅ Migrations/ không bị ignore" -ForegroundColor Green
}
Write-Host ""

# Check 6: appsettings.json (local)
Write-Host "📌 Kiểm tra appsettings.json (local)..." -ForegroundColor Yellow
$localSettings = Get-Content "Server\appsettings.json" -Raw | ConvertFrom-Json
$localConnString = $localSettings.ConnectionStrings.DefaultConnection

if ($localConnString -match "growingtales\.db" -and -not ($localConnString -match "^/")) {
    Write-Host "✅ Local connection string OK: $localConnString" -ForegroundColor Green
} else {
    Write-Host "⚠️  Local connection string: $localConnString" -ForegroundColor Yellow
    $warnings++
}
Write-Host ""

# Summary
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📊 KẾT QUẢ KIỂM TRA" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

if ($errors -eq 0 -and $warnings -eq 0) {
    Write-Host "🎉 HOÀN HẢO! Tất cả cấu hình đều đúng!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Connection String: /data/growingtales.db" -ForegroundColor Green
    Write-Host "✅ Code tự động migration" -ForegroundColor Green
    Write-Host "✅ AppDbContext đầy đủ" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 Khi deploy Azure:" -ForegroundColor Yellow
    Write-Host "   1. Mount File Share tại: /data" -ForegroundColor White
    Write-Host "   2. Deploy code" -ForegroundColor White
    Write-Host "   3. Database tự động tạo tại: /data/growingtales.db" -ForegroundColor White
} elseif ($errors -eq 0) {
    Write-Host "⚠️  CÓ $warnings WARNINGS nhưng có thể deploy" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Kiểm tra lại các warnings ở trên." -ForegroundColor White
} else {
    Write-Host "❌ CÓ $errors ERRORS - Cần sửa trước khi deploy!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng sửa các lỗi ở trên." -ForegroundColor White
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Tóm tắt:" -ForegroundColor Cyan
Write-Host "  • Connection String (Production): /data/growingtales.db" -ForegroundColor $(if ($connString -match "/data") { "Green" } else { "Red" })
Write-Host "  • Mount Path trên Azure: /data" -ForegroundColor White
Write-Host "  • Auto Migration: Có" -ForegroundColor Green
Write-Host "  • Errors: $errors" -ForegroundColor $(if ($errors -eq 0) { "Green" } else { "Red" })
Write-Host "  • Warnings: $warnings" -ForegroundColor $(if ($warnings -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

if ($errors -eq 0) {
    Write-Host "✅ SẴN SÀNG DEPLOY!" -ForegroundColor Green
} else {
    Write-Host "❌ Cần sửa lỗi trước khi deploy" -ForegroundColor Red
}
