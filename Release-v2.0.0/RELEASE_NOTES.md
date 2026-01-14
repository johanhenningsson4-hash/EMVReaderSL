# EMV Card Reader v2.0.1 - Release Notes

**Release Date:** [TODAY]  
**Version:** 2.0.1  
**License:** MIT  
**Target Framework:** .NET Framework 4.7.2

## ?? What's New in v2.0.1

### Bug Fixes and Improvements
- Fixed: `SendGPO` now throws `ArgumentNullException` for null input, not `NullReferenceException`.
- Fixed: All tests now use valid PDOL TLV structures, preventing `IndexOutOfRangeException`.
- Fixed: No more duplicate or partial method declarations; all methods have valid bodies.
- Improved: Logging is now fully configurable at runtime via `SetLoggingLevel`.
- Improved: All code and tests are now fully compatible with C# 7.3 and .NET Framework 4.7.2.
- Improved: Moq-based tests work by making `SendApduWithAutoFix` virtual.
- Improved: Release notes and documentation updated for new version.
- Added: Comprehensive unit tests for `EmvGpoProcessor`.

### Upgrade Notes
- No breaking changes. All users are encouraged to upgrade for improved stability and diagnostics.
