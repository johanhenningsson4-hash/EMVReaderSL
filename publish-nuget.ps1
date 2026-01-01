# NuGet Package Publishing Script
# Version: 1.0.3 / 1.0.2
# Date: January 1, 2026

param(
    [string]$ApiKey = $env:NUGET_API_KEY
)

# ANSI Colors
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Cyan = "`e[36m"
$Reset = "`e[0m"

Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Cyan}  NuGet Package Publishing Script${Reset}"
Write-Host "${Cyan}  NfcReaderLib 1.0.3 / EMVCard.Core 1.0.2${Reset}"
Write-Host "${Cyan}========================================${Reset}`n"

# Check if API key is set
if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Host "${Red}ERROR: NuGet API key not set!${Reset}" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please set the API key using one of these methods:" -ForegroundColor Yellow
    Write-Host "  1. Environment variable: `$env:NUGET_API_KEY = 'your-api-key'" -ForegroundColor Yellow
    Write-Host "  2. Script parameter: .\publish-nuget.ps1 -ApiKey 'your-api-key'" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Get your API key from: https://www.nuget.org/account/apikeys" -ForegroundColor Cyan
    exit 1
}

# Set location
$ProjectRoot = "C:\Jobb\EMVReaderSLCard"
if (-not (Test-Path $ProjectRoot)) {
    Write-Host "${Red}ERROR: Project directory not found: $ProjectRoot${Reset}" -ForegroundColor Red
    exit 1
}

Set-Location $ProjectRoot

# Package paths
$NfcPackagePath = "NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg"
$EmvPackagePath = "EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg"

# Verify packages exist
Write-Host "Verifying packages..." -ForegroundColor Cyan
if (-not (Test-Path $NfcPackagePath)) {
    Write-Host "${Red}ERROR: NfcReaderLib package not found: $NfcPackagePath${Reset}" -ForegroundColor Red
    Write-Host "Run: dotnet pack NfcReaderLib\NfcReaderLib.csproj -c Release" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $EmvPackagePath)) {
    Write-Host "${Red}ERROR: EMVCard.Core package not found: $EmvPackagePath${Reset}" -ForegroundColor Red
    Write-Host "Run: dotnet pack EMVCard.Core\EMVCard.Core.csproj -c Release" -ForegroundColor Yellow
    exit 1
}

Write-Host "${Green}? Both packages found${Reset}`n" -ForegroundColor Green

# Package information
$NfcSize = (Get-Item $NfcPackagePath).Length
$EmvSize = (Get-Item $EmvPackagePath).Length

Write-Host "Package Information:" -ForegroundColor Cyan
Write-Host "  NfcReaderLib 1.0.3: $($NfcSize / 1KB) KB" -ForegroundColor White
Write-Host "  EMVCard.Core 1.0.2: $($EmvSize / 1KB) KB`n" -ForegroundColor White

# Confirmation
Write-Host "Ready to publish to NuGet.org" -ForegroundColor Yellow
$confirmation = Read-Host "Continue? (Y/N)"
if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
    Write-Host "${Yellow}Publishing cancelled by user${Reset}" -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Publish NfcReaderLib
Write-Host "${Cyan}[1/2] Publishing NfcReaderLib 1.0.3...${Reset}" -ForegroundColor Cyan
try {
    dotnet nuget push $NfcPackagePath `
        --api-key $ApiKey `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? NfcReaderLib 1.0.3 published successfully!${Reset}`n" -ForegroundColor Green
        $nfcSuccess = $true
    } else {
        Write-Host "${Red}? Failed to publish NfcReaderLib 1.0.3 (Exit code: $LASTEXITCODE)${Reset}`n" -ForegroundColor Red
        $nfcSuccess = $false
    }
} catch {
    Write-Host "${Red}? Error publishing NfcReaderLib: $_${Reset}`n" -ForegroundColor Red
    $nfcSuccess = $false
}

# Publish EMVCard.Core
Write-Host "${Cyan}[2/2] Publishing EMVCard.Core 1.0.2...${Reset}" -ForegroundColor Cyan
try {
    dotnet nuget push $EmvPackagePath `
        --api-key $ApiKey `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? EMVCard.Core 1.0.2 published successfully!${Reset}`n" -ForegroundColor Green
        $emvSuccess = $true
    } else {
        Write-Host "${Red}? Failed to publish EMVCard.Core 1.0.2 (Exit code: $LASTEXITCODE)${Reset}`n" -ForegroundColor Red
        $emvSuccess = $false
    }
} catch {
    Write-Host "${Red}? Error publishing EMVCard.Core: $_${Reset}`n" -ForegroundColor Red
    $emvSuccess = $false
}

# Summary
Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Cyan}  Publishing Summary${Reset}"
Write-Host "${Cyan}========================================${Reset}"

if ($nfcSuccess) {
    Write-Host "${Green}? NfcReaderLib 1.0.3:${Reset} Published" -ForegroundColor Green
} else {
    Write-Host "${Red}? NfcReaderLib 1.0.3:${Reset} Failed" -ForegroundColor Red
}

if ($emvSuccess) {
    Write-Host "${Green}? EMVCard.Core 1.0.2:${Reset} Published" -ForegroundColor Green
} else {
    Write-Host "${Red}? EMVCard.Core 1.0.2:${Reset} Failed" -ForegroundColor Red
}

Write-Host ""

if ($nfcSuccess -and $emvSuccess) {
    Write-Host "${Green}?? All packages published successfully!${Reset}" -ForegroundColor Green
    Write-Host ""
    Write-Host "Packages will be available at (after indexing ~5-10 minutes):" -ForegroundColor Yellow
    Write-Host "  https://www.nuget.org/packages/NfcReaderLib/1.0.3" -ForegroundColor Cyan
    Write-Host "  https://www.nuget.org/packages/EMVCard.Core/1.0.2" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Installation commands:" -ForegroundColor Yellow
    Write-Host "  dotnet add package NfcReaderLib --version 1.0.3" -ForegroundColor White
    Write-Host "  dotnet add package EMVCard.Core --version 1.0.2" -ForegroundColor White
    exit 0
} else {
    Write-Host "${Red}??  Some packages failed to publish${Reset}" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Check API key is valid and has push permissions" -ForegroundColor White
    Write-Host "  2. Verify package ownership on NuGet.org" -ForegroundColor White
    Write-Host "  3. Ensure version doesn't already exist" -ForegroundColor White
    Write-Host "  4. Check https://www.nuget.org/account/apikeys" -ForegroundColor White
    exit 1
}
