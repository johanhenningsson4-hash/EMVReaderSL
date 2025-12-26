# SL Token Space-Separated Format Update

## Change Summary

Updated the SL Token generation to use **space-separated format** (e.g., "AB CD EF 01 23") instead of continuous format (e.g., "ABCDEF0123").

## Why This Change?

The SL Token needs to be displayed in a more readable format with spaces after every 2 characters, matching standard hexadecimal display conventions used in EMV specifications.

## Implementation

### SLCard.cs Methods

The `SLCard` class already had two methods for token generation:

```csharp
// Method 1: Continuous format (no spaces)
public string GetSLToken()
{
    using (var sha256 = SHA256.Create())
    {
        var encodedHash = sha256.ComputeHash(IccPublicKeyCertificate ?? Array.Empty<byte>());
        return BitConverter.ToString(encodedHash).Replace("-", "");
    }
}

// Method 2: Space-separated format ?
public string GetSLToken2()
{
    using (var sha256 = SHA256.Create())
    {
        var encodedHash = sha256.ComputeHash(IccPublicKeyCertificate ?? Array.Empty<byte>());
        return BitConverter.ToString(encodedHash).Replace("-", " ");
        //                                                    ?
        //                                    Replace "-" with " " (space)
    }
}
```

### Change in EmvTokenGenerator.cs

Updated both token generation methods to use `GetSLToken2()`:

#### Before
```csharp
string token = slCard.GetSLToken();  // Returns: "ABCDEF0123456789..."
```

#### After
```csharp
string token = slCard.GetSLToken2(); // Returns: "AB CD EF 01 23 45 67 89..."
```

### Updated Methods

**1. GenerateToken()**
```csharp
public TokenResult GenerateToken(EmvDataParser.EmvCardData cardData, string pan, string aid)
{
    // ... parsing certificate ...
    
    var slCard = new SLCard
    {
        PAN = pan,
        AID = aid,
        IccPublicKeyCertificate = certificate,
        IccPublicKeyExponent = exponent,
        IccPublicKeyRemainder = remainder
    };

    string token = slCard.GetSLToken2(); // ? Changed from GetSLToken()
    
    // ...
}
```

**2. GenerateTokenFromCertificate()**
```csharp
public TokenResult GenerateTokenFromCertificate(byte[] certificate)
{
    var slCard = new SLCard
    {
        IccPublicKeyCertificate = certificate
    };

    string token = slCard.GetSLToken2(); // ? Changed from GetSLToken()
    
    return TokenResult.CreateSuccess(token);
}
```

## Token Format Comparison

### Before (Continuous)
```
E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855
```
- 64 characters
- No spaces
- Less readable

### After (Space-Separated)
```
E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55
```
- 95 characters (64 hex chars + 31 spaces)
- Space after every 2 characters
- More readable
- Matches standard hex display format

## Display Example

### In txtSLToken TextBox

**Before:**
```
???????????????????????????????????????????
? E3B0C44298FC1C149AFBF4C8996FB9242...  ?
???????????????????????????????????????????
```

**After:**
```
???????????????????????????????????????????
? E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8... ?
???????????????????????????????????????????
```

## Technical Details

### BitConverter.ToString() Behavior

```csharp
byte[] hash = new byte[] { 0xE3, 0xB0, 0xC4, 0x42 };

// Default BitConverter format
string default = BitConverter.ToString(hash);
// Result: "E3-B0-C4-42"

// GetSLToken() format
string continuous = BitConverter.ToString(hash).Replace("-", "");
// Result: "E3B0C442"

// GetSLToken2() format ?
string spaced = BitConverter.ToString(hash).Replace("-", " ");
// Result: "E3 B0 C4 42"
```

## Benefits

### 1. Readability
```
// Hard to read
E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855

// Easy to read
E3 B0 C4 42 98 FC 1C 14 9A FB F4 C8 99 6F B9 24 27 AE 41 E4 64 9B 93 4C A4 95 99 1B 78 52 B8 55
```

### 2. Standard Format
- Matches EMV specification display conventions
- Consistent with how ICC certificates are displayed
- Aligns with APDU logging format (e.g., "< 00 A4 04 00")

### 3. Copy/Paste Friendly
- Easier to select byte ranges
- Clearer when copying specific bytes
- Better for debugging and comparison

### 4. Professional Appearance
- Looks more polished
- Follows industry standards
- Consistent with other hex displays in the app

## Logging Updates

Also updated the log message to show more characters (23 vs 16) since spaces take up room:

```csharp
// Before
OnLogMessage($"SL Token generated successfully: {token.Substring(0, Math.Min(16, token.Length))}...");
// Output: "SL Token generated successfully: E3B0C44298FC1C14..."

// After
OnLogMessage($"SL Token generated successfully: {token.Substring(0, Math.Min(23, token.Length))}...");
// Output: "SL Token generated successfully: E3 B0 C4 42 98 FC 1C 14..."
```

## Backward Compatibility

### If You Need Continuous Format

You can still get the continuous format by calling `GetSLToken()` directly:

```csharp
// From UI
string spacedToken = txtSLToken.Text;  // "AB CD EF 01 23"

// Convert to continuous if needed
string continuous = spacedToken.Replace(" ", "");  // "ABCDEF0123"
```

### If You Need Different Separator

You can modify the separator in `SLCard.cs`:

```csharp
// Hyphen-separated
return BitConverter.ToString(encodedHash).Replace("-", "-");  // "AB-CD-EF-01"

// Colon-separated
return BitConverter.ToString(encodedHash).Replace("-", ":");  // "AB:CD:EF:01"

// No separator (original)
return BitConverter.ToString(encodedHash).Replace("-", "");   // "ABCDEF01"
```

## Testing

### Test Scenario 1: Normal Token Generation
```
1. Read card with ICC certificate
2. Generate SL Token
3. Check txtSLToken.Text
Expected: "E3 B0 C4 42 98 FC 1C 14 9A FB..." (with spaces)
```

### Test Scenario 2: Token Length
```csharp
string token = tokenGenerator.GenerateToken(...).Token;
int charCount = token.Length;
// Expected: 95 (64 hex chars + 31 spaces)
```

### Test Scenario 3: Format Validation
```csharp
string token = tokenGenerator.GenerateToken(...).Token;
bool hasSpaces = token.Contains(" ");
Assert.IsTrue(hasSpaces);

string[] bytes = token.Split(' ');
Assert.AreEqual(32, bytes.Length); // SHA-256 = 32 bytes
```

## Summary

? **Updated** - `EmvTokenGenerator` now uses `GetSLToken2()`  
? **Format** - SL Token displayed as "AB CD EF 01 23..." (space-separated)  
? **Readability** - Much easier to read and work with  
? **Standard** - Matches EMV industry conventions  
? **Build** - Successful compilation  
? **Compatible** - Can still convert to continuous format if needed  

The SL Token is now displayed in the standard space-separated hexadecimal format! ??
