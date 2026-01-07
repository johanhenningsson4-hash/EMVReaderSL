# Markdown Documentation Cleanup Script
# This script organizes markdown documentation files into a structured docs/ directory

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Markdown Documentation Cleanup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Define the root and docs directory
$rootDir = "C:\Jobb\EMVReaderSLCard"
$docsDir = Join-Path $rootDir "docs"

# Create subdirectories
$subdirs = @(
    "architecture",
    "features",
    "fixes",
    "nuget",
    "platform",
    "releases",
    "security",
    "status"
)

Write-Host "Creating documentation structure..." -ForegroundColor Yellow
foreach ($subdir in $subdirs) {
    $path = Join-Path $docsDir $subdir
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
        Write-Host "  Created: docs/$subdir" -ForegroundColor Green
    }
}

Write-Host ""

# Define file mappings (source -> destination)
$fileMappings = @{
    # Architecture
    "REFACTORING_DOCUMENTATION.md" = "architecture/REFACTORING_DOCUMENTATION.md"
    
    # Features
    "CARD_POLLING_FEATURE.md" = "features/CARD_POLLING_FEATURE.md"
    "PAN_MASKING_FEATURE.md" = "features/PAN_MASKING_FEATURE.md"
    "LOGGING_DOCUMENTATION.md" = "features/LOGGING_DOCUMENTATION.md"
    
    # Fixes
    "COMBOBOX_SELECTION_FIX.md" = "fixes/COMBOBOX_SELECTION_FIX.md"
    "CLEARBUFFERS_FIX.md" = "fixes/CLEARBUFFERS_FIX.md"
    "POLLING_CONNECTION_FIX.md" = "fixes/POLLING_CONNECTION_FIX.md"
    "POLLING_RECONNECTION_FIX.md" = "fixes/POLLING_RECONNECTION_FIX.md"
    
    # NuGet
    "NUGET_PACKAGES_CREATED.md" = "nuget/NUGET_PACKAGES_CREATED.md"
    "NUGET_PUBLISHING_GUIDE.md" = "nuget/NUGET_PUBLISHING_GUIDE.md"
    "NUGET_PUBLISHING_STATUS.md" = "nuget/NUGET_PUBLISHING_STATUS.md"
    "NUGET_PUBLISHING_SUCCESS_v1.0.1.md" = "nuget/NUGET_PUBLISHING_SUCCESS_v1.0.1.md"
    "NUGET_PACKAGE_ISSUE_RESOLVED.md" = "nuget/NUGET_PACKAGE_ISSUE_RESOLVED.md"
    "NUGET_PACKAGES_UPDATE_REPORT.md" = "nuget/NUGET_PACKAGES_UPDATE_REPORT.md"
    "NUGET_RELEASE_v1.0.3_PUBLISHING_GUIDE.md" = "nuget/NUGET_RELEASE_v1.0.3_PUBLISHING_GUIDE.md"
    "NUGET_RELEASE_v1.0.3_SUMMARY.md" = "nuget/NUGET_RELEASE_v1.0.3_SUMMARY.md"
    "NUGET_PACKAGES_UPDATE_STATUS_2026.md" = "status/NUGET_PACKAGES_UPDATE_STATUS_2026.md"
    "SUB_PROJECTS_NUGET_UPDATE_REPORT_2026.md" = "status/SUB_PROJECTS_NUGET_UPDATE_REPORT_2026.md"
    
    # Platform
    "MIGRATION_SUMMARY.md" = "platform/MIGRATION_SUMMARY.md"
    
    # Releases
    "RELEASE_CREATION_SUMMARY.md" = "releases/RELEASE_CREATION_SUMMARY.md"
    "GITHUB_RELEASE_COMPLETE.md" = "releases/GITHUB_RELEASE_COMPLETE.md"
    "LICENSE_YEAR_UPDATE_2026.md" = "releases/LICENSE_YEAR_UPDATE_2026.md"
    "FINAL_README_UPDATE_2026.md" = "releases/FINAL_README_UPDATE_2026.md"
    "README_NUGET_VERSION_UPDATE.md" = "releases/README_NUGET_VERSION_UPDATE.md"
    "README_UPDATE_COMPLETE.md" = "releases/README_UPDATE_COMPLETE.md"
    "README_UPDATE_ENHANCEMENT_2026.md" = "releases/README_UPDATE_ENHANCEMENT_2026.md"
    "README_UPDATE_SUMMARY.md" = "releases/README_UPDATE_SUMMARY.md"
    "README_UPDATE_v1.0.3_COMPLETE.md" = "releases/README_UPDATE_v1.0.3_COMPLETE.md"
    
    # Security
    "ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md" = "security/ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md"
    "ISSUER_KEY_EXTRACTION_DOCUMENTATION.md" = "security/ISSUER_KEY_EXTRACTION_DOCUMENTATION.md"
    "SL_TOKEN_INTEGRATION_DOCUMENTATION.md" = "security/SL_TOKEN_INTEGRATION_DOCUMENTATION.md"
    "SL_TOKEN_FORMAT_UPDATE.md" = "security/SL_TOKEN_FORMAT_UPDATE.md"
    
    # Status
    "SOLUTION_SYNC_STATUS.md" = "status/SOLUTION_SYNC_STATUS.md"
}

Write-Host "Moving documentation files..." -ForegroundColor Yellow
$movedCount = 0
$skippedCount = 0

foreach ($mapping in $fileMappings.GetEnumerator()) {
    $sourcePath = Join-Path $rootDir $mapping.Key
    $destPath = Join-Path $docsDir $mapping.Value
    
    if (Test-Path $sourcePath) {
        try {
            Move-Item -Path $sourcePath -Destination $destPath -Force
            Write-Host "  Moved: $($mapping.Key) -> docs/$($mapping.Value)" -ForegroundColor Green
            $movedCount++
        } catch {
            Write-Host "  Error moving $($mapping.Key): $_" -ForegroundColor Red
        }
    } else {
        Write-Host "  Skipped: $($mapping.Key) (not found)" -ForegroundColor Gray
        $skippedCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Cleanup Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Files moved: $movedCount" -ForegroundColor Green
Write-Host "  Files skipped: $skippedCount" -ForegroundColor Yellow
Write-Host ""

# List remaining .md files in root (excluding README.md and LICENSE)
Write-Host "Remaining markdown files in root:" -ForegroundColor Yellow
$remainingMd = Get-ChildItem -Path $rootDir -Filter "*.md" | 
    Where-Object { $_.Name -notin @("README.md", "LICENSE.md") }

if ($remainingMd.Count -eq 0) {
    Write-Host "  None - all documentation organized!" -ForegroundColor Green
} else {
    foreach ($file in $remainingMd) {
        Write-Host "  - $($file.Name)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Documentation cleanup complete!" -ForegroundColor Green
Write-Host "Review the docs/ directory and commit changes when ready." -ForegroundColor Cyan
