# NuGet Publishing - Complete Setup ?

## ?? All Files Created Successfully!

Your NfcReaderLib project is now fully configured for NuGet publishing using environment variables.

---

## ?? Quick Navigation

### ?? Getting Started
1. **[QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md)** - Start here! Quick guide to publish in 5 minutes
2. **[SETUP_COMPLETE.md](./SETUP_COMPLETE.md)** - Overview of what's been set up
3. **[ENV_SETUP_TEMPLATES.md](./ENV_SETUP_TEMPLATES.md)** - Copy-paste commands for setting API key

### ?? Documentation
- **[NUGET_PUBLISHING.md](./NUGET_PUBLISHING.md)** - Complete publishing guide with all details
- **[README_ModWinsCard.md](./README_ModWinsCard.md)** - Platform wrapper documentation
- **[MIGRATION_SUMMARY.md](./MIGRATION_SUMMARY.md)** - 32/64-bit migration details

### ?? Security
- **[INSTALL_GIT_HOOKS.md](./INSTALL_GIT_HOOKS.md)** - Prevent committing secrets
- **[.gitignore](./.gitignore)** - Ignore sensitive files

### ??? Scripts
- **[publish-nuget.ps1](./publish-nuget.ps1)** - PowerShell (recommended)
- **[publish-nuget.cmd](./publish-nuget.cmd)** - Windows CMD
- **[publish-nuget.sh](./publish-nuget.sh)** - Linux/macOS

### ?? CI/CD
- **[.github/workflows/publish-nuget.yml](./.github/workflows/publish-nuget.yml)** - GitHub Actions

---

## ?? Choose Your Publishing Method

### Method 1: GitHub Actions (Automated) ? Recommended for CI/CD

**Setup Once:**
1. Get API key from https://www.nuget.org/account/apikeys
2. Add to GitHub: Repo ? Settings ? Secrets ? New secret
   - Name: `NUGET_API_KEY`
   - Value: [your key]

**Publish:**
```bash
git tag v1.0.2
git push origin v1.0.2
```

?? **Full Guide**: [QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md#method-1-github-actions-recommended-for-cicd)

---

### Method 2: PowerShell Script (Local) ? Recommended for Manual

**Setup:**
```powershell
$env:NUGET_API_KEY = "your-api-key-here"
```

**Publish:**
```powershell
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib
./publish-nuget.ps1
```

?? **Full Guide**: [QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md#method-2-local-publishing-quick--easy)

---

### Method 3: Command Line (Manual)

```powershell
$env:NUGET_API_KEY = "your-key"
dotnet build --configuration Release
dotnet pack --configuration Release --output ./nupkg
dotnet nuget push ./nupkg/*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```

?? **Full Guide**: [QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md#method-3-manual-step-by-step)

---

## ?? Pre-Publishing Checklist

Before your first publish:

- [ ] Get NuGet API key: https://www.nuget.org/account/apikeys
- [ ] Set environment variable: See [ENV_SETUP_TEMPLATES.md](./ENV_SETUP_TEMPLATES.md)
- [ ] Install Git hook (optional): See [INSTALL_GIT_HOOKS.md](./INSTALL_GIT_HOOKS.md)
- [ ] Test build: `dotnet build`
- [ ] Update version if needed (currently `1.0.2`)

---

## ?? Setting Your API Key

### Windows PowerShell (Temporary)
```powershell
$env:NUGET_API_KEY = "your-api-key-here"
```

### Windows PowerShell (Permanent)
```powershell
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'your-api-key-here', 'User')
```

### Verify It's Set
```powershell
echo $env:NUGET_API_KEY
```

?? **More Options**: [ENV_SETUP_TEMPLATES.md](./ENV_SETUP_TEMPLATES.md)

---

## ?? Package Information

| Property | Value |
|----------|-------|
| **Package ID** | NfcReaderLib |
| **Version** | 1.0.2 |
| **Platform** | .NET Framework 4.7.2 |
| **Repository** | https://github.com/johanhenningsson4-hash/EMVReaderSL |
| **NuGet Page** | https://www.nuget.org/packages/NfcReaderLib |

### What's New in v1.0.2
- ? Platform-independent wrapper (`ModWinsCard`)
- ? Automatic 32-bit/64-bit detection
- ? Updated `EmvCardReader` to use `IntPtr`
- ? Enhanced documentation
- ? NuGet publishing workflow

---

## ?? Quick Start (30 Seconds)

```powershell
# 1. Set API key
$env:NUGET_API_KEY = "paste-your-key-here"

# 2. Navigate to project
cd C:\Jobb\EMVReaderSLCard\NfcReaderLib

# 3. Publish
./publish-nuget.ps1

# Done! ?
```

---

## ??? Security Features

### Included Protection
- ? `.gitignore` - Prevents committing secrets
- ? Pre-commit hook - Detects secrets before commit
- ? Environment variable usage - No hardcoded keys
- ? GitHub Secrets support - Secure CI/CD

### How to Install Git Hook
```powershell
Copy-Item .git-hooks\pre-commit .git\hooks\pre-commit
```

?? **Full Guide**: [INSTALL_GIT_HOOKS.md](./INSTALL_GIT_HOOKS.md)

---

## ?? File Structure

```
NfcReaderLib/
??? ?? NfcReaderLib.csproj          # Project file (updated to v1.0.2)
??? ?? publish-nuget.ps1            # PowerShell publishing script
??? ?? publish-nuget.cmd            # Windows CMD script
??? ?? publish-nuget.sh             # Linux/macOS script
??? ?? .gitignore                   # Ignore secrets and artifacts
?
??? ?? Documentation/
?   ??? QUICKSTART_NUGET.md         # Quick start guide
?   ??? NUGET_PUBLISHING.md         # Complete guide
?   ??? SETUP_COMPLETE.md           # Setup summary
?   ??? ENV_SETUP_TEMPLATES.md      # Environment variable templates
?   ??? INSTALL_GIT_HOOKS.md        # Git hook installation
?   ??? README_ModWinsCard.md       # Platform wrapper docs
?   ??? MIGRATION_SUMMARY.md        # Migration details
?
??? ?? Security/
?   ??? .git-hooks/
?       ??? pre-commit              # Secret detection hook
?
??? ?? CI/CD/
    ??? .github/
        ??? workflows/
            ??? publish-nuget.yml   # GitHub Actions workflow
```

---

## ?? Learning Path

### New to NuGet Publishing?
1. Read: [QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md)
2. Set environment variable: [ENV_SETUP_TEMPLATES.md](./ENV_SETUP_TEMPLATES.md)
3. Run: `./publish-nuget.ps1`

### Want Full Control?
1. Read: [NUGET_PUBLISHING.md](./NUGET_PUBLISHING.md)
2. Review: [SETUP_COMPLETE.md](./SETUP_COMPLETE.md)
3. Customize scripts as needed

### Setting Up CI/CD?
1. Read: [GitHub Actions section](./QUICKSTART_NUGET.md#method-1-github-actions-recommended-for-cicd)
2. Add GitHub Secret: `NUGET_API_KEY`
3. Push tag: `git tag v1.0.2 && git push origin v1.0.2`

---

## ?? Testing

After publishing, test the package:

```powershell
# Create test project
dotnet new console -n TestNfcReader
cd TestNfcReader

# Install your package
dotnet add package NfcReaderLib

# Test it
# Edit Program.cs:
# using NfcReaderLib;
# Console.WriteLine(ModWinsCard.GetPlatformInfo());

dotnet run
```

---

## ? Troubleshooting

| Issue | Solution | Reference |
|-------|----------|-----------|
| "API key is invalid" | Check key is copied correctly | [QUICKSTART](./QUICKSTART_NUGET.md#troubleshooting) |
| "Package already exists" | Increment version in `.csproj` | [NUGET_PUBLISHING](./NUGET_PUBLISHING.md#version-management) |
| "Unauthorized" | Verify "Push" permission | [SETUP_COMPLETE](./SETUP_COMPLETE.md#getting-your-nuget-api-key) |
| Environment variable not set | Restart terminal | [ENV_SETUP](./ENV_SETUP_TEMPLATES.md#verification-commands) |

---

## ?? Getting Help

- **Documentation**: All `.md` files in this directory
- **Issues**: https://github.com/johanhenningsson4-hash/EMVReaderSL/issues
- **NuGet Docs**: https://docs.microsoft.com/nuget/

---

## ? Next Steps

1. **Get API Key**: https://www.nuget.org/account/apikeys
2. **Set Environment Variable**: Use [ENV_SETUP_TEMPLATES.md](./ENV_SETUP_TEMPLATES.md)
3. **Choose Publishing Method**: See options above
4. **Publish**: Run your chosen method
5. **Verify**: Check https://www.nuget.org/packages/NfcReaderLib
6. **Test**: Install in a test project

---

## ?? You're Ready!

Everything is configured and ready to go. Just add your NuGet API key and publish!

**Recommended first-time flow:**
1. Read [QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md)
2. Set `NUGET_API_KEY` environment variable
3. Run `./publish-nuget.ps1`

**Good luck with your publishing! ??**

---

*Generated for NfcReaderLib v1.0.2*
