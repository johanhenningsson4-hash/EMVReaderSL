# ClearBuffers Issue Fix

## Problem

The `bReadApp_Click` method was calling `ClearBuffers()` at the start, which cleared the `_appDisplayNameToInfo` dictionary BEFORE checking if an application was selected:

```csharp
private void bReadApp_Click(object sender, EventArgs e) {
    ClearBuffers();  // ? This clears _appDisplayNameToInfo!

    // Check selection (but dictionary is now empty!)
    if (string.IsNullOrEmpty(cbPSE.Text) || !_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
    {
        displayOut(0, 0, "Please select an application");  // ? Always triggered!
        return;
    }
    // ...
}
```

### Why It Failed

1. User selects "1. Visa Credit" from dropdown
2. User clicks "Read App" button
3. `bReadApp_Click()` is called
4. `ClearBuffers()` clears `_appDisplayNameToInfo` dictionary
5. Check `_appDisplayNameToInfo.ContainsKey(cbPSE.Text)` returns `false` (dictionary is empty!)
6. Shows "Please select an application" error

## Solution

**Move the selection check BEFORE clearing** and only clear the card data fields:

```csharp
private void bReadApp_Click(object sender, EventArgs e) {
    // ? Check selection FIRST (before clearing)
    if (string.IsNullOrEmpty(cbPSE.Text) || !_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
    {
        displayOut(0, 0, "Please select an application");
        return;
    }

    var selectedApp = _appDisplayNameToInfo[cbPSE.Text];
    
    // ? Now clear only card data fields (not the application selection)
    _currentCardData.Clear();
    textCardNum.Text = "";
    textEXP.Text = "";
    textHolder.Text = "";
    textTrack.Text = "";
    textIccCert.Text = "";
    txtSLToken.Text = "";
    
    // Continue with card reading...
}
```

## Key Changes

### 1. Selection Check Moved Up
```csharp
// BEFORE: Check after clearing (always fails)
ClearBuffers();
if (!_appDisplayNameToInfo.ContainsKey(cbPSE.Text))

// AFTER: Check before clearing (works correctly)
if (!_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
    return;
// Only clear if check passes
```

### 2. Selective Clearing
Instead of calling `ClearBuffers()` which clears everything including:
- `cbPSE.Items.Clear()`
- `cbPSE.Text = ""`
- `_appDisplayNameToInfo?.Clear()`

We now only clear the **card data fields**:
- `_currentCardData.Clear()`
- Individual text boxes
- `txtSLToken.Text`

This preserves the application selection while clearing previous card data.

## Why This Design is Better

### Clear Separation of Concerns

**Application Selection** (persists during read):
- `cbPSE` ComboBox
- `_appDisplayNameToInfo` dictionary
- `_applications` list

**Card Data** (cleared on each read):
- `_currentCardData`
- Text boxes (PAN, expiry, holder, track2, cert)
- SL Token

### User Experience Flow

```
1. User loads PSE/PPSE
   ?
2. User selects application (persists)
   ?
3. User clicks "Read App"
   ?
4. Previous card data cleared ?
   Application selection preserved ?
   ?
5. New card data read and displayed
   ?
6. User can click "Read App" again
   Same application still selected ?
```

## Alternative Approaches Considered

### Option 1: Don't Clear at All ?
```csharp
// No clearing - old data mixed with new
private void bReadApp_Click(...) {
    // Just read and update
}
```
**Problem:** Old card data remains if new read fails partially

### Option 2: Clear After Selection ?
```csharp
var selectedApp = _appDisplayNameToInfo[cbPSE.Text];
ClearBuffers();
// Continue...
```
**Problem:** Still clears the dropdown and dictionary

### Option 3: Selective Clear (Chosen) ?
```csharp
// Check first, then clear only card data
if (!_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
    return;
var selectedApp = _appDisplayNameToInfo[cbPSE.Text];
// Clear only card data fields
```
**Advantages:**
- Preserves application selection
- Clears old card data
- Best user experience

## Updated ClearBuffers Usage

`ClearBuffers()` should now only be called when we want to **completely reset** the UI:

### When to Call ClearBuffers()
- ? `bConnect_Click()` - Starting fresh connection
- ? `bLoadPSE_Click()` - Loading new application list
- ? `bLoadPPSE_Click()` - Loading new application list  
- ? `bReset_Click()` - Full reset

### When NOT to Call ClearBuffers()
- ? `bReadApp_Click()` - Only clear card data, keep app selection

## Testing Scenarios

### Scenario 1: Normal Read
```
1. Load PPSE ? ? Apps loaded
2. Select "1. Visa Credit" ? ? Selected
3. Click "Read App" ? ? Reads card successfully
4. Click "Read App" again ? ? Still works (selection preserved)
```

### Scenario 2: Multiple Reads
```
1. Load PPSE ? ? Apps loaded
2. Select "1. Visa Credit" ? ? Selected
3. Click "Read App" ? ? First read
4. Remove card
5. Insert different card
6. Click "Read App" ? ? Second read (same app, new card)
```

### Scenario 3: Change Application
```
1. Load PPSE ? ? Apps loaded
2. Select "1. Visa Credit" ? ? Selected
3. Click "Read App" ? ? Reads card
4. Select "2. Mastercard Debit" ? ? Changed selection
5. Click "Read App" ? ? Reads with new app
```

### Scenario 4: No Selection
```
1. Load PPSE ? ? Apps loaded
2. Clear dropdown text manually
3. Click "Read App" ? ? Shows "Please select an application"
```

## Summary

The fix resolves the issue by:

? **Checking selection BEFORE clearing** - Ensures dictionary exists when checked  
? **Selective clearing** - Only clears card data, preserves app selection  
? **Better UX** - User can read multiple cards with same app selection  
? **Logical flow** - Validate ? Get selection ? Clear old data ? Read new data  

**Build Status:** ? Successful  
**Functionality:** ? Application selection now works correctly!
