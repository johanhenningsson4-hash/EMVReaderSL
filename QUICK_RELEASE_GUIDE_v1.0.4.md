# Quick Release Guide - v1.0.4 / v1.0.3

**?? Execute this release in 5 simple steps**

---

## Prerequisites

- ? All documentation cleanup work done
- ? Scripts tested and working
- ? Git working tree clean
- ? NuGet API key ready

---

## Step-by-Step Execution

### Step 1: Update Versions ?? 2 minutes
```powershell
cd C:\Jobb\EMVReaderSLCard
.\update-version.ps1
```

**? Success Indicators:**
- NfcReaderLib version shows 1.0.4
- EMVCard.Core version shows 1.0.3
- Both projects build successfully
- Packages created

---

### Step 2: Organize Documentation ?? 1 minute
```powershell
.\cleanup-docs.ps1
```

**? Success Indicators:**
- docs/ directory created with subdirectories
- ~33 files moved
- Root directory clean
- No errors reported

---

### Step 3: Update README Links ?? 3 minutes
```powershell
# Manually edit README.md
# Update documentation links from:
#   [REFACTORING_DOCUMENTATION.md](REFACTORING_DOCUMENTATION.md)
# To:
#   [REFACTORING_DOCUMENTATION.md](docs/architecture/REFACTORING_DOCUMENTATION.md)
```

**? Success Indicators:**
- All documentation links point to docs/ directory
- Links tested and working
- No broken references

---

### Step 4: Commit & Push ?? 2 minutes
```powershell
.\commit-docs-cleanup.ps1 -CommitMessage "Release v1.0.4/v1.0.3 - Documentation organization"
```

**? Success Indicators:**
- All changes committed
- Pushed to GitHub successfully
- No merge conflicts

---

### Step 5: Publish to NuGet ?? 3 minutes
```powershell
# Set your API key
$env:NUGET_API_KEY = "your-nuget-api-key-here"

# Publish packages
.\publish-nuget.ps1
```

**? Success Indicators:**
- NfcReaderLib 1.0.4 published
- EMVCard.Core 1.0.3 published
- Both packages appear on NuGet.org

---

## Total Time: ~15 minutes

---

## Verification (5 minutes)

### Test Installation
```powershell
# Create test project
mkdir test-release
cd test-release
dotnet new console -f net472

# Install packages
dotnet add package NfcReaderLib --version 1.0.4
dotnet add package EMVCard.Core --version 1.0.3

# Verify
dotnet build
```

### Check GitHub
- ? Visit: https://github.com/johanhenningsson4-hash/EMVReaderSL
- ? Verify docs/ directory visible
- ? Check documentation links work
- ? Verify commits pushed

### Check NuGet
- ? Visit: https://www.nuget.org/packages/NfcReaderLib
- ? Verify version 1.0.4 visible
- ? Visit: https://www.nuget.org/packages/EMVCard.Core
- ? Verify version 1.0.3 visible

---

## Optional: Create GitHub Release

1. Go to: https://github.com/johanhenningsson4-hash/EMVReaderSL/releases
2. Click "Draft a new release"
3. Choose tag: Create new tag "v1.0.4"
4. Release title: **v1.0.4 - Documentation & Project Organization**
5. Description:
```markdown
## What's New

### ?? Documentation Improvements
- Restructured documentation into organized `docs/` directory
- Created comprehensive documentation index
- Moved 33+ files to categorized subdirectories
- Clean root directory - professional project structure

### ?? Automation Scripts
- `cleanup-docs.ps1` - Automated documentation organization
- `commit-docs-cleanup.ps1` - Git workflow automation
- `update-version.ps1` - Version management

### ?? Package Versions
- **NfcReaderLib 1.0.4** - Documentation improvements
- **EMVCard.Core 1.0.3** - Synchronized with NfcReaderLib

### ? No Breaking Changes
- No functional code changes
- Fully backward compatible
- All dependencies up to date
- No security vulnerabilities

## Installation

```powershell
dotnet add package NfcReaderLib --version 1.0.4
dotnet add package EMVCard.Core --version 1.0.3
```

## Documentation

All documentation now organized in the `docs/` directory:
- [Documentation Index](docs/README.md)
- [Architecture](docs/architecture/)
- [Features](docs/features/)
- [Security](docs/security/)
- [NuGet Guides](docs/nuget/)
```

6. Click "Publish release"

---

## If Something Goes Wrong

### Before Publishing
```powershell
# Undo everything
git reset --hard HEAD~1
```

### After Publishing
```powershell
# Publish a hotfix
# Increment to 1.0.5 and fix the issue
```

---

## Success! ??

**You've successfully released:**
- ? NfcReaderLib v1.0.4
- ? EMVCard.Core v1.0.3
- ? Organized documentation structure
- ? Automation scripts for future releases

**Links:**
- **NuGet NfcReaderLib:** https://www.nuget.org/packages/NfcReaderLib/1.0.4
- **NuGet EMVCard.Core:** https://www.nuget.org/packages/EMVCard.Core/1.0.3
- **GitHub:** https://github.com/johanhenningsson4-hash/EMVReaderSL

---

**Next Steps:**
- Monitor downloads and feedback
- Respond to any issues
- Plan next feature release

**Reference Documents:**
- `RELEASE_v1.0.4_PLAN.md` - Detailed plan
- `RELEASE_CHECKLIST_v1.0.4.md` - Complete checklist
- `docs/README.md` - Documentation index
