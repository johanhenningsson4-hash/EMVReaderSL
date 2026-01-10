# Transaction Saving - .NET Framework 4.7.2 Compatibility Notes

**Target Framework:** .NET Framework 4.7.2  
**Issue:** Some APIs used are from .NET Core/5+  
**Status:** ?? REQUIRES COMPATIBILITY UPDATES

---

## ?? Required Changes for .NET Framework 4.7.2

### 1. JSON Serialization

**Problem:** `System.Text.Json` not available in .NET Framework 4.7.2

**Solution:** Use Newtonsoft.Json instead

```powershell
# Install NuGet package
Install-Package Newtonsoft.Json -Version 13.0.3
```

**Code Changes:**

```csharp
// Replace this:
using System.Text.Json;

//With this:
using Newtonsoft.Json;

// Replace this:
var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

// With this:
var json = JsonConvert.SerializeObject(transactions, Formatting.Indented, 
    new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    });

// Replace this:
var transactions = JsonSerializer.Deserialize<List<CardTransaction>>(json);

// With this:
var transactions = JsonConvert.DeserializeObject<List<CardTransaction>>(json);
```

### 2. File.ReadAllTextAsync / WriteAllTextAsync

**Problem:** Async file methods not available in .NET Framework 4.7.2

**Solution:** Use synchronous methods or Task.Run

```csharp
// Option A: Use synchronous methods (simpler)
private List<CardTransaction> LoadAllTransactions()
{
    try
    {
        var json = File.ReadAllText(_dataFile);
        return JsonConvert.DeserializeObject<List<CardTransaction>>(json) 
            ?? new List<CardTransaction>();
    }
    catch (JsonException)
    {
        return new List<CardTransaction>();
    }
}

private void SaveAllTransactions(List<CardTransaction> transactions)
{
    var json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
    File.WriteAllText(_dataFile, json);
}

// Option B: Wrap in Task.Run to keep async interface
private async Task<List<CardTransaction>> LoadAllTransactionsAsync()
{
    return await Task.Run(() =>
    {
        var json = File.ReadAllText(_dataFile);
        return JsonConvert.DeserializeObject<List<CardTransaction>>(json)
            ?? new List<CardTransaction>();
    });
}
```

### 3. EmvCardData Property Names

**Problem:** Property names don't match actual class

**Solution:** Check actual property names in EmvDataParser.cs

```csharp
// Need to verify these property names exist in EmvCardData class:
// If not, adjust the mapping in CardTransaction.FromCardData()

public static CardTransaction FromCardData(EmvDataParser.EmvCardData cardData, string readerName)
{
    return new CardTransaction
    {
        PAN = cardData.PAN ?? string.Empty,
        ExpiryDate = cardData.ExpiryDate ?? string.Empty,
        CardholderName = cardData.CardholderName ?? string.Empty,
        // Map these based on actual property names:
        AID = cardData./* actual property */ ?? string.Empty,
        ApplicationLabel = cardData./* actual property */ ?? string.Empty,
        SLToken = cardData./* actual property */ ?? string.Empty,
        // etc.
    };
}
```

---

## ? Quick Fix Package

I'll create updated versions compatible with .NET Framework 4.7.2:

### Files to Update:
1. JsonTransactionStorage.cs - Use Newtonsoft.Json
2. CardTransaction.cs - Fix property mappings
3. SQLiteTransactionStorage.cs - Keep as optional (requires NuGet)

---

## ?? Required NuGet Packages

```xml
<!-- Add to .csproj -->
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  <!-- Optional: For SQLite storage -->
  <PackageReference Include="System.Data.SQLite" Version="1.0.118" />
</ItemGroup>
```

---

## ?? Installation Steps

### Step 1: Install Newtonsoft.Json
```powershell
cd C:\Jobb\EMVReaderSLCard
dotnet add package Newtonsoft.Json --version 13.0.3
```

### Step 2: Update JSON Storage
- Replace System.Text.Json with Newtonsoft.Json
- Replace async file operations with synchronous ones

### Step 3: Fix Property Mappings
- Check EmvDataParser.cs for actual property names
- Update CardTransaction.FromCardData() accordingly

### Step 4: Test
```csharp
var storage = new JsonTransactionStorage("transactions");
var transaction = new CardTransaction { PAN = "test" };
storage.SaveAsync(transaction).Wait(); // or await in async context
```

---

## ?? Recommendation

**For .NET Framework 4.7.2, use JSON storage only:**
- Easier to set up
- Newtonsoft.Json is widely used
- No additional complexity

**SQLite can be added later if needed:**
- Requires System.Data.SQLite package
- More complex setup
- Better for high-volume scenarios

---

## ?? Next Steps

1. I'll create .NET Framework 4.7.2 compatible versions
2. Test with actual EMVCardData structure
3. Provide updated installation guide

Would you like me to:
1. Create the updated .NET Framework 4.7.2 compatible versions?
2. Check the actual EmvDataParser.EmvCardData structure first?
3. Provide a simplified version without async?
