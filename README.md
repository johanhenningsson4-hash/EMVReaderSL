# EMV Card Reader with SL Token Generation

A professional Windows Forms application for reading EMV chip cards (contact and contactless) using PC/SC readers, with integrated SL (Secure Link) Token generation based on ICC Public Key Certificates.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![C#](https://img.shields.io/badge/C%23-7.3-green)
![License](https://img.shields.io/badge/license-MIT-blue)
![Status](https://img.shields.io/badge/status-active-success)
![NuGet](https://img.shields.io/badge/NuGet-v1.0.1-blue)

## ?? NuGet Packages

This project is available as NuGet packages for easy integration into your applications:

| Package | Version | Downloads | Description |
|---------|---------|-----------|-------------|
| [**NfcReaderLib**](https://www.nuget.org/packages/NfcReaderLib) | 1.0.1 | ![NuGet](https://img.shields.io/nuget/dt/NfcReaderLib) | PC/SC communication, SL Token generation, utilities |
| [**EMVCard.Core**](https://www.nuget.org/packages/EMVCard.Core) | 1.0.1 | ![NuGet](https://img.shields.io/nuget/dt/EMVCard.Core) | EMV card reading, PSE/PPSE, GPO, TLV parsing |

### Installation

**Package Manager Console:**
```powershell
Install-Package NfcReaderLib -Version 1.0.1
Install-Package EMVCard.Core -Version 1.0.1
```

**.NET CLI:**
```bash
dotnet add package NfcReaderLib --version 1.0.1
dotnet add package EMVCard.Core --version 1.0.1
```

**PackageReference:**
```xml
<ItemGroup>
  <PackageReference Include="NfcReaderLib" Version="1.0.1" />
  <PackageReference Include="EMVCard.Core" Version="1.0.1" />
</ItemGroup>
```

## ? Features

### Core Functionality
- ?? **PC/SC Card Reader Support** - Works with any PC/SC compliant card reader
- ?? **Contact & Contactless** - Supports both contact (chip) and contactless (NFC) cards
- ??? **PSE/PPSE Support** - Payment System Environment enumeration for card applications
- ?? **EMV Compliant** - Full EMV v4.3 TLV parsing and data extraction
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ?? **Multi-Application Cards** - Handles cards with multiple payment applications
- ?? **Card Polling** - Automated continuous card reading with configurable intervals
- ?? **PAN Masking** - Optional masking of card numbers for privacy

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
- ? **Testable Design** - Business logic classes can be unit tested
- ? **Async Operations** - Non-blocking asynchronous card operations

## ?? Requirements

### Hardware
- PC/SC compatible smart card reader (contact or contactless)
- EMV chip card (Visa, Mastercard, UnionPay, etc.)

### Software
- Windows 7 or later
- .NET Framework 4.7.2 or later
- Visual Studio 2017 or later (for development)

### NuGet Dependencies
- **System.Security.Cryptography.Algorithms** (v4.3.1) - SHA-256 hashing

### Tested Readers
- ACR122U (contactless)
- SCM SCR331 (contact)
- Omnikey 5321 (dual interface)
- Generic PC/SC readers

## ?? Quick Start

### Using NuGet Packages (Recommended)

```csharp
using EMVCard;
using NfcReaderLib;

// Initialize card reader
var cardReader = new EmvCardReader();
var readers = await cardReader.InitializeAsync();
await cardReader.ConnectAsync(readers[0]);

// Load applications
var appSelector = new EmvApplicationSelector(cardReader);
var apps = await appSelector.LoadPPSEAsync();

// Select and read application
var (success, fciData) = await appSelector.SelectApplicationAsync(apps[0].AID);

// Process GPO
var gpoProcessor = new EmvGpoProcessor(cardReader);
var (gpoSuccess, gpoResponse) = await gpoProcessor.SendGPOAsync(fciData);

// Read records
var dataParser = new EmvDataParser();
var cardData = new EmvDataParser.EmvCardData();
dataParser.ParseTLV(gpoResponse, 0, gpoResponse.Length - 2, cardData, 0);

// Generate SL Token
var tokenGenerator = new EmvTokenGenerator();
var tokenResult = await tokenGenerator.GenerateTokenAsync(cardData, cardData.PAN, apps[0].AID);

Console.WriteLine($"PAN: {cardData.PAN}");
Console.WriteLine($"Expiry: {cardData.ExpiryDate}");
Console.WriteLine($"Token: {tokenResult.Token}");
```

### From Source
1. Clone the repository:
```bash
git clone https://github.com/johanhenningsson4-hash/EMVReaderSL.git
cd EMVReaderSLCard
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Open in Visual Studio:
```bash
start EMVReaderSL.sln
```

4. Build and run (F5)

### Basic Usage

1. **Initialize Reader** ? Click "Initialize" ? Select reader ? Click "Connect Card"
2. **Load Applications** ? Click "Load PSE" (contact) or "Load PPSE" (contactless)
3. **Read Card Data** ? Select application ? Click "ReadApp"
4. **View Results** ? Card data and SL Token displayed automatically

## ??? Architecture

### Clean Architecture Design

```
?????????????????????????????????????????????????
?           Presentation Layer                  ?
?         (EMVReader.cs - WinForms)            ?
?????????????????????????????????????????????????
                    ?
?????????????????????????????????????????????????
?          Business Logic Layer                 ?
?????????????????????????????????????????????????
?  • EmvCardReader          • EmvDataParser     ?
?  • EmvApplicationSelector  • EmvRecordReader  ?
?  • EmvGpoProcessor        • EmvTokenGenerator ?
?????????????????????????????????????????????????
                    ?
?????????????????????????????????????????????????
?         Infrastructure Layer                  ?
?  • ModWinsCard64 (PC/SC)   • SLCard • Util    ?
?????????????????????????????????????????????????
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

? **Unique:** One-of-a-kind for each card
? **Persistent:** Remains consistent across sessions
? **Secure:** Derived from cryptographic certificate
? **Non-reversible:** Cannot be converted back to ICC certificate

### Troubleshooting SL Token Issues

- **Common Issues:**
  - Token not generating
  - Invalid token format
  - Token changes with each read

- **Solutions:**
  - Ensure ICC certificate is present and valid
  - Check SHA-256 implementation
  - Verify buffer management in code
  - Consult [Logging Documentation](LOGGING_DOCUMENTATION.md) for APDU level debugging

## ??? Troubleshooting

### Common Issues

- **Card Not Detected:**
  - Ensure card is properly inserted/held near reader
  - Check reader connections and driver installation
  - Restart the application and try again

- **APDU Errors:**
  - Refer to [EMVCo Declined Transactions](https://www.emvco.com) for error code meanings
  - Common errors:
    - `6A82`: Inconsistent attributes
    - `6A84`: Requested data not available
    - `6282`: Encryption not successful

- **SL Token Issues:**
  - Ensure SL Token generation code is called after successful card data read
  - Validate ICC certificate presence (Tag 9F46)
  - Check SHA-256 hash implementation

- **Buffer Related Issues:**
  - Increase buffer size in code/configuration
  - Monitor buffer usage to prevent overflows

- **GUI Freezing:**
  - Ensure all card operations are awaited properly
  - Offload heavy processing from UI thread

### Debugging Tips

1. **Enable Detailed Logging**
   - Set log level to `Debug` or `Trace`
   - Reproduce the issue
   - Check logs for anomalies

2. **Use Test Cards**
   - For consistent results, use test cards with known data

3. **Consult Documentation**
   - Refer to individual class/method documentation for usage details

4. **Clean and Rebuild Solution**
   - Often resolves missing references or outdated binaries

5. **Inspect NuGet Package Contents**
   - Ensure all required files are included in the package

## ?? Version History

### Version 2.0.0 (January 2026) - Current Release
**Application Release:**
- ??? Complete architectural refactoring with 6 business logic classes
- ?? Integrated SL Token generation
- ? Async/await operations throughout
- ?? Card polling feature for automated reading
- ?? PAN masking for privacy compliance
- ?? Comprehensive logging with TraceSource
- ?? Fixed UI selection issues and buffer management
- ?? Complete documentation suite

**NuGet Packages:**
- ?? Published NfcReaderLib v1.0.1
- ?? Published EMVCard.Core v1.0.1
- ?? Updated copyright to 2026
- ?? Enhanced package documentation

### NuGet Package Releases

**v1.0.1 (January 2026)**
- ?? Updated copyright year to 2026
- ?? Improved package descriptions and documentation
- ?? Added comprehensive release notes
- ?? No functional code changes (documentation release)

**v1.0.0 (2025)**
- ?? Initial NuGet package release
- ?? Core EMV card reading functionality
- ?? SL Token generation from ICC certificates
- ?? PC/SC smart card reader communication
- ?? EMV TLV parsing and data extraction

### Version 1.0 (2008) - Original Release
- ?? Initial EMV card reader implementation
- ??? Basic PSE support for contact cards
- ?? TLV parsing for EMV tags
- ?? Original work by Eternal TUTU

## ?? Documentation

Comprehensive documentation available in the repository:

- ?? [REFACTORING_DOCUMENTATION.md](REFACTORING_DOCUMENTATION.md) - Architecture details and design patterns
- ?? [ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md](ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md) - ICC certificate parsing
- ?? [SL_TOKEN_INTEGRATION_DOCUMENTATION.md](SL_TOKEN_INTEGRATION_DOCUMENTATION.md) - Token generation guide
- ?? [LOGGING_DOCUMENTATION.md](LOGGING_DOCUMENTATION.md) - Logging configuration and usage
- ?? [PAN_MASKING_FEATURE.md](PAN_MASKING_FEATURE.md) - PAN masking feature details
- ?? [CARD_POLLING_FEATURE.md](CARD_POLLING_FEATURE.md) - Polling feature documentation
- ?? [NUGET_PACKAGES_CREATED.md](NUGET_PACKAGES_CREATED.md) - NuGet package information
- ?? [COMBOBOX_SELECTION_FIX.md](COMBOBOX_SELECTION_FIX.md) - UI fixes
- ?? [CLEARBUFFERS_FIX.md](CLEARBUFFERS_FIX.md) - Buffer management
- ?? [SL_TOKEN_FORMAT_UPDATE.md](SL_TOKEN_FORMAT_UPDATE.md) - Token formatting

## ?? Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? License

**MIT License**

Copyright © Johan Henningsson 2008-2026

This project is licensed under the MIT License. See the LICENSE file for details.

## ?? Acknowledgments

- **EMVCo** - EMV specifications and standards
- **PC/SC Workgroup** - Smart card reader standards
- **Eternal TUTU** - Original EMVReader implementation (2008)
- **.NET Community** - Libraries and support

## ?? Contact

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)
- Issues: [GitHub Issues](https://github.com/johanhenningsson4-hash/EMVReaderSL/issues)

## ??? Roadmap

Future enhancements planned:

- [ ] **Export Functionality** - Export to JSON/XML formats
- [ ] **Configuration File** - External configuration support
- [ ] **DDA/CDA Verification** - Full cryptographic verification
- [ ] **Multi-language Support** - Internationalization (i18n)
- [ ] **Web API** - RESTful API wrapper
- [ ] **Database Integration** - Card data persistence
- [ ] **Reporting** - Transaction and analytics reports

## ?? Project Stats

- **Language:** C# 7.3
- **Framework:** .NET Framework 4.7.2
- **Projects:** 3 (Main app + 2 libraries)
- **NuGet Packages:** 2 published
- **Lines of Code:** ~2,000+
- **Documentation Files:** 15+
- **License:** MIT
- **First Release:** 2008
- **Current Version:** 2.0.0 (2026)

---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this repo if you find it useful!**

[![GitHub stars](https://img.shields.io/github/stars/johanhenningsson4-hash/EMVReaderSL?style=social)](https://github.com/johanhenningsson4-hash/EMVReaderSL/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/johanhenningsson4-hash/EMVReaderSL?style=social)](https://github.com/johanhenningsson4-hash/EMVReaderSL/network/members)