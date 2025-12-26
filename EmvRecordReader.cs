using NfcReaderLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EMVCard
{
    /// <summary>
    /// Handles reading EMV records from the card using AFL.
    /// </summary>
    public class EmvRecordReader
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvRecordReader");

        private readonly EmvCardReader _cardReader;
        private readonly EmvDataParser _dataParser;

        public event EventHandler<string> LogMessage;

        public EmvRecordReader(EmvCardReader cardReader, EmvDataParser dataParser)
        {
            _cardReader = cardReader ?? throw new ArgumentNullException(nameof(cardReader));
            _dataParser = dataParser ?? throw new ArgumentNullException(nameof(dataParser));
        }

        /// <summary>
        /// Read all records specified in AFL list.
        /// </summary>
        /// <param name="aflList">List of AFL entries</param>
        /// <param name="cardData">Card data object to populate</param>
        /// <returns>True if at least one record was read successfully</returns>
        public bool ReadAFLRecords(List<(int sfi, int start, int end)> aflList, EmvDataParser.EmvCardData cardData)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"ReadAFLRecords: Reading {aflList.Count} AFL entries");

            bool anySuccess = false;

            foreach (var (sfi, start, end) in aflList)
            {
                for (int rec = start; rec <= end; rec++)
                {
                    if (ReadRecord(sfi, rec, cardData))
                    {
                        anySuccess = true;
                    }
                }
            }

            return anySuccess;
        }

        /// <summary>
        /// Read a specific record from the card.
        /// </summary>
        /// <param name="sfi">Short File Identifier</param>
        /// <param name="record">Record number</param>
        /// <param name="cardData">Card data object to populate</param>
        /// <returns>True if record read successfully</returns>
        public bool ReadRecord(int sfi, int record, EmvDataParser.EmvCardData cardData)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ReadRecord: SFI={sfi}, Record={record}");

            // Build READ RECORD command
            byte p2 = (byte)((sfi << 3) | 0x04);
            byte[] apdu = new byte[] { 0x00, 0xB2, (byte)record, p2, 0x00 };

            if (!_cardReader.SendApduWithAutoFix(apdu, out byte[] response))
            {
                OnLogMessage($"SFI {sfi} Record {record} transmission failed");
                return false;
            }

            // Check status words
            if (response.Length < 2 || response[response.Length - 2] != 0x90 || response[response.Length - 1] != 0x00)
            {
                OnLogMessage($"SFI {sfi} Record {record} did not return 90 00, skipping parse");
                return false;
            }

            OnLogMessage($"Successfully read SFI {sfi} Record {record}");

            // Parse the record content
            ParseRecordContent(response, response.Length - 2, cardData);
            return true;
        }

        /// <summary>
        /// Try reading common SFI/Record combinations when AFL is not available.
        /// </summary>
        /// <param name="cardData">Card data object to populate</param>
        /// <returns>True if at least one record was read</returns>
        public bool TryReadCommonRecords(EmvDataParser.EmvCardData cardData)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "TryReadCommonRecords: Attempting to read common records");

            int[][] commonRecords = new int[][]
            {
                new int[] { 1, 1 },  // SFI 1, Record 1
                new int[] { 2, 1 },  // SFI 2, Record 1
                new int[] { 3, 1 },  // SFI 3, Record 1
                new int[] { 4, 1 },  // SFI 4, Record 1
                new int[] { 1, 2 },  // SFI 1, Record 2
                new int[] { 2, 2 }   // SFI 2, Record 2
            };

            bool anySuccess = false;

            foreach (var record in commonRecords)
            {
                int sfi = record[0];
                int rec = record[1];

                if (ReadRecord(sfi, rec, cardData))
                {
                    anySuccess = true;
                }
            }

            return anySuccess;
        }

        /// <summary>
        /// Parse record content (handles template format).
        /// </summary>
        private void ParseRecordContent(byte[] buffer, int len, EmvDataParser.EmvCardData cardData)
        {
            // Check if template format (starts with 70)
            if (buffer[0] == 0x70)
            {
                int templateLen = buffer[1];
                int startPos = 2;

                // Handle long format length
                if (buffer[1] > 0x80)
                {
                    int lenBytes = buffer[1] & 0x7F;
                    templateLen = 0;
                    for (int i = 0; i < lenBytes; i++)
                    {
                        templateLen = (templateLen << 8) | buffer[2 + i];
                    }
                    startPos = 2 + lenBytes;
                }
            }

            // Parse TLV with high priority (1)
            _dataParser.ParseTLV(buffer, 0, len, cardData, 1);
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
