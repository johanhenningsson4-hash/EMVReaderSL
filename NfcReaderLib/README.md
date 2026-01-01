# NfcReaderLib

Low-level NFC/Smart card utilities library providing PC/SC communication, SL Token generation, and utility functions for card data processing.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![NuGet](https://img.shields.io/nuget/v/NfcReaderLib)
![License](https://img.shields.io/badge/license-MIT-blue)

## ?? Installation

### Package Manager Console
```powershell
Install-Package NfcReaderLib
```

### .NET CLI
```bash
dotnet add package NfcReaderLib
```

### PackageReference
```xml
<PackageReference Include="NfcReaderLib" Version="1.0.0" />
```

## ?? Target Framework

- **.NET Framework 4.7.2**

Compatible with:
- .NET Framework 4.7.2+
- .NET Standard 2.0+
- .NET Core 2.0+
- .NET 5+

## ? Features

### Core Functionality
- ?? **PC/SC Communication** - Low-level smart card reader communication
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ?? **Card Utilities** - Helper functions for card data processing
- ?? **Data Formatting** - Byte array and hex string conversions
- ??? **TLV Support** - Tag-Length-Value structure handling

## ?? Quick Start

```csharp
using NfcReaderLib;

// Convert hex string to byte array
byte[] data = Util.FromHexString("9F 46 01 02 03");

// Generate SL Token from ICC certificate
var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010",
    IccPublicKeyCertificate = iccCertData
};
string token = slCard.GetSLToken2();

// Format card number
string masked = Util.MaskPAN("1234567890123456");
// Output: "1234 56** **** 3456"

// Calculate SHA-256 hash
byte[] hash = Util.CalculateSHA1(data); // Despite name, uses SHA-256
```

## ?? Main Classes

### SLCard
**Purpose:** Card model with ICC certificate parsing and SL Token generation

**Properties:**
- `PAN` - Primary Account Number
- `AID` - Application Identifier
- `IccPublicKeyCertificate` - ICC certificate data (Tag 9F46)
- `IccPublicKeyExponent` - ICC exponent (Tag 9F47)
- `IccPublicKeyRemainder` - ICC remainder (Tag 9F48)

**Methods:**
- `GetSLToken2()` - Generate SHA-256 based SL Token (space-separated hex format)
- `ParseIccCertificate(byte[] data)` - Parse multi-line ICC certificate format

**Example:**
```csharp
var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010",
    IccPublicKeyCertificate = certificateBytes
};

string token = slCard.GetSLToken2();
// Output: "E3 B0 C4 42 98 FC 1C 14 9A FB ... 52 B8 55" (64 hex bytes, space-separated)
```

### Util
**Purpose:** Utility functions for data conversion and formatting

**Key Methods:**

**Hex Conversions:**
- `FromHexString(string hex)` - Convert hex string to byte array
  ```csharp
  byte[] data = Util.FromHexString("9F 46 01 02");
  ```
- `ByteArrayToHexString(byte[] bytes)` - Convert byte array to hex string
  ```csharp
  string hex = Util.ByteArrayToHexString(data); // "9F460102"
  ```
- `PrettyPrintHex(byte[] data)` - Format as space-separated hex
  ```csharp
  string pretty = Util.PrettyPrintHex(data); // "9F 46 01 02"
  ```

**PAN Masking:**
- `MaskPAN(string pan)` - Mask card number for privacy
  ```csharp
  string masked = Util.MaskPAN("1234567890123456");
  // Output: "1234 56** **** 3456"
  ```
- `MaskCardNumber(string cardNumber, string mask)` - Custom masking
  ```csharp
  string masked = Util.MaskCardNumber("1234567890123456", "####-##xx-xxxx-####");
  // Output: "1234-56xx-xxxx-3456"
  ```

**Cryptography:**
- `CalculateSHA1(byte[] data)` - SHA-256 hash (note: method name is legacy)
  ```csharp
  byte[] hash = Util.CalculateSHA1(certificateData);
  ```

**Data Conversion:**
- `ByteToInt(byte b)` - Convert byte to integer
- `ByteToInt(byte first, byte second)` - Convert two bytes to integer
- `Byte2Short(byte b1, byte b2)` - Convert two bytes to short
- `HexToAscii(string hexStr)` - Convert hex string to ASCII
- `NotEmpty(byte[] bytearray)` - Check if array has data

**String Utilities:**
- `GetSpaces(int length)` - Generate spacing string

### ModWinsCard64
**Purpose:** PC/SC (Personal Computer/Smart Card) wrapper for Windows

**Key Methods:**
- PC/SC context management
- Card reader enumeration
- APDU command transmission
- ATR (Answer To Reset) reading

**Status Word Handling:**
- `GetScardErrMsg(int retCode)` - Get error message for PC/SC return code

**Note:** This is a low-level class typically used through higher-level wrappers.

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

? **Unique** - Each card generates a unique token  
? **Consistent** - Same card always produces the same token  
? **Secure** - One-way hash, cannot be reversed  
? **Privacy-Friendly** - No PAN or sensitive data exposed  
? **Format** - 64 bytes as space-separated hex (191 characters)  

### Use Cases

- ?? Loyalty program card identification
- ?? Card binding to user accounts
- ?? Transaction correlation
- ?? Duplicate card detection
- ?? Analytics without storing sensitive data

## ??? Advanced Usage

### Multi-Line ICC Certificate Parsing

The SLCard class handles multi-line certificate formats automatically:

```csharp
string certData = @"Cert: 9F4681...
Exp: 03
Rem: 12345...";

var slCard = new SLCard
{
    PAN = "1234567890123456",
    AID = "A0000000031010"
};

// Parse multi-line format
string[] lines = certData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
foreach (string line in lines)
{
    if (line.StartsWith("Cert:"))
        slCard.IccPublicKeyCertificate = Util.FromHexString(line.Substring(5));
    else if (line.StartsWith("Exp:"))
        slCard.IccPublicKeyExponent = Util.FromHexString(line.Substring(4));
    else if (line.StartsWith("Rem:"))
        slCard.IccPublicKeyRemainder = Util.FromHexString(line.Substring(4));
}

string token = slCard.GetSLToken2();
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

### PAN Masking Patterns

```csharp
// Standard masking (first 6 + last 4)
string pan1 = Util.MaskPAN("1234567890123456"); 
// "1234 56** **** 3456"

// Custom masking
string pan2 = Util.MaskCardNumber("1234567890123456", "####-####-####-####");
// "1234-5678-9012-3456"

string pan3 = Util.MaskCardNumber("1234567890123456", "####-##xx-xxxx-####");
// "1234-56xx-xxxx-3456"
```

## ?? Dependencies

- **System.Security.Cryptography.Algorithms** (v4.3.1) - SHA-256 hashing

## ?? Requirements

- .NET Framework 4.7.2 or later
- Windows 7 or later (for PC/SC support)

## ?? Documentation

For complete documentation and usage examples, visit:
- [GitHub Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)
- [NuGet Package](https://www.nuget.org/packages/NfcReaderLib)

## ?? Related Packages

- **EMVCard.Core** - High-level EMV card reading library (depends on NfcReaderLib)

## ?? License

Copyright © Johan Henningsson 2026

This project is licensed under the MIT License.

## ?? Author

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Acknowledgments

- EMV specifications by EMVCo
- PC/SC Workgroup
- .NET Cryptography API

---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this package on GitHub if you find it useful!**