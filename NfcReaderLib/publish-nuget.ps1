#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes NfcReaderLib to NuGet using environment variable for API key.

.DESCRIPTION
    This script builds, packs, and publishes the NfcReaderLib package to NuGet.
    It uses the NUGET_API_KEY environment variable for authentication.

.PARAMETER ApiKey
    Optional. NuGet API key. If not provided, uses $env:NUGET_API_KEY.
    For security, prefer using the environment variable.

.PARAMETER SkipBuild
    Optional. Skip the build step (useful if already built).

.PARAMETER SkipTests
    Optional. Skip running tests before publishing.

.EXAMPLE
    # Using environment variable (recommended)
    $env:NUGET_API_KEY = "your-key-here"
    ./publish-nuget.ps1

.EXAMPLE
    # Passing key directly (not recommended)
    ./publish-nuget.ps1 -ApiKey "your-key-here"

.EXAMPLE
    # Skip build if already done
    ./publish-nuget.ps1 -SkipBuild
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory=$false)]
    [string]$ApiKey,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests
)

# Script configuration
$ErrorActionPreference = "Stop"
$ProjectPath = "NfcReaderLib.csproj"
$Configuration = "Release"
$OutputDir = "nupkg"

# ANSI color codes for output
$ColorReset = "`e[0m"
$ColorGreen = "`e[32m"
$ColorYellow = "`e[33m"
$ColorRed = "`e[31m"
$ColorBlue = "`e[34m"

function Write-ColorOutput {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message,
        
        [Parameter(Mandatory=$false)]
        [ValidateSet('Info', 'Success', 'Warning', 'Error')]
        [string]$Type = 'Info'
    )
    
    $color = switch ($Type) {
        'Success' { $ColorGreen }
        'Warning' { $ColorYellow }
        'Error'   { $ColorRed }
        default   { $ColorBlue }
    }
    
    Write-Host "$color$Message$ColorReset"
}

function Get-ProjectVersion {
    [xml]$project = Get-Content $ProjectPath
    $version = $project.Project.PropertyGroup.Version
    return $version
}

# Main script
try {
    Write-ColorOutput "========================================" -Type Info
    Write-ColorOutput "  NfcReaderLib NuGet Publishing Script" -Type Info
    Write-ColorOutput "========================================" -Type Info
    Write-Host ""

    # Check if project file exists
    if (-not (Test-Path $ProjectPath)) {
        throw "Project file not found: $ProjectPath"
    }

    # Get version from project
    $version = Get-ProjectVersion
    Write-ColorOutput "Package Version: $version" -Type Info
    Write-Host ""

    # Determine API key source
    if ([string]::IsNullOrWhiteSpace($ApiKey)) {
        $ApiKey = $env:NUGET_API_KEY
        if ([string]::IsNullOrWhiteSpace($ApiKey)) {
            throw "NuGet API key not found. Set NUGET_API_KEY environment variable or pass -ApiKey parameter."
        }
        Write-ColorOutput "? Using API key from environment variable" -Type Success
    } else {
        Write-ColorOutput "? Using API key from parameter (consider using environment variable)" -Type Warning
    }
    Write-Host ""

    # Clean output directory
    if (Test-Path $OutputDir) {
        Write-ColorOutput "Cleaning output directory..." -Type Info
        Remove-Item -Path $OutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

    # Build step
    if (-not $SkipBuild) {
        Write-ColorOutput "Building project..." -Type Info
        dotnet build $ProjectPath --configuration $Configuration
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed with exit code $LASTEXITCODE"
        }
        Write-ColorOutput "? Build completed successfully" -Type Success
        Write-Host ""
    } else {
        Write-ColorOutput "? Skipping build (as requested)" -Type Warning
        Write-Host ""
    }

    # Test step (if not skipped)
    if (-not $SkipTests) {
        Write-ColorOutput "Running tests..." -Type Info
        # Note: Add test command here if you have tests
        # dotnet test --configuration $Configuration --no-build
        Write-ColorOutput "? No tests configured (skipping)" -Type Warning
        Write-Host ""
    }

    # Pack step
    Write-ColorOutput "Packing NuGet package..." -Type Info
    dotnet pack $ProjectPath --configuration $Configuration --no-build --output $OutputDir
    if ($LASTEXITCODE -ne 0) {
        throw "Pack failed with exit code $LASTEXITCODE"
    }
    Write-ColorOutput "? Package created successfully" -Type Success
    Write-Host ""

    # Find the generated package
    $packageFile = Get-ChildItem -Path $OutputDir -Filter "*.nupkg" | Select-Object -First 1
    if (-not $packageFile) {
        throw "No package file found in $OutputDir"
    }
    Write-ColorOutput "Package: $($packageFile.Name)" -Type Info
    Write-ColorOutput "Size: $([math]::Round($packageFile.Length / 1KB, 2)) KB" -Type Info
    Write-Host ""

    # Confirm before publishing
    Write-ColorOutput "Ready to publish to NuGet.org" -Type Warning
    Write-ColorOutput "Package: $($packageFile.Name)" -Type Warning
    $confirmation = Read-Host "Do you want to continue? (Y/N)"
    
    if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
        Write-ColorOutput "? Publishing cancelled by user" -Type Warning
        exit 0
    }
    Write-Host ""

    # Push to NuGet
    Write-ColorOutput "Publishing to NuGet.org..." -Type Info
    dotnet nuget push "$OutputDir/*.nupkg" `
        --api-key $ApiKey `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate
    
    if ($LASTEXITCODE -ne 0) {
        throw "NuGet push failed with exit code $LASTEXITCODE"
    }
    
    Write-Host ""
    Write-ColorOutput "========================================" -Type Success
    Write-ColorOutput "  ? Successfully published to NuGet!" -Type Success
    Write-ColorOutput "========================================" -Type Success
    Write-Host ""
    Write-ColorOutput "Package: NfcReaderLib v$version" -Type Info
    Write-ColorOutput "URL: https://www.nuget.org/packages/NfcReaderLib/$version" -Type Info
    Write-Host ""
    Write-ColorOutput "Note: It may take a few minutes for the package to appear in search results." -Type Warning
    
} catch {
    Write-Host ""
    Write-ColorOutput "========================================" -Type Error
    Write-ColorOutput "  ? Publishing failed!" -Type Error
    Write-ColorOutput "========================================" -Type Error
    Write-Host ""
    Write-ColorOutput "Error: $($_.Exception.Message)" -Type Error
    Write-Host ""
    Write-ColorOutput "Troubleshooting:" -Type Warning
    Write-ColorOutput "1. Check that NUGET_API_KEY is set correctly" -Type Warning
    Write-ColorOutput "2. Verify API key has 'Push' permission" -Type Warning
    Write-ColorOutput "3. Ensure version number is incremented" -Type Warning
    Write-ColorOutput "4. Check network connectivity" -Type Warning
    exit 1
}
