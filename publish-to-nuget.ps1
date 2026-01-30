# NuGet 包构建和发布脚本
# 使用方法: .\publish-to-nuget.ps1 [-ApiKey YOUR_API_KEY] [-DryRun]

param(
    [string]$ApiKey = "",
    [switch]$DryRun = $false,
    [switch]$SkipTests = $false
)

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  CsvTo NuGet Package Publisher" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 获取版本号
$csprojPath = "CsvTo\CsvTo.csproj"
[xml]$csproj = Get-Content $csprojPath
$version = $csproj.Project.PropertyGroup.Version
Write-Host "📦 Package Version: $version" -ForegroundColor Green
Write-Host ""

# 步骤 1: 运行测试
if (-not $SkipTests) {
    Write-Host "🧪 Step 1: Running Tests..." -ForegroundColor Yellow
    dotnet test UnitTest/UnitTest.csproj --configuration Release --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Tests failed! Aborting publish." -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ All tests passed!" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "⏭️  Skipping tests (--SkipTests flag used)" -ForegroundColor Yellow
    Write-Host ""
}

# 步骤 2: 清理之前的构建
Write-Host "🧹 Step 2: Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean CsvTo/CsvTo.csproj --configuration Release --verbosity minimal
Remove-Item -Path "CsvTo\bin\Release\*.nupkg" -ErrorAction SilentlyContinue
Write-Host "✅ Clean completed!" -ForegroundColor Green
Write-Host ""

# 步骤 3: 构建 Release 版本
Write-Host "🔨 Step 3: Building Release configuration..." -ForegroundColor Yellow
dotnet build CsvTo/CsvTo.csproj --configuration Release --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""

# 步骤 4: 查找生成的包
Write-Host "🔍 Step 4: Locating NuGet package..." -ForegroundColor Yellow
$packagePath = Get-ChildItem -Path "CsvTo\bin\Release" -Filter "CsvTo.$version.nupkg" -Recurse | Select-Object -First 1
if (-not $packagePath) {
    Write-Host "❌ Package not found!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Package found: $($packagePath.FullName)" -ForegroundColor Green
Write-Host ""

# 步骤 5: 显示包信息
Write-Host "📋 Step 5: Package Information" -ForegroundColor Yellow
Write-Host "   Path: $($packagePath.FullName)" -ForegroundColor White
Write-Host "   Size: $([math]::Round($packagePath.Length / 1KB, 2)) KB" -ForegroundColor White
Write-Host ""

# 步骤 6: 验证包（如果 dotnet nuget verify 可用）
Write-Host "✔️  Step 6: Validating package..." -ForegroundColor Yellow
try {
    dotnet nuget verify $packagePath.FullName 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Package validation passed!" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Package verification not available or failed" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  Package verification not available" -ForegroundColor Yellow
}
Write-Host ""

# Dry Run 模式
if ($DryRun) {
    Write-Host "🏃 DRY RUN MODE - Package built but not published" -ForegroundColor Magenta
    Write-Host ""
    Write-Host "To publish, run:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push `"$($packagePath.FullName)`" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host ""
    exit 0
}

# 步骤 7: 发布到 NuGet
if ($ApiKey -eq "") {
    Write-Host "⚠️  No API key provided. Package built but not published." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To publish, run:" -ForegroundColor Yellow
    Write-Host "  dotnet nuget push `"$($packagePath.FullName)`" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or run this script with: .\publish-to-nuget.ps1 -ApiKey YOUR_API_KEY" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host "🚀 Step 7: Publishing to NuGet.org..." -ForegroundColor Yellow
    Write-Host ""
    
    # 确认发布
    Write-Host "⚠️  WARNING: You are about to publish version $version to NuGet.org" -ForegroundColor Red
    $confirmation = Read-Host "Type 'YES' to confirm"
    
    if ($confirmation -ne "YES") {
        Write-Host "❌ Publish cancelled by user" -ForegroundColor Yellow
        exit 0
    }
    
    dotnet nuget push $packagePath.FullName --api-key $ApiKey --source https://api.nuget.org/v3/index.json
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "=====================================" -ForegroundColor Green
        Write-Host "  ✅ SUCCESSFULLY PUBLISHED!" -ForegroundColor Green
        Write-Host "=====================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Package: CsvTo $version" -ForegroundColor White
        Write-Host "URL: https://www.nuget.org/packages/CsvTo/$version" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Note: It may take a few minutes for the package to appear in search results." -ForegroundColor Yellow
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "❌ Publish failed!" -ForegroundColor Red
        Write-Host "Please check the error messages above." -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "✨ Done!" -ForegroundColor Green
