using System;
using EMVCard;
using FluentAssertions;
using Xunit;

namespace EMVCard.Tests
{
    /// <summary>
    /// Unit tests for CardTransaction model
    /// </summary>
    public class CardTransactionTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var transaction = new CardTransaction();

            // Assert
            transaction.TransactionId.Should().NotBeNullOrEmpty();
            transaction.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            transaction.TransactionType.Should().Be("Read");
            transaction.Status.Should().Be("Unknown");
            transaction.UserName.Should().Be(Environment.UserName);
            transaction.MachineName.Should().Be(Environment.MachineName);
            transaction.ApplicationVersion.Should().Be("2.0.0");
        }

        [Fact]
        public void TransactionId_ShouldBeUniqueGuid()
        {
            // Act
            var transaction1 = new CardTransaction();
            var transaction2 = new CardTransaction();

            // Assert
            transaction1.TransactionId.Should().NotBe(transaction2.TransactionId);
            Guid.TryParse(transaction1.TransactionId, out _).Should().BeTrue();
        }

        [Fact]
        public void FromCardData_WithNullCardData_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => CardTransaction.FromCardData(null, "TestReader");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("cardData");
        }

        [Fact]
        public void FromCardData_WithValidData_ShouldMapAllFields()
        {
            // Arrange
            var cardData = new EmvDataParser.EmvCardData
            {
                PAN = "4111111111111111",
                ExpiryDate = "2025-12",
                CardholderName = "JOHN DOE",
                IccCertificate = "9F46 Certificate Data",
                Track2Data = "4111111111111111D2512101"
            };

            // Act
            var transaction = CardTransaction.FromCardData(cardData, "ACS ACR122U");

            // Assert
            transaction.PAN.Should().Be("4111111111111111");
            transaction.ExpiryDate.Should().Be("2025-12");
            transaction.CardholderName.Should().Be("JOHN DOE");
            transaction.ICCCertificate.Should().Be("9F46 Certificate Data");
            transaction.Track2Data.Should().Be("4111111111111111D2512101");
            transaction.ReaderName.Should().Be("ACS ACR122U");
            transaction.Status.Should().Be("Success");
            transaction.TransactionType.Should().Be("Read");
        }

        [Fact]
        public void FromCardData_WithNullFields_ShouldUseEmptyStrings()
        {
            // Arrange
            var cardData = new EmvDataParser.EmvCardData
            {
                PAN = null,
                ExpiryDate = null,
                CardholderName = null
            };

            // Act
            var transaction = CardTransaction.FromCardData(cardData, null);

            // Assert
            transaction.PAN.Should().BeEmpty();
            transaction.ExpiryDate.Should().BeEmpty();
            transaction.CardholderName.Should().BeEmpty();
            transaction.ReaderName.Should().BeEmpty();
        }

        [Fact]
        public void CreateFailed_ShouldCreateFailedTransaction()
        {
            // Arrange
            var readerName = "ACS ACR122U";
            var errorMessage = "Card not responding";

            // Act
            var transaction = CardTransaction.CreateFailed(readerName, errorMessage);

            // Assert
            transaction.Status.Should().Be("Failed");
            transaction.TransactionType.Should().Be("Read");
            transaction.ErrorMessage.Should().Be("Card not responding");
            transaction.ReaderName.Should().Be("ACS ACR122U");
        }

        [Fact]
        public void CreateFailed_WithNullParameters_ShouldUseEmptyStrings()
        {
            // Act
            var transaction = CardTransaction.CreateFailed(null, null);

            // Assert
            transaction.ReaderName.Should().BeEmpty();
            transaction.ErrorMessage.Should().BeEmpty();
            transaction.Status.Should().Be("Failed");
        }

        [Fact]
        public void GetSummary_ShouldReturnFormattedString()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "4111111111111111",
                Status = "Success",
                Timestamp = new DateTime(2026, 1, 1, 10, 30, 0)
            };

            // Act
            var summary = transaction.GetSummary();

            // Assert
            summary.Should().Contain("2026-01-01 10:30:00");
            summary.Should().Contain("Success");
            summary.Should().Contain("4111111111111111"); // Full PAN, not masked
        }

        [Fact]
        public void GetSummary_WithShortPAN_ShouldNotMask()
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = "123456789", // Less than 10 digits
                Status = "Success"
            };

            // Act
            var summary = transaction.GetSummary();

            // Assert
            summary.Should().Contain("123456789"); // Not masked
        }

        [Theory]
        [InlineData("4111111111111111", "4111111111111111")]
        [InlineData("5500000000000004", "5500000000000004")]
        [InlineData("371449635398431", "371449635398431")]
        public void GetSummary_ShouldMaskPANCorrectly(string pan, string expectedPAN)
        {
            // Arrange
            var transaction = new CardTransaction
            {
                PAN = pan,
                Status = "Success"
            };

            // Act
            var summary = transaction.GetSummary();

            // Assert
            summary.Should().Contain(expectedPAN);
        }

        [Fact]
        public void ProcessingTimeMs_ShouldBeSettable()
        {
            // Arrange
            var transaction = new CardTransaction();

            // Act
            transaction.ProcessingTimeMs = 1234;

            // Assert
            transaction.ProcessingTimeMs.Should().Be(1234);
        }

        [Fact]
        public void AllProperties_ShouldBeSettable()
        {
            // Arrange & Act
            var transaction = new CardTransaction
            {
                TransactionId = "test-id",
                Timestamp = new DateTime(2026, 1, 1),
                PAN = "4111111111111111",
                ExpiryDate = "2025-12",
                CardholderName = "JOHN DOE",
                ICCCertificate = "Cert Data",
                Track2Data = "Track2",
                ReaderName = "Reader1",
                TransactionType = "Verify",
                Status = "Success",
                ErrorMessage = "No error",
                ProcessingTimeMs = 500,
                Notes = "Test notes",
                UserName = "testuser",
                MachineName = "testmachine",
                ApplicationVersion = "1.0.0"
            };

            // Assert
            transaction.TransactionId.Should().Be("test-id");
            transaction.Timestamp.Should().Be(new DateTime(2026, 1, 1));
            transaction.PAN.Should().Be("4111111111111111");
            transaction.ExpiryDate.Should().Be("2025-12");
            transaction.CardholderName.Should().Be("JOHN DOE");
            transaction.ICCCertificate.Should().Be("Cert Data");
            transaction.Track2Data.Should().Be("Track2");
            transaction.ReaderName.Should().Be("Reader1");
            transaction.TransactionType.Should().Be("Verify");
            transaction.Status.Should().Be("Success");
            transaction.ErrorMessage.Should().Be("No error");
            transaction.ProcessingTimeMs.Should().Be(500);
            transaction.Notes.Should().Be("Test notes");
            transaction.UserName.Should().Be("testuser");
            transaction.MachineName.Should().Be("testmachine");
            transaction.ApplicationVersion.Should().Be("1.0.0");
        }
    }
}
