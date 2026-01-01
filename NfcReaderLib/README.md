# EMV Card Reader with SL Token Generation

A professional Windows Forms application for reading EMV chip cards (contact and contactless) using PC/SC readers, with integrated SL (Secure Link) Token generation based on ICC Public Key Certificates.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![C#](https://img.shields.io/badge/C%23-7.3-green)
![License](https://img.shields.io/badge/license-MIT-blue)
![Status](https://img.shields.io/badge/status-active-success)

## ?? Features

### Core Functionality
- ? **PC/SC Card Reader Support** - Works with any PC/SC compliant card reader
- ? **Contact & Contactless** - Supports both contact (chip) and contactless (NFC) cards
- ? **PSE/PPSE Support** - Payment System Environment enumeration for card applications
- ? **EMV Compliant** - Full EMV v4.3 TLV parsing and data extraction
- ? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ? **Multi-Application Cards** - Handles cards with multiple payment applications

### Data Extraction
- ?? **Card Number (PAN)** - Primary Account Number
- ?? **Expiry Date** - Card expiration information
- ?? **Cardholder Name** - Name embossed on card
- ?? **Track 2 Data** - Magnetic stripe equivalent data
- ?? **ICC Certificate** - ICC Public Key Certificate (Tag 9F46)
- ?? **SL Token** - Unique card identifier (SHA-256 hash of ICC cert)

### Technical Features
- ??? **Clean Architecture** - Separated business logic from UI
- ?? **Comprehensive Logging** - Detailed APDU command/response logs
- ?? **Auto Error Handling** - Automatic handling of common APDU errors (6C, 67, 61)
- ?? **User-Friendly UI** - Intuitive interface with clear feedback
- ?? **Testable Design** - Business logic classes can be unit tested

## ?? Requirements

### Hardware
- PC/SC compatible smart card reader (contact or contactless)
- EMV chip card (Visa, Mastercard, UnionPay, etc.)

### Software
- Windows 7 or later
- .NET Framework 4.7.2 or later
- Visual Studio 2017 or later (for development)

### Tested Readers
- ACR122U (contactless)
- SCM SCR331 (contact)
- Omnikey 5321 (dual interface)
- Generic PC/SC readers

- ?? **PC/SC Communication** - Low-level smart card reader communication
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ??? **Card Utilities** - Helper functions for card data processing
- ?? **Data Formatting** - Byte array and hex string conversions
- ?? **TLV Parsing Support** - Tag-Length-Value structure handling

## ?? Quick Start

### From Source
1. Clone the repository:
```bash
git clone https://github.com/johanhenningsson4-hash/EMVReaderSL.git
cd EMVReaderSLCard
```

2. Open in Visual Studio:
```bash
start EMVReaderSL.sln
```

3. Build and run (F5)

### Basic Usage

1. **Initialize Reader** ? Click "Initialize" ? Select reader ? Click "Connect Card"
2. **Load Applications** ? Click "Load PSE" (contact) or "Load PPSE" (contactless)
3. **Read Card Data** ? Select application ? Click "ReadApp"
4. **View Results** ? Card data and SL Token displayed automatically

## ??? Architecture

### Clean Architecture Design

// Generate SL Token from ICC certificate
byte[] iccCert = GetIccCertificate(); // Tag 9F46
string slToken = SLCard.GenerateToken(iccCert);

// Output: "E3 B0 C4 42 98 FC 1C 14..." (SHA-256 hash)
```
????????????????????????????????????????????????
?           Presentation Layer                 ?
?         (EMVReader.cs - WinForms)           ?
????????????????????????????????????????????????
                    ?
????????????????????????????????????????????????
?          Business Logic Layer                ?
????????????????????????????????????????????????
?  • EmvCardReader         • EmvDataParser     ?
?  • EmvApplicationSelector • EmvRecordReader  ?
?  • EmvGpoProcessor       • EmvTokenGenerator ?
????????????????????????????????????????????????
                    ?
????????????????????????????????????????????????
?         Infrastructure Layer                 ?
?  • ModWinsCard64 (PC/SC)  • SLCard • Util    ?
????????????????????????????????????????????????
```

### Project Structure
```
EMVReaderSLCard/
??? EMVReader.cs              # Main UI Form (250 lines)
??? EmvCardReader.cs          # PC/SC communication (315 lines)
??? EmvDataParser.cs          # TLV parsing (280 lines)
??? EmvRecordReader.cs        # Record reading (150 lines)
??? EmvApplicationSelector.cs # PSE/PPSE handling (320 lines)
??? EmvGpoProcessor.cs        # GPO command (200 lines)
??? EmvTokenGenerator.cs      # SL Token generation (150 lines)
??? SLCard.cs                 # Card model with ICC parser
??? Util.cs                   # Utility functions
??? ModWinsCard64.cs          # PC/SC wrapper
```

## ?? SL Token

### What is it?

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

### How it works

```
ICC Certificate (Tag 9F46) ? SHA-256 Hash ? Hex String ? SL Token
```

**Example Output:**
```
E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55
```

### Properties

? **Unique** - Each card generates unique token  
? **Consistent** - Same card = same token  
? **Secure** - One-way hash, cannot be reversed  
? **Privacy-Friendly** - No PAN or sensitive data exposed  
? **Format** - Space-separated hex (95 characters)  

### Use Cases

- ?? Loyalty program card identification
- ?? Card binding to user accounts
- ?? Transaction correlation
- ? Duplicate card detection
- ?? Analytics without storing sensitive data

## ?? Key Classes

### EmvCardReader
**Purpose:** PC/SC card reader communication

**Methods:**
- `Initialize()` - Detect card readers
- `Connect(readerName)` - Connect to card
- `SendApduWithAutoFix()` - Send APDU with auto-retry
- `Disconnect()` / `Release()` - Cleanup

### EmvDataParser
**Purpose:** TLV parsing and data extraction

**Methods:**
- `ParseTLV()` - Parse EMV TLV structures
- `ParseAFL()` - Extract Application File Locator
- `ExtractFromTrack2()` - Extract missing data from Track2

**Supported Tags:**
- `5A` - PAN (Card Number)
- `5F24` - Expiry Date
- `5F20` - Cardholder Name
- `57` - Track 2 Data
- `9F46/47/48` - ICC Certificate/Exponent/Remainder

### EmvTokenGenerator
**Purpose:** SL Token generation

**Methods:**
- `GenerateToken(cardData, pan, aid)` - Generate from card data
- `GenerateTokenFromCertificate(certificate)` - Generate from cert bytes

**Returns:** `TokenResult` with `Success`, `Token`, or `ErrorMessage`

## ??? EMV Tags Reference

| Tag | Name | Description |
|-----|------|-------------|
| `5A` | Application PAN | Card number |
| `5F24` | Expiration Date | YYMMDD format |
| `5F20` | Cardholder Name | Name on card |
| `57` | Track 2 Equivalent | Magnetic data |
| `9F46` | ICC Public Key Cert | For DDA/CDA |
| `9F47` | ICC PK Exponent | Usually 03 |
| `9F48` | ICC PK Remainder | Extra modulus |
| `9F38` | PDOL | Processing options |
| `94` | AFL | File locator |

## ?? Troubleshooting

### No Card Readers Found
1. Check USB connection
2. Install reader drivers
3. Restart PC/SC service: `net stop SCardSvr && net start SCardSvr`

### No Applications Found
1. Try both PSE and PPSE
2. Card may not support PSE
3. Check APDU logs for errors (`6A 82` = File not found)

### SL Token Error
1. Verify card supports DDA/CDA (has Tag 9F46)
2. Some cards only support SDA (no ICC cert)
3. Check logs for "ICC Public Key Certificate"

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

Comprehensive documentation available in the repository:

- ?? [REFACTORING_DOCUMENTATION.md](REFACTORING_DOCUMENTATION.md) - Architecture details
- ?? [ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md](ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md) - ICC parsing
- ?? [SL_TOKEN_INTEGRATION_DOCUMENTATION.md](SL_TOKEN_INTEGRATION_DOCUMENTATION.md) - Token generation
- ?? [COMBOBOX_SELECTION_FIX.md](COMBOBOX_SELECTION_FIX.md) - UI fixes
- ?? [CLEARBUFFERS_FIX.md](CLEARBUFFERS_FIX.md) - Buffer management
- ?? [SL_TOKEN_FORMAT_UPDATE.md](SL_TOKEN_FORMAT_UPDATE.md) - Token formatting

## ??? Development

### Building

```bash
# Clone repository
git clone https://github.com/johanhenningsson4-hash/EMVReaderSL.git
cd EMVReaderSLCard

# Build
msbuild EMVReaderSL.sln /p:Configuration=Release
```

### Code Style
- **Naming:** PascalCase for public, _camelCase for private
- **Logging:** TraceSource for all classes
- **Error Handling:** Try-catch with specific messages
- **Documentation:** XML docs for public members

## ?? Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create feature branch
3. Commit changes
4. Push and create Pull Request

## ?? License

Copyright (C) Johan Henningsson

This project is licensed under the MIT License.

## ?? Acknowledgments

- EMV specifications by EMVCo
- PC/SC Workgroup
- Original EMVReader by Eternal TUTU (2008)

## ?? Contact

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Version History

### Version 2.0 (2024)
- ? Refactored architecture (6 business logic classes)
- ? Added SL Token generation
- ? Enhanced TLV parsing
- ? Comprehensive logging
- ?? Fixed UI selection issues
- ?? Complete documentation

### Version 1.0 (2008)
- Initial EMV card reader
- Basic PSE support
- TLV parsing

## ??? Roadmap

- [ ] Async/await operations
- [ ] Export to JSON/XML
- [ ] Configuration file
- [ ] DDA/CDA verification
- [ ] Multiple languages
- [ ] Web API integration

---

**Made with ?? by Johan Henningsson** | **2008-2024**

? **Star this repo if you find it useful!**