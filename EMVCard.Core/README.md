# EMVCard.Core

This library provides core functionality for EMV card transaction storage and processing, including:

- Transaction data model (`CardTransaction`)
- Storage interfaces and implementations (JSON and SQLite)
- Batch and summary operations for high performance
- .NET Framework 4.7.2 compatibility

## Features
- Save, retrieve, filter, and export EMV card transactions
- JSON and SQLite storage backends
- Batch insert with `SaveBatchAsync`
- Lightweight summary queries with `GetAllSummaryAsync`
- Designed for integration with Windows Forms and other .NET apps

## What's New in v2.0.4
- Maintenance and compatibility improvements
- Updated NuGet version: 2.0.4

## Usage

Install via NuGet:

```
Install-Package EMVCard.Core -Version 2.0.4
```

**.NET CLI:**
```
dotnet add package EMVCard.Core --version 2.0.4
```

Example:

```csharp
using EMVCard.Storage;

var storage = new SQLiteTransactionStorage("transactions");
await storage.SaveAsync(transaction);

// Batch insert
await storage.SaveBatchAsync(transactions);

// Get summaries
var summaries = await storage.GetAllSummaryAsync();
```

## Requirements
- .NET Framework 4.7.2
- Newtonsoft.Json
- (Optional) System.Data.SQLite for SQLite storage

## License
MIT

# NfcReaderLib

A .NET library for PC/SC smart card communication, SL Token generation, and EMV/NFC card utilities.

## NuGet Packages

| Package | Version | Description |
|---------|---------|-------------|
| [**NfcReaderLib**](https://www.nuget.org/packages/NfcReaderLib) | 2.0.3 | PC/SC communication, SL Token generation |
| [**EMVCard.Core**](https://www.nuget.org/packages/EMVCard.Core) | 2.0.4 | EMV card reading, storage, and export |

### Installation

**Package Manager Console:**
```
Install-Package NfcReaderLib -Version 2.0.3
Install-Package EMVCard.Core -Version 2.0.4
```

**.NET CLI:**
```
dotnet add package NfcReaderLib --version 2.0.3
dotnet add package EMVCard.Core --version 2.0.4
```

## Features
- PC/SC card reader support (contact/contactless)
- 32/64-bit Windows support
- SL Token (SHA-256) generation from ICC certificates
- EMV TLV parsing, PSE/PPSE, GPO, record reading
- Card polling, PAN masking, async operations

## What's New in v2.0.4
- Maintenance and compatibility improvements
- Updated NuGet version: 2.0.4

## Requirements
- .NET Framework 4.7.2

## License
MIT
