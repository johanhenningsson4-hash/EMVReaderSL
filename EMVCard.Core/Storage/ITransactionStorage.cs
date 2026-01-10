using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMVCard.Storage
{
    /// <summary>
    /// Interface for transaction storage implementations
    /// </summary>
    public interface ITransactionStorage
    {
        /// <summary>
        /// Saves a transaction
        /// </summary>
        Task SaveAsync(CardTransaction transaction);

        /// <summary>
        /// Retrieves a transaction by ID
        /// </summary>
        Task<CardTransaction> GetByIdAsync(string transactionId);

        /// <summary>
        /// Retrieves all transactions
        /// </summary>
        Task<List<CardTransaction>> GetAllAsync();

        /// <summary>
        /// Retrieves transactions within a date range
        /// </summary>
        Task<List<CardTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves transactions by PAN
        /// </summary>
        Task<List<CardTransaction>> GetByPANAsync(string pan);

        /// <summary>
        /// Retrieves transactions by SL Token
        /// </summary>
        Task<List<CardTransaction>> GetBySLTokenAsync(string slToken);

        /// <summary>
        /// Deletes a transaction
        /// </summary>
        Task<bool> DeleteAsync(string transactionId);

        /// <summary>
        /// Deletes all transactions
        /// </summary>
        Task<int> DeleteAllAsync();

        /// <summary>
        /// Gets transaction count
        /// </summary>
        Task<int> GetCountAsync();

        /// <summary>
        /// Exports transactions to a file
        /// </summary>
        Task ExportAsync(string filePath, ExportFormat format);
    }

    /// <summary>
    /// Export format enumeration
    /// </summary>
    public enum ExportFormat
    {
        Json,
        Xml,
        Csv
    }

    /// <summary>
    /// Storage configuration
    /// </summary>
    public class StorageConfiguration
    {
        public StorageType StorageType { get; set; } = StorageType.Json;
        public string StoragePath { get; set; } = "transactions";
        public bool EnableAutoBackup { get; set; } = true;
        public int MaxTransactions { get; set; } = 10000;
        public int RetentionDays { get; set; } = 365;
    }

    /// <summary>
    /// Storage type enumeration
    /// </summary>
    public enum StorageType
    {
        Json,
        Xml,
        Csv,
        SQLite,
        SqlServer
    }
}
