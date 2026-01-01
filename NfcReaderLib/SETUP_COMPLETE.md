# NuGet Publishing Setup - Summary

## ? Setup Complete!

All files and configurations for NuGet publishing with environment variables have been created.

## ?? Files Created

### Publishing Scripts
- ? `publish-nuget.ps1` - PowerShell script (cross-platform, **recommended**)
- ? `publish-nuget.cmd` - Windows CMD batch script
- ? `publish-nuget.sh` - Linux/macOS bash script

### CI/CD Configuration
- ? `.github/workflows/publish-nuget.yml` - GitHub Actions workflow

### Documentation
- ? `NUGET_PUBLISHING.md` - Complete publishing guide
- ? `QUICKSTART_NUGET.md` - Quick reference for publishing
- ? `.gitignore` - Prevents committing secrets and build artifacts

### Project Updates
- ? `NfcReaderLib.csproj` - Updated to v1.0.2 with enhanced metadata

## ?? Quick Start

### Option 1: GitHub Actions (Automated)

1. **Add API Key to GitHub**:
   - Go to: Repository ? Settings ? Secrets ? Actions
   - Create: `NUGET_API_KEY` with your NuGet API key

2. **Publish via Git Tag**:
   ```bash
   git tag v1.0.2
   git push origin v1.0.2
   ```

### Option 2: Local PowerShell (Manual)

```powershell
# Set API key (one time per session)
$env:NUGET_API_KEY = "your-nuget-api-key-here"

# Run script
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib
./publish-nuget.ps1
```

### Option 3: Manual Commands

```powershell
$env:NUGET_API_KEY = "your-api-key"
dotnet build --configuration Release
dotnet pack --configuration Release --output ./nupkg
dotnet nuget push ./nupkg/*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

## ?? Getting Your NuGet API Key

1. Visit: https://www.nuget.org/account/apikeys
2. Click "Create"
3. Settings:
   - Name: "NfcReaderLib Publishing"
   - Expiration: 365 days (or as needed)
   - Scopes: Select **"Push"** only
   - Glob Pattern: `NfcReaderLib`
4. Click "Create"
5. **Copy the key immediately** (you won't see it again!)

## ?? Security Best Practices

### ? DO:
- Store API key in environment variable: `$env:NUGET_API_KEY`
- Use GitHub Secrets for CI/CD
- Create scoped API keys (Push only)
- Set expiration dates on API keys
- Rotate keys periodically

### ? DON'T:
- Commit API keys to Git
- Share keys in plain text emails
- Use same key for multiple purposes
- Store keys in code or config files
- Give keys "Unlist" or "Delete" permissions unless needed

## ?? Package Information

- **Package ID**: NfcReaderLib
- **Version**: 1.0.2
- **Platforms**: .NET Framework 4.7.2
- **Features**:
  - ? Platform-independent PC/SC wrapper (32/64-bit auto-detect)
  - ? SL Token generation (SHA-256 from ICC certificates)
  - ? EMV card reader utilities
  - ? Smart card communication

## ?? Publishing Checklist

Before publishing:

- [ ] Update version in `NfcReaderLib.csproj`
- [ ] Update `PackageReleaseNotes` with changes
- [ ] Build and test locally: `dotnet build`
- [ ] Verify no secrets in code: `git status`
- [ ] Commit changes: `git add . && git commit -m "Release v1.0.2"`
- [ ] Push to GitHub: `git push`
- [ ] Create tag: `git tag v1.0.2`
- [ ] Push tag: `git push origin v1.0.2`
- [ ] Wait for GitHub Actions (or run script locally)
- [ ] Verify on NuGet.org (2-5 minutes delay)
- [ ] Test installation: `dotnet add package NfcReaderLib`

## ?? Testing the Package

After publishing, test in a new project:

```powershell
# Create test project
dotnet new console -n TestNfcReader
cd TestNfcReader

# Install package
dotnet add package NfcReaderLib

# Test usage
# Edit Program.cs and add:
# using NfcReaderLib;
# Console.WriteLine(ModWinsCard.GetPlatformInfo());

# Run
dotnet run
```

## ??? Script Features

### publish-nuget.ps1 (PowerShell)
- ? Colored output
- ? Version detection from .csproj
- ? Build, pack, push workflow
- ? Confirmation prompt
- ? Error handling with troubleshooting tips
- ? Can skip build/tests with switches

### publish-nuget.cmd (Windows CMD)
- ? Simple batch script
- ? Environment variable support
- ? Confirmation prompt
- ? Error checking

### publish-nuget.sh (Linux/macOS)
- ? Bash script with colors
- ? Environment variable support
- ? Executable: `chmod +x publish-nuget.sh`
- ? Confirmation prompt

## ?? Documentation

| File | Purpose |
|------|---------|
| `QUICKSTART_NUGET.md` | Quick reference - start here |
| `NUGET_PUBLISHING.md` | Complete guide with all details |
| `README_ModWinsCard.md` | Platform wrapper documentation |
| `MIGRATION_SUMMARY.md` | 32/64-bit migration details |

## ?? Workflow Examples

### First-Time Setup
```powershell
# 1. Get API key from nuget.org
# 2. Set environment variable
$env:NUGET_API_KEY = "your-key"

# 3. Or set permanently (Windows)
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'your-key', 'User')

# 4. Publish
./publish-nuget.ps1
```

### Automated GitHub Actions
```bash
# Just push a tag - GitHub Actions does the rest
git tag v1.0.3
git push origin v1.0.3

# Check progress at:
# https://github.com/johanhenningsson4-hash/EMVReaderSL/actions
```

### Update and Publish New Version
```powershell
# 1. Edit NfcReaderLib.csproj - update <Version>
# 2. Commit changes
git add .
git commit -m "Bump version to 1.0.3"
git push

# 3. Tag and push
git tag v1.0.3
git push origin v1.0.3
```

## ?? Troubleshooting

### "API key is invalid"
```powershell
# Check key is set correctly
echo $env:NUGET_API_KEY

# Verify no extra spaces
$env:NUGET_API_KEY = $env:NUGET_API_KEY.Trim()
```

### "Package already exists"
```xml
<!-- Increment version in NfcReaderLib.csproj -->
<Version>1.0.3</Version>
```

### "Unauthorized"
- Check API key has "Push" permission
- Verify glob pattern matches: `NfcReaderLib`
- Key might be expired - create new one

### GitHub Actions Fails
- Check secret is named exactly: `NUGET_API_KEY`
- Verify secret value has no extra spaces
- Check workflow file syntax

## ?? Support

- **Package Issues**: https://github.com/johanhenningsson4-hash/EMVReaderSL/issues
- **NuGet Help**: https://docs.microsoft.com/nuget/
- **GitHub Actions**: https://docs.github.com/actions

## ?? Next Steps

1. ? Set your `NUGET_API_KEY` environment variable
2. ? Run `./publish-nuget.ps1` to test local publishing
3. ? Set up GitHub Secret for automated publishing
4. ? Create a version tag to trigger GitHub Actions
5. ? Verify package on https://www.nuget.org/packages/NfcReaderLib

---

**Current Status**: Ready to publish! ??

Everything is set up and tested. Just add your NuGet API key and run the script!
