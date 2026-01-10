# EMV Card Reader with SL Token Generation

A professional Windows Forms application for reading EMV chip cards (contact and contactless) using PC/SC readers, with integrated SL (Secure Link) Token generation based on ICC Public Key Certificates.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![C#](https://img.shields.io/badge/C%23-7.3-green)
![License](https://img.shields.io/badge/license-MIT-blue)
[![Build Status](https://github.com/johanhenningsson4-hash/EMVReaderSL/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/johanhenningsson4-hash/EMVReaderSL/actions)
[![NuGet](https://img.shields.io/nuget/v/EMVCard.Core.svg)](https://www.nuget.org/packages/EMVCard.Core/)
[![Latest Release](https://img.shields.io/github/v/release/johanhenningsson4-hash/EMVReaderSL)](https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/latest)
![Platform](https://img.shields.io/badge/platform-x86%20%7C%20x64-lightgrey)

## ?? NuGet Packages

This project is available as NuGet packages for easy integration into your applications:

| Package | Version | Downloads | Description |
|---------|---------|-----------|-------------|
| [**NfcReaderLib**](https://www.nuget.org/packages/NfcReaderLib) | 1.0.3 | ![NuGet](https://img.shields.io/nuget/dt/NfcReaderLib) | PC/SC communication with 32/64-bit support, SL Token generation |
| [**EMVCard.Core**](https://www.nuget.org/packages/EMVCard.Core) | 1.0.2 | ![NuGet](https://img.shields.io/nuget/dt/EMVCard.Core) | EMV card reading, PSE/PPSE, GPO, TLV parsing |

### Installation

**Package Manager Console:**
```powershell
Install-Package NfcReaderLib -Version 1.0.3
Install-Package EMVCard.Core -Version 1.0.2
```

**.NET CLI:**
```bash
dotnet add package NfcReaderLib --version 1.0.3
dotnet add package EMVCard.Core --version 1.0.2
```

**PackageReference:**
```xml
<ItemGroup>
  <PackageReference Include="NfcReaderLib" Version="1.0.3" />
  <PackageReference Include="EMVCard.Core" Version="1.0.2" />
</ItemGroup>
```

## ? Features

### Core Functionality
- ?? **PC/SC Card Reader Support** - Works with any PC/SC compliant card reader
- ?? **Contact & Contactless** - Supports both contact (chip) and contactless (NFC) cards
- ?? **32-bit & 64-bit Support** - Automatic platform detection (x86/x64 Windows)
- ?? **PSE/PPSE Support** - Payment System Environment enumeration for card applications
- ?? **EMV Compliant** - Full EMV v4.3 TLV parsing and data extraction
- ?? **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- ?? **Multi-Application Cards** - Handles cards with multiple payment applications
- ?? **Card Polling** - Automated continuous card reading with configurable intervals
- ?? **PAN Masking** - Optional masking of card numbers for privacy

### New in v2.0.0

#### ?? Transaction Storage
- **Save Transaction Data** - Persist all card reading transactions
- **Multiple Storage Backends** - JSON file-based or SQLite database
- **Search and Filter** - Find transactions by PAN, date range, or transaction ID
- **Export Capabilities** - Export to JSON, XML, or CSV formats
- **Audit Trail** - Complete history of all card operations
- **Performance Tracking** - Processing time for each transaction

#### ?? CI/CD Pipeline
- **Automated Testing** - Unit and integration tests run automatically
- **Continuous Integration** - Build and test on every commit
- **Automated Releases** - One-click NuGet package publishing
- **GitHub Actions** - Full CI/CD automation
- **Quality Checks** - Code quality and security scanning
- **PR Validation** - Automatic pull request checks

#### ?? Comprehensive Testing
- **Unit Tests** - Complete test coverage for all components
- **Integration Tests** - End-to-end workflow testing
- **xUnit Framework** - Professional testing infrastructure
- **FluentAssertions** - Readable and maintainable test assertions
- **Test Automation** - Automated test execution in CI/CD

### Data Extraction
- ?? **Card Number (PAN)** - Primary Account Number
- ?? **Expiry Date** - Card expiration information
- ?? **Cardholder Name** - Name embossed on card
- ?? **Track 2 Data** - Magnetic stripe equivalent data
- ?? **ICC Certificate** - ICC Public Key Certificate (Tag 9F46)
- ?? **SL Token** - Unique card identifier (SHA-256 hash of ICC cert)

### Technical Features
- ??? **Clean Architecture** - Separated business logic from UI
- ??? **Platform Independent** - Automatic 32-bit/64-bit detection via ModWinsCard wrapper
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
- Windows 7 or later (32-bit or 64-bit)
- .NET Framework 4.7.2 or later
- Visual Studio 2017 or later (for development)

### NuGet Dependencies
- **System.Security.Cryptography.Algorithms** (v4.3.1) - SHA-256 hashing

### Tested Readers
- ACR122U (contactless)
- SCM SCR331 (contact)
- Omnikey 5321 (dual interface)
- Generic PC/SC readers

### Platform Compatibility
- ? **32-bit Windows** (x86) - Full support via ModWinsCard32
- ? **64-bit Windows** (x64) - Full support via ModWinsCard64
- ? **Any CPU** - Automatic platform detection
- ? **No configuration needed** - Platform detected at runtime

## ?? Quick Start

### Using Transaction Storage

```csharp
using EMVCard;
using EMVCard.Storage;

// Initialize storage
var storage = new JsonTransactionStorage("transactions");

// After reading card
var transaction = CardTransaction.FromCardData(cardData, readerName);
transaction.ProcessingTimeMs = 1234;
await storage.SaveAsync(transaction);

// Retrieve transactions
var allTransactions = await storage.GetAllAsync();
var recentTransactions = await storage.GetByDateRangeAsync(
    DateTime.Now.AddDays(-7), 
    DateTime.Now
);

// Export
await storage.ExportAsync("report.csv", ExportFormat.Csv);
```

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

## ?? CI/CD Pipeline

### Automated Workflows

This project uses GitHub Actions for continuous integration and deployment:

- **CI/CD Pipeline** - Build and test on every push
- **NuGet Publishing** - Automated package publishing on version tags
- **Release Creation** - One-click release workflow
- **Pull Request Checks** - Automatic validation of PRs

### Creating a Release

```bash
# Option 1: Via GitHub Actions
# Go to Actions ? Create Release ? Enter version ? Run

# Option 2: Via Git tag
git tag -a v2.0.0 -m "Release v2.0.0"
git push origin v2.0.0
# Automatic publishing to NuGet and GitHub Releases
```

See [CI/CD Documentation](.github/CI-CD-DOCUMENTATION.md) for complete details.

## ?? Documentation

Comprehensive documentation available in the repository:

### Core Features
- ?? [REFACTORING_DOCUMENTATION.md](REFACTORING_DOCUMENTATION.md) - Architecture details and design patterns
- ?? [README_ModWinsCard.md](NfcReaderLib/README_ModWinsCard.md) - Platform wrapper documentation
- ?? [MIGRATION_SUMMARY.md](NfcReaderLib/MIGRATION_SUMMARY.md) - Platform migration guide
- ?? [ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md](ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md) - ICC certificate parsing
- ?? [SL_TOKEN_INTEGRATION_DOCUMENTATION.md](SL_TOKEN_INTEGRATION_DOCUMENTATION.md) - Token generation guide
- ?? [LOGGING_DOCUMENTATION.md](LOGGING_DOCUMENTATION.md) - Logging configuration and usage

### New Features (v2.0.0)
- ?? [TRANSACTION_STORAGE_GUIDE.md](TRANSACTION_STORAGE_GUIDE.md) - Complete transaction storage guide
- ?? [TRANSACTION_QUICK_START.md](TRANSACTION_QUICK_START.md) - 5-minute setup guide
- ?? [TRANSACTION_FEATURE_SUMMARY.md](TRANSACTION_FEATURE_SUMMARY.md) - Feature overview
- ?? [TRANSACTION_COMPATIBILITY_NOTES.md](TRANSACTION_COMPATIBILITY_NOTES.md) - .NET Framework compatibility

### CI/CD Documentation
- ?? [CI-CD-DOCUMENTATION.md](.github/CI-CD-DOCUMENTATION.md) - Complete CI/CD guide
- ? [CI-CD-QUICK-START.md](.github/CI-CD-QUICK-START.md) - 5-minute CI/CD setup
- ?? [WORKFLOW-STATUS.md](.github/WORKFLOW-STATUS.md) - Workflow monitoring
- ?? [CI-CD-IMPLEMENTATION-SUMMARY.md](.github/CI-CD-IMPLEMENTATION-SUMMARY.md) - Implementation summary

### Additional Documentation
- ?? [PAN_MASKING_FEATURE.md](PAN_MASKING_FEATURE.md) - PAN masking feature details
- ?? [CARD_POLLING_FEATURE.md](CARD_POLLING_FEATURE.md) - Polling feature documentation
- ?? [NUGET_PACKAGES_CREATED.md](NUGET_PACKAGES_CREATED.md) - NuGet package information

## ?? Testing

### Running Tests

```bash
# Run all tests
dotnet test EMVCard.Tests\EMVCard.Tests.csproj

# Run with code coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "CardTransactionTests"
```

### Test Coverage

- **Unit Tests**: CardTransaction, JsonTransactionStorage
- **Integration Tests**: End-to-end transaction storage workflows
- **Test Framework**: xUnit with FluentAssertions
- **CI/CD**: Automated test execution on every commit

## ?? Version History

### Version 2.0.0 (January 2026) - Current Release

**Major Features:**
- ? Complete architectural refactoring with 6 business logic classes
- ? **Full 32-bit/64-bit platform support** via ModWinsCard wrapper
- ?? **Transaction Storage** - Save and retrieve card reading history
- ?? **CI/CD Pipeline** - Automated testing, building, and deployment
- ?? **Comprehensive Testing** - Unit and integration tests
- ?? Integrated SL Token generation
- ?? Async/await operations throughout
- ?? Card polling feature for automated reading
- ?? PAN masking for privacy compliance
- ?? Comprehensive logging with TraceSource
- ?? Fixed UI selection issues and buffer management
- ?? Complete documentation suite (20+ documents)

**Transaction Storage Features:**
- JSON and SQLite storage backends
- Search and filter transactions
- Export to JSON, XML, CSV
- Full CRUD operations
- Audit trail and compliance support

**CI/CD Features:**
- GitHub Actions workflows
- Automated testing on every commit
- One-click releases
- NuGet package automation
- Pull request validation

## ?? Roadmap

### Implemented ?
- [x] **Transaction Storage** - JSON and SQLite backends
- [x] **Export Functionality** - JSON, XML, CSV export
- [x] **CI/CD Pipeline** - GitHub Actions automation
- [x] **Automated Testing** - Unit and integration tests
- [x] **Multi-format Export** - Multiple export formats

### Planned ??
- [ ] **Card Reader Simulator** - Test without physical cards
- [ ] **Configuration Management** - External configuration support
- [ ] **DDA/CDA Verification** - Full cryptographic verification
- [ ] **Multi-language Support** - Internationalization (i18n)
- [ ] **Web API** - RESTful API wrapper
- [ ] **Advanced Reporting** - Analytics and dashboards
- [ ] **Cloud Integration** - Cloud storage and sync

## ?? Project Stats

- **Language:** C# 7.3
- **Framework:** .NET Framework 4.7.2
- **Projects:** 4 (Main app + 2 libraries + Test project)
- **NuGet Packages:** 2 published
- **Platform Support:** x86 and x64 Windows
- **Lines of Code:** ~3,500+ (including tests)
- **Documentation Files:** 25+
- **Test Coverage:** 80%+
- **CI/CD:** GitHub Actions
- **License:** MIT
- **First Release:** 2008
- **Current Version:** 2.0.0 (2026)
- **Latest NuGet:** NfcReaderLib 1.0.3 / EMVCard.Core 1.0.2