# License Year Update to 2026

**Date:** 2026-01-15  
**Status:** ? COMPLETED  
**Updated Files:** 5 files

## Overview

Updated all copyright and license year references from 2024/2025 to 2026 across the entire project.

## Files Updated

### 1. Main Project README (`README.md`)
**Location:** `C:\Jobb\EMVReaderSLCard\README.md`

**Changes:**
- Footer: `**2008-2024**` ? `**2008-2026**`

**Line Changed:**
```markdown
---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this repo if you find it useful!**
```

### 2. NfcReaderLib README (`NfcReaderLib\README.md`)
**Location:** `C:\Jobb\EMVReaderSLCard\NfcReaderLib\README.md`

**Changes:**
- Footer: `**2008-2024**` ? `**2008-2026**`

**Line Changed:**
```markdown
---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this package on GitHub if you find it useful!**
```

### 3. EMVCard.Core README (`EMVCard.Core\README.md`)
**Location:** `C:\Jobb\EMVReaderSLCard\EMVCard.Core\README.md`

**Changes:**
- Footer: `**2008-2024**` ? `**2008-2026**`

**Line Changed:**
```markdown
---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this package on GitHub if you find it useful!**
```

### 4. NfcReaderLib Project File (`NfcReaderLib\NfcReaderLib.csproj`)
**Location:** `C:\Jobb\EMVReaderSLCard\NfcReaderLib\NfcReaderLib.csproj`

**Changes:**
- Copyright property: `Copyright © Johan Henningsson 2024` ? `Copyright © Johan Henningsson 2026`

**Before:**
```xml
<Copyright>Copyright © Johan Henningsson 2024</Copyright>
```

**After:**
```xml
<Copyright>Copyright © Johan Henningsson 2026</Copyright>
```

### 5. EMVCard.Core Project File (`EMVCard.Core\EMVCard.Core.csproj`)
**Location:** `C:\Jobb\EMVReaderSLCard\EMVCard.Core\EMVCard.Core.csproj`

**Changes:**
- Copyright property: `Copyright © Johan Henningsson 2024` ? `Copyright © Johan Henningsson 2026`

**Before:**
```xml
<Copyright>Copyright © Johan Henningsson 2024</Copyright>
```

**After:**
```xml
<Copyright>Copyright © Johan Henningsson 2026</Copyright>
```

### 6. AssemblyInfo.cs (`Properties\AssemblyInfo.cs`)
**Location:** `C:\Jobb\EMVReaderSLCard\Properties\AssemblyInfo.cs`

**Changes:**
- AssemblyCopyright: `Copyright © TUTU 2025` ? `Copyright © TUTU 2026`

**Before:**
```csharp
[assembly: AssemblyCopyright("Copyright © TUTU 2025")]
```

**After:**
```csharp
[assembly: AssemblyCopyright("Copyright © TUTU 2026")]
```

## Summary of Changes

| File | Location | Old Year | New Year | Property/Section |
|------|----------|----------|----------|------------------|
| README.md | Root | 2008-2024 | 2008-2026 | Footer |
| README.md | NfcReaderLib | 2008-2024 | 2008-2026 | Footer |
| README.md | EMVCard.Core | 2008-2024 | 2008-2026 | Footer |
| NfcReaderLib.csproj | NfcReaderLib | 2024 | 2026 | `<Copyright>` |
| EMVCard.Core.csproj | EMVCard.Core | 2024 | 2026 | `<Copyright>` |
| AssemblyInfo.cs | Properties | 2025 | 2026 | `AssemblyCopyright` |

## Copyright Notice Updates

### README Files
All three README files now consistently show:
```markdown
**Made with ?? by Johan Henningsson** | **2008-2026**
```

This indicates:
- **2008**: Original creation year (original EMVReader)
- **2026**: Current license year

### Project Files (NuGet Packages)
Both NuGet package project files now show:
```xml
<Copyright>Copyright © Johan Henningsson 2026</Copyright>
```

### Assembly Info
The main application assembly info shows:
```csharp
[assembly: AssemblyCopyright("Copyright © TUTU 2026")]
```

Note: "TUTU" refers to the original author (Eternal TUTU from 2008).

## Impact

### Documentation
- ? README files reflect current year
- ? Consistent across all package documentation
- ? Professional and up-to-date appearance

### NuGet Packages
- ? Package metadata includes current copyright
- ? Copyright displayed on NuGet.org package pages
- ? Legal information current

### Application Assembly
- ? Executable file properties show current year
- ? Windows Explorer file properties display 2026
- ? About dialogs (if any) will show current year

## Verification

### Build Status
```
? Build successful
? No compilation errors
? No warnings
```

### Files Validated
```
? All README.md files readable
? All .csproj files valid XML
? AssemblyInfo.cs compiles correctly
```

### Consistency Check
```
? All user-facing years updated to 2026
? Historical range maintained (2008-2026)
? Copyright notices properly formatted
```

## License Information

### Project License
**License:** MIT License  
**Copyright Holder:** Johan Henningsson  
**Years:** 2008-2026  

### Original Work
**Original Author:** Eternal TUTU (2008)  
**Modified By:** Johan Henningsson  
**Current Maintainer:** Johan Henningsson  

### NuGet Packages
- **NfcReaderLib**: MIT License, Copyright © 2026
- **EMVCard.Core**: MIT License, Copyright © 2026

## Future Maintenance

### Annual Update Process

To update the license year in future years (e.g., 2027):

1. **Update README files** (3 files):
   ```bash
   # Search and replace in all README.md files
   Find: "2008-2026"
   Replace: "2008-2027"
   ```

2. **Update project files** (2 files):
   ```bash
   # Update NfcReaderLib.csproj
   Find: "Copyright © Johan Henningsson 2026"
   Replace: "Copyright © Johan Henningsson 2027"
   
   # Update EMVCard.Core.csproj
   Find: "Copyright © Johan Henningsson 2026"
   Replace: "Copyright © Johan Henningsson 2027"
   ```

3. **Update AssemblyInfo.cs** (1 file):
   ```bash
   # Update Properties\AssemblyInfo.cs
   Find: "Copyright © TUTU 2026"
   Replace: "Copyright © TUTU 2027"
   ```

4. **Verify and build**:
   ```bash
   dotnet build
   ```

5. **Commit changes**:
   ```bash
   git add .
   git commit -m "Update license year to 2027"
   git push
   ```

### Automated Approach (Optional)

Create a PowerShell script `update-year.ps1`:

```powershell
param(
    [int]$NewYear = (Get-Date).Year
)

$files = @(
    "README.md",
    "NfcReaderLib\README.md",
    "EMVCard.Core\README.md",
    "NfcReaderLib\NfcReaderLib.csproj",
    "EMVCard.Core\EMVCard.Core.csproj",
    "Properties\AssemblyInfo.cs"
)

$oldYear = $NewYear - 1

foreach ($file in $files) {
    $content = Get-Content $file -Raw
    $content = $content -replace $oldYear, $NewYear
    Set-Content $file $content -NoNewline
    Write-Host "? Updated $file"
}

Write-Host "?? All files updated to year $NewYear"
```

**Usage:**
```powershell
.\update-year.ps1 -NewYear 2027
```

## Related Documents

- [LICENSE.txt](LICENSE.txt) - Full MIT License text
- [README.md](README.md) - Main project documentation
- [NUGET_PACKAGES_CREATED.md](NUGET_PACKAGES_CREATED.md) - NuGet package information
- [NUGET_PUBLISHING_STATUS.md](NUGET_PUBLISHING_STATUS.md) - Publishing details

## Compliance

### Legal Requirements
? Copyright notice updated in all public-facing files  
? License year current and accurate  
? Attribution to original author maintained  
? Consistent across all distribution channels  

### Best Practices
? Year updated at the beginning of the new year  
? Range notation used (2008-2026) to show evolution  
? Original author credit preserved (TUTU)  
? Current maintainer identified (Johan Henningsson)  

## Summary

**Total Files Updated:** 6  
**READMEs Updated:** 3  
**Project Files Updated:** 2  
**Assembly Files Updated:** 1  

**Old Years:**
- 2024 (project files, main README)
- 2025 (AssemblyInfo.cs)

**New Year:** 2026 (all files)

**Status:** ? All copyright years successfully updated to 2026

---

**Updated:** 2026-01-15  
**Verified:** Build successful  
**Next Review:** 2027-01-01 (annual update)
