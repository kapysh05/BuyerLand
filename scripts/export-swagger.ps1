# Поднимает API на временном порту, дёргает /swagger/v1/swagger.json,
# сохраняет в contracts/swagger.json, затем гасит API.
# Использование:  pwsh ./scripts/export-swagger.ps1

param(
    [string]$Output = "$PSScriptRoot/../contracts/swagger.json",
    [int]$Port = 5099
)

$ErrorActionPreference = 'Stop'
$apiProject = Resolve-Path "$PSScriptRoot/../src/BuyersMarket.Api/BuyersMarket.Api.csproj"
$outDir = Split-Path -Parent $Output
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null }

$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_URLS = "http://localhost:$Port"
# Глушим Swagger BasicAuth на время экспорта.
$env:Swagger__BasicAuth__Username = ''
$env:Swagger__BasicAuth__Password = ''

Write-Host "Starting API on port $Port..."
$proc = Start-Process -FilePath 'dotnet' `
    -ArgumentList @('run', '--no-build', '--no-launch-profile', '--project', "$apiProject") `
    -PassThru -WindowStyle Hidden

try {
    $url = "http://localhost:$Port/swagger/v1/swagger.json"
    $deadline = (Get-Date).AddSeconds(40)
    do {
        Start-Sleep -Milliseconds 800
        try {
            $resp = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 3
            if ($resp.StatusCode -eq 200) { break }
        } catch { }
    } while ((Get-Date) -lt $deadline)

    if (-not $resp -or $resp.StatusCode -ne 200) {
        throw "Не дождался /swagger/v1/swagger.json"
    }
    $resp.Content | Out-File -FilePath $Output -Encoding utf8
    Write-Host "Swagger экспортирован → $Output"
}
finally {
    if ($proc -and -not $proc.HasExited) {
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
}
