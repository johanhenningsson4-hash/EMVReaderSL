# Quick Start: Publishing to NuGet

## Prerequisites

1. **NuGet Account**: Create one at https://www.nuget.org
2. **API Key**: Get from https://www.nuget.org/account/apikeys
   - Click "Create"
   - Name: "NfcReaderLib Publishing"
   - Select "Push" only
   - Glob Pattern: `NfcReaderLib`
   - Copy the generated key (you won't see it again!)

## Method 1: GitHub Actions (Recommended for CI/CD)

### One-Time Setup

1. **Add secret to GitHub**:
   ```
   GitHub Repo ? Settings ? Secrets and variables ? Actions ? New repository secret
   Name: NUGET_API_KEY
   Value: [paste your key]
   ```

2. **Push a version tag**:
   ```bash
   git tag v1.0.2
   git push origin v1.0.2
   ```

3. **Done!** GitHub Actions automatically builds and publishes.

### Manual Trigger
```
GitHub Repo ? Actions ? Publish to NuGet ? Run workflow
```

## Method 2: Local Publishing (Quick & Easy)

### Windows PowerShell (Recommended)

```powershell
# 1. Set API key (one time per session)
$env:NUGET_API_KEY = "paste-your-key-here"

# 2. Run the script
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib
./publish-nuget.ps1
```

### Windows CMD

```cmd
# 1. Set API key
set NUGET_API_KEY=paste-your-key-here

# 2. Run the script
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib
publish-nuget.cmd
```

### Linux/macOS

```bash
# 1. Set API key
export NUGET_API_KEY="paste-your-key-here"

# 2. Make script executable (one time only)
chmod +x publish-nuget.sh

# 3. Run the script
./publish-nuget.sh
```

## Method 3: Manual Step-by-Step

```powershell
# Set API key
$env:NUGET_API_KEY = "your-key"

# Navigate to project
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib

# Build
dotnet build --configuration Release

# Pack
dotnet pack --configuration Release --output ./nupkg

# Push
dotnet nuget push ./nupkg/*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

## Updating Version

Edit `NfcReaderLib.csproj`:

```xml
<Version>1.0.3</Version>
<PackageReleaseNotes>v1.0.3 - Your changes here</PackageReleaseNotes>
```

## Verification

After publishing:

1. **Wait 2-5 minutes** for indexing
2. **Check**: https://www.nuget.org/packages/NfcReaderLib
3. **Test installation**:
   ```powershell
   dotnet add package NfcReaderLib
   ```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "API key is invalid" | Verify key is copied correctly (no spaces) |
| "Package already exists" | Increment version number in `.csproj` |
| "Authentication failed" | Restart terminal after setting env var |
| "Unauthorized" | Check API key has "Push" permission |

## Files Overview

```
NfcReaderLib/
??? publish-nuget.ps1      # PowerShell script (recommended)
??? publish-nuget.cmd      # Windows CMD script
??? publish-nuget.sh       # Linux/macOS script
??? NUGET_PUBLISHING.md    # Detailed documentation
??? .github/
    ??? workflows/
        ??? publish-nuget.yml  # GitHub Actions workflow
```

## Security Tips

? **DO**:
- Use environment variables
- Store keys in GitHub Secrets
- Use scoped API keys

? **DON'T**:
- Commit keys to Git
- Share keys in plain text
- Reuse keys across projects

## Need Help?

See full documentation: `NUGET_PUBLISHING.md`
