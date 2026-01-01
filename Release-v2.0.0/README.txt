# EMV Card Reader v2.0.0

**Professional EMV Chip Card Reader with SL Token Generation**

## Quick Start

1. **Extract** all files to a folder
2. **Run** `EMVReader.exe`
3. **Connect** your PC/SC card reader
4. **Click "Initialize"** to start

## System Requirements

- Windows 7 or later
- .NET Framework 4.7.2 or later
- PC/SC compatible card reader

## Package Contents

| File | Description |
|------|-------------|
| `EMVReader.exe` | Main application |
| `EMVReader.exe.config` | Configuration file |
| `EMVCard.Core.dll` | EMV reading library |
| `NfcReaderLib.dll` | PC/SC utilities library |
| `*.pdb` | Debug symbols (optional) |
| `RELEASE_NOTES.md` | Complete release notes |

## Features

- ? Read contact and contactless EMV cards
- ? Extract card number, expiry date, cardholder name
- ? Generate secure SL Tokens from ICC certificates
- ? Support for PSE/PPSE application selection
- ? Automated card polling for kiosk mode
- ? PAN masking for privacy
- ? Comprehensive APDU logging

## Basic Usage

1. **Initialize Reader**
   - Click "Initialize" button
   - Select your card reader from dropdown
   - Click "Connect Card"

2. **Read Card**
   - Click "Load PPSE" (contactless) or "Load PSE" (contact)
   - Select application from dropdown
   - Click "ReadApp"

3. **View Results**
   - Card data displayed in text fields
   - SL Token shown at bottom
   - APDU logs in right panel

## Troubleshooting

### No Readers Found
- Check USB connection
- Install card reader drivers
- Restart PC/SC service:
  ```cmd
  net stop SCardSvr
  net start SCardSvr
  ```

### Card Not Detected
- Ensure card is properly seated
- Try both PSE and PPSE buttons
- Check reader LED status

### SL Token Error
- Not all cards support SL Token generation
- Card must have ICC certificate (DDA/CDA)
- Check logs for details

## Support

- **Documentation**: See `RELEASE_NOTES.md` for detailed information
- **GitHub**: https://github.com/johanhenningsson4-hash/EMVReaderSL
- **Issues**: https://github.com/johanhenningsson4-hash/EMVReaderSL/issues

## License

MIT License - Copyright © Johan Henningsson 2008-2026

See full license terms in `RELEASE_NOTES.md`

---

**Version 2.0.0** | **Released: January 1, 2026** | **.NET Framework 4.7.2**
