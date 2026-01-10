using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EMVCard;

namespace EMVCard.Storage
{
    /// <summary>
    /// SQLite-based transaction storage implementation (minimal, for testing)
    /// </summary>
    public class SQLiteTransactionStorage : ITransactionStorage
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public SQLiteTransactionStorage(string storagePath = "transactions")
        {
            _dbPath = Path.Combine(storagePath, "transactions.db");
            Directory.CreateDirectory(storagePath);
            _connectionString = $"Data Source={_dbPath};Version=3;";
            EnsureSchema();
        }

        private void EnsureSchema()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Transactions (
                        TransactionId TEXT PRIMARY KEY,
                        Timestamp TEXT,
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
                    )";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Task SaveAsync(CardTransaction transaction)
        {
            return Task.Run(() =>
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"REPLACE INTO Transactions (
                            TransactionId, Timestamp, PAN, ExpiryDate, CardholderName, ICCCertificate, Track2Data, ReaderName, TransactionType, Status, ErrorMessage, ProcessingTimeMs, Notes, UserName, MachineName, ApplicationVersion
                        ) VALUES (
                            @TransactionId, @Timestamp, @PAN, @ExpiryDate, @CardholderName, @ICCCertificate, @Track2Data, @ReaderName, @TransactionType, @Status, @ErrorMessage, @ProcessingTimeMs, @Notes, @UserName, @MachineName, @ApplicationVersion
                        )";
                        cmd.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
                        cmd.Parameters.AddWithValue("@Timestamp", transaction.Timestamp.ToString("o"));
                        cmd.Parameters.AddWithValue("@PAN", transaction.PAN);
                        cmd.Parameters.AddWithValue("@ExpiryDate", transaction.ExpiryDate);
                        cmd.Parameters.AddWithValue("@CardholderName", transaction.CardholderName);
                        cmd.Parameters.AddWithValue("@ICCCertificate", transaction.ICCCertificate);
                        cmd.Parameters.AddWithValue("@Track2Data", transaction.Track2Data);
                        cmd.Parameters.AddWithValue("@ReaderName", transaction.ReaderName);
                        cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                        cmd.Parameters.AddWithValue("@Status", transaction.Status);
                        cmd.Parameters.AddWithValue("@ErrorMessage", transaction.ErrorMessage);
                        cmd.Parameters.AddWithValue("@ProcessingTimeMs", transaction.ProcessingTimeMs);
                        cmd.Parameters.AddWithValue("@Notes", transaction.Notes);
                        cmd.Parameters.AddWithValue("@UserName", transaction.UserName);
                        cmd.Parameters.AddWithValue("@MachineName", transaction.MachineName);
                        cmd.Parameters.AddWithValue("@ApplicationVersion", transaction.ApplicationVersion);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        public Task<CardTransaction> GetByIdAsync(string transactionId)
        {
            return Task.Run(() =>
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Transactions WHERE TransactionId = @TransactionId";
                        cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return ReadTransaction(reader);
                        }
                    }
                }
                return null;
            });
        }

        public Task<List<CardTransaction>> GetAllAsync()
        {
            return Task.Run(() =>
            {
                var list = new List<CardTransaction>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Transactions";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(ReadTransaction(reader));
                        }
                    }
                }
                return list;
            });
        }

        public Task<List<CardTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return Task.Run(() =>
            {
                var list = new List<CardTransaction>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Transactions WHERE Timestamp >= @Start AND Timestamp <= @End ORDER BY Timestamp DESC";
                        cmd.Parameters.AddWithValue("@Start", startDate.ToString("o"));
                        cmd.Parameters.AddWithValue("@End", endDate.ToString("o"));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(ReadTransaction(reader));
                        }
                    }
                }
                return list;
            });
        }

        public Task<List<CardTransaction>> GetByPANAsync(string pan)
        {
            return Task.Run(() =>
            {
                var list = new List<CardTransaction>();
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Transactions WHERE PAN = @PAN ORDER BY Timestamp DESC";
                        cmd.Parameters.AddWithValue("@PAN", pan);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add(ReadTransaction(reader));
                        }
                    }
                }
                return list;
            });
        }

        public Task<List<CardTransaction>> GetBySLTokenAsync(string slToken)
        {
            // Not implemented in CardTransaction
            return Task.FromResult(new List<CardTransaction>());
        }

        public Task<bool> DeleteAsync(string transactionId)
        {
            return Task.Run(() =>
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Transactions WHERE TransactionId = @TransactionId";
                        cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            });
        }

        public Task<int> DeleteAllAsync()
        {
            return Task.Run(() =>
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Transactions";
                        return cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        public Task<int> GetCountAsync()
        {
            return Task.Run(() =>
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM Transactions";
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            });
        }

        public Task ExportAsync(string filePath, ExportFormat format)
        {
            return Task.Run(async () =>
            {
                var transactions = await GetAllAsync();
                switch (format)
                {
                    case ExportFormat.Json:
                        File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(transactions, Newtonsoft.Json.Formatting.Indented));
                        break;
                    case ExportFormat.Csv:
                        using (var writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine("TransactionId,Timestamp,PAN,ExpiryDate,CardholderName,ICCCertificate,Track2Data,ReaderName,Status,ProcessingTimeMs");
                            foreach (var t in transactions)
                            {
                                writer.WriteLine(string.Format(
                                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",{9}",
                                    t.TransactionId,
                                    t.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                                    t.PAN,
                                    t.ExpiryDate,
                                    t.CardholderName,
                                    t.ICCCertificate != null ? t.ICCCertificate.Replace("\"", "\"\"") : "",
                                    t.Track2Data,
                                    t.ReaderName,
                                    t.Status,
                                    t.ProcessingTimeMs));
                            }
                        }
                        break;
                    case ExportFormat.Xml:
                        using (var writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                            writer.WriteLine("<Transactions>");
                            foreach (var t in transactions)
                            {
                                writer.WriteLine("  <Transaction>");
                                writer.WriteLine(string.Format("    <TransactionId>{0}</TransactionId>", t.TransactionId));
                                writer.WriteLine(string.Format("    <Timestamp>{0}</Timestamp>", t.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")));
                                writer.WriteLine(string.Format("    <PAN>{0}</PAN>", t.PAN));
                                writer.WriteLine(string.Format("    <ExpiryDate>{0}</ExpiryDate>", t.ExpiryDate));
                                writer.WriteLine(string.Format("    <CardholderName>{0}</CardholderName>", t.CardholderName));
                                writer.WriteLine(string.Format("    <ReaderName>{0}</ReaderName>", t.ReaderName));
                                writer.WriteLine(string.Format("    <Status>{0}</Status>", t.Status));
                                writer.WriteLine(string.Format("    <ProcessingTimeMs>{0}</ProcessingTimeMs>", t.ProcessingTimeMs));
                                writer.WriteLine("  </Transaction>");
                            }
                            writer.WriteLine("</Transactions>");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unsupported export format: {format}");
                }
            });
        }

        private CardTransaction ReadTransaction(SQLiteDataReader reader)
        {
            return new CardTransaction
            {
                TransactionId = reader["TransactionId"] as string,
                Timestamp = DateTime.TryParse(reader["Timestamp"] as string, out var ts) ? ts : DateTime.Now,
                PAN = reader["PAN"] as string,
                ExpiryDate = reader["ExpiryDate"] as string,
                CardholderName = reader["CardholderName"] as string,
                ICCCertificate = reader["ICCCertificate"] as string,
                Track2Data = reader["Track2Data"] as string,
                ReaderName = reader["ReaderName"] as string,
                TransactionType = reader["TransactionType"] as string,
                Status = reader["Status"] as string,
                ErrorMessage = reader["ErrorMessage"] as string,
                ProcessingTimeMs = reader["ProcessingTimeMs"] is long l ? l : Convert.ToInt64(reader["ProcessingTimeMs"]),
                Notes = reader["Notes"] as string,
                UserName = reader["UserName"] as string,
                MachineName = reader["MachineName"] as string,
                ApplicationVersion = reader["ApplicationVersion"] as string
            };
        }
    }
}
