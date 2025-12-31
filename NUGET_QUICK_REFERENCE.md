# NuGet Publishing Quick Reference

## ?? Quick Publish Commands

### One-Line Publishing (from project root)
```powershell
# NfcReaderLib
dotnet build NfcReaderLib\NfcReaderLib.csproj -c Release; dotnet nuget push "NfcReaderLib\bin\Release\NfcReaderLib.*.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# EMVCard.Core
dotnet build EMVCard.Core\EMVCard.Core.csproj -c Release; dotnet nuget push "EMVCard.Core\bin\Release\EMVCard.Core.*.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

## ?? Pre-Publishing Checklist

- [ ] Update version number in `.csproj` file
- [ ] Update CHANGELOG or release notes
- [ ] Update README if needed
- [ ] Build in Release configuration
- [ ] Run tests (if available)
- [ ] Verify package contents
- [ ] Check dependencies are correct
- [ ] Commit all changes to Git
- [ ] Tag the release in Git

## ?? Version Management

### Current Versions
- **NfcReaderLib:** 1.0.0
- **EMVCard.Core:** 1.0.0

### Update Version
Edit the `.csproj` file:
```xml
<Version>1.0.1</Version>
```

### Semantic Versioning
- **1.0.X** - Bug fixes (patch)
- **1.X.0** - New features (minor)
- **X.0.0** - Breaking changes (major)

## ?? API Key Setup

```powershell
# Set environment variable (current session)
$env:NUGET_API_KEY = "your-key-here"

# Set permanently (Windows)
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'your-key-here', 'User')
```

## ?? Package Locations

### After Build
```
NfcReaderLib\bin\Release\NfcReaderLib.X.Y.Z.nupkg
EMVCard.Core\bin\Release\EMVCard.Core.X.Y.Z.nupkg
```

## ?? Verify Package Contents

```powershell
# List contents of NuGet package
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead("path\to\package.nupkg")
$zip.Entries | Select-Object FullName
$zip.Dispose()
```

## ?? Published Package URLs

- **NfcReaderLib:** https://www.nuget.org/packages/NfcReaderLib
- **EMVCard.Core:** https://www.nuget.org/packages/EMVCard.Core

## ??? Troubleshooting

### Package Already Exists
- You cannot overwrite existing versions
- Increment version number and republish

### API Key Invalid
- Regenerate key at https://www.nuget.org/account/apikeys
- Ensure key has "Push new packages and package versions" permission

### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet build -c Release
```

### Package Not Appearing
- Wait 5-15 minutes for indexing
- Check package status at https://www.nuget.org/account/Packages

## ?? Useful Commands

### List All NuGet Packages
```powershell
Get-ChildItem -Path . -Recurse -Filter "*.nupkg" | Where-Object { $_.FullName -notlike "*\.nuget\*" }
```

### Delete Old Packages
```powershell
Remove-Item -Path "NfcReaderLib\bin\Release\*.nupkg"
Remove-Item -Path "EMVCard.Core\bin\Release\*.nupkg"
```

### Test Package Installation
```powershell
# Create test project
mkdir TestProject
cd TestProject
dotnet new console
dotnet add package NfcReaderLib --version 1.0.0
dotnet add package EMVCard.Core --version 1.0.0
```

## ?? Publishing Workflow

1. **Make changes** to code
2. **Test** functionality
3. **Update version** in `.csproj`
4. **Update documentation** (README, CHANGELOG)
5. **Commit to Git**
   ```bash
   git add .
   git commit -m "Release v1.0.1"
   git tag v1.0.1
   git push origin main --tags
   ```
6. **Build Release**
   ```powershell
   dotnet build -c Release
   ```
7. **Publish to NuGet**
   ```powershell
   dotnet nuget push "path\to\package.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```
8. **Verify on NuGet.org**
9. **Announce release** (GitHub Release, Twitter, etc.)

## ?? Quick Links

- [NuGet Account](https://www.nuget.org/account)
- [API Keys](https://www.nuget.org/account/apikeys)
- [My Packages](https://www.nuget.org/account/Packages)
- [NuGet Documentation](https://docs.microsoft.com/nuget/)
- [GitHub Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)

---

**Last Updated:** 2025-12-31  
**Published Versions:** NfcReaderLib 1.0.0, EMVCard.Core 1.0.0
