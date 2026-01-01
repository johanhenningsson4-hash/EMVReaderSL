# ModWinsCard Platform-Independent Wrapper

## Overview

This library provides automatic platform detection for 32-bit and 64-bit Windows applications when working with PC/SC smart card readers.

**? Migration Complete**: All code in this project now uses the platform-independent `ModWinsCard` wrapper.

## Files

- **ModWinsCard.cs** - Platform-independent wrapper (automatically detects 32/64-bit) - **USE THIS**
- **ModWinsCard64.cs** - 64-bit specific implementation (used internally by wrapper)
- **ModWinsCard32.cs** - 32-bit specific implementation (used internally by wrapper)

## Current Status

? **EmvCardReader** classes have been updated to use `ModWinsCard`  
? All handle types changed from `Int64` to `IntPtr`  
? Build verified - all code compiles successfully  
? Automatic platform detection enabled

## Usage

### Using the Platform-Independent Wrapper (Current Implementation)

The codebase now uses `ModWinsCard` class which automatically detects the platform at runtime:

```csharp
using System;

// The wrapper automatically detects if running as 32-bit or 64-bit
IntPtr hContext = IntPtr.Zero;
int result = ModWinsCard.SCardEstablishContext(
    ModWinsCard.SCARD_SCOPE_USER, 
    0, 
    0, 
    ref hContext);

if (result == ModWinsCard.SCARD_S_SUCCESS)
{
    Console.WriteLine($"Running on {ModWinsCard.GetPlatformInfo()} platform");
    
    // Use the context...
    
    ModWinsCard.SCardReleaseContext(hContext);
}
else
{
    string error = ModWinsCard.GetScardErrMsg(result);
    Console.WriteLine($"Error: {error}");
}
```

### Option 2: Use Platform-Specific Classes Directly

If you need to explicitly target a specific platform:

```csharp
// For 64-bit applications
Int64 hContext64 = 0;
ModWinsCard64.SCardEstablishContext(
    ModWinsCard64.SCARD_SCOPE_USER, 
    0, 
    0, 
    ref hContext64);

// For 32-bit applications
int hContext32 = 0;
ModWinsCard32.SCardEstablishContext(
    ModWinsCard32.SCARD_SCOPE_USER, 
    0, 
    0, 
    ref hContext32);
```

## Platform Detection

The wrapper uses `IntPtr.Size` to detect the platform at runtime:
- `IntPtr.Size == 8` ? 64-bit platform ? uses `ModWinsCard64`
- `IntPtr.Size == 4` ? 32-bit platform ? uses `ModWinsCard32`

## Key Differences Between 32-bit and 64-bit

### Handle Types
- **64-bit**: Uses `Int64` for context and card handles
- **32-bit**: Uses `int` for context and card handles
- **Wrapper**: Uses `IntPtr` which automatically adjusts to platform size

### Structure Members
The `SCARD_READERSTATE` structure differs:
- **64-bit**: `UserData` is `Int64`
- **32-bit**: `UserData` is `int`
- **Wrapper**: `UserData` is `IntPtr`

## Migrating Existing Code

If you have existing code using `ModWinsCard64` directly, you can migrate to the wrapper:

### Before (64-bit only):
```csharp
Int64 hContext = 0;
Int64 hCard = 0;
int protocol = 0;

ModWinsCard64.SCardEstablishContext(
    ModWinsCard64.SCARD_SCOPE_USER, 0, 0, ref hContext);
ModWinsCard64.SCardConnect(
    hContext, readerName, ModWinsCard64.SCARD_SHARE_SHARED,
    ModWinsCard64.SCARD_PROTOCOL_T0 | ModWinsCard64.SCARD_PROTOCOL_T1,
    ref hCard, ref protocol);
```

### After (Platform-independent):
```csharp
IntPtr hContext = IntPtr.Zero;
IntPtr hCard = IntPtr.Zero;
int protocol = 0;

ModWinsCard.SCardEstablishContext(
    ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);
ModWinsCard.SCardConnect(
    hContext, readerName, ModWinsCard.SCARD_SHARE_SHARED,
    ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1,
    ref hCard, ref protocol);
```

## Building for Different Platforms

### Visual Studio Configuration

1. **AnyCPU** (Recommended): Builds a single executable that runs on both platforms
   - Project Properties ? Build ? Platform target: **Any CPU**
   - The wrapper will automatically detect the platform at runtime

2. **x64**: Explicitly target 64-bit
   - Project Properties ? Build ? Platform target: **x64**

3. **x86**: Explicitly target 32-bit
   - Project Properties ? Build ? Platform target: **x86**

## Migration Summary

The following changes were made to support flexible platform switching:

### Updated Files
- **EmvCardReader.cs** (both copies)
  - Changed `ModWinsCard64` ? `ModWinsCard`
  - Changed `Int64 _hContext` ? `IntPtr _hContext`
  - Changed `Int64 _hCard` ? `IntPtr _hCard`
  - Changed `Int64 _sendLength` ? `IntPtr _sendLength`
  - Changed `Int64 _recvLength` ? `IntPtr _recvLength`
  - Changed `ModWinsCard64.SCARD_IO_REQUEST` ? `ModWinsCard.SCARD_IO_REQUEST`
  - Updated all method calls to use `ModWinsCard`

### Breaking Changes
None! The API surface remains identical, only the underlying implementation changed.

### Advantages

1. **Single Codebase**: Write once, run on both 32-bit and 64-bit platforms
2. **Runtime Detection**: Automatically uses the correct implementation
3. **Type Safety**: Uses `IntPtr` for proper platform-specific handle sizes
4. **Backward Compatible**: Existing 32/64-bit specific code continues to work
5. **No Performance Overhead**: Simple delegation pattern with inline checks

## Troubleshooting

### "BadImageFormatException" errors
- This usually means your application bitness doesn't match the DLL/system requirements
- Use the wrapper with **AnyCPU** target to avoid this issue

### Checking current platform at runtime
```csharp
Console.WriteLine($"Platform: {ModWinsCard.GetPlatformInfo()}");
Console.WriteLine($"Running 64-bit: {ModWinsCard.IsRunning64Bit()}");
```

## Example: Complete Card Reader Application

```csharp
using System;
using System.Collections.Generic;

public class CardReaderExample
{
    private IntPtr _hContext = IntPtr.Zero;
    private IntPtr _hCard = IntPtr.Zero;

    public List<string> GetReaders()
    {
        var readers = new List<string>();

        // Establish context
        int ret = ModWinsCard.SCardEstablishContext(
            ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref _hContext);
        
        if (ret != ModWinsCard.SCARD_S_SUCCESS)
            return readers;

        // Get reader list size
        int pcchReaders = 0;
        ret = ModWinsCard.SCardListReaders(
            _hContext, null, null, ref pcchReaders);
        
        if (ret != ModWinsCard.SCARD_S_SUCCESS)
            return readers;

        // Get readers
        byte[] readersList = new byte[pcchReaders];
        ret = ModWinsCard.SCardListReaders(
            _hContext, null, readersList, ref pcchReaders);
        
        if (ret != ModWinsCard.SCARD_S_SUCCESS)
            return readers;

        // Parse reader names
        string rName = "";
        for (int i = 0; i < pcchReaders; i++)
        {
            if (readersList[i] == 0)
            {
                if (rName.Length > 0)
                {
                    readers.Add(rName);
                    rName = "";
                }
                if (i + 1 < pcchReaders && readersList[i + 1] == 0)
                    break;
            }
            else
            {
                rName += (char)readersList[i];
            }
        }

        return readers;
    }

    public bool Connect(string readerName)
    {
        int protocol = 0;
        int ret = ModWinsCard.SCardConnect(
            _hContext,
            readerName,
            ModWinsCard.SCARD_SHARE_SHARED,
            ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1,
            ref _hCard,
            ref protocol);

        return ret == ModWinsCard.SCARD_S_SUCCESS;
    }

    public void Disconnect()
    {
        if (_hCard != IntPtr.Zero)
        {
            ModWinsCard.SCardDisconnect(_hCard, ModWinsCard.SCARD_LEAVE_CARD);
            _hCard = IntPtr.Zero;
        }

        if (_hContext != IntPtr.Zero)
        {
            ModWinsCard.SCardReleaseContext(_hContext);
            _hContext = IntPtr.Zero;
        }
    }
}
```

## Technical Notes

- All constants (error codes, protocols, etc.) are identical across platforms
- The wrapper uses the `static class` pattern for cleaner syntax
- No runtime reflection is used - all platform detection is compile-time safe
- The implementation follows the existing EMV reader conventions in the codebase

## See Also

- PC/SC Workgroup Specifications: https://pcscworkgroup.com/
- Microsoft Winscard.dll Documentation
- EMV Specifications (for smart card protocols)
