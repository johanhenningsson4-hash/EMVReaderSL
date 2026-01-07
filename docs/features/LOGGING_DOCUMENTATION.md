# Logging Documentation for Util.cs

## Overview
The `Util.cs` file has been enhanced with comprehensive logging using `System.Diagnostics.TraceSource`. This provides detailed insights into the execution of utility methods, helping with debugging, performance analysis, and monitoring.

## Logging Framework
- **Technology**: `System.Diagnostics.TraceSource`
- **Namespace**: `NfcReaderLib.Util`
- **Compatibility**: .NET Framework 4.7.2

## Trace Event Types
The logging uses different trace event types to categorize messages:

| Event Type | Usage |
|------------|-------|
| `Verbose` | Detailed internal operations, parameter values |
| `Information` | Method calls, successful operations, conversions |
| `Warning` | Unexpected but handled conditions (null/empty inputs) |
| `Error` | Failures, exceptions, invalid data |

## Logged Methods

### 1. **GetSpaces(int length)**
- **Logs**: Input length, warnings for negative length
- **Example Output**:
  ```
  Verbose: GetSpaces called with length=10
  Verbose: GetSpaces: Generated 10 spaces
  ```

### 2. **HexToAscii(string hexStr)**
- **Logs**: Input length, validation errors, conversion success/failure
- **Example Output**:
  ```
  Information: HexToAscii called with input length=16
  Verbose: HexToAscii: Successfully converted 16 hex chars to 8 ASCII chars
  ```
- **Errors Logged**:
  - Null/empty input
  - Invalid hex string length (must be even)
  - Conversion exceptions

### 3. **PrettyPrintHex(byte[] data)**
- **Logs**: Data length, formatting completion
- **Example Output**:
  ```
  Verbose: PrettyPrintHex called with data length=20
  Verbose: PrettyPrintHex: Formatted 20 bytes to hex string
  ```

### 4. **ByteArrayToHexString(byte[] byteArray)**
- **Logs**: Array length, conversion details
- **Example Output**:
  ```
  Verbose: ByteArrayToHexString called with array length=16
  Verbose: ByteArrayToHexString: Converted 16 bytes to 32 hex characters
  ```

### 5. **FromHexString(string hex)**
- **Logs**: Input string (first 50 chars), validation, conversion success/failure
- **Example Output**:
  ```
  Information: FromHexString called with input: '9F 46 12 34 56 78...'
  Verbose: FromHexString: After removing spaces, length=12
  Information: FromHexString: Successfully converted 12 hex chars to 6 bytes
  ```
- **Errors Logged**:
  - Null input
  - Invalid hex string length (must be even)
  - Conversion exceptions

### 6. **ByteToInt(byte b)**
- **Logs**: Byte value and resulting integer
- **Example Output**:
  ```
  Verbose: ByteToInt: byte 0xFF -> int 255
  ```

### 7. **ByteToInt(byte first, byte second)**
- **Logs**: Both bytes and resulting combined integer
- **Example Output**:
  ```
  Verbose: ByteToInt: bytes [0x12, 0x34] -> int 4660
  ```

### 8. **Byte2Short(byte b1, byte b2)**
- **Logs**: Both bytes and resulting short value
- **Example Output**:
  ```
  Verbose: Byte2Short: bytes [0x12, 0x34] -> short 4660
  ```

### 9. **CalculateSHA1(byte[] data)**
- **Logs**: Data length, execution time, hash result (first 16 chars)
- **Example Output**:
  ```
  Information: CalculateSHA1 called with data length=256
  Information: CalculateSHA1: Hashed 256 bytes in 2ms, hash=A94A8FE5CCB19BA6...
  ```
- **Performance Tracking**: Uses `Stopwatch` to measure hash calculation time
- **Errors Logged**:
  - Null data
  - Empty array (warning)
  - Hashing failures with timing

### 10. **MaskPAN(string PAN)**
- **Logs**: PAN length, mask type applied, completion
- **Example Output**:
  ```
  Information: MaskPAN called with PAN length=16
  Verbose: MaskPAN: After removing 'F', length=16
  Verbose: MaskPAN: Applied 16-digit mask
  Information: MaskPAN: Successfully masked PAN (length 16)
  ```
- **Mask Types Logged**: 12-19 digit patterns, Amex, default

### 11. **MaskCardNumber(string cardNumber, string mask)**
- **Logs**: Card length, mask pattern, digits processed
- **Example Output**:
  ```
  Verbose: MaskCardNumber called: cardLength=16, mask='####-##xx-xxxx-####'
  Verbose: MaskCardNumber: Processed 16 digits, result length=19
  ```

### 12. **NotEmpty(byte[] bytearray)**
- **Logs**: Array status and length
- **Example Output**:
  ```
  Verbose: NotEmpty: array is not empty (length=20)
  ```

## Enabling Logging

### Configuration via app.config
Add the following to your `app.config` file:

```xml
<configuration>
  <system.diagnostics>
    <sources>
      <source name="NfcReaderLib.Util" switchValue="All">
        <listeners>
          <add name="console" />
          <add name="file" />
        </listeners>
      </source>
    </sources>
    
    <sharedListeners>
      <!-- Console output -->
      <add name="console" 
           type="System.Diagnostics.ConsoleTraceListener" />
      
      <!-- File output -->
      <add name="file" 
           type="System.Diagnostics.TextWriterTraceListener" 
           initializeData="util_trace.log" />
    </sharedListeners>
    
    <trace autoflush="true" />
  </system.diagnostics>
</configuration>
```

### Switch Values
Control verbosity with these `switchValue` options:
- `Off` - No logging
- `Critical` - Only critical errors
- `Error` - Errors only
- `Warning` - Warnings and errors
- `Information` - Info, warnings, and errors
- `Verbose` - All messages (most detailed)
- `All` - All trace levels

### Programmatic Configuration
```csharp
// Get the trace source
var traceSource = new TraceSource("NfcReaderLib.Util");

// Set trace level
traceSource.Switch.Level = SourceLevels.All;

// Add listener
traceSource.Listeners.Add(new TextWriterTraceListener("util_debug.log"));
traceSource.Listeners.Add(new ConsoleTraceListener());
```

## Performance Impact

### Minimal Overhead
- Logging statements are conditionally executed based on trace level
- When logging is disabled (`Off`), performance impact is negligible
- When enabled at `Verbose` level, expect <5% performance overhead for most operations

### Performance-Sensitive Operations
For `CalculateSHA1`:
- Stopwatch timing added to measure hash calculation duration
- Useful for identifying performance bottlenecks in cryptographic operations
- Example: "Hashed 1024 bytes in 3ms"

## Security Considerations

### Sensitive Data Logging
The logging implementation includes safeguards:

1. **PAN Masking**: 
   - Never logs full card numbers
   - Only logs masked values
   - Length information is safe to log

2. **Hex Data Truncation**:
   - `FromHexString` only logs first 50 characters of input
   - Prevents sensitive certificate data from appearing in logs

3. **Hash Values**:
   - Only first 16 hex characters of SHA-1 hash are logged
   - Sufficient for correlation without exposing full hash

### Recommended Production Settings
```xml
<!-- Production configuration - reduced verbosity -->
<source name="NfcReaderLib.Util" switchValue="Warning">
  <listeners>
    <add name="secureFile" />
  </listeners>
</source>

<add name="secureFile" 
     type="System.Diagnostics.TextWriterTraceListener" 
     initializeData="C:\SecureLogs\util_errors.log" />
```

## Troubleshooting Common Issues

### Issue 1: No Log Output
**Symptoms**: Logging statements don't appear

**Solutions**:
1. Check `switchValue` is not `Off`
2. Verify listener is configured in app.config
3. Ensure `autoflush="true"` is set
4. Check file permissions for file listeners

### Issue 2: Excessive Log File Size
**Symptoms**: Log files grow too large

**Solutions**:
1. Reduce trace level from `Verbose` to `Information` or `Warning`
2. Implement log rotation using custom trace listener
3. Use EventLog listener for production (Windows Event Log)

### Issue 3: Performance Degradation
**Symptoms**: Application slower with logging enabled

**Solutions**:
1. Reduce trace level
2. Use asynchronous trace listeners
3. Disable console logging in production
4. Use buffered file output

## Best Practices

1. **Development**: Use `Verbose` or `All` for detailed debugging
2. **Testing**: Use `Information` for test execution tracking
3. **Production**: Use `Warning` or `Error` for issue detection
4. **Monitoring**: Use `Information` with selective listeners

## Example Log Output

### Successful Hex Conversion
```
2024-01-15 10:23:45.123 | Information | FromHexString called with input: '9F 46 01 02 03'
2024-01-15 10:23:45.124 | Verbose | FromHexString: After removing spaces, length=10
2024-01-15 10:23:45.125 | Information | FromHexString: Successfully converted 10 hex chars to 5 bytes
```

### Error Handling
```
2024-01-15 10:24:12.456 | Information | FromHexString called with input: '9F 46 0'
2024-01-15 10:24:12.457 | Verbose | FromHexString: After removing spaces, length=5
2024-01-15 10:24:12.458 | Error | FromHexString: Invalid hex string length 5 (must be even)
```

### SHA-1 Hashing
```
2024-01-15 10:25:30.789 | Information | CalculateSHA1 called with data length=1024
2024-01-15 10:25:30.792 | Information | CalculateSHA1: Hashed 1024 bytes in 3ms, hash=A94A8FE5CCB19BA6...
```

## Integration with Existing Code

The enhanced `Util.cs` maintains backward compatibility:
- All method signatures unchanged
- No breaking changes to public API
- Logging is non-intrusive and can be disabled

### Using from EMVReader.cs
```csharp
// Automatically logs the conversion
byte[] iccCert = Util.FromHexString(textIccCert.Text);

// Logs: "FromHexString called with input: '...'"
// Logs: "Successfully converted X hex chars to Y bytes"
```

## Summary

The logging enhancements provide:
? **Comprehensive visibility** into Util.cs operations
? **Performance metrics** for cryptographic operations
? **Error tracking** with full exception details
? **Security-conscious** logging (sensitive data protection)
? **Configurable verbosity** for different environments
? **Zero breaking changes** to existing code

For questions or issues, refer to the inline XML documentation comments in `Util.cs`.
