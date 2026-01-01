# NuGet Package Publishing Status

**Date:** 2025-01-15  
**Status:** ? PACKAGES ALREADY PUBLISHED  
**API Key:** Configured in environment variables

## Current Status

Both NuGet packages are **already published** to NuGet.org:

### Published Packages

| Package Name | Version | Status | URL |
|--------------|---------|--------|-----|
| **NfcReaderLib** | 1.0.0 | ? Published | https://www.nuget.org/packages/NfcReaderLib/1.0.0 |
| **EMVCard.Core** | 1.0.0 | ? Published | https://www.nuget.org/packages/EMVCard.Core/1.0.0 |

## Publishing Attempt Results

### NfcReaderLib 1.0.0
```
? Error: 409 Conflict
Response: A package with ID 'NfcReaderLib' and version '1.0.0' already exists and cannot be modified.
```

### EMVCard.Core 1.0.0
```
? Error: 409 Conflict
Response: A package with ID 'EMVCard.Core' and version '1.0.0' already exists and cannot be modified.
```

## What This Means

? **Good News:** Your packages are already live on NuGet.org!  
? **Protected:** NuGet.org prevents overwriting existing versions (security feature)  
? **Available:** Anyone can install them right now using:
```bash
dotnet add package NfcReaderLib --version 1.0.0
dotnet add package EMVCard.Core --version 1.0.0
```

## API Key Configuration

The NuGet API key has been configured in your environment:

### Current Session
```powershell
$env:NUGET_API_KEY = 'YOUR_API_KEY_HERE'
```

### Permanent Configuration (User Level)
```powershell
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'YOUR_API_KEY_HERE', 'User')
```

**Status:** ? Configured successfully

### Security Recommendation
?? **Important:** The API key is now stored in your user environment variables and will be available in all future PowerShell sessions. Make sure to:
- Keep this key confidential
- Don't commit it to source control
- Rotate it if compromised
- Manage it at: https://www.nuget.org/account/apikeys

## How to Publish New Versions

Since version 1.0.0 is already published, you'll need to increment the version number for any updates.

### Step-by-Step Process

#### 1. Update Version Numbers

**NfcReaderLib\NfcReaderLib.csproj:**
```xml
<PropertyGroup>
  <Version>1.0.1</Version>  <!-- Change from 1.0.0 -->
</PropertyGroup>
```

**EMVCard.Core\EMVCard.Core.csproj:**
```xml
<PropertyGroup>
  <Version>1.0.1</Version>  <!-- Change from 1.0.0 -->
</PropertyGroup>
```

#### 2. Build Release Packages

```powershell
# Build NfcReaderLib
dotnet build "C:\Jobb\EMVReaderSLCard\NfcReaderLib\NfcReaderLib.csproj" -c Release

# Build EMVCard.Core
dotnet build "C:\Jobb\EMVReaderSLCard\EMVCard.Core\EMVCard.Core.csproj" -c Release
```

#### 3. Pack the Packages

```powershell
# Pack NfcReaderLib
dotnet pack "C:\Jobb\EMVReaderSLCard\NfcReaderLib\NfcReaderLib.csproj" -c Release --no-build

# Pack EMVCard.Core
dotnet pack "C:\Jobb\EMVReaderSLCard\EMVCard.Core\EMVCard.Core.csproj" -c Release --no-build
```

#### 4. Publish to NuGet.org

```powershell
# Publish NfcReaderLib (must be first - EMVCard.Core depends on it)
dotnet nuget push "C:\Jobb\EMVReaderSLCard\NfcReaderLib\bin\Release\NfcReaderLib.1.0.1.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Wait 5-10 minutes for NuGet.org to index NfcReaderLib 1.0.1

# Then publish EMVCard.Core
dotnet nuget push "C:\Jobb\EMVReaderSLCard\EMVCard.Core\bin\Release\EMVCard.Core.1.0.1.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

#### 5. Verify Publication

Check your packages at:
- https://www.nuget.org/account/Packages
- https://www.nuget.org/packages/NfcReaderLib
- https://www.nuget.org/packages/EMVCard.Core

## Semantic Versioning Guidelines

Follow these rules when updating version numbers:

| Change Type | Version Update | Example |
|-------------|----------------|---------|
| Bug fixes, minor changes | Patch (X.Y.**Z**) | 1.0.0 ? 1.0.1 |
| New features (backward compatible) | Minor (X.**Y**.0) | 1.0.0 ? 1.1.0 |
| Breaking changes | Major (**X**.0.0) | 1.0.0 ? 2.0.0 |

### Examples

**Patch Release (1.0.0 ? 1.0.1):**
- Bug fixes
- Documentation updates
- Performance improvements
- No new features
- No breaking changes

**Minor Release (1.0.0 ? 1.1.0):**
- New features added
- Backward compatible
- Existing code still works
- No breaking changes

**Major Release (1.0.0 ? 2.0.0):**
- Breaking API changes
- Removed methods/classes
- Changed method signatures
- Requires code updates by consumers

## Quick Reference Commands

### One-Liner to Publish (After Version Bump)

```powershell
# Publish NfcReaderLib
dotnet build "C:\Jobb\EMVReaderSLCard\NfcReaderLib\NfcReaderLib.csproj" -c Release; dotnet pack "C:\Jobb\EMVReaderSLCard\NfcReaderLib\NfcReaderLib.csproj" -c Release --no-build; dotnet nuget push "C:\Jobb\EMVReaderSLCard\NfcReaderLib\bin\Release\NfcReaderLib.*.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Wait 5-10 minutes, then publish EMVCard.Core
dotnet build "C:\Jobb\EMVReaderSLCard\EMVCard.Core\EMVCard.Core.csproj" -c Release; dotnet pack "C:\Jobb\EMVReaderSLCard\EMVCard.Core\EMVCard.Core.csproj" -c Release --no-build; dotnet nuget push "C:\Jobb\EMVReaderSLCard\EMVCard.Core\bin\Release\EMVCard.Core.*.nupkg" --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

### Check API Key

```powershell
# Check if API key is set in current session
if ($env:NUGET_API_KEY) { Write-Host "? API Key is set" } else { Write-Host "? API Key is NOT set" }

# Check if API key is set permanently
$userKey = [System.Environment]::GetEnvironmentVariable('NUGET_API_KEY', 'User')
if ($userKey) { Write-Host "? API Key is set permanently" } else { Write-Host "? API Key is NOT set permanently" }
```

### List Published Packages

```powershell
# View packages in Release folder
Get-ChildItem -Path "C:\Jobb\EMVReaderSLCard" -Recurse -Filter "*.nupkg" | Where-Object { $_.Directory.Name -eq "Release" } | Select-Object Name, Directory, Length, LastWriteTime
```

### Clean Old Packages

```powershell
# Remove old .nupkg files (do this after successful publish)
Remove-Item -Path "C:\Jobb\EMVReaderSLCard\NfcReaderLib\bin\Release\*.nupkg" -Force
Remove-Item -Path "C:\Jobb\EMVReaderSLCard\EMVCard.Core\bin\Release\*.nupkg" -Force
```

## Troubleshooting

### Error: "A package with ID 'X' and version 'Y' already exists"
**Solution:** This is normal! You cannot overwrite published versions. Increment the version number in the `.csproj` file and rebuild.

### Error: "The API key 'X' is invalid"
**Solution:** 
1. Check the API key at https://www.nuget.org/account/apikeys
2. Ensure it has "Push new packages and package versions" permission
3. Regenerate if necessary and update environment variable

### Error: "Response status code does not indicate success: 403 (Forbidden)"
**Solution:** 
- The API key may not have permission to push to these package IDs
- Verify you're the owner of the packages
- Check API key permissions

### Packages Not Appearing After Push
**Solution:** 
- Wait 5-15 minutes for NuGet.org to index the package
- Check package status at https://www.nuget.org/account/Packages
- Verify at https://www.nuget.org/packages/[PackageName]

## Current Package Information

### NfcReaderLib 1.0.0
```
Location: C:\Jobb\EMVReaderSLCard\NfcReaderLib\bin\Release\NfcReaderLib.1.0.0.nupkg
Size: 22,424 bytes
Created: 2026-01-01 12:50:29
Status: ? Published to NuGet.org
```

### EMVCard.Core 1.0.0
```
Location: C:\Jobb\EMVReaderSLCard\EMVCard.Core\bin\Release\EMVCard.Core.1.0.0.nupkg
Size: 22,424 bytes
Created: 2026-01-01 12:50:36
Status: ? Published to NuGet.org
```

## Installation Instructions for Consumers

Once published, users can install your packages using:

### Package Manager Console (Visual Studio)
```powershell
Install-Package NfcReaderLib
Install-Package EMVCard.Core
```

### .NET CLI
```bash
dotnet add package NfcReaderLib
dotnet add package EMVCard.Core
```

### PackageReference (Manual)
```xml
<ItemGroup>
  <PackageReference Include="NfcReaderLib" Version="1.0.0" />
  <PackageReference Include="EMVCard.Core" Version="1.0.0" />
</ItemGroup>
```

## Links

- **NuGet Account:** https://www.nuget.org/account
- **API Keys Management:** https://www.nuget.org/account/apikeys
- **My Packages:** https://www.nuget.org/account/Packages
- **NfcReaderLib Package:** https://www.nuget.org/packages/NfcReaderLib
- **EMVCard.Core Package:** https://www.nuget.org/packages/EMVCard.Core
- **GitHub Repository:** https://github.com/johanhenningsson4-hash/EMVReaderSL

## Summary

? **API Key Configured:** Environment variable set  
? **Packages Built:** Release packages created successfully  
? **Publishing Attempted:** Both packages already exist on NuGet.org  
? **Status:** Version 1.0.0 of both packages is live and available  

**Next Action:** If you want to publish updates, increment the version numbers in the `.csproj` files to 1.0.1 (or higher) and follow the steps above.

---

**Generated:** 2025-01-15  
**API Key:** Configured in User environment  
**Current Published Versions:** NfcReaderLib 1.0.0, EMVCard.Core 1.0.0  
**Status:** ? ALREADY LIVE ON NUGET.ORG
