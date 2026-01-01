# NfcReaderLib

Low-level NFC/Smart card utilities library providing **platform-independent PC/SC communication**, SL Token generation, and utility functions for card data processing.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![NuGet](https://img.shields.io/nuget/v/NfcReaderLib)
![License](https://img.shields.io/badge/license-MIT-blue)
![Platform](https://img.shields.io/badge/platform-32bit%20%7C%2064bit-green)

## ? What's New in v1.0.2

- ?? **Platform-Independent Wrapper** - Automatic 32-bit/64-bit detection
- ?? **Unified API** - Single codebase for both platforms
- ?? **Enhanced PC/SC Support** - `ModWinsCard`, `ModWinsCard32`, `ModWinsCard64`
- ?? **Improved Documentation** - Complete guides and examples
- ?? **NuGet Publishing** - Easy updates with GitHub Actions

## ?? Installation

### Package Manager Console
```powershell
Install-Package NfcReaderLib -Version 1.0.2
```

### .NET CLI
```bash
dotnet add package NfcReaderLib --version 1.0.2
```

### PackageReference
```xml
<PackageReference Include="NfcReaderLib" Version="1.0.2" />
```

## ?? Target Framework

- **.NET Framework 4.7.2**

Compatible with:
- .NET Framework 4.7.2+
- .NET Standard 2.0+
- .NET Core 2.0+
- .NET 5+

**Platforms:** Windows (32-bit and 64-bit automatically detected)

## ?? Features

### Core Functionality
- ?? **Platform-Independent PC/SC** - Automatic 32/64-bit detection
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ?? **Card Utilities** - Helper functions for card data processing
- ?? **Data Formatting** - Byte array and hex string conversions
- ??? **EMV Support** - ICC certificate parsing and validation
- ?? **Cryptography** - SHA-256 hashing utilities

### Platform Support
- ? **Windows 32-bit** - Full support via `ModWinsCard32`
- ? **Windows 64-bit** - Full support via `ModWinsCard64`
- ? **Auto-Detection** - Use `ModWinsCard` wrapper for automatic platform selection
- ? **IntPtr Handles** - Platform-appropriate handle sizes

## ?? Quick Start

### Platform-Independent PC/SC Communication

```csharp
using NfcReaderLib;

// ModWinsCard automatically detects 32-bit or 64-bit platform
IntPtr hContext = IntPtr.Zero;
int result = ModWinsCard.SCardEstablishContext(
    ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);

if (result == ModWinsCard.SCARD_S_SUCCESS)
{
    Console.WriteLine($"Running on {ModWinsCard.GetPlatformInfo()} platform");
    
    // List readers
    int pcchReaders = 0;
    ModWinsCard.SCardListReaders(hContext, null, null, ref pcchReaders);
    
    byte[] readers = new byte[pcchReaders];
    ModWinsCard.SCardListReaders(hContext, null, readers, ref pcchReaders);
    
    // Release context
    ModWinsCard.SCardReleaseContext(hContext);
}
```

### SL Token Generation

```csharp
using NfcReaderLib;

// Generate SL Token from ICC certificate
var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010",
    IccPublicKeyCertificate = iccCertData
};

string token = slCard.GetSLToken();      // No spaces: "E3B0C442..."
string token2 = slCard.GetSLToken2();    // With spaces: "E3 B0 C4 42..."
```

### Utility Functions

```csharp
// Hex conversions
byte[] data = Util.FromHexString("9F 46 01 02 03");
string hex = Util.ByteArrayToHexString(data);

// PAN masking
string masked = Util.MaskPAN("1234567890123456");
// Output: "1234 56** **** 3456"

// SHA-256 hash
byte[] hash = Util.CalculateSHA1(data);
```

## ?? Main Classes

### ModWinsCard (Platform-Independent Wrapper) ? NEW

**Purpose:** Automatic 32-bit/64-bit platform detection for PC/SC operations

**Key Features:**
- ?? Automatic platform detection using `IntPtr.Size`
- ?? Single API for both 32-bit and 64-bit Windows
- ? Type-safe with `IntPtr` handles
- ? Zero performance overhead

**Example:**
```csharp
// Check platform
bool is64Bit = ModWinsCard.IsRunning64Bit();
string platform = ModWinsCard.GetPlatformInfo(); // "32-bit" or "64-bit"

// Establish context (works on both platforms)
IntPtr hContext = IntPtr.Zero;
int result = ModWinsCard.SCardEstablishContext(
    ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);
```

**Methods:**
- Context Management: `SCardEstablishContext`, `SCardReleaseContext`
- Reader Operations: `SCardListReaders`, `SCardConnect`, `SCardDisconnect`
- Card Operations: `SCardStatus`, `SCardTransmit`, `SCardBeginTransaction`
- Error Handling: `GetScardErrMsg(int returnCode)`
- Platform Info: `IsRunning64Bit()`, `GetPlatformInfo()`

**See also:** [Platform Wrapper Documentation](./README_ModWinsCard.md)

---

### SLCard

**Purpose:** Card model with ICC certificate parsing and SL Token generation

**Properties:**
- `PAN` - Primary Account Number
- `AID` - Application Identifier
- `IccPublicKeyCertificate` - ICC certificate data (Tag 9F46)
- `IccPublicKeyExponent` - ICC exponent (Tag 9F47)
- `IccPublicKeyRemainder` - ICC remainder (Tag 9F48)
- `IssuerPublicKeyCertificate` - Issuer certificate (Tag 90)
- `IssuerPublicKeyExponent` - Issuer exponent (Tag 9F32)
- `IssuerPublicKeyRemainder` - Issuer remainder (Tag 92)

**Methods:**
- `GetSLToken()` - Generate SHA-256 based SL Token (no spaces)
- `GetSLToken2()` - Generate SHA-256 based SL Token (space-separated hex)
- `GetMaskedPAN()` - Get masked PAN for privacy
- `ParseIccPublicKey(EmvPublicKey issuerKey)` - Parse ICC certificate (EMV v4.3)

**Example:**
```csharp
var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010",
    IccPublicKeyCertificate = certificateBytes
};

string token = slCard.GetSLToken2();
// Output: "E3 B0 C4 42 98 FC 1C 14 ... 52 B8 55" (64 hex bytes)

string maskedPan = slCard.GetMaskedPAN();
// Output: "1234 56** **** 3456"
```

---

### Util

**Purpose:** Utility functions for data conversion and formatting

**Key Methods:**

**Hex Conversions:**
- `FromHexString(string hex)` - Convert hex string to byte array
- `ByteArrayToHexString(byte[] bytes)` - Convert byte array to hex string
- `PrettyPrintHex(byte[] data)` - Format as space-separated hex

**PAN Masking:**
- `MaskPAN(string pan)` - Mask card number for privacy
- `MaskCardNumber(string cardNumber, string mask)` - Custom masking

**Cryptography:**
- `CalculateSHA1(byte[] data)` - SHA-256 hash (note: legacy name)

**Data Conversion:**
- `ByteToInt(byte b)` - Convert byte to integer
- `Byte2Short(byte b1, byte b2)` - Convert two bytes to short
- `HexToAscii(string hexStr)` - Convert hex string to ASCII
- `NotEmpty(byte[] bytearray)` - Check if array has data

---

## ?? Platform-Independent Architecture

### How It Works

```
Your Application
       ?
  ModWinsCard (Wrapper)
       ?
   [Platform Detection: IntPtr.Size == 8?]
       ?
  ???????????
  ?         ?
64-bit    32-bit
(Int64)   (int)
  ?         ?
winscard.dll
```

### Benefits

? **Single Codebase** - Write once, run on both platforms  
? **Automatic Detection** - No configuration needed  
? **Type Safe** - IntPtr ensures correct handle sizes  
? **Zero Overhead** - Simple delegation pattern  
? **Backward Compatible** - Platform-specific classes still available  

### Migration Example

**Before (64-bit only):**
```csharp
Int64 hContext = 0;
ModWinsCard64.SCardEstablishContext(
    ModWinsCard64.SCARD_SCOPE_USER, 0, 0, ref hContext);
```

**After (Platform-independent):**
```csharp
IntPtr hContext = IntPtr.Zero;
ModWinsCard.SCardEstablishContext(
    ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);
```

**See:** [Migration Guide](./MIGRATION_SUMMARY.md)

---

## ?? SL Token Details

### What is SL Token?

The **SL (Secure Link) Token** is a unique cryptographic identifier derived from the card's ICC Public Key Certificate using SHA-256 hashing.

### How It Works

```
ICC Certificate (Tag 9F46) ? SHA-256 Hash ? Space-Separated Hex ? SL Token
```

**Example:**
```
Input:  ICC Certificate (128-248 bytes)
Output: "E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55"
```

### Properties

?? **Unique** - Each card generates a unique token  
?? **Consistent** - Same card always produces the same token  
?? **Secure** - One-way hash, cannot be reversed  
??? **Privacy-Friendly** - No PAN or sensitive data exposed  
?? **Format** - 64 bytes as space-separated hex (191 characters)  

### Use Cases

- ?? Loyalty program card identification
- ?? Card binding to user accounts
- ?? Transaction correlation
- ?? Duplicate card detection
- ?? Analytics without storing sensitive data

---

## ?? Advanced Usage

### EMV Certificate Parsing

```csharp
using NfcReaderLib;

var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010",
    IccPublicKeyCertificate = iccCert,
    IccPublicKeyExponent = iccExp,
    IccPublicKeyRemainder = iccRem
};

// Parse ICC public key (requires issuer public key)
var issuerKey = new SLCard.EmvPublicKey(issuerModulus, issuerExponent);
var iccKey = slCard.ParseIccPublicKey(issuerKey);

if (iccKey != null)
{
    Console.WriteLine($"ICC Key: {iccKey}");
}
```

### Custom Hex Formatting

```csharp
byte[] data = new byte[] { 0x9F, 0x46, 0x01, 0x02 };

// Standard hex string (no spaces)
string standard = Util.ByteArrayToHexString(data); // "9F460102"

// Pretty print (with spaces)
string pretty = Util.PrettyPrintHex(data); // "9F 46 01 02"

// Parse back
byte[] parsed = Util.FromHexString(pretty);
```

### Platform Detection

```csharp
// Check current platform
if (ModWinsCard.IsRunning64Bit())
{
    Console.WriteLine("Running on 64-bit Windows");
}
else
{
    Console.WriteLine("Running on 32-bit Windows");
}

// Or get string
Console.WriteLine($"Platform: {ModWinsCard.GetPlatformInfo()}");
```

---

## ?? Dependencies

- **System.Security.Cryptography.Algorithms** (v4.3.1) - SHA-256 hashing

## ?? Requirements

- .NET Framework 4.7.2 or later
- Windows 7 or later (for PC/SC support)
- PC/SC smart card reader (for card operations)

---

## ?? Documentation

### Included Documentation

- **[README_ModWinsCard.md](./README_ModWinsCard.md)** - Platform wrapper guide
- **[MIGRATION_SUMMARY.md](./MIGRATION_SUMMARY.md)** - Migration details
- **[QUICKSTART_NUGET.md](./QUICKSTART_NUGET.md)** - Publishing guide

### Online Resources

- [GitHub Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)
- [NuGet Package](https://www.nuget.org/packages/NfcReaderLib)
- [Issues & Support](https://github.com/johanhenningsson4-hash/EMVReaderSL/issues)

---

## ?? Related Packages

- **EMVCard.Core** - High-level EMV card reading library (depends on NfcReaderLib)

---

## ?? Contributing

We welcome contributions! See the repository for contribution guidelines.

### Publishing to NuGet

This package uses environment variables for secure NuGet publishing:

```powershell
# Set API key
$env:NUGET_API_KEY = "your-key-here"

# Publish
./publish-nuget.ps1
```

**See:** [Publishing Guide](./QUICKSTART_NUGET.md)

---

## ?? License

Copyright © Johan Henningsson 2026

This project is licensed under the MIT License.

---

## ????? Author

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

---

## ?? Acknowledgments

- EMV specifications by EMVCo
- PC/SC Workgroup
- .NET Cryptography API
- Contributors and users of this library

---

## ?? Version History

### v1.0.2 (Latest)
- ? Added platform-independent wrapper (`ModWinsCard`)
- ? Automatic 32-bit/64-bit detection
- ? Updated `EmvCardReader` to use `IntPtr`
- ?? Enhanced documentation with guides
- ?? Added NuGet publishing workflow
- ?? Added security features (Git hooks, .gitignore)

### v1.0.1
- ?? Updated copyright to 2026
- ?? Improved documentation
- No functional changes

### v1.0.0
- ?? Initial release
- ?? SL Token generation
- ?? Card utilities
- ?? Cryptography helpers

---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this package on GitHub if you find it useful!**