# Cleanup Workspace - Optimized Script
# Version: 2.0
# Purpose: Efficiently remove redundant markdown and PowerShell files

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(HelpMessage = "Create backup before deletion")]
    [switch]$Backup
)

#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

#region Configuration
$Config = @{
    ProjectRoot = $PSScriptRoot
    Colors = @{
        Info    = 'Cyan'
        Success = 'Green'
        Warning = 'Yellow'
        Error   = 'Red'
    }
    FilesToKeep = @('README.md', 'LICENSE', 'LICENSE.md')
    ScriptsToKeep = @('commit-docs-cleanup.ps1', 'publish-nuget.ps1', 'cleanup-workspace.ps1')
    FilesToDelete = @(
        # Planning docs (7 files)
        'DOCUMENTATION_CLEANUP_PLAN.md', 'CLEANUP_QUICK_REFERENCE.md', 
        'READY_TO_EXECUTE.md', 'RELEASE_v1.0.4_PLAN.md',
        'RELEASE_CHECKLIST_v1.0.4.md', 'QUICK_RELEASE_GUIDE_v1.0.4.md',
        'RELEASE_v1.0.4_SUMMARY.md',
        
        # Temp scripts (2 files)
        'cleanup-docs.ps1', 'update-version.ps1',
        
        # Moved documentation (35 files) - categorized
        @(
            # Architecture
            'REFACTORING_DOCUMENTATION.md',
            
            # Features
            'CARD_POLLING_FEATURE.md', 'PAN_MASKING_FEATURE.md', 'LOGGING_DOCUMENTATION.md',
            
            # Fixes
            'COMBOBOX_SELECTION_FIX.md', 'CLEARBUFFERS_FIX.md', 
            'POLLING_CONNECTION_FIX.md', 'POLLING_RECONNECTION_FIX.md',
            
            # NuGet
            'NUGET_PACKAGES_CREATED.md', 'NUGET_PUBLISHING_GUIDE.md',
            'NUGET_PUBLISHING_STATUS.md', 'NUGET_PUBLISHING_SUCCESS_v1.0.1.md',
            'NUGET_PACKAGE_ISSUE_RESOLVED.md', 'NUGET_PACKAGES_UPDATE_REPORT.md',
            'NUGET_RELEASE_v1.0.3_PUBLISHING_GUIDE.md', 'NUGET_RELEASE_v1.0.3_SUMMARY.md',
            
            # Platform
            'MIGRATION_SUMMARY.md',
            
            # Releases
            'RELEASE_CREATION_SUMMARY.md', 'GITHUB_RELEASE_COMPLETE.md',
            'LICENSE_YEAR_UPDATE_2026.md', 'FINAL_README_UPDATE_2026.md',
            'README_NUGET_VERSION_UPDATE.md', 'README_UPDATE_COMPLETE.md',
            'README_UPDATE_ENHANCEMENT_2026.md', 'README_UPDATE_SUMMARY.md',
            'README_UPDATE_v1.0.3_COMPLETE.md',
            
            # Security
            'ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md', 'ISSUER_KEY_EXTRACTION_DOCUMENTATION.md',
            'SL_TOKEN_INTEGRATION_DOCUMENTATION.md', 'SL_TOKEN_FORMAT_UPDATE.md',
            
            # Status
            'SOLUTION_SYNC_STATUS.md', 'NUGET_PACKAGES_UPDATE_STATUS_2026.md',
            'SUB_PROJECTS_NUGET_UPDATE_REPORT_2026.md',
            
            # Cleanup guides
            'CLEANUP_SUMMARY.md', 'CLEANUP_UNUSED_FILES_GUIDE.md',
            'CLEANUP_OPTIMIZED_GUIDE.md', 'CODE_OPTIMIZATION_SUMMARY.md'
        )
    ) | ForEach-Object { $_ }  # Flatten array
}
#endregion

#region Helper Functions
function Write-Status {
    param([string]$Message, [string]$Type = 'Info')
    $color = $Config.Colors[$Type]
    Write-Host $Message -ForegroundColor $color
}

function Format-FileSize([long]$Bytes) {
    switch ($Bytes) {
        {$_ -ge 1MB} { return "{0:N2} MB" -f ($_ / 1MB) }
        {$_ -ge 1KB} { return "{0:N2} KB" -f ($_ / 1KB) }
        default { return "$_ bytes" }
    }
}

function Get-FilesToProcess {
    $files = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
    
    foreach ($fileName in $Config.FilesToDelete) {
        $path = Join-Path $Config.ProjectRoot $fileName
        if (Test-Path $path -PathType Leaf) {
            $files.Add((Get-Item $path))
        }
    }
    
    return $files
}

function Show-Summary([System.Collections.Generic.List[System.IO.FileInfo]]$Files) {
    Write-Status "`n=== Files to Delete ===" -Type Info
    
    if ($Files.Count -eq 0) {
        Write-Status "? Workspace is already clean!" -Type Success
        return $false
    }
    
    $totalSize = ($Files | Measure-Object Length -Sum).Sum
    
    foreach ($file in $Files) {
        $size = Format-FileSize $file.Length
        Write-Status "  ? $($file.Name) ($size)" -Type Error
    }
    
    Write-Status "`nTotal: $($Files.Count) files, $(Format-FileSize $totalSize)" -Type Warning
    return $true
}

function Invoke-Cleanup([System.Collections.Generic.List[System.IO.FileInfo]]$Files) {
    $results = @{
        Deleted = 0
        Failed = 0
        FailedFiles = [System.Collections.Generic.List[string]]::new()
    }
    
    Write-Status "`nDeleting files..." -Type Info
    
    foreach ($file in $Files) {
        try {
            if ($PSCmdlet.ShouldProcess($file.Name, "Delete")) {
                Remove-Item $file.FullName -Force -ErrorAction Stop
                Write-Status "  ? $($file.Name)" -Type Success
                $results.Deleted++
            }
        }
        catch {
            Write-Status "  ? $($file.Name): $($_.Exception.Message)" -Type Error
            $results.Failed++
            $results.FailedFiles.Add($file.Name)
        }
    }
    
    return $results
}

function Show-Results($Results) {
    Write-Status "`n=== Cleanup Results ===" -Type Info
    
    if ($Results.Deleted -gt 0) {
        Write-Status "? Deleted: $($Results.Deleted) files" -Type Success
    }
    
    if ($Results.Failed -gt 0) {
        Write-Status "? Failed: $($Results.Failed) files" -Type Error
        $Results.FailedFiles | ForEach-Object { Write-Status "    - $_" -Type Error }
    }
}

function Show-RemainingFiles {
    Write-Status "`n=== Remaining Files ===" -Type Info
    
    $md = Get-ChildItem $Config.ProjectRoot -Filter "*.md" -ErrorAction SilentlyContinue
    $ps1 = Get-ChildItem $Config.ProjectRoot -Filter "*.ps1" -ErrorAction SilentlyContinue
    
    Write-Status "Markdown files:" -Type Info
    $md | ForEach-Object {
        $status = if ($Config.FilesToKeep -contains $_.Name) { "?" } else { "??" }
        $color = if ($Config.FilesToKeep -contains $_.Name) { "Success" } else { "Warning" }
        Write-Status "  $status $($_.Name)" -Type $color
    }
    
    Write-Status "`nPowerShell scripts:" -Type Info
    $ps1 | ForEach-Object {
        $status = if ($Config.ScriptsToKeep -contains $_.Name) { "?" } else { "??" }
        $color = if ($Config.ScriptsToKeep -contains $_.Name) { "Success" } else { "Warning" }
        Write-Status "  $status $($_.Name)" -Type $color
    }
}
#endregion

#region Main Execution
try {
    Write-Status "=== Workspace Cleanup ===" -Type Info
    Write-Status "Working directory: $($Config.ProjectRoot)`n" -Type Info
    
    $filesToProcess = Get-FilesToProcess
    
    if (-not (Show-Summary $filesToProcess)) {
        exit 0
    }
    
    if ($WhatIfPreference) {
        Write-Status "`n[WhatIf Mode] No files will be deleted." -Type Warning
        exit 0
    }
    
    if ($Backup) {
        $backupPath = Join-Path $Config.ProjectRoot "backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
        Write-Status "`nCreating backup at: $backupPath" -Type Info
        $null = New-Item -ItemType Directory -Path $backupPath -Force
        $filesToProcess | ForEach-Object { Copy-Item $_.FullName $backupPath -Force }
        Write-Status "? Backup created" -Type Success
    }
    
    if ($ConfirmPreference -eq 'Low' -and -not $WhatIfPreference) {
        $response = Read-Host "`nContinue with deletion? (Y/N)"
        if ($response -ne 'Y' -and $response -ne 'y') {
            Write-Status "Cancelled by user" -Type Warning
            exit 0
        }
    }
    
    $results = Invoke-Cleanup $filesToProcess
    Show-Results $results
    Show-RemainingFiles
    
    if ($results.Deleted -gt 0) {
        Write-Status "`n? Next steps:" -Type Success
        Write-Status "  git add -A" -Type Info
        Write-Status "  git commit -m 'Clean up unused documentation files'" -Type Info
        Write-Status "  git push origin master" -Type Info
    }
}
catch {
    Write-Status "`n? Error: $($_.Exception.Message)" -Type Error
    exit 1
}
#endregion
