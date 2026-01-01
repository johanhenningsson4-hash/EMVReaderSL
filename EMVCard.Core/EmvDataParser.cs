using NfcReaderLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EMVCard
{
    /// <summary>
    /// Handles TLV (Tag-Length-Value) parsing for EMV data.
    /// </summary>
    public class EmvDataParser
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvDataParser");

        public event EventHandler<string> LogMessage;

        /// <summary>
        /// Represents parsed EMV card data.
        /// </summary>
        public class EmvCardData
        {
            public string PAN { get; set; }
            public string ExpiryDate { get; set; }
            public string CardholderName { get; set; }
            public string Track2Data { get; set; }
            public string IccCertificate { get; set; }
            public string IccExponent { get; set; }
            public string IccRemainder { get; set; }
            public List<string> AllTags { get; set; } = new List<string>();

            public void Clear()
            {
                PAN = null;
                ExpiryDate = null;
                CardholderName = null;
                Track2Data = null;
                IccCertificate = null;
                IccExponent = null;
                IccRemainder = null;
                AllTags.Clear();
            }
        }

        /// <summary>
        /// Parse TLV-encoded data and extract EMV tags.
        /// </summary>
        /// <param name="buffer">Buffer containing TLV data</param>
        /// <param name="startIndex">Start index in buffer</param>
        /// <param name="endIndex">End index in buffer</param>
        /// <param name="cardData">Card data object to populate</param>
        /// <param name="priority">Priority level (higher = overwrite existing)</param>
        /// <returns>Track2 data if found</returns>
        public string ParseTLV(byte[] buffer, int startIndex, int endIndex, EmvCardData cardData, int priority = 0)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                $"ParseTLV: Parsing {endIndex - startIndex} bytes, priority={priority}");

            string track2Data = null;
            int index = startIndex;

            while (index < endIndex && index < buffer.Length)
            {
                // Parse Tag
                if (index >= buffer.Length)
                    break;

                byte tag = buffer[index++];
                byte? tag2 = null;

                // Two-byte tag
                if ((tag & 0x1F) == 0x1F)
                {
                    if (index >= buffer.Length)
                        break;
                    tag2 = buffer[index++];
                }

                int tagValue = tag2.HasValue ? (tag << 8 | tag2.Value) : tag;

                // Parse Length
                if (index >= buffer.Length)
                    break;

                int len = buffer[index++];
                if (len >= 0x80)
                {
                    int lenLen = (len & 0x7F);

                    if (lenLen <= 0 || lenLen > 3 || index + lenLen > buffer.Length)
                    {
                        OnLogMessage($"TLV length field abnormal: lenLen={lenLen}, index={index}");
                        break;
                    }

                    len = 0;
                    for (int i = 0; i < lenLen; i++)
                    {
                        len = (len << 8) + buffer[index++];
                    }
                }

                // Safety check
                if (len < 0 || len > 4096 || index + len > buffer.Length)
                {
                    OnLogMessage($"TLV length illegal: len={len}, index={index}");
                    break;
                }

                // Extract Value
                byte[] value = new byte[len];
                Array.Copy(buffer, index, value, 0, len);
                index += len;

                // Process tag
                ProcessTag(tagValue, value, cardData, priority);
            }

            return track2Data;
        }

        /// <summary>
        /// Process individual EMV tag.
        /// </summary>
        private void ProcessTag(int tagValue, byte[] value, EmvCardData cardData, int priority)
        {
            switch (tagValue)
            {
                case 0x5A: // PAN (Card Number)
                    if (priority > 0 || string.IsNullOrEmpty(cardData.PAN))
                    {
                        string pan = BitConverter.ToString(value).Replace("-", "").TrimEnd('F');
                        cardData.PAN = pan;
                        OnLogMessage($"Card Number (PAN): {pan}");
                    }
                    break;

                case 0x5F24: // Expiry Date
                    if (priority > 0 || string.IsNullOrEmpty(cardData.ExpiryDate))
                    {
                        string rawDate = BitConverter.ToString(value).Replace("-", "");
                        string expiry = "";

                        if (rawDate.Length >= 6)
                        {
                            expiry = $"20{rawDate.Substring(0, 2)}-{rawDate.Substring(2, 2)}-{rawDate.Substring(4, 2)}";
                        }
                        else if (rawDate.Length >= 4)
                        {
                            expiry = $"20{rawDate.Substring(0, 2)}-{rawDate.Substring(2, 2)}";
                        }

                        if (!string.IsNullOrEmpty(expiry))
                        {
                            cardData.ExpiryDate = expiry;
                            OnLogMessage($"Expiry Date: {expiry}");
                        }
                    }
                    break;

                case 0x5F20: // Cardholder Name
                    if (priority > 0 || string.IsNullOrEmpty(cardData.CardholderName))
                    {
                        string name = Encoding.ASCII.GetString(value).Trim();
                        cardData.CardholderName = name;
                        OnLogMessage($"Cardholder Name: {name}");
                    }
                    break;

                case 0x57: // Track2 Data
                    string track2 = BitConverter.ToString(value).Replace("-", "");
                    cardData.Track2Data = track2;
                    OnLogMessage($"Track2 Data: {track2}");
                    break;

                case 0x9F6B: // Track2 Equivalent Data
                    if (string.IsNullOrEmpty(cardData.Track2Data))
                    {
                        string track2Equiv = BitConverter.ToString(value).Replace("-", "");
                        cardData.Track2Data = track2Equiv;
                        OnLogMessage($"Track2 Equivalent Data: {track2Equiv}");
                    }
                    break;

                case 0x9F46: // ICC Public Key Certificate
                    string iccCert = BitConverter.ToString(value).Replace("-", " ");
                    if (string.IsNullOrEmpty(cardData.IccCertificate))
                    {
                        cardData.IccCertificate = iccCert;
                    }
                    else
                    {
                        cardData.IccCertificate += Environment.NewLine + "Cert: " + iccCert;
                    }
                    OnLogMessage($"ICC Public Key Certificate: {iccCert}");
                    break;

                case 0x9F47: // ICC Public Key Exponent
                    string iccExp = BitConverter.ToString(value).Replace("-", " ");
                    cardData.IccExponent = iccExp;
                    if (string.IsNullOrEmpty(cardData.IccCertificate))
                    {
                        cardData.IccCertificate = "Exp: " + iccExp;
                    }
                    else
                    {
                        cardData.IccCertificate += Environment.NewLine + "Exp: " + iccExp;
                    }
                    OnLogMessage($"ICC Public Key Exponent: {iccExp}");
                    break;

                case 0x9F48: // ICC Public Key Remainder
                    string iccRem = BitConverter.ToString(value).Replace("-", " ");
                    cardData.IccRemainder = iccRem;
                    if (string.IsNullOrEmpty(cardData.IccCertificate))
                    {
                        cardData.IccCertificate = "Rem: " + iccRem;
                    }
                    else
                    {
                        cardData.IccCertificate += Environment.NewLine + "Rem: " + iccRem;
                    }
                    OnLogMessage($"ICC Public Key Remainder: {iccRem}");
                    break;

                case 0x9F49: // DDOL
                    string ddol = BitConverter.ToString(value).Replace("-", " ");
                    OnLogMessage($"DDOL: {ddol}");
                    break;

                case 0x9F4A: // Static Data Authentication Tag List
                    string sdaTagList = BitConverter.ToString(value).Replace("-", " ");
                    OnLogMessage($"SDA Tag List: {sdaTagList}");
                    break;

                case 0x70: // Record Template
                case 0x77: // Response Message Template Format 2
                    ParseTLV(value, 0, value.Length, cardData, priority);
                    break;

                case 0x80: // Response Message Template Format 1
                    if (value.Length > 2)
                    {
                        ParseTLV(value, 2, value.Length, cardData, priority);
                    }
                    break;
            }
        }

        /// <summary>
        /// Parse AFL (Application File Locator) from GPO response.
        /// </summary>
        /// <param name="buffer">GPO response buffer</param>
        /// <param name="length">Buffer length</param>
        /// <returns>List of (SFI, startRecord, endRecord) tuples</returns>
        public List<(int sfi, int startRecord, int endRecord)> ParseAFL(byte[] buffer, long length)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "ParseAFL: Parsing AFL from GPO response");

            var aflList = new List<(int, int, int)>();

            if (buffer.Length == 0)
                return aflList;

            if (buffer[0] == 0x77) // GPO response template 77
            {
                int i = 0;
                while (i < length - 2)
                {
                    if (buffer[i] == 0x94)
                    {
                        int len = buffer[i + 1];
                        int pos = i + 2;
                        while (pos + 3 < i + 2 + len)
                        {
                            int sfi = buffer[pos] >> 3;
                            int start = buffer[pos + 1];
                            int end = buffer[pos + 2];
                            aflList.Add((sfi, start, end));
                            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                                $"ParseAFL: Found AFL entry - SFI={sfi}, Start={start}, End={end}");
                            pos += 4;
                        }
                        break;
                    }
                    i++;
                }
            }
            else if (buffer[0] == 0x80) // GPO response template 80 (Visa)
            {
                int totalLen = buffer[1];
                if (totalLen + 2 > buffer.Length)
                    return aflList;

                int pos = 2;
                pos += 2; // Skip AIP (2 bytes)

                while (pos + 3 < 2 + totalLen)
                {
                    int sfi = buffer[pos] >> 3;
                    int start = buffer[pos + 1];
                    int end = buffer[pos + 2];
                    if (sfi >= 1 && sfi <= 31 && start >= 1 && end >= start)
                    {
                        aflList.Add((sfi, start, end));
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                            $"ParseAFL: Found AFL entry - SFI={sfi}, Start={start}, End={end}");
                    }
                    pos += 4;
                }
            }

            _traceSource.TraceEvent(TraceEventType.Information, 0, $"ParseAFL: Found {aflList.Count} AFL entries");
            return aflList;
        }

        /// <summary>
        /// Extract missing card info from Track2 data.
        /// </summary>
        /// <param name="cardData">Card data to update</param>
        public void ExtractFromTrack2(EmvCardData cardData)
        {
            if (string.IsNullOrEmpty(cardData.Track2Data))
                return;

            _traceSource.TraceEvent(TraceEventType.Information, 0, "ExtractFromTrack2: Extracting missing data from Track2");

            string track2 = cardData.Track2Data;
            int dIndex = track2.IndexOf("D");

            if (dIndex <= 0)
                dIndex = track2.IndexOf("=");

            if (dIndex > 0 && track2.Length >= dIndex + 5)
            {
                // Extract PAN
                if (string.IsNullOrEmpty(cardData.PAN))
                {
                    string pan = track2.Substring(0, dIndex).TrimEnd('F');
                    cardData.PAN = pan;
                    OnLogMessage($"Extracted card number from Track2: {pan}");
                }

                // Extract expiry date
                if (string.IsNullOrEmpty(cardData.ExpiryDate) && track2.Length >= dIndex + 5)
                {
                    string expiryYYMM = track2.Substring(dIndex + 1, 4);
                    if (System.Text.RegularExpressions.Regex.IsMatch(expiryYYMM, @"^\d{4}$"))
                    {
                        string expiry = $"20{expiryYYMM.Substring(0, 2)}-{expiryYYMM.Substring(2)}";
                        cardData.ExpiryDate = expiry;
                        OnLogMessage($"Extracted expiry date from Track2: {expiry}");
                    }
                }
            }
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
