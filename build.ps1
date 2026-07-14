# build.ps1
$ErrorActionPreference = "Stop"

$frontendPath = "Frontend"
$backendPath  = "Backend"
$outputPath   = "publish"

Write-Host "==> Building frontend..." -ForegroundColor Cyan
Push-Location $frontendPath
npm ci
npm run build
Pop-Location

Write-Host "==> Publishing backend (self-contained, single file)..." -ForegroundColor Cyan
dotnet publish "$backendPath" `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -o $outputPath

Write-Host "==> Done! Executable is in .\$outputPath" -ForegroundColor Green