# NfcReaderLib

A .NET library for PC/SC smart card communication, SL Token generation, and EMV/NFC card utilities.

## NuGet Packages

| Package | Version | Description |
|---------|---------|-------------|
| [**NfcReaderLib**](https://www.nuget.org/packages/NfcReaderLib) | 2.0.3 | PC/SC communication, SL Token generation |
| [**EMVCard.Core**](https://www.nuget.org/packages/EMVCard.Core) | 2.0.3 | EMV card reading, storage, and export |

### Installation

**Package Manager Console:**
```
Install-Package NfcReaderLib -Version 2.0.3
Install-Package EMVCard.Core -Version 2.0.3
```

**.NET CLI:**
```
dotnet add package NfcReaderLib --version 2.0.3
dotnet add package EMVCard.Core --version 2.0.3
```

## Features
- PC/SC card reader support (contact/contactless)
- 32/64-bit Windows support
- SL Token (SHA-256) generation from ICC certificates
- EMV TLV parsing, PSE/PPSE, GPO, record reading
- Card polling, PAN masking, async operations

## What's New in v2.0.3
- Maintenance and compatibility improvements
- Updated NuGet version: 2.0.3

## Requirements
- .NET Framework 4.7.2

## License
MIT

## Copyright
Copyright holders: Johan Henningsson