# EMVReaderSL.csproj Solution File Fix

**Date:** 2025-12-31  
**Issue:** Project file showing as "unloaded" in Visual Studio  
**Status:** ? Fixed

## Problem

The `EMVReaderSL.csproj` file was showing as unloaded in Visual Studio because:
1. The project reference GUIDs in EMVReaderSL.csproj didn't match the GUIDs in the solution file
2. The SDK-style projects (NfcReaderLib and EMVCard.Core) were using the wrong project type GUID in the solution file

## Root Cause

### Issue 1: Project Reference GUID Mismatch

**Mismatch between Solution File and Project References:**

| Project | Solution File GUID | EMVReaderSL.csproj Reference | Match |
|---------|-------------------|------------------------------|-------|
| NfcReaderLib | `{27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}` | `{B8A5F8E1-2A3D-4C5E-9F7B-1D8E6A4C2B9F}` | ? |
| EMVCard.Core | `{3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D}` | `{C9B6F9E2-3B4E-5D6F-AF8C-2E9F7B5D3C0A}` | ? |

### Issue 2: Wrong Project Type GUID in Solution

**All projects were using the old C# project type GUID:**
```
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = ...
```

**SDK-style projects should use:**
```
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = ...
```

## Solution

### Fix 1: Updated EMVReaderSL.csproj Project References

Changed the `<ProjectReference>` sections in `EMVReaderSL.csproj` to use the correct GUIDs from the solution file:

#### Before (Incorrect)
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

#### After (Correct)
```xml
<ProjectReference Include="EMVCard.Core\EMVCard.Core.csproj">
  <Project>{3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D}</Project>
  <Name>EMVCard.Core</Name>
</ProjectReference>
<ProjectReference Include="NfcReaderLib\NfcReaderLib.csproj">
  <Project>{27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}</Project>
  <Name>NfcReaderLib</Name>
</ProjectReference>
```

### Fix 2: Updated Solution File Project Type GUIDs

Changed the project type GUID for SDK-style projects in `EMVReaderSL.sln`:

#### Before (Incorrect)
```
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "NfcReaderLib", "NfcReaderLib\NfcReaderLib.csproj", "{27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}"
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "EMVCard.Core", "EMVCard.Core\EMVCard.Core.csproj", "{3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D}"
```

#### After (Correct)
```
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "NfcReaderLib", "NfcReaderLib\NfcReaderLib.csproj", "{27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}"
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "EMVCard.Core", "EMVCard.Core\EMVCard.Core.csproj", "{3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D}"
```

**Note:** The EMVReaderSL project correctly keeps the old C# GUID since it's a classic .NET Framework project:
```
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "EMVReaderSL", "EMVReaderSL.csproj", "{36C17DE2-A271-47FC-989A-CA2165BF3639}"
```

## Project Type GUIDs Reference

| Project Type | GUID | Usage |
|--------------|------|-------|
| Classic C# Project | `{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}` | Old .NET Framework projects |
| SDK-Style Project | `{9A19103F-16F7-4668-BE54-9A1E7A4F7556}` | .NET Core, .NET 5+, SDK-style .NET Framework |

## Verification

### Build Test
```powershell
PS C:\Jobb\EMVReaderSLCard> msbuild EMVReaderSL.sln /p:Configuration=Debug /t:Rebuild

Build succeeded.
    2 Warning(s)
    0 Error(s)
Time Elapsed 00:00:02.40
```

? **All projects now build successfully**

### Project Loading
The project should now load correctly in Visual Studio without showing as "(unloaded)".

## Steps to Reload in Visual Studio

If the project still shows as unloaded after this fix:

1. **Close Visual Studio** completely
2. **Delete `.vs` folder** in the solution directory (hidden folder with Visual Studio cache)
   ```powershell
   Remove-Item -Path "C:\Jobb\EMVReaderSLCard\.vs" -Recurse -Force
   ```
3. **Reopen the solution** in Visual Studio
4. The project should now load correctly

Alternatively, in Visual Studio:
1. Right-click on the unloaded project in Solution Explorer
2. Select **Reload Project**

## Correct Project Structure

```
EMVReaderSL Solution
??? EMVReaderSL {36C17DE2-A271-47FC-989A-CA2165BF3639} [Classic C#]
?   ??? References: System, System.Data, System.Deployment, etc.
?   ??? Project References:
?   ?   ??? EMVCard.Core {3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D}
?   ?   ??? NfcReaderLib {27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}
?   ??? Files: EMVReader.cs, Program.cs, etc.
?
??? NfcReaderLib {27ECDC60-44BC-4FC7-BE76-2FA34A00C26F} [SDK-Style]
?   ??? Files: ModWinsCard64.cs, SLCard.cs, Util.cs
?
??? EMVCard.Core {3E0BC05C-4EA5-4584-AB19-3D3DF024CE6D} [SDK-Style]
    ??? Project References:
    ?   ??? NfcReaderLib {27ECDC60-44BC-4FC7-BE76-2FA34A00C26F}
    ??? Files: EmvApplicationSelector.cs, EmvCardReader.cs, etc.
```

## Important Notes

### SDK-Style Project GUIDs
SDK-style projects (.NET Core/.NET 5+/.NET Framework with SDK-style format) don't have explicit `<ProjectGuid>` elements in their `.csproj` files. The GUIDs are:
- Generated when the project is added to a solution
- Stored only in the `.sln` file
- Used for project references
- **Must use project type GUID `{9A19103F-16F7-4668-BE54-9A1E7A4F7556}` in the solution file**

### Classic .NET Framework Project GUIDs
Classic .NET Framework projects (like EMVReaderSL.csproj) have:
- Explicit `<ProjectGuid>` elements in their `.csproj` files
- This GUID must match what's in the solution file
- **Use project type GUID `{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}` in the solution file**

### Consistency is Key
- Always use the GUIDs from the solution file for project references
- Use the correct project type GUID for each project type
- Never mix up classic and SDK-style project type GUIDs

## Files Modified

1. **EMVReaderSL.csproj** - Updated project reference GUIDs
2. **EMVReaderSL.sln** - Updated project type GUIDs for SDK-style projects

## Backup

A backup of the original solution file was created:
- `EMVReaderSL.sln.backup`

## Related Files

- `EMVReaderSL.sln` - Solution file with corrected project type GUIDs
- `EMVReaderSL.csproj` - Main project with corrected project reference GUIDs
- `SOLUTION_STRUCTURE_FIX.md` - Overall solution structure documentation

---

**Status:** ? Fixed  
**Build:** ? Successful  
**Visual Studio:** Should now load correctly  
**Last Updated:** 2025-12-31
