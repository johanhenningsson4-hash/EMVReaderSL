using System;

namespace EMVCard
{
    /// <summary>
    /// Represents a complete EMV card transaction with all relevant data.
    /// Compatible with .NET Framework 4.7.2
    /// </summary>
    public class CardTransaction
    {
        /// <summary>
        /// Unique transaction identifier
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Timestamp when transaction was recorded
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Card Primary Account Number (may be masked)
        /// </summary>
        public string PAN { get; set; }

        /// <summary>
        /// Card expiration date (YYYY-MM-DD format from EMV)
        /// </summary>
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Cardholder name from card
        /// </summary>
        public string CardholderName { get; set; }

        /// <summary>
        /// ICC Public Key Certificate (formatted with spaces)
        /// </summary>
        public string ICCCertificate { get; set; }

        /// <summary>
        /// Track 2 Equivalent Data (hex string)
        /// </summary>
        public string Track2Data { get; set; }

        /// <summary>
        /// Card reader name used for transaction
        /// </summary>
        public string ReaderName { get; set; }

        /// <summary>
        /// Transaction type (Read, Verify, etc.)
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Transaction status (Success, Failed, etc.)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Error message if transaction failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Processing time in milliseconds
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// Additional notes or comments
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// User who performed the transaction
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Machine/Computer name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Application version
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Creates a new transaction with default values
        /// </summary>
        public CardTransaction()
        {
            TransactionId = Guid.NewGuid().ToString();
            Timestamp = DateTime.Now;
            TransactionType = "Read";
            Status = "Unknown";
            UserName = Environment.UserName;
            MachineName = Environment.MachineName;
            ApplicationVersion = "2.0.0";
        }

        /// <summary>
        /// Creates a transaction from EMV card data
        /// </summary>
        public static CardTransaction FromCardData(EmvDataParser.EmvCardData cardData, string readerName)
        {
            if (cardData == null)
                throw new ArgumentNullException(nameof(cardData));

            return new CardTransaction
            {
                PAN = cardData.PAN ?? string.Empty,
                ExpiryDate = cardData.ExpiryDate ?? string.Empty,
                CardholderName = cardData.CardholderName ?? string.Empty,
                ICCCertificate = cardData.IccCertificate ?? string.Empty,
                Track2Data = cardData.Track2Data ?? string.Empty,
                ReaderName = readerName ?? string.Empty,
                TransactionType = "Read",
                Status = "Success"
            };
        }

        /// <summary>
        /// Creates a failed transaction record
        /// </summary>
        public static CardTransaction CreateFailed(string readerName, string errorMessage)
        {
            return new CardTransaction
            {
                ReaderName = readerName ?? string.Empty,
                TransactionType = "Read",
                Status = "Failed",
                ErrorMessage = errorMessage ?? string.Empty
            };
        }

        /// <summary>
        /// Gets a display-friendly summary of the transaction
        /// </summary>
        public string GetSummary()
        {
            // Do not mask PAN: always show full PAN
            string timestamp = Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            return string.Format("{0} - {1} - PAN: {2}", timestamp, Status, PAN);
        }

        /// <summary>
        /// Masks the PAN for display/logging
        /// </summary>
        private string MaskPAN(string pan)
        {
            // Masking disabled: always return full PAN
            return pan;
        }
    }
}
