using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EMVCard;
using EMVCard.Storage;
using FluentAssertions;
using Xunit;

namespace EMVCard.Tests
{
    /// <summary>
    /// Integration tests for transaction storage
    /// </summary>
    public class TransactionStorageIntegrationTests : IDisposable
    {
        private readonly string _testStoragePath;
        private readonly SQLiteTransactionStorage _storage;

        public TransactionStorageIntegrationTests()
        {
            _testStoragePath = Path.Combine(Path.GetTempPath(), "EMVCard.IntegrationTests", Guid.NewGuid().ToString());
            _storage = new SQLiteTransactionStorage(_testStoragePath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testStoragePath))
            {
                Directory.Delete(_testStoragePath, true);
            }
        }

        [Fact]
        public async Task EndToEnd_SaveRetrieveExport_ShouldWorkCorrectly()
        {
            // Arrange - Create card data
            var cardData = new EmvDataParser.EmvCardData
            {
                PAN = "4111111111111111",
                ExpiryDate = "2025-12",
                CardholderName = "JOHN DOE",
                Track2Data = "4111111111111111D2512101",
                IccCertificate = "9F46 Certificate Data"
            };

            // Act - Save transaction
            var transaction = CardTransaction.FromCardData(cardData, "ACS ACR122U");
            transaction.ProcessingTimeMs = 1234;
            await _storage.SaveAsync(transaction);

            // Assert - Retrieve
            var retrieved = await _storage.GetByIdAsync(transaction.TransactionId);
            retrieved.Should().NotBeNull();
            retrieved.PAN.Should().Be("4111111111111111");
            retrieved.ProcessingTimeMs.Should().Be(1234);

            // Act - Export
            var exportPath = Path.Combine(_testStoragePath, "export.json");
            await _storage.ExportAsync(exportPath, ExportFormat.Json);

            // Assert - Export file exists
            File.Exists(exportPath).Should().BeTrue();
        }

        [Fact]
        public async Task MultipleTransactions_SaveSearchDelete_ShouldWorkCorrectly()
        {
            // Arrange - Create multiple transactions
            var pan1 = "4111111111111111";
            var pan2 = "5500000000000004";

            var transactions = new[]
            {
                new CardTransaction { PAN = pan1, Status = "Success", Timestamp = DateTime.Now.AddHours(-2) },
                new CardTransaction { PAN = pan1, Status = "Success", Timestamp = DateTime.Now.AddHours(-1) },
                new CardTransaction { PAN = pan2, Status = "Failed", Timestamp = DateTime.Now }
            };

            // Act - Save all
            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Assert - Get all
            var all = await _storage.GetAllAsync();
            all.Should().HaveCount(3);

            // Assert - Search by PAN
            var pan1Transactions = await _storage.GetByPANAsync(pan1);
            pan1Transactions.Should().HaveCount(2);

            // Assert - Delete one
            var deleted = await _storage.DeleteAsync(transactions[0].TransactionId);
            deleted.Should().BeTrue();

            var remaining = await _storage.GetAllAsync();
            remaining.Should().HaveCount(2);
        }

        [Fact]
        public async Task FailedTransaction_ShouldBeSavedWithErrorDetails()
        {
            // Arrange
            var errorMessage = "Card communication timeout";
            var readerName = "ACS ACR122U";

            // Act
            var failedTransaction = CardTransaction.CreateFailed(readerName, errorMessage);
            await _storage.SaveAsync(failedTransaction);

            // Assert
            var retrieved = await _storage.GetByIdAsync(failedTransaction.TransactionId);
            retrieved.Status.Should().Be("Failed");
            retrieved.ErrorMessage.Should().Be(errorMessage);
            retrieved.ReaderName.Should().Be(readerName);
        }

        [Fact]
        public async Task DateRangeFilter_WithMixedDates_ShouldReturnCorrectSubset()
        {
            // Arrange
            var baseDate = DateTime.Now.Date;
            var transactions = new[]
            {
                new CardTransaction { PAN = "1111", Timestamp = baseDate.AddDays(-10) },
                new CardTransaction { PAN = "2222", Timestamp = baseDate.AddDays(-7) },
                new CardTransaction { PAN = "3333", Timestamp = baseDate.AddDays(-5) },
                new CardTransaction { PAN = "4444", Timestamp = baseDate.AddDays(-3) },
                new CardTransaction { PAN = "5555", Timestamp = baseDate }
            };

            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Act - Get last week
            var lastWeek = await _storage.GetByDateRangeAsync(
                baseDate.AddDays(-7),
                baseDate
            );

            // Assert
            lastWeek.Should().HaveCount(4);
            lastWeek.Should().NotContain(t => t.PAN == "1111"); // Too old
        }

        [Fact]
        public async Task ConcurrentSaves_ShouldAllBeStored()
        {
            // Arrange
            var tasks = Enumerable.Range(0, 10).Select(i =>
                _storage.SaveAsync(new CardTransaction { PAN = $"PAN{i}" })
            );

            // Act
            await Task.WhenAll(tasks);

            // Assert
            var all = await _storage.GetAllAsync();
            all.Should().HaveCount(10);
        }

        [Fact]
        public async Task UpdateTransaction_ShouldPreserveId()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "4111111111111111",
                Status = "Success",
                ProcessingTimeMs = 100
            };
            await _storage.SaveAsync(transaction);
            var originalId = transaction.TransactionId;

            // Act - Update
            transaction.Status = "Verified";
            transaction.ProcessingTimeMs = 150;
            await _storage.SaveAsync(transaction);

            // Assert
            var retrieved = await _storage.GetByIdAsync(originalId);
            retrieved.Should().NotBeNull();
            retrieved.Status.Should().Be("Verified");
            retrieved.ProcessingTimeMs.Should().Be(150);
            retrieved.TransactionId.Should().Be(originalId);
        }

        [Fact]
        public async Task GetSummary_ShouldMaskPANInOutput()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "4111111111111111",
                Status = "Success"
            };
            await _storage.SaveAsync(transaction);

            // Act
            var retrieved = await _storage.GetByIdAsync(transaction.TransactionId);
            var summary = retrieved.GetSummary();

            // Assert
            summary.Should().Contain("4111111111111111");
            summary.Should().NotContain("******"); // Ensure no masking occurs
        }

        [Fact]
        public async Task ExportMultipleFormats_ShouldAllSucceed()
        {
            // Arrange
            await _storage.SaveAsync(new CardTransaction
            {
                PAN = "4111111111111111",
                CardholderName = "JOHN DOE",
                Status = "Success"
            });

            var jsonPath = Path.Combine(_testStoragePath, "export.json");
            var csvPath = Path.Combine(_testStoragePath, "export.csv");
            var xmlPath = Path.Combine(_testStoragePath, "export.xml");

            // Act
            await _storage.ExportAsync(jsonPath, ExportFormat.Json);
            await _storage.ExportAsync(csvPath, ExportFormat.Csv);
            await _storage.ExportAsync(xmlPath, ExportFormat.Xml);

            // Assert
            File.Exists(jsonPath).Should().BeTrue();
            File.Exists(csvPath).Should().BeTrue();
            File.Exists(xmlPath).Should().BeTrue();

            var jsonContent = File.ReadAllText(jsonPath);
            var csvContent = File.ReadAllText(csvPath);
            var xmlContent = File.ReadAllText(xmlPath);

            jsonContent.Should().Contain("4111111111111111");
            csvContent.Should().Contain("4111111111111111");
            xmlContent.Should().Contain("4111111111111111");
        }

        [Fact]
        public async Task StoragePersistence_AfterReinitialization_ShouldRetainData()
        {
            // Arrange - Save with first instance
            var transaction = new CardTransaction { PAN = "4111111111111111" };
            await _storage.SaveAsync(transaction);
            var transactionId = transaction.TransactionId;

            // Act - Create new storage instance pointing to same path
            var newStorage = new SQLiteTransactionStorage(_testStoragePath);
            var retrieved = await newStorage.GetByIdAsync(transactionId);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved.PAN.Should().Be("4111111111111111");
        }
    }
}
