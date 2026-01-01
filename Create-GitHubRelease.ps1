# GitHub Release Creation Script for v2.0.0
# This script helps create the GitHub release

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "      EMV Card Reader v2.0.0 - GitHub Release Creation" -ForegroundColor Yellow
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host ""

# Check if release assets exist
$zipFile = "C:\Jobb\EMVReaderSLCard\EMVCardReader-v2.0.0.zip"
$releaseNotes = "C:\Jobb\EMVReaderSLCard\Release-v2.0.0\RELEASE_NOTES.md"

Write-Host "Checking release assets..." -ForegroundColor Green
if (Test-Path $zipFile) {
    $fileSize = (Get-Item $zipFile).Length / 1KB
    Write-Host "  ? Release package found: $fileSize KB" -ForegroundColor Green
} else {
    Write-Host "  ? Release package not found!" -ForegroundColor Red
    exit 1
}

if (Test-Path $releaseNotes) {
    Write-Host "  ? Release notes found" -ForegroundColor Green
} else {
    Write-Host "  ??  Release notes not found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Manual Release Creation Steps:" -ForegroundColor Yellow
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. Navigate to GitHub Releases:" -ForegroundColor White
Write-Host "   https://github.com/johanhenningsson4-hash/EMVReaderSL/releases" -ForegroundColor Cyan
Write-Host ""

Write-Host "2. Click 'Draft a new release'" -ForegroundColor White
Write-Host ""

Write-Host "3. Configure the release:" -ForegroundColor White
Write-Host "   Tag version:    v2.0.0 (select from dropdown)" -ForegroundColor Gray
Write-Host "   Release title:  EMV Card Reader v2.0.0 - 2026 Release" -ForegroundColor Gray
Write-Host "   Target:         master" -ForegroundColor Gray
Write-Host ""

Write-Host "4. Copy Release Description:" -ForegroundColor White
Write-Host "   Press Enter to copy the release notes to clipboard..." -ForegroundColor Yellow
Read-Host

# Create release description
$releaseDescription = @"
# ?? EMV Card Reader v2.0.0

**A professional EMV chip card reader with SL Token generation - 2026 Release**

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)](https://dotnet.microsoft.com/download/dotnet-framework)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-Published-blue)](https://www.nuget.org/)

## What's New in v2.0.0

### ??? Major Features

- **Refactored Architecture** - Complete separation into 6 business logic classes
- **SL Token Generation** - SHA-256 based secure tokens from ICC certificates
- **Async Operations** - Non-blocking asynchronous card operations
- **Card Polling** - Automated continuous card reading with configurable intervals
- **PAN Masking** - PCI DSS compliant card number masking

### ?? NuGet Packages

This release includes two published NuGet packages:

- **[NfcReaderLib](https://www.nuget.org/packages/NfcReaderLib)** v1.0.0 - PC/SC communication and utilities
- **[EMVCard.Core](https://www.nuget.org/packages/EMVCard.Core)** v1.0.0 - EMV card reading library

``````bash
dotnet add package NfcReaderLib --version 1.0.0
dotnet add package EMVCard.Core --version 1.0.0
``````

### ? Technical Improvements

- Comprehensive logging with System.Diagnostics.TraceSource
- Enhanced error handling with automatic APDU retries
- Improved UI responsiveness with async/await
- Better memory management and resource cleanup

### ?? Bug Fixes

- Fixed ComboBox application selection persistence
- Improved buffer clearing and state management
- Enhanced polling reconnection logic
- Fixed card detection between polling cycles

## ?? Installation

### Binary Package

**Download:** [EMVCardReader-v2.0.0.zip](https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/download/v2.0.0/EMVCardReader-v2.0.0.zip)

**Requirements:**
- Windows 7 or later
- .NET Framework 4.7.2 or later
- PC/SC compatible card reader

**Quick Start:**
1. Extract ZIP file
2. Run ``EMVReader.exe``
3. Connect your card reader
4. Click "Initialize" to start

### From NuGet (Developers)

``````powershell
Install-Package NfcReaderLib
Install-Package EMVCard.Core
``````

### From Source

``````bash
git clone https://github.com/johanhenningsson4-hash/EMVReaderSL.git
cd EMVReaderSLCard
msbuild EMVReaderSL.sln /p:Configuration=Release
``````

## ?? Documentation

- **[README.md](https://github.com/johanhenningsson4-hash/EMVReaderSL/blob/master/README.md)** - Project overview
- **RELEASE_NOTES.md** - Complete release notes (included in ZIP)
- **Architecture docs** - Available in repository

## ?? Supported Hardware

- ? ACR122U (contactless)
- ? SCM SCR331 (contact)
- ? Omnikey 5321 (dual interface)
- ? Generic PC/SC compliant readers

## ?? Supported Cards

- ? Visa (contact & contactless)
- ? Mastercard (contact & contactless)
- ? UnionPay
- ? Discover
- ? JCB
- ? American Express

## ?? Known Issues

- Two minor warnings in SLCard.cs (unused exception variables) - does not affect functionality
- SDA-only cards cannot generate SL Tokens (requires DDA/CDA with ICC certificate)

## ?? What's Next

See the [Roadmap](https://github.com/johanhenningsson4-hash/EMVReaderSL#-roadmap) for planned features:
- Export to JSON/XML
- Configuration file support
- DDA/CDA verification
- Multi-language support
- Web API integration

## ?? License

MIT License - Copyright © Johan Henningsson 2008-2026

## ?? Acknowledgments

- **EMVCo** - EMV specifications
- **PC/SC Workgroup** - Smart card standards
- **Eternal TUTU** - Original EMVReader (2008)

---

**Made with ?? by Johan Henningsson** | **[GitHub](https://github.com/johanhenningsson4-hash)** | **2008-2026**

? **Star this repo if you find it useful!**
"@

Set-Clipboard -Value $releaseDescription
Write-Host "   ? Release description copied to clipboard!" -ForegroundColor Green
Write-Host "   Paste it into the description field on GitHub" -ForegroundColor Yellow
Write-Host ""

Write-Host "5. Upload Release Assets:" -ForegroundColor White
Write-Host "   Click 'Attach binaries...'" -ForegroundColor Gray
Write-Host "   Upload: $zipFile" -ForegroundColor Cyan
Write-Host ""

Write-Host "6. Configure Release Options:" -ForegroundColor White
Write-Host "   ? Set as a pre-release (LEAVE UNCHECKED)" -ForegroundColor Gray
Write-Host "   ? Set as the latest release (CHECK THIS)" -ForegroundColor Green
Write-Host ""

Write-Host "7. Click 'Publish release'" -ForegroundColor White
Write-Host ""

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "After Publishing:" -ForegroundColor Yellow
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Verify the release at:" -ForegroundColor White
Write-Host "  https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/tag/v2.0.0" -ForegroundColor Cyan
Write-Host ""

Write-Host "Download link will be:" -ForegroundColor White
Write-Host "  https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/download/v2.0.0/EMVCardReader-v2.0.0.zip" -ForegroundColor Cyan
Write-Host ""

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Press Enter to open GitHub Releases page in browser..." -ForegroundColor Yellow
Read-Host

Start-Process "https://github.com/johanhenningsson4-hash/EMVReaderSL/releases/new?tag=v2.0.0&title=EMV%20Card%20Reader%20v2.0.0%20-%202026%20Release"

Write-Host ""
Write-Host "? Release creation script completed!" -ForegroundColor Green
Write-Host "   Follow the steps above to complete the release on GitHub" -ForegroundColor Yellow
Write-Host ""
