# SQLite Transaction Storage (Optional)

**Status:** ?? OPTIONAL - Requires Additional NuGet Package

---

## ?? Installation

To use SQLite storage, install the required package:

```powershell
cd EMVCard.Core
dotnet add package System.Data.SQLite --version 1.0.118
```

---

## ?? Implementation

Create a file `Storage\SQLiteTransactionStorage.cs` with the following content:

```csharp
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace EMVCard.Storage
{
    /// <summary>
    /// SQLite database-based transaction storage
    /// Requires: System.Data.SQLite NuGet package
    /// </summary>
    public class SQLiteTransactionStorage : ITransactionStorage
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public SQLiteTransactionStorage(string storagePath = "transactions")
        {
            _databasePath = Path.Combine(storagePath, "transactions.db");
            _connectionString = string.Format("Data Source={0};Version=3;", _databasePath);
            EnsureDatabaseExists();
        }

        private void EnsureDatabaseExists()
        {
            var directory = Path.GetDirectoryName(_databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool isNewDatabase = !File.Exists(_databasePath);
            if (isNewDatabase)
            {
                SQLiteConnection.CreateFile(_databasePath);
                CreateTables();
            }
        }

        private void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var createTableSql = @"
                    CREATE TABLE IF NOT EXISTS Transactions (
                        TransactionId TEXT PRIMARY KEY,
                        Timestamp TEXT NOT NULL,
                        PAN TEXT,
                        ExpiryDate TEXT,
                        CardholderName TEXT,
                        ICCCertificate TEXT,
                        Track2Data TEXT,
                        ReaderName TEXT,
                        TransactionType TEXT,
                        Status TEXT,
                        ErrorMessage TEXT,
                        ProcessingTimeMs INTEGER,
                        Notes TEXT,
                        UserName TEXT,
                        MachineName TEXT,
                        ApplicationVersion TEXT
                    );
                    CREATE INDEX IF NOT EXISTS idx_timestamp ON Transactions(Timestamp);
                    CREATE INDEX IF NOT EXISTS idx_pan ON Transactions(PAN);
                    CREATE INDEX IF NOT EXISTS idx_status ON Transactions(Status);
                ";
                using (var command = new SQLiteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public Task SaveAsync(CardTransaction transaction)
        {
            return Task.Run(() =>
            {
                if (transaction == null)
                    throw new ArgumentNullException(nameof(transaction));

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"
                        INSERT OR REPLACE INTO Transactions 
                        (TransactionId, Timestamp, PAN, ExpiryDate, CardholderName, ICCCertificate, 
                         Track2Data, ReaderName, TransactionType, Status, ErrorMessage, 
                         ProcessingTimeMs, Notes, UserName, MachineName, ApplicationVersion)
                        VALUES 
                        (@TransactionId, @Timestamp, @PAN, @ExpiryDate, @CardholderName, @ICCCertificate,
                         @Track2Data, @ReaderName, @TransactionType, @Status, @ErrorMessage,
                         @ProcessingTimeMs, @Notes, @UserName, @MachineName, @ApplicationVersion)
                    ";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
                        command.Parameters.AddWithValue("@Timestamp", transaction.Timestamp.ToString("O"));
                        command.Parameters.AddWithValue("@PAN", transaction.PAN ?? string.Empty);
                        command.Parameters.AddWithValue("@ExpiryDate", transaction.ExpiryDate ?? string.Empty);
                        command.Parameters.AddWithValue("@CardholderName", transaction.CardholderName ?? string.Empty);
                        command.Parameters.AddWithValue("@ICCCertificate", transaction.ICCCertificate ?? string.Empty);
                        command.Parameters.AddWithValue("@Track2Data", transaction.Track2Data ?? string.Empty);
                        command.Parameters.AddWithValue("@ReaderName", transaction.ReaderName ?? string.Empty);
                        command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType ?? string.Empty);
                        command.Parameters.AddWithValue("@Status", transaction.Status ?? string.Empty);
                        command.Parameters.AddWithValue("@ErrorMessage", transaction.ErrorMessage ?? string.Empty);
                        command.Parameters.AddWithValue("@ProcessingTimeMs", transaction.ProcessingTimeMs);
                        command.Parameters.AddWithValue("@Notes", transaction.Notes ?? string.Empty);
                        command.Parameters.AddWithValue("@UserName", transaction.UserName ?? string.Empty);
                        command.Parameters.AddWithValue("@MachineName", transaction.MachineName ?? string.Empty);
                        command.Parameters.AddWithValue("@ApplicationVersion", transaction.ApplicationVersion ?? string.Empty);
                        command.ExecuteNonQuery();
                    }
                }
            });
        }

        // Implement other ITransactionStorage methods similarly...
        // See full implementation in documentation
    }
}
```

---

## ?? Usage

```csharp
// After installing System.Data.SQLite package
using EMVCard.Storage;

var storage = new SQLiteTransactionStorage("transactions");
await storage.SaveAsync(transaction);
```

---

## ?? Recommendation

**For most users, use `JsonTransactionStorage` instead:**
- No additional packages required
- Simpler setup
- Good performance for typical use
- Easy to backup (single JSON file)

**Use SQLite only if:**
- You need to store thousands of transactions
- You need complex queries
- You want database-level performance

---

## ? Already Included

The JSON storage implementation is already included and ready to use:
- ? No additional packages needed (Newtonsoft.Json already added)
- ? Compatible with .NET Framework 4.7.2
- ? Simple and reliable

---

**Status:** ?? Optional feature - add only if needed
