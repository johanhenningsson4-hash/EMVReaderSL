using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EMVCard
{
    /// <summary>
    /// Handles GPO (Get Processing Options) command construction and execution.
    /// </summary>
    public class EmvGpoProcessor
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvGpoProcessor");

        private readonly EmvCardReader _cardReader;

        public event EventHandler<string> LogMessage;

        public EmvGpoProcessor(EmvCardReader cardReader)
        {
            _cardReader = cardReader ?? throw new ArgumentNullException(nameof(cardReader));
        }

        /// <summary>
        /// Send GPO command with automatic PDOL construction.
        /// </summary>
        /// <param name="fciData">FCI data from application selection</param>
        /// <param name="gpoResponse">GPO response data</param>
        /// <returns>True if GPO successful</returns>
        public bool SendGPO(byte[] fciData, out byte[] gpoResponse)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "SendGPO: Processing GPO command");

            gpoResponse = null;

            // Look for PDOL (tag 9F38) in FCI
            int index = 0;
            while (index < fciData.Length - 2)
            {
                if (fciData[index] == 0x9F && fciData[index + 1] == 0x38)
                {
                    // Found PDOL
                    index += 2;
                    int len = fciData[index++];
                    byte[] pdolRaw = new byte[len];
                    Array.Copy(fciData, index, pdolRaw, 0, len);

                    // Construct PDOL data
                    byte[] pdolData = ConstructPDOLData(pdolRaw);

                    // Build GPO command with PDOL
                    List<byte> gpo = new List<byte>
                    {
                        0x80, 0xA8, 0x00, 0x00,
                        (byte)(pdolData.Length + 2),
                        0x83,
                        (byte)pdolData.Length
                    };
                    gpo.AddRange(pdolData);
                    gpo.Add(0x00); // Le

                    if (!_cardReader.SendApduWithAutoFix(gpo.ToArray(), out gpoResponse))
                    {
                        OnLogMessage("GPO command failed");
                        return false;
                    }

                    if (gpoResponse[0] == 0x80 || gpoResponse[0] == 0x77)
                    {
                        OnLogMessage("GPO successful response (with PDOL)");
                        return true;
                    }
                    else
                    {
                        OnLogMessage("GPO response format abnormal");
                        return false;
                    }
                }
                index++;
            }

            // No PDOL found, send simplified GPO
            OnLogMessage("No PDOL found, sending simplified GPO");
            return SendSimplifiedGPO(out gpoResponse);
        }

        /// <summary>
        /// Send simplified GPO (no PDOL).
        /// </summary>
        private bool SendSimplifiedGPO(out byte[] gpoResponse)
        {
            byte[] gpoCmd = new byte[] { 0x80, 0xA8, 0x00, 0x00, 0x02, 0x83, 0x00, 0x00 };

            if (!_cardReader.SendApduWithAutoFix(gpoCmd, out gpoResponse))
            {
                OnLogMessage("Simplified GPO command failed");
                return false;
            }

            if (gpoResponse[0] == 0x80 || gpoResponse[0] == 0x77)
            {
                OnLogMessage("GPO successful response (simplified mode)");
                return true;
            }
            else
            {
                OnLogMessage("Simplified GPO response format abnormal");
                return false;
            }
        }

        /// <summary>
        /// Construct PDOL data by filling appropriate values for each tag.
        /// </summary>
        private byte[] ConstructPDOLData(byte[] pdolRaw)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0,
                $"ConstructPDOLData: Constructing PDOL data from {pdolRaw.Length} bytes");

            List<byte> pdolData = new List<byte>();
            int pdolIndex = 0;

            while (pdolIndex < pdolRaw.Length)
            {
                int tag = pdolRaw[pdolIndex++];
                if ((tag & 0x1F) == 0x1F)
                {
                    tag = (tag << 8) | pdolRaw[pdolIndex++];
                }

                int tagLen = pdolRaw[pdolIndex++];

                // Fill data based on tag
                switch (tag)
                {
                    case 0x9F66: // TTQ (Terminal Transaction Qualifiers)
                        pdolData.AddRange(new byte[] { 0x37, 0x00, 0x00, 0x00 });
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added TTQ");
                        break;

                    case 0x9F02: // Amount Authorized
                        pdolData.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added Amount Authorized");
                        break;

                    case 0x9F03: // Amount Other (Cashback)
                        pdolData.AddRange(new byte[tagLen]);
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added Amount Other");
                        break;

                    case 0x9F1A: // Terminal Country Code
                    case 0x5F2A: // Transaction Currency Code
                        pdolData.AddRange(new byte[] { 0x01, 0x56 }); // China/RMB: 0156
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ConstructPDOLData: Added Country/Currency Code (tag {tag:X})");
                        break;

                    case 0x9A: // Transaction Date (YYMMDD)
                        var date = DateTime.Now;
                        pdolData.AddRange(new byte[]
                        {
                            (byte)(date.Year % 100),
                            (byte)date.Month,
                            (byte)date.Day
                        });
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added Transaction Date");
                        break;

                    case 0x9C: // Transaction Type
                        pdolData.Add(0x00); // Purchase
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added Transaction Type");
                        break;

                    case 0x9F37: // Unpredictable Number
                        var rnd = new Random();
                        for (int i = 0; i < tagLen; i++)
                        {
                            pdolData.Add((byte)rnd.Next(0, 256));
                        }
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, "ConstructPDOLData: Added Unpredictable Number");
                        break;

                    default:
                        // Fill with zeros for unknown tags
                        pdolData.AddRange(new byte[tagLen]);
                        _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"ConstructPDOLData: Added {tagLen} zeros for tag {tag:X}");
                        break;
                }
            }

            _traceSource.TraceEvent(TraceEventType.Information, 0,
                $"ConstructPDOLData: Constructed {pdolData.Count} bytes of PDOL data");

            return pdolData.ToArray();
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
