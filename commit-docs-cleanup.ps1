# Git Commit and Push Script for Documentation Cleanup
# This script commits the documentation reorganization and pushes to GitHub

param(
    [string]$CommitMessage = "Organize documentation into docs/ directory structure",
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
Write-Host "${Cyan}  Git Commit & Push - Documentation${Reset}"
Write-Host "${Cyan}========================================${Reset}"
Write-Host ""

# Set location
$ProjectRoot = "C:\Jobb\EMVReaderSLCard"
if (-not (Test-Path $ProjectRoot)) {
    Write-Host "${Red}ERROR: Project directory not found: $ProjectRoot${Reset}" -ForegroundColor Red
    exit 1
}

Set-Location $ProjectRoot

# Check if we're in a git repository
if (-not (Test-Path ".git")) {
    Write-Host "${Red}ERROR: Not a git repository!${Reset}" -ForegroundColor Red
    exit 1
}

Write-Host "${Cyan}Step 1: Checking Git Status${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    $gitStatus = git status --short
    
    if ([string]::IsNullOrWhiteSpace($gitStatus)) {
        Write-Host "${Yellow}No changes to commit. Working tree clean.${Reset}" -ForegroundColor Yellow
        exit 0
    }
    
    Write-Host "Changes detected:" -ForegroundColor Yellow
    git status --short
    Write-Host ""
    
} catch {
    Write-Host "${Red}ERROR: Failed to get git status: $_${Reset}" -ForegroundColor Red
    exit 1
}

# Show detailed status
Write-Host "${Cyan}Step 2: Detailed Status${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    git status
    Write-Host ""
} catch {
    Write-Host "${Red}ERROR: Failed to show detailed status: $_${Reset}" -ForegroundColor Red
    exit 1
}

# Dry run mode - show what would be done
if ($DryRun) {
    Write-Host "${Yellow}========================================${Reset}" -ForegroundColor Yellow
    Write-Host "${Yellow}  DRY RUN MODE - No changes will be made${Reset}" -ForegroundColor Yellow
    Write-Host "${Yellow}========================================${Reset}" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Would execute:" -ForegroundColor Yellow
    Write-Host "  git add docs/" -ForegroundColor White
    Write-Host "  git add *.md" -ForegroundColor White
    Write-Host "  git add *.ps1" -ForegroundColor White
    Write-Host "  git commit -m `"$CommitMessage`"" -ForegroundColor White
    Write-Host "  git push origin master" -ForegroundColor White
    Write-Host ""
    Write-Host "Run without -DryRun flag to actually commit and push." -ForegroundColor Cyan
    exit 0
}

# Confirmation
Write-Host "${Yellow}Ready to commit and push changes${Reset}" -ForegroundColor Yellow
Write-Host "Commit message: ${Cyan}$CommitMessage${Reset}" -ForegroundColor White
Write-Host ""
$confirmation = Read-Host "Continue? (Y/N)"

if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
    Write-Host "${Yellow}Operation cancelled by user${Reset}" -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# Step 3: Add files
Write-Host "${Cyan}Step 3: Adding files to staging${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    # Add docs directory
    if (Test-Path "docs") {
        Write-Host "  Adding docs/ directory..." -ForegroundColor White
        git add docs/
        Write-Host "${Green}  ? Added docs/${Reset}" -ForegroundColor Green
    }
    
    # Add markdown files
    Write-Host "  Adding markdown files..." -ForegroundColor White
    git add *.md
    Write-Host "${Green}  ? Added *.md${Reset}" -ForegroundColor Green
    
    # Add PowerShell scripts
    Write-Host "  Adding PowerShell scripts..." -ForegroundColor White
    git add *.ps1
    Write-Host "${Green}  ? Added *.ps1${Reset}" -ForegroundColor Green
    
    # Add any deleted files
    Write-Host "  Staging deletions..." -ForegroundColor White
    git add -u
    Write-Host "${Green}  ? Staged deletions${Reset}" -ForegroundColor Green
    
    Write-Host ""
} catch {
    Write-Host "${Red}ERROR: Failed to add files: $_${Reset}" -ForegroundColor Red
    exit 1
}

# Step 4: Show what will be committed
Write-Host "${Cyan}Step 4: Files to be committed${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    git diff --cached --name-status
    Write-Host ""
    
    $stagedCount = (git diff --cached --name-only | Measure-Object).Count
    Write-Host "Total files staged: ${Green}$stagedCount${Reset}" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "${Red}ERROR: Failed to show staged files: $_${Reset}" -ForegroundColor Red
    exit 1
}

# Step 5: Commit
Write-Host "${Cyan}Step 5: Creating commit${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    git commit -m "$CommitMessage"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? Commit created successfully${Reset}" -ForegroundColor Green
        
        # Get commit hash
        $commitHash = git rev-parse --short HEAD
        Write-Host "  Commit: ${Cyan}$commitHash${Reset}" -ForegroundColor White
        Write-Host "  Message: ${Cyan}$CommitMessage${Reset}" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "${Red}? Commit failed${Reset}" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "${Red}ERROR: Failed to create commit: $_${Reset}" -ForegroundColor Red
    exit 1
}

# Step 6: Push to remote
Write-Host "${Cyan}Step 6: Pushing to GitHub${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    # Check current branch
    $currentBranch = git rev-parse --abbrev-ref HEAD
    Write-Host "  Current branch: ${Cyan}$currentBranch${Reset}" -ForegroundColor White
    
    # Push
    Write-Host "  Pushing to origin/$currentBranch..." -ForegroundColor White
    git push origin $currentBranch
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "${Green}? Successfully pushed to GitHub${Reset}" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "${Red}? Push failed${Reset}" -ForegroundColor Red
        Write-Host ""
        Write-Host "You may need to pull first if remote has changes:" -ForegroundColor Yellow
        Write-Host "  git pull origin $currentBranch" -ForegroundColor White
        exit 1
    }
} catch {
    Write-Host "${Red}ERROR: Failed to push: $_${Reset}" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  1. Check internet connection" -ForegroundColor White
    Write-Host "  2. Verify GitHub credentials" -ForegroundColor White
    Write-Host "  3. Try: git pull origin $currentBranch" -ForegroundColor White
    Write-Host "  4. Check GitHub repository access" -ForegroundColor White
    exit 1
}

# Step 7: Verify on remote
Write-Host "${Cyan}Step 7: Verification${Reset}" -ForegroundColor Cyan
Write-Host ""

try {
    # Get remote URL
    $remoteUrl = git remote get-url origin
    Write-Host "  Remote URL: ${Cyan}$remoteUrl${Reset}" -ForegroundColor White
    
    # Check if push was successful
    $localCommit = git rev-parse HEAD
    $remoteCommit = git rev-parse origin/$currentBranch
    
    if ($localCommit -eq $remoteCommit) {
        Write-Host "${Green}  ? Local and remote are in sync${Reset}" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "${Yellow}  ? Local and remote commits differ${Reset}" -ForegroundColor Yellow
        Write-Host "    Local:  $localCommit" -ForegroundColor White
        Write-Host "    Remote: $remoteCommit" -ForegroundColor White
        Write-Host ""
    }
} catch {
    Write-Host "${Yellow}  ? Could not verify remote status${Reset}" -ForegroundColor Yellow
    Write-Host ""
}

# Summary
Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Cyan}  Summary${Reset}"
Write-Host "${Cyan}========================================${Reset}"
Write-Host "${Green}? Files added to staging${Reset}" -ForegroundColor Green
Write-Host "${Green}? Commit created${Reset}" -ForegroundColor Green
Write-Host "${Green}? Pushed to GitHub${Reset}" -ForegroundColor Green
Write-Host ""

# Show GitHub link
$repoUrl = $remoteUrl -replace '\.git$', ''
Write-Host "View on GitHub:" -ForegroundColor Cyan
Write-Host "  $repoUrl" -ForegroundColor White
Write-Host ""

# Next steps
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Verify changes on GitHub" -ForegroundColor White
Write-Host "  2. Check that documentation links work" -ForegroundColor White
Write-Host "  3. Review the new docs/ structure" -ForegroundColor White
Write-Host ""

Write-Host "${Green}?? Documentation cleanup committed and pushed successfully!${Reset}" -ForegroundColor Green
