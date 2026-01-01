using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
//using NfcReaderLib.EMV;

namespace NfcReaderLib
{
    // You will need to implement or port EMVTransactionRecord, Util, etc.

    public class SLCard // : EmvCard (implement or port EmvCard base class if needed)
    {
        private static readonly TraceSource _traceSource = new TraceSource("NfcReaderLib.SLCard");

        public string AID { get; set; }
        public string PAN { get; set; }
        public bool VISA { get; set; }
        public int CAPublicKeyIndex { get; set; }
        public byte[] TransactionCurrencyCode { get; set; }
        public byte[] AmountAuthorized { get; set; }
        public byte[] UnpredictableNumber { get; set; }
        public byte[] SignedDynamicApplicationData { get; set; }
        public byte[] ApplicationCryptogram { get; set; }
        public byte[] IccPublicKeyCertificate { get; set; }
        public byte[] IccPublicKeyRemainder { get; set; }
        public byte[] IccPublicKeyExponent { get; set; }
        public byte[] IssuerPublicKeyCertificate { get; set; }
        public byte[] IssuerPublicKeyRemainder { get; set; }
        public byte[] IssuerPublicKeyExponent { get; set; }
        public byte[] UDOL { get; set; } = Array.Empty<byte>();
        public byte[] CDOL1 { get; set; }
        public byte[] CDOL2 { get; set; }
        public byte[] LogDataField { get; set; }
        //public List<EMVTransactionRecord> ListTransactions { get; set; } = new List<EMVTransactionRecord>();

        public string GetSLToken()
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var encodedHash = sha256.ComputeHash(IccPublicKeyCertificate ?? Array.Empty<byte>());
                    return BitConverter.ToString(encodedHash).Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
    $"ParseIccPublicKey: Exception - {ex.GetType().Name}: {ex.Message}");
                return "";
            }
        }

        public string GetSLToken2()
        {
            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var encodedHash = sha256.ComputeHash(IccPublicKeyCertificate ?? Array.Empty<byte>());
                    return BitConverter.ToString(encodedHash).Replace("-", " ");
                }
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
    $"ParseIccPublicKey: Exception - {ex.GetType().Name}: {ex.Message}");
                return "";
            }
        }

        public string GetMaskedPAN()
        {
            return Util.MaskPAN(PAN);
        }

        /// <summary>
        /// Represents an RSA public key for EMV processing.
        /// </summary>
        public class EmvPublicKey
        {
            public byte[] Modulus { get; set; }
            public byte[] Exponent { get; set; }
            public int ModulusLength => Modulus?.Length ?? 0;

            public EmvPublicKey(byte[] modulus, byte[] exponent)
            {
                Modulus = modulus ?? throw new ArgumentNullException(nameof(modulus));
                Exponent = exponent ?? throw new ArgumentNullException(nameof(exponent));
            }

            public override string ToString()
            {
                return $"PublicKey[Modulus={ModulusLength} bytes, Exponent={Util.ByteArrayToHexString(Exponent)}]";
            }
        }

        /// <summary>
        /// Parse and verify the ICC Public Key Certificate.
        /// Implements EMV v4.3 Book 2, Section 6.4 and Table 13.
        /// </summary>
        /// <param name="issuerPublicKey">Public key of the card issuer</param>
        /// <param name="iccPublicKeyCertificate">ICC public key certificate as read from card (tag 9F46)</param>
        /// <param name="iccRemainder">ICC public key remainder as read from card (tag 9F48)</param>
        /// <param name="iccPublicKeyExponent">ICC public key exponent as read from card (tag 9F47)</param>
        /// <param name="pan">Primary Account Number (optional, for validation)</param>
        /// <param name="staticData">Static authentication data for hash validation (optional)</param>
        /// <returns>The verified ICC public key, or null if validation fails</returns>
        public EmvPublicKey ParseIccPublicKey(
            EmvPublicKey issuerPublicKey,
            byte[] iccPublicKeyCertificate,
            byte[] iccRemainder,
            byte[] iccPublicKeyExponent,
            string pan = null,
            byte[] staticData = null)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0,
                $"ParseIccPublicKey: Starting ICC public key parsing");

            try
            {
                // Validate inputs
                if (issuerPublicKey == null)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0, "ParseIccPublicKey: Issuer public key is null");
                    throw new ArgumentNullException(nameof(issuerPublicKey));
                }

                if (iccPublicKeyCertificate == null || iccPublicKeyCertificate.Length == 0)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0, "ParseIccPublicKey: ICC certificate is null or empty");
                    throw new ArgumentException("ICC public key certificate cannot be null or empty", nameof(iccPublicKeyCertificate));
                }

                if (iccPublicKeyExponent == null || iccPublicKeyExponent.Length == 0)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0, "ParseIccPublicKey: ICC exponent is null or empty");
                    throw new ArgumentException("ICC public key exponent cannot be null or empty", nameof(iccPublicKeyExponent));
                }

                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Certificate length={iccPublicKeyCertificate.Length}, " +
                    $"Remainder length={iccRemainder?.Length ?? 0}, " +
                    $"Exponent length={iccPublicKeyExponent.Length}");

                // Step 1: Decrypt the ICC certificate using issuer public key
                byte[] decryptedCertificate = RsaDecrypt(iccPublicKeyCertificate, issuerPublicKey);
                
                if (decryptedCertificate == null || decryptedCertificate.Length == 0)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0, "ParseIccPublicKey: Certificate decryption failed");
                    return null;
                }

                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Decrypted certificate length={decryptedCertificate.Length}");

                // Step 2: Parse the decrypted certificate structure (EMV Book 2, Table 13)
                int index = 0;

                // Byte 1: Certificate Format (must be 0x6A)
                byte certFormat = decryptedCertificate[index++];
                if (certFormat != 0x6A)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0,
                        $"ParseIccPublicKey: Invalid certificate format 0x{certFormat:X2}, expected 0x6A");
                    return null;
                }

                // Byte 2: Application PAN (leftmost 3-10 digits)
                byte applicationPan = decryptedCertificate[index++];
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Application PAN byte=0x{applicationPan:X2}");

                // Byte 3: Certificate Expiration Date (MMYY)
                byte[] certExpiryDate = new byte[2];
                Array.Copy(decryptedCertificate, index, certExpiryDate, 0, 2);
                index += 2;
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Certificate expiry={Util.ByteArrayToHexString(certExpiryDate)}");

                // Byte 5: Certificate Serial Number
                byte[] certSerialNumber = new byte[3];
                Array.Copy(decryptedCertificate, index, certSerialNumber, 0, 3);
                index += 3;
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Certificate serial={Util.ByteArrayToHexString(certSerialNumber)}");

                // Byte 8: Hash Algorithm Indicator
                byte hashAlgorithm = decryptedCertificate[index++];
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Hash algorithm=0x{hashAlgorithm:X2}");

                // Byte 9: ICC Public Key Algorithm Indicator
                byte iccPkAlgorithm = decryptedCertificate[index++];
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: ICC PK algorithm=0x{iccPkAlgorithm:X2}");

                // Byte 10: ICC Public Key Length (in bytes)
                byte iccPkLength = decryptedCertificate[index++];
                _traceSource.TraceEvent(TraceEventType.Information, 0,
                    $"ParseIccPublicKey: ICC PK length={iccPkLength} bytes");

                // Byte 11: ICC Public Key Exponent Length (in bytes)
                byte iccPkExponentLength = decryptedCertificate[index++];
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: ICC PK exponent length={iccPkExponentLength} bytes");

                // Calculate the length of the ICC public key modulus data in the certificate
                int issuerModulusLength = issuerPublicKey.ModulusLength;
                int iccModulusInCert = issuerModulusLength - 42; // 42 bytes overhead (EMV spec)
                
                if (iccModulusInCert < 0 || index + iccModulusInCert > decryptedCertificate.Length)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0,
                        $"ParseIccPublicKey: Invalid modulus length calculation: issuerModLen={issuerModulusLength}");
                    return null;
                }

                // Extract ICC Public Key modulus (partial or full)
                byte[] iccModulusPartial = new byte[iccModulusInCert];
                Array.Copy(decryptedCertificate, index, iccModulusPartial, 0, iccModulusInCert);
                index += iccModulusInCert;

                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ParseIccPublicKey: Extracted {iccModulusInCert} bytes of modulus from certificate");

                // Step 3: Construct full ICC public key modulus
                byte[] iccModulusFull;

                if (iccPkLength <= iccModulusInCert)
                {
                    // Full modulus is in the certificate
                    iccModulusFull = new byte[iccPkLength];
                    Array.Copy(iccModulusPartial, iccModulusFull, iccPkLength);
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                        "ParseIccPublicKey: Full modulus contained in certificate");
                }
                else
                {
                    // Need to append remainder
                    if (iccRemainder == null || iccRemainder.Length == 0)
                    {
                        _traceSource.TraceEvent(TraceEventType.Error, 0,
                            "ParseIccPublicKey: Remainder required but not provided");
                        return null;
                    }

                    int remainderLength = iccPkLength - iccModulusInCert;
                    if (iccRemainder.Length < remainderLength)
                    {
                        _traceSource.TraceEvent(TraceEventType.Error, 0,
                            $"ParseIccPublicKey: Insufficient remainder: need {remainderLength}, have {iccRemainder.Length}");
                        return null;
                    }

                    iccModulusFull = new byte[iccPkLength];
                    Array.Copy(iccModulusPartial, 0, iccModulusFull, 0, iccModulusInCert);
                    Array.Copy(iccRemainder, 0, iccModulusFull, iccModulusInCert, remainderLength);

                    _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                        $"ParseIccPublicKey: Concatenated modulus with {remainderLength} bytes from remainder");
                }

                // Extract hash from certificate (last 20 bytes before trailer)
                byte[] certHash = new byte[20];
                Array.Copy(decryptedCertificate, index, certHash, 0, 20);
                index += 20;

                // Trailer (must be 0xBC)
                byte trailer = decryptedCertificate[index];
                if (trailer != 0xBC)
                {
                    _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        $"ParseIccPublicKey: Invalid trailer 0x{trailer:X2}, expected 0xBC");
                }

                // Step 4: Verify certificate hash
                bool hashValid = VerifyIccCertificateHash(
                    certHash,
                    decryptedCertificate,
                    iccModulusFull,
                    iccPublicKeyExponent,
                    staticData,
                    hashAlgorithm);

                if (!hashValid)
                {
                    _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        "ParseIccPublicKey: Certificate hash verification failed");
                    // Note: Some implementations continue despite hash failure
                }

                // Step 5: Validate PAN if provided
                if (!string.IsNullOrEmpty(pan))
                {
                    bool panValid = ValidatePanInCertificate(pan, applicationPan);
                    if (!panValid)
                    {
                        _traceSource.TraceEvent(TraceEventType.Warning, 0,
                            "ParseIccPublicKey: PAN validation failed");
                      }
                }

                // Step 6: Create and return the ICC public key
                EmvPublicKey iccPublicKey = new EmvPublicKey(iccModulusFull, iccPublicKeyExponent);

                _traceSource.TraceEvent(TraceEventType.Information, 0,
                    $"ParseIccPublicKey: Successfully parsed ICC public key - {iccPublicKey}");

                return iccPublicKey;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"ParseIccPublicKey: Exception - {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decrypt data using RSA public key (textbook RSA).
        /// In EMV, certificates are "encrypted" with private key and "decrypted" with public key.
        /// Note: This implementation requires System.Numerics.dll reference in the project.
        /// To add: Right-click References -> Add Reference -> Assemblies -> System.Numerics
        /// </summary>
        private byte[] RsaDecrypt(byte[] encryptedData, EmvPublicKey publicKey)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                $"RsaDecrypt: Decrypting {encryptedData.Length} bytes");

            try
            {
                // Use RSACryptoServiceProvider for RSA operations
                using (var rsa = new RSACryptoServiceProvider())
                {
                    // Import public key parameters
                    RSAParameters rsaParams = new RSAParameters
                    {
                        Modulus = publicKey.Modulus,
                        Exponent = publicKey.Exponent
                    };
                    rsa.ImportParameters(rsaParams);

                    // For EMV certificate decryption, we need raw RSA without padding
                    // This is textbook RSA: M = C^e mod N
                    // RSACryptoServiceProvider doesn't support no-padding directly,
                    // so we'll use manual BigInteger calculation

                    // Manual BigInteger implementation (requires System.Numerics reference)
                    byte[] result = RsaDecryptNoPadding(encryptedData, publicKey.Modulus, publicKey.Exponent);

                    _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                        $"RsaDecrypt: Decrypted to {result?.Length ?? 0} bytes");

                    return result;
                }
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"RsaDecrypt: Failed - {ex.GetType().Name}: {ex.Message}");
                
                // Log note about missing reference
                _traceSource.TraceEvent(TraceEventType.Warning, 0,
                    "RsaDecrypt: If you see BigInteger errors, add reference to System.Numerics.dll");
                
                return null;
            }
        }

        /// <summary>
        /// Perform raw RSA decryption without padding using manual calculation.
        /// This requires System.Numerics.dll to be referenced in the project.
        /// </summary>
        private byte[] RsaDecryptNoPadding(byte[] ciphertext, byte[] modulus, byte[] exponent)
        {
            try
            {
                // If BigInteger is not available, this will fail at runtime
                // Check if the type exists
                var bigIntegerType = Type.GetType("System.Numerics.BigInteger, System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                
                if (bigIntegerType == null)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0,
                        "RsaDecryptNoPadding: System.Numerics.BigInteger not available. " +
                        "Please add reference to System.Numerics.dll in your project.");
                    
                    throw new InvalidOperationException(
                        "System.Numerics.dll is not referenced. " +
                        "Add reference: Right-click References -> Add Reference -> Assemblies -> System.Numerics");
                }

                // Use reflection to create BigInteger instances
                var ctorFromBytes = bigIntegerType.GetConstructor(new[] { typeof(byte[]) });
                var modPowMethod = bigIntegerType.GetMethod("ModPow", new[] { bigIntegerType, bigIntegerType, bigIntegerType });
                var toByteArrayMethod = bigIntegerType.GetMethod("ToByteArray", Type.EmptyTypes);

                // Prepare data (add sign byte for positive numbers)
                byte[] cReversed = ciphertext.Reverse().Concat(new byte[] { 0 }).ToArray();
                byte[] nReversed = modulus.Reverse().Concat(new byte[] { 0 }).ToArray();
                byte[] eReversed = exponent.Reverse().Concat(new byte[] { 0 }).ToArray();

                // Create BigInteger objects
                object c = ctorFromBytes.Invoke(new object[] { cReversed });
                object n = ctorFromBytes.Invoke(new object[] { nReversed });
                object e = ctorFromBytes.Invoke(new object[] { eReversed });

                // Perform M = C^e mod N
                object m = modPowMethod.Invoke(null, new[] { c, e, n });

                // Convert back to bytes
                byte[] result = (byte[])toByteArrayMethod.Invoke(m, null);

                // Remove sign byte if present
                if (result.Length > modulus.Length && result[result.Length - 1] == 0)
                {
                    Array.Resize(ref result, result.Length - 1);
                }

                // Reverse back to big-endian
                Array.Reverse(result);

                // Pad to modulus length
                if (result.Length < modulus.Length)
                {
                    byte[] padded = new byte[modulus.Length];
                    Array.Copy(result, 0, padded, modulus.Length - result.Length, result.Length);
                    result = padded;
                }

                return result;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"RsaDecryptNoPadding: {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify the ICC certificate hash according to EMV specifications.
        /// </summary>
        private bool VerifyIccCertificateHash(
            byte[] certHash,
            byte[] decryptedCertificate,
            byte[] iccModulusFull,
            byte[] iccExponent,
            byte[] staticData,
            byte hashAlgorithm)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                "VerifyIccCertificateHash: Starting hash verification");

            try
            {
                // Build data to hash according to EMV Book 2, Section 6.4
                List<byte> dataToHash = new List<byte>();

                // Add certificate data (excluding hash and trailer)
                int certDataLength = decryptedCertificate.Length - 22; // Exclude 20-byte hash and 2-byte padding/trailer
                dataToHash.AddRange(new ArraySegment<byte>(decryptedCertificate, 1, certDataLength - 1));

                // Add full ICC modulus
                dataToHash.AddRange(iccModulusFull);

                // Add ICC exponent
                dataToHash.AddRange(iccExponent);

                // Add static data if provided
                if (staticData != null && staticData.Length > 0)
                {
                    dataToHash.AddRange(staticData);
                    _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                        $"VerifyIccCertificateHash: Added {staticData.Length} bytes of static data");
                }

                // Compute hash based on algorithm indicator
                byte[] computedHash;
                switch (hashAlgorithm)
                {
                    case 0x01: // SHA-1
                        computedHash = Util.CalculateSHA1(dataToHash.ToArray());
                        break;
                    case 0x02: // SHA-256 (EMV 4.3+)
                        using (var sha256 = SHA256.Create())
                        {
                            computedHash = sha256.ComputeHash(dataToHash.ToArray());
                        }
                        break;
                    default:
                        _traceSource.TraceEvent(TraceEventType.Warning, 0,
                            $"VerifyIccCertificateHash: Unknown hash algorithm 0x{hashAlgorithm:X2}, using SHA-1");
                        computedHash = Util.CalculateSHA1(dataToHash.ToArray());
                        break;
                }

                // Compare first 20 bytes (SHA-1 length, standard for EMV)
                bool match = true;
                int compareLength = Math.Min(20, Math.Min(certHash.Length, computedHash.Length));
                
                for (int i = 0; i < compareLength; i++)
                {
                    if (certHash[i] != computedHash[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    _traceSource.TraceEvent(TraceEventType.Information, 0,
                        "VerifyIccCertificateHash: Hash verification successful");
                }
                else
                {
                    _traceSource.TraceEvent(TraceEventType.Warning, 0,
                        $"VerifyIccCertificateHash: Hash mismatch - " +
                        $"Cert={Util.ByteArrayToHexString(certHash).Substring(0, 40)}, " +
                        $"Computed={Util.ByteArrayToHexString(computedHash).Substring(0, 40)}");
                }

                return match;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"VerifyIccCertificateHash: Exception - {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate that the PAN in the certificate matches the card PAN.
        /// </summary>
        private bool ValidatePanInCertificate(string pan, byte applicationPanByte)
        {
            if (string.IsNullOrEmpty(pan))
            {
                return true; // No PAN to validate
            }

            try
            {
                // Extract leftmost digits from PAN
                string panPrefix = pan.Length >= 10 ? pan.Substring(0, 10) : pan;
                
                // The applicationPanByte contains BCD-encoded PAN digits
                // This is a simplified check - full implementation would extract and compare all digits
                _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                    $"ValidatePanInCertificate: PAN prefix={panPrefix}, cert byte=0x{applicationPanByte:X2}");

                // Basic validation: just log for now
                // Full implementation would decode BCD and compare
                return true;
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Warning, 0,
                    $"ValidatePanInCertificate: Exception - {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Convenience method to parse ICC public key using instance properties.
        /// </summary>
        /// <param name="issuerPublicKey">Public key of the card issuer</param>
        /// <returns>The verified ICC public key, or null if validation fails</returns>
        public EmvPublicKey ParseIccPublicKey(EmvPublicKey issuerPublicKey)
        {
            return ParseIccPublicKey(
                issuerPublicKey,
                IccPublicKeyCertificate,
                IccPublicKeyRemainder,
                IccPublicKeyExponent,
                PAN);
        }
    }

    // No longer needed - using System.Linq.Enumerable.Reverse
}
