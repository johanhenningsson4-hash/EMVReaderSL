using System;
using System.Collections.Generic;
using EMVCard;

namespace EMVCard.Tests
{
    /// <summary>
    /// Helper class for creating test data
    /// </summary>
    public static class TestDataHelper
    {
        /// <summary>
        /// Creates a valid test card transaction
        /// </summary>
        public static CardTransaction CreateTestTransaction(string pan = "4111111111111111")
        {
            return new CardTransaction
            {
                PAN = pan,
                ExpiryDate = "2025-12",
                CardholderName = "JOHN DOE",
                ICCCertificate = "9F46 Test Certificate",
                Track2Data = $"{pan}D2512101",
                ReaderName = "ACS ACR122U PICC Interface",
                TransactionType = "Read",
                Status = "Success",
                ProcessingTimeMs = 1000,
                Notes = "Test transaction"
            };
        }

        /// <summary>
        /// Creates a failed transaction
        /// </summary>
        public static CardTransaction CreateFailedTransaction(string errorMessage = "Test error")
        {
            return CardTransaction.CreateFailed("ACS ACR122U", errorMessage);
        }

        /// <summary>
        /// Creates test EMV card data
        /// </summary>
        public static EmvDataParser.EmvCardData CreateTestCardData()
        {
            return new EmvDataParser.EmvCardData
            {
                PAN = "4111111111111111",
                ExpiryDate = "2025-12",
                CardholderName = "JOHN DOE",
                Track2Data = "4111111111111111D2512101",
                IccCertificate = "9F46 Certificate Data\r\nExp: 01 00 01\r\nRem: Remainder",
                IccExponent = "01 00 01",
                IccRemainder = "Remainder Data"
            };
        }

        /// <summary>
        /// Creates multiple test transactions with different dates
        /// </summary>
        public static List<CardTransaction> CreateTransactionsWithDates(int count, DateTime baseDate)
        {
            var transactions = new List<CardTransaction>();

            for (int i = 0; i < count; i++)
            {
                var transaction = CreateTestTransaction($"411111111111111{i}");
                transaction.Timestamp = baseDate.AddDays(-i);
                transactions.Add(transaction);
            }

            return transactions;
        }

        /// <summary>
        /// Creates transactions for different card types
        /// </summary>
        public static Dictionary<string, CardTransaction> CreateMultiCardTypeTransactions()
        {
            return new Dictionary<string, CardTransaction>
            {
                ["Visa"] = CreateTestTransaction("4111111111111111"),
                ["Mastercard"] = CreateTestTransaction("5500000000000004"),
                ["AmEx"] = CreateTestTransaction("371449635398431"),
                ["Discover"] = CreateTestTransaction("6011111111111117")
            };
        }

        /// <summary>
        /// Creates transactions with various statuses
        /// </summary>
        public static List<CardTransaction> CreateTransactionsWithStatuses()
        {
            return new List<CardTransaction>
            {
                new CardTransaction { PAN = "4111111111111111", Status = "Success" },
                new CardTransaction { PAN = "5500000000000004", Status = "Failed", ErrorMessage = "Timeout" },
                new CardTransaction { PAN = "371449635398431", Status = "Success" },
                new CardTransaction { PAN = "6011111111111117", Status = "Failed", ErrorMessage = "Card removed" }
            };
        }

        /// <summary>
        /// Masks a PAN for testing
        /// </summary>
        public static string MaskPAN(string pan)
        {
            if (string.IsNullOrEmpty(pan) || pan.Length < 10)
                return pan;

            return pan.Substring(0, 6) + new string('*', pan.Length - 10) + pan.Substring(pan.Length - 4);
        }
    }
}
