using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NfcReaderLib
{
    /// <summary>
    /// Provides diagnostics and user-friendly error messages for PC/SC card reader and driver availability.
    /// </summary>
    public static class ReaderDiagnostics
    {
        /// <summary>
        /// Checks if at least one PC/SC card reader is available. Throws InvalidOperationException with a clear message if not.
        /// </summary>
        public static void EnsureCardReaderAvailable()
        {
            IntPtr hContext = IntPtr.Zero;
            int result = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);
            if (result != ModWinsCard.SCARD_S_SUCCESS)
            {
                throw new InvalidOperationException(
                    $"PC/SC subsystem could not be initialized (error 0x{result:X8}: {ModWinsCard.GetScardErrMsg(result)}).\n" +
                    "Please ensure that your system has the required smart card drivers and services enabled. " +
                    "On Windows, the Smart Card service is usually included by default. If you see this error, your system may be missing the Smart Card service or its drivers.");
            }

            int pcchReaders = 0;
            result = ModWinsCard.SCardListReaders(hContext, null, null, ref pcchReaders);
            if (result == ModWinsCard.SCARD_E_NO_READERS_AVAILABLE || pcchReaders == 0)
            {
                throw new InvalidOperationException(
                    "No PC/SC card readers found. Please connect a compatible card reader and ensure drivers are installed. " +
                    "If you have already connected a reader, install the latest drivers from the manufacturer and restart your computer.");
            }
            else if (result != ModWinsCard.SCARD_S_SUCCESS)
            {
                throw new InvalidOperationException(
                    $"Failed to enumerate card readers (error 0x{result:X8}: {ModWinsCard.GetScardErrMsg(result)}).\n" +
                    "Please ensure your card reader drivers are installed and the device is connected.");
            }
        }
    }
}
