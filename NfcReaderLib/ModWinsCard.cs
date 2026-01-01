/*=========================================================================================
'   Author :  Auto-generated wrapper
'
'   Module :  ModWinscard.cs (Platform-independent wrapper)
'
'   Date   :  Auto-generated
'
'   Description:
'       Automatically detects platform (32-bit or 64-bit) and delegates to the
'       appropriate ModWinsCard32 or ModWinsCard64 implementation.
'
'=========================================================================================*/

using System;
using System.Runtime.InteropServices;

public static class ModWinsCard
{
    private static readonly bool Is64Bit = IntPtr.Size == 8;

    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_IO_REQUEST
    {
        public int dwProtocol;
        public int cbPciLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APDURec
    {
        public byte bCLA;
        public byte bINS;
        public byte bP1;
        public byte bP2;
        public byte bP3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] SW;
        public bool IsSend;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_READERSTATE
    {
        public string RdrName;
        public IntPtr UserData;
        public uint RdrCurrState;
        public uint RdrEventState;
        public uint ATRLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
        public byte[] ATRValue;
    }

    public const int SCARD_S_SUCCESS = 0;
    public const int SCARD_ATR_LENGTH = 33;

    /* ===========================================================
    '  Memory Card type constants
    '===========================================================*/
    public const int CT_MCU = 0x00;
    public const int CT_IIC_Auto = 0x01;
    public const int CT_IIC_1K = 0x02;
    public const int CT_IIC_2K = 0x03;
    public const int CT_IIC_4K = 0x04;
    public const int CT_IIC_8K = 0x05;
    public const int CT_IIC_16K = 0x06;
    public const int CT_IIC_32K = 0x07;
    public const int CT_IIC_64K = 0x08;
    public const int CT_IIC_128K = 0x09;
    public const int CT_IIC_256K = 0x0A;
    public const int CT_IIC_512K = 0x0B;
    public const int CT_IIC_1024K = 0x0C;
    public const int CT_AT88SC153 = 0x0D;
    public const int CT_AT88SC1608 = 0x0E;
    public const int CT_SLE4418 = 0x0F;
    public const int CT_SLE4428 = 0x10;
    public const int CT_SLE4432 = 0x11;
    public const int CT_SLE4442 = 0x12;
    public const int CT_SLE4406 = 0x13;
    public const int CT_SLE4436 = 0x14;
    public const int CT_SLE5536 = 0x15;
    public const int CT_MCUT0 = 0x16;
    public const int CT_MCUT1 = 0x17;
    public const int CT_MCU_Auto = 0x18;

    /*===============================================================
    ' CONTEXT SCOPE
    ===============================================================*/
    public const int SCARD_SCOPE_USER = 0;
    public const int SCARD_SCOPE_TERMINAL = 1;
    public const int SCARD_SCOPE_SYSTEM = 2;

    /*===============================================================
    ' Context State
    ===============================================================*/
    public const int SCARD_STATE_UNAWARE = 0x00;
    public const int SCARD_STATE_IGNORE = 0x01;
    public const int SCARD_STATE_CHANGED = 0x02;
    public const int SCARD_STATE_UNKNOWN = 0x04;
    public const int SCARD_STATE_UNAVAILABLE = 0x08;
    public const int SCARD_STATE_EMPTY = 0x10;
    public const int SCARD_STATE_PRESENT = 0x20;
    public const int SCARD_STATE_ATRMATCH = 0x40;
    public const int SCARD_STATE_EXCLUSIVE = 0x80;
    public const int SCARD_STATE_INUSE = 0x100;
    public const int SCARD_STATE_MUTE = 0x200;
    public const int SCARD_STATE_UNPOWERED = 0x400;

    /*===============================================================
    ' Share Mode
    ===============================================================*/
    public const int SCARD_SHARE_EXCLUSIVE = 1;
    public const int SCARD_SHARE_SHARED = 2;
    public const int SCARD_SHARE_DIRECT = 3;

    /*===========================================================
    '   Disposition
    '===========================================================*/
    public const int SCARD_LEAVE_CARD = 0;
    public const int SCARD_RESET_CARD = 1;
    public const int SCARD_UNPOWER_CARD = 2;
    public const int SCARD_EJECT_CARD = 3;

    /* ===========================================================
    ' ACS IOCTL class
    '===========================================================*/
    public const long FILE_DEVICE_SMARTCARD = 0x310000;
    public const long IOCTL_SMARTCARD_DIRECT = FILE_DEVICE_SMARTCARD + 2050 * 4;
    public const long IOCTL_SMARTCARD_SELECT_SLOT = FILE_DEVICE_SMARTCARD + 2051 * 4;
    public const long IOCTL_SMARTCARD_DRAW_LCDBMP = FILE_DEVICE_SMARTCARD + 2052 * 4;
    public const long IOCTL_SMARTCARD_DISPLAY_LCD = FILE_DEVICE_SMARTCARD + 2053 * 4;
    public const long IOCTL_SMARTCARD_CLR_LCD = FILE_DEVICE_SMARTCARD + 2054 * 4;
    public const long IOCTL_SMARTCARD_READ_KEYPAD = FILE_DEVICE_SMARTCARD + 2055 * 4;
    public const long IOCTL_SMARTCARD_READ_RTC = FILE_DEVICE_SMARTCARD + 2057 * 4;
    public const long IOCTL_SMARTCARD_SET_RTC = FILE_DEVICE_SMARTCARD + 2058 * 4;
    public const long IOCTL_SMARTCARD_SET_OPTION = FILE_DEVICE_SMARTCARD + 2059 * 4;
    public const long IOCTL_SMARTCARD_SET_LED = FILE_DEVICE_SMARTCARD + 2060 * 4;
    public const long IOCTL_SMARTCARD_LOAD_KEY = FILE_DEVICE_SMARTCARD + 2062 * 4;
    public const long IOCTL_SMARTCARD_READ_EEPROM = FILE_DEVICE_SMARTCARD + 2065 * 4;
    public const long IOCTL_SMARTCARD_WRITE_EEPROM = FILE_DEVICE_SMARTCARD + 2066 * 4;
    public const long IOCTL_SMARTCARD_GET_VERSION = FILE_DEVICE_SMARTCARD + 2067 * 4;
    public const long IOCTL_SMARTCARD_GET_READER_INFO = FILE_DEVICE_SMARTCARD + 2051 * 4;
    public const long IOCTL_SMARTCARD_SET_CARD_TYPE = FILE_DEVICE_SMARTCARD + 2060 * 4;
    public const long IOCTL_SMARTCARD_ACR128_ESCAPE_COMMAND = FILE_DEVICE_SMARTCARD + 2079 * 4;

    /*===========================================================
    '   Error Codes
    '===========================================================*/
    public const int SCARD_F_INTERNAL_ERROR = -2146435071;
    public const int SCARD_E_CANCELLED = -2146435070;
    public const int SCARD_E_INVALID_HANDLE = -2146435069;
    public const int SCARD_E_INVALID_PARAMETER = -2146435068;
    public const int SCARD_E_INVALID_TARGET = -2146435067;
    public const int SCARD_E_NO_MEMORY = -2146435066;
    public const int SCARD_F_WAITED_TOO_LONG = -2146435065;
    public const int SCARD_E_INSUFFICIENT_BUFFER = -2146435064;
    public const int SCARD_E_UNKNOWN_READER = -2146435063;
    public const int SCARD_E_TIMEOUT = -2146435062;
    public const int SCARD_E_SHARING_VIOLATION = -2146435061;
    public const int SCARD_E_NO_SMARTCARD = -2146435060;
    public const int SCARD_E_UNKNOWN_CARD = -2146435059;
    public const int SCARD_E_CANT_DISPOSE = -2146435058;
    public const int SCARD_E_PROTO_MISMATCH = -2146435057;
    public const int SCARD_E_NOT_READY = -2146435056;
    public const int SCARD_E_INVALID_VALUE = -2146435055;
    public const int SCARD_E_SYSTEM_CANCELLED = -2146435054;
    public const int SCARD_F_COMM_ERROR = -2146435053;
    public const int SCARD_F_UNKNOWN_ERROR = -2146435052;
    public const int SCARD_E_INVALID_ATR = -2146435051;
    public const int SCARD_E_NOT_TRANSACTED = -2146435050;
    public const int SCARD_E_READER_UNAVAILABLE = -2146435049;
    public const int SCARD_P_SHUTDOWN = -2146435048;
    public const int SCARD_E_PCI_TOO_SMALL = -2146435047;
    public const int SCARD_E_READER_UNSUPPORTED = -2146435046;
    public const int SCARD_E_DUPLICATE_READER = -2146435045;
    public const int SCARD_E_CARD_UNSUPPORTED = -2146435044;
    public const int SCARD_E_NO_SERVICE = -2146435043;
    public const int SCARD_E_SERVICE_STOPPED = -2146435042;
    public const int SCARD_E_UNEXPECTED = -2146435041;
    public const int SCARD_E_ICC_INSTALLATION = -2146435040;
    public const int SCARD_E_ICC_CREATEORDER = -2146435039;
    public const int SCARD_E_UNSUPPORTED_FEATURE = -2146435038;
    public const int SCARD_E_DIR_NOT_FOUND = -2146435037;
    public const int SCARD_E_FILE_NOT_FOUND = -2146435036;
    public const int SCARD_E_NO_DIR = -2146435035;
    public const int SCARD_E_NO_FILE = -2146435034;
    public const int SCARD_E_NO_ACCESS = -2146435033;
    public const int SCARD_E_WRITE_TOO_MANY = -2146435032;
    public const int SCARD_E_BAD_SEEK = -2146435031;
    public const int SCARD_E_INVALID_CHV = -2146435030;
    public const int SCARD_E_UNKNOWN_RES_MNG = -2146435029;
    public const int SCARD_E_NO_SUCH_CERTIFICATE = -2146435028;
    public const int SCARD_E_CERTIFICATE_UNAVAILABLE = -2146435027;
    public const int SCARD_E_NO_READERS_AVAILABLE = -2146435026;
    public const int SCARD_E_COMM_DATA_LOST = -2146435025;
    public const int SCARD_E_NO_KEY_CONTAINER = -2146435024;
    public const int SCARD_E_SERVER_TOO_BUSY = -2146435023;
    public const int SCARD_W_UNSUPPORTED_CARD = -2146434971;
    public const int SCARD_W_UNRESPONSIVE_CARD = -2146434970;
    public const int SCARD_W_UNPOWERED_CARD = -2146434969;
    public const int SCARD_W_RESET_CARD = -2146434968;
    public const int SCARD_W_REMOVED_CARD = -2146434967;
    public const int SCARD_W_SECURITY_VIOLATION = -2146434966;
    public const int SCARD_W_WRONG_CHV = -2146434965;
    public const int SCARD_W_CHV_BLOCKED = -2146434964;
    public const int SCARD_W_EOF = -2146434963;
    public const int SCARD_W_CANCELLED_BY_USER = -2146434962;
    public const int SCARD_W_CARD_NOT_AUTHENTICATED = -2146434961;
    public const int SCARD_W_CACHE_ITEM_NOT_FOUND = -2146434960;
    public const int SCARD_W_CACHE_ITEM_STALE = -2146434959;
    public const int SCARD_W_CACHE_ITEM_TOO_BIG = -2146434958;

    /*===========================================================
    '   PROTOCOL
    '===========================================================*/
    public const int SCARD_PROTOCOL_UNDEFINED = 0x00;
    public const int SCARD_PROTOCOL_T0 = 0x01;
    public const int SCARD_PROTOCOL_T1 = 0x02;
    public const int SCARD_PROTOCOL_RAW = 0x10000;

    /*===========================================================
    '   READER STATE
    '===========================================================*/
    public const int SCARD_UNKNOWN = 0;
    public const int SCARD_ABSENT = 1;
    public const int SCARD_PRESENT = 2;
    public const int SCARD_SWALLOWED = 3;
    public const int SCARD_POWERED = 4;
    public const int SCARD_NEGOTIABLE = 5;
    public const int SCARD_SPECIFIC = 6;

    /*==========================================================================
    ' Wrapper Methods - Platform Independent
    '==========================================================================*/

    public static int SCardEstablishContext(int dwScope, int pvReserved1, int pvReserved2, ref IntPtr phContext)
    {
        if (Is64Bit)
        {
            long context64 = 0;
            int result = ModWinsCard64.SCardEstablishContext(dwScope, pvReserved1, pvReserved2, ref context64);
            phContext = new IntPtr(context64);
            return result;
        }
        else
        {
            int context32 = 0;
            int result = ModWinsCard32.SCardEstablishContext(dwScope, pvReserved1, pvReserved2, ref context32);
            phContext = new IntPtr(context32);
            return result;
        }
    }

    public static int SCardReleaseContext(IntPtr phContext)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardReleaseContext(phContext.ToInt64());
        }
        else
        {
            return ModWinsCard32.SCardReleaseContext(phContext.ToInt32());
        }
    }

    public static int SCardConnect(IntPtr hContext, string szReaderName, int dwShareMode, int dwPrefProtocol, ref IntPtr phCard, ref int ActiveProtocol)
    {
        if (Is64Bit)
        {
            long card64 = 0;
            int result = ModWinsCard64.SCardConnect(hContext.ToInt64(), szReaderName, dwShareMode, dwPrefProtocol, ref card64, ref ActiveProtocol);
            phCard = new IntPtr(card64);
            return result;
        }
        else
        {
            int card32 = 0;
            int result = ModWinsCard32.SCardConnect(hContext.ToInt32(), szReaderName, dwShareMode, dwPrefProtocol, ref card32, ref ActiveProtocol);
            phCard = new IntPtr(card32);
            return result;
        }
    }

    public static int SCardBeginTransaction(IntPtr hCard)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardBeginTransaction(hCard.ToInt64());
        }
        else
        {
            return ModWinsCard32.SCardBeginTransaction(hCard.ToInt32());
        }
    }

    public static int SCardDisconnect(IntPtr hCard, int Disposition)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardDisconnect(hCard.ToInt64(), Disposition);
        }
        else
        {
            return ModWinsCard32.SCardDisconnect(hCard.ToInt32(), Disposition);
        }
    }

    public static int SCardListReaderGroups(IntPtr hContext, ref string mzGroups, ref int pcchGroups)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardListReaderGroups(hContext.ToInt64(), ref mzGroups, ref pcchGroups);
        }
        else
        {
            return ModWinsCard32.SCardListReaderGroups(hContext.ToInt32(), ref mzGroups, ref pcchGroups);
        }
    }

    public static int SCardListReaders(IntPtr hContext, byte[] Groups, byte[] Readers, ref int pcchReaders)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardListReaders(hContext.ToInt64(), Groups, Readers, ref pcchReaders);
        }
        else
        {
            return ModWinsCard32.SCardListReaders(hContext.ToInt32(), Groups, Readers, ref pcchReaders);
        }
    }

    public static int SCardStatus(IntPtr hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref int Protocol, byte[] ATR, ref IntPtr ATRLen)
    {
        if (Is64Bit)
        {
            long atrLen64 = ATRLen.ToInt64();
            int result = ModWinsCard64.SCardStatus(hCard.ToInt64(), szReaderName, ref pcchReaderLen, ref State, ref Protocol, ATR, ref atrLen64);
            ATRLen = new IntPtr(atrLen64);
            return result;
        }
        else
        {
            int atrLen32 = ATRLen.ToInt32();
            int result = ModWinsCard32.SCardStatus(hCard.ToInt32(), szReaderName, ref pcchReaderLen, ref State, ref Protocol, ATR, ref atrLen32);
            ATRLen = new IntPtr(atrLen32);
            return result;
        }
    }

    public static int SCardEndTransaction(IntPtr hCard, int Disposition)
    {
        if (Is64Bit)
        {
            return ModWinsCard64.SCardEndTransaction(hCard.ToInt64(), Disposition);
        }
        else
        {
            return ModWinsCard32.SCardEndTransaction(hCard.ToInt32(), Disposition);
        }
    }

    public static int SCardState(IntPtr hCard, ref IntPtr State, ref IntPtr Protocol, byte[] ATR, ref IntPtr ATRLen)
    {
        if (Is64Bit)
        {
            long state64 = State.ToInt64();
            long protocol64 = Protocol.ToInt64();
            long atrLen64 = ATRLen.ToInt64();
            int result = ModWinsCard64.SCardState(hCard.ToInt64(), ref state64, ref protocol64, ATR, ref atrLen64);
            State = new IntPtr(state64);
            Protocol = new IntPtr(protocol64);
            ATRLen = new IntPtr(atrLen64);
            return result;
        }
        else
        {
            int state32 = State.ToInt32();
            int protocol32 = Protocol.ToInt32();
            int atrLen32 = ATRLen.ToInt32();
            int result = ModWinsCard32.SCardState(hCard.ToInt32(), ref state32, ref protocol32, ATR, ref atrLen32);
            State = new IntPtr(state32);
            Protocol = new IntPtr(protocol32);
            ATRLen = new IntPtr(atrLen32);
            return result;
        }
    }

    public static int SCardTransmit(IntPtr hCard, ref SCARD_IO_REQUEST pioSendRequest, byte[] SendBuff, IntPtr SendBuffLen, ref SCARD_IO_REQUEST pioRecvRequest, byte[] RecvBuff, ref IntPtr RecvBuffLen)
    {
        if (Is64Bit)
        {
            var sendReq64 = new ModWinsCard64.SCARD_IO_REQUEST { dwProtocol = pioSendRequest.dwProtocol, cbPciLength = pioSendRequest.cbPciLength };
            var recvReq64 = new ModWinsCard64.SCARD_IO_REQUEST { dwProtocol = pioRecvRequest.dwProtocol, cbPciLength = pioRecvRequest.cbPciLength };
            long recvLen64 = RecvBuffLen.ToInt64();
            int result = ModWinsCard64.SCardTransmit(hCard.ToInt64(), ref sendReq64, SendBuff, SendBuffLen.ToInt64(), ref recvReq64, RecvBuff, ref recvLen64);
            RecvBuffLen = new IntPtr(recvLen64);
            pioSendRequest = new SCARD_IO_REQUEST { dwProtocol = sendReq64.dwProtocol, cbPciLength = sendReq64.cbPciLength };
            pioRecvRequest = new SCARD_IO_REQUEST { dwProtocol = recvReq64.dwProtocol, cbPciLength = recvReq64.cbPciLength };
            return result;
        }
        else
        {
            var sendReq32 = new ModWinsCard32.SCARD_IO_REQUEST { dwProtocol = pioSendRequest.dwProtocol, cbPciLength = pioSendRequest.cbPciLength };
            var recvReq32 = new ModWinsCard32.SCARD_IO_REQUEST { dwProtocol = pioRecvRequest.dwProtocol, cbPciLength = pioRecvRequest.cbPciLength };
            int recvLen32 = RecvBuffLen.ToInt32();
            int result = ModWinsCard32.SCardTransmit(hCard.ToInt32(), ref sendReq32, SendBuff, SendBuffLen.ToInt32(), ref recvReq32, RecvBuff, ref recvLen32);
            RecvBuffLen = new IntPtr(recvLen32);
            pioSendRequest = new SCARD_IO_REQUEST { dwProtocol = sendReq32.dwProtocol, cbPciLength = sendReq32.cbPciLength };
            pioRecvRequest = new SCARD_IO_REQUEST { dwProtocol = recvReq32.dwProtocol, cbPciLength = recvReq32.cbPciLength };
            return result;
        }
    }

    public static int SCardGetStatusChange(IntPtr hContext, int TimeOut, ref SCARD_READERSTATE ReaderState, int ReaderCount)
    {
        if (Is64Bit)
        {
            var state64 = new ModWinsCard64.SCARD_READERSTATE
            {
                RdrName = ReaderState.RdrName,
                UserData = ReaderState.UserData.ToInt64(),
                RdrCurrState = ReaderState.RdrCurrState,
                RdrEventState = ReaderState.RdrEventState,
                ATRLength = ReaderState.ATRLength,
                ATRValue = ReaderState.ATRValue
            };
            int result = ModWinsCard64.SCardGetStatusChange(hContext.ToInt64(), TimeOut, ref state64, ReaderCount);
            ReaderState = new SCARD_READERSTATE
            {
                RdrName = state64.RdrName,
                UserData = new IntPtr(state64.UserData),
                RdrCurrState = state64.RdrCurrState,
                RdrEventState = state64.RdrEventState,
                ATRLength = state64.ATRLength,
                ATRValue = state64.ATRValue
            };
            return result;
        }
        else
        {
            var state32 = new ModWinsCard32.SCARD_READERSTATE
            {
                RdrName = ReaderState.RdrName,
                UserData = ReaderState.UserData.ToInt32(),
                RdrCurrState = ReaderState.RdrCurrState,
                RdrEventState = ReaderState.RdrEventState,
                ATRLength = ReaderState.ATRLength,
                ATRValue = ReaderState.ATRValue
            };
            int result = ModWinsCard32.SCardGetStatusChange(hContext.ToInt32(), TimeOut, ref state32, ReaderCount);
            ReaderState = new SCARD_READERSTATE
            {
                RdrName = state32.RdrName,
                UserData = new IntPtr(state32.UserData),
                RdrCurrState = state32.RdrCurrState,
                RdrEventState = state32.RdrEventState,
                ATRLength = state32.ATRLength,
                ATRValue = state32.ATRValue
            };
            return result;
        }
    }

    public static string GetScardErrMsg(int ReturnCode)
    {
        return ModWinsCard64.GetScardErrMsg(ReturnCode);
    }

    public static bool IsRunning64Bit()
    {
        return Is64Bit;
    }

    public static string GetPlatformInfo()
    {
        return Is64Bit ? "64-bit" : "32-bit";
    }
}
