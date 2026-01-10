using System;
using System.Collections.Generic;
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
    /// Unit tests for JsonTransactionStorage
    /// </summary>
    public class JsonTransactionStorageTests : IDisposable
    {
        private readonly string _testStoragePath;
        private readonly JsonTransactionStorage _storage;

        public JsonTransactionStorageTests()
        {
            // Use unique test directory for each test run
            _testStoragePath = Path.Combine(Path.GetTempPath(), "EMVCard.Tests", Guid.NewGuid().ToString());
            _storage = new JsonTransactionStorage(_testStoragePath);
        }

        public void Dispose()
        {
            // Cleanup test directory
            if (Directory.Exists(_testStoragePath))
            {
                Directory.Delete(_testStoragePath, true);
            }
        }

        [Fact]
        public async Task SaveAsync_WithValidTransaction_ShouldSave()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "4111111111111111",
                ExpiryDate = "2025-12",
                Status = "Success"
            };

            // Act
            await _storage.SaveAsync(transaction);

            // Assert
            var retrieved = await _storage.GetByIdAsync(transaction.TransactionId);
            retrieved.Should().NotBeNull();
            retrieved.PAN.Should().Be(transaction.PAN);
            retrieved.Status.Should().Be(transaction.Status);
        }

        [Fact]
        public async Task SaveAsync_WithNullTransaction_ShouldThrowArgumentNullException()
        {
            // Act
            Func<Task> act = async () => await _storage.SaveAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("transaction");
        }

        [Fact]
        public async Task SaveAsync_UpdateExisting_ShouldUpdateTransaction()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "4111111111111111",
                Status = "Success"
            };
            await _storage.SaveAsync(transaction);

            // Act - Update the transaction
            transaction.Status = "Failed";
            transaction.ErrorMessage = "Card read error";
            await _storage.SaveAsync(transaction);

            // Assert
            var retrieved = await _storage.GetByIdAsync(transaction.TransactionId);
            retrieved.Status.Should().Be("Failed");
            retrieved.ErrorMessage.Should().Be("Card read error");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var result = await _storage.GetByIdAsync("non-existent-id");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WithNullId_ShouldThrowArgumentException()
        {
            // Act
            Func<Task> act = async () => await _storage.GetByIdAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Transaction ID cannot be null or empty*");
        }

        [Fact]
        public async Task GetAllAsync_WithNoTransactions_ShouldReturnEmptyList()
        {
            // Act
            var result = await _storage.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleTransactions_ShouldReturnAll()
        {
            // Arrange
            var transactions = new[]
            {
                new CardTransaction { PAN = "4111111111111111" },
                new CardTransaction { PAN = "5500000000000004" },
                new CardTransaction { PAN = "371449635398431" }
            };

            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Act
            var result = await _storage.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Select(t => t.PAN).Should().Contain(new[] 
            { 
                "4111111111111111", 
                "5500000000000004", 
                "371449635398431" 
            });
        }

        [Fact]
        public async Task GetByDateRangeAsync_ShouldFilterCorrectly()
        {
            // Arrange
            var baseDate = DateTime.Now.Date;
            var transactions = new[]
            {
                new CardTransaction { PAN = "1111", Timestamp = baseDate.AddDays(-5) },
                new CardTransaction { PAN = "2222", Timestamp = baseDate.AddDays(-3) },
                new CardTransaction { PAN = "3333", Timestamp = baseDate.AddDays(-1) },
                new CardTransaction { PAN = "4444", Timestamp = baseDate },
                new CardTransaction { PAN = "5555", Timestamp = baseDate.AddDays(1) }
            };

            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Act
            var result = await _storage.GetByDateRangeAsync(
                baseDate.AddDays(-3), 
                baseDate
            );

            // Assert
            result.Should().HaveCount(3);
            result.Select(t => t.PAN).Should().Contain(new[] { "2222", "3333", "4444" });
        }

        [Fact]
        public async Task GetByDateRangeAsync_ShouldReturnInDescendingOrder()
        {
            // Arrange
            var baseDate = DateTime.Now.Date;
            var transactions = new[]
            {
                new CardTransaction { PAN = "1111", Timestamp = baseDate.AddDays(-3) },
                new CardTransaction { PAN = "2222", Timestamp = baseDate.AddDays(-1) },
                new CardTransaction { PAN = "3333", Timestamp = baseDate }
            };

            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Act
            var result = await _storage.GetByDateRangeAsync(
                baseDate.AddDays(-5), 
                baseDate
            );

            // Assert
            result[0].PAN.Should().Be("3333"); // Most recent first
            result[1].PAN.Should().Be("2222");
            result[2].PAN.Should().Be("1111");
        }

        [Fact]
        public async Task GetByPANAsync_ShouldReturnMatchingTransactions()
        {
            // Arrange
            var targetPAN = "4111111111111111";
            var transactions = new[]
            {
                new CardTransaction { PAN = targetPAN, Timestamp = DateTime.Now },
                new CardTransaction { PAN = "5500000000000004", Timestamp = DateTime.Now },
                new CardTransaction { PAN = targetPAN, Timestamp = DateTime.Now.AddHours(1) }
            };

            foreach (var t in transactions)
            {
                await _storage.SaveAsync(t);
            }

            // Act
            var result = await _storage.GetByPANAsync(targetPAN);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.PAN == targetPAN);
        }

        [Fact]
        public async Task GetByPANAsync_WithNullPAN_ShouldThrowArgumentException()
        {
            // Act
            Func<Task> act = async () => await _storage.GetByPANAsync(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*PAN cannot be null or empty*");
        }

        [Fact]
        public async Task DeleteAsync_WithExistingTransaction_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var transaction = new CardTransaction { PAN = "4111111111111111" };
            await _storage.SaveAsync(transaction);

            // Act
            var result = await _storage.DeleteAsync(transaction.TransactionId);

            // Assert
            result.Should().BeTrue();
            var retrieved = await _storage.GetByIdAsync(transaction.TransactionId);
            retrieved.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentTransaction_ShouldReturnFalse()
        {
            // Act
            var result = await _storage.DeleteAsync("non-existent-id");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAllAsync_ShouldDeleteAllAndReturnCount()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                await _storage.SaveAsync(new CardTransaction { PAN = $"PAN{i}" });
            }

            // Act
            var count = await _storage.DeleteAllAsync();

            // Assert
            count.Should().Be(5);
            var remaining = await _storage.GetAllAsync();
            remaining.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            for (int i = 0; i < 3; i++)
            {
                await _storage.SaveAsync(new CardTransaction { PAN = $"PAN{i}" });
            }

            // Act
            var count = await _storage.GetCountAsync();

            // Assert
            count.Should().Be(3);
        }

        [Fact]
        public async Task ExportAsync_ToJson_ShouldCreateFile()
        {
            // Arrange
            await _storage.SaveAsync(new CardTransaction { PAN = "4111111111111111" });
            var exportPath = Path.Combine(_testStoragePath, "export.json");

            // Act
            await _storage.ExportAsync(exportPath, ExportFormat.Json);

            // Assert
            File.Exists(exportPath).Should().BeTrue();
            var content = File.ReadAllText(exportPath);
            content.Should().Contain("4111111111111111");
        }

        [Fact]
        public async Task ExportAsync_ToCsv_ShouldCreateFile()
        {
            // Arrange
            await _storage.SaveAsync(new CardTransaction 
            { 
                PAN = "4111111111111111",
                Status = "Success"
            });
            var exportPath = Path.Combine(_testStoragePath, "export.csv");

            // Act
            await _storage.ExportAsync(exportPath, ExportFormat.Csv);

            // Assert
            File.Exists(exportPath).Should().BeTrue();
            var content = File.ReadAllText(exportPath);
            content.Should().Contain("TransactionId,Timestamp,PAN"); // Header
            content.Should().Contain("4111111111111111");
            content.Should().Contain("Success");
        }

        [Fact]
        public async Task ExportAsync_ToXml_ShouldCreateFile()
        {
            // Arrange
            await _storage.SaveAsync(new CardTransaction 
            { 
                PAN = "4111111111111111",
                CardholderName = "JOHN DOE"
            });
            var exportPath = Path.Combine(_testStoragePath, "export.xml");

            // Act
            await _storage.ExportAsync(exportPath, ExportFormat.Xml);

            // Assert
            File.Exists(exportPath).Should().BeTrue();
            var content = File.ReadAllText(exportPath);
            content.Should().Contain("<?xml");
            content.Should().Contain("<Transactions>");
            content.Should().Contain("<PAN>4111111111111111</PAN>");
            content.Should().Contain("<CardholderName>JOHN DOE</CardholderName>");
        }

        [Fact]
        public async Task ExportAsync_WithNullPath_ShouldThrowArgumentException()
        {
            // Act
            Func<Task> act = async () => await _storage.ExportAsync(null, ExportFormat.Json);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*File path cannot be null or empty*");
        }
    }
}
