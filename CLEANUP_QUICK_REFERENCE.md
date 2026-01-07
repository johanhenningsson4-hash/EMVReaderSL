# Documentation Cleanup - Quick Reference

## ?? Quick Start

Execute the complete cleanup in 3 steps:

### Step 1: Run Cleanup Script
```powershell
.\cleanup-docs.ps1
```

### Step 2: Review Changes
```powershell
git status
git diff
```

### Step 3: Commit and Push
```powershell
.\commit-docs-cleanup.ps1
```

---

## ?? Available Scripts

### 1. `cleanup-docs.ps1`
**Purpose:** Organize markdown files into docs/ directory structure

**Usage:**
```powershell
.\cleanup-docs.ps1
```

**What it does:**
- Creates docs/ subdirectories (architecture, features, fixes, nuget, platform, releases, security, status)
- Moves ~33 markdown files to appropriate locations
- Reports moved and skipped files
- Lists remaining markdown files in root

**Output:**
```
? Created: docs/architecture
? Created: docs/features
...
? Moved: REFACTORING_DOCUMENTATION.md -> docs/architecture/
? Moved: CARD_POLLING_FEATURE.md -> docs/features/
...
Files moved: 30
Files skipped: 3
```

---

### 2. `commit-docs-cleanup.ps1`
**Purpose:** Commit and push documentation changes to GitHub

**Basic Usage:**
```powershell
.\commit-docs-cleanup.ps1
```

**Advanced Usage:**
```powershell
# Custom commit message
.\commit-docs-cleanup.ps1 -CommitMessage "Docs: Organize documentation structure"

# Dry run (preview without making changes)
.\commit-docs-cleanup.ps1 -DryRun
```

**What it does:**
1. Checks git status
2. Shows changes to be committed
3. Asks for confirmation
4. Stages files (docs/, *.md, *.ps1)
5. Creates commit
6. Pushes to GitHub
7. Verifies sync with remote

**Output:**
```
Step 1: Checking Git Status
Step 2: Detailed Status
Step 3: Adding files to staging
Step 4: Files to be committed
Step 5: Creating commit
Step 6: Pushing to GitHub
Step 7: Verification
? Documentation cleanup committed and pushed successfully!
```

---

### 3. `publish-nuget.ps1`
**Purpose:** Publish NuGet packages to NuGet.org

**Usage:**
```powershell
# Set API key first
$env:NUGET_API_KEY = "your-api-key-here"

# Run publish script
.\publish-nuget.ps1
```

**Located in:** Root directory (for both packages)

---

## ?? Directory Structure After Cleanup

```
EMVReaderSLCard/
??? README.md                      # Main project documentation
??? LICENSE                        # License file
??? cleanup-docs.ps1               # Documentation cleanup script
??? commit-docs-cleanup.ps1        # Git commit/push script
??? DOCUMENTATION_CLEANUP_PLAN.md  # This cleanup plan
??? docs/                          # ?? All documentation here
?   ??? README.md                  # Documentation index
?   ??? architecture/              # Architecture docs
?   ??? features/                  # Feature documentation
?   ??? fixes/                     # Bug fix documentation
?   ??? nuget/                     # NuGet package docs
?   ??? platform/                  # Platform support docs
?   ??? releases/                  # Release notes
?   ??? security/                  # Security documentation
?   ??? status/                    # Status reports
??? EMVReaderSL.csproj
??? NfcReaderLib/
?   ??? NfcReaderLib.csproj
??? EMVCard.Core/
    ??? EMVCard.Core.csproj
```

---

## ?? Complete Workflow

### Initial Cleanup
```powershell
# 1. Navigate to project
cd C:\Jobb\EMVReaderSLCard

# 2. Review the plan
notepad DOCUMENTATION_CLEANUP_PLAN.md

# 3. Run cleanup
.\cleanup-docs.ps1

# 4. Check results
dir docs -Recurse

# 5. Review changes
git status

# 6. Commit and push
.\commit-docs-cleanup.ps1
```

### If You Need to Rollback
```powershell
# Undo the commit (if not pushed yet)
git reset --hard HEAD~1

# If already pushed, revert the commit
git revert HEAD
git push origin master

# Or manually move files back
Get-ChildItem -Path docs -Recurse -Filter "*.md" | 
    Where-Object { $_.Name -ne "README.md" } |
    ForEach-Object { Move-Item $_.FullName -Destination . }
```

---

## ?? Checklist

### Before Running Scripts
- [ ] Backup important files (optional)
- [ ] Read DOCUMENTATION_CLEANUP_PLAN.md
- [ ] Ensure Git working tree is clean
- [ ] Verify you're on correct branch

### After cleanup-docs.ps1
- [ ] Check docs/ directory structure
- [ ] Verify files moved correctly
- [ ] Check root directory is clean
- [ ] Review remaining files in root

### After commit-docs-cleanup.ps1
- [ ] Verify commit created successfully
- [ ] Check files pushed to GitHub
- [ ] Verify on GitHub web interface
- [ ] Test documentation links

---

## ?? Verification Commands

```powershell
# Check what files are in docs/
Get-ChildItem -Path docs -Recurse -Filter "*.md" | Select-Object FullName

# Count files in each subdirectory
Get-ChildItem -Path docs -Directory | ForEach-Object {
    $count = (Get-ChildItem $_.FullName -Filter "*.md").Count
    "$($_.Name): $count files"
}

# List markdown files still in root
Get-ChildItem -Path . -Filter "*.md" | Where-Object { 
    $_.Name -notin @("README.md", "LICENSE.md") 
} | Select-Object Name

# Check git status
git status --short

# View last commit
git log -1 --oneline
```

---

## ?? Troubleshooting

### Problem: Files not moving
**Solution:**
```powershell
# Check file exists
Test-Path "REFACTORING_DOCUMENTATION.md"

# Check docs directory exists
Test-Path "docs"

# Run with -Verbose flag if available
```

### Problem: Git won't commit
**Solution:**
```powershell
# Check git status
git status

# Make sure you have changes
git diff

# Check if files are staged
git diff --cached

# Configure git identity if needed
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### Problem: Push failed
**Solutions:**
```powershell
# Pull first if remote has changes
git pull origin master

# Force push (use with caution!)
git push origin master --force

# Check remote URL
git remote -v

# Test connection
git ls-remote origin
```

### Problem: Merge conflicts
**Solution:**
```powershell
# View conflicts
git status

# Abort merge if needed
git merge --abort

# Or resolve conflicts manually, then:
git add .
git commit
git push origin master
```

---

## ?? Support

### Getting Help
- **Script Issues:** Check error messages in red text
- **Git Issues:** Run `git status` and `git log`
- **GitHub Issues:** Check repository settings and permissions

### Useful Git Commands
```powershell
# View commit history
git log --oneline

# See what changed in last commit
git show HEAD

# View remote repository
git remote -v

# Check branch status
git branch -vv

# Sync with remote
git fetch origin
git status
```

---

## ?? Success Indicators

? **After cleanup-docs.ps1:**
- docs/ directory created with subdirectories
- ~30 files moved to docs/ structure
- Root directory has only README.md and LICENSE
- No errors in script output

? **After commit-docs-cleanup.ps1:**
- Commit created with appropriate message
- All changes pushed to GitHub
- Local and remote are in sync
- Changes visible on GitHub web interface

? **Overall Success:**
- Professional documentation structure
- Easy to find specific documentation
- Clean root directory
- All links working (after README update)

---

## ?? Notes

- **Execution Time:** ~5 minutes total
- **Risk Level:** Low (easy to rollback)
- **Internet Required:** For git push
- **Permissions:** Normal user (no admin required)

---

**Last Updated:** January 1, 2026  
**Version:** 1.0
