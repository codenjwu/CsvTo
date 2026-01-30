# NuGet Package Quick Build Script
# 仅构建包，不发布

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CsvTo NuGet Package Builder" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 清理
Write-Host "🧹 Cleaning..." -ForegroundColor Yellow
dotnet clean CsvTo/CsvTo.csproj --configuration Release --verbosity quiet

# 构建
Write-Host "🔨 Building Release..." -ForegroundColor Yellow
dotnet build CsvTo/CsvTo.csproj --configuration Release

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Build Successful!" -ForegroundColor Green
    Write-Host ""
    
    # 查找包
    $package = Get-ChildItem -Path "CsvTo\bin\Release" -Filter "*.nupkg" -Recurse | Select-Object -First 1
    if ($package) {
        Write-Host "📦 Package created: $($package.FullName)" -ForegroundColor Cyan
        Write-Host "   Size: $([math]::Round($package.Length / 1KB, 2)) KB" -ForegroundColor White
        Write-Host ""
    }
} else {
    Write-Host ""
    Write-Host "❌ Build Failed!" -ForegroundColor Red
    exit 1
}
