# NfcReaderLib

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue)
![License](https://img.shields.io/badge/license-MIT-blue)

NFC/Smart card utilities library providing PC/SC communication, SL Token generation (SHA-256 based tokens from ICC certificates), and utility functions for card data processing.

## ?? Installation

### NuGet Package Manager
```bash
Install-Package NfcReaderLib -Version 1.0.0
```

### .NET CLI
```bash
dotnet add package NfcReaderLib --version 1.0.0
```

### Package Reference
```xml
<PackageReference Include="NfcReaderLib" Version="1.0.0" />
```

## ?? Target Framework

- **.NET Framework 4.7.2**

## ? Features

- ?? **PC/SC Communication** - Low-level smart card reader communication
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ??? **Card Utilities** - Helper functions for card data processing
- ?? **Data Formatting** - Byte array and hex string conversions
- ?? **TLV Parsing Support** - Tag-Length-Value structure handling

## ?? Quick Start

### Basic PC/SC Communication

```csharp
using NfcReaderLib;

// Initialize card reader
var reader = new CardReader();
reader.Initialize();

// Connect to card
reader.Connect("ACS ACR122U PICC Interface 0");

// Send APDU command
byte[] apdu = { 0x00, 0xA4, 0x04, 0x00, 0x00 };
byte[] response = reader.SendApdu(apdu);
```

### SL Token Generation

```csharp
using NfcReaderLib;

// Generate SL Token from ICC certificate
byte[] iccCert = GetIccCertificate(); // Tag 9F46
string slToken = SLCard.GenerateToken(iccCert);

// Output: "E3 B0 C4 42 98 FC 1C 14..." (SHA-256 hash)
```

### Utility Functions

```csharp
using NfcReaderLib;

// Convert hex string to byte array
byte[] data = Util.HexStringToByteArray("00A4040000");

// Convert byte array to hex string
string hex = Util.ByteArrayToHexString(data);

// Format card number
string pan = Util.FormatCardNumber("1234567890123456");
// Output: "1234 5678 9012 3456"
```

## ?? Main Components

### SLCard Class
**Purpose:** Card data model with ICC certificate parsing and token generation

**Key Methods:**
- `GenerateToken(byte[] iccCertificate)` - Generate SL Token from ICC cert
- `ParseIccCertificate(byte[] data)` - Parse ICC public key certificate

**Properties:**
- `CardNumber` - Primary Account Number (PAN)
- `ExpiryDate` - Card expiration date
- `CardholderName` - Name on card
- `IccCertificate` - ICC Public Key Certificate (Tag 9F46)
- `SlToken` - Generated secure link token

### Util Class
**Purpose:** Utility functions for data conversion and formatting

**Key Methods:**
- `HexStringToByteArray(string hex)` - Convert hex to bytes
- `ByteArrayToHexString(byte[] bytes)` - Convert bytes to hex
- `FormatCardNumber(string pan)` - Format PAN with spaces
- `FormatExpiryDate(string date)` - Format expiry as MM/YY

## ?? SL Token Details

### What is SL Token?

The **SL (Secure Link) Token** is a unique cryptographic identifier derived from the card's ICC Public Key Certificate using SHA-256 hashing.

### Token Generation Process

```
ICC Certificate (Tag 9F46) ? SHA-256 Hash ? Hex String ? SL Token
```

**Example:**
```csharp
byte[] iccCert = new byte[] { /* ICC cert data */ };
string token = SLCard.GenerateToken(iccCert);
// Output: "E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24..."
```

### Properties

? **Unique** - Each card generates unique token  
? **Consistent** - Same card always generates same token  
? **Secure** - One-way hash, cannot be reversed  
? **Privacy-Friendly** - No PAN or sensitive data exposed  
? **Format** - Space-separated hex string (95 characters)

### Use Cases

- ?? Loyalty program card identification
- ?? Card binding to user accounts
- ?? Transaction correlation
- ?? Duplicate card detection
- ?? Analytics without storing sensitive data

## ?? Requirements

### Runtime Requirements
- .NET Framework 4.7.2 or later
- Windows 7 or later
- PC/SC smart card service (SCardSvr)

### Dependencies
- **System.Security.Cryptography.Algorithms** (4.3.0) - For SHA-256 hashing

## ?? PC/SC Integration

### Supported Readers

This library works with any PC/SC compliant smart card reader:

- ? ACR122U (contactless)
- ? SCM SCR331 (contact)
- ? Omnikey 5321 (dual interface)
- ? Generic PC/SC readers

### PC/SC Service

Ensure the PC/SC service is running:

```powershell
# Check service status
Get-Service SCardSvr

# Start service if stopped
Start-Service SCardSvr
```

## ?? Documentation

For complete documentation, see the main project:
- [EMVReaderSL Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Contributing

Contributions welcome! Please submit issues and pull requests on GitHub.

## ?? License

Copyright © Johan Henningsson 2024

This project is licensed under the MIT License.

## ?? Related Packages

- **EMVCard.Core** - High-level EMV card reading functionality
- **AztecQRGenerator.Core** - QR code generation for card data

## ?? Author

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Support

If you find this library useful, please ? star the repository on GitHub!

---

**Version 1.0.0** | **.NET Framework 4.7.2** | **MIT License**
