# NuGet Publishing with Environment Variables

This document explains how to publish the NfcReaderLib package to NuGet using environment variables for secure credential management.

## Setup Instructions

### Option 1: GitHub Actions (Recommended)

1. **Get your NuGet API Key**:
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push" permission
   - Copy the generated key

2. **Add the API Key to GitHub Secrets**:
   - Go to your GitHub repository
   - Navigate to Settings ? Secrets and variables ? Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click "Add secret"

3. **Publish via GitHub Actions**:
   
   **Automatic (via Git tag)**:
   ```bash
   git tag v1.0.2
   git push origin v1.0.2
   ```
   
   **Manual trigger**:
   - Go to Actions tab in GitHub
   - Select "Publish to NuGet" workflow
   - Click "Run workflow"

### Option 2: Local Publishing with Environment Variable

#### Windows (PowerShell)

1. **Set environment variable temporarily** (current session only):
   ```powershell
   $env:NUGET_API_KEY = "your-api-key-here"
   ```

2. **Or set it permanently** (for current user):
   ```powershell
   [System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'your-api-key-here', 'User')
   ```

3. **Build and publish**:
   ```powershell
   # Navigate to project directory
   cd C:\Jobb\EMVReaderSLCard\NfcReaderLib

   # Build the project
   dotnet build --configuration Release

   # Pack the NuGet package
   dotnet pack --configuration Release --output ./nupkg

   # Publish to NuGet using environment variable
   dotnet nuget push ./nupkg/*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```

#### Windows (CMD)

1. **Set environment variable**:
   ```cmd
   set NUGET_API_KEY=your-api-key-here
   ```

2. **Build and publish**:
   ```cmd
   cd C:\Jobb\EMVReaderSLCard\NfcReaderLib
   dotnet build --configuration Release
   dotnet pack --configuration Release --output ./nupkg
   dotnet nuget push ./nupkg/*.nupkg --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json
   ```

#### Linux/macOS (Bash)

1. **Set environment variable**:
   ```bash
   export NUGET_API_KEY="your-api-key-here"
   ```

2. **Or add to ~/.bashrc or ~/.zshrc** for persistence:
   ```bash
   echo 'export NUGET_API_KEY="your-api-key-here"' >> ~/.bashrc
   source ~/.bashrc
   ```

3. **Build and publish**:
   ```bash
   cd /path/to/NfcReaderLib
   dotnet build --configuration Release
   dotnet pack --configuration Release --output ./nupkg
   dotnet nuget push ./nupkg/*.nupkg --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json
   ```

### Option 3: Using PowerShell Script

A convenience script is provided in `publish-nuget.ps1`. Usage:

```powershell
# Make sure NUGET_API_KEY environment variable is set first
./publish-nuget.ps1
```

Or pass the API key directly (not recommended for security):
```powershell
./publish-nuget.ps1 -ApiKey "your-api-key-here"
```

## Version Management

Update the version in `NfcReaderLib.csproj`:

```xml
<Version>1.0.2</Version>
<PackageReleaseNotes>v1.0.2 - Added platform-independent wrapper for 32/64-bit support.</PackageReleaseNotes>
```

## Security Best Practices

? **DO**:
- Store API keys in environment variables or secure secret management
- Use GitHub Secrets for CI/CD pipelines
- Rotate API keys periodically
- Use scoped API keys (only "Push" permission)

? **DON'T**:
- Commit API keys to source control
- Share API keys in plain text
- Use the same API key for multiple purposes
- Store keys in configuration files

## Troubleshooting

### "API key is invalid"
- Verify the key is correctly copied (no extra spaces)
- Check that the key hasn't expired
- Ensure the key has "Push" permission

### "Package already exists"
- Increment the version number in `.csproj`
- NuGet doesn't allow overwriting existing versions

### "Authentication failed"
- Check that environment variable is set: `echo $env:NUGET_API_KEY` (PowerShell)
- Restart terminal if you just set the variable
- Try setting the variable again

## Package Information

- **Package ID**: NfcReaderLib
- **Current Version**: 1.0.1
- **NuGet URL**: https://www.nuget.org/packages/NfcReaderLib
- **Repository**: https://github.com/johanhenningsson4-hash/EMVReaderSL

## What's New in This Version

### v1.0.1 Features:
- ? Platform-independent PC/SC wrapper (ModWinsCard)
- ? Automatic 32-bit/64-bit detection
- ? SL Token generation (SHA-256 from ICC certificates)
- ? EMV card reader utilities
- ? Updated documentation and copyright

## Publishing Checklist

Before publishing a new version:

- [ ] Update version number in `NfcReaderLib.csproj`
- [ ] Update `PackageReleaseNotes` with changes
- [ ] Update `README.md` if needed
- [ ] Build and test locally
- [ ] Commit all changes
- [ ] Create and push version tag (for GitHub Actions)
- [ ] Verify package appears on NuGet.org
- [ ] Test installing the package in a sample project

## Example: Complete Publishing Workflow

```powershell
# 1. Update version in .csproj (manually edit)
# 2. Commit changes
git add .
git commit -m "Bump version to 1.0.2"
git push

# 3. Create and push tag (triggers GitHub Actions)
git tag v1.0.2
git push origin v1.0.2

# 4. Wait for GitHub Actions to complete
# 5. Verify on https://www.nuget.org/packages/NfcReaderLib
```

## Alternative: Manual Publishing

If you prefer manual control:

```powershell
# Set environment variable (if not already set)
$env:NUGET_API_KEY = "your-api-key-here"

# Run the publish script
./publish-nuget.ps1

# Or do it manually
cd NfcReaderLib
dotnet pack --configuration Release
dotnet nuget push bin\Release\*.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
```
