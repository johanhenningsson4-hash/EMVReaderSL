# EMVReaderSL Solution Structure Fix - Complete

**Date:** 2025-12-31  
**Status:** ? Fixed and Building Successfully

## Problem Summary

The EMVReaderSL.csproj was not loading correctly in Visual Studio because:
1. The solution file didn't include the NfcReaderLib and EMVCard.Core library projects
2. The main project was referencing source files that had been moved to library projects
3. Building with `dotnet build` caused resource compilation errors for the .NET Framework 4.7.2 WinForms project

## Solution Structure (Fixed)

### Projects in Solution
```
EMVReaderSLCard/
??? EMVReaderSL.sln (main solution)
??? EMVReaderSL.csproj (WinForms UI - .NET Framework 4.7.2)
??? NfcReaderLib/
?   ??? NfcReaderLib.csproj (Library - .NET Framework 4.7.2)
??? EMVCard.Core/
    ??? EMVCard.Core.csproj (Library - .NET Framework 4.7.2)
```

### Project Dependencies
```
EMVReaderSL (UI)
  ??? EMVCard.Core
  ?     ??? NfcReaderLib
  ??? NfcReaderLib
```

## Changes Made

### 1. Created Library Project Files ?

**NfcReaderLib/NfcReaderLib.csproj**
- SDK-style project targeting .NET Framework 4.7.2
- Contains: ModWinsCard64.cs, SLCard.cs, Util.cs
- Dependency: System.Security.Cryptography.Algorithms (4.3.0)
- Package generation: Disabled temporarily

**EMVCard.Core/EMVCard.Core.csproj**
- SDK-style project targeting .NET Framework 4.7.2
- Contains: EmvApplicationSelector.cs, EmvCardReader.cs, EmvDataParser.cs, EmvGpoProcessor.cs, EmvRecordReader.cs, EmvTokenGenerator.cs
- Dependency: NfcReaderLib project reference
- Package generation: Disabled temporarily

### 2. Updated Solution File ?

Added both library projects to EMVReaderSL.sln:
```
dotnet sln add "NfcReaderLib\NfcReaderLib.csproj"
dotnet sln add "EMVCard.Core\EMVCard.Core.csproj"
```

### 3. Fixed EMVReaderSL.csproj ?

**Removed from compilation:**
- EmvApplicationSelector.cs
- EmvCardReader.cs
- EmvDataParser.cs
- EmvGpoProcessor.cs
- EmvRecordReader.cs
- EmvTokenGenerator.cs
- ModWinsCard64.cs
- SLCard.cs
- Util.cs

**Kept in main project:**
- EMVReader.cs (WinForms UI)
- EMVReader.Designer.cs
- Program.cs (entry point)
- Properties files

**Added project references:**
```xml
<ProjectReference Include="EMVCard.Core\EMVCard.Core.csproj">
  <Project>{C9B6F9E2-3B4E-5D6F-AF8C-2E9F7B5D3C0A}</Project>
  <Name>EMVCard.Core</Name>
</ProjectReference>
<ProjectReference Include="NfcReaderLib\NfcReaderLib.csproj">
  <Project>{B8A5F8E1-2A3D-4C5E-9F7B-1D8E6A4C2B9F}</Project>
  <Name>NfcReaderLib</Name>
</ProjectReference>
```

## Build Instructions

### ? Use MSBuild (Recommended for .NET Framework)
```powershell
cd C:\Jobb\EMVReaderSLCard
msbuild EMVReaderSL.sln /p:Configuration=Debug /p:Platform="Any CPU"
msbuild EMVReaderSL.sln /p:Configuration=Release /p:Platform="Any CPU"
```

### ?? Avoid dotnet build for Main Project
The `dotnet build` command uses newer SDK that has stricter resource requirements, causing errors with WinForms .resx files in .NET Framework 4.7.2 projects.

**Error when using dotnet build:**
```
MSB3823: Non-string resources require the property GenerateResourceUsePreserializedResources to be set to true.
MSB3822: Non-string resources require the System.Resources.Extensions assembly at runtime
```

**Workaround:** Use MSBuild instead of dotnet build for the main solution.

### ? Library Projects Can Use dotnet build
The SDK-style library projects build fine with dotnet:
```powershell
dotnet build NfcReaderLib\NfcReaderLib.csproj
dotnet build EMVCard.Core\EMVCard.Core.csproj
```

## Visual Studio Usage

### Opening in Visual Studio
1. Open `EMVReaderSL.sln` in Visual Studio
2. All three projects should now load correctly:
   - EMVReaderSL (startup project)
   - NfcReaderLib
   - EMVCard.Core

### Building in Visual Studio
- Use the standard Build menu (F6) or Build Solution (Ctrl+Shift+B)
- Visual Studio uses MSBuild internally, so it works correctly

### Debugging
- Set EMVReaderSL as the startup project (right-click ? Set as Startup Project)
- Press F5 to debug
- The library DLLs will be copied to the output directory automatically

## File Locations After Build

### Debug Build Output
```
bin\Debug\
??? EMVReader.exe (main application)
??? EMVReader.exe.config
??? EMVReader.pdb
??? EMVCard.Core.dll (library)
??? EMVCard.Core.pdb
??? NfcReaderLib.dll (library)
??? NfcReaderLib.pdb
```

### Library Build Outputs
```
NfcReaderLib\bin\Debug\net472\
??? NfcReaderLib.dll
??? NfcReaderLib.pdb

EMVCard.Core\bin\Debug\net472\
??? EMVCard.Core.dll
??? EMVCard.Core.pdb
```

## Verification

### ? Solution Restore
```powershell
PS C:\Jobb\EMVReaderSLCard> dotnet restore EMVReaderSL.sln
# ? Restore complete (0,9s)
```

### ? Solution Build (MSBuild)
```powershell
PS C:\Jobb\EMVReaderSLCard> msbuild EMVReaderSL.sln /p:Configuration=Debug
# ? Build succeeded - 2 Warning(s) 0 Error(s)
```

### ? Solution List
```powershell
PS C:\Jobb\EMVReaderSLCard> dotnet sln list
# Project(s)
# ----------
# EMVCard.Core\EMVCard.Core.csproj
# EMVReaderSL.csproj
# NfcReaderLib\NfcReaderLib.csproj
```

## Known Issues & Warnings

### ?? Minor Warnings (Non-Critical)
```
NfcReaderLib\SLCard.cs(49,30): warning CS0168: The variable 'e' is declared but never used
NfcReaderLib\SLCard.cs(66,30): warning CS0168: The variable 'e' is declared but never used
```

**Fix:** Remove unused exception variables in catch blocks:
```csharp
// Before:
catch (Exception e) { }

// After:
catch (Exception) { }
```

### ?? NuGet Package Generation Disabled
Package generation is currently disabled (`GeneratePackageOnBuild>false</GeneratePackageOnBuild>`) to avoid README.md path issues during development builds.

**To re-enable for publishing:**
1. Ensure README.md files exist in project directories
2. Set `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`
3. Build in Release mode
4. Packages will be in `bin\Release\` directories

## Next Steps

### Immediate
- [x] Solution structure fixed
- [x] All projects loading correctly
- [x] Solution builds successfully

### Cleanup (Optional)
- [ ] Fix unused variable warnings in SLCard.cs
- [ ] Remove duplicate source files from main directory (already in libraries)
- [ ] Create proper README.md files for library projects

### For NuGet Publishing
- [ ] Create/update README.md in NfcReaderLib/
- [ ] Create/update README.md in EMVCard.Core/
- [ ] Re-enable package generation
- [ ] Update version numbers if needed
- [ ] Build in Release configuration
- [ ] Publish to NuGet.org

## Project GUIDs

For reference, the project GUIDs used in the solution:

| Project | GUID |
|---------|------|
| EMVReaderSL | {36C17DE2-A271-47FC-989A-CA2165BF3639} |
| NfcReaderLib | {B8A5F8E1-2A3D-4C5E-9F7B-1D8E6A4C2B9F} |
| EMVCard.Core | {C9B6F9E2-3B4E-5D6F-AF8C-2E9F7B5D3C0A} |

## Build Commands Quick Reference

```powershell
# Navigate to solution directory
cd C:\Jobb\EMVReaderSLCard

# Restore packages
dotnet restore

# Build solution (use MSBuild)
msbuild EMVReaderSL.sln /p:Configuration=Debug
msbuild EMVReaderSL.sln /p:Configuration=Release

# Build specific project with dotnet (libraries only)
dotnet build NfcReaderLib\NfcReaderLib.csproj
dotnet build EMVCard.Core\EMVCard.Core.csproj

# Run application
.\bin\Debug\EMVReader.exe
```

## Summary

? **All projects are now properly structured and building successfully**

The solution has been fixed by:
1. Creating proper project files for the library projects
2. Adding them to the solution
3. Removing duplicate source file references from the main project
4. Adding project references instead
5. Using MSBuild for building (instead of dotnet build for WinForms)

Visual Studio should now load all projects correctly, and you can build, debug, and run the application without issues.

---

**Status:** ? Complete and Working  
**Framework:** .NET Framework 4.7.2  
**Build Tool:** MSBuild (recommended) or Visual Studio  
**Last Updated:** 2025-12-31
