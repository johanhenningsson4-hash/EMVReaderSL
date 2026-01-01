using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EMVCard
{
    /// <summary>
    /// Handles PC/SC card reader communication and APDU transmission.
    /// </summary>
    public class EmvCardReader
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvCardReader");

        private IntPtr _hContext;
        private IntPtr _hCard;
        private int _protocol;

        public IntPtr HContext => _hContext;
        public IntPtr HCard => _hCard;
        public int Protocol => _protocol;
        public bool IsConnected { get; private set; }

        private byte[] _sendBuffer = new byte[263];
        private byte[] _recvBuffer = new byte[263];
        private IntPtr _sendLength;
        private IntPtr _recvLength;
        private ModWinsCard.SCARD_IO_REQUEST _pioSendRequest;

        public event EventHandler<string> LogMessage;

        /// <summary>
        /// Initialize the PC/SC context and enumerate card readers.
        /// </summary>
        /// <returns>List of available reader names</returns>
        public List<string> Initialize()
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "Initialize: Establishing PC/SC context");
            
            var readers = new List<string>();

            // Establish Context
            int retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref _hContext);
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"Initialize: Failed to establish context - {error}");
                OnLogMessage(error);
                return readers;
            }

            // Get reader list size
            int pcchReaders = 0;
            retCode = ModWinsCard.SCardListReaders(_hContext, null, null, ref pcchReaders);
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"Initialize: Failed to list readers - {error}");
                OnLogMessage(error);
                return readers;
            }

            // Get reader list
            byte[] readersList = new byte[pcchReaders];
            retCode = ModWinsCard.SCardListReaders(_hContext, null, readersList, ref pcchReaders);
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"Initialize: Failed to get reader list - {error}");
                OnLogMessage(error);
                return readers;
            }

            // Parse reader names
            string rName = "";
            int index = 0;
            while (readersList[index] != 0)
            {
                while (readersList[index] != 0)
                {
                    rName += (char)readersList[index];
                    index++;
                }
                readers.Add(rName);
                _traceSource.TraceEvent(TraceEventType.Information, 0, $"Initialize: Found reader - {rName}");
                rName = "";
                index++;
            }

            return readers;
        }

        /// <summary>
        /// Connect to a specific card reader.
        /// </summary>
        /// <param name="readerName">Name of the reader to connect to</param>
        /// <returns>True if connection successful</returns>
        public bool Connect(string readerName)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"Connect: Connecting to {readerName}");

            // Disconnect existing connection
            if (IsConnected)
            {
                ModWinsCard.SCardDisconnect(_hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            // Connect to card
            int retCode = ModWinsCard.SCardConnect(
                _hContext,
                readerName,
                ModWinsCard.SCARD_SHARE_SHARED,
                ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1,
                ref _hCard,
                ref _protocol);

            if (retCode == ModWinsCard.SCARD_S_SUCCESS)
            {
                IsConnected = true;
                _traceSource.TraceEvent(TraceEventType.Information, 0, $"Connect: Successfully connected to {readerName}");
                OnLogMessage($"Successful connection to {readerName}");

                // Read ATR
                ReadATR();
                return true;
            }
            else
            {
                IsConnected = false;
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"Connect: Failed - {error}");
                OnLogMessage(error);
                return false;
            }
        }

        /// <summary>
        /// Read and log the ATR (Answer To Reset) from the card.
        /// </summary>
        private void ReadATR()
        {
            byte[] atr = new byte[33];
            IntPtr atrLen = new IntPtr(atr.Length);
            int readerLen = 0;
            int state = 0;
            int protocol = Protocol;

            int retCode = ModWinsCard.SCardStatus(
                _hCard,
                null,
                ref readerLen,
                ref state,
                ref protocol,
                atr,
                ref atrLen);

            if (retCode == ModWinsCard.SCARD_S_SUCCESS)
            {
                string atrStr = BitConverter.ToString(atr, 0, atrLen.ToInt32());
                _traceSource.TraceEvent(TraceEventType.Information, 0, $"ReadATR: {atrStr}");
                OnLogMessage($"ATR: {atrStr}");

                // Determine card type
                if (atrLen.ToInt32() > 0 && (atr[0] == 0x3B || atr[0] == 0x3F))
                {
                    OnLogMessage("Card default working in contact mode");
                }
                else
                {
                    OnLogMessage("Card default working in contactless mode");
                }
            }
            else
            {
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Warning, 0, $"ReadATR: Failed - {error}");
                OnLogMessage($"Unable to read ATR: {error}");
            }
        }

        /// <summary>
        /// Send APDU command to the card.
        /// </summary>
        /// <param name="apdu">APDU command as byte array</param>
        /// <param name="response">Response from card</param>
        /// <returns>True if transmission successful</returns>
        public bool SendApdu(byte[] apdu, out byte[] response)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"SendApdu: Sending {apdu.Length} bytes");

            response = null;

            if (!IsConnected)
            {
                _traceSource.TraceEvent(TraceEventType.Error, 0, "SendApdu: Not connected to card");
                return false;
            }

            // Copy APDU to send buffer
            Array.Copy(apdu, _sendBuffer, apdu.Length);
            _sendLength = new IntPtr(apdu.Length);

            // Log outgoing APDU
            string apduHex = BitConverter.ToString(apdu).Replace("-", " ");
            OnLogMessage($"< {apduHex}");

            // Setup request structure
            _pioSendRequest.dwProtocol = Protocol;
            _pioSendRequest.cbPciLength = 8;

            // Transmit
            _recvLength = new IntPtr(_recvBuffer.Length);
            int retCode = ModWinsCard.SCardTransmit(
                _hCard,
                ref _pioSendRequest,
                _sendBuffer,
                _sendLength,
                ref _pioSendRequest,
                _recvBuffer,
                ref _recvLength);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                string error = ModWinsCard.GetScardErrMsg(retCode);
                _traceSource.TraceEvent(TraceEventType.Error, 0, $"SendApdu: Transmission failed - {error}");
                OnLogMessage(error);
                return false;
            }

            // Copy response
            response = new byte[_recvLength.ToInt32()];
            Array.Copy(_recvBuffer, response, _recvLength.ToInt32());

            // Log response
            string responseHex = BitConverter.ToString(response).Replace("-", " ");
            OnLogMessage($"> {responseHex}");

            _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"SendApdu: Received {response.Length} bytes");
            return true;
        }

        /// <summary>
        /// Send APDU with automatic error handling for common cases (6C, 67, 61).
        /// </summary>
        /// <param name="apdu">APDU command</param>
        /// <param name="response">Response from card</param>
        /// <returns>True if successful</returns>
        public bool SendApduWithAutoFix(byte[] apdu, out byte[] response)
        {
            if (!SendApdu(apdu, out response))
            {
                return false;
            }

            // Case 1: SW = 6C XX (Wrong Le)
            if (response.Length == 2 && response[0] == 0x6C)
            {
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"SendApduWithAutoFix: Retrying with Le={response[1]:X2}");
                apdu[apdu.Length - 1] = response[1];
                return SendApdu(apdu, out response);
            }

            // Case 2: SW = 67 00 (Wrong length, no Le)
            if (response.Length == 2 && response[0] == 0x67 && response[1] == 0x00)
            {
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, "SendApduWithAutoFix: Retrying with Le=FF");
                apdu[apdu.Length - 1] = 0xFF;
                return SendApdu(apdu, out response);
            }

            // Case 3: SW = 61 XX (More data available)
            if (response.Length == 2 && response[0] == 0x61)
            {
                byte le = response[1];
                _traceSource.TraceEvent(TraceEventType.Verbose, 0, $"SendApduWithAutoFix: Getting response with Le={le:X2}");
                byte[] getResponse = new byte[] { 0x00, 0xC0, 0x00, 0x00, le };
                return SendApdu(getResponse, out response);
            }

            return true;
        }

        /// <summary>
        /// Disconnect from the card.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                _traceSource.TraceEvent(TraceEventType.Information, 0, "Disconnect: Disconnecting from card");
                ModWinsCard.SCardDisconnect(_hCard, ModWinsCard.SCARD_UNPOWER_CARD);
                IsConnected = false;
            }
        }

        /// <summary>
        /// Release PC/SC context and cleanup resources.
        /// </summary>
        public void Release()
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "Release: Releasing PC/SC context");
            Disconnect();
            if (_hContext != IntPtr.Zero)
            {
                ModWinsCard.SCardReleaseContext(_hContext);
                _hContext = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Clear internal buffers.
        /// </summary>
        public void ClearBuffers()
        {
            Array.Clear(_sendBuffer, 0, _sendBuffer.Length);
            Array.Clear(_recvBuffer, 0, _recvBuffer.Length);
        }

        /// <summary>
        /// Initialize the PC/SC context and enumerate card readers asynchronously.
        /// </summary>
        /// <returns>List of available reader names</returns>
        public Task<List<string>> InitializeAsync()
        {
            return Task.Run(() => Initialize());
        }

        /// <summary>
        /// Connect to a specific card reader asynchronously.
        /// </summary>
        /// <param name="readerName">Name of the reader to connect to</param>
        /// <returns>True if connection successful</returns>
        public Task<bool> ConnectAsync(string readerName)
        {
            return Task.Run(() => Connect(readerName));
        }

        /// <summary>
        /// Send APDU command to the card asynchronously.
        /// </summary>
        /// <param name="apdu">APDU command as byte array</param>
        /// <returns>Tuple containing success flag and response</returns>
        public Task<(bool success, byte[] response)> SendApduAsync(byte[] apdu)
        {
            return Task.Run(() =>
            {
                bool success = SendApdu(apdu, out byte[] response);
                return (success, response);
            });
        }

        /// <summary>
        /// Send APDU with automatic error handling for common cases asynchronously.
        /// </summary>
        /// <param name="apdu">APDU command</param>
        /// <returns>Tuple containing success flag and response</returns>
        public Task<(bool success, byte[] response)> SendApduWithAutoFixAsync(byte[] apdu)
        {
            return Task.Run(() =>
            {
                bool success = SendApduWithAutoFix(apdu, out byte[] response);
                return (success, response);
            });
        }

        /// <summary>
        /// Disconnect from the card asynchronously.
        /// </summary>
        public Task DisconnectAsync()
        {
            return Task.Run(() => Disconnect());
        }

        /// <summary>
        /// Release PC/SC context and cleanup resources asynchronously.
        /// </summary>
        public Task ReleaseAsync()
        {
            return Task.Run(() => Release());
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
