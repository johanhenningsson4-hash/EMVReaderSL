# EMV Card Reader v2.0.0 - Release Notes

**Release Date:** January 1, 2026  
**Version:** 2.0.0  
**License:** MIT  
**Target Framework:** .NET Framework 4.7.2

## ?? What's New in v2.0.0

### Major Features

#### ??? **Refactored Architecture**
- Complete separation of concerns with 6 dedicated business logic classes
- Clean architecture design with layered structure
- Improved maintainability and testability
- Reduced UI class from ~1500 lines to ~250 lines

#### ?? **SL Token Generation**
- Integrated SHA-256 based SL (Secure Link) Token generation
- Generates unique tokens from ICC Public Key Certificates (Tag 9F46)
- Privacy-friendly card identification without exposing PAN
- Consistent token generation for loyalty programs and card binding

#### ? **Async/Await Operations**
- Non-blocking asynchronous card operations
- Improved UI responsiveness
- Better error handling with async patterns
- Async methods for all card reader operations

#### ?? **Card Polling Feature**
- Automated continuous card reading
- Configurable polling intervals and counts
- Automatic card detection and reconnection
- Ideal for kiosk and unattended applications

#### ?? **PAN Masking**
- Optional card number masking for privacy
- Real-time toggle between masked and full display
- PCI DSS compliant masking (first 6 + last 4 digits)
- Configurable masking patterns

### New Classes

#### **EmvCardReader**
- PC/SC card reader communication
- Automatic error handling (6C, 67, 61 status words)
- Connection management and cleanup
- Comprehensive APDU logging

#### **EmvApplicationSelector**
- PSE (1PAY.SYS.DDF01) support for contact cards
- PPSE (2PAY.SYS.DDF01) support for contactless cards
- Multi-application enumeration
- FCI template parsing

#### **EmvGpoProcessor**
- Get Processing Options (GPO) command handling
- PDOL extraction and processing
- Default values for common PDOL tags
- Simplified GPO fallback

#### **EmvRecordReader**
- AFL (Application File Locator) based record reading
- Fallback to common SFI/record combinations
- Template format handling
- Automatic TLV parsing of records

#### **EmvDataParser**
- Complete BER-TLV parsing
- Support for all standard EMV tags
- Priority-based field updates
- Nested template support (tags 70, 77)
- Track2 data extraction

#### **EmvTokenGenerator**
- SL Token generation from ICC certificates
- Async token generation
- Multi-line certificate format support
- Detailed error reporting

### Technical Improvements

#### ?? **Comprehensive Logging**
- `System.Diagnostics.TraceSource` integration
- Detailed APDU command/response logging
- Performance tracking with timing information
- Configurable log levels (Verbose, Information, Warning, Error)

#### ?? **Enhanced Error Handling**
- Automatic Le adjustment for 6C XX responses
- Automatic length correction for 67 00 responses
- Automatic GET RESPONSE for 61 XX responses
- Graceful handling of missing files and records

#### ?? **NuGet Packages**
Two professional NuGet packages published:

**NfcReaderLib v1.0.0**
- PC/SC communication utilities
- SL Token generation
- Data conversion and formatting helpers
- PAN masking utilities

**EMVCard.Core v1.0.0**
- High-level EMV card reading
- PSE/PPSE application selection
- GPO processing
- TLV parsing and data extraction
- Depends on NfcReaderLib

### Bug Fixes

#### ?? **ComboBox Selection Fix**
- Fixed application selection persistence
- Proper dictionary management for application mapping
- Cleared buffers preserve application selection
- Improved UI state management

#### ?? **Buffer Management**
- Proper clearing of all data fields
- Reset of card data structures
- Prevention of stale data display
- Improved memory management

#### ?? **Polling Reconnection**
- Automatic card detection between polls
- Proper disconnect/reconnect cycle
- Graceful handling of card removal
- Improved polling reliability

## ?? Installation

### Binary Release
Download the release package and extract to any folder. No installation required.

**Requirements:**
- Windows 7 or later
- .NET Framework 4.7.2 or later
- PC/SC compatible card reader

### From NuGet (Recommended for Developers)

```bash
dotnet add package NfcReaderLib --version 1.0.0
dotnet add package EMVCard.Core --version 1.0.0
```

### From Source

```bash
git clone https://github.com/johanhenningsson4-hash/EMVReaderSL.git
cd EMVReaderSLCard
msbuild EMVReaderSL.sln /p:Configuration=Release
```

## ?? Quick Start

1. **Connect your card reader** to a USB port
2. **Run EMVReader.exe** from the Release folder
3. **Click "Initialize"** to detect card readers
4. **Select your reader** from the dropdown
5. **Click "Connect Card"** to connect to the card
6. **Click "Load PPSE"** (contactless) or "Load PSE"** (contact)
7. **Select an application** from the dropdown
8. **Click "ReadApp"** to read card data
9. **View card data and SL Token** in the interface

## ?? What's Included

### Binary Package Contents

```
EMVReader.exe                 - Main application
EMVReader.exe.config          - Configuration file
EMVCard.Core.dll             - EMV card reading library
NfcReaderLib.dll             - PC/SC and utility library
*.pdb                         - Debug symbols (optional)
```

### Supported EMV Tags

- **5A** - Application PAN (Card Number)
- **5F20** - Cardholder Name
- **5F24** - Application Expiration Date
- **57** - Track 2 Equivalent Data
- **9F46** - ICC Public Key Certificate
- **9F47** - ICC Public Key Exponent
- **9F48** - ICC Public Key Remainder
- **94** - Application File Locator (AFL)
- **9F38** - Processing Options Data Object List (PDOL)
- And many more...

## ?? Configuration

### Polling Settings
- **Poll Count**: Number of consecutive reads (1-1000)
- **Poll Interval**: Time between reads (default: 2000ms)
- **Auto-reconnect**: Automatic card detection

### Privacy Settings
- **Mask PAN**: Toggle card number masking
- **Logging Level**: Configure via app.config

## ?? Known Issues

### Minor Warnings
- Two unused exception variables in `SLCard.cs` (lines 49, 66)
- These warnings do not affect functionality

### Limitations
- **SDA-only cards**: Cannot generate SL Token (requires DDA/CDA with ICC certificate)
- **Non-PSE cards**: May require manual AID entry
- **Windows only**: PC/SC implementation is Windows-specific

## ?? Troubleshooting

### No Card Readers Found
1. Check USB connection
2. Install reader drivers
3. Restart PC/SC service: `net stop SCardSvr && net start SCardSvr`

### No Applications Found
1. Try both PSE and PPSE buttons
2. Card may not support PSE/PPSE
3. Check APDU logs for error codes

### SL Token Generation Failed
1. Verify card supports DDA/CDA (check for Tag 9F46 in logs)
2. Some cards only support SDA and don't have ICC certificates
3. Check logs for "ICC Public Key Certificate" presence

### Common Status Words
- **90 00** - Success
- **6C XX** - Wrong Le (auto-handled)
- **67 00** - Wrong length (auto-handled)
- **61 XX** - More data available (auto-handled)
- **6A 82** - File not found (PSE/PPSE not supported)
- **6A 83** - Record not found

## ?? Documentation

Comprehensive documentation available in the repository:

- **REFACTORING_DOCUMENTATION.md** - Architecture and design details
- **ICC_PUBLIC_KEY_PARSER_DOCUMENTATION.md** - ICC certificate parsing
- **SL_TOKEN_INTEGRATION_DOCUMENTATION.md** - Token generation guide
- **LOGGING_DOCUMENTATION.md** - Logging configuration and usage
- **PAN_MASKING_FEATURE.md** - PAN masking feature details
- **CARD_POLLING_FEATURE.md** - Polling feature documentation
- **NUGET_PACKAGES_CREATED.md** - NuGet package information

## ?? Security

### PCI DSS Compliance
- PAN masking follows PCI DSS guidelines (first 6 + last 4 digits)
- No sensitive data stored permanently
- Logging can be configured to exclude sensitive fields

### API Key Management
- NuGet API keys should be stored in environment variables
- Never commit API keys to source control
- Rotate keys if compromised

## ?? NuGet Packages

### NfcReaderLib 1.0.0
- **Package ID**: NfcReaderLib
- **Download**: https://www.nuget.org/packages/NfcReaderLib
- **Description**: PC/SC communication, SL Token generation, utilities
- **Dependencies**: System.Security.Cryptography.Algorithms (4.3.1)

### EMVCard.Core 1.0.0
- **Package ID**: EMVCard.Core
- **Download**: https://www.nuget.org/packages/EMVCard.Core
- **Description**: EMV card reading, PSE/PPSE, GPO, TLV parsing
- **Dependencies**: NfcReaderLib (1.0.0)

## ?? Tested Card Readers

- ? ACR122U (contactless)
- ? SCM SCR331 (contact)
- ? Omnikey 5321 (dual interface)
- ? Generic PC/SC compliant readers

## ?? Tested Cards

- ? Visa (contact and contactless)
- ? Mastercard (contact and contactless)
- ? UnionPay
- ? Discover
- ? JCB
- ? American Express

## ?? Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push and create a Pull Request

## ?? License

**MIT License**

Copyright © Johan Henningsson 2008-2026

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## ?? Acknowledgments

- **EMVCo** - EMV specifications
- **PC/SC Workgroup** - Smart card standards
- **Eternal TUTU** - Original EMVReader (2008)

## ?? Contact

**Johan Henningsson**
- GitHub: [@johanhenningsson4-hash](https://github.com/johanhenningsson4-hash)
- Repository: [EMVReaderSL](https://github.com/johanhenningsson4-hash/EMVReaderSL)

## ?? Version History

### v2.0.0 (January 1, 2026) - Current Release
- Complete architectural refactoring
- SL Token generation
- Async/await operations
- Card polling feature
- PAN masking
- NuGet packages published
- Comprehensive documentation

### v1.0.0 (2008) - Original Release
- Initial EMV card reader
- Basic PSE support
- TLV parsing

## ??? Roadmap

Future enhancements planned:

- [ ] Export to JSON/XML formats
- [ ] Configuration file support
- [ ] DDA/CDA signature verification
- [ ] Multi-language support
- [ ] Web API integration
- [ ] RESTful service wrapper

---

**Made with ?? by Johan Henningsson** | **2008-2026**

? **Star this repo if you find it useful!**

?? **Thank you for using EMV Card Reader!**
