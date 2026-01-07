# NuGet Package Release v1.0.3/v1.0.2 - 32-bit/64-bit Support

**Date:** January 1, 2026  
**Status:** ? PACKAGES BUILT - READY FOR PUBLISHING  
**Version:** NfcReaderLib 1.0.3 / EMVCard.Core 1.0.2

## Release Summary

This release adds complete 32-bit and 64-bit Windows platform support through a platform-independent wrapper architecture.

### Packages Updated

1. **NfcReaderLib v1.0.3**
   - Complete 32-bit/64-bit platform support
   - Platform-independent ModWinsCard wrapper
   - ModWinsCard32 for x86 systems
   - ModWinsCard64 for x64 systems
   - EmvCardReader migrated to IntPtr
   - All NuGet dependencies verified up to date

2. **EMVCard.Core v1.0.2**
   - Updated to depend on NfcReaderLib 1.0.3
   - Inherits 32-bit/64-bit platform support
   - Enhanced platform compatibility
   - All NuGet dependencies verified

## Build Results

### NfcReaderLib 1.0.3
```
Build Status: ? SUCCESS
Build Time: 2.1 seconds
Output: NfcReaderLib\bin\Release\net472\NfcReaderLib.dll
Package: NfcReaderLib.1.0.3.nupkg (31,092 bytes)
Warnings: 0
Errors: 0
```

### EMVCard.Core 1.0.2
```
Build Status: ? SUCCESS
Build Time: 2.3 seconds
Output: EMVCard.Core\bin\Release\net472\EMVCard.Core.dll
Package: EMVCard.Core.1.0.2.nupkg (23,262 bytes)
Warnings: 0
Errors: 0
```

## Package Details

### NfcReaderLib 1.0.3

**File:** `NfcReaderLib.1.0.3.nupkg`  
**Size:** 31,092 bytes  
**Location:** `C:\Jobb\EMVReaderSLCard\NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg`

**Release Notes:**
```
v1.0.3 - Complete 32-bit/64-bit platform support. Includes ModWinsCard 
platform-independent wrapper, ModWinsCard32 for 32-bit systems, and 
ModWinsCard64 for 64-bit systems. EmvCardReader migrated to IntPtr for 
full cross-platform compatibility. All PC/SC operations now work seamlessly 
on both x86 and x64 Windows. Verified all NuGet dependencies up to date 
with no security vulnerabilities.
```

**Changes:**
- ? Version: 1.0.2 ? 1.0.3
- ? Added ModWinsCard platform-independent wrapper
- ? Added ModWinsCard32 for 32-bit systems
- ? Added ModWinsCard64 for 64-bit systems
- ? EmvCardReader migrated to use IntPtr
- ? Automatic platform detection (x86/x64)
- ? Verified all dependencies up to date
- ? No security vulnerabilities

### EMVCard.Core 1.0.2

**File:** `EMVCard.Core.1.0.2.nupkg`  
**Size:** 23,262 bytes  
**Location:** `C:\Jobb\EMVReaderSLCard\EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg`

**Release Notes:**
```
v1.0.2 - Updated to depend on NfcReaderLib 1.0.3 with complete 32-bit/64-bit 
platform support. Now works seamlessly on both x86 and x64 Windows systems. 
All NuGet dependencies verified up to date with no security vulnerabilities. 
Enhanced documentation and improved platform compatibility.
```

**Changes:**
- ? Version: 1.0.1 ? 1.0.2
- ? Updated dependency: NfcReaderLib 1.0.2 ? 1.0.3
- ? Inherits 32-bit/64-bit platform support
- ? Added 32bit and 64bit package tags
- ? Enhanced description with platform support
- ? Verified all dependencies up to date
- ? No security vulnerabilities

## Publishing to NuGet.org

### Prerequisites

1. **NuGet API Key Required**
   - Obtain from: https://www.nuget.org/account/apikeys
   - Ensure key has push permissions for both packages

2. **Set Environment Variable**

**Windows PowerShell:**
```powershell
$env:NUGET_API_KEY = "your-api-key-here"
```

**Windows Command Prompt:**
```cmd
set NUGET_API_KEY=your-api-key-here
```

**Persistent (System-wide):**
```powershell
# Run as Administrator
[System.Environment]::SetEnvironmentVariable("NUGET_API_KEY", "your-api-key-here", "User")
```

### Publishing Commands

#### Option 1: Using Environment Variable (Recommended)

**Publish NfcReaderLib 1.0.3:**
```powershell
cd C:\Jobb\EMVReaderSLCard
dotnet nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json
```

**Publish EMVCard.Core 1.0.2:**
```powershell
dotnet nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json
```

#### Option 2: Direct API Key (Less Secure)

```powershell
# NfcReaderLib
dotnet nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# EMVCard.Core
dotnet nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

#### Option 3: Using NuGet CLI

```powershell
# NfcReaderLib
nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg %NUGET_API_KEY% -Source https://api.nuget.org/v3/index.json

# EMVCard.Core
nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg %NUGET_API_KEY% -Source https://api.nuget.org/v3/index.json
```

### Complete Publishing Script

**PowerShell Script (publish-nuget.ps1):**
```powershell
# Check if API key is set
if (-not $env:NUGET_API_KEY) {
    Write-Error "NUGET_API_KEY environment variable not set!"
    Write-Host "Set it with: `$env:NUGET_API_KEY = 'your-api-key'"
    exit 1
}

# Set location
Set-Location C:\Jobb\EMVReaderSLCard

Write-Host "Publishing NfcReaderLib 1.0.3..." -ForegroundColor Cyan
dotnet nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg `
    --api-key $env:NUGET_API_KEY `
    --source https://api.nuget.org/v3/index.json

if ($LASTEXITCODE -eq 0) {
    Write-Host "? NfcReaderLib 1.0.3 published successfully!" -ForegroundColor Green
} else {
    Write-Error "? Failed to publish NfcReaderLib 1.0.3"
    exit 1
}

Write-Host "`nPublishing EMVCard.Core 1.0.2..." -ForegroundColor Cyan
dotnet nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg `
    --api-key $env:NUGET_API_KEY `
    --source https://api.nuget.org/v3/index.json

if ($LASTEXITCODE -eq 0) {
    Write-Host "? EMVCard.Core 1.0.2 published successfully!" -ForegroundColor Green
    Write-Host "`n?? All packages published successfully!" -ForegroundColor Green
} else {
    Write-Error "? Failed to publish EMVCard.Core 1.0.2"
    exit 1
}

Write-Host "`nPackages will be available at:" -ForegroundColor Yellow
Write-Host "  https://www.nuget.org/packages/NfcReaderLib/1.0.3"
Write-Host "  https://www.nuget.org/packages/EMVCard.Core/1.0.2"
```

**Usage:**
```powershell
# Set API key first
$env:NUGET_API_KEY = "your-api-key-here"

# Run the script
.\publish-nuget.ps1
```

### Batch Script (publish-nuget.bat)

```batch
@echo off
setlocal enabledelayedexpansion

REM Check if API key is set
if "%NUGET_API_KEY%"=="" (
    echo ERROR: NUGET_API_KEY environment variable not set!
    echo Set it with: set NUGET_API_KEY=your-api-key
    exit /b 1
)

cd /d C:\Jobb\EMVReaderSLCard

echo Publishing NfcReaderLib 1.0.3...
dotnet nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json

if %ERRORLEVEL% neq 0 (
    echo Failed to publish NfcReaderLib 1.0.3
    exit /b 1
)

echo Successfully published NfcReaderLib 1.0.3
echo.

echo Publishing EMVCard.Core 1.0.2...
dotnet nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json

if %ERRORLEVEL% neq 0 (
    echo Failed to publish EMVCard.Core 1.0.2
    exit /b 1
)

echo Successfully published EMVCard.Core 1.0.2
echo.
echo All packages published successfully!
echo.
echo Packages will be available at:
echo   https://www.nuget.org/packages/NfcReaderLib/1.0.3
echo   https://www.nuget.org/packages/EMVCard.Core/1.0.2

endlocal
```

## Verification After Publishing

### Check Package Status

**Wait 5-10 minutes for indexing, then verify:**

```powershell
# Check NfcReaderLib
dotnet nuget list NfcReaderLib --source https://api.nuget.org/v3/index.json

# Check EMVCard.Core
dotnet nuget list EMVCard.Core --source https://api.nuget.org/v3/index.json
```

### Test Installation

**Create a test project and install:**
```powershell
mkdir test-install
cd test-install
dotnet new console -f net472
dotnet add package NfcReaderLib --version 1.0.3
dotnet add package EMVCard.Core --version 1.0.2
dotnet restore
```

### Browse Packages

- **NfcReaderLib:** https://www.nuget.org/packages/NfcReaderLib/1.0.3
- **EMVCard.Core:** https://www.nuget.org/packages/EMVCard.Core/1.0.2

## Security Best Practices

### API Key Management

1. **Never commit API keys to Git**
   ```powershell
   # Add to .gitignore
   echo "**/nuget-api-key.txt" >> .gitignore
   echo "publish-config.ps1" >> .gitignore
   ```

2. **Use environment variables**
   - Temporary: `$env:NUGET_API_KEY = "key"`
   - Persistent: System Environment Variables

3. **Rotate keys regularly**
   - Create new keys at https://www.nuget.org/account/apikeys
   - Delete old keys after rotation

4. **Use scoped keys**
   - Create separate keys for different packages
   - Set expiration dates
   - Limit permissions (push only, specific packages)

### CI/CD Integration

**GitHub Actions Example:**
```yaml
name: Publish NuGet Packages

on:
  release:
    types: [created]

jobs:
  publish:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '7.0.x'
      
      - name: Build NfcReaderLib
        run: dotnet build NfcReaderLib\NfcReaderLib.csproj -c Release
      
      - name: Build EMVCard.Core
        run: dotnet build EMVCard.Core\EMVCard.Core.csproj -c Release
      
      - name: Pack NfcReaderLib
        run: dotnet pack NfcReaderLib\NfcReaderLib.csproj -c Release --no-build
      
      - name: Pack EMVCard.Core
        run: dotnet pack EMVCard.Core\EMVCard.Core.csproj -c Release --no-build
      
      - name: Publish NfcReaderLib
        run: dotnet nuget push NfcReaderLib\bin\Release\*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
        
      - name: Publish EMVCard.Core
        run: dotnet nuget push EMVCard.Core\bin\Release\*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

## Troubleshooting

### Common Issues

**1. "403 Forbidden - API key is invalid"**
- Check API key is correct
- Verify key has push permissions
- Ensure key hasn't expired
- Check package ownership

**2. "409 Conflict - Package version already exists"**
- Version 1.0.3/1.0.2 already published
- Increment version number
- Cannot overwrite existing versions

**3. "Environment variable not set"**
```powershell
# Check if set
echo $env:NUGET_API_KEY

# Set if not present
$env:NUGET_API_KEY = "your-key"
```

**4. "Package validation failed"**
- Check package metadata in .csproj
- Ensure all required files included
- Verify package builds without errors

### Getting Help

- **NuGet Documentation:** https://docs.microsoft.com/en-us/nuget/
- **API Keys:** https://www.nuget.org/account/apikeys
- **Package Management:** https://www.nuget.org/account/Packages
- **Support:** https://www.nuget.org/policies/Contact

## Release Checklist

### Pre-Publishing

- [x] Update version numbers (NfcReaderLib 1.0.3, EMVCard.Core 1.0.2)
- [x] Update release notes in .csproj files
- [x] Build both projects in Release mode
- [x] Create NuGet packages
- [x] Verify package files created
- [x] Test packages locally (optional)

### Publishing

- [ ] Set NUGET_API_KEY environment variable
- [ ] Publish NfcReaderLib 1.0.3
- [ ] Publish EMVCard.Core 1.0.2
- [ ] Wait for NuGet.org indexing (5-10 minutes)
- [ ] Verify packages appear on NuGet.org

### Post-Publishing

- [ ] Test installation from NuGet.org
- [ ] Update README with new version numbers
- [ ] Create Git tag (v1.0.3)
- [ ] Create GitHub release
- [ ] Update documentation
- [ ] Announce release

## Installation Instructions for Users

After publishing, users can install with:

**Package Manager Console:**
```powershell
Install-Package NfcReaderLib -Version 1.0.3
Install-Package EMVCard.Core -Version 1.0.2
```

**.NET CLI:**
```bash
dotnet add package NfcReaderLib --version 1.0.3
dotnet add package EMVCard.Core --version 1.0.2
```

**PackageReference:**
```xml
<ItemGroup>
  <PackageReference Include="NfcReaderLib" Version="1.0.3" />
  <PackageReference Include="EMVCard.Core" Version="1.0.2" />
</ItemGroup>
```

## Summary

**Packages Built:**
- ? NfcReaderLib 1.0.3 (31,092 bytes)
- ? EMVCard.Core 1.0.2 (23,262 bytes)

**Status:**
- ? Both packages built successfully
- ? Zero warnings, zero errors
- ? Ready for publishing
- ? Awaiting API key configuration

**To Publish:**
```powershell
# 1. Set API key
$env:NUGET_API_KEY = "your-api-key-here"

# 2. Publish NfcReaderLib
dotnet nuget push NfcReaderLib\bin\Release\NfcReaderLib.1.0.3.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# 3. Publish EMVCard.Core
dotnet nuget push EMVCard.Core\bin\Release\EMVCard.Core.1.0.2.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

---

**Document Created:** January 1, 2026  
**Status:** ? PACKAGES READY FOR PUBLISHING  
**Next Step:** Set NUGET_API_KEY and run publishing commands
