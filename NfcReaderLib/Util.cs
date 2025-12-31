using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NfcReaderLib
{
    /// <summary>
    /// Utility class providing helper methods for hex conversion, cryptography, and data manipulation.
    /// Includes comprehensive logging for debugging and monitoring.
    /// </summary>
    public static class Util
    {
        private static readonly TraceSource _traceSource = new TraceSource("NfcReaderLib.Util");

        /// <summary>
        /// Generates a string containing the specified number of spaces.
        /// </summary>
        /// <param name="length">Number of spaces to generate</param>
        /// <returns>String with specified number of spaces</returns>
        public static string GetSpaces(int length)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"GetSpaces called with length={length}");
            
            if (length < 0)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, $"GetSpaces: Negative length {length} provided, returning empty string");
                return string.Empty;
            }

            string result = new string(' ', length);
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"GetSpaces: Generated {length} spaces");
            return result;
        }

        /// <summary>
        /// Converts a hexadecimal string to ASCII text.
        /// </summary>
        /// <param name="hexStr">Hexadecimal string to convert</param>
        /// <returns>ASCII representation of the hex string</returns>
        public static string HexToAscii(string hexStr)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"HexToAscii called with input length={hexStr?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(hexStr))
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "HexToAscii: Null or empty input, returning empty string");
                return string.Empty;
            }

            if (hexStr.Length % 2 != 0)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"HexToAscii: Invalid hex string length {hexStr.Length} (must be even)");
                throw new ArgumentException($"Hex string must have even length, got {hexStr.Length}", nameof(hexStr));
            }

            try
            {
                var output = new StringBuilder(hexStr.Length / 2);
                for (int i = 0; i < hexStr.Length; i += 2)
                {
                    string str = hexStr.Substring(i, 2);
                    output.Append((char)Convert.ToInt32(str, 16));
                }
                
                string result = output.ToString();
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"HexToAscii: Successfully converted {hexStr.Length} hex chars to {result.Length} ASCII chars");
                return result;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"HexToAscii: Conversion failed - {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Formats byte array as space-separated hex string for display.
        /// </summary>
        /// <param name="data">Byte array to format</param>
        /// <returns>Space-separated hex string</returns>
        public static string PrettyPrintHex(byte[] data)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"PrettyPrintHex called with data length={data?.Length ?? 0}");
            
            if (data == null || data.Length == 0)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "PrettyPrintHex: Null or empty data, returning empty string");
                return string.Empty;
            }

            string result = BitConverter.ToString(data).Replace("-", " ");
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"PrettyPrintHex: Formatted {data.Length} bytes to hex string");
            return result;
        }

        /// <summary>
        /// Converts byte array to continuous hex string (no separators).
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <returns>Continuous hex string</returns>
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ByteArrayToHexString called with array length={byteArray?.Length ?? 0}");
            
            if (byteArray == null)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "ByteArrayToHexString: Null array, returning empty string");
                return "";
            }

            if (byteArray.Length == 0)
            {
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ByteArrayToHexString: Empty array, returning empty string");
                return "";
            }

            string result = BitConverter.ToString(byteArray).Replace("-", "");
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ByteArrayToHexString: Converted {byteArray.Length} bytes to {result.Length} hex characters");
            return result;
        }

        /// <summary>
        /// Converts hex string to byte array. Spaces are ignored.
        /// </summary>
        /// <param name="hex">Hex string to convert</param>
        /// <returns>Byte array</returns>
        /// <exception cref="ArgumentException">If hex string has odd length after removing spaces</exception>
        public static byte[] FromHexString(string hex)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"FromHexString called with input: '{hex?.Substring(0, Math.Min(hex?.Length ?? 0, 50)) ?? "null"}'");
            
            if (hex == null)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "FromHexString: Null input, returning empty array");
                return new byte[0];
            }

            hex = hex.Replace(" ", "");
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"FromHexString: After removing spaces, length={hex.Length}");
            
            if (hex.Length == 0)
            {
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, "FromHexString: Empty string after space removal, returning empty array");
                return new byte[0];
            }

            if (hex.Length % 2 != 0)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"FromHexString: Invalid hex string length {hex.Length} (must be even)");
                throw new ArgumentException("Input string must contain an even number of characters: " + hex);
            }

            try
            {
                byte[] result = new byte[hex.Length / 2];
                for (int i = 0; i < hex.Length; i += 2)
                {
                    result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                
                _traceSource.TraceEvent(TraceEventType.Information, 0, $"FromHexString: Successfully converted {hex.Length} hex chars to {result.Length} bytes");
                return result;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"FromHexString: Conversion failed - {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Converts a single byte to integer (0-255).
        /// </summary>
        /// <param name="b">Byte to convert</param>
        /// <returns>Integer value (0-255)</returns>
        public static int ByteToInt(byte b)
        {
            int result = b & 0xFF;
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ByteToInt: byte 0x{b:X2} -> int {result}");
            return result;
        }

        /// <summary>
        /// Converts two bytes to integer (big-endian).
        /// </summary>
        /// <param name="first">First (high) byte</param>
        /// <param name="second">Second (low) byte</param>
        /// <returns>Combined integer value</returns>
        public static int ByteToInt(byte first, byte second)
        {
            int result = ((first & 0xFF) << 8) + (second & 0xFF);
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ByteToInt: bytes [0x{first:X2}, 0x{second:X2}] -> int {result}");
            return result;
        }

        /// <summary>
        /// Converts two bytes to short (big-endian).
        /// </summary>
        /// <param name="b1">First (high) byte</param>
        /// <param name="b2">Second (low) byte</param>
        /// <returns>Short value</returns>
        public static short Byte2Short(byte b1, byte b2)
        {
            short result = (short)((b1 << 8) | (b2 & 0xFF));
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"Byte2Short: bytes [0x{b1:X2}, 0x{b2:X2}] -> short {result}");
            return result;
        }

        /// <summary>
        /// Calculates SHA-1 hash of the provided data.
        /// </summary>
        /// <param name="data">Data to hash</param>
        /// <returns>SHA-1 hash (20 bytes)</returns>
        public static byte[] CalculateSHA1(byte[] data)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"CalculateSHA1 called with data length={data?.Length ?? 0}");
            
            if (data == null)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, "CalculateSHA1: Null data provided");
                throw new ArgumentNullException(nameof(data), "Data cannot be null");
            }

            if (data.Length == 0)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "CalculateSHA1: Empty data array provided");
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                using (var sha1 = SHA1.Create())
                {
                    byte[] hash = sha1.ComputeHash(data);
                    stopwatch.Stop();
                    
                    _traceSource.TraceEvent(TraceEventType.Information, 0, 
                        $"CalculateSHA1: Hashed {data.Length} bytes in {stopwatch.ElapsedMilliseconds}ms, hash={ByteArrayToHexString(hash).Substring(0, 16)}...");
                    
                    return hash;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"CalculateSHA1: Failed after {stopwatch.ElapsedMilliseconds}ms - {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Masks a PAN (Primary Account Number) for display, showing only first and last digits.
        /// </summary>
        /// <param name="PAN">Card number to mask</param>
        /// <returns>Masked card number</returns>
        public static string MaskPAN(string PAN)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"MaskPAN called with PAN length={PAN?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(PAN))
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "MaskPAN: Null or empty PAN, returning empty string");
                return "";
            }

            string originalPAN = PAN;
            PAN = PAN.Replace("F", "");
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"MaskPAN: After removing 'F', length={PAN.Length}");

            string maskedPAN = null;
            switch (PAN.Length)
            {
                case 12: 
                    maskedPAN = MaskCardNumber(PAN, "xxxx-xxxx-####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 12-digit mask");
                    break;
                case 13: 
                    maskedPAN = MaskCardNumber(PAN, "xxxxx-xxxx-####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 13-digit mask");
                    break;
                case 14: 
                    maskedPAN = MaskCardNumber(PAN, "xxxx-xxx-xxx-####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 14-digit mask");
                    break;
                case 15: 
                    maskedPAN = MaskCardNumber(PAN, "####-xxxxxx-x####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 15-digit mask (Amex)");
                    break;
                case 16: 
                    maskedPAN = MaskCardNumber(PAN, "####-##xx-xxxx-####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 16-digit mask");
                    break;
                case 17: 
                    maskedPAN = MaskCardNumber(PAN, "#####-#xxx-xxxx-####");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 17-digit mask");
                    break;
                case 18: 
                    maskedPAN = MaskCardNumber(PAN, "####-##xx-xxxx-x##-###");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 18-digit mask");
                    break;
                case 19: 
                    maskedPAN = MaskCardNumber(PAN, "####-##xx-xxxx-xx##-###");
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0, "MaskPAN: Applied 19-digit mask");
                    break;
                default:
                    maskedPAN = new string('X', Math.Max(0, PAN.Length - 4)) + PAN.Substring(Math.Max(0, PAN.Length - 4));
                    _traceSource.TraceEvent(TraceEventType.Warning, 0, $"MaskPAN: Unusual PAN length {PAN.Length}, applied default mask");
                    break;
            }

            _traceSource.TraceEvent(TraceEventType.Information, 0, $"MaskPAN: Successfully masked PAN (length {PAN.Length})");
            return maskedPAN;
        }

        /// <summary>
        /// Applies a mask pattern to a card number.
        /// </summary>
        /// <param name="cardNumber">Card number to mask</param>
        /// <param name="mask">Mask pattern (# = show digit, x = hide with 'x', other = literal)</param>
        /// <returns>Masked card number</returns>
        public static string MaskCardNumber(string cardNumber, string mask)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"MaskCardNumber called: cardLength={cardNumber?.Length ?? 0}, mask='{mask}'");
            
            if (string.IsNullOrEmpty(cardNumber))
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "MaskCardNumber: Empty card number");
                return string.Empty;
            }

            if (string.IsNullOrEmpty(mask))
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0, "MaskCardNumber: Empty mask, returning original");
                return cardNumber;
            }

            int index = 0;
            var maskedNumber = new StringBuilder();
            for (int i = 0; i < mask.Length && index < cardNumber.Length; i++)
            {
                char c = mask[i];
                if (c == '#')
                {
                    maskedNumber.Append(cardNumber[index]);
                    index++;
                }
                else if (c == 'x')
                {
                    maskedNumber.Append('x');
                    index++;
                }
                else
                {
                    maskedNumber.Append(c);
                }
            }

            string result = maskedNumber.ToString();
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"MaskCardNumber: Processed {index} digits, result length={result.Length}");
            return result;
        }

        /// <summary>
        /// Checks if a byte array is not null and not empty.
        /// </summary>
        /// <param name="bytearray">Byte array to check</param>
        /// <returns>True if array has content, false otherwise</returns>
        public static bool NotEmpty(byte[] bytearray)
        {
            bool result = bytearray != null && bytearray.Length > 0;
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"NotEmpty: array is {(result ? "not empty" : "empty or null")} (length={bytearray?.Length ?? 0})");
            return result;
        }

        /// <summary>
        /// Represents the issuer public key data extracted from EMV tags.
        /// </summary>
        public class IssuerPublicKeyData
        {
            /// <summary>
            /// Issuer Public Key Certificate (Tag 90)
            /// </summary>
            public byte[] Certificate { get; set; }

            /// <summary>
            /// Issuer Public Key Remainder (Tag 92)
            /// </summary>
            public byte[] Remainder { get; set; }

            /// <summary>
            /// Issuer Public Key Exponent (Tag 9F32)
            /// </summary>
            public byte[] Exponent { get; set; }

            /// <summary>
            /// Indicates if all required components were found
            /// </summary>
            public bool IsComplete => Certificate != null && Exponent != null;

            public override string ToString()
            {
                return $"IssuerPublicKey[Cert={Certificate?.Length ?? 0}B, Rem={Remainder?.Length ?? 0}B, Exp={Exponent?.Length ?? 0}B, Complete={IsComplete}]";
            }
        }

        /// <summary>
        /// Extracts issuer public key components from TLV-encoded EMV data.
        /// Searches for tags: 90 (Certificate), 92 (Remainder), 9F32 (Exponent)
        /// </summary>
        /// <param name="tlvData">TLV-encoded byte array containing EMV tags</param>
        /// <returns>IssuerPublicKeyData containing extracted components, or null if parsing fails</returns>
        public static IssuerPublicKeyData ExtractIssuerPublicKeyTags(byte[] tlvData)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0,
                $"ExtractIssuerPublicKeyTags: Starting extraction from {tlvData?.Length ?? 0} bytes");

            if (tlvData == null || tlvData.Length == 0)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ExtractIssuerPublicKeyTags: Null or empty TLV data");
                return null;
            }

            try
            {
                var result = new IssuerPublicKeyData();
                int index = 0;

                while (index < tlvData.Length)
                {
                    // Parse tag
                    if (index >= tlvData.Length)
                        break;

                    byte firstTagByte = tlvData[index++];
                    int tag = firstTagByte;

                    // Check for two-byte tag (EMV standard: if lower 5 bits of first byte = 0x1F)
                    if ((firstTagByte & 0x1F) == 0x1F)
                    {
                        if (index >= tlvData.Length)
                        {
                            _traceSource.TraceEvent(TraceEventType.Warning, 0,
                                "ExtractIssuerPublicKeyTags: Incomplete two-byte tag at end of data");
                            break;
                        }
                        byte secondTagByte = tlvData[index++];
                        tag = (firstTagByte << 8) | secondTagByte;

                        // Handle three-byte tags (bit 7 of second byte indicates continuation)
                        while ((secondTagByte & 0x80) != 0 && index < tlvData.Length)
                        {
                            byte nextByte = tlvData[index++];
                            tag = (tag << 8) | nextByte;
                            secondTagByte = nextByte;
                        }
                    }

                    // Parse length
                    if (index >= tlvData.Length)
                    {
                        _traceSource.TraceEvent(TraceEventType.Warning, 0,
                            $"ExtractIssuerPublicKeyTags: Missing length byte for tag 0x{tag:X}");
                        break;
                    }

                    int length = tlvData[index++];

                    // Handle BER-TLV length encoding (EMV uses BER-TLV)
                    if (length > 0x80)
                    {
                        int numLengthBytes = length & 0x7F;
                        if (numLengthBytes > 3 || index + numLengthBytes > tlvData.Length)
                        {
                            _traceSource.TraceEvent(TraceEventType.Warning, 0,
                                $"ExtractIssuerPublicKeyTags: Invalid length encoding for tag 0x{tag:X}");
                            break;
                        }

                        length = 0;
                        for (int i = 0; i < numLengthBytes; i++)
                        {
                            length = (length << 8) | tlvData[index++];
                        }
                    }

                    // Validate length
                    if (length < 0 || index + length > tlvData.Length)
                    {
                        _traceSource.TraceEvent(TraceEventType.Warning, 0,
                            $"ExtractIssuerPublicKeyTags: Invalid length {length} for tag 0x{tag:X} at index {index}");
                        break;
                    }

                    // Extract value
                    byte[] value = new byte[length];
                    Array.Copy(tlvData, index, value, 0, length);
                    index += length;

                    // Check for issuer public key tags
                    switch (tag)
                    {
                        case 0x90: // Issuer Public Key Certificate
                            result.Certificate = value;
                            _traceSource.TraceEvent(TraceEventType.Information, 0,
                                $"ExtractIssuerPublicKeyTags: Found Issuer Public Key Certificate (tag 90), length={length}");
                            break;

                        case 0x92: // Issuer Public Key Remainder
                            result.Remainder = value;
                            _traceSource.TraceEvent(TraceEventType.Information, 0,
                                $"ExtractIssuerPublicKeyTags: Found Issuer Public Key Remainder (tag 92), length={length}");
                            break;

                        case 0x9F32: // Issuer Public Key Exponent
                            result.Exponent = value;
                            _traceSource.TraceEvent(TraceEventType.Information, 0,
                                $"ExtractIssuerPublicKeyTags: Found Issuer Public Key Exponent (tag 9F32), length={length}");
                            break;

                        case 0x70: // Data template (recursive parse)
                        case 0x77: // Response Message Template Format 2
                            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                                $"ExtractIssuerPublicKeyTags: Recursively parsing template tag 0x{tag:X}");
                            var nestedResult = ExtractIssuerPublicKeyTags(value);
                            if (nestedResult != null)
                            {
                                // Merge results from nested parsing
                                if (nestedResult.Certificate != null && result.Certificate == null)
                                    result.Certificate = nestedResult.Certificate;
                                if (nestedResult.Remainder != null && result.Remainder == null)
                                    result.Remainder = nestedResult.Remainder;
                                if (nestedResult.Exponent != null && result.Exponent == null)
                                    result.Exponent = nestedResult.Exponent;
                            }
                            break;

                        default:
                            // Log other tags at verbose level
                            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                                $"ExtractIssuerPublicKeyTags: Skipping tag 0x{tag:X}, length={length}");
                            break;
                    }
                }

                // Log extraction results
                _traceSource.TraceEvent(TraceEventType.Information, 0,
                    $"ExtractIssuerPublicKeyTags: Extraction complete - {result}");

                if (!result.IsComplete)
                {
                    _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        "ExtractIssuerPublicKeyTags: Incomplete issuer public key data - missing Certificate or Exponent");
                }

                return result;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"ExtractIssuerPublicKeyTags: Exception - {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extracts issuer public key components from hex string.
        /// Convenience method that converts hex string to bytes then extracts tags.
        /// </summary>
        /// <param name="tlvHexString">Hex string containing TLV-encoded EMV data (spaces allowed)</param>
        /// <returns>IssuerPublicKeyData containing extracted components, or null if parsing fails</returns>
        public static IssuerPublicKeyData ExtractIssuerPublicKeyTags(string tlvHexString)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0,
                $"ExtractIssuerPublicKeyTags: Converting hex string (length={tlvHexString?.Length ?? 0})");

            if (string.IsNullOrWhiteSpace(tlvHexString))
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ExtractIssuerPublicKeyTags: Null or empty hex string");
                return null;
            }

            try
            {
                byte[] tlvData = FromHexString(tlvHexString);
                return ExtractIssuerPublicKeyTags(tlvData);
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"ExtractIssuerPublicKeyTags: Hex conversion failed - {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }


    }
}
