# EMVCard.Core

High-level EMV chip card reading library with support for PSE/PPSE application selection, GPO processing, record reading, TLV parsing, and SL Token generation.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![NuGet](https://img.shields.io/nuget/v/EMVCard.Core)
![License](https://img.shields.io/badge/license-MIT-blue)

## ?? Installation

### Package Manager Console
```powershell
Install-Package EMVCard.Core
```

### .NET CLI
```bash
dotnet add package EMVCard.Core
```

### PackageReference
```xml
<PackageReference Include="EMVCard.Core" Version="1.0.0" />
```

## ?? Target Framework

- **.NET Framework 4.7.2**

Compatible with:
- .NET Framework 4.7.2+
- .NET Standard 2.0+
- .NET Core 2.0+
- .NET 5+

## ? Features

### Core EMV Functionality
- ??? **PSE/PPSE Support** - Payment System Environment enumeration (contact & contactless)
- ?? **Multi-Application Cards** - Handle cards with multiple payment applications
- ?? **GPO Processing** - Get Processing Options command handling with PDOL
- ?? **Record Reading** - Read card records via AFL (Application File Locator)
- ??? **TLV Parsing** - Complete EMV Tag-Length-Value parsing
- ?? **SL Token Generation** - SHA-256 based tokens from ICC certificates
- ? **Async Operations** - Non-blocking asynchronous card operations

### Data Extraction
- ?? **Card Number (PAN)** - Primary Account Number (Tag 5A)
- ?? **Expiry Date** - Card expiration (Tag 5F24)
- ?? **Cardholder Name** - Name on card (Tag 5F20)
- ?? **Track 2 Data** - Magnetic stripe equivalent (Tag 57)
- ?? **ICC Certificate** - ICC Public Key Certificate (Tag 9F46)
- ?? **SL Token** - Unique card identifier

## ?? Quick Start

### Basic Card Reading

```csharp
using EMVCard;
using NfcReaderLib;

// Step 1: Initialize card reader
var cardReader = new EmvCardReader();
cardReader.LogMessage += (s, msg) => Console.WriteLine(msg);

var readers = await cardReader.InitializeAsync();
await cardReader.ConnectAsync(readers[0]);

// Step 2: Load applications
var appSelector = new EmvApplicationSelector(cardReader);
appSelector.LogMessage += (s, msg) => Console.WriteLine(msg);

var apps = await appSelector.LoadPPSEAsync(); // or LoadPSEAsync() for contact

// Step 3: Select application
var selectedApp = apps[0];
var (selectSuccess, fciData) = await appSelector.SelectApplicationAsync(selectedApp.AID);

if (!selectSuccess)
{
    Console.WriteLine("Failed to select application");
    return;
}

// Step 4: Send GPO
var gpoProcessor = new EmvGpoProcessor(cardReader);
var (gpoSuccess, gpoResponse) = await gpoProcessor.SendGPOAsync(fciData);

// Step 5: Parse and read records
var dataParser = new EmvDataParser();
var cardData = new EmvDataParser.EmvCardData();

if (gpoSuccess)
{
    dataParser.ParseTLV(gpoResponse, 0, gpoResponse.Length - 2, cardData, 0);
    
    var aflList = dataParser.ParseAFL(gpoResponse, gpoResponse.Length);
    
    var recordReader = new EmvRecordReader(cardReader, dataParser);
    await recordReader.ReadAFLRecordsAsync(aflList, cardData);
}

// Step 6: Extract missing data from Track2
dataParser.ExtractFromTrack2(cardData);

// Step 7: Generate SL Token
var tokenGenerator = new EmvTokenGenerator();
var tokenResult = await tokenGenerator.GenerateTokenAsync(cardData, cardData.PAN, selectedApp.AID);

// Step 8: Display results
Console.WriteLine($"Card Number: {cardData.PAN}");
Console.WriteLine($"Expiry Date: {cardData.ExpiryDate}");
Console.WriteLine($"Cardholder: {cardData.CardholderName}");
Console.WriteLine($"SL Token: {tokenResult.Token}");

// Cleanup
await cardReader.DisconnectAsync();
await cardReader.ReleaseAsync();
```

## ?? Main Classes

### EmvCardReader
**Purpose:** PC/SC card reader communication and APDU handling

**Key Methods:**
- `InitializeAsync()` - Detect available card readers
- `ConnectAsync(readerName)` - Connect to specific reader
- `SendApduWithAutoFix(apdu, out response)` - Send APDU with automatic error retry
- `DisconnectAsync()` / `ReleaseAsync()` - Cleanup

**Features:**
- ? Automatic 6C/67 Le adjustment
- ? Automatic 61 XX GET RESPONSE
- ? Comprehensive APDU logging
- ? Async/await support

**Example:**
```csharp
var cardReader = new EmvCardReader();
cardReader.LogMessage += (s, msg) => Console.WriteLine(msg);

var readers = await cardReader.InitializeAsync();
bool connected = await cardReader.ConnectAsync(readers[0]);

byte[] selectCmd = new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x07, 0xA0, 0x00, 0x00, 0x00, 0x03, 0x10, 0x10, 0x00 };
bool success = cardReader.SendApduWithAutoFix(selectCmd, out byte[] response);
```

### EmvApplicationSelector
**Purpose:** PSE/PPSE application enumeration and selection

**Key Methods:**
- `LoadPSEAsync()` - Load Payment System Environment (contact cards)
- `LoadPPSEAsync()` - Load Proximity PSE (contactless cards)
- `SelectApplicationAsync(aid)` - Select specific application

**Returns:**
- `List<ApplicationInfo>` - List of available applications
- `(bool success, byte[] fciData)` - Selection result with FCI template

**Supported AIDs:**
- Visa (A0000000031010)
- Mastercard (A0000000041010)
- UnionPay, Discover, JCB, Amex, etc.

**Example:**
```csharp
var appSelector = new EmvApplicationSelector(cardReader);

// For contactless cards
var apps = await appSelector.LoadPPSEAsync();

foreach (var app in apps)
{
    Console.WriteLine($"{app.DisplayName} - AID: {app.AID}");
}

var (success, fciData) = await appSelector.SelectApplicationAsync(apps[0].AID);
```

### EmvGpoProcessor
**Purpose:** Get Processing Options (GPO) command handling

**Key Methods:**
- `SendGPOAsync(fciData)` - Execute GPO command with PDOL processing

**Returns:**
- `(bool success, byte[] gpoResponse)` - GPO result with AIP and AFL

**Features:**
- ? Automatic PDOL extraction from FCI
- ? Default values for common PDOL tags
- ? Fallback to simplified GPO if PDOL fails

**Example:**
```csharp
var gpoProcessor = new EmvGpoProcessor(cardReader);
var (gpoSuccess, gpoResponse) = await gpoProcessor.SendGPOAsync(fciData);

if (gpoSuccess)
{
    // GPO response contains AIP and AFL
}
```

### EmvRecordReader
**Purpose:** Read card records specified in AFL

**Key Methods:**
- `ReadAFLRecordsAsync(aflList, cardData)` - Read all AFL records
- `TryReadCommonRecordsAsync(cardData)` - Fallback to common SFI/record combinations

**Features:**
- ? Handles multiple SFI (Short File Identifier) files
- ? Reads all records in AFL range
- ? Automatic TLV parsing of record data
- ? Fallback to common records if AFL parsing fails

**Example:**
```csharp
var recordReader = new EmvRecordReader(cardReader, dataParser);

var aflList = dataParser.ParseAFL(gpoResponse, gpoResponse.Length);
await recordReader.ReadAFLRecordsAsync(aflList, cardData);

// Or try common records if AFL is not available
await recordReader.TryReadCommonRecordsAsync(cardData);
```

### EmvDataParser
**Purpose:** TLV parsing and data extraction

**Key Methods:**
- `ParseTLV(buffer, startIndex, endIndex, cardData, priority)` - Parse EMV TLV structures
- `ParseAFL(buffer, length)` - Extract Application File Locator
- `ExtractFromTrack2(cardData)` - Extract PAN/expiry from Track 2

**Supported Tags:**
- `5A` - Application PAN
- `5F20` - Cardholder Name
- `5F24` - Expiration Date
- `57` - Track 2 Equivalent Data
- `9F46` - ICC Public Key Certificate
- `9F47` - ICC Public Key Exponent
- `9F48` - ICC Public Key Remainder
- All standard EMV tags

**Features:**
- ? Recursive TLV parsing
- ? Priority-based field updates
- ? Nested template support (tags 70, 77)
- ? Comprehensive tag recognition

**Example:**
```csharp
var dataParser = new EmvDataParser();
var cardData = new EmvDataParser.EmvCardData();

dataParser.ParseTLV(gpoResponse, 0, gpoResponse.Length - 2, cardData, 0);

var aflList = dataParser.ParseAFL(gpoResponse, gpoResponse.Length);
dataParser.ExtractFromTrack2(cardData);

Console.WriteLine($"PAN: {cardData.PAN}");
Console.WriteLine($"Expiry: {cardData.ExpiryDate}");
Console.WriteLine($"Name: {cardData.CardholderName}");
```

### EmvTokenGenerator
**Purpose:** SL Token generation from ICC certificates

**Key Methods:**
- `GenerateTokenAsync(cardData, pan, aid)` - Generate token from card data
- `GenerateTokenFromCertificateAsync(certificate)` - Generate from certificate bytes

**Returns:**
- `TokenResult` with:
  - `bool Success` - Whether generation succeeded
  - `string Token` - Generated SL Token (if successful)
  - `string ErrorMessage` - Error details (if failed)

**Example:**
```csharp
var tokenGenerator = new EmvTokenGenerator();
tokenGenerator.LogMessage += (s, msg) => Console.WriteLine(msg);

var tokenResult = await tokenGenerator.GenerateTokenAsync(
    cardData, 
    cardData.PAN, 
    selectedApp.AID
);

if (tokenResult.Success)
{
    Console.WriteLine($"SL Token: {tokenResult.Token}");
}
else
{
    Console.WriteLine($"Error: {tokenResult.ErrorMessage}");
}
```

## ??? EMV Tags Reference

### Card Data Tags

| Tag | Name | Description | Type |
|-----|------|-------------|------|
| `5A` | Application PAN | Primary Account Number | Binary |
| `5F20` | Cardholder Name | Name on card | ASCII |
| `5F24` | Expiration Date | YYMMDD format | Binary |
| `5F25` | Effective Date | Card effective date | Binary |
| `5F28` | Issuer Country Code | ISO 3166-1 numeric | Binary |
| `57` | Track 2 Equivalent | Magnetic stripe data | Binary |

### Processing Tags

| Tag | Name | Description | Type |
|-----|------|-------------|------|
| `9F38` | PDOL | Processing Options DOL | Binary |
| `94` | AFL | Application File Locator | Binary |
| `82` | AIP | Application Interchange Profile | Binary |

### Cryptographic Tags

| Tag | Name | Description | Type |
|-----|------|-------------|------|
| `9F46` | ICC Public Key Certificate | For DDA/CDA | Binary |
| `9F47` | ICC Public Key Exponent | Usually 03 | Binary |
| `9F48` | ICC Public Key Remainder | Extra modulus bytes | Binary |

### Template Tags

| Tag | Name | Description | Type |
|-----|------|-------------|------|
| `70` | Data Template | EMV response template | Constructed |
| `77` | Response Template Format 2 | Response data | Constructed |
| `61` | Application Template | Application info | Constructed |

## ?? SL Token Details

### What is SL Token?

The **SL (Secure Link) Token** is a unique cryptographic identifier derived from the card's ICC Public Key Certificate using SHA-256 hashing.

### How It Works

```
ICC Certificate (Tag 9F46) ? SHA-256 Hash ? Hex String ? SL Token
```

**Example Output:**
```
E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55
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

### Error Handling

```csharp
try
{
    var cardReader = new EmvCardReader();
    var readers = await cardReader.InitializeAsync();
    
    if (readers.Count == 0)
    {
        Console.WriteLine("No card readers found");
        return;
    }
    
    bool connected = await cardReader.ConnectAsync(readers[0]);
    if (!connected)
    {
        Console.WriteLine("Failed to connect to card");
        return;
    }
    
    // ... card operations ...
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    await cardReader?.DisconnectAsync();
    await cardReader?.ReleaseAsync();
}
```

### Logging Integration

```csharp
// Subscribe to log events from all components
var cardReader = new EmvCardReader();
var appSelector = new EmvApplicationSelector(cardReader);
var gpoProcessor = new EmvGpoProcessor(cardReader);
var recordReader = new EmvRecordReader(cardReader, dataParser);
var tokenGenerator = new EmvTokenGenerator();

// Centralized logging
EventHandler<string> logHandler = (s, msg) => 
{
    File.AppendAllText("card_operations.log", $"[{DateTime.Now}] {msg}\n");
    Console.WriteLine(msg);
};

cardReader.LogMessage += logHandler;
appSelector.LogMessage += logHandler;
gpoProcessor.LogMessage += logHandler;
recordReader.LogMessage += logHandler;
tokenGenerator.LogMessage += logHandler;
```

### Custom PDOL Values

```csharp
// The GPO processor uses these default PDOL values:
// - Terminal Transaction Qualifiers (9F66): 26000000
// - Amount Authorized (9F02): 000000000100
// - Amount Other (9F03): 000000000000
// - Transaction Date (9A): Current date (YYMMDD)
// - Transaction Type (9C): 00 (Purchase)
// - Unpredictable Number (9F37): Random 4 bytes

// To customize, you can modify the EmvGpoProcessor source or
// send GPO command directly through EmvCardReader
```

## ?? Dependencies

- **NfcReaderLib** (v1.0.0) - PC/SC communication and utilities

## ?? Requirements

- .NET Framework 4.7.2 or later
- Windows 7 or later (for PC/SC support)
- PC/SC compatible smart card reader
- EMV chip card (Visa, Mastercard, etc.)

### Tested Readers

- ? ACR122U (contactless)
- ? SCM SCR331 (contact)
- ? Omnikey 5321 (dual interface)
- ? Generic PC/SC readers

## ? Troubleshooting

### No Applications Found

**Problem:** PSE/PPSE returns no applications

**Solutions:**
1. Try both `LoadPSEAsync()` (contact) and `LoadPPSEAsync()` (contactless)
2. Card may not support PSE - check APDU logs for `6A 82` (File not found)
3. Ensure card is properly seated in reader

### SL Token Generation Error

**Problem:** Token generation fails

**Solutions:**
1. Verify card supports DDA/CDA (check for Tag 9F46 in logs)
2. Some cards only support SDA (no ICC certificate available)
3. Check logs for "ICC Public Key Certificate" presence

### Common Status Words

| Status Word | Meaning | Auto-Handled | Action |
|-------------|---------|--------------|--------|
| `90 00` | Success | - | None |
| `6C XX` | Wrong Le | ? Yes | Automatic retry with correct Le |
| `67 00` | Wrong length | ? Yes | Automatic retry |
| `61 XX` | More data available | ? Yes | Automatic GET RESPONSE |
| `6A 82` | File not found | ? No | Try alternative method (PPSE vs PSE) |
| `6A 83` | Record not found | ? No | Use TryReadCommonRecords |
| `6985` | Conditions not satisfied | ? No | Card may be locked |
| `6A 86` | Incorrect P1/P2 | ? No | Check command parameters |

## ?? Documentation

For complete documentation and usage examples, visit:
- [GitHub Repository](https://github.com/johanhenningsson4-hash/EMVReaderSL)
- [NuGet Package](https://www.nuget.org/packages/EMVCard.Core)
- [NfcReaderLib Package](https://www.nuget.org/packages/NfcReaderLib)

## ?? Related Packages

- **NfcReaderLib** - Low-level PC/SC communication and utilities (required dependency)

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
- Original EMVReader by Eternal TUTU (2008)

---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this package on GitHub if you find it useful!**