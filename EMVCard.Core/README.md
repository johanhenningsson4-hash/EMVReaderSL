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

## Usage

Install via NuGet:
