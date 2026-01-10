# Transaction Saving - 5 Minute Quick Start

**Get transaction saving working in 5 minutes!**

---

## ? Step 1: Add Files to Project (1 min)

Copy these files to your EMVCard.Core project:
- `CardTransaction.cs` ? Root of project
- `Storage/ITransactionStorage.cs` ? Storage folder
- `Storage/JsonTransactionStorage.cs` ? Storage folder
- `Storage/SQLiteTransactionStorage.cs` ? Storage folder (optional)

---

## ? Step 2: Add Using Statement (30 sec)

```csharp
// At top of EMVReader.cs
using EMVCard.Storage;
```

---

## ? Step 3: Initialize Storage (1 min)

```csharp
// In EMVReader.cs class
private ITransactionStorage _storage;

private void EMVReader_Load(object sender, EventArgs e)
{
    // ... existing code ...
    
    // Add this line
    _storage = new JsonTransactionStorage("transactions");
}
```

---

## ? Step 4: Save After Card Read (2 min)

```csharp
private async void btnReadCard_Click(object sender, EventArgs e)
{
    var startTime = DateTime.Now;
    
    try
    {
        // ... your existing card reading code ...
        // ... gets cardData result ...
        
        // ADD THIS: Save transaction
        var transaction = CardTransaction.FromCardData(
            cardData, 
            cmbReaders.SelectedItem?.ToString()
        );
        transaction.ProcessingTimeMs = (long)(DateTime.Now - startTime).TotalMilliseconds;
        await _storage.SaveAsync(transaction);
        
        // Update status
        statusLabel.Text = $"Card read and saved. Total: {await _storage.GetCountAsync()}";
    }
    catch (Exception ex)
    {
        // ADD THIS: Save failed transaction
        var failedTrans = CardTransaction.CreateFailed(
            cmbReaders.SelectedItem?.ToString(), 
            ex.Message
        );
        await _storage.SaveAsync(failedTrans);
        
        // ... existing error handling ...
    }
}
```

---

## ? Step 5: View Transactions (30 sec)

```csharp
// Add a menu item or button
private async void btnViewHistory_Click(object sender, EventArgs e)
{
    var transactions = await _storage.GetAllAsync();
    
    var msg = $"Total Transactions: {transactions.Count}\n\n";
    msg += "Recent:\n";
    
    foreach (var t in transactions.Take(5))
    {
        msg += t.GetSummary() + "\n";
    }
    
    MessageBox.Show(msg, "Transaction History");
}
```

---

## ?? Done! 

Your transactions are now being saved to:
```
C:\Jobb\EMVReaderSLCard\transactions\transactions.json
```

---

## ?? Verify It's Working

1. **Read a card** - Check status bar shows transaction count
2. **Open the file** - transactions/transactions.json should exist
3. **Click View History** - See your saved transactions

---

## ?? Example Output

After reading 3 cards, transactions.json will contain:

```json
[
  {
    "transactionId": "abc-123",
    "timestamp": "2026-01-01T10:30:00",
    "pan": "4111111111111111",
    "expiryDate": "2512",
    "cardholderName": "JOHN DOE",
    "slToken": "E94DB63485CB4F8C...",
    "readerName": "ACS ACR122U PICC Interface",
    "status": "Success",
    "processingTimeMs": 1234,
    "userName": "johndoe",
    "machineName": "DESKTOP-ABC"
  },
  ...
]
```

---

## ?? What's Next?

### More Features
- Export to CSV/XML: See `TRANSACTION_STORAGE_GUIDE.md`
- Search by PAN/Token: Examples in guide
- SQLite database: Better performance
- Delete old data: Cleanup examples

### UI Enhancements
- Add "Export" menu item
- Add "Clear History" button
- Add transaction grid view
- Add date range filter

---

## ?? Pro Tips

### Tip 1: Check Transaction Count

```csharp
private async void UpdateTransactionCount()
{
    var count = await _storage.GetCountAsync();
    lblTransactionCount.Text = $"Saved: {count}";
}
```

### Tip 2: Export for Excel

```csharp
await _storage.ExportAsync("report.csv", ExportFormat.Csv);
```

### Tip 3: Find Specific Card

```csharp
var history = await _storage.GetByPANAsync("4111111111111111");
MessageBox.Show($"Found {history.Count} transactions for this card");
```

---

## ? Troubleshooting

### "Type not found"
- Make sure all files are added to project
- Check using statement is present

### "Method not found"
- Make sure method is marked `async`
- Use `await` keyword

### "File access denied"
- Check folder permissions
- Make sure app has write access

---

## ?? Full Documentation

**Complete Guide:** `TRANSACTION_STORAGE_GUIDE.md`  
**API Reference:** Comments in source files  
**Examples:** See TRANSACTION_FEATURE_SUMMARY.md  

---

**Time Spent:** 5 minutes  
**Value Added:** Huge! ??  
**Difficulty:** Easy ?  

**You're all set! Start saving transactions now! ??**
