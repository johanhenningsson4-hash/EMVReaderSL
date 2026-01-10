using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EMVCard.Storage
{
    /// <summary>
    /// JSON file-based transaction storage implementation
    /// Compatible with .NET Framework 4.7.2 using Newtonsoft.Json
    /// </summary>
    public class JsonTransactionStorage : ITransactionStorage
    {
        private readonly string _storagePath;
        private readonly string _dataFile;
        private readonly JsonSerializerSettings _jsonSettings;

        public JsonTransactionStorage(string storagePath = "transactions")
        {
            _storagePath = storagePath;
            _dataFile = Path.Combine(_storagePath, "transactions.json");
            
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            EnsureStorageExists();
        }

        private void EnsureStorageExists()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }

            if (!File.Exists(_dataFile))
            {
                File.WriteAllText(_dataFile, "[]");
            }
        }

        public Task SaveAsync(CardTransaction transaction)
        {
            return Task.Run(() =>
            {
                if (transaction == null)
                    throw new ArgumentNullException(nameof(transaction));

                var transactions = LoadAllTransactions();
                
                var existingIndex = transactions.FindIndex(t => t.TransactionId == transaction.TransactionId);
                if (existingIndex >= 0)
                {
                    transactions[existingIndex] = transaction;
                }
                else
                {
                    transactions.Add(transaction);
                }

                SaveAllTransactions(transactions);
            });
        }

        public Task<CardTransaction> GetByIdAsync(string transactionId)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(transactionId))
                    throw new ArgumentException("Transaction ID cannot be null or empty", nameof(transactionId));

                var transactions = LoadAllTransactions();
                return transactions.FirstOrDefault(t => t.TransactionId == transactionId);
            });
        }

        public Task<List<CardTransaction>> GetAllAsync()
        {
            return Task.Run(() => LoadAllTransactions());
        }

        public Task<List<CardTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return Task.Run(() =>
            {
                var transactions = LoadAllTransactions();
                return transactions
                    .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();
            });
        }

        public Task<List<CardTransaction>> GetByPANAsync(string pan)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(pan))
                    throw new ArgumentException("PAN cannot be null or empty", nameof(pan));

                var transactions = LoadAllTransactions();
                return transactions
                    .Where(t => t.PAN == pan)
                    .OrderByDescending(t => t.Timestamp)
                    .ToList();
            });
        }

        public Task<List<CardTransaction>> GetBySLTokenAsync(string slToken)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(slToken))
                    throw new ArgumentException("SL Token cannot be null or empty", nameof(slToken));

                // Note: SL Token not currently in CardTransaction model
                // Return empty list for now
                return new List<CardTransaction>();
            });
        }

        public Task<bool> DeleteAsync(string transactionId)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(transactionId))
                    throw new ArgumentException("Transaction ID cannot be null or empty", nameof(transactionId));

                var transactions = LoadAllTransactions();
                var removed = transactions.RemoveAll(t => t.TransactionId == transactionId);
                
                if (removed > 0)
                {
                    SaveAllTransactions(transactions);
                    return true;
                }

                return false;
            });
        }

        public Task<int> DeleteAllAsync()
        {
            return Task.Run(() =>
            {
                var transactions = LoadAllTransactions();
                var count = transactions.Count;
                
                SaveAllTransactions(new List<CardTransaction>());
                
                return count;
            });
        }

        public Task<int> GetCountAsync()
        {
            return Task.Run(() =>
            {
                var transactions = LoadAllTransactions();
                return transactions.Count;
            });
        }

        public Task ExportAsync(string filePath, ExportFormat format)
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

                var transactions = LoadAllTransactions();

                switch (format)
                {
                    case ExportFormat.Json:
                        ExportToJson(transactions, filePath);
                        break;
                    case ExportFormat.Csv:
                        ExportToCsv(transactions, filePath);
                        break;
                    case ExportFormat.Xml:
                        ExportToXml(transactions, filePath);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Unsupported export format: {0}", format));
                }
            });
        }

        private List<CardTransaction> LoadAllTransactions()
        {
            try
            {
                var json = File.ReadAllText(_dataFile);
                return JsonConvert.DeserializeObject<List<CardTransaction>>(json, _jsonSettings) 
                    ?? new List<CardTransaction>();
            }
            catch (JsonException)
            {
                return new List<CardTransaction>();
            }
            catch (Exception)
            {
                return new List<CardTransaction>();
            }
        }

        private void SaveAllTransactions(List<CardTransaction> transactions)
        {
            var json = JsonConvert.SerializeObject(transactions, _jsonSettings);
            File.WriteAllText(_dataFile, json);
        }

        private void ExportToJson(List<CardTransaction> transactions, string filePath)
        {
            var json = JsonConvert.SerializeObject(transactions, _jsonSettings);
            File.WriteAllText(filePath, json);
        }

        private void ExportToCsv(List<CardTransaction> transactions, string filePath)
        {
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
        }

        private void ExportToXml(List<CardTransaction> transactions, string filePath)
        {
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
        }
    }
}
