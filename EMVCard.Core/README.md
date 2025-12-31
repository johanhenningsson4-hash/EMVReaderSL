# EMVCard.Core

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue)
![License](https://img.shields.io/badge/license-MIT-blue)

EMV chip card reading library with support for PSE/PPSE application selection, GPO processing, record reading, TLV parsing, and SL Token generation. Works with contact and contactless EMV cards via PC/SC readers.

## ?? Installation

### NuGet Package Manager
```bash
Install-Package EMVCard.Core -Version 1.0.0
```

### .NET CLI
```bash
dotnet add package EMVCard.Core --version 1.0.0
```

### Package Reference
```xml
<PackageReference Include="EMVCard.Core" Version="1.0.0" />
```

## ?? Target Framework

- **.NET Framework 4.7.2**

## ? Features

### Core EMV Functionality
- ?? **PSE/PPSE Support** - Payment System Environment enumeration
- ?? **Multi-Application Cards** - Handle cards with multiple payment apps
- ?? **GPO Processing** - Get Processing Options command handling
- ?? **Record Reading** - Read card records via AFL (Application File Locator)
- ??? **TLV Parsing** - Complete EMV Tag-Length-Value parsing
- ?? **SL Token Generation** - SHA-256 based tokens from ICC certificates

### Data Extraction
- ?? **Card Number (PAN)** - Primary Account Number (Tag 5A)
- ?? **Expiry Date** - Card expiration (Tag 5F24)
- ?? **Cardholder Name** - Name on card (Tag 5F20)
- ?? **Track 2 Data** - Magnetic stripe equivalent (Tag 57)
- ?? **ICC Certificate** - ICC Public Key Certificate (Tag 9F46)
- ?? **SL Token** - Unique card identifier

### Technical Features
- ? **EMV v4.3 Compliant** - Full EMV specification support
- ?? **Auto Error Handling** - Automatic APDU retry (6C, 67, 61)
- ?? **Comprehensive Logging** - Detailed operation logs
- ?? **Clean Architecture** - Testable, maintainable design
- ?? **PC/SC Integration** - Works with any PC/SC reader

## ?? Quick Start

### Read Card Application

```csharp
using EMVCard.Core;

// Initialize card reader
var cardReader = new EmvCardReader();
cardReader.Initialize();
cardReader.Connect("ACS ACR122U PICC Interface 0");

// Load applications via PPSE (contactless)
var selector = new EmvApplicationSelector();
var apps = selector.LoadPPSE(cardReader);

// Select first application
if (apps.Count > 0)
{
    var selectedApp = apps[0];
    selector.SelectApplication(cardReader, selectedApp);
}
```

### Process GPO and Read Records

```csharp
// Process Get Processing Options
var gpoProcessor = new EmvGpoProcessor();
var gpoResult = gpoProcessor.ProcessGPO(cardReader, pdolData);

// Parse AFL and read records
var parser = new EmvDataParser();
var aflRecords = parser.ParseAFL(gpoResult);

var recordReader = new EmvRecordReader();
var recordData = recordReader.ReadRecords(cardReader, aflRecords);
```

### Parse Card Data

```csharp
// Parse TLV data
var cardData = parser.ParseTLV(recordData);

// Extract card information
string pan = cardData.ContainsKey("5A") 
    ? Encoding.ASCII.GetString(cardData["5A"]) 
    : null;

string expiryDate = cardData.ContainsKey("5F24") 
    ? Encoding.ASCII.GetString(cardData["5F24"]) 
    : null;

string cardholderName = cardData.ContainsKey("5F20") 
    ? Encoding.ASCII.GetString(cardData["5F20"]) 
    : null;
```

### Generate SL Token

```csharp
// Generate token from ICC certificate
var tokenGen = new EmvTokenGenerator();

if (cardData.ContainsKey("9F46"))
{
    byte[] iccCert = cardData["9F46"];
    var tokenResult = tokenGen.GenerateToken(iccCert);
    
    if (tokenResult.Success)
    {
        string slToken = tokenResult.Token;
        Console.WriteLine($"SL Token: {slToken}");
    }
}
```

## ?? Main Classes

### EmvCardReader
**Purpose:** PC/SC card reader communication and APDU handling

**Key Methods:**
- `Initialize()` - Detect available card readers
- `Connect(readerName)` - Connect to specific reader
- `SendApduWithAutoFix()` - Send APDU with automatic error retry
- `Disconnect()` / `Release()` - Cleanup and release resources

**Features:**
- Automatic 6C/67 Le adjustment
- Automatic 61 XX GET RESPONSE
- Comprehensive APDU logging

### EmvApplicationSelector
**Purpose:** PSE/PPSE application enumeration and selection

**Key Methods:**
- `LoadPSE(cardReader)` - Load Payment System Environment (contact)
- `LoadPPSE(cardReader)` - Load Proximity PSE (contactless)
- `SelectApplication(cardReader, app)` - Select specific application
- `ParseApplicationList(data)` - Parse FCI template

**Supported AIDs:**
- Visa (A0000000031010)
- Mastercard (A0000000041010)
- UnionPay, Discover, JCB, etc.

### EmvGpoProcessor
**Purpose:** Get Processing Options command handling

**Key Methods:**
- `ProcessGPO(cardReader, pdolData)` - Execute GPO command
- `BuildPDOL(pdolTags, cardData)` - Build PDOL data

**Returns:**
- GPO response data (AIP + AFL or data templates)

### EmvRecordReader
**Purpose:** Read card records specified in AFL

**Key Methods:**
- `ReadRecords(cardReader, aflRecords)` - Read all AFL records
- `ReadSingleRecord(cardReader, sfi, record)` - Read specific record

**Features:**
- Handles multiple SFI (Short File Identifier) files
- Reads all records in AFL range

### EmvDataParser
**Purpose:** TLV parsing and data extraction

**Key Methods:**
- `ParseTLV(data)` - Parse EMV TLV structures
- `ParseAFL(gpoResponse)` - Extract Application File Locator
- `ExtractFromTrack2(track2Data)` - Extract PAN/expiry from Track 2

**Supported Tags:**
- All standard EMV tags (5A, 5F20, 5F24, 57, 9F46, etc.)

### EmvTokenGenerator
**Purpose:** SL Token generation from ICC certificates

**Key Methods:**
- `GenerateToken(iccCertificate)` - Generate SHA-256 token

**Returns:**
- `TokenResult` with `Success`, `Token`, or `ErrorMessage`

## ??? EMV Tags Reference

| Tag | Name | Description | Type |
|-----|------|-------------|------|
| `5A` | Application PAN | Card number | Binary |
| `5F20` | Cardholder Name | Name on card | ASCII |
| `5F24` | Expiration Date | YYMMDD format | Binary |
| `57` | Track 2 Equivalent | Magnetic data | Binary |
| `9F38` | PDOL | Processing options DOL | Binary |
| `94` | AFL | Application File Locator | Binary |
| `9F46` | ICC Public Key Cert | For DDA/CDA | Binary |
| `9F47` | ICC PK Exponent | Usually 03 | Binary |
| `9F48` | ICC PK Remainder | Extra modulus | Binary |

## ?? SL Token

### Overview

The **SL (Secure Link) Token** is a unique cryptographic identifier derived from the card's ICC Public Key Certificate using SHA-256 hashing.

### Generation Process

```
ICC Certificate (Tag 9F46) ? SHA-256 Hash ? Hex String ? SL Token
```

**Example Output:**
```
E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55
```

### Properties

? **Unique** - Each card generates unique token  
? **Consistent** - Same card always generates same token  
? **Secure** - One-way hash, cannot be reversed  
? **Privacy-Friendly** - No PAN or sensitive data exposed  
? **EMV Compliant** - Based on ICC certificate standard

### Use Cases

- ?? Loyalty program card identification
- ?? Card binding to user accounts
- ?? Transaction correlation and analytics
- ?? Duplicate card detection
- ?? Secure card identification without PAN storage

## ?? Requirements

### Runtime Requirements
- .NET Framework 4.7.2 or later
- Windows 7 or later
- PC/SC smart card service (SCardSvr)
- EMV chip card (Visa, Mastercard, UnionPay, etc.)

### Dependencies
- **NfcReaderLib** (1.0.0) - PC/SC communication and utilities

### Hardware Requirements
- PC/SC compatible smart card reader (contact or contactless)

### Tested Readers
- ? ACR122U (contactless)
- ? SCM SCR331 (contact)
- ? Omnikey 5321 (dual interface)
- ? Generic PC/SC readers

## ?? Configuration

### PC/SC Service

Ensure the PC/SC service is running:

```powershell
# Check service status
Get-Service SCardSvr

# Start service if stopped
Start-Service SCardSvr

# Restart if needed
Restart-Service SCardSvr
```

### Reader Detection

```csharp
var cardReader = new EmvCardReader();
cardReader.Initialize();

// Get list of available readers
var readers = cardReader.GetReaders();
foreach (var reader in readers)
{
    Console.WriteLine($"Found reader: {reader}");
}
```

## ?? Complete Example

```csharp
using EMVCard.Core;

// Step 1: Initialize reader
var cardReader = new EmvCardReader();
cardReader.Initialize();
cardReader.Connect("ACS ACR122U PICC Interface 0");

// Step 2: Load applications
var selector = new EmvApplicationSelector();
var apps = selector.LoadPPSE(cardReader);

if (apps.Count == 0)
{
    Console.WriteLine("No applications found");
    return;
}

// Step 3: Select application
var selectedApp = apps[0];
selector.SelectApplication(cardReader, selectedApp);

// Step 4: Process GPO
var gpoProcessor = new EmvGpoProcessor();
var gpoResult = gpoProcessor.ProcessGPO(cardReader, null);

// Step 5: Read records
var parser = new EmvDataParser();
var aflRecords = parser.ParseAFL(gpoResult);

var recordReader = new EmvRecordReader();
var recordData = recordReader.ReadRecords(cardReader, aflRecords);

// Step 6: Parse card data
var cardData = parser.ParseTLV(recordData);

// Step 7: Generate SL Token
var tokenGen = new EmvTokenGenerator();
if (cardData.ContainsKey("9F46"))
{
    var tokenResult = tokenGen.GenerateToken(cardData["9F46"]);
    if (tokenResult.Success)
    {
        Console.WriteLine($"Card Number: {GetPAN(cardData)}");
        Console.WriteLine($"Expiry: {GetExpiry(cardData)}");
        Console.WriteLine($"SL Token: {tokenResult.Token}");
    }
}

// Step 8: Cleanup
cardReader.Disconnect();
cardReader.Release();
```

## ?? Troubleshooting

### No Applications Found
- ? Try both `LoadPSE()` (contact) and `LoadPPSE()` (contactless)
- ? Some cards don't support PSE
- ? Check APDU logs for error codes

### SL Token Generation Failed
- ? Card must support DDA/CDA (has Tag 9F46)
- ? Some cards only support SDA (no ICC certificate)
- ? Check if `9F46` tag is present in card data

### Common Status Words

| SW | Meaning | Auto-Handled |
|----|---------|--------------|
| `90 00` | Success | - |
| `6C XX` | Wrong Le | ? Yes |
| `67 00` | Wrong length | ? Yes |
| `61 XX` | More data | ? Yes |
| `6A 82` | File not found | ? No |
| `6A 83` | Record not found | ? No |

## ?? Documentation

For complete documentation, see:
- [EMVReaderSL Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)
- [ICC Public Key Parser Documentation](https://github.com/johanhenningsson4-hash/EMVReaderSL/blob/main/ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md)
- [SL Token Integration Documentation](https://github.com/johanhenningsson4-hash/EMVReaderSL/blob/main/SL_TOKEN_INTEGRATION_DOCUMENTATION.md)
- [Refactoring Documentation](https://github.com/johanhenningsson4-hash/EMVReaderSL/blob/main/REFACTORING_DOCUMENTATION.md)

## ?? Contributing

Contributions welcome! Please submit issues and pull requests on GitHub.

## ?? License

Copyright © Johan Henningsson 2024

This project is licensed under the MIT License.

## ?? Related Packages

- **NfcReaderLib** - Low-level PC/SC communication and utilities
- **AztecQRGenerator.Core** - QR code generation for card data

## ?? Author

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Support

If you find this library useful, please ? star the repository on GitHub!

---

**Version 1.0.0** | **.NET Framework 4.7.2** | **MIT License** | **EMV v4.3 Compliant**
