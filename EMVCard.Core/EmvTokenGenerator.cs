using NfcReaderLib;
using System;
using System.Diagnostics;

namespace EMVCard
{
    /// <summary>
    /// Generates SL (Secure Link) tokens from ICC certificate data.
    /// </summary>
    public class EmvTokenGenerator
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvTokenGenerator");

        public event EventHandler<string> LogMessage;

        /// <summary>
        /// Result of token generation.
        /// </summary>
        public class TokenResult
        {
            public bool Success { get; set; }
            public string Token { get; set; }
            public string ErrorMessage { get; set; }

            public static TokenResult CreateSuccess(string token)
            {
                return new TokenResult { Success = true, Token = token };
            }

            public static TokenResult CreateError(string errorMessage)
            {
                return new TokenResult { Success = false, ErrorMessage = errorMessage };
            }
        }

        /// <summary>
        /// Generate SL Token from card data.
        /// </summary>
        /// <param name="cardData">Card data containing ICC certificate</param>
        /// <param name="pan">Primary Account Number</param>
        /// <param name="aid">Application ID</param>
        /// <returns>Token generation result</returns>
        public TokenResult GenerateToken(EmvDataParser.EmvCardData cardData, string pan, string aid)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "GenerateToken: Starting SL token generation");

            if (cardData == null)
            {
                return TokenResult.CreateError("Card data is null");
            }

            if (string.IsNullOrWhiteSpace(cardData.IccCertificate))
            {
                OnLogMessage("Warning: No ICC certificate data found for token generation");
                return TokenResult.CreateError("No ICC certificate data available");
            }

            try
            {
                // Parse ICC certificate data (may have multi-line format)
                string[] lines = cardData.IccCertificate.Split(new[] { Environment.NewLine, "\n", "\r" },
                    StringSplitOptions.RemoveEmptyEntries);

                byte[] certificate = null;
                byte[] exponent = null;
                byte[] remainder = null;

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith("Cert:"))
                    {
                        string certHex = trimmedLine.Substring(5).Trim();
                        certificate = Util.FromHexString(certHex);
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                            $"GenerateToken: Extracted certificate ({certificate.Length} bytes)");
                    }
                    else if (trimmedLine.StartsWith("Exp:"))
                    {
                        string expHex = trimmedLine.Substring(4).Trim();
                        exponent = Util.FromHexString(expHex);
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                            $"GenerateToken: Extracted exponent ({exponent.Length} bytes)");
                    }
                    else if (trimmedLine.StartsWith("Rem:"))
                    {
                        string remHex = trimmedLine.Substring(4).Trim();
                        remainder = Util.FromHexString(remHex);
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                            $"GenerateToken: Extracted remainder ({remainder.Length} bytes)");
                    }
                    else
                    {
                        // No prefix, assume it's the certificate
                        if (certificate == null && !string.IsNullOrWhiteSpace(trimmedLine))
                        {
                            certificate = Util.FromHexString(trimmedLine);
                            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                                $"GenerateToken: Extracted certificate from unprefixed line ({certificate.Length} bytes)");
                        }
                    }
                }

                if (certificate == null || certificate.Length == 0)
                {
                    OnLogMessage("Failed to generate SL Token - certificate could not be parsed");
                    return TokenResult.CreateError("Failed to parse ICC certificate");
                }

                // Create SLCard and generate token
                var slCard = new SLCard
                {
                    PAN = pan,
                    AID = aid,
                    IccPublicKeyCertificate = certificate,
                    IccPublicKeyExponent = exponent,
                    IccPublicKeyRemainder = remainder
                };

                string token = slCard.GetSLToken2(); // Use GetSLToken2() for space-separated format

                if (string.IsNullOrEmpty(token))
                {
                    OnLogMessage("Failed to generate SL Token - GetSLToken2 returned empty");
                    return TokenResult.CreateError("Failed to generate SL Token");
                }

                OnLogMessage($"Loaded ICC certificate ({certificate.Length} bytes) for token generation");
                OnLogMessage($"SL Token generated successfully: {token.Substring(0, Math.Min(23, token.Length))}...");

                _traceSource.TraceEvent(TraceEventType.Information, 0,
                    $"GenerateToken: Successfully generated token (length={token.Length})");

                return TokenResult.CreateSuccess(token);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error generating SL Token: {ex.Message}";
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"GenerateToken: {errorMsg}");
                OnLogMessage(errorMsg);
                return TokenResult.CreateError(ex.Message);
            }
        }

        /// <summary>
        /// Generate SL Token directly from ICC certificate bytes.
        /// </summary>
        /// <param name="certificate">ICC public key certificate</param>
        /// <returns>Token generation result</returns>
        public TokenResult GenerateTokenFromCertificate(byte[] certificate)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0,
                $"GenerateTokenFromCertificate: Generating token from {certificate?.Length ?? 0} bytes");

            if (certificate == null || certificate.Length == 0)
            {
                return TokenResult.CreateError("Certificate is null or empty");
            }

            try
            {
                var slCard = new SLCard
                {
                    IccPublicKeyCertificate = certificate
                };

                string token = slCard.GetSLToken2(); // Use GetSLToken2() for space-separated format

                if (string.IsNullOrEmpty(token))
                {
                    return TokenResult.CreateError("Failed to generate token");
                }

                _traceSource.TraceEvent(TraceEventType.Information, 0,
                    $"GenerateTokenFromCertificate: Successfully generated token");

                return TokenResult.CreateSuccess(token);
            }
            catch (Exception ex)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0,
                    $"GenerateTokenFromCertificate: {ex.Message}");
                return TokenResult.CreateError(ex.Message);
            }
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
