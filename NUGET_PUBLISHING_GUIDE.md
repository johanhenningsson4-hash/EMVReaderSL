# Publishing EMV Reader to NuGet - Complete Guide

## Current State Analysis

### Project Structure
Your current solution has:
- **EMVReaderSL.csproj** - Windows Forms application (.NET Framework 4.7.2)
- Contains both UI and business logic in one project
- Uses ModWinsCard64 for PC/SC communication
- Has SLCard.cs in NfcReaderLib namespace

### Challenge
NuGet packages should be **class libraries**, not applications. Your project is currently an executable Windows Forms app.

## Option 1: Publish As-Is (Quick - Not Recommended)

You *could* technically publish the EXE as a "tool" package, but this is not standard practice and won't be useful as a library.

**Why Not:**
- ? Includes UI components (not reusable)
- ? Windows Forms dependencies
- ? .NET Framework 4.7.2 (not cross-platform)
- ? Not suitable for library consumers

## Option 2: Extract Library Components (Recommended)

### Step 1: Create Class Library Projects

#### 1.1 Create NfcReaderLib Project
```bash
cd C:\Jobb\EMVReaderSLCard
dotnet new classlib -n NfcReaderLib -f netstandard2.0
```

**Move to this project:**
- `SLCard.cs`
- `Util.cs`
- `ModWinsCard64.cs`

**Package Info:**
- Name: `NfcReaderLib`
- Description: NFC/Smart card utilities and SL Token generation
- Target: .NET Standard 2.0 (for maximum compatibility)

#### 1.2 Create EMVCard.Core Project
```bash
dotnet new classlib -n EMVCard.Core -f netstandard2.0
```

**Move to this project:**
- `EmvCardReader.cs`
- `EmvDataParser.cs`
- `EmvRecordReader.cs`
- `EmvApplicationSelector.cs`
- `EmvGpoProcessor.cs`
- `EmvTokenGenerator.cs`

**Package Info:**
- Name: `EMVCard.Core`
- Description: EMV chip card reading and parsing library
- Dependency: NfcReaderLib
- Target: .NET Standard 2.0

### Step 2: Update Solution Structure

```
EMVReaderSLCard/
??? NfcReaderLib/                    # NuGet Package 1
?   ??? NfcReaderLib.csproj
?   ??? SLCard.cs
?   ??? Util.cs
?   ??? ModWinsCard64.cs
?
??? EMVCard.Core/                    # NuGet Package 2
?   ??? EMVCard.Core.csproj
?   ??? EmvCardReader.cs
?   ??? EmvDataParser.cs
?   ??? EmvRecordReader.cs
?   ??? EmvApplicationSelector.cs
?   ??? EmvGpoProcessor.cs
?   ??? EmvTokenGenerator.cs
?
??? EMVReaderSL/                     # Windows Forms App (not published)
    ??? EMVReaderSL.csproj
    ??? EMVReader.cs
    ??? EMVReader.Designer.cs
    ??? Program.cs
```

### Step 3: Create NuGet Package Specifications

#### NfcReaderLib.nuspec
```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>NfcReaderLib</id>
    <version>1.0.0</version>
    <title>NFC Reader Library with SL Token Generation</title>
    <authors>Johan Henningsson</authors>
    <owners>Johan Henningsson</owners>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type="expression">MIT</license>
    <projectUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</projectUrl>
    <description>NFC/Smart card utilities including PC/SC communication, SL Token generation (SHA-256 based tokens from ICC certificates), and utility functions for card data processing.</description>
    <summary>NFC smart card reader library with SL Token generation capabilities</summary>
    <releaseNotes>Initial release with SL Token generation, PC/SC wrapper, and utility functions.</releaseNotes>
    <copyright>Copyright © Johan Henningsson 2024</copyright>
    <tags>nfc smartcard pcsc sl-token emv icc-certificate</tags>
    <dependencies>
      <group targetFramework=".NETStandard2.0">
        <dependency id="System.Security.Cryptography.Algorithms" version="4.3.0" />
      </group>
    </dependencies>
  </metadata>
  <files>
    <file src="bin\Release\netstandard2.0\NfcReaderLib.dll" target="lib\netstandard2.0" />
    <file src="bin\Release\netstandard2.0\NfcReaderLib.pdb" target="lib\netstandard2.0" />
    <file src="README.md" target="" />
  </files>
</package>
```

#### EMVCard.Core.nuspec
```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
  <metadata>
    <id>EMVCard.Core</id>
    <version>1.0.0</version>
    <title>EMV Card Reader Core Library</title>
    <authors>Johan Henningsson</authors>
    <owners>Johan Henningsson</owners>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type="expression">MIT</license>
    <projectUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</projectUrl>
    <description>EMV chip card reading library with support for PSE/PPSE application selection, GPO processing, record reading, TLV parsing, and SL Token generation. Works with contact and contactless EMV cards via PC/SC readers.</description>
    <summary>Complete EMV chip card reading and processing library</summary>
    <releaseNotes>Initial release with PSE/PPSE support, GPO processing, AFL record reading, TLV parsing, PAN masking, and SL Token generation.</releaseNotes>
    <copyright>Copyright © Johan Henningsson 2024</copyright>
    <tags>emv chipcard pcsc pse ppse gpo afl tlv sl-token visa mastercard</tags>
    <dependencies>
      <group targetFramework=".NETStandard2.0">
        <dependency id="NfcReaderLib" version="1.0.0" />
      </group>
    </dependencies>
  </metadata>
  <files>
    <file src="bin\Release\netstandard2.0\EMVCard.Core.dll" target="lib\netstandard2.0" />
    <file src="bin\Release\netstandard2.0\EMVCard.Core.pdb" target="lib\netstandard2.0" />
    <file src="README.md" target="" />
  </files>
</package>
```

### Step 4: Update Project Files for NuGet

#### NfcReaderLib.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageId>NfcReaderLib</PackageId>
    <Version>1.0.0</Version>
    <Authors>Johan Henningsson</Authors>
    <Company>Johan Henningsson</Company>
    <Product>NFC Reader Library</Product>
    <Description>NFC/Smart card utilities including PC/SC communication, SL Token generation, and utility functions</Description>
    <PackageTags>nfc;smartcard;pcsc;sl-token;emv</PackageTags>
    <PackageProjectUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</PackageProjectUrl>
    <RepositoryUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <Copyright>Copyright © Johan Henningsson 2024</Copyright>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Security.Cryptography.Algorithms" Version="4.3.0" />
  </ItemGroup>
</Project>
```

#### EMVCard.Core.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <PackageId>EMVCard.Core</PackageId>
    <Version>1.0.0</Version>
    <Authors>Johan Henningsson</Authors>
    <Company>Johan Henningsson</Company>
    <Product>EMV Card Reader Core</Product>
    <Description>EMV chip card reading library with PSE/PPSE, GPO, TLV parsing, and SL Token generation</Description>
    <PackageTags>emv;chipcard;pcsc;pse;ppse;gpo;tlv;visa;mastercard</PackageTags>
    <PackageProjectUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</PackageProjectUrl>
    <RepositoryUrl>https://github.com/johanhenningsson4-hash/EMVReaderSL</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <Copyright>Copyright © Johan Henningsson 2024</Copyright>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\NfcReaderLib\NfcReaderLib.csproj" />
  </ItemGroup>
</Project>
```

## Step-by-Step Publishing Process

### 1. Prepare Your NuGet Account
```bash
# Sign up at https://www.nuget.org
# Get your API key from https://www.nuget.org/account/apikeys
```

### 2. Build Release Packages
```bash
# Build in Release mode
dotnet build -c Release NfcReaderLib/NfcReaderLib.csproj
dotnet build -c Release EMVCard.Core/EMVCard.Core.csproj

# Or use pack command
dotnet pack NfcReaderLib/NfcReaderLib.csproj -c Release
dotnet pack EMVCard.Core/EMVCard.Core.csproj -c Release
```

This creates:
- `NfcReaderLib.1.0.0.nupkg`
- `EMVCard.Core.1.0.0.nupkg`

### 3. Test Locally
```bash
# Create local NuGet feed for testing
mkdir C:\LocalNuGet
dotnet nuget add source C:\LocalNuGet -n LocalNuGet

# Copy packages
copy NfcReaderLib\bin\Release\*.nupkg C:\LocalNuGet\
copy EMVCard.Core\bin\Release\*.nupkg C:\LocalNuGet\

# Test in new project
mkdir TestProject
cd TestProject
dotnet new console
dotnet add package NfcReaderLib --source C:\LocalNuGet
dotnet add package EMVCard.Core --source C:\LocalNuGet
```

### 4. Publish to NuGet
```bash
# Set API key (one time)
dotnet nuget push NfcReaderLib.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Wait for NfcReaderLib to be indexed (5-10 minutes)

# Then publish EMVCard.Core
dotnet nuget push EMVCard.Core.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### 5. Verify Publication
```
Visit:
- https://www.nuget.org/packages/NfcReaderLib
- https://www.nuget.org/packages/EMVCard.Core

Check package appears in search
```

## Usage Example for Consumers

### Installation
```bash
# In consumer project
dotnet add package EMVCard.Core
dotnet add package NfcReaderLib
```

### Code Example
```csharp
using EMVCard;
using NfcReaderLib;

// Initialize reader
var cardReader = new EmvCardReader();
var readers = cardReader.Initialize();

// Connect to card
cardReader.Connect(readers[0]);

// Select application
var appSelector = new EmvApplicationSelector(cardReader);
var applications = appSelector.LoadPPSE();

// Read card
var dataParser = new EmvDataParser();
var recordReader = new EmvRecordReader(cardReader, dataParser);
var gpoProcessor = new EmvGpoProcessor(cardReader);

// Generate SL Token
var tokenGenerator = new EmvTokenGenerator();
var token = tokenGenerator.GenerateToken(cardData, pan, aid);

Console.WriteLine($"SL Token: {token.Token}");
```

## Alternative: Publish Current Project As-Is

If you want to publish without refactoring:

### Quick Publish (Not Recommended)
```xml
<!-- Add to EMVReaderSL.csproj -->
<PropertyGroup>
  <IsPackable>true</IsPackable>
  <PackAsTool>true</PackAsTool>
  <ToolCommandName>emvreader</ToolCommandName>
</PropertyGroup>
```

Then:
```bash
dotnet pack -c Release
dotnet nuget push bin\Release\EMVReaderSL.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Users would install as a tool:
```bash
dotnet tool install --global EMVReaderSL
emvreader  # Runs the application
```

**Limitations:**
- Not usable as a library
- Windows only
- Includes unnecessary UI code
- Not following NuGet best practices

## Recommended Approach

### Phase 1: Create Libraries (1-2 days)
1. Create NfcReaderLib and EMVCard.Core projects
2. Move business logic code
3. Keep UI in EMVReaderSL
4. Test everything still works

### Phase 2: Documentation (1 day)
1. Create README for each library
2. Add XML documentation comments
3. Create usage examples
4. Add license file

### Phase 3: Publishing (1 day)
1. Build Release packages
2. Test locally
3. Publish to NuGet
4. Monitor for issues

### Phase 4: Maintenance
1. Version updates (semantic versioning)
2. Bug fixes
3. Feature additions
4. Community support

## NuGet Package Versioning

Follow **Semantic Versioning** (SemVer):
- **1.0.0** - Initial release
- **1.0.1** - Bug fixes (patch)
- **1.1.0** - New features (minor)
- **2.0.0** - Breaking changes (major)

## Files to Include in Packages

### Must Have
- ? DLL files (compiled code)
- ? README.md
- ? LICENSE file

### Should Have
- ? PDB files (debugging symbols)
- ? XML documentation
- ? Icon (64x64 PNG)

### Optional
- Sample code
- Change log
- Migration guides

## NuGet Package Metadata Checklist

- [ ] Unique package ID
- [ ] Semantic version number
- [ ] Clear description
- [ ] Author information
- [ ] License (MIT recommended)
- [ ] Project URL
- [ ] Repository URL
- [ ] Tags (for discoverability)
- [ ] Dependencies listed
- [ ] Release notes
- [ ] Icon (optional but recommended)

## Common Pitfalls to Avoid

### 1. Wrong Target Framework
? .NET Framework 4.7.2 (Windows only)
? .NET Standard 2.0 (cross-platform)

### 2. Missing Dependencies
? Assume dependencies are installed
? Explicitly declare in .nuspec or .csproj

### 3. Including UI Code
? Windows Forms controls in library
? Pure business logic only

### 4. No Documentation
? Users don't know how to use it
? XML comments + README + examples

### 5. Version Conflicts
? Same version for updates
? Increment version each release

## Cost and Requirements

### Publishing to NuGet.org
- ? **FREE** - No cost to publish
- ? **Account** - Free nuget.org account
- ? **API Key** - Generate from your account
- ? **Time** - ~5-10 minutes per package to index

### Benefits
- ?? Easy distribution
- ?? Automatic updates via NuGet
- ?? Global availability
- ?? Download statistics
- ?? Searchable/discoverable
- ?? Documentation hosting

## Summary

### To Publish Your Project to NuGet:

**Option A: Proper Library (Recommended)**
1. Extract business logic into class library projects
2. Target .NET Standard 2.0
3. Create separate packages (NfcReaderLib + EMVCard.Core)
4. Keep UI separate
5. Publish both libraries

**Option B: Quick Tool Package (Not Recommended)**
1. Publish as-is as a dotnet tool
2. Users install globally
3. Not usable as a library
4. Windows-only

### My Recommendation

Go with **Option A** because:
- ? Follows NuGet best practices
- ? Reusable by other developers
- ? Cross-platform compatible
- ? Easier to maintain
- ? Professional appearance
- ? Better documentation
- ? Community adoption potential

**Time Investment:** 3-4 days
**Value:** High-quality, reusable packages

## Next Steps

1. **Decide**: Which option fits your goals?
2. **Plan**: Set aside time for refactoring (if Option A)
3. **Execute**: Follow the step-by-step guide
4. **Test**: Verify locally before publishing
5. **Publish**: Push to NuGet.org
6. **Maintain**: Respond to issues, add features

## Support

If you need help with any step, ask about:
- Creating class library projects
- Moving code between projects
- Writing .nuspec files
- Testing NuGet packages locally
- Publishing commands
- Version management

I'm here to help! ??
