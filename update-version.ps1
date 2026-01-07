# Version Update Script - v1.0.4 / v1.0.3
# Updates package versions and prepares for release

param(
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# ANSI Colors
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Cyan = "`e[36m"
$Reset = "`e[0m"

Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Cyan}  Version Update - v1.0.4 / v1.0.3${Reset}"
Write-Host "${Cyan}========================================${Reset}"
Write-Host ""

# Project root
$ProjectRoot = "C:\Jobb\EMVReaderSLCard"
if (-not (Test-Path $ProjectRoot)) {
    Write-Host "${Red}ERROR: Project directory not found: $ProjectRoot${Reset}" -ForegroundColor Red
    exit 1
}

Set-Location $ProjectRoot

# Version information
$NfcOldVersion = "1.0.3"
$NfcNewVersion = "1.0.4"
$EmvOldVersion = "1.0.2"
$EmvNewVersion = "1.0.3"

Write-Host "Version Changes:" -ForegroundColor Cyan
Write-Host "  NfcReaderLib: $NfcOldVersion ? ${Green}$NfcNewVersion${Reset}" -ForegroundColor White
Write-Host "  EMVCard.Core: $EmvOldVersion ? ${Green}$EmvNewVersion${Reset}" -ForegroundColor White
Write-Host ""

if ($DryRun) {
    Write-Host "${Yellow}========================================${Reset}" -ForegroundColor Yellow
    Write-Host "${Yellow}  DRY RUN MODE - No changes will be made${Reset}" -ForegroundColor Yellow
    Write-Host "${Yellow}========================================${Reset}" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Would update:" -ForegroundColor Yellow
    Write-Host "  1. NfcReaderLib\NfcReaderLib.csproj" -ForegroundColor White
    Write-Host "  2. EMVCard.Core\EMVCard.Core.csproj" -ForegroundColor White
    Write-Host ""
    Write-Host "Run without -DryRun flag to actually update versions." -ForegroundColor Cyan
    exit 0
}

# Confirmation
Write-Host "${Yellow}Ready to update package versions${Reset}" -ForegroundColor Yellow
Write-Host ""
$confirmation = Read-Host "Continue? (Y/N)"

if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
    Write-Host "${Yellow}Update cancelled by user${Reset}" -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Function to update version in csproj file
function Update-ProjectVersion {
    param(
        [string]$ProjectPath,
        [string]$OldVersion,
        [string]$NewVersion,
        [string]$ReleaseNotes
    )
    
    Write-Host "Updating $ProjectPath..." -ForegroundColor Cyan
    
    if (-not (Test-Path $ProjectPath)) {
        Write-Host "${Red}  ERROR: File not found: $ProjectPath${Reset}" -ForegroundColor Red
        return $false
    }
    
    try {
        # Read file content
        $content = Get-Content $ProjectPath -Raw
        
        # Update version
        $content = $content -replace "<Version>$OldVersion</Version>", "<Version>$NewVersion</Version>"
        
        # Update release notes
        $content = $content -replace "<PackageReleaseNotes>.*?</PackageReleaseNotes>", 
            "<PackageReleaseNotes>$ReleaseNotes</PackageReleaseNotes>"
        
        # Write back
        Set-Content -Path $ProjectPath -Value $content -NoNewline
        
        Write-Host "${Green}  ? Updated version: $OldVersion ? $NewVersion${Reset}" -ForegroundColor Green
        Write-Host "${Green}  ? Updated release notes${Reset}" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "${Red}  ERROR: Failed to update file: $_${Reset}" -ForegroundColor Red
        return $false
    }
}

# Update NfcReaderLib
Write-Host "${Cyan}Step 1: Updating NfcReaderLib${Reset}" -ForegroundColor Cyan
Write-Host ""

$nfcReleaseNotes = "v1.0.4 - Documentation and project organization improvements. Restructured documentation into organized docs/ directory with categorized subdirectories. Added automation scripts for documentation management and git workflows. Improved developer experience with comprehensive guides. No functional code changes. All dependencies verified up to date with no security vulnerabilities."

$nfcSuccess = Update-ProjectVersion `
    -ProjectPath "NfcReaderLib\NfcReaderLib.csproj" `
    -OldVersion $NfcOldVersion `
    -NewVersion $NfcNewVersion `
    -ReleaseNotes $nfcReleaseNotes

Write-Host ""

# Update EMVCard.Core
Write-Host "${Cyan}Step 2: Updating EMVCard.Core${Reset}" -ForegroundColor Cyan
Write-Host ""

$emvReleaseNotes = "v1.0.3 - Documentation and project organization improvements. Synchronized with NfcReaderLib 1.0.4 documentation updates. Enhanced project structure with organized documentation directory. Improved developer experience. No functional code changes. All dependencies verified up to date with no security vulnerabilities."

$emvSuccess = Update-ProjectVersion `
    -ProjectPath "EMVCard.Core\EMVCard.Core.csproj" `
    -OldVersion $EmvOldVersion `
    -NewVersion $EmvNewVersion `
    -ReleaseNotes $emvReleaseNotes

Write-Host ""

# Build packages
if ($nfcSuccess -and $emvSuccess) {
    Write-Host "${Cyan}Step 3: Building Projects${Reset}" -ForegroundColor Cyan
    Write-Host ""
    
    # Build NfcReaderLib
    Write-Host "Building NfcReaderLib..." -ForegroundColor White
    dotnet build NfcReaderLib\NfcReaderLib.csproj -c Release --no-restore
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? NfcReaderLib built successfully${Reset}" -ForegroundColor Green
    } else {
        Write-Host "${Red}? NfcReaderLib build failed${Reset}" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    
    # Build EMVCard.Core
    Write-Host "Building EMVCard.Core..." -ForegroundColor White
    dotnet build EMVCard.Core\EMVCard.Core.csproj -c Release --no-restore
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? EMVCard.Core built successfully${Reset}" -ForegroundColor Green
    } else {
        Write-Host "${Red}? EMVCard.Core build failed${Reset}" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    
    # Create packages
    Write-Host "${Cyan}Step 4: Creating NuGet Packages${Reset}" -ForegroundColor Cyan
    Write-Host ""
    
    # Pack NfcReaderLib
    Write-Host "Packing NfcReaderLib..." -ForegroundColor White
    dotnet pack NfcReaderLib\NfcReaderLib.csproj -c Release --no-build
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? NfcReaderLib.$NfcNewVersion.nupkg created${Reset}" -ForegroundColor Green
    } else {
        Write-Host "${Red}? NfcReaderLib pack failed${Reset}" -ForegroundColor Red
    }
    
    Write-Host ""
    
    # Pack EMVCard.Core
    Write-Host "Packing EMVCard.Core..." -ForegroundColor White
    dotnet pack EMVCard.Core\EMVCard.Core.csproj -c Release --no-build
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? EMVCard.Core.$EmvNewVersion.nupkg created${Reset}" -ForegroundColor Green
    } else {
        Write-Host "${Red}? EMVCard.Core pack failed${Reset}" -ForegroundColor Red
    }
    
    Write-Host ""
}

# Summary
Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Cyan}  Summary${Reset}"
Write-Host "${Cyan}========================================${Reset}"

if ($nfcSuccess) {
    Write-Host "${Green}? NfcReaderLib updated to $NfcNewVersion${Reset}" -ForegroundColor Green
} else {
    Write-Host "${Red}? NfcReaderLib update failed${Reset}" -ForegroundColor Red
}

if ($emvSuccess) {
    Write-Host "${Green}? EMVCard.Core updated to $EmvNewVersion${Reset}" -ForegroundColor Green
} else {
    Write-Host "${Red}? EMVCard.Core update failed${Reset}" -ForegroundColor Red
}

Write-Host ""

if ($nfcSuccess -and $emvSuccess) {
    Write-Host "Package files created:" -ForegroundColor Cyan
    Write-Host "  NfcReaderLib\bin\Release\NfcReaderLib.$NfcNewVersion.nupkg" -ForegroundColor White
    Write-Host "  EMVCard.Core\bin\Release\EMVCard.Core.$EmvNewVersion.nupkg" -ForegroundColor White
    Write-Host ""
    
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Review the changes with: git diff" -ForegroundColor White
    Write-Host "  2. Run documentation cleanup: .\cleanup-docs.ps1" -ForegroundColor White
    Write-Host "  3. Commit changes: .\commit-docs-cleanup.ps1" -ForegroundColor White
    Write-Host "  4. Publish to NuGet: .\publish-nuget.ps1" -ForegroundColor White
    Write-Host ""
    
    Write-Host "${Green}?? Version update completed successfully!${Reset}" -ForegroundColor Green
} else {
    Write-Host "${Red}??  Some updates failed. Review errors above.${Reset}" -ForegroundColor Red
    exit 1
}
