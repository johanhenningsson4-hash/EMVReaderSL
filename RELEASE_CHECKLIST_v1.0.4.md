# Release Checklist - v1.0.4 / v1.0.3

**Release Date:** January 1, 2026  
**Version:** NfcReaderLib 1.0.4 / EMVCard.Core 1.0.3  
**Type:** Documentation & Organization Release

---

## Pre-Release Checklist

### ?? Planning
- [x] Create release plan (RELEASE_v1.0.4_PLAN.md)
- [x] Document changes since last release
- [x] Define version numbers
- [x] Write release notes
- [x] Create automation scripts

### ?? Documentation
- [x] Create docs/ directory structure
- [x] Move documentation files to appropriate locations
- [x] Create documentation index (docs/README.md)
- [x] Write cleanup guides
- [ ] Update main README.md with new docs paths
- [ ] Verify all documentation links work
- [ ] Check for broken references

### ?? Code Updates
- [ ] Update NfcReaderLib version: 1.0.3 ? 1.0.4
- [ ] Update EMVCard.Core version: 1.0.2 ? 1.0.3
- [ ] Update release notes in .csproj files
- [ ] Verify no functional code changes
- [ ] Check all dependencies are current

### ??? Build & Test
- [ ] Restore NuGet packages
- [ ] Build NfcReaderLib (Release)
- [ ] Build EMVCard.Core (Release)
- [ ] Run tests (if applicable)
- [ ] Create NuGet packages
- [ ] Verify package contents

---

## Release Execution

### Step 1: Version Update
```powershell
# Run version update script
.\update-version.ps1

# Or manually update:
# - NfcReaderLib\NfcReaderLib.csproj
# - EMVCard.Core\EMVCard.Core.csproj
```

**Verification:**
- [ ] NfcReaderLib.csproj shows version 1.0.4
- [ ] EMVCard.Core.csproj shows version 1.0.3
- [ ] Release notes updated in both files
- [ ] No syntax errors in .csproj files

### Step 2: Documentation Cleanup
```powershell
# Run documentation cleanup
.\cleanup-docs.ps1

# Review changes
git status
dir docs -Recurse
```

**Verification:**
- [ ] docs/ directory created
- [ ] All documentation files moved
- [ ] Root directory is clean
- [ ] docs/README.md exists
- [ ] All subdirectories populated

### Step 3: Update Links
```powershell
# Manually update main README.md
# Change: [Doc](DOC.md)
# To: [Doc](docs/category/DOC.md)
```

**Verification:**
- [ ] All links in README.md updated
- [ ] No broken links
- [ ] Relative paths correct
- [ ] Links open correctly

### Step 4: Build Projects
```powershell
# Restore packages
dotnet restore

# Build NfcReaderLib
dotnet build NfcReaderLib\NfcReaderLib.csproj -c Release

# Build EMVCard.Core
dotnet build EMVCard.Core\EMVCard.Core.csproj -c Release
```

**Verification:**
- [ ] NfcReaderLib builds without errors
- [ ] NfcReaderLib builds without warnings
- [ ] EMVCard.Core builds without errors
- [ ] EMVCard.Core builds without warnings
- [ ] Output files generated correctly

### Step 5: Create Packages
```powershell
# Pack NfcReaderLib
dotnet pack NfcReaderLib\NfcReaderLib.csproj -c Release --no-build

# Pack EMVCard.Core
dotnet pack EMVCard.Core\EMVCard.Core.csproj -c Release --no-build
```

**Verification:**
- [ ] NfcReaderLib.1.0.4.nupkg created
- [ ] EMVCard.Core.1.0.3.nupkg created
- [ ] Package sizes reasonable
- [ ] Package contents correct

### Step 6: Git Operations
```powershell
# Review changes
git status
git diff

# Run commit script
.\commit-docs-cleanup.ps1

# Or manually:
# git add .
# git commit -m "Release v1.0.4/v1.0.3 - Documentation organization"
# git push origin master
```

**Verification:**
- [ ] All files staged
- [ ] Commit message appropriate
- [ ] Commit created successfully
- [ ] Pushed to GitHub
- [ ] No merge conflicts
- [ ] Changes visible on GitHub

### Step 7: Create Git Tag
```powershell
# Create tag for NfcReaderLib
git tag v1.0.4 -m "NfcReaderLib v1.0.4 - Documentation organization"

# Push tag
git push origin v1.0.4

# Create tag for EMVCard.Core (optional)
git tag emvcard-v1.0.3 -m "EMVCard.Core v1.0.3 - Documentation updates"
git push origin emvcard-v1.0.3
```

**Verification:**
- [ ] Tag created successfully
- [ ] Tag pushed to GitHub
- [ ] Tag visible on GitHub tags page

### Step 8: Publish to NuGet
```powershell
# Set API key
$env:NUGET_API_KEY = "your-api-key"

# Run publish script
.\publish-nuget.ps1

# Or manually:
# dotnet nuget push NfcReaderLib\bin\Release\*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
# dotnet nuget push EMVCard.Core\bin\Release\*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

**Verification:**
- [ ] NfcReaderLib 1.0.4 published successfully
- [ ] EMVCard.Core 1.0.3 published successfully
- [ ] No publishing errors
- [ ] Packages appear on NuGet.org

### Step 9: Create GitHub Release
```
1. Go to: https://github.com/johanhenningsson4-hash/EMVReaderSL/releases
2. Click "Draft a new release"
3. Choose tag: v1.0.4
4. Release title: "v1.0.4 - Documentation & Project Organization"
5. Add release notes (from RELEASE_v1.0.4_PLAN.md)
6. Attach packages (optional)
7. Publish release
```

**Verification:**
- [ ] GitHub release created
- [ ] Release notes included
- [ ] Tag linked correctly
- [ ] Release visible publicly

---

## Post-Release Verification

### NuGet Verification
```powershell
# Check packages on NuGet.org
# Wait 5-10 minutes for indexing

# Test installation
mkdir test-install
cd test-install
dotnet new console -f net472
dotnet add package NfcReaderLib --version 1.0.4
dotnet add package EMVCard.Core --version 1.0.3
dotnet build
```

**Verification:**
- [ ] NfcReaderLib 1.0.4 appears on NuGet.org
- [ ] EMVCard.Core 1.0.3 appears on NuGet.org
- [ ] Packages install successfully
- [ ] Dependencies resolve correctly
- [ ] Test project builds successfully

### GitHub Verification
**Repository:**
- [ ] All files in docs/ directory
- [ ] Root directory clean
- [ ] README.md updated
- [ ] All links working
- [ ] Tags visible
- [ ] Release published

**Actions/CI:**
- [ ] No failed builds
- [ ] All checks passing

### Documentation Verification
- [ ] docs/README.md accessible
- [ ] All categories populated
- [ ] Links in index working
- [ ] Cross-references correct
- [ ] No 404 errors

---

## Post-Release Tasks

### Immediate (Within 1 hour)
- [ ] Monitor NuGet.org for package appearance
- [ ] Test installation in clean environment
- [ ] Check GitHub release page
- [ ] Verify documentation links
- [ ] Check for immediate issues

### Short-term (Within 24 hours)
- [ ] Monitor download statistics
- [ ] Check for user feedback
- [ ] Respond to any issues
- [ ] Update project website (if applicable)
- [ ] Announce release (if applicable)

### Medium-term (Within 1 week)
- [ ] Review download trends
- [ ] Check for bug reports
- [ ] Update roadmap
- [ ] Plan next release

---

## Rollback Procedures

### If Issues Before Publishing
```powershell
# Undo local changes
git reset --hard HEAD~1

# Or selectively undo
git checkout HEAD -- file-to-revert
```

### If Issues After Publishing
**NuGet packages cannot be deleted**, but you can:
1. Unlist the package (makes it hidden but still installable)
2. Publish a fixed version (1.0.5)
3. Document issues in release notes

### Emergency Rollback
```powershell
# Revert the commit
git revert HEAD

# Push revert
git push origin master

# Publish hotfix version if needed
```

---

## Issue Tracking

### Known Issues (if any)
- None currently

### Fixed Issues
- Documentation organization
- Project structure cleanup

---

## Communication

### Release Announcement Template

**Subject:** NfcReaderLib v1.0.4 & EMVCard.Core v1.0.3 Released

**Body:**
```
We're pleased to announce the release of NfcReaderLib v1.0.4 and 
EMVCard.Core v1.0.3!

This release focuses on documentation and project organization:
- ?? Restructured documentation into organized docs/ directory
- ?? Added automation scripts for easier contribution
- ?? Comprehensive documentation index
- ? All dependencies verified up to date
- ? No functional changes - fully backward compatible

Installation:
dotnet add package NfcReaderLib --version 1.0.4
dotnet add package EMVCard.Core --version 1.0.3

Release notes: https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/tag/v1.0.4

Feedback welcome!
```

### Channels
- [ ] GitHub Discussions (if enabled)
- [ ] Twitter/X (if applicable)
- [ ] LinkedIn (if applicable)
- [ ] Project website (if applicable)

---

## Metrics to Track

### Download Statistics
- NfcReaderLib 1.0.4 downloads
- EMVCard.Core 1.0.3 downloads
- Comparison to previous versions

### Engagement
- GitHub stars
- Issues opened
- Pull requests
- Discussions

---

## Sign-off

**Release Manager:** Johan Henningsson  
**Date:** January 1, 2026  
**Status:** ? IN PROGRESS

### Completion Sign-off
- [ ] All pre-release checks completed
- [ ] Release executed successfully
- [ ] Post-release verification passed
- [ ] Communication sent
- [ ] Metrics tracking set up

**Final Sign-off:** ________________  
**Date:** ________________

---

**Document Version:** 1.0  
**Last Updated:** January 1, 2026
