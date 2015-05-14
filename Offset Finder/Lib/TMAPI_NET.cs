using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Offset_Finder
{
    public class PS3TMAPI
    {
        #region  PS3TMAPI Vars
        private static EnumerateTargetsExCallbackPriv ms_enumTargetsExCallbackPriv = new EnumerateTargetsExCallbackPriv(PS3TMAPI.EnumTargetsExPriv);
        private static EnumerateTargetsExCallback ms_enumTargetsExCallback = (EnumerateTargetsExCallback)null;
        private static object ms_enumTargetsExUserData = (object)null;
        private static Dictionary<TTYChannel, TTYCallbackAndUserData> ms_userTtyCallbacks = new Dictionary<TTYChannel, TTYCallbackAndUserData>(1);
        private static Dictionary<int, PadPlaybackCallbackAndUserData> ms_userPadPlaybackCallbacks = new Dictionary<int, PS3TMAPI.PadPlaybackCallbackAndUserData>(1);
        private static Dictionary<int, PadCaptureCallbackAndUserData> ms_userPadCaptureCallbacks = new Dictionary<int, PS3TMAPI.PadCaptureCallbackAndUserData>(1);
        private static CustomProtocolCallbackPriv ms_customProtoCallbackPriv = new CustomProtocolCallbackPriv(PS3TMAPI.CustomProtocolHandler);
        private static Dictionary<CustomProtocolId, CusProtoCallbackAndUserData> ms_userCustomProtoCallbacks = new Dictionary<PS3TMAPI.CustomProtocolId, PS3TMAPI.CusProtoCallbackAndUserData>(1);
        private static Dictionary<int, FtpCallbackAndUserData> ms_userFtpCallbacks = new Dictionary<int, PS3TMAPI.FtpCallbackAndUserData>(1);
        private static Dictionary<int, FileTraceCallbackAndUserData> ms_userFileTraceCallbacks = new Dictionary<int, PS3TMAPI.FileTraceCallbackAndUserData>(1);
        private static Dictionary<int, TargetCallbackAndUserData> ms_userTargetCallbacks = new Dictionary<int, PS3TMAPI.TargetCallbackAndUserData>(1);
        private static HandleEventCallbackPriv ms_eventHandlerWrapper = new HandleEventCallbackPriv(PS3TMAPI.EventHandlerWrapper);
        public const uint AllTTYStreams = 4294967295U;
        public const uint DefaultProcessPriority = 999U;
        public const uint DefaultProtocolPriority = 128U;

        public static int Target = 0xFF;
        public static bool AssemblyLoaded = true;
        private static bool connection = false;
        public static uint ProcessID;

        #region  Flag Vars
        // PPU Register Definitions.
        public enum PpuGeneralPurposeRegisters
        {//! PPU general purpose register.
          SNPS3_gpr_0	=	0x00,
          SNPS3_gpr_1	=	0x01,
          SNPS3_gpr_2	=	0x02,
          SNPS3_gpr_3	=	0x03,
          SNPS3_gpr_4	=	0x04,
          SNPS3_gpr_5	=	0x05,
          SNPS3_gpr_6	=	0x06,
          SNPS3_gpr_7	=	0x07,
          SNPS3_gpr_8	=	0x08,
          SNPS3_gpr_9	=	0x09,
          SNPS3_gpr_10	=	0x0a,
          SNPS3_gpr_11	=	0x0b,
          SNPS3_gpr_12	=	0x0c,
          SNPS3_gpr_13	=	0x0d,
          SNPS3_gpr_14	=	0x0e,
          SNPS3_gpr_15	=	0x0f,
          SNPS3_gpr_16	=	0x10,
          SNPS3_gpr_17	=	0x11,
          SNPS3_gpr_18	=	0x12,
          SNPS3_gpr_19	=	0x13,
          SNPS3_gpr_20	=	0x14,
          SNPS3_gpr_21	=	0x15,
          SNPS3_gpr_22	=	0x16,
          SNPS3_gpr_23	=	0x17,
          SNPS3_gpr_24	=	0x18,
          SNPS3_gpr_25	=	0x19,
          SNPS3_gpr_26	=	0x1a,
          SNPS3_gpr_27	=	0x1b,
          SNPS3_gpr_28	=	0x1c,
          SNPS3_gpr_29	=	0x1d,
          SNPS3_gpr_30	=	0x1e,
          SNPS3_gpr_31	=	0x1f,
        }
        public enum PpuFloatingPointRegisters
        {//! PPU floating point register.
          SNPS3_fpr_0	=	0x20,
          SNPS3_fpr_1	=	0x21,
          SNPS3_fpr_2	=	0x22,
          SNPS3_fpr_3	=	0x23,
          SNPS3_fpr_4	=	0x24,
          SNPS3_fpr_5	=	0x25,
          SNPS3_fpr_6	=	0x26,
          SNPS3_fpr_7	=	0x27,
          SNPS3_fpr_8	=	0x28,
          SNPS3_fpr_9	=	0x29,
          SNPS3_fpr_10	=	0x2A,
          SNPS3_fpr_11	=	0x2B,
          SNPS3_fpr_12	=	0x2C,
          SNPS3_fpr_13	=	0x2D,
          SNPS3_fpr_14	=	0x2E,
          SNPS3_fpr_15	=	0x2F,
          SNPS3_fpr_16	=	0x30,
          SNPS3_fpr_17	=	0x31,
          SNPS3_fpr_18	=	0x32,
          SNPS3_fpr_19	=	0x33,
          SNPS3_fpr_20	=	0x34,
          SNPS3_fpr_21	=	0x35,
          SNPS3_fpr_22	=	0x36,
          SNPS3_fpr_23	=	0x37,
          SNPS3_fpr_24	=	0x38,
          SNPS3_fpr_25	=	0x39,
          SNPS3_fpr_26	=	0x3A,
          SNPS3_fpr_27	=	0x3B,
          SNPS3_fpr_28	=	0x3C,
          SNPS3_fpr_29	=	0x3D,
          SNPS3_fpr_30	=	0x3E,
          SNPS3_fpr_31	=	0x3F,
        }
        public enum PpuRegisters
        {//! PPU register.
         SNPS3_pc = 0x40,
         SNPS3_cr =	0x41,
         SNPS3_lr =	0x42,
         SNPS3_ctr = 0x43,
         SNPS3_xer = 0x44,
         SNPS3_fpscr = 0x45,
         SNPS3_vscr	= 0x46,
         SNPS3_vrsave =	0x47,
         SNPS3_msr	= 0x48,

        SNPS3_vmx_0	= 0x60,
        SNPS3_vmx_1	= 0x61,
        SNPS3_vmx_2 = 0x62,
        SNPS3_vmx_3	= 0x63,
        SNPS3_vmx_4 = 0x64,
        SNPS3_vmx_5	= 0x65,
        SNPS3_vmx_6	= 0x66,
        SNPS3_vmx_7	= 0x67,
        SNPS3_vmx_8	= 0x68,
        SNPS3_vmx_9	= 0x69,
        SNPS3_vmx_10 = 0x6A,
        SNPS3_vmx_11 = 0x6B,
        SNPS3_vmx_12 = 0x6C,
        SNPS3_vmx_13 = 0x6D,
        SNPS3_vmx_14 = 0x6E,
        SNPS3_vmx_15 = 0x6F,
        SNPS3_vmx_16 = 0x70,
        SNPS3_vmx_17 = 0x71,
        SNPS3_vmx_18 = 0x72,
        SNPS3_vmx_19 = 0x73,
        SNPS3_vmx_20 = 0x74,
        SNPS3_vmx_21 = 0x75,
        SNPS3_vmx_22 = 0x76,
        SNPS3_vmx_23 = 0x77,
        SNPS3_vmx_24 = 0x78,
        SNPS3_vmx_25 = 0x79,
        SNPS3_vmx_26 = 0x7A,
        SNPS3_vmx_27 = 0x7B,
        SNPS3_vmx_28 = 0x7C,
        SNPS3_vmx_29 = 0x7D,
        SNPS3_vmx_30 = 0x7E,
        SNPS3_vmx_31 = 0x7F,
      }

        public enum SNRESULT
        {
            SN_E_ERROR = -2147483648,
            SN_E_COMMS_EVENT_MISMATCHED_ERR = -39,
            SN_E_CONNECTED = -38,
            SN_E_PROTOCOL_ALREADY_REGISTERED = -37,
            SN_E_COMMAND_CANCELLED = -36,
            SN_E_CONNECT_TO_GAMEPORT_FAILED = -35,
            SN_E_MODULE_NOT_FOUND = -34,
            SN_E_CHECK_TARGET_CONFIGURATION = -33,
            SN_E_LICENSE_ERROR = -32,
            SN_E_LOAD_MODULE_FAILED = -31,
            SN_E_NOT_SUPPORTED_IN_SDK_VERSION = -30,
            SN_E_FILE_ERROR = -29,
            SN_E_BAD_ALIGN = -28,
            SN_E_DEPRECATED = -27,
            SN_E_DATA_TOO_LONG = -26,
            SN_E_INSUFFICIENT_DATA = -25,
            SN_E_EXISTING_CALLBACK = -24,
            SN_E_DECI_ERROR = -23,
            SN_E_BUSY = -22,
            SN_E_BAD_PARAM = -21,
            SN_E_NO_SEL = -20,
            SN_E_NO_TARGETS = -19,
            SN_E_BAD_MEMSPACE = -18,
            SN_E_TARGET_RUNNING = -17,
            SN_E_DLL_NOT_INITIALISED = -15,
            SN_E_TM_VERSION = -14,
            SN_E_NOT_LISTED = -13,
            SN_E_OUT_OF_MEM = -12,
            SN_E_BAD_UNIT = -11,
            SN_E_LOAD_ELF_FAILED = -10,
            SN_E_TARGET_IN_USE = -9,
            SN_E_HOST_NOT_FOUND = -8,
            SN_E_TIMEOUT = -7,
            SN_E_TM_COMMS_ERR = -6,
            SN_E_COMMS_ERR = -5,
            SN_E_NOT_CONNECTED = -4,
            SN_E_BAD_TARGET = -3,
            SN_E_TM_NOT_RUNNING = -2,
            SN_E_NOT_IMPL = -1,
            SN_S_OK = 0,
            SN_S_PENDING = 1,
            SN_S_NO_MSG = 3,
            SN_S_TM_VERSION = 4,
            SN_S_REPLACED = 5,
            SN_S_NO_ACTION = 6,
            SN_S_TARGET_STILL_REGISTERED = 7,
        }

        public enum ConnectStatus
        {
            Connected,
            Connecting,
            NotConnected,
            InUse,
            Unavailable,
        }

        public delegate int EnumerateTargetsCallback(int target);

        public delegate int EnumerateTargetsExCallback(int target, object userData);

        private delegate int EnumerateTargetsExCallbackPriv(int target, IntPtr unused);

        public struct BDInfo
        {
            public uint bdemu_data_size;
            public byte bdemu_total_entry;
            public byte bdemu_selected_index;
            public byte image_index;
            public byte image_type;
            public string image_file_name;
            public ulong image_file_size;
            public string image_product_code;
            public string image_producer;
            public string image_author;
            public string image_date;
            public uint image_sector_layer0;
            public uint image_sector_layer1;
            public string image_memorandum;
        }

        private struct DirEntryPriv
        {
            public uint Type;
            public uint Mode;
            public PS3TMAPI.Time AccessTime;
            public PS3TMAPI.Time ModifiedTime;
            public PS3TMAPI.Time CreateTime;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Name;
        }

        private struct DirEntryExPriv
        {
            public uint Type;
            public uint Mode;
            public ulong AccessTimeUTC;
            public ulong ModifiedTimeUTC;
            public ulong CreateTimeUTC;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] Name;
        }

        [Flags]
        public enum BootParameter : ulong
        {
            Default = 0UL,
            SystemMode = 17UL,
            ReleaseMode = 1UL,
            DebugMode = 16UL,
            MemSizeConsole = 2UL,
            BluRayEmuOff = 4UL,
            HDDSpeedBluRayEmu = 8UL,
            BluRayEmuUSB = 32UL,
            HostFSTarget = 64UL,
            DualNIC = 128UL,
        }

        [Flags]
        public enum BootParameterMask : ulong
        {
            BootMode = 17UL,
            Memsize = 2UL,
            BlurayEmulation = 4UL,
            HDDSpeed = 8UL,
            BlurayEmuSelect = 32UL,
            HostFS = 64UL,
            NIC = 128UL,
            All = NIC | HostFS | BlurayEmuSelect | HDDSpeed | BlurayEmulation | Memsize | BootMode,
        }

        [Flags]
        public enum ResetParameter : ulong
        {
            Soft = 0UL,
            Hard = 1UL,
            Quick = 2UL,
            ResetEx = 9223372036854775808UL,
        }

        [Flags]
        public enum ResetParameterMask : ulong
        {
            All = 9223372036854775811UL,
        }

        [Flags]
        public enum SystemParameter : ulong
        {
            TargetModel60GB = 281474976710656UL,
            TargetModel20GB = 562949953421312UL,
            ReleaseCheckMode = 140737488355328UL,
        }

        [Flags]
        public enum SystemParameterMask : ulong
        {
            TargetModel = 71776119061217280UL,
            ReleaseCheck = 140737488355328UL,
            All = ReleaseCheck | TargetModel,
        }

        [Flags]
        public enum TargetInfoFlag : uint
        {
            TargetID = 1U,
            Name = 2U,
            Info = 4U,
            HomeDir = 8U,
            FileServingDir = 16U,
            Boot = 32U,
        }

        public struct TargetInfo
        {
            public TargetInfoFlag Flags;
            public int Target;
            [MarshalAs(UnmanagedType.LPStr)]
            public string Name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string Type;
            [MarshalAs(UnmanagedType.LPStr)]
            public string Info;
            [MarshalAs(UnmanagedType.LPStr)]
            public string HomeDir;
            [MarshalAs(UnmanagedType.LPStr)]
            public string FSDir;
            public BootParameter Boot;
        }

        public struct TargetType
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TCPIPConnectProperties
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string IPAddress;
            public uint Port;
        }

        [Flags]
        public enum SystemInfoFlag : uint
        {
            SDKVersion = 1U,
            TimebaseFreq = 2U,
            RTSDKVersion = 4U,
            TotalSystemMem = 8U,
            AvailableSysMem = 16U,
            DCMBufferSize = 32U,
        }

        public struct SystemInfo
        {
            public uint CellSDKVersion;
            public ulong TimebaseFrequency;
            public uint CellRuntimeSDKVersion;
            public uint TotalSystemMemory;
            public uint AvailableSystemMemory;
            public uint DCMBufferSize;
        }

        [Flags]
        public enum ExtraLoadFlag : ulong
        {
            EnableLv2ExceptionHandler = 1UL,
            EnableRemotePlay = 2UL,
            EnableGCMDebug = 4UL,
            LoadLibprofSPRXAutomatically = 8UL,
            EnableCoreDump = 16UL,
            EnableAccForRemotePlay = 32UL,
            EnableHUDRSXTools = 64UL,
            EnableMAT = 128UL,
            EnableMiscSettings = 9223372036854775808UL,
            GameAttributeInviteMessage = 256UL,
            GameAttributeCustomMessage = 512UL,
            LoadingPatch = 4096UL,
        }

        [Flags]
        public enum ExtraLoadFlagMask : ulong
        {
            GameAttributeMessageMask = 3840UL,
            All = 9223372036854783999UL,
            OverrideTVGUIMask = 9223372036854775808UL,
        }

        public struct TTYStream
        {
            public uint Index;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Name;
        }

        private enum SNPS3_TM_TIMEOUT
        {
            DEFAULT_TIMEOUT,
            RESET_TIMEOUT,
            CONNECT_TIMEOUT,
            LOAD_TIMEOUT,
            GET_STATUS_TIMEOUT,
            RECONNECT_TIMEOUT,
            GAMEPORT_TIMEOUT,
            GAMEEXIT_TIMEOUT,
        }

        public enum TimeoutType
        {
            Default,
            Reset,
            Connect,
            Load,
            GetStatus,
            Reconnect,
            GamePort,
            GameExit,
        }

        public delegate void TTYCallback(int target, uint streamID, PS3TMAPI.SNRESULT res, string data, object userData);

        private class TTYCallbackAndUserData
        {
            public TTYCallback m_callback;
            public object m_userData;
        }

        private struct TTYChannel
        {
            public readonly int Target;
            public readonly uint Channel;

            public TTYChannel(int target, uint channel)
            {
                this.Target = target;
                this.Channel = channel;
            }
        }

        public enum UnitType
        {
            PPU,
            SPU,
            SPURAW,
        }

        public enum UnitStatus : uint
        {
            Unknown,
            Running,
            Stopped,
            Signalled,
            Resetting,
            Missing,
            Reset,
            NotConnected,
            Connected,
            StatusChange,
        }

        [Flags]
        public enum LoadFlag : uint
        {
            EnableDebugging = 1U,
            UseELFPriority = 256U,
            UseELFStackSize = 512U,
            WaitBDMounted = 8192U,
            PPUNotDebug = 65536U,
            SPUNotDebug = 131072U,
            IgnoreDefaults = 2147483648U,
            ParamSFOUseELFDir = 1048576U,
            ParamSFOUseCustomDir = 2097152U,
        }

        public enum ProcessStatus : uint
        {
            Creating = 1U,
            Ready = 2U,
            Exited = 3U,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessInfoHdr
        {
            public PS3TMAPI.ProcessStatus Status;
            public uint NumPPUThreads;
            public uint NumSPUThreads;
            public uint ParentProcessID;
            public ulong MaxMemorySize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string ELFPath;
        }

        public struct ProcessInfo
        {
            public ProcessInfoHdr Hdr;
            public ulong[] ThreadIDs;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ExtraProcessInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadParams
        {
            public ulong Version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ulong[] Data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProcessLoadInfo
        {
            public uint InfoValid;
            public uint DebugFlags;
            public ProcessLoadParams LoadInfo;
        }

        public struct ModuleInfoHdr
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public sbyte[] Version;
            public uint Attribute;
            public uint StartEntry;
            public uint StopEntry;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string ELFName;
            public uint NumSegments;
        }

        public struct PRXSegment
        {
            public ulong Base;
            public ulong FileSize;
            public ulong MemSize;
            public ulong Index;
            public ulong ELFType;
        }

        public struct ModuleInfo
        {
            public ModuleInfoHdr Hdr;
            public PRXSegment[] Segments;
        }

        public struct PRXSegmentEx
        {
            public ulong Base;
            public ulong FileSize;
            public ulong MemSize;
            public ulong Index;
            public ulong ELFType;
            public ulong Flags;
            public ulong Align;
        }

        public struct ModuleInfoEx
        {
            public ModuleInfoHdr Hdr;
            public PRXSegmentEx[] Segments;
        }

        public struct MSELFInfo
        {
            public ulong MSELFFileOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Reserved;
        }

        public struct ExtraModuleInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint[] PPUGUIDs;
        }

        public enum PPUThreadState
        {
            Idle,
            Runnable,
            OnProc,
            Sleep,
            Suspended,
            SleepSuspended,
            Stop,
            Zombie,
            Deleted,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BDInfoPriv
        {
            public uint bdemu_data_size;
            public byte bdemu_total_entry;
            public byte bdemu_selected_index;
            public byte image_index;
            public byte image_type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] image_file_name;
            public ulong image_file_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_product_code;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_producer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_author;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] image_date;
            public uint image_sector_layer0;
            public uint image_sector_layer1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] image_memorandum;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PPUThreadInfoPriv
        {
            public ulong ThreadID;
            public uint Priority;
            public uint State;
            public ulong StackAddress;
            public ulong StackSize;
            public uint ThreadNameLen;
        }

        public struct PPUThreadInfo
        {
            public ulong ThreadID;
            public uint Priority;
            public PS3TMAPI.PPUThreadState State;
            public ulong StackAddress;
            public ulong StackSize;
            public string ThreadName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PPUThreadInfoExPriv
        {
            public ulong ThreadId;
            public uint Priority;
            public uint BasePriority;
            public uint State;
            public ulong StackAddress;
            public ulong StackSize;
            public uint ThreadNameLen;
        }

        public struct PPUThreadInfoEx
        {
            public ulong ThreadID;
            public uint Priority;
            public uint BasePriority;
            public PS3TMAPI.PPUThreadState State;
            public ulong StackAddress;
            public ulong StackSize;
            public string ThreadName;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SpuThreadInfoPriv
        {
            public uint ThreadGroupId;
            public uint ThreadId;
            public uint FilenameLen;
            public uint ThreadNameLen;
        }

        public struct SPUThreadInfo
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public string Filename;
            public string ThreadName;
        }

        [Flags]
        public enum ELFStackSize : uint
        {
            Stack32k = 32U,
            Stack64k = 64U,
            Stack96k = Stack64k | Stack32k,
            Stack128k = 128U,
            Stack256k = 256U,
            Stack512k = 512U,
            Stack1024k = 1024U,
            StackDefault = Stack64k,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DebugThreadControlInfoPriv
        {
            public ulong ControlFlags;
            public uint NumEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ControlKeywordEntry
        {
            public uint MatchConditionFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Keyword;
        }

        public struct DebugThreadControlInfo
        {
            public ulong ControlFlags;
            public PS3TMAPI.ControlKeywordEntry[] ControlKeywords;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ProcessTreeBranchPriv
        {
            public uint ProcessId;
            public PS3TMAPI.ProcessStatus ProcessState;
            public uint NumPpuThreads;
            public uint NumSpuThreadGroups;
            public ushort ProcessFlags;
            public ushort RawSPU;
            public IntPtr PpuThreadStatuses;
            public IntPtr SpuThreadGroupStatuses;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PPUThreadStatus
        {
            public ulong ThreadID;
            public PS3TMAPI.PPUThreadState ThreadState;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SPUThreadGroupStatus
        {
            public uint ThreadGroupID;
            public PS3TMAPI.SPUThreadGroupState ThreadGroupState;
        }

        public struct ProcessTreeBranch
        {
            public uint ProcessID;
            public PS3TMAPI.ProcessStatus ProcessState;
            public ushort ProcessFlags;
            public ushort RawSPU;
            public PS3TMAPI.PPUThreadStatus[] PPUThreadStatuses;
            public PS3TMAPI.SPUThreadGroupStatus[] SPUThreadGroupStatuses;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SpuThreadGroupInfoPriv
        {
            public uint ThreadGroupId;
            public uint State;
            public uint Priority;
            public uint NumThreads;
            public uint ThreadGroupNameLen;
        }

        public enum SPUThreadGroupState : uint
        {
            NotConfigured,
            Configured,
            Ready,
            Waiting,
            Suspended,
            WaitingSuspended,
            Running,
            Stopped,
        }

        public struct SPUThreadGroupInfo
        {
            public uint ThreadGroupID;
            public PS3TMAPI.SPUThreadGroupState State;
            public uint Priority;
            public string GroupName;
            public uint[] ThreadIDs;
        }

        public enum MemoryCompressionLevel : uint
        {
            None = 0U,
            BestSpeed = 1U,
            BestCompression = 9U,
            Default = 4294967295U,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VirtualMemoryArea
        {
            public ulong Address;
            public ulong Flags;
            public ulong VSize;
            public ulong Options;
            public ulong PageFaultPPU;
            public ulong PageFaultSPU;
            public ulong PageIn;
            public ulong PageOut;
            public ulong PMemTotal;
            public ulong PMemUsed;
            public ulong Time;
            public ulong[] Pages;
        }

        public struct SyncPrimitiveCounts
        {
            public uint NumMutexes;
            public uint NumConditionVariables;
            public uint NumRWLocks;
            public uint NumLWMutexes;
            public uint NumEventQueues;
            public uint NumSemaphores;
            public uint NumLWConditionVariables;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MutexInfoPriv
        {
            public uint Id;
            public PS3TMAPI.MutexAttr Attribute;
            public ulong OwnerThreadId;
            public uint LockCounter;
            public uint ConditionRefCounter;
            public uint ConditionVarId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MutexAttr
        {
            public uint Protocol;
            public uint Recursive;
            public uint PShared;
            public uint Adaptive;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct MutexInfo
        {
            public uint ID;
            public PS3TMAPI.MutexAttr Attribute;
            public ulong OwnerThreadID;
            public uint LockCounter;
            public uint ConditionRefCounter;
            public uint ConditionVarID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct LwMutexInfoPriv
        {
            public uint Id;
            public PS3TMAPI.LWMutexAttr Attribute;
            public ulong OwnerThreadId;
            public uint LockCounter;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LWMutexAttr
        {
            public uint Protocol;
            public uint Recursive;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct LWMutexInfo
        {
            public uint ID;
            public PS3TMAPI.LWMutexAttr Attribute;
            public ulong OwnerThreadID;
            public uint LockCounter;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ConditionVarInfoPriv
        {
            public uint Id;
            public PS3TMAPI.ConditionVarAttr Attribute;
            public uint MutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ConditionVarAttr
        {
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct ConditionVarInfo
        {
            public uint ID;
            public PS3TMAPI.ConditionVarAttr Attribute;
            public uint MutexID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        private struct LwConditionVarInfoPriv
        {
            public uint Id;
            public PS3TMAPI.LWConditionVarAttr Attribute;
            public uint LwMutexId;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        public struct LWConditionVarAttr
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct LWConditionVarInfo
        {
            public uint ID;
            public PS3TMAPI.LWConditionVarAttr Attribute;
            public uint LWMutexID;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RwLockInfoPriv
        {
            public uint Id;
            public PS3TMAPI.RWLockAttr Attribute;
            public ulong OwnerThreadId;
            public uint NumWaitingReadThreads;
            public uint NumWaitAllReadThreads;
            public uint NumWaitingWriteThreads;
            public uint NumWaitAllWriteThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RWLockAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct RWLockInfo
        {
            public uint ID;
            public PS3TMAPI.RWLockAttr Attribute;
            public ulong OwnerThreadID;
            public uint NumWaitingReadThreads;
            public uint NumWaitAllReadThreads;
            public uint NumWaitingWriteThreads;
            public uint NumWaitAllWriteThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SemaphoreInfoPriv
        {
            public uint Id;
            public PS3TMAPI.SemaphoreAttr Attribute;
            public uint MaxValue;
            public uint CurrentValue;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SemaphoreAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct SemaphoreInfo
        {
            public uint ID;
            public PS3TMAPI.SemaphoreAttr Attribute;
            public uint MaxValue;
            public uint CurrentValue;
            public uint NumWaitAllThreads;
            public ulong[] WaitingThreads;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct EventQueueInfoPriv
        {
            public uint Id;
            public PS3TMAPI.EventQueueAttr Attribute;
            public ulong Key;
            public uint Size;
            public uint NumWaitingThreads;
            public uint NumWaitAllThreads;
            public uint NumReadableEvQueue;
            public uint NumReadableAllEvQueue;
            public IntPtr WaitingThreadIds;
            public IntPtr QueueEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventQueueAttr
        {
            public uint Protocol;
            public uint Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct SystemEvent
        {
            public ulong Source;
            public ulong Data1;
            public ulong Data2;
            public ulong Data3;
        }

        public struct EventQueueInfo
        {
            public uint ID;
            public PS3TMAPI.EventQueueAttr Attribute;
            public ulong Key;
            public uint Size;
            public uint NumWaitAllThreads;
            public uint NumReadableAllEvQueue;
            public ulong[] WaitingThreadIDs;
            public PS3TMAPI.SystemEvent[] QueueEntries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagWaitThread
        {
            public ulong ID;
            public ulong BitPattern;
            public uint Mode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFlagAttr
        {
            public uint Protocol;
            public uint PShared;
            public ulong Key;
            public uint Flags;
            public uint Type;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Name;
        }

        public struct EventFlagInfo
        {
            public uint ID;
            public PS3TMAPI.EventFlagAttr Attribute;
            public ulong BitPattern;
            public uint NumWaitAllThreads;
            public PS3TMAPI.EventFlagWaitThread[] WaitingThreads;
        }

        public enum PowerStatus
        {
            Off,
            On,
            Suspended,
            Unknown,
            SwitchingOn,
        }

        public struct UserMemoryStats
        {
            public uint CreatedSharedMemorySize;
            public uint AttachedSharedMemorySize;
            public uint ProcessLocalMemorySize;
            public uint ProcessLocalTextSize;
            public uint PRXTextSize;
            public uint PRXDataSize;
            public uint MiscMemorySize;
        }

        public struct GamePortIPAddressData
        {
            public uint ReturnValue;
            public uint IPAddress;
            public uint SubnetMask;
            public uint BroadcastAddress;
        }

        [Flags]
        public enum RSXProfilingFlag : ulong
        {
            UseRSXProfilingTools = 1UL,
            UseFullHUDFeatures = 2UL,
        }

        [Flags]
        public enum CoreDumpFlag : ulong
        {
            ToDevMS = 1UL,
            ToAppHome = 2UL,
            ToDevUSB = 4UL,
            ToDevHDD0 = 8UL,
            DisablePPUExceptionDetection = 36028797018963968UL,
            DisableSPUExceptionDetection = 18014398509481984UL,
            DisableRSXExceptionDetection = 9007199254740992UL,
            DisableFootSwitchDetection = 4503599627370496UL,
            DisableMemoryDump = 3489660928UL,
            EnableRestartProcess = 32768UL,
            EnableKeepRunningHandler = 8192UL,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ScatteredWrite
        {
            public uint Address;
            public byte[] Data;
        }

        public enum MATCondition : byte
        {
            Transparent,
            Write,
            ReadWrite,
            Error,
        }

        public struct MATRange
        {
            public uint StartAddress;
            public uint Size;
            public PS3TMAPI.MATCondition[] PageConditions;
        }

        public enum PadPlaybackResponse : uint
        {
            Ok = 0U,
            InvalidPacket = 2147549186U,
            InsufficientMemory = 2147549188U,
            Busy = 2147549194U,
            NoDev = 2147549229U,
        }

        public delegate void PadPlaybackCallback(int target, SNRESULT res, PadPlaybackResponse playbackResult, object userData);

        private class PadPlaybackCallbackAndUserData
        {
            public PS3TMAPI.PadPlaybackCallback m_callback;
            public object m_userData;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PadData
        {
            public uint TimeHi;
            public uint TimeLo;
            public uint Reserved0;
            public uint Reserved1;
            public byte Port;
            public byte PortStatus;
            public byte Length;
            public byte Reserved2;
            public uint Reserved3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public short[] buttons;
        }

        public delegate void PadCaptureCallback(int target, SNRESULT res, PadData[] padData, object userData);

        private class PadCaptureCallbackAndUserData
        {
            public PadCaptureCallback m_callback;
            public object m_userData;
        }

        [Flags]
        public enum VRAMCaptureFlag : ulong
        {
            Enabled = 1UL,
            Disabled = 0UL,
        }

        public class VRAMInfo
        {
            public ulong BPAddress;
            public ulong TopAddressPointer;
            public uint Width;
            public uint Height;
            public uint Pitch;
            public byte Colour;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VramInfoPriv
        {
            public ulong BpAddress;
            public ulong TopAddressPointer;
            public uint Width;
            public uint Height;
            public uint Pitch;
            public byte Colour;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PS3Protocol
        {
            public uint Protocol;
            public uint Port;
            public uint LPARDesc;
        }

        private struct PS3ProtocolPriv
        {
            public readonly uint Protocol;
            public readonly uint Port;

            public PS3ProtocolPriv(uint protocol, uint port)
            {
                this.Protocol = port;
                this.Port = protocol;
            }
        }

        private struct CustomProtocolId
        {
            public readonly int Target;
            public readonly PS3ProtocolPriv Protocol;

            public CustomProtocolId(int target, PS3ProtocolPriv protocol)
            {
                this.Target = target;
                this.Protocol = protocol;
            }
        }

        private delegate void CustomProtocolCallbackPriv(int target, PS3TMAPI.PS3Protocol protocol, IntPtr unmanagedBuf, uint length, IntPtr userData);

        public delegate void CustomProtocolCallback(int target, PS3TMAPI.PS3Protocol protocol, byte[] data, object userData);

        private class CusProtoCallbackAndUserData
        {
            public CustomProtocolCallback m_callback;
            public object m_userData;
        }

        [Flags]
        public enum FileServingEventFlag : ulong
        {
            Create = 1UL,
            Close = 4UL,
            Read = 8UL,
            Write = 16UL,
            Seek = 32UL,
            Delete = 64UL,
            Rename = 128UL,
            SetAttr = 256UL,
            GetAttr = 512UL,
            SetTime = 1024UL,
            MKDir = 2048UL,
            RMDir = 4096UL,
            OpenDir = 8192UL,
            CloseDir = 16384UL,
            ReadDir = 32768UL,
            Truncate = 65536UL,
            FGetAttr64 = 131072UL,
            GetAttr64 = 262144UL,
            All = GetAttr64 | FGetAttr64 | Truncate | ReadDir | CloseDir | OpenDir | RMDir | MKDir | SetTime | GetAttr | SetAttr | Rename | Delete | Seek | Write | Read | Close | Create,
        }

        public enum FileTransferNotificationType : uint
        {
            Progress = 0U,
            Finish = 1U,
            Skipped = 2U,
            Cancelled = 3U,
            Error = 4U,
            Pending = 5U,
            Unknown = 6U,
            RefreshList = 2147483648U,
        }

        public struct FTPNotification
        {
            public FileTransferNotificationType Type;
            public uint TransferID;
            public ulong BytesTransferred;
        }

        public delegate void FTPEventCallback(int target, SNRESULT res, FTPNotification[] ftpNotifications, object userData);

        private class FtpCallbackAndUserData
        {
            public FTPEventCallback m_callback;
            public object m_userData;
        }

        public enum FileTraceType
        {
            GetBlockSize = 1,
            Stat = 2,
            WidgetStat = 3,
            Unlink = 4,
            WidgetUnlink = 5,
            RMDir = 6,
            WidgetRMDir = 7,
            Rename = 14,
            WidgetRename = 15,
            Truncate = 18,
            TruncateNoAlloc = 19,
            Truncate2 = 20,
            Truncate2NoInit = 21,
            OpenDir = 24,
            WidgetOpenDir = 25,
            CHMod = 26,
            MkDir = 27,
            UTime = 29,
            Open = 33,
            WidgetOpen = 34,
            Close = 35,
            CloseDir = 36,
            FSync = 37,
            ReadDir = 38,
            FStat = 39,
            FGetBlockSize = 40,
            Read = 47,
            Write = 48,
            GetDirEntries = 49,
            ReadOffset = 50,
            WriteOffset = 51,
            FTruncate = 52,
            FTruncateNoAlloc = 53,
            LSeek = 56,
            SetIOBuffer = 57,
            OfflineEnd = 9999,
        }

        public enum FileTraceNotificationStatus
        {
            Processed,
            Received,
            Waiting,
            Processing,
            Suspended,
            Finished,
        }

        public struct FileTraceLogData
        {
            public FileTraceLogType1 LogType1;
            public FileTraceLogType2 LogType2;
            public FileTraceLogType3 LogType3;
            public FileTraceLogType4 LogType4;
            public FileTraceLogType6 LogType6;
            public FileTraceLogType8 LogType8;
            public FileTraceLogType9 LogType9;
            public FileTraceLogType10 LogType10;
            public FileTraceLogType11 LogType11;
            public FileTraceLogType12 LogType12;
            public FileTraceLogType13 LogType13;
            public FileTraceLogType14 LogType14;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType1
        {
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType2
        {
            public string Path1;
            public string Path2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType3
        {
            public ulong Arg;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType4
        {
            public uint Mode;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType6
        {
            public ulong Arg1;
            public ulong Arg2;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceProcessInfo
        {
            public ulong VFSID;
            public ulong FD;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType8
        {
            public FileTraceProcessInfo ProcessInfo;
            public uint Arg1;
            public uint Arg2;
            public uint Arg3;
            public uint Arg4;
            public byte[] VArg;
            public string Path;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType9
        {
            public FileTraceProcessInfo ProcessInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType10
        {
            public FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Address;
            public uint TxSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType11
        {
            public FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Address;
            public ulong Offset;
            public uint TxSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType12
        {
            public FileTraceProcessInfo ProcessInfo;
            public ulong TargetSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType13
        {
            public FileTraceProcessInfo ProcessInfo;
            public uint Size;
            public ulong Offset;
            public ulong CurPos;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceLogType14
        {
            public FileTraceProcessInfo ProcessInfo;
            public uint MaxSize;
            public uint Page;
            public uint ContainerID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileTraceEvent
        {
            public ulong SerialID;
            public PS3TMAPI.FileTraceType TraceType;
            public PS3TMAPI.FileTraceNotificationStatus Status;
            public uint ProcessID;
            public uint ThreadID;
            public ulong TimeBaseStartOfTrace;
            public ulong TimeBase;
            public byte[] BackTraceData;
            public FileTraceLogData LogData;
        }

        public delegate void FileTraceCallback(int target, SNRESULT res, FileTraceEvent fileTraceEvent, object userData);

        private class FileTraceCallbackAndUserData
        {
            public FileTraceCallback m_callback;
            public object m_userData;
        }

        private struct FileTransferInfoPriv
        {
            public uint TransferId;
            public uint Status;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string SourcePath;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1056)]
            public string DestinationPath;
            public ulong Size;
            public ulong BytesRead;
        }

        public enum FileTransferStatus : uint
        {
            Pending = 1U,
            Failed = 2U,
            Succeeded = 4U,
            Skipped = 8U,
            InProgress = 16U,
            Cancelled = 32U,
        }

        public struct FileTransferInfo
        {
            public uint TransferID;
            public PS3TMAPI.FileTransferStatus Status;
            public string SourcePath;
            public string DestinationPath;
            public ulong Size;
            public ulong BytesRead;
        }

        public struct Time
        {
            private int Sec;
            private int Min;
            private int Hour;
            private int MDay;
            private int Mon;
            private int Year;
            private int WDay;
            private int YDay;
            private int IsDST;
        }

        public enum DirEntryType : uint
        {
            Unknown,
            Directory,
            Regular,
            Symlink,
        }

        public struct DirEntry
        {
            public DirEntryType Type;
            public uint Mode;
            public Time AccessTime;
            public Time ModifiedTime;
            public Time CreateTime;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }

        public struct DirEntryEx
        {
            public DirEntryType Type;
            public uint Mode;
            public ulong AccessTimeUTC;
            public ulong ModifiedTimeUTC;
            public ulong CreateTimeUTC;
            public ulong Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }

        public struct TargetTimezone
        {
            public int Timezone;
            public int DST;
        }

        public enum ChModFilePermission : uint
        {
            ReadOnly = 256U,
            ReadWrite = 384U,
        }

        public enum LogCategory : uint
        {
            Off = 0U,
            All = 4294967295U,
        }

        public enum TargetEventType : uint
        {
            UnitStatusChange = 0U,
            ResetStarted = 1U,
            ResetEnd = 2U,
            Details = 4U,
            ModuleLoad = 5U,
            ModuleRunning = 6U,
            ModuleDoneRemove = 7U,
            ModuleDoneResident = 8U,
            ModuleStopped = 9U,
            ModuleStoppedRemove = 10U,
            PowerStatusChange = 11U,
            TTYStreamAdded = 12U,
            TTYStreamDeleted = 13U,
            TargetSpecific = 2147483648U,
        }

        public struct TGTEventUnitStatusChangeData
        {
            public PS3TMAPI.UnitType Unit;
            public PS3TMAPI.UnitStatus Status;
        }

        public struct TGTEventDetailsData
        {
            public uint Flags;
        }

        public struct TGTEventModuleEventData
        {
            public uint Unit;
            public uint ModuleID;
        }

        public struct TargetEventData
        {
            public TGTEventUnitStatusChangeData UnitStatusChangeData;
            public TGTEventDetailsData DetailsData;
            public TGTEventModuleEventData ModuleEventData;
        }

        public struct TargetEvent
        {
            public uint TargetID;
            public TargetEventType Type;
            public TargetEventData EventData;
            public TargetSpecificEvent TargetSpecific;
        }

        public delegate void TargetEventCallback(int target, SNRESULT res, TargetEvent[] targetEventList, object userData);

        private class TargetCallbackAndUserData
        {
            public TargetEventCallback m_callback;
            public object m_userData;
        }

        public enum TargetSpecificEventType : uint
        {
            ProcessCreate = 0U,
            ProcessExit = 1U,
            ProcessKill = 2U,
            ProcessExitSpawn = 3U,
            PPUExcTrap = 16U,
            PPUExcPrevInt = 17U,
            PPUExcAlignment = 18U,
            PPUExcIllInst = 19U,
            PPUExcTextHtabMiss = 20U,
            PPUExcTextSlbMiss = 21U,
            PPUExcDataHtabMiss = 22U,
            PPUExcFloat = 23U,
            PPUExcDataSlbMiss = 24U,
            PPUExcDabrMatch = 25U,
            PPUExcStop = 26U,
            PPUExcStopInit = 27U,
            PPUExcDataMAT = 28U,
            PPUThreadCreate = 32U,
            PPUThreadExit = 33U,
            SPUThreadStart = 48U,
            SPUThreadStop = 49U,
            SPUThreadStopInit = 50U,
            SPUThreadGroupDestroy = 51U,
            SPUThreadStopEx = 52U,
            PRXLoad = 64U,
            PRXUnload = 65U,
            DAInitialised = 96U,
            Footswitch = 112U,
            InstallPackageProgress = 128U,
            InstallPackagePath = 129U,
            CoreDumpComplete = 256U,
            RawNotify = 4026531855U,
        }

        public struct PPUProcessCreateData
        {
            public uint ParentProcessID;
            public string Filename;
        }

        public struct PPUProcessExitData
        {
            public ulong ExitCode;
        }

        public struct PPUExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUAlignmentExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong DSISR;
            public ulong DAR;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUDataMatExceptionData
        {
            public ulong ThreadID;
            public uint HWThreadNumber;
            public ulong DSISR;
            public ulong DAR;
            public ulong PC;
            public ulong SP;
        }

        public struct PPUThreadCreateData
        {
            public ulong ThreadID;
        }

        public struct PPUThreadExitData
        {
            public ulong ThreadID;
        }

        public struct SPUThreadStartData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public string ElfFilename;
        }

        public enum SPUThreadStopReason : uint
        {
            NoException = 0U,
            DMAAlignment = 1U,
            DMACommand = 2U,
            Error = 4U,
            MFCFIR = 8U,
            MFCSegment = 16U,
            MFCStorage = 32U,
            NoValue = 64U,
            StopCall = 256U,
            StopDCall = 512U,
            Halt = 1024U,
        }

        public struct SPUThreadStopData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public uint PC;
            public PS3TMAPI.SPUThreadStopReason Reason;
            public uint SP;
        }

        public struct SPUThreadStopExData
        {
            public uint ThreadGroupID;
            public uint ThreadID;
            public uint PC;
            public PS3TMAPI.SPUThreadStopReason Reason;
            public uint SP;
            public ulong MFCDSISR;
            public ulong MFCDSIPR;
            public ulong MFCDAR;
        }

        public struct SPUThreadGroupDestroyData
        {
            public uint ThreadGroupID;
        }

        public struct NotifyPRXLoadData
        {
            public ulong PPUThreadID;
            public uint PRXID;
            public ulong Timestamp;
        }

        public struct NotifyPRXUnloadData
        {
            public ulong PPUThreadID;
            public uint PRXID;
            public ulong Timestamp;
        }

        public struct FootswitchData
        {
            public ulong EventSource;
            public ulong EventData1;
            public ulong EventData2;
            public ulong EventData3;
            public ulong Reserved;
        }

        public struct InstallPackageProgress
        {
            public uint Percent;
        }

        public struct InstallPackagePath
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string Path;
        }

        public struct CoreDumpComplete
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string Filename;
        }

        public struct TargetSpecificData
        {
            public TargetSpecificEventType Type;
            public PPUProcessCreateData PPUProcessCreate;
            public PPUProcessExitData PPUProcessExit;
            public PPUExceptionData PPUException;
            public PPUAlignmentExceptionData PPUAlignmentException;
            public PPUDataMatExceptionData PPUDataMatException;
            public PPUThreadCreateData PPUThreadCreate;
            public PPUThreadExitData PPUThreadExit;
            public SPUThreadStartData SPUThreadStart;
            public SPUThreadStopData SPUThreadStop;
            public SPUThreadStopExData SPUThreadStopEx;
            public SPUThreadGroupDestroyData SPUThreadGroupDestroyData;
            public NotifyPRXLoadData PRXLoad;
            public NotifyPRXUnloadData PRXUnload;
            public FootswitchData Footswitch;
            public InstallPackageProgress InstallPackageProgress;
            public InstallPackagePath InstallPackagePath;
            public CoreDumpComplete CoreDumpComplete;
        }

        public struct TargetSpecificEvent
        {
            public uint CommandID;
            public uint RequestID;
            public uint ProcessID;
            public uint Result;
            public PS3TMAPI.TargetSpecificData Data;
        }

        private enum EventType
        {
            TTY = 100,
            Target = 101,
            System = 102,
            FTP = 103,
            PadCapture = 104,
            FileTrace = 105,
            PadPlayback = 106,
        }

        private class ScopedGlobalHeapPtr
        {
            private IntPtr m_intPtr = IntPtr.Zero;

            public ScopedGlobalHeapPtr(IntPtr intPtr)
            {
                this.m_intPtr = intPtr;
            }

            ~ScopedGlobalHeapPtr()
            {
                if (!(this.m_intPtr != IntPtr.Zero))
                    return;
                Marshal.FreeHGlobal(this.m_intPtr);
            }

            public IntPtr Get()
            {
                return this.m_intPtr;
            }
        }

        private delegate void SearchTargetsCallbackPriv(IntPtr name, IntPtr type, IntPtr connectInfo, IntPtr userData);
        public delegate void SearchTargetsCallback(string name, string type, PS3TMAPI.TCPIPConnectProperties ConnectInfo, object userData);
        private class SearchForTargetsCallbackHandler
        {
            private SearchTargetsCallback m_SearchForTargetCallback;
            private object m_UserData;

            public SearchForTargetsCallbackHandler(SearchTargetsCallback callback, object userData)
            {
                this.m_SearchForTargetCallback = callback;
                this.m_UserData = userData;
            }

            public static void SearchForTargetsCallback(IntPtr namePtr, IntPtr typePtr, IntPtr connectInfoPtr, IntPtr userDataPtr)
            {
                SearchForTargetsCallbackHandler targetsCallbackHandler = (SearchForTargetsCallbackHandler)GCHandle.FromIntPtr(userDataPtr).Target;
                TCPIPConnectProperties ConnectInfo = (TCPIPConnectProperties)null;
                if (connectInfoPtr != IntPtr.Zero)
                {
                    ConnectInfo = new TCPIPConnectProperties();
                    Marshal.PtrToStructure(connectInfoPtr, (object)ConnectInfo);
                }
                /*string name = AllocUtf8FromString(namePtr, uint.MaxValue);
                if (name == "")
                    name = (string)null;
                 string type = AllocUtf8FromString(typePtr, uint.MaxValue);
                targetsCallbackHandler.m_SearchForTargetCallback(name, type, ConnectInfo, targetsCallbackHandler.m_UserData);*/
            }
        }

        private delegate void HandleEventCallbackPriv(int target, PS3TMAPI.EventType type, uint param, PS3TMAPI.SNRESULT result, uint length, IntPtr data, IntPtr userData);
        #endregion
        #endregion

        #region dlls
        [DllImport("CCAPI.dll", EntryPoint = "CCAPIGetProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessList(ref uint numberProcesses, IntPtr processIdPtr);
        [DllImport("CCAPI.dll", EntryPoint = "CCAPIGetProcessName", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessName(uint processId, IntPtr processIdPtr);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTMVersionX86(out IntPtr version);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTMVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTMVersionX64(out IntPtr version);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetAPIVersionX86(out IntPtr version);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetAPIVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetAPIVersionX64(out IntPtr version);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TranslateErrorX86(SNRESULT res, out IntPtr message);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TranslateError", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TranslateErrorX64(SNRESULT res, out IntPtr message);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetErrorQualifierX86(out uint qualifier, out IntPtr message);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetErrorQualifier", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetErrorQualifierX64(out uint qualifier, out IntPtr message);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectStatusX86(int target, out uint status, out IntPtr usage);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectStatusX64(int target, out uint status, out IntPtr usage);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InitTargetCommsX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InitTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InitTargetCommsX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CloseTargetCommsX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CloseTargetComms", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CloseTargetCommsX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsX86(EnumerateTargetsCallback callback);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsX64(EnumerateTargetsCallback callback);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsExX86(EnumerateTargetsExCallbackPriv callback, IntPtr unused);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnumerateTargetsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnumerateTargetsExX64(EnumerateTargetsExCallbackPriv callback, IntPtr unused);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetNumTargetsX86(out uint numTargets);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetNumTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetNumTargetsX64(out uint numTargets);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetFromNameX86(string name, out int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetFromName", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetFromNameX64(string name, out int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetX86(int target, ulong resetParameter);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Reset", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetX64(int target, ulong resetParameter);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetExX86(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ResetEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ResetExX64(int target, ulong boot, ulong bootMask, ulong reset, ulong resetMask, ulong system, ulong systemMask);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetResetParametersX86(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetResetParameters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetResetParametersX64(int target, out ulong boot, out ulong bootMask, out ulong reset, out ulong resetMask, out ulong system, out ulong systemMask);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBootParameterX86(int target, ulong boot, ulong bootMask);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBootParameterX64(int target, ulong boot, ulong bootMask);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCurrentBootParameterX86(int target, out ulong boot);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCurrentBootParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCurrentBootParameterX64(int target, out ulong boot);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSystemParameterX86(int target, ulong system, ulong systemMask);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSystemParameter", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSystemParameterX64(int target, ulong system, ulong systemMask);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetInfoX86(IntPtr unmanagedMem);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTargetInfoX64(IntPtr unmanagedMem);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTargetInfoX86(ref TargetInfo info);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTargetInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTargetInfoX64(ref TargetInfo info);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTargetTypesX86(ref uint size, IntPtr targetTypes);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTargetTypes", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTargetTypesX64(ref uint size, IntPtr targetTypes);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT AddTargetX86(string name, string type, int connParamsSize, IntPtr connectParams, out int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3AddTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT AddTargetX64(string name, string type, int connParamsSize, IntPtr connectParams, out int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectionInfoX86(int target, IntPtr connectProperties);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConnectionInfoX64(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetConnectionInfoX86(int target, IntPtr connectProperties);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetConnectionInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetConnectionInfoX64(int target, IntPtr connectProperties);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteTargetX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteTargetX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectX86(int target, string application);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Connect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectX64(int target, string application);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectExX86(int target, string application, bool bForceFlag);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ConnectEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ConnectExX64(int target, string application, bool bForceFlag);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DisconnectX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Disconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DisconnectX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceDisconnectX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceDisconnectX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSystemInfoX86(int target, uint reserved, out uint mask, out SystemInfo info);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSystemInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSystemInfoX64(int target, uint reserved, out uint mask, out SystemInfo info);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetExtraLoadFlagsX86(int target, out ulong extraLoadFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetExtraLoadFlagsX64(int target, out ulong extraLoadFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetExtraLoadFlagsX86(int target, ulong extraLoadFlags, ulong mask);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetExtraLoadFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetExtraLoadFlagsX64(int target, ulong extraLoadFlags, ulong mask);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSDKVersionX86(int target, out ulong sdkVersion);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSDKVersionX64(int target, out ulong sdkVersion);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCPVersionX86(int target, out ulong cpVersion);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCPVersion", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCPVersionX64(int target, out ulong cpVersion);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTimeoutsX86(int target, uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetTimeoutsX64(int target, uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTimeoutsX86(int target, out uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetTimeouts", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetTimeoutsX64(int target, out uint numTimeouts, TimeoutType[] timeoutIds, uint[] timeoutValues);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTtyStreamsX86(int target, ref uint size, IntPtr streamArray);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ListTTYStreams", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ListTtyStreamsX64(int target, ref uint size, IntPtr streamArray);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTtyEventHandlerX86(int target, uint streamIndex, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTTYEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTtyEventHandlerX64(int target, uint streamIndex, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTtyEventsX86(int target, uint index);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTTYEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTtyEventsX64(int target, uint index);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYX86(int target, uint index, string text);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendTTY", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendTTYX64(int target, uint index, string text);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearTTYCache", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearTTYCacheX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearTTYCache", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearTTYCacheX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT KickX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Kick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT KickX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetStatusX86(int target, UnitType unit, out long status, IntPtr reasonCode);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetStatusX64(int target, UnitType unit, out long status, IntPtr reasonCode);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessLoadX86(int target, uint priority, string fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessLoadX64(int target, uint priority, string fileName, int argCount, string[] args, int envCount, string[] env, out uint processId, out ulong threadId, uint flags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessListX86(int target, ref uint count, IntPtr processIdArray);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessListX64(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserProcessListX86(int target, ref uint count, IntPtr processIdArray);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UserProcessList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserProcessListX64(int target, ref uint count, IntPtr processIdArray);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessStopX86(int target, uint processId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessStopX64(int target, uint processId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessContinueX86(int target, uint processId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessContinueX64(int target, uint processId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessKillX86(int target, uint processId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessKill", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessKillX64(int target, uint processId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TerminateGameProcessX86(int target, uint processId, uint timeout);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TerminateGameProcess", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TerminateGameProcessX64(int target, uint processId, uint timeout);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadListX86(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadListX64(int target, uint processId, ref uint numPPUThreads, ulong[] ppuThreadIds, ref uint numSPUThreadGroups, ulong[] spuThreadGroupIds);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadStopX86(int target, UnitType unit, uint processId, ulong threadId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadStopX64(int target, UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadContinueX86(int target, UnitType unit, uint processId, ulong threadId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadContinueX64(int target, UnitType unit, uint processId, ulong threadId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadGetRegistersX86(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadGetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadGetRegistersX64(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadSetRegistersX86(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadSetRegisters", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadSetRegistersX64(int target, UnitType unit, uint processId, ulong threadId, uint numRegisters, uint[] registerNums, ulong[] registerValues);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoExX86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoExX64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoEx2X86(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessInfoEx2", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessInfoEx2X64(int target, uint processId, ref uint bufferSize, IntPtr processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleListX86(int target, uint processId, ref uint numModules, uint[] moduleList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleListX64(int target, uint processId, ref uint numModules, uint[] moduleList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoExX86(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out PS3TMAPI.ExtraModuleInfo extraModuleInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetModuleInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetModuleInfoExX64(int target, uint processId, uint moduleId, ref ulong bufferSize, IntPtr moduleInfoEx, out IntPtr mselfInfo, out PS3TMAPI.ExtraModuleInfo extraModuleInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadInfoX86(int target, UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetThreadInfoX64(int target, UnitType unit, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPPUThreadInfoExX86(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PPUThreadInfoEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPPUThreadInfoExX64(int target, uint processId, ulong threadId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultPPUThreadStackSizeX86(int target, ELFStackSize size);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultPPUThreadStackSizeX64(int target, ELFStackSize size);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultPPUThreadStackSizeX86(int target, out ELFStackSize size);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultPPUThreadStackSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultPPUThreadStackSizeX64(int target, out ELFStackSize size);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSPULoopPointX86(int target, uint processId, ulong threadId, uint address, int bCurrentPc);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetSPULoopPointX64(int target, uint processId, ulong threadId, uint address, int bCurrentPc);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearSPULoopPointX86(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearSPULoopPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearSPULoopPointX64(int target, uint processId, ulong threadId, uint address, bool bCurrentPc);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearBreakPointX86(int target, uint unit, uint processId, ulong threadId, ulong address);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ClearBreakPoint", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ClearBreakPointX64(int target, uint unit, uint processId, ulong threadId, ulong address);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetBreakPointsX86(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetBreakPoints", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetBreakPointsX64(int target, uint unit, uint processId, ulong threadId, out uint numBreakpoints, ulong[] addresses);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDebugThreadControlInfoX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDebugThreadControlInfoX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDebugThreadControlInfoX86(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDebugThreadControlInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDebugThreadControlInfoX64(int target, uint processId, IntPtr threadCtrlInfo, out uint maxEntries);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadExceptionCleanX86(int target, uint processId, ulong threadId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ThreadExceptionClean", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ThreadExceptionCleanX64(int target, uint processId, ulong threadId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRawSPULogicalIdsX86(int target, uint processId, ulong[] logicalIds);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRawSPULogicalIDs", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRawSPULogicalIdsX64(int target, uint processId, ulong[] logicalIds);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupStopX86(int target, uint processId, ulong threadGroupId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupStop", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupStopX64(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupContinueX86(int target, uint processId, ulong threadGroupId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SPUThreadGroupContinue", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SPUThreadGroupContinueX64(int target, uint processId, ulong threadGroupId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessTreeX86(int target, ref uint numProcesses, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetProcessTree", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetProcessTreeX64(int target, ref uint numProcesses, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSPUThreadGroupInfoX86(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSPUThreadGroupInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSPUThreadGroupInfoX64(int target, uint processId, ulong threadGroupId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessGetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessGetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessGetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessSetMemoryX86(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessSetMemoryX64(int target, UnitType unit, uint processId, ulong threadId, ulong address, int count, byte[] buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemoryCompressedX86(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemoryCompressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemoryCompressedX64(int target, uint processId, uint compressionLevel, uint address, uint size, byte[] buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemory64CompressedX86(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMemory64Compressed", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMemory64CompressedX64(int target, uint processId, uint compressionLevel, ulong address, uint size, byte[] buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVirtualMemoryInfoX86(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVirtualMemoryInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVirtualMemoryInfoX64(int target, uint processId, bool bStatsOnly, out uint areaCount, out uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSyncPrimitiveCountsExX86(int target, uint processId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSyncPrimitiveCountsEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSyncPrimitiveCountsExX64(int target, uint processId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexListX86(int target, uint processId, ref uint numMutexes, uint[] mutexList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexListX64(int target, uint processId, ref uint numMutexes, uint[] mutexList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexInfoX86(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMutexInfoX64(int target, uint processId, uint mutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexListX86(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexListX64(int target, uint processId, ref uint numLWMutexes, uint[] lwMutexList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexInfoX86(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightMutexInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightMutexInfoX64(int target, uint processId, uint lwMutexId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableListX86(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableListX64(int target, uint processId, ref uint numConditionVars, uint[] conditionVarList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableInfoX86(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetConditionalVariableInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetConditionalVariableInfoX64(int target, uint processId, uint conditionVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalListX86(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalListX64(int target, uint processId, ref uint numLWConditionVars, uint[] lwConditionVarList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExportTargetSettingsX86(int target, IntPtr szFileName);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExportTargetSettingsX64(int target, IntPtr szFileName);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ImportTargetSettingsX86(int target, IntPtr szFileName);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ImportTargetSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ImportTargetSettingsX64(int target, IntPtr szFileName);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTargetEventsX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelTargetEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelTargetEventsX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnmapFileSystemX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnmapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnmapFileSystemX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MapFileSystemX86(char driveLetter);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MapFileSystem", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MapFileSystemX64(char driveLetter);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDisplaySettingsX86(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDisplaySettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDisplaySettingsX64(int target, IntPtr executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsValidResolutionX86(uint monitorType, uint startupResolution);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsValidResolution", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsValidResolutionX64(uint monitorType, uint startupResolution);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsScanningX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3IsScanning", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT IsScanningX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopSearchForTargetsX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopSearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopSearchForTargetsX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SearchForTargetsX86(string ipAddressFrom, string ipAddressTo, SearchTargetsCallbackPriv callback, IntPtr userData, int port);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SearchForTargets", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SearchForTargetsX64(string ipAddressFrom, string ipAddressTo, SearchTargetsCallbackPriv callback, IntPtr userData, int port);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDQueryX86(int target, string deviceName, ref BDInfoPriv infoPriv);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDQuery", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDQueryX64(int target, string deviceName, ref BDInfoPriv infoPriv);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDFormatX86(int target, string deviceName, uint formatMode);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDFormat", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDFormatX64(int target, string deviceName, uint formatMode);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileX86(int target, string file, out DirEntry dirEntry);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileX64(int target, string file, out DirEntry dirEntry);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileExX86(int target, string file, out DirEntryEx dirEntry, out PS3TMAPI.TargetTimezone timeZone);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StatTargetFileEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StatTargetFileExX64(int target, string file, out DirEntryEx dirEntry, out TargetTimezone timeZone);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CHModX86(int target, string filePath, uint mode);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CHMod", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CHModX64(int target, string filePath, uint mode);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileTimeX86(int target, string filePath, ulong accessTime, ulong modifiedTime);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileTime", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileTimeX64(int target, string filePath, ulong accessTime, ulong modifiedTime);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallGameExX86(int target, string paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallGameEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallGameExX64(int target, string paramSfoPath, out IntPtr titleId, out IntPtr targetPath, out uint txId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FormatHDDX86(int target, uint initRegistry);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FormatHDD", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FormatHDDX64(int target, uint initRegistry);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UninstallGameX86(int target, string gameDirectory);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UninstallGame", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UninstallGameX64(int target, string gameDirectory);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT WaitForFileTransferX86(int target, uint txId, out FileTransferNotificationType notificationType, uint msTimeout);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3WaitForFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT WaitForFileTransferX64(int target, uint txId, out FileTransferNotificationType notificationType, uint msTimeout);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FSGetFreeSizeX86(int target, string fsDir, out uint blockSize, out ulong freeBlockCount);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FSGetFreeSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FSGetFreeSizeX64(int target, string fsDir, out uint blockSize, out ulong freeBlockCount);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLogOptionsX86(out LogCategory category);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLogOptionsX64(out LogCategory category);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetLogOptionsX86(LogCategory category);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetLogOptions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetLogOptionsX64(LogCategory category);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessOfflineFileTraceX86(int target, string path);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessOfflineFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessOfflineFileTraceX64(int target, string path);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTargetEventHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterTargetEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterTargetEventHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDEjectX86(int target, string deviceName);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDEject", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDEjectX64(int target, string deviceName);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDInsertX86(int target, string deviceName);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDInsert", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDInsertX64(int target, string deviceName);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDTransferImageX86(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3BDTransferImage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT BDTransferImageX64(int target, IntPtr sourceFileName, string destinationDevice, out uint transactionId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableInternalKickX86(bool enable);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableInternalKick", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableInternalKickX64(bool enable);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MakeDirectoryX86(int target, string directory, uint mode);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3MakeDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT MakeDirectoryX64(int target, string directory, uint mode);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileX86(int target, string path);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Delete", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileX64(int target, string path);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileExX86(int target, string path, uint msTimeout);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DeleteEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DeleteFileExX64(int target, string path, uint msTimeout);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RenameFileX86(int target, string source, string dest);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Rename", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RenameFileX64(int target, string source, string dest);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadFileX86(int target, string source, string dest, out uint transactionId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadFileX64(int target, string source, string dest, out uint transactionId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadDirectoryX86(int target, string source, string dest, out uint lastTransactionId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3DownloadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT DownloadDirectoryX64(int target, string source, string dest, out uint lastTransactionId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadDirectoryX86(int target, string source, string dest, out uint lastTransactionId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadDirectoryX64(int target, string source, string dest, out uint lastTransactionId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockInfoX86(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockInfoX64(int target, uint processId, uint rwLockId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalInfoX86(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetLightWeightConditionalInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetLightWeightConditionalInfoX64(int target, uint processId, uint lwCondVarId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockListX86(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetReadWriteLockList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetReadWriteLockListX64(int target, uint processId, ref uint numRWLocks, uint[] rwLockList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreListX86(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreListX64(int target, uint processId, ref uint numSemaphores, uint[] semaphoreList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreInfoX86(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetSemaphoreInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetSemaphoreInfoX64(int target, uint processId, uint semaphoreId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueListX86(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueListX64(int target, uint processId, ref uint numEventQueues, uint[] eventQueueList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueInfoX86(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventQueueInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventQueueInfoX64(int target, uint processId, uint eventQueueId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagListX86(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagListX64(int target, uint processId, ref uint numEventFlags, uint[] eventFlagList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagInfoX86(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetEventFlagInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetEventFlagInfoX64(int target, uint processId, uint eventFlagId, ref uint bufferSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PickTargetX86(IntPtr hWndOwner, out int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PickTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PickTargetX64(IntPtr hWndOwner, out int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableAutoStatusUpdateX86(int target, uint enabled, out uint previousState);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableAutoStatusUpdate", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableAutoStatusUpdateX64(int target, uint enabled, out uint previousState);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferInfoX86(int target, uint txId, out FileTransferInfoPriv fileTransferInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferInfoX64(int target, uint txId, out FileTransferInfoPriv fileTransferInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFileTransferX86(int target, uint txID);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFileTransferX64(int target, uint txID);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RetryFileTransferX86(int target, uint txID, bool bForce);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RetryFileTransfer", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RetryFileTransferX64(int target, uint txID, bool bForce);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RemoveTransferItemsByStatusX86(int target, uint filter);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RemoveTransferItemsByStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SNPS3RemoveTransferItemsByStatusX64(int target, uint filter);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListX86(int target, string directory, ref uint numDirEntries, IntPtr dirEntryList);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListX64(int target, string directory, ref uint numDirEntries, IntPtr dirEntryList);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListExX86(int target, string directory, ref uint numDirEntries, IntPtr dirEntryListEx, ref PS3TMAPI.TargetTimezone timeZone);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDirectoryListEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDirectoryListExX64(int target, string directory, ref uint numDirEntries, IntPtr dirEntryListEx, ref PS3TMAPI.TargetTimezone timeZone);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFileTraceHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFileTraceHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterFileTraceHandlerX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterFileTraceHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterFileTraceHandlerX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartFileTraceX86(int target, uint processId, uint size, string filename);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartFileTraceX64(int target, uint processId, uint size, string filename);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopFileTraceX86(int target, uint processId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopFileTrace", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopFileTraceX64(int target, uint processId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallPackageX86(int target, string packagePath);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3InstallPackage", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT InstallPackageX64(int target, string packagePath);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadFileX86(int target, string source, string dest, out uint transactionId);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UploadFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UploadFileX64(int target, string source, string dest, out uint transactionId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferListX86(int target, ref uint count, IntPtr fileTransferInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileTransferList", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileTransferListX64(int target, ref uint count, IntPtr fileTransferInfo);


        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMInformationX86(int target, uint processId, out VramInfoPriv primaryVRAMInfo, out VramInfoPriv secondaryVRAMInfo);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMInformation", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMInformationX64(int target, uint processId, out VramInfoPriv primaryVRAMInfo, out VramInfoPriv secondaryVRAMInfo);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT VRAMCaptureX86(int target, uint processId, IntPtr vramInfo, string fileName);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3VRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT VRAMCaptureX64(int target, uint processId, IntPtr vramInfo, string fileName);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterCustomProtocolExX86(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterCustomProtocolEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterCustomProtocolExX64(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterCustomProtocolX86(int target, ref PS3Protocol protocol);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterCustomProtocolX64(int target, ref PS3Protocol protocol);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceUnregisterCustomProtocolX86(int target, ref PS3Protocol protocol);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ForceUnRegisterCustomProtocol", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ForceUnregisterCustomProtocolX64(int target, ref PS3Protocol protocol);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendCustomProtocolDataX86(int target, ref PS3Protocol protocol, byte[] data, int length);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendCustomProtocolData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendCustomProtocolDataX64(int target, ref PS3Protocol protocol, byte[] data, int length);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileServingEventFlagsX86(int target, ulong eventFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetFileServingEventFlagsX64(int target, ulong eventFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileServingEventFlagsX86(int target, ref ulong eventFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetFileServingEventFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetFileServingEventFlagsX64(int target, ref ulong eventFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCaseSensitiveFileServingX86(int target, bool bOn, out bool bOldSetting);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCaseSensitiveFileServing", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCaseSensitiveFileServingX64(int target, bool bOn, out bool bOldSetting);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFTPEventHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterFTPEventHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterFTPEventHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFTPEventsX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3CancelFTPEvents", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT CancelFTPEventsX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitExX86(uint millisecondTimeout);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ExitEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitExX64(uint millisecondTimeout);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadPlaybackNotificationHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadPlaybackNotificationHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadPlaybackHandlerX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadPlaybackNotificationHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadPlaybackHandlerX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadPlaybackX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadPlaybackX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadPlaybackX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadPlayback", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadPlaybackX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendPadPlaybackDataX86(int target, ref PadData data);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SendPadPlaybackData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SendPadPlaybackDataX64(int target, ref PadData data);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadCaptureHandlerX86(int target, HandleEventCallbackPriv callback, IntPtr userData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3RegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT RegisterPadCaptureHandlerX64(int target, HandleEventCallbackPriv callback, IntPtr userData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadCaptureHandlerX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3UnRegisterPadCaptureHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT UnregisterPadCaptureHandlerX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadCaptureX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StartPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StartPadCaptureX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadCaptureX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3StopPadCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT StopPadCaptureX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMCaptureFlagsX86(int target, out ulong vramFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetVRAMCaptureFlagsX64(int target, out ulong vramFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetVRAMCaptureFlagsX86(int target, ulong vramFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetVRAMCaptureFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetVRAMCaptureFlagsX64(int target, ulong vramFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableVRAMCaptureX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableVRAMCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableVRAMCaptureX864(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATRangesX86(int target, uint processId, ref uint rangeCount, IntPtr matRanges);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATRanges", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATRangesX64(int target, uint processId, ref uint rangeCount, IntPtr matRanges);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATConditionsX86(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMATConditionsX64(int target, uint processId, ref uint rangeCount, IntPtr ranges, ref uint bufSize, IntPtr outputBuf);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetMATConditionsX86(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetMATConditions", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetMATConditionsX64(int target, uint processId, uint rangeCount, uint bufSize, IntPtr buffer);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SaveSettingsX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SaveSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SaveSettingsX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitX86();
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3Exit", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ExitX64();

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TriggerCoreDumpX86(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3TriggerCoreDump", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT TriggerCoreDumpX64(int target, uint processId, ulong userData1, ulong userData2, ulong userData3);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCoreDumpFlagsX86(int target, out ulong flags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetCoreDumpFlagsX64(int target, out ulong flags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCoreDumpFlagsX86(int tarSet, ulong flags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCoreDumpFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCoreDumpFlagsX64(int tarSet, ulong flags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessAttachX86(int target, uint unitId, uint processId);

        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessAttach", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessAttachX64(int target, uint unitId, uint processId);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FlashTargetX86(int target, string updaterToolPath, string flashImagePath);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FlashTarget", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FlashTargetX64(int target, string updaterToolPath, string flashImagePath);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMacAddressX86(int target, out IntPtr stringPtr);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetMacAddress", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetMacAddressX64(int target, out IntPtr stringPtr);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessScatteredSetMemoryX86(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3ProcessScatteredSetMemory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT ProcessScatteredSetMemoryX64(int target, uint processId, uint numWrites, uint writeSize, IntPtr writeData, out uint errorCode, out uint failedAddress);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPowerStatusX86(int target, out PowerStatus status);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetPowerStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetPowerStatusX64(int target, out PowerStatus status);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOnX86(int target);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOn", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOnX64(int target);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOffX86(int target, uint force);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3PowerOff", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT PowerOffX64(int target, uint force);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserMemoryStatsX86(int target, uint processId, out UserMemoryStats memoryStats);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetUserMemoryStats", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetUserMemoryStatsX64(int target, uint processId, out UserMemoryStats memoryStats);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultLoadPriorityX86(int target, uint priority);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDefaultLoadPriorityX64(int target, uint priority);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultLoadPriorityX86(int target, out uint priority);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDefaultLoadPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDefaultLoadPriorityX64(int target, out uint priority);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortIPAddrDataX86(int target, string deviceName, out GamePortIPAddressData ipAddressData);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortIPAddrDataX64(int target, string deviceName, out GamePortIPAddressData ipAddressData);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortDebugIPAddrDataX86(int target, string deviceName, out GamePortIPAddressData data);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetGamePortDebugIPAddrData", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetGamePortDebugIPAddrDataX64(int target, string deviceName, out GamePortIPAddressData data);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDABRX86(int target, uint processId, ulong address);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetDABRX64(int target, uint processId, ulong address);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDABRX86(int target, uint processId, out ulong address);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetDABR", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetDABRX64(int target, uint processId, out ulong address);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetRSXProfilingFlagsX86(int target, ulong rsxFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetRSXProfilingFlagsX64(int target, ulong rsxFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRSXProfilingFlagsX86(int target, out ulong rsxFlags);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetRSXProfilingFlags", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetRSXProfilingFlagsX64(int target, out ulong rsxFlags);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCustomParamSFOMappingDirectoryX86(int target, string paramSfoDir);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetCustomParamSFOMappingDirectory", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetCustomParamSFOMappingDirectoryX64(int target, string paramSfoDir);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableXMBSettingsX86(int target, int enable);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3EnableXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT EnableXMBSettingsX64(int target, int enable);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetXMBSettingsX86(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3GetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT GetXMBSettingsX64(int target, IntPtr buffer, ref uint bufferSize, bool bUpdateCache);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetXMBSettingsX86(int target, string xmbSettings, bool bUpdateCache);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3SetXMBSettings", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT SetXMBSettingsX64(int target, string xmbSettings, bool bUpdateCache);

        [DllImport("PS3TMAPI.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FootswitchControlX86(int target, uint enabled);
        [DllImport("PS3TMAPIX64.dll", EntryPoint = "SNPS3FootswitchControl", CallingConvention = CallingConvention.Cdecl)]
        private static extern SNRESULT FootswitchControlX64(int target, uint enabled);
        #endregion

        static PS3TMAPI()
        {

        }

        public void InitComms()
        {
            InitTargetComms();
        }

        public uint Inits()
        {
             uint proc = ProcessID = GetProcessID();
            return proc;
        }

        public uint GetProcessID()
        {
            uint[] numArray;
            var proc = GetProcessList(Target, out numArray);
            return numArray[0];
        }

        public static bool FAILED(SNRESULT res)
        {
            return !SUCCEEDED(res);
        }
        public static bool SUCCEEDED(SNRESULT res)
        {
            return res >= SNRESULT.SN_S_OK;
        }

        private static bool Is32Bit()
        {
            return IntPtr.Size == 4;
        }

        private static byte VersionMajor(ulong version)
        {
            return (byte)(version >> 16);
        }
        private static byte VersionMinor(ulong version)
        {
            return (byte)(version >> 8);
        }
        private static byte VersionFix(ulong version)
        {
            return (byte)version;
        }
        private static void VersionComponents(ulong version, out byte major, out byte minor, out byte fix)
        {
            major = PS3TMAPI.VersionMajor(version);
            minor = PS3TMAPI.VersionMinor(version);
            fix = PS3TMAPI.VersionFix(version);
        }

        public static byte SDKVersionMajor(ulong sdkVersion)
        {
            return PS3TMAPI.VersionMajor(sdkVersion);
        }
        public static byte SDKVersionMinor(ulong sdkVersion)
        {
            return PS3TMAPI.VersionMinor(sdkVersion);
        }
        public static byte SDKVersionFix(ulong sdkVersion)
        {
            return PS3TMAPI.VersionFix(sdkVersion);
        }
        public static void SDKVersionComponents(ulong sdkVersion, out byte major, out byte minor, out byte fix)
        {
            major = PS3TMAPI.SDKVersionMajor(sdkVersion);
            minor = PS3TMAPI.SDKVersionMinor(sdkVersion);
            fix = PS3TMAPI.SDKVersionFix(sdkVersion);
        }

        public static byte CPVersionMajor(ulong cpVersion)
        {
            return PS3TMAPI.VersionMajor(cpVersion);
        }
        public static byte CPVersionMinor(ulong cpVersion)
        {
            return VersionMinor(cpVersion);
        }
        public static byte CPVersionFix(ulong cpVersion)
        {
            return VersionFix(cpVersion);
        }
        public static void CPVersionComponents(ulong cpVersion, out byte major, out byte minor, out byte fix)
        {
            major = CPVersionMajor(cpVersion);
            minor = CPVersionMinor(cpVersion);
            fix = CPVersionFix(cpVersion);
        }

        #region dll's Calls

        ///CCAPI
        /// <summary>Get the process name of your choice.</summary>
        public static int GetProcessName(uint processId, out string name)
        {
            IntPtr ptr = Marshal.AllocHGlobal((int)(0x211)); int result = -1;
            result = ProcessName(processId, ptr);
            name = String.Empty;
            if (result != 0)
                name = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
        public static int ProcessName(uint processId, IntPtr processIdPtr)
        {
            GetProcessName(processId, processIdPtr);
            return (int)processIdPtr;
        }

        ///CCAPI
        /// <summary>Get a list of all processes available.</summary>
        public static int GetProcessList(out uint[] processIds)
        {
            uint numOfProcs = 64; int result = -1;
            IntPtr ptr = Marshal.AllocHGlobal((int)(4 * 0x40));
             result = GetProcess(ref numOfProcs, ptr);
            processIds = new uint[numOfProcs];
            if (result != 0)
            {
                IntPtr unBuf = ptr;
                for (uint i = 0; i < numOfProcs; i++)
                    unBuf = ReadDataFromUnmanagedIncPtr<uint>(unBuf, ref processIds[i]);
            }
            Marshal.FreeHGlobal(ptr);
            return result;
        }
        public static int GetProcess(ref uint numberProcesses, IntPtr processIdPtr)
        {
            GetProcessList(ref numberProcesses, processIdPtr);
            return (int)processIdPtr;
        }

        public static SNRESULT GetTMVersion(out string version)
        {
            IntPtr version1;
            SNRESULT snresult = Is32Bit() ? GetTMVersionX86(out version1) : GetTMVersionX64(out version1);
            version = Marshal.PtrToStringAnsi(version1);
            return snresult;
        }

        public static SNRESULT GetAPIVersion(out string version)
        {
            IntPtr version1;
            SNRESULT snresult = Is32Bit() ? GetAPIVersionX86(out version1) : GetAPIVersionX64(out version1);
            version = Marshal.PtrToStringAnsi(version1);
            return snresult;
        }

        public static SNRESULT TranslateError(SNRESULT errorCode, out string message)
        {
            IntPtr message1;
            SNRESULT snresult = Is32Bit() ? TranslateErrorX86(errorCode, out message1) : TranslateErrorX64(errorCode, out message1);
            message = Marshal.PtrToStringAnsi(message1);
            return snresult;
        }

        public static SNRESULT GetErrorQualifier(out uint qualifier, out string message)
        {
            IntPtr message1;
            SNRESULT snresult = Is32Bit() ? GetErrorQualifierX86(out qualifier, out message1) : GetErrorQualifierX64(out qualifier, out message1);
            message = Marshal.PtrToStringAnsi(message1);
            return snresult;
        }

        public static SNRESULT GetConnectStatus(int target, out ConnectStatus status, out string usage)
        {
            uint status1;
            IntPtr usage1;
            SNRESULT snresult = Is32Bit() ? GetConnectStatusX86(target, out status1, out usage1) : GetConnectStatusX64(target, out status1, out usage1);
            status = (ConnectStatus)status1;
            usage = Marshal.PtrToStringAnsi(usage1);
            return snresult;
        }

        public static SNRESULT InitTargetComms()
        {
            if (!Is32Bit())
                return InitTargetCommsX64();
            else
                return InitTargetCommsX86();
        }

        public static SNRESULT CloseTargetComms()
        {
            if (!Is32Bit())
                return CloseTargetCommsX64();
            else
                return CloseTargetCommsX86();
        }

        public static SNRESULT EnumerateTargets(EnumerateTargetsCallback callback)
        {
            if (!Is32Bit())
                return EnumerateTargetsX64(callback);
            else
                return EnumerateTargetsX86(callback);
        }
        private static int EnumTargetsExPriv(int target, IntPtr unused)
        {
            return ms_enumTargetsExCallback(target, ms_enumTargetsExUserData);
        }

        public static SNRESULT EnumerateTargetsEx(EnumerateTargetsExCallback callback, ref object userData)
        {
            ms_enumTargetsExCallback = callback;
            ms_enumTargetsExUserData = userData;
            if (!Is32Bit())
                return EnumerateTargetsExX64(ms_enumTargetsExCallbackPriv, IntPtr.Zero);
            else
                return EnumerateTargetsExX86(ms_enumTargetsExCallbackPriv, IntPtr.Zero);
        }

        public static SNRESULT GetNumTargets(out uint numTargets)
        {
            if (!Is32Bit())
                return GetNumTargetsX64(out numTargets);
            else
                return GetNumTargetsX86(out numTargets);
        }

        public static SNRESULT GetTargetFromName(string name, out int target)
        {
            if (!Is32Bit())
                return GetTargetFromNameX64(name, out target);
            else
                return GetTargetFromNameX86(name, out target);
        }

        public static SNRESULT Reset(int target, ResetParameter resetParameter)
        {
            if (!Is32Bit())
                return ResetX64(target, (ulong)resetParameter);
            else
                return ResetX86(target, (ulong)resetParameter);
        }

        public static SNRESULT ResetEx(int target, BootParameter bootParameter, BootParameterMask bootMask, ResetParameter resetParameter, ResetParameterMask resetMask, SystemParameter systemParameter, SystemParameterMask systemMask)
        {
            if (!Is32Bit())
                return ResetExX64(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
            else
                return ResetExX86(target, (ulong)bootParameter, (ulong)bootMask, (ulong)resetParameter, (ulong)resetMask, (ulong)systemParameter, (ulong)systemMask);
        }

        public static SNRESULT GetResetParameters(int target, out BootParameter bootParameter, out BootParameterMask bootMask, out ResetParameter resetParameter, out ResetParameterMask resetMask, out SystemParameter systemParameter, out SystemParameterMask systemMask)
        {
            ulong boot;
            ulong bootMask1;
            ulong reset;
            ulong resetMask1;
            ulong system;
            ulong systemMask1;
            SNRESULT snresult = Is32Bit() ? GetResetParametersX86(target, out boot, out bootMask1, out reset, out resetMask1, out system, out systemMask1) : GetResetParametersX64(target, out boot, out bootMask1, out reset, out resetMask1, out system, out systemMask1);
            bootParameter = (BootParameter)boot;
            bootMask = (BootParameterMask)bootMask1;
            resetParameter = (ResetParameter)reset;
            resetMask = (ResetParameterMask)resetMask1;
            systemParameter = (SystemParameter)system;
            systemMask = (SystemParameterMask)systemMask1;
            return snresult;
        }

        public static SNRESULT SetBootParameter(int target, BootParameter bootParameter, BootParameterMask bootMask)
        {
            if (!Is32Bit())
                return SetBootParameterX64(target, (ulong)bootParameter, (ulong)bootMask);
            else
                return SetBootParameterX86(target, (ulong)bootParameter, (ulong)bootMask);
        }

        public static SNRESULT GetCurrentBootParameter(int target, out BootParameter bootParameter)
        {
            ulong boot;
            SNRESULT snresult = Is32Bit() ? GetCurrentBootParameterX86(target, out boot) : GetCurrentBootParameterX64(target, out boot);
            bootParameter = (BootParameter)boot;
            return snresult;
        }

        public static SNRESULT SetSystemParameter(int target, SystemParameter systemParameter, SystemParameterMask systemMask)
        {
            if (!Is32Bit())
                return SetSystemParameterX64(target, (ulong)systemParameter, (ulong)systemMask);
            else
                return SetSystemParameterX86(target, (ulong)systemParameter, (ulong)systemMask);
        }

        public static SNRESULT GetTargetInfo(ref TargetInfo targetInfo)
        {
            IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object)targetInfo));
            Marshal.StructureToPtr((object)targetInfo, num, false);
            SNRESULT res = Is32Bit() ? GetTargetInfoX86(num) : GetTargetInfoX64(num);
            if (SUCCEEDED(res))
                targetInfo = (TargetInfo)Marshal.PtrToStructure(num, typeof(TargetInfo));
            Marshal.FreeHGlobal(num);
            return res;
        }

        public static SNRESULT SetTargetInfo(TargetInfo targetInfo)
        {
            if (!Is32Bit())
                return SetTargetInfoX64(ref targetInfo);
            else
                return SetTargetInfoX86(ref targetInfo);
        }

        public static SNRESULT ListTargetTypes(out TargetType[] targetTypes)
        {
            targetTypes = null;
            IntPtr targetTypes1 = IntPtr.Zero;
            uint size = 0U;
            SNRESULT res1 = Is32Bit() ? ListTargetTypesX86(ref size, targetTypes1) : ListTargetTypesX64(ref size, targetTypes1);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(TargetType)) * (long)size));
            SNRESULT res2 = Is32Bit() ? ListTargetTypesX86(ref size, num) : ListTargetTypesX64(ref size, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                targetTypes = new TargetType[size];
                for (uint index = 0U; index < size; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.TargetType>(unmanagedBuf, ref targetTypes[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT AddTarget(string name, string targetType, TCPIPConnectProperties connectProperties, out int target)
        {
            IntPtr num1 = IntPtr.Zero;
            int num2 = 0;
            if (connectProperties != null)
            {
                num2 = Marshal.SizeOf((object)connectProperties);
                num1 = Marshal.AllocHGlobal(num2);
                Marshal.StructureToPtr((object)connectProperties, num1, false);
            }
            SNRESULT snresult = Is32Bit() ? AddTargetX86(name, targetType, num2, num1, out target) : AddTargetX64(name, targetType, num2, num1, out target);
            if (num1 != IntPtr.Zero)
                Marshal.FreeHGlobal(num1);
            return snresult;
        }

        public static SNRESULT GetConnectionInfo(int target, out TCPIPConnectProperties connectProperties)
        {
            connectProperties = (TCPIPConnectProperties)null;
            IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TCPIPConnectProperties)));
            SNRESULT res = Is32Bit() ? GetConnectionInfoX86(target, num) : GetConnectionInfoX64(target, num);
            if (SUCCEEDED(res))
            {
                connectProperties = new TCPIPConnectProperties();
                Marshal.PtrToStructure(num, (object)connectProperties);
            }
            Marshal.FreeHGlobal(num);
            return res;
        }

        public static SNRESULT SetConnectionInfo(int target, TCPIPConnectProperties connectProperties)
        {
            IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object)connectProperties));
            WriteDataToUnmanagedIncPtr<PS3TMAPI.TCPIPConnectProperties>(connectProperties, num);
            SNRESULT snresult = Is32Bit() ? SetConnectionInfoX86(target, num) : SetConnectionInfoX64(target, num);
            Marshal.FreeHGlobal(num);
            return snresult;
        }

        public static SNRESULT DeleteTarget(int target)
        {
            if (!Is32Bit())
                return DeleteTargetX64(target);
            else
                return DeleteTargetX86(target);
        }

        public static SNRESULT Connect(int target, string application)
        {
            if (!Is32Bit())
                return ConnectX64(target, application);
            else
                return ConnectX86(target, application);
        }

        public static SNRESULT ConnectEx(int target, string application, bool bForceFlag)
        {
            if (!Is32Bit())
                return ConnectExX64(target, application, bForceFlag);
            else
                return ConnectExX86(target, application, bForceFlag);
        }

        public static SNRESULT Disconnect(int target)
        {
            if (!Is32Bit())
                return DisconnectX64(target);
            else
                return DisconnectX86(target);
        }

        public static SNRESULT ForceDisconnect(int target)
        {
            if (!Is32Bit())
                return ForceDisconnectX64(target);
            else
                return ForceDisconnectX86(target);
        }

        public static SNRESULT GetSystemInfo(int target, out SystemInfoFlag mask, out SystemInfo systemInfo)
        {
            systemInfo = new SystemInfo();
            uint mask1;
            SNRESULT snresult = Is32Bit() ? GetSystemInfoX86(target, 0U, out mask1, out systemInfo) : GetSystemInfoX64(target, 0U, out mask1, out systemInfo);
            mask = (SystemInfoFlag)mask1;
            return snresult;
        }

        public static SNRESULT GetExtraLoadFlags(int target, out ExtraLoadFlag extraLoadFlags)
        {
            ulong extraLoadFlags1 = 0UL;
            SNRESULT snresult = Is32Bit() ? GetExtraLoadFlagsX86(target, out extraLoadFlags1) : GetExtraLoadFlagsX64(target, out extraLoadFlags1);
            extraLoadFlags = (ExtraLoadFlag)extraLoadFlags1;
            return snresult;
        }

        public static SNRESULT SetExtraLoadFlags(int target, ExtraLoadFlag extraLoadFlags, ExtraLoadFlagMask mask)
        {
            if (!Is32Bit())
                return SetExtraLoadFlagsX64(target, (ulong)extraLoadFlags, (ulong)mask);
            else
                return SetExtraLoadFlagsX86(target, (ulong)extraLoadFlags, (ulong)mask);
        }

        public static SNRESULT GetSDKVersion(int target, out ulong sdkVersion)
        {
            if (!Is32Bit())
                return GetSDKVersionX64(target, out sdkVersion);
            else
                return GetSDKVersionX86(target, out sdkVersion);
        }

        public static SNRESULT GetCPVersion(int target, out ulong cpVersion)
        {
            if (!Is32Bit())
                return GetCPVersionX64(target, out cpVersion);
            else
                return GetCPVersionX86(target, out cpVersion);
        }

        public static SNRESULT SetTimeouts(int target, TimeoutType[] timeoutTypes, uint[] timeoutValues)
        {
            if (timeoutTypes == null || timeoutTypes.Length < 1 || (timeoutValues == null || timeoutValues.Length != timeoutTypes.Length))
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SetTimeoutsX64(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
            else
                return SetTimeoutsX86(target, (uint)timeoutTypes.Length, timeoutTypes, timeoutValues);
        }

        public static SNRESULT GetTimeouts(int target, out TimeoutType[] timeoutTypes, out uint[] timeoutValues)
        {
            timeoutTypes = (TimeoutType[])null;
            timeoutValues = (uint[])null;
            uint numTimeouts;
            SNRESULT res = Is32Bit() ? GetTimeoutsX86(target, out numTimeouts, (TimeoutType[])null, (uint[])null) : GetTimeoutsX64(target, out numTimeouts, (TimeoutType[])null, (uint[])null);
            if (FAILED(res))
                return res;
            timeoutTypes = new TimeoutType[numTimeouts];
            timeoutValues = new uint[numTimeouts];
            if (!Is32Bit())
                return GetTimeoutsX64(target, out numTimeouts, timeoutTypes, timeoutValues);
            else
                return GetTimeoutsX86(target, out numTimeouts, timeoutTypes, timeoutValues);
        }

        public static SNRESULT ListTTYStreams(int target, out TTYStream[] streamArray)
        {
            streamArray = (TTYStream[])null;
            IntPtr streamArray1 = IntPtr.Zero;
            uint size = 0U;
            SNRESULT res1 = Is32Bit() ? ListTtyStreamsX86(target, ref size, streamArray1) : ListTtyStreamsX64(target, ref size, streamArray1);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(TTYStream)) * (long)size));
            SNRESULT res2 = Is32Bit() ? ListTtyStreamsX86(target, ref size, num) : ListTtyStreamsX64(target, ref size, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                streamArray = new TTYStream[size];
                for (uint index = 0U; index < size; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.TTYStream>(unmanagedBuf, ref streamArray[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT RegisterTTYEventHandler(int target, uint streamID, TTYCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterTtyEventHandlerX86(target, streamID, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterTtyEventHandlerX64(target, streamID, ms_eventHandlerWrapper, IntPtr.Zero);
            if (FAILED(res))
                return res;
            List<TTYChannel> list = new List<TTYChannel>();
            if ((int)streamID == -1)
            {
                TTYStream[] streamArray = (TTYStream[])null;
                res = ListTTYStreams(target, out streamArray);
                if (FAILED(res) || streamArray == null || streamArray.Length == 0)
                    return res;
                foreach (TTYStream ttyStream in streamArray)
                    list.Add(new TTYChannel(target, ttyStream.Index));
            }
            else
                list.Add(new TTYChannel(target, streamID));
            TTYCallbackAndUserData callbackAndUserData = new TTYCallbackAndUserData();
            callbackAndUserData.m_callback = callback;
            callbackAndUserData.m_userData = userData;
            foreach (TTYChannel index in list)
                ms_userTtyCallbacks[index] = callbackAndUserData;
            return res;
        }

        public static SNRESULT CancelTTYEvents(int target, uint streamID)
        {
            SNRESULT res = Is32Bit() ? CancelTtyEventsX86(target, streamID) : CancelTtyEventsX64(target, streamID);
            if (SUCCEEDED(res))
            {
                if ((int)streamID == -1)
                {
                    List<PS3TMAPI.TTYChannel> list = new List<PS3TMAPI.TTYChannel>();
                    foreach (KeyValuePair<PS3TMAPI.TTYChannel, TTYCallbackAndUserData> keyValuePair in ms_userTtyCallbacks)
                    {
                        if (keyValuePair.Key.Target == target)
                            list.Add(keyValuePair.Key);
                    }
                    foreach (PS3TMAPI.TTYChannel key in list)
                        ms_userTtyCallbacks.Remove(key);
                }
                else
                    ms_userTtyCallbacks.Remove(new PS3TMAPI.TTYChannel(target, streamID));
            }
            return res;
        }

        public static SNRESULT SendTTY(int target, uint streamID, string text)
        {
            if (!Is32Bit())
                return SendTTYX64(target, streamID, text);
            else
                return SendTTYX86(target, streamID, text);
        }

        private static void MarshalTTYEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            TTYChannel key = new TTYChannel(target, param);
            TTYCallbackAndUserData callbackAndUserData;
            if (!ms_userTtyCallbacks.TryGetValue(key, out callbackAndUserData))
                return;
            string data1 = Marshal.PtrToStringAnsi(data, (int)length);
            callbackAndUserData.m_callback(target, param, result, data1, callbackAndUserData.m_userData);
        }

        public static SNRESULT ClearTTYCache(int target)
        {
            return Is32Bit() ? ClearTTYCacheX86(target) : ClearTTYCacheX64(target);
        }

        public static SNRESULT Kick()
        {
            if (!Is32Bit())
                return KickX64();
            else
                return KickX86();
        }

        public static SNRESULT GetStatus(int target, UnitType unit, out UnitStatus unitStatus)
        {
            long status;
            SNRESULT snresult = Is32Bit() ? GetStatusX86(target, unit, out status, IntPtr.Zero) : GetStatusX64(target, unit, out status, IntPtr.Zero);
            unitStatus = (UnitStatus)status;
            return snresult;
        }

        public static SNRESULT ProcessLoad(int target, uint priority, string fileName, string[] argv, string[] envv, out uint processID, out ulong threadID, PS3TMAPI.LoadFlag loadFlags)
        {
            int argCount = 0;
            if (argv != null)
                argCount = argv.Length;
            int envCount = 0;
            if (envv != null)
                envCount = envv.Length;
            if (!Is32Bit())
                return ProcessLoadX64(target, priority, fileName, argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
            else
                return ProcessLoadX86(target, priority, fileName, argCount, argv, envCount, envv, out processID, out threadID, (uint)loadFlags);
        }

        public static SNRESULT GetProcessList(int target, out uint[] processIDs)
        {
            processIDs = (uint[])null;
            IntPtr processIdArray = IntPtr.Zero;
            uint count = 0U;
            SNRESULT res1 = Is32Bit() ? GetProcessListX86(target, ref count, processIdArray) : GetProcessListX64(target, ref count, processIdArray);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal(4 * (int)count);
            SNRESULT res2 = Is32Bit() ? GetProcessListX86(target, ref count, num) : GetProcessListX64(target, ref count, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                processIDs = new uint[count];
                for (uint index = 0U; index < count; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetUserProcessList(int target, out uint[] processIDs)
        {
            IntPtr processIdArray = IntPtr.Zero;
            uint count = 0U;
            SNRESULT res1 = Is32Bit() ? GetUserProcessListX86(target, ref count, processIdArray) : GetUserProcessListX64(target, ref count, processIdArray);
            if (FAILED(res1))
            {
                processIDs = (uint[])null;
                return res1;
            }
            else
            {
                IntPtr num = Marshal.AllocHGlobal(4 * (int)count);
                SNRESULT res2 = Is32Bit() ? GetUserProcessListX86(target, ref count, num) : GetUserProcessListX64(target, ref count, num);
                if (FAILED(res2))
                {
                    Marshal.FreeHGlobal(num);
                    processIDs = (uint[])null;
                    return res2;
                }
                else
                {
                    IntPtr unmanagedBuf = num;
                    processIDs = new uint[count];
                    for (uint index = 0U; index < count; ++index)
                        unmanagedBuf = ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref processIDs[index]);
                    Marshal.FreeHGlobal(num);
                    return res2;
                }
            }
        }

        public static SNRESULT ProcessStop(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessStopX64(target, processID);
            else
                return ProcessStopX86(target, processID);
        }

        public static SNRESULT ProcessContinue(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessContinueX64(target, processID);
            else
                return ProcessContinueX86(target, processID);
        }

        public static SNRESULT ProcessKill(int target, uint processID)
        {
            if (!Is32Bit())
                return ProcessKillX64(target, processID);
            else
                return ProcessKillX86(target, processID);
        }

        public static SNRESULT TerminateGameProcess(int target, uint processID, uint timeout)
        {
            if (!Is32Bit())
                return TerminateGameProcessX64(target, processID, timeout);
            else
                return TerminateGameProcessX86(target, processID, timeout);
        }

        public static SNRESULT GetThreadList(int target, uint processID, out ulong[] ppuThreadIDs, out ulong[] spuThreadGroupIDs)
        {
            ppuThreadIDs = (ulong[])null;
            spuThreadGroupIDs = (ulong[])null;
            uint numPPUThreads = 0U;
            uint numSPUThreadGroups = 0U;
            SNRESULT res = Is32Bit() ? GetThreadListX86(target, processID, ref numPPUThreads, (ulong[])null, ref numSPUThreadGroups, (ulong[])null) : GetThreadListX64(target, processID, ref numPPUThreads, (ulong[])null, ref numSPUThreadGroups, (ulong[])null);
            if (FAILED(res))
                return res;
            ppuThreadIDs = new ulong[numPPUThreads];
            spuThreadGroupIDs = new ulong[numSPUThreadGroups];
            return Is32Bit() ? GetThreadListX86(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs) : GetThreadListX64(target, processID, ref numPPUThreads, ppuThreadIDs, ref numSPUThreadGroups, spuThreadGroupIDs);
        }

        public static SNRESULT ThreadStop(int target, UnitType unit, uint processID, ulong threadID)
        {
            if (!Is32Bit())
                return ThreadStopX64(target, unit, processID, threadID);
            else
                return ThreadStopX86(target, unit, processID, threadID);
        }

        public static SNRESULT ThreadContinue(int target, UnitType unit, uint processID, ulong threadID)
        {
            if (!Is32Bit())
                return ThreadContinueX64(target, unit, processID, threadID);
            else
                return ThreadContinueX86(target, unit, processID, threadID);
        }

        public static SNRESULT ThreadGetRegisters(int target, UnitType unit, uint processID, ulong threadID, uint[] registerNums, out ulong[] registerValues)
        {
            registerValues = (ulong[])null;
            if (registerNums == null)
                return SNRESULT.SN_E_BAD_PARAM;
            registerValues = new ulong[registerNums.Length];
            if (!Is32Bit())
                return ThreadGetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            else
                return ThreadGetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        public static SNRESULT ThreadSetRegisters(int target, UnitType unit, uint processID, ulong threadID, uint[] registerNums, ulong[] registerValues)
        {
            if (registerNums == null || registerValues == null || registerNums.Length != registerValues.Length)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return ThreadSetRegistersX64(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
            else
                return ThreadSetRegistersX86(target, unit, processID, threadID, (uint)registerNums.Length, registerNums, registerValues);
        }

        private static void ProcessInfoMarshalHelper(IntPtr unmanagedBuf, ref ProcessInfo processInfo)
        {
            IntPtr unmanagedBuf1 = ReadDataFromUnmanagedIncPtr<PS3TMAPI.ProcessInfoHdr>(unmanagedBuf, ref processInfo.Hdr);
            uint num = processInfo.Hdr.NumPPUThreads + processInfo.Hdr.NumSPUThreads;
            processInfo.ThreadIDs = new ulong[num];
            for (int index = 0; (long)index < (long)num; ++index)
                unmanagedBuf1 = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref processInfo.ThreadIDs[index]);
        }

        public static SNRESULT GetProcessInfo(int target, uint processID, out ProcessInfo processInfo)
        {
            processInfo = new ProcessInfo();
            IntPtr processInfo1 = IntPtr.Zero;
            uint bufferSize = 0U;
            SNRESULT res1 = Is32Bit() ? GetProcessInfoX86(target, processID, ref bufferSize, processInfo1) : GetProcessInfoX64(target, processID, ref bufferSize, processInfo1);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetProcessInfoX86(target, processID, ref bufferSize, num) : GetProcessInfoX64(target, processID, ref bufferSize, num);
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(num, ref processInfo);
            Marshal.FreeHGlobal(num);
            return res2;
        }

        public static SNRESULT GetProcessInfoEx(int target, uint processID, out ProcessInfo processInfo, out ExtraProcessInfo extraProcessInfo)
        {
            processInfo = new ProcessInfo();
            extraProcessInfo = new ExtraProcessInfo();
            IntPtr processInfo1 = IntPtr.Zero;
            uint bufferSize = 0U;
            SNRESULT res1 = Is32Bit() ? GetProcessInfoExX86(target, processID, ref bufferSize, processInfo1, out extraProcessInfo) : GetProcessInfoExX64(target, processID, ref bufferSize, processInfo1, out extraProcessInfo);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetProcessInfoExX86(target, processID, ref bufferSize, num, out extraProcessInfo) : GetProcessInfoExX64(target, processID, ref bufferSize, num, out extraProcessInfo);
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(num, ref processInfo);
            Marshal.FreeHGlobal(num);
            return res2;
        }

        public static SNRESULT GetProcessInfoEx2(int target, uint processID, out ProcessInfo processInfo, out ExtraProcessInfo extraProcessInfo, out ProcessLoadInfo processLoadInfo)
        {
            IntPtr processInfo1 = IntPtr.Zero;
            uint bufferSize = 0U;
            processInfo = new ProcessInfo();
            extraProcessInfo = new ExtraProcessInfo();
            processLoadInfo = new ProcessLoadInfo();
            SNRESULT res1 = Is32Bit() ? GetProcessInfoEx2X86(target, processID, ref bufferSize, processInfo1, out extraProcessInfo, out processLoadInfo) : GetProcessInfoEx2X64(target, processID, ref bufferSize, processInfo1, out extraProcessInfo, out processLoadInfo);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetProcessInfoEx2X86(target, processID, ref bufferSize, num, out extraProcessInfo, out processLoadInfo) : PS3TMAPI.GetProcessInfoEx2X64(target, processID, ref bufferSize, num, out extraProcessInfo, out processLoadInfo);
            if (SUCCEEDED(res2))
                ProcessInfoMarshalHelper(num, ref processInfo);
            Marshal.FreeHGlobal(num);
            return res2;
        }

        public static SNRESULT GetModuleList(int target, uint processID, out uint[] modules)
        {
            modules = (uint[])null;
            uint numModules = 0U;
            SNRESULT res = Is32Bit() ? GetModuleListX86(target, processID, ref numModules, (uint[])null) : GetModuleListX64(target, processID, ref numModules, (uint[])null);
            if (FAILED(res))
                return res;
            modules = new uint[numModules];
            return Is32Bit() ? GetModuleListX86(target, processID, ref numModules, modules) : GetModuleListX64(target, processID, ref numModules, modules);
        }

        public static SNRESULT GetModuleInfo(int target, uint processID, uint moduleID, out ModuleInfo moduleInfo)
        {
            moduleInfo = new ModuleInfo();
            IntPtr moduleInfo1 = IntPtr.Zero;
            ulong bufferSize = 0UL;
            SNRESULT res1 = Is32Bit() ? GetModuleInfoX86(target, processID, moduleID, ref bufferSize, moduleInfo1) : GetModuleInfoX64(target, processID, moduleID, ref bufferSize, moduleInfo1);
            if (FAILED(res1))
                return res1;
            if (bufferSize > (ulong)int.MaxValue)
                return SNRESULT.SN_E_ERROR;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetModuleInfoX86(target, processID, moduleID, ref bufferSize, num) : GetModuleInfoX64(target, processID, moduleID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.ModuleInfoHdr>(num, ref moduleInfo.Hdr);
                moduleInfo.Segments = new PRXSegment[moduleInfo.Hdr.NumSegments];
                for (int index = 0; (long)index < (long)moduleInfo.Hdr.NumSegments; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.PRXSegment>(unmanagedBuf, ref moduleInfo.Segments[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetModuleInfoEx(int target, uint processID, uint moduleID, out ModuleInfoEx moduleInfoEx, out MSELFInfo mselfInfo, out ExtraModuleInfo extraModuleInfo)
        {
            moduleInfoEx = new ModuleInfoEx();
            mselfInfo = new MSELFInfo();
            extraModuleInfo = new ExtraModuleInfo();
            IntPtr moduleInfoEx1 = IntPtr.Zero;
            ulong bufferSize = 0UL;
            IntPtr mselfInfo1 = IntPtr.Zero;
            SNRESULT res = Is32Bit() ? GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, moduleInfoEx1, out mselfInfo1, out extraModuleInfo) : GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, moduleInfoEx1, out mselfInfo1, out extraModuleInfo);
            if (FAILED(res))
                return res;
            if (bufferSize > (ulong)int.MaxValue)
                return SNRESULT.SN_E_ERROR;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res1 = Is32Bit() ? GetModuleInfoExX86(target, processID, moduleID, ref bufferSize, num, out mselfInfo1, out extraModuleInfo) : GetModuleInfoExX64(target, processID, moduleID, ref bufferSize, num, out mselfInfo1, out extraModuleInfo);
            if (FAILED(res1))
            {
                Marshal.FreeHGlobal(num);
                return res1;
            }
            else
            {
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.ModuleInfoHdr>(num, ref moduleInfoEx.Hdr);
                moduleInfoEx.Segments = new PRXSegmentEx[moduleInfoEx.Hdr.NumSegments];
                for (int index = 0; (long)index < (long)moduleInfoEx.Hdr.NumSegments; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.PRXSegmentEx>(unmanagedBuf, ref moduleInfoEx.Segments[index]);
                ReadDataFromUnmanagedIncPtr<PS3TMAPI.MSELFInfo>(mselfInfo1, ref mselfInfo);
                Marshal.FreeHGlobal(num);
                return res1;
            }
        }

        public static SNRESULT GetPPUThreadInfo(int target, uint processID, ulong threadID, out PPUThreadInfo threadInfo)
        {
            threadInfo = new PPUThreadInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetThreadInfoX86(target, UnitType.PPU, processID, threadID, ref bufferSize, buffer) : GetThreadInfoX64(target, UnitType.PPU, processID, threadID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetThreadInfoX86(target, UnitType.PPU, processID, threadID, ref bufferSize, num) : GetThreadInfoX64(target, UnitType.PPU, processID, threadID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                PPUThreadInfoPriv storage = new PPUThreadInfoPriv();
                IntPtr ptr = ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadInfoPriv>(num, ref storage);
                threadInfo.ThreadID = storage.ThreadID;
                threadInfo.Priority = storage.Priority;
                threadInfo.State = (PPUThreadState)storage.State;
                threadInfo.StackAddress = storage.StackAddress;
                threadInfo.StackSize = storage.StackSize;
                if (storage.ThreadNameLen > 0U)
                    threadInfo.ThreadName = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetPPUThreadInfoEx(int target, uint processID, ulong threadID, out PPUThreadInfoEx threadInfoEx)
        {
            threadInfoEx = new PPUThreadInfoEx();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, buffer) : GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetPPUThreadInfoExX86(target, processID, threadID, ref bufferSize, num) : GetPPUThreadInfoExX64(target, processID, threadID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                PPUThreadInfoExPriv storage = new PPUThreadInfoExPriv();
                IntPtr ptr = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadInfoExPriv>(num, ref storage);
                threadInfoEx.ThreadID = storage.ThreadId;
                threadInfoEx.Priority = storage.Priority;
                threadInfoEx.BasePriority = storage.BasePriority;
                threadInfoEx.State = (PPUThreadState)storage.State;
                threadInfoEx.StackAddress = storage.StackAddress;
                threadInfoEx.StackSize = storage.StackSize;
                if (storage.ThreadNameLen > 0U)
                    threadInfoEx.ThreadName = Marshal.PtrToStringAnsi(ptr);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetSPUThreadInfo(int target, uint processID, ulong threadID, out SPUThreadInfo threadInfo)
        {
            threadInfo = new SPUThreadInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetThreadInfoX86(target, UnitType.SPU, processID, threadID, ref bufferSize, buffer) : GetThreadInfoX64(target, UnitType.SPU, processID, threadID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetThreadInfoX86(target, UnitType.SPU, processID, threadID, ref bufferSize, num) : GetThreadInfoX64(target, UnitType.SPU, processID, threadID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                SpuThreadInfoPriv storage = new SpuThreadInfoPriv();
                IntPtr ptr1 = ReadDataFromUnmanagedIncPtr<PS3TMAPI.SpuThreadInfoPriv>(num, ref storage);
                threadInfo.ThreadGroupID = storage.ThreadGroupId;
                threadInfo.ThreadID = storage.ThreadId;
                if (storage.FilenameLen > 0U)
                    threadInfo.Filename = Marshal.PtrToStringAnsi(ptr1);
                IntPtr ptr2 = new IntPtr(ptr1.ToInt64() + (long)storage.FilenameLen);
                if (storage.ThreadNameLen > 0U)
                    threadInfo.ThreadName = Marshal.PtrToStringAnsi(ptr2);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT SetDefaultPPUThreadStackSize(int target, ELFStackSize stackSize)
        {
            if (!Is32Bit())
                return SetDefaultPPUThreadStackSizeX64(target, stackSize);
            else
                return SetDefaultPPUThreadStackSizeX86(target, stackSize);
        }

        public static SNRESULT GetDefaultPPUThreadStackSize(int target, out ELFStackSize stackSize)
        {
            if (!Is32Bit())
                return GetDefaultPPUThreadStackSizeX64(target, out stackSize);
            else
                return GetDefaultPPUThreadStackSizeX86(target, out stackSize);
        }

        public static SNRESULT SetSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            int bCurrentPc = bCurrentPC ? 1 : 0;
            if (!Is32Bit())
                return SetSPULoopPointX64(target, processID, threadID, address, bCurrentPc);
            else
                return SetSPULoopPointX86(target, processID, threadID, address, bCurrentPc);
        }

        public static SNRESULT ClearSPULoopPoint(int target, uint processID, ulong threadID, uint address, bool bCurrentPC)
        {
            if (!Is32Bit())
                return ClearSPULoopPointX64(target, processID, threadID, address, bCurrentPC);
            else
                return ClearSPULoopPointX86(target, processID, threadID, address, bCurrentPC);
        }
        
        public static SNRESULT SetBreakPoint(int target, UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!Is32Bit())
                return SetBreakPointX64(target, (uint)unit, processID, threadID, address);
            else
                return SetBreakPointX86(target, (uint)unit, processID, threadID, address);
        }
        
        public static SNRESULT ClearBreakPoint(int target, UnitType unit, uint processID, ulong threadID, ulong address)
        {
            if (!Is32Bit())
                return ClearBreakPointX64(target, (uint)unit, processID, threadID, address);
            else
                return ClearBreakPointX86(target, (uint)unit, processID, threadID, address);
        }

        public static SNRESULT GetBreakPoints(int target, UnitType unit, uint processID, ulong threadID, out ulong[] bpAddresses)
        {
            bpAddresses = (ulong[])null;
            uint numBreakpoints;
            SNRESULT res = Is32Bit() ? GetBreakPointsX86(target, (uint)unit, processID, threadID, out numBreakpoints, (ulong[])null) : GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, (ulong[])null);
            if (FAILED(res))
                return res;
            bpAddresses = new ulong[numBreakpoints];
            if (!Is32Bit())
                return GetBreakPointsX64(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
            else
                return GetBreakPointsX86(target, (uint)unit, processID, threadID, out numBreakpoints, bpAddresses);
        }

        public static SNRESULT GetDebugThreadControlInfo(int target, uint processID, out DebugThreadControlInfo threadCtrlInfo)
        {
            threadCtrlInfo = new DebugThreadControlInfo();
            IntPtr buffer = IntPtr.Zero;
            uint bufferSize = 0U;
            SNRESULT res1 = Is32Bit() ? GetDebugThreadControlInfoX86(target, processID, ref bufferSize, buffer) : GetDebugThreadControlInfoX64(target, processID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num1 = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetDebugThreadControlInfoX86(target, processID, ref bufferSize, num1) : GetDebugThreadControlInfoX64(target, processID, ref bufferSize, num1);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num1);
                return res2;
            }
            else
            {
                PS3TMAPI.DebugThreadControlInfoPriv storage = new PS3TMAPI.DebugThreadControlInfoPriv();
                IntPtr unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.DebugThreadControlInfoPriv>(num1, ref storage);
                threadCtrlInfo.ControlFlags = storage.ControlFlags;
                uint num2 = storage.NumEntries;
                threadCtrlInfo.ControlKeywords = new PS3TMAPI.ControlKeywordEntry[num2];
                for (uint index = 0U; index < num2; ++index)
                    unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.ControlKeywordEntry>(unmanagedBuf, ref threadCtrlInfo.ControlKeywords[index]);
                Marshal.FreeHGlobal(num1);
                return res2;
            }
        }

        public static SNRESULT SetDebugThreadControlInfo(int target, uint processID, DebugThreadControlInfo threadCtrlInfo, out uint maxEntries)
        {
            DebugThreadControlInfoPriv storage = new DebugThreadControlInfoPriv();
            storage.ControlFlags = threadCtrlInfo.ControlFlags;
            if (threadCtrlInfo.ControlKeywords != null)
                storage.NumEntries = (uint)threadCtrlInfo.ControlKeywords.Length;
            IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object)storage) + (int)storage.NumEntries * Marshal.SizeOf(typeof(PS3TMAPI.ControlKeywordEntry)));
            IntPtr unmanagedBuf = WriteDataToUnmanagedIncPtr<PS3TMAPI.DebugThreadControlInfoPriv>(storage, num);
            for (int index = 0; (long)index < (long)storage.NumEntries; ++index)
                unmanagedBuf = WriteDataToUnmanagedIncPtr<PS3TMAPI.ControlKeywordEntry>(threadCtrlInfo.ControlKeywords[index], unmanagedBuf);
            SNRESULT snresult = Is32Bit() ? SetDebugThreadControlInfoX86(target, processID, num, out maxEntries) : SetDebugThreadControlInfoX64(target, processID, num, out maxEntries);
            Marshal.FreeHGlobal(num);
            return snresult;
        }

        public static SNRESULT ThreadExceptionClean(int target, uint processID, ulong threadID)
        {
            return Is32Bit() ? ThreadExceptionCleanX86(target, processID, threadID) : ThreadExceptionCleanX64(target, processID, threadID);
        }

        public static SNRESULT GetRawSPULogicalIDs(int target, uint processID, out ulong[] logicalIDs)
        {
            logicalIDs = new ulong[8];
            if (!Is32Bit())
                return GetRawSPULogicalIdsX64(target, processID, logicalIDs);
            else
                return GetRawSPULogicalIdsX86(target, processID, logicalIDs);
        }

        public static SNRESULT SPUThreadGroupStop(int target, uint processID, ulong threadGroupID)
        {
            if (!Is32Bit())
                return SPUThreadGroupStopX64(target, processID, threadGroupID);
            else
                return SPUThreadGroupStopX86(target, processID, threadGroupID);
        }

        public static SNRESULT SPUThreadGroupContinue(int target, uint processID, ulong threadGroupID)
        {
            if (!Is32Bit())
                return SPUThreadGroupContinueX64(target, processID, threadGroupID);
            else
                return SPUThreadGroupContinueX86(target, processID, threadGroupID);
        }

        public static SNRESULT GetProcessTree(int target, out ProcessTreeBranch[] processTree)
        {
            processTree = (ProcessTreeBranch[])null;
            IntPtr buffer = IntPtr.Zero;
            uint numProcesses = 0U;
            SNRESULT res1 = Is32Bit() ? GetProcessTreeX86(target, ref numProcesses, buffer) : GetProcessTreeX64(target, ref numProcesses, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)numProcesses * Marshal.SizeOf(typeof(ProcessTreeBranchPriv)));
            SNRESULT res2 = Is32Bit() ? GetProcessTreeX86(target, ref numProcesses, num) : GetProcessTreeX64(target, ref numProcesses, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                processTree = new ProcessTreeBranch[numProcesses];
                for (int index1 = 0; (long)index1 < (long)numProcesses; ++index1)
                {
                    ProcessTreeBranchPriv processTreeBranchPriv = (ProcessTreeBranchPriv)Marshal.PtrToStructure(num, typeof(ProcessTreeBranchPriv));
                    processTree[index1].ProcessID = processTreeBranchPriv.ProcessId;
                    processTree[index1].ProcessState = processTreeBranchPriv.ProcessState;
                    processTree[index1].ProcessFlags = processTreeBranchPriv.ProcessFlags;
                    processTree[index1].RawSPU = processTreeBranchPriv.RawSPU;
                    processTree[index1].PPUThreadStatuses = new PPUThreadStatus[processTreeBranchPriv.NumPpuThreads];
                    processTree[index1].SPUThreadGroupStatuses = new SPUThreadGroupStatus[processTreeBranchPriv.NumSpuThreadGroups];
                    for (int index2 = 0; (long)index2 < (long)processTreeBranchPriv.NumPpuThreads; ++index2)
                        processTreeBranchPriv.PpuThreadStatuses = ReadDataFromUnmanagedIncPtr<PS3TMAPI.PPUThreadStatus>(processTreeBranchPriv.PpuThreadStatuses, ref processTree[index1].PPUThreadStatuses[index2]);
                    for (int index2 = 0; (long)index2 < (long)processTreeBranchPriv.NumSpuThreadGroups; ++index2)
                        processTreeBranchPriv.SpuThreadGroupStatuses = ReadDataFromUnmanagedIncPtr<PS3TMAPI.SPUThreadGroupStatus>(processTreeBranchPriv.SpuThreadGroupStatuses, ref processTree[index1].SPUThreadGroupStatuses[index2]);
                }
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetSPUThreadGroupInfo(int target, uint processID, ulong threadGroupID, out PS3TMAPI.SPUThreadGroupInfo threadGroupInfo)
        {
            threadGroupInfo = new SPUThreadGroupInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, buffer) : GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num1 = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetSPUThreadGroupInfoX86(target, processID, threadGroupID, ref bufferSize, num1) : GetSPUThreadGroupInfoX64(target, processID, threadGroupID, ref bufferSize, num1);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num1);
                return res2;
            }
            else
            {
                SpuThreadGroupInfoPriv storage = new SpuThreadGroupInfoPriv();
                IntPtr num2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.SpuThreadGroupInfoPriv>(num1, ref storage);
                threadGroupInfo.ThreadGroupID = storage.ThreadGroupId;
                threadGroupInfo.State = (SPUThreadGroupState)storage.State;
                threadGroupInfo.Priority = storage.Priority;
                threadGroupInfo.ThreadIDs = new uint[storage.NumThreads];
                for (int index = 0; (long)index < (long)storage.NumThreads; ++index)
                    num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref threadGroupInfo.ThreadIDs[index]);
                if (storage.ThreadGroupNameLen > 0U)
                    threadGroupInfo.GroupName = Marshal.PtrToStringAnsi(num2);
                Marshal.FreeHGlobal(num1);
                return res2;
            }
        }

        public static SNRESULT ProcessGetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return ProcessGetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            else
                return ProcessGetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }


        public static SNRESULT ProcessSetMemorySize(int target, UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer, int size)
        {
            if (!Is32Bit())
                return ProcessSetMemoryX64(target, unit, processID, threadID, address, size, buffer);
            else
                return ProcessSetMemoryX86(target, unit, processID, threadID, address, size, buffer);
        }

        public static SNRESULT ProcessSetMemory(int target, UnitType unit, uint processID, ulong threadID, ulong address, byte[] buffer)
        {
            if (!Is32Bit())
                return ProcessSetMemoryX64(target, unit, processID, threadID, address, buffer.Length, buffer);
            else
                return ProcessSetMemoryX86(target, unit, processID, threadID, address, buffer.Length, buffer);
        }
        
        public static SNRESULT GetMemoryCompressed(int target, uint processID, MemoryCompressionLevel compressionLevel, uint address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return GetMemoryCompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            else
                return GetMemoryCompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        public static SNRESULT GetMemory64Compressed(int target, uint processID, MemoryCompressionLevel compressionLevel, ulong address, ref byte[] buffer)
        {
            if (!Is32Bit())
                return GetMemory64CompressedX64(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
            else
                return GetMemory64CompressedX86(target, processID, (uint)compressionLevel, address, (uint)buffer.Length, buffer);
        }

        public static SNRESULT GetVirtualMemoryInfo(int target, uint processID, bool bStatsOnly, out VirtualMemoryArea[] vmAreas)
        {
            vmAreas = (VirtualMemoryArea[])null;
            uint areaCount;
            uint bufferSize;
            SNRESULT res1 = Is32Bit() ? GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out areaCount, out bufferSize, IntPtr.Zero) : GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, IntPtr.Zero);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetVirtualMemoryInfoX86(target, processID, bStatsOnly, out areaCount, out bufferSize, num) : GetVirtualMemoryInfoX64(target, processID, bStatsOnly, out areaCount, out bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                vmAreas = new VirtualMemoryArea[areaCount];
                IntPtr unmanagedBuf1 = num;
                for (int index = 0; (long)index < (long)areaCount; ++index)
                {
                    IntPtr unmanagedBuf2 = ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref vmAreas[index].Address), ref vmAreas[index].Flags), ref vmAreas[index].VSize), ref vmAreas[index].Options), ref vmAreas[index].PageFaultPPU), ref vmAreas[index].PageFaultSPU), ref vmAreas[index].PageIn), ref vmAreas[index].PageOut), ref vmAreas[index].PMemTotal), ref vmAreas[index].PMemUsed), ref vmAreas[index].Time);
                    ulong storage1 = 0UL;
                    IntPtr unmanagedBuf3 = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf2, ref storage1);
                    vmAreas[index].Pages = new ulong[storage1];
                    IntPtr storage2 = IntPtr.Zero;
                    unmanagedBuf1 = ReadDataFromUnmanagedIncPtr<IntPtr>(unmanagedBuf3, ref storage2);
                }
                for (int index1 = 0; (long)index1 < (long)areaCount; ++index1)
                {
                    int length = vmAreas[index1].Pages.Length;
                    for (int index2 = 0; index2 < length; ++index2)
                        unmanagedBuf1 = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref vmAreas[index1].Pages[index2]);
                }
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetSyncPrimitiveCounts(int target, uint processID, out SyncPrimitiveCounts primitiveCounts)
        {
            primitiveCounts = new SyncPrimitiveCounts();
            uint bufferSize = 28U;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res = Is32Bit() ? GetSyncPrimitiveCountsExX86(target, processID, ref bufferSize, num) : GetSyncPrimitiveCountsExX64(target, processID, ref bufferSize, num);
            if (FAILED(res))
            {
                Marshal.FreeHGlobal(num);
                return res;
            }
            else
            {
                primitiveCounts = (SyncPrimitiveCounts)Marshal.PtrToStructure(num, typeof(SyncPrimitiveCounts));
                Marshal.FreeHGlobal(num);
                return res;
            }
        }

        public static SNRESULT GetMutexList(int target, uint processID, out uint[] mutexIDs)
        {
            mutexIDs = (uint[])null;
            uint numMutexes = 0U;
            SNRESULT res = Is32Bit() ? GetMutexListX86(target, processID, ref numMutexes, (uint[])null) : GetMutexListX64(target, processID, ref numMutexes, (uint[])null);
            if (FAILED(res))
                return res;
            mutexIDs = new uint[numMutexes];
            return Is32Bit() ? GetMutexListX86(target, processID, ref numMutexes, mutexIDs) : GetMutexListX64(target, processID, ref numMutexes, mutexIDs);
        }

        public static SNRESULT GetMutexInfo(int target, uint processID, uint mutexID, out MutexInfo mutexInfo)
        {
            mutexInfo = new MutexInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetMutexInfoX86(target, processID, mutexID, ref bufferSize, buffer) : GetMutexInfoX64(target, processID, mutexID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetMutexInfoX86(target, processID, mutexID, ref bufferSize, num) : GetMutexInfoX64(target, processID, mutexID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                MutexInfoPriv storage = new MutexInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.MutexInfoPriv>(num, ref storage);
                mutexInfo.ID = storage.Id;
                mutexInfo.Attribute = storage.Attribute;
                mutexInfo.OwnerThreadID = storage.OwnerThreadId;
                mutexInfo.LockCounter = storage.LockCounter;
                mutexInfo.ConditionRefCounter = storage.ConditionRefCounter;
                mutexInfo.ConditionVarID = storage.ConditionVarId;
                mutexInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
                mutexInfo.WaitingThreads = new ulong[storage.NumWaitingThreads];
                for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref mutexInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetLightWeightMutexList(int target, uint processID, out uint[] lwMutexIDs)
        {
            lwMutexIDs = (uint[])null;
            uint numLWMutexes = 0U;
            SNRESULT res = Is32Bit() ? GetLightWeightMutexListX86(target, processID, ref numLWMutexes, (uint[])null) : GetLightWeightMutexListX64(target, processID, ref numLWMutexes, (uint[])null);
            if (FAILED(res))
                return res;
            lwMutexIDs = new uint[numLWMutexes];
            return Is32Bit() ? GetLightWeightMutexListX86(target, processID, ref numLWMutexes, lwMutexIDs) : GetLightWeightMutexListX64(target, processID, ref numLWMutexes, lwMutexIDs);
        }

        public static SNRESULT GetLightWeightMutexInfo(int target, uint processID, uint lwMutexID, out PS3TMAPI.LWMutexInfo lwMutexInfo)
        {
            lwMutexInfo = new LWMutexInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, buffer) : GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetLightWeightMutexInfoX86(target, processID, lwMutexID, ref bufferSize, num) : GetLightWeightMutexInfoX64(target, processID, lwMutexID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                LwMutexInfoPriv storage = new LwMutexInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.LwMutexInfoPriv>(num, ref storage);
                lwMutexInfo.ID = storage.Id;
                lwMutexInfo.Attribute = storage.Attribute;
                lwMutexInfo.OwnerThreadID = storage.OwnerThreadId;
                lwMutexInfo.LockCounter = storage.LockCounter;
                lwMutexInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
                lwMutexInfo.WaitingThreads = new ulong[storage.NumWaitingThreads];
                for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref lwMutexInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetConditionalVariableList(int target, uint processID, out uint[] conditionVarIDs)
        {
            conditionVarIDs = (uint[])null;
            uint numConditionVars = 0U;
            SNRESULT res = Is32Bit() ? GetConditionalVariableListX86(target, processID, ref numConditionVars, (uint[])null) : GetConditionalVariableListX64(target, processID, ref numConditionVars, (uint[])null);
            if (FAILED(res))
                return res;
            conditionVarIDs = new uint[numConditionVars];
            return Is32Bit() ? GetConditionalVariableListX86(target, processID, ref numConditionVars, conditionVarIDs) : GetConditionalVariableListX64(target, processID, ref numConditionVars, conditionVarIDs);
        }

        public static SNRESULT GetConditionalVariableInfo(int target, uint processID, uint conditionVarID, out ConditionVarInfo conditionVarInfo)
        {
            conditionVarInfo = new ConditionVarInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, buffer) : GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetConditionalVariableInfoX86(target, processID, conditionVarID, ref bufferSize, num) : GetConditionalVariableInfoX64(target, processID, conditionVarID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                ConditionVarInfoPriv storage = new ConditionVarInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.ConditionVarInfoPriv>(num, ref storage);
                conditionVarInfo.ID = storage.Id;
                conditionVarInfo.Attribute = storage.Attribute;
                conditionVarInfo.MutexID = storage.MutexId;
                conditionVarInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
                conditionVarInfo.WaitingThreads = new ulong[storage.NumWaitingThreads];
                for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref conditionVarInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetLightWeightConditionalList(int target, uint processID, out uint[] lwConditionVarIDs)
        {
            lwConditionVarIDs = (uint[])null;
            uint numLWConditionVars = 0U;
            SNRESULT res = Is32Bit() ? GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, (uint[])null) : PS3TMAPI.GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, (uint[])null);
            if (FAILED(res))
                return res;
            lwConditionVarIDs = new uint[numLWConditionVars];
            return Is32Bit() ? GetLightWeightConditionalListX86(target, processID, ref numLWConditionVars, lwConditionVarIDs) : PS3TMAPI.GetLightWeightConditionalListX64(target, processID, ref numLWConditionVars, lwConditionVarIDs);
        }

        public static SNRESULT GetLightWeightConditionalInfo(int target, uint processID, uint lwCondVarID, out LWConditionVarInfo lwConditonVarInfo)
        {
            lwConditonVarInfo = new LWConditionVarInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, buffer) : GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetLightWeightConditionalInfoX86(target, processID, lwCondVarID, ref bufferSize, num) : GetLightWeightConditionalInfoX64(target, processID, lwCondVarID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                LwConditionVarInfoPriv storage = new LwConditionVarInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.LwConditionVarInfoPriv>(num, ref storage);
                lwConditonVarInfo = new LWConditionVarInfo();
                lwConditonVarInfo.ID = storage.Id;
                lwConditonVarInfo.Attribute = storage.Attribute;
                lwConditonVarInfo.LWMutexID = storage.LwMutexId;
                lwConditonVarInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
                lwConditonVarInfo.WaitingThreads = new ulong[storage.NumWaitingThreads];
                for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref lwConditonVarInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetReadWriteLockList(int target, uint processID, out uint[] rwLockList)
        {
            rwLockList = (uint[])null;
            uint numRWLocks = 0U;
            SNRESULT res = Is32Bit() ? GetReadWriteLockListX86(target, processID, ref numRWLocks, (uint[])null) : GetReadWriteLockListX64(target, processID, ref numRWLocks, (uint[])null);
            if (FAILED(res))
                return res;
            rwLockList = new uint[numRWLocks];
            return Is32Bit() ? GetReadWriteLockListX86(target, processID, ref numRWLocks, rwLockList) : GetReadWriteLockListX64(target, processID, ref numRWLocks, rwLockList);
        }

        public static SNRESULT GetReadWriteLockInfo(int target, uint processID, uint rwLockID, out RWLockInfo rwLockInfo)
        {
            rwLockInfo = new RWLockInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, buffer) : GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num1 = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetReadWriteLockInfoX86(target, processID, rwLockID, ref bufferSize, num1) : GetReadWriteLockInfoX64(target, processID, rwLockID, ref bufferSize, num1);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num1);
                return res2;
            }
            else
            {
                RwLockInfoPriv storage = new RwLockInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.RwLockInfoPriv>(num1, ref storage);
                rwLockInfo.ID = storage.Id;
                rwLockInfo.Attribute = storage.Attribute;
                rwLockInfo.NumWaitingReadThreads = storage.NumWaitingReadThreads;
                rwLockInfo.NumWaitAllReadThreads = storage.NumWaitAllReadThreads;
                rwLockInfo.NumWaitingWriteThreads = storage.NumWaitingWriteThreads;
                rwLockInfo.NumWaitAllWriteThreads = storage.NumWaitAllWriteThreads;
                uint num2 = rwLockInfo.NumWaitingReadThreads + rwLockInfo.NumWaitingWriteThreads;
                rwLockInfo.WaitingThreads = new ulong[num2];
                for (int index = 0; (long)index < (long)num2; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref rwLockInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num1);
                return res2;
            }
        }

        public static SNRESULT GetSemaphoreList(int target, uint processID, out uint[] semaphoreIDs)
        {
            semaphoreIDs = (uint[])null;
            uint numSemaphores = 0U;
            SNRESULT res = Is32Bit() ? GetSemaphoreListX86(target, processID, ref numSemaphores, (uint[])null) : GetSemaphoreListX64(target, processID, ref numSemaphores, (uint[])null);
            if (FAILED(res))
                return res;
            semaphoreIDs = new uint[numSemaphores];
            return Is32Bit() ? GetSemaphoreListX86(target, processID, ref numSemaphores, semaphoreIDs) : GetSemaphoreListX64(target, processID, ref numSemaphores, semaphoreIDs);
        }

        public static SNRESULT GetSemaphoreInfo(int target, uint processID, uint semaphoreID, out SemaphoreInfo semaphoreInfo)
        {
            semaphoreInfo = new SemaphoreInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, buffer) : GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetSemaphoreInfoX86(target, processID, semaphoreID, ref bufferSize, num) : GetSemaphoreInfoX64(target, processID, semaphoreID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                SemaphoreInfoPriv storage = new SemaphoreInfoPriv();
                IntPtr unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.SemaphoreInfoPriv>(num, ref storage);
                semaphoreInfo.ID = storage.Id;
                semaphoreInfo.Attribute = storage.Attribute;
                semaphoreInfo.MaxValue = storage.MaxValue;
                semaphoreInfo.CurrentValue = storage.CurrentValue;
                semaphoreInfo.NumWaitAllThreads = storage.NumWaitAllThreads;
                semaphoreInfo.WaitingThreads = new ulong[storage.NumWaitingThreads];
                for (int index = 0; (long)index < (long)storage.NumWaitingThreads; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf, ref semaphoreInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetEventQueueList(int target, uint processID, out uint[] eventQueueIDs)
        {
            eventQueueIDs = (uint[])null;
            uint numEventQueues = 0U;
            SNRESULT res = Is32Bit() ? GetEventQueueListX86(target, processID, ref numEventQueues, (uint[])null) : GetEventQueueListX64(target, processID, ref numEventQueues, (uint[])null);
            if (FAILED(res))
                return res;
            eventQueueIDs = new uint[numEventQueues];
            return Is32Bit() ? GetEventQueueListX86(target, processID, ref numEventQueues, eventQueueIDs) : GetEventQueueListX64(target, processID, ref numEventQueues, eventQueueIDs);
        }

        public static SNRESULT GetEventQueueInfo(int target, uint processID, uint eventQueueID, out EventQueueInfo eventQueueInfo)
        {
            eventQueueInfo = new EventQueueInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, buffer) : GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, buffer);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetEventQueueInfoX86(target, processID, eventQueueID, ref bufferSize, num) : GetEventQueueInfoX64(target, processID, eventQueueID, ref bufferSize, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                EventQueueInfoPriv eventQueueInfoPriv = (EventQueueInfoPriv)Marshal.PtrToStructure(num, typeof(EventQueueInfoPriv));
                eventQueueInfo.ID = eventQueueInfoPriv.Id;
                eventQueueInfo.Attribute = eventQueueInfoPriv.Attribute;
                eventQueueInfo.Key = eventQueueInfoPriv.Key;
                eventQueueInfo.Size = eventQueueInfoPriv.Size;
                eventQueueInfo.NumWaitAllThreads = eventQueueInfoPriv.NumWaitAllThreads;
                eventQueueInfo.NumReadableAllEvQueue = eventQueueInfoPriv.NumReadableAllEvQueue;
                eventQueueInfo.WaitingThreadIDs = new ulong[eventQueueInfoPriv.NumWaitingThreads];
                IntPtr unmanagedBuf1 = eventQueueInfoPriv.WaitingThreadIds;
                for (int index = 0; (long)index < (long)eventQueueInfoPriv.NumWaitingThreads; ++index)
                    unmanagedBuf1 = ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref eventQueueInfo.WaitingThreadIDs[index]);
                eventQueueInfo.QueueEntries = new SystemEvent[eventQueueInfoPriv.NumReadableEvQueue];
                IntPtr unmanagedBuf2 = eventQueueInfoPriv.QueueEntries;
                for (int index = 0; (long)index < (long)eventQueueInfoPriv.NumReadableEvQueue; ++index)
                    unmanagedBuf2 = ReadDataFromUnmanagedIncPtr<PS3TMAPI.SystemEvent>(unmanagedBuf2, ref eventQueueInfo.QueueEntries[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetEventFlagList(int target, uint processID, out uint[] eventFlagIDs)
        {
            eventFlagIDs = (uint[])null;
            uint numEventFlags = 0U;
            SNRESULT res = Is32Bit() ? GetEventFlagListX86(target, processID, ref numEventFlags, (uint[])null) : GetEventFlagListX64(target, processID, ref numEventFlags, (uint[])null);
            if (FAILED(res))
                return res;
            eventFlagIDs = new uint[numEventFlags];
            return Is32Bit() ? GetEventFlagListX86(target, processID, ref numEventFlags, eventFlagIDs) : GetEventFlagListX64(target, processID, ref numEventFlags, eventFlagIDs);
        }

        public static SNRESULT GetEventFlagInfo(int target, uint processID, uint eventFlagID, out PS3TMAPI.EventFlagInfo eventFlagInfo)
        {
            eventFlagInfo = new EventFlagInfo();
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, buffer) : PS3TMAPI.GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, buffer);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetEventFlagInfoX86(target, processID, eventFlagID, ref bufferSize, num) : PS3TMAPI.GetEventFlagInfoX64(target, processID, eventFlagID, ref bufferSize, num);
            if (PS3TMAPI.FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf1 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.EventFlagAttr>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(num, ref eventFlagInfo.ID), ref eventFlagInfo.Attribute), ref eventFlagInfo.BitPattern);
                uint storage = 0U;
                IntPtr unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf1, ref storage), ref eventFlagInfo.NumWaitAllThreads);
                eventFlagInfo.WaitingThreads = new PS3TMAPI.EventFlagWaitThread[storage];
                for (int index = 0; (long)index < (long)storage; ++index)
                    unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.EventFlagWaitThread>(unmanagedBuf2, ref eventFlagInfo.WaitingThreads[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT PickTarget(IntPtr hWndOwner, out int target)
        {
            if (!Is32Bit())
                return PickTargetX64(hWndOwner, out target);
            else
                return PickTargetX86(hWndOwner, out target);
        }

        public static SNRESULT EnableAutoStatusUpdate(int target, bool bEnabled, out bool bPreviousState)
        {
            uint enabled = bEnabled ? 1U : 0U;
            uint previousState;
            SNRESULT snresult = Is32Bit() ? EnableAutoStatusUpdateX86(target, enabled, out previousState) : EnableAutoStatusUpdateX64(target, enabled, out previousState);
            bPreviousState = (int)previousState != 0;
            return snresult;
        }

        public static SNRESULT GetPowerStatus(int target, out PowerStatus status)
        {
            if (!Is32Bit())
                return GetPowerStatusX64(target, out status);
            else
                return GetPowerStatusX86(target, out status);
        }

        public static SNRESULT PowerOn(int target)
        {
            if (!Is32Bit())
                return PowerOnX64(target);
            else
                return PowerOnX86(target);
        }

        public static SNRESULT PowerOff(int target, bool bForce)
        {
            uint force = bForce ? 1U : 0U;
            if (!Is32Bit())
                return PowerOffX64(target, force);
            else
                return PowerOffX86(target, force);
        }
       
        public static SNRESULT GetUserMemoryStats(int target, uint processID, out UserMemoryStats memoryStats)
        {
            memoryStats = new UserMemoryStats();
            if (!Is32Bit())
                return GetUserMemoryStatsX64(target, processID, out memoryStats);
            else
                return GetUserMemoryStatsX86(target, processID, out memoryStats);
        }

        public static SNRESULT SetDefaultLoadPriority(int target, uint priority)
        {
            if (!Is32Bit())
                return SetDefaultLoadPriorityX64(target, priority);
            else
                return SetDefaultLoadPriorityX86(target, priority);
        }

        public static SNRESULT GetDefaultLoadPriority(int target, out uint priority)
        {
            if (!Is32Bit())
                return GetDefaultLoadPriorityX64(target, out priority);
            else
                return GetDefaultLoadPriorityX86(target, out priority);
        }

        public static SNRESULT GetGamePortIPAddrData(int target, string deviceName, out GamePortIPAddressData ipAddressData)
        {
            ipAddressData = new GamePortIPAddressData();
            if (!Is32Bit())
                return GetGamePortIPAddrDataX64(target, deviceName, out ipAddressData);
            else
                return GetGamePortIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        public static SNRESULT GetGamePortDebugIPAddrData(int target, string deviceName, out GamePortIPAddressData ipAddressData)
        {
            ipAddressData = new GamePortIPAddressData();
            if (!Is32Bit())
                return GetGamePortDebugIPAddrDataX64(target, deviceName, out ipAddressData);
            else
                return GetGamePortDebugIPAddrDataX86(target, deviceName, out ipAddressData);
        }

        public static SNRESULT SetDABR(int target, uint processID, ulong address)
        {
            if (!Is32Bit())
                return SetDABRX64(target, processID, address);
            else
                return SetDABRX86(target, processID, address);
        }

        public static SNRESULT GetDABR(int target, uint processID, out ulong address)
        {
            if (!Is32Bit())
                return GetDABRX64(target, processID, out address);
            else
                return GetDABRX86(target, processID, out address);
        }

        public static SNRESULT SetRSXProfilingFlags(int target, RSXProfilingFlag rsxFlags)
        {
            if (!Is32Bit())
                return SetRSXProfilingFlagsX64(target, (ulong)rsxFlags);
            else
                return SetRSXProfilingFlagsX86(target, (ulong)rsxFlags);
        }

        public static SNRESULT GetRSXProfilingFlags(int target, out RSXProfilingFlag rsxFlags)
        {
            ulong rsxFlags1;
            SNRESULT snresult = Is32Bit() ? GetRSXProfilingFlagsX86(target, out rsxFlags1) : GetRSXProfilingFlagsX64(target, out rsxFlags1);
            rsxFlags = (RSXProfilingFlag)rsxFlags1;
            return snresult;
        }

        public static SNRESULT SetCustomParamSFOMappingDirectory(int target, string paramSFODir)
        {
            if (!Is32Bit())
                return SetCustomParamSFOMappingDirectoryX64(target, paramSFODir);
            else
                return SetCustomParamSFOMappingDirectoryX86(target, paramSFODir);
        }

        public static SNRESULT EnableXMBSettings(int target, bool bEnable)
        {
            int enable = bEnable ? 1 : 0;
            if (!Is32Bit())
                return EnableXMBSettingsX64(target, enable);
            else
                return EnableXMBSettingsX86(target, enable);
        }
        //Use
        public static SNRESULT GetXMBSettings(int target, out string xmbSettings, bool bUpdateCache)
        {
            xmbSettings = (string)null;
            uint bufferSize = 0U;
            IntPtr buffer = IntPtr.Zero;
            SNRESULT res1 = Is32Bit() ? GetXMBSettingsX86(target, buffer, ref bufferSize, bUpdateCache) : GetXMBSettingsX64(target, buffer, ref bufferSize, bUpdateCache);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)bufferSize);
            SNRESULT res2 = Is32Bit() ? GetXMBSettingsX86(target, num, ref bufferSize, bUpdateCache) : GetXMBSettingsX64(target, num, ref bufferSize, bUpdateCache);
            if (SUCCEEDED(res2))
                xmbSettings = Marshal.PtrToStringAnsi(num);
            Marshal.FreeHGlobal(num);
            return res2;
        }

        public static SNRESULT SetXMBSettings(int target, string xmbSettings, bool bUpdateCache)
        {
            return Is32Bit() ? SetXMBSettingsX86(target, xmbSettings, bUpdateCache) : SetXMBSettingsX64(target, xmbSettings, bUpdateCache);
        }

        public static SNRESULT FootswitchControl(int target, bool bEnabled)
        {
            uint enabled = bEnabled ? 1U : 0U;
            if (!Is32Bit())
                return FootswitchControlX64(target, enabled);
            else
                return FootswitchControlX86(target, enabled);
        }

        public static SNRESULT TriggerCoreDump(int target, uint processID, ulong userData1, ulong userData2, ulong userData3)
        {
            if (!Is32Bit())
                return TriggerCoreDumpX64(target, processID, userData1, userData2, userData3);
            else
                return TriggerCoreDumpX86(target, processID, userData1, userData2, userData3);
        }

        public static SNRESULT GetCoreDumpFlags(int target, out CoreDumpFlag flags)
        {
            ulong flags1;
            SNRESULT snresult = Is32Bit() ? GetCoreDumpFlagsX86(target, out flags1) : GetCoreDumpFlagsX64(target, out flags1);
            flags = (CoreDumpFlag)flags1;
            return snresult;
        }

        public static SNRESULT SetCoreDumpFlags(int tarSet, CoreDumpFlag flags)
        {
            if (Is32Bit())
                return SetCoreDumpFlagsX64(tarSet, (ulong)flags);
            else
                return SetCoreDumpFlagsX86(tarSet, (ulong)flags);
        }

        public static SNRESULT ProcessAttach(int target, UnitType unit, uint processID)
        {
            if (!Is32Bit())
                return ProcessAttachX64(target, (uint)unit, processID);
            else
                return ProcessAttachX86(target, (uint)unit, processID);
        }

        public static SNRESULT FlashTarget(int target, string updaterToolPath, string flashImagePath)
        {
            if (!Is32Bit())
                return FlashTargetX64(target, updaterToolPath, flashImagePath);
            else
                return FlashTargetX86(target, updaterToolPath, flashImagePath);
        }

        public static SNRESULT GetMACAddress(int target, out string macAddress)
        {
            IntPtr stringPtr;
            SNRESULT snresult = Is32Bit() ? GetMacAddressX86(target, out stringPtr) : GetMacAddressX64(target, out stringPtr);
            macAddress = Marshal.PtrToStringAnsi(stringPtr);
            return snresult;
        }

        public static SNRESULT ProcessScatteredSetMemory(int target, uint processID, ScatteredWrite[] writeData, out uint errorCode, out uint failedAddress)
        {
            errorCode = 0U;
            failedAddress = 0U;
            if (writeData == null || writeData.Length == 0)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            int length1 = writeData.Length;
            if (writeData[0].Data == null)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            int length2 = writeData[0].Data.Length;
            IntPtr num1 = Marshal.AllocHGlobal(length1 * (Marshal.SizeOf((object)writeData[0].Address) + length2));
            IntPtr num2 = num1;
            for (int index = 0; index < length1; ++index)
            {
                num2 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(writeData[index].Address, num2);
                if (writeData[index].Data == null || writeData[index].Data.Length != length2)
                {
                    Marshal.FreeHGlobal(num1);
                    return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
                }
                else
                {
                    Marshal.Copy(writeData[index].Data, 0, num2, writeData[index].Data.Length);
                    num2 = new IntPtr(num2.ToInt64() + (long)writeData[index].Data.Length);
                }
            }
            SNRESULT snresult = Is32Bit() ? ProcessScatteredSetMemoryX86(target, processID, (uint)length1, (uint)length2, num1, out errorCode, out failedAddress) : ProcessScatteredSetMemoryX64(target, processID, (uint)length1, (uint)length2, num1, out errorCode, out failedAddress);
            Marshal.FreeHGlobal(num1);
            return snresult;
        }

        public static SNRESULT GetMATRanges(int target, uint processID, out MATRange[] matRanges)
        {
            matRanges = (MATRange[])null;
            IntPtr matRanges1 = IntPtr.Zero;
            uint rangeCount = 0U;
            SNRESULT res1 = Is32Bit() ? GetMATRangesX86(target, processID, ref rangeCount, matRanges1) : GetMATRangesX64(target, processID, ref rangeCount, matRanges1);
            if (FAILED(res1))
                return res1;
            if ((int)rangeCount == 0)
            {
                matRanges = new MATRange[0];
                return SNRESULT.SN_S_OK;
            }
            else
            {
                IntPtr num = Marshal.AllocHGlobal((int)((long)(2 * Marshal.SizeOf(typeof(uint))) * (long)rangeCount));
                SNRESULT res2 = Is32Bit() ? GetMATRangesX86(target, processID, ref rangeCount, num) : GetMATRangesX64(target, processID, ref rangeCount, num);
                if (FAILED(res2))
                {
                    Marshal.FreeHGlobal(num);
                    return res2;
                }
                else
                {
                    IntPtr unmanagedBuf = num;
                    matRanges = new MATRange[rangeCount];
                    for (uint index = 0U; index < rangeCount; ++index)
                        unmanagedBuf = ReadDataFromUnmanagedIncPtr<uint>(ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf, ref matRanges[index].StartAddress), ref matRanges[index].Size);
                    Marshal.FreeHGlobal(num);
                    return res2;
                }
            }
        }

        public static SNRESULT GetMATConditions(int target, uint processID, ref MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return PS3TMAPI.SNRESULT.SN_E_BAD_PARAM;
            uint rangeCount = (uint)matRanges.Length;
            IntPtr num1 = Marshal.AllocHGlobal(8 * (int)rangeCount);
            IntPtr unmanagedBuf1 = num1;
            foreach (PS3TMAPI.MATRange matRange in matRanges)
            {
                IntPtr unmanagedBuf2 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.StartAddress, unmanagedBuf1);
                unmanagedBuf1 = PS3TMAPI.WriteDataToUnmanagedIncPtr<uint>(matRange.Size, unmanagedBuf2);
            }
            uint bufSize = 0U;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATConditionsX86(target, processID, ref rangeCount, num1, ref bufSize, IntPtr.Zero) : PS3TMAPI.GetMATConditionsX64(target, processID, ref rangeCount, num1, ref bufSize, IntPtr.Zero);
            if (PS3TMAPI.FAILED(res1))
            {
                Marshal.FreeHGlobal(num1);
                return res1;
            }
            else
            {
                IntPtr num2 = Marshal.AllocHGlobal((int)bufSize);
                PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetMATConditionsX86(target, processID, ref rangeCount, num1, ref bufSize, num2) : PS3TMAPI.GetMATConditionsX64(target, processID, ref rangeCount, num1, ref bufSize, num2);
                if (PS3TMAPI.FAILED(res2))
                {
                    Marshal.FreeHGlobal(num1);
                    Marshal.FreeHGlobal(num2);
                    return res2;
                }
                else
                {
                    IntPtr unmanagedBuf2 = num2;
                    for (int index1 = 0; (long)index1 < (long)rangeCount; ++index1)
                    {
                        unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf2, ref matRanges[index1].StartAddress), ref matRanges[index1].Size);
                        uint num3 = matRanges[index1].Size / 4096U;
                        matRanges[index1].PageConditions = new PS3TMAPI.MATCondition[num3];
                        for (int index2 = 0; (long)index2 < (long)num3; ++index2)
                        {
                            byte storage = (byte)0;
                            unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(unmanagedBuf2, ref storage);
                            matRanges[index1].PageConditions[index2] = (PS3TMAPI.MATCondition)storage;
                        }
                        bufSize -= 8U + num3;
                    }
                    if ((int)bufSize != 0)
                        res2 = PS3TMAPI.SNRESULT.SN_E_ERROR;
                    Marshal.FreeHGlobal(num1);
                    Marshal.FreeHGlobal(num2);
                    return res2;
                }
            }
        }

        public static SNRESULT SetMATConditions(int target, uint processID, MATRange[] matRanges)
        {
            if (matRanges == null || matRanges.Length < 1)
                return SNRESULT.SN_E_BAD_PARAM;
            int length = matRanges.Length;
            int num1 = 0;
            foreach (MATRange matRange in matRanges)
            {
                if (matRange.PageConditions == null)
                    return SNRESULT.SN_E_BAD_PARAM;
                num1 += matRange.PageConditions.Length;
            }
            IntPtr num2 = Marshal.AllocHGlobal(num1 + 2 * length * 4);
            IntPtr unmanagedBuf1 = num2;
            foreach (MATRange matRange in matRanges)
            {
                IntPtr unmanagedBuf2 = WriteDataToUnmanagedIncPtr<uint>(matRange.StartAddress, unmanagedBuf1);
                unmanagedBuf1 = WriteDataToUnmanagedIncPtr<uint>(matRange.Size, unmanagedBuf2);
                foreach (byte storage in matRange.PageConditions)
                    WriteDataToUnmanagedIncPtr<byte>(storage, unmanagedBuf1);
            }
            uint bufSize = 1U;
            SNRESULT snresult = Is32Bit() ? SetMATConditionsX86(target, processID, (uint)length, bufSize, num2) : SetMATConditionsX64(target, processID, (uint)length, bufSize, num2);
            Marshal.FreeHGlobal(num2);
            return snresult;
        }

        public static SNRESULT SaveSettings()
        {
            if (!Is32Bit())
                return SaveSettingsX64();
            else
                return SaveSettingsX86();
        }

        public static SNRESULT Exit()
        {
            if (!Is32Bit())
                return ExitX64();
            else
                return ExitX86();
        }

        public static SNRESULT ExitEx(uint millisecondTimeout)
        {
            if (!Is32Bit())
                return ExitExX64(millisecondTimeout);
            else
                return ExitExX86(millisecondTimeout);
        }

        public static SNRESULT RegisterPadPlaybackHandler(int target, PadPlaybackCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterPadPlaybackNotificationHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterPadPlaybackNotificationHandlerX64(target, PS3TMAPI.ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
                ms_userPadPlaybackCallbacks[target] = new PadPlaybackCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            return res;
        }

        public static SNRESULT UnregisterPadPlaybackHandler(int target)
        {
            SNRESULT res = Is32Bit() ? UnregisterPadPlaybackHandlerX86(target) : UnregisterPadPlaybackHandlerX64(target);
            if (SUCCEEDED(res))
                ms_userPadPlaybackCallbacks.Remove(target);
            return res;
        }

        public static SNRESULT StartPadPlayback(int target)
        {
            if (!Is32Bit())
                return StartPadPlaybackX64(target);
            else
                return StartPadPlaybackX86(target);
        }

        public static SNRESULT StopPadPlayback(int target)
        {
            if (!Is32Bit())
                return StopPadPlaybackX64(target);
            else
                return StopPadPlaybackX86(target);
        }

        public static SNRESULT SendPadPlaybackData(int target, PadData padData)
        {
            if (padData.buttons == null || padData.buttons.Length != 24)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SendPadPlaybackDataX64(target, ref padData);
            else
                return SendPadPlaybackDataX86(target, ref padData);
        }

        public static SNRESULT RegisterPadCaptureHandler(int target, PadCaptureCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterPadCaptureHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterPadCaptureHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
                ms_userPadCaptureCallbacks[target] = new PadCaptureCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            return res;
        }

        public static SNRESULT UnregisterPadCaptureHandler(int target)
        {
            SNRESULT res = Is32Bit() ? UnregisterPadCaptureHandlerX86(target) : UnregisterPadCaptureHandlerX64(target);
            if (SUCCEEDED(res))
                ms_userPadCaptureCallbacks.Remove(target);
            return res;
        }

        public static SNRESULT StartPadCapture(int target)
        {
            if (!Is32Bit())
                return StartPadCaptureX64(target);
            else
                return StartPadCaptureX86(target);
        }

        public static SNRESULT StopPadCapture(int target)
        {
            if (!Is32Bit())
                return StopPadCaptureX64(target);
            else
                return StopPadCaptureX86(target);
        }

        private static void MarshalPadCaptureEvent(int target, uint param, SNRESULT res, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            PadData[] padData = new PadData[1];
            padData[0].buttons = new short[24];
            ReadDataFromUnmanagedIncPtr<PadData>(data, ref padData[0]);
            ms_userPadCaptureCallbacks[target].m_callback(target, res, padData, ms_userPadCaptureCallbacks[target].m_userData);
        }

        private static void MarshalPadPlaybackEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            if ((int)length != 1)
                return;
            uint storage = 0U;
            ReadDataFromUnmanagedIncPtr<uint>(data, ref storage);
            ms_userPadPlaybackCallbacks[target].m_callback(target, result, (PadPlaybackResponse)storage, ms_userPadPlaybackCallbacks[target].m_userData);
        }

        public static SNRESULT GetVRAMCaptureFlags(int target, out VRAMCaptureFlag vramFlags)
        {
            ulong vramFlags1;
            SNRESULT snresult = Is32Bit() ? GetVRAMCaptureFlagsX86(target, out vramFlags1) : GetVRAMCaptureFlagsX64(target, out vramFlags1);
            vramFlags = (VRAMCaptureFlag)vramFlags1;
            return snresult;
        }

        public static SNRESULT SetVRAMCaptureFlags(int target, VRAMCaptureFlag vramFlags)
        {
            if (!Is32Bit())
                return SetVRAMCaptureFlagsX64(target, (ulong)vramFlags);
            else
                return SetVRAMCaptureFlagsX86(target, (ulong)vramFlags);
        }

        public static SNRESULT EnableVRAMCapture(int target)
        {
            if (!Is32Bit())
                return EnableVRAMCaptureX864(target);
            else
                return EnableVRAMCaptureX86(target);
        }

        public static SNRESULT GetVRAMInformation(int target, uint processID, out VRAMInfo primaryVRAMInfo, out VRAMInfo secondaryVRAMInfo)
        {
            primaryVRAMInfo = (VRAMInfo)null;
            secondaryVRAMInfo = (VRAMInfo)null;
            VramInfoPriv primaryVRAMInfo1 = new VramInfoPriv();
            VramInfoPriv secondaryVRAMInfo1 = new VramInfoPriv();
            SNRESULT res = Is32Bit() ? GetVRAMInformationX86(target, processID, out primaryVRAMInfo1, out secondaryVRAMInfo1) : GetVRAMInformationX64(target, processID, out primaryVRAMInfo1, out secondaryVRAMInfo1);
            if (FAILED(res))
                return res;
            primaryVRAMInfo = new VRAMInfo();
            primaryVRAMInfo.BPAddress = primaryVRAMInfo1.BpAddress;
            primaryVRAMInfo.TopAddressPointer = primaryVRAMInfo1.TopAddressPointer;
            primaryVRAMInfo.Width = primaryVRAMInfo1.Width;
            primaryVRAMInfo.Height = primaryVRAMInfo1.Height;
            primaryVRAMInfo.Pitch = primaryVRAMInfo1.Pitch;
            primaryVRAMInfo.Colour = primaryVRAMInfo1.Colour;
            secondaryVRAMInfo = new VRAMInfo();
            secondaryVRAMInfo.BPAddress = secondaryVRAMInfo1.BpAddress;
            secondaryVRAMInfo.TopAddressPointer = secondaryVRAMInfo1.TopAddressPointer;
            secondaryVRAMInfo.Width = secondaryVRAMInfo1.Width;
            secondaryVRAMInfo.Height = secondaryVRAMInfo1.Height;
            secondaryVRAMInfo.Pitch = secondaryVRAMInfo1.Pitch;
            secondaryVRAMInfo.Colour = secondaryVRAMInfo1.Colour;
            return res;
        }

        public static SNRESULT VRAMCapture(int target, uint processID, VRAMInfo vramInfo, string fileName)
        {
            IntPtr num = IntPtr.Zero;
            if (vramInfo != null)
            {
                VramInfoPriv vramInfoPriv = new VramInfoPriv();
                vramInfoPriv.BpAddress = vramInfo.BPAddress;
                vramInfoPriv.TopAddressPointer = vramInfo.TopAddressPointer;
                vramInfoPriv.Width = vramInfo.Width;
                vramInfoPriv.Height = vramInfo.Height;
                vramInfoPriv.Pitch = vramInfo.Pitch;
                vramInfoPriv.Colour = vramInfo.Colour;
                num = Marshal.AllocHGlobal(Marshal.SizeOf((object)vramInfoPriv));
                Marshal.StructureToPtr((object)vramInfoPriv, num, false);
            }
            SNRESULT snresult = Is32Bit() ? VRAMCaptureX86(target, processID, num, fileName) : VRAMCaptureX64(target, processID, num, fileName);
            if (vramInfo != null)
                Marshal.FreeHGlobal(num);
            return snresult;
        }

        private static void CustomProtocolHandler(int target, PS3Protocol ps3Protocol, IntPtr unmanagedBuf, uint length, IntPtr userData)
        {
            PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
            CustomProtocolId key = new CustomProtocolId(target, protocol);
            CusProtoCallbackAndUserData callbackAndUserData;
            if (!ms_userCustomProtoCallbacks.TryGetValue(key, out callbackAndUserData))
                return;
            byte[] numArray = new byte[length];
            Marshal.Copy(unmanagedBuf, numArray, 0, numArray.Length);
            callbackAndUserData.m_callback(target, ps3Protocol, numArray, callbackAndUserData.m_userData);
        }

        public static SNRESULT RegisterCustomProtocol(int target, uint protocol, uint port, string lparDesc, uint priority, out PS3TMAPI.PS3Protocol ps3Protocol, PS3TMAPI.CustomProtocolCallback callback, ref object userData)
        {
            ps3Protocol = new PS3Protocol();
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterCustomProtocolExX86(target, protocol, port, lparDesc, priority, out ps3Protocol, PS3TMAPI.ms_customProtoCallbackPriv, IntPtr.Zero) : PS3TMAPI.RegisterCustomProtocolExX64(target, protocol, port, lparDesc, priority, out ps3Protocol, PS3TMAPI.ms_customProtoCallbackPriv, IntPtr.Zero);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol1 = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId index = new CustomProtocolId(target, protocol1);
                ms_userCustomProtoCallbacks[index] = new CusProtoCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            }
            return res;
        }

        public static SNRESULT UnregisterCustomProtocol(int target, PS3Protocol ps3Protocol)
        {
            SNRESULT res = Is32Bit() ? UnregisterCustomProtocolX86(target, ref ps3Protocol) : UnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId key = new CustomProtocolId(target, protocol);
                ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        public static SNRESULT ForceUnregisterCustomProtocol(int target, PS3Protocol ps3Protocol)
        {
            SNRESULT res = Is32Bit() ? ForceUnregisterCustomProtocolX86(target, ref ps3Protocol) : ForceUnregisterCustomProtocolX64(target, ref ps3Protocol);
            if (SUCCEEDED(res))
            {
                PS3ProtocolPriv protocol = new PS3ProtocolPriv(ps3Protocol.Protocol, ps3Protocol.Port);
                CustomProtocolId key = new CustomProtocolId(target, protocol);
                ms_userCustomProtoCallbacks.Remove(key);
            }
            return res;
        }

        public static SNRESULT SendCustomProtocolData(int target, PS3Protocol ps3Protocol, byte[] data)
        {
            if (data == null || data.Length < 1)
                return SNRESULT.SN_E_BAD_PARAM;
            if (!Is32Bit())
                return SendCustomProtocolDataX64(target, ref ps3Protocol, data, data.Length);
            else
                return SendCustomProtocolDataX86(target, ref ps3Protocol, data, data.Length);
        }

        public static SNRESULT SetFileServingEventFlags(int target, FileServingEventFlag eventFlags)
        {
            if (!Is32Bit())
                return SetFileServingEventFlagsX64(target, (ulong)eventFlags);
            else
                return SetFileServingEventFlagsX86(target, (ulong)eventFlags);
        }

        public static SNRESULT GetFileServingEventFlags(int target, out FileServingEventFlag eventFlags)
        {
            ulong eventFlags1 = 0UL;
            SNRESULT snresult = Is32Bit() ? GetFileServingEventFlagsX86(target, ref eventFlags1) : GetFileServingEventFlagsX64(target, ref eventFlags1);
            eventFlags = (FileServingEventFlag)eventFlags1;
            return snresult;
        }

        public static SNRESULT SetCaseSensitiveFileServing(int target, bool bOn, out bool bOldSetting)
        {
            if (!Is32Bit())
                return SetCaseSensitiveFileServingX64(target, bOn, out bOldSetting);
            else
                return SetCaseSensitiveFileServingX86(target, bOn, out bOldSetting);
        }

        public static SNRESULT RegisterFTPEventHandler(int target, FTPEventCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterFTPEventHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterFTPEventHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
                ms_userFtpCallbacks[target] = new FtpCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            return res;
        }

        public static SNRESULT CancelFTPEvents(int target)
        {
            SNRESULT res = Is32Bit() ? CancelFTPEventsX86(target) : CancelFTPEventsX64(target);
            if (SUCCEEDED(res))
                ms_userFtpCallbacks.Remove(target);
            return res;
        }

        private static void MarshalFTPEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            FTPNotification[] ftpNotifications = new FTPNotification[0];
            if (length > 0U)
            {
                uint num = (uint)((ulong)length / (ulong)Marshal.SizeOf(typeof(FTPNotification)));
                ftpNotifications = new FTPNotification[num];
                for (int index = 0; (long)index < (long)num; ++index)
                    data = ReadDataFromUnmanagedIncPtr<FTPNotification>(data, ref ftpNotifications[index]);
            }
            ms_userFtpCallbacks[target].m_callback(target, result, ftpNotifications, ms_userFtpCallbacks[target].m_userData);
        }

        public static SNRESULT RegisterFileTraceHandler(int target, FileTraceCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterFileTraceHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterFileTraceHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
                ms_userFileTraceCallbacks[target] = new FileTraceCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            return res;
        }

        public static SNRESULT UnregisterFileTraceHandler(int target)
        {
            PS3TMAPI.SNRESULT res = PS3TMAPI.Is32Bit() ? PS3TMAPI.UnregisterFileTraceHandlerX86(target) : PS3TMAPI.UnregisterFileTraceHandlerX64(target);
            if (PS3TMAPI.SUCCEEDED(res))
                PS3TMAPI.ms_userFileTraceCallbacks.Remove(target);
            return res;
        }

        private static void MarshalFileTraceEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            PS3TMAPI.FileTraceEvent fileTraceEvent = new PS3TMAPI.FileTraceEvent();
            IntPtr unmanagedBuf1 = data;
            uint num1 = 44U;
            if (length < num1)
                return;
            IntPtr unmanagedBuf2 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf1, ref fileTraceEvent.SerialID);
            int storage1 = 0;
            IntPtr unmanagedBuf3 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<int>(unmanagedBuf2, ref storage1);
            fileTraceEvent.TraceType = (PS3TMAPI.FileTraceType)storage1;
            int storage2 = 0;
            IntPtr unmanagedBuf4 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<int>(unmanagedBuf3, ref storage2);
            fileTraceEvent.Status = (PS3TMAPI.FileTraceNotificationStatus)storage2;
            IntPtr unmanagedBuf5 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf4, ref fileTraceEvent.ProcessID), ref fileTraceEvent.ThreadID), ref fileTraceEvent.TimeBaseStartOfTrace), ref fileTraceEvent.TimeBase);
            uint storage3 = 0U;
            IntPtr unmanagedBuf6 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf5, ref storage3);
            uint num2 = num1 + storage3;
            if (length < num2)
                return;
            fileTraceEvent.BackTraceData = new byte[storage3];
            for (int index = 0; (long)index < (long)storage3; ++index)
                unmanagedBuf6 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(unmanagedBuf6, ref fileTraceEvent.BackTraceData[index]);
            IntPtr num3;
            switch (fileTraceEvent.TraceType)
            {
                case PS3TMAPI.FileTraceType.GetBlockSize:
                case PS3TMAPI.FileTraceType.Stat:
                case PS3TMAPI.FileTraceType.WidgetStat:
                case PS3TMAPI.FileTraceType.Unlink:
                case PS3TMAPI.FileTraceType.WidgetUnlink:
                case PS3TMAPI.FileTraceType.RMDir:
                case PS3TMAPI.FileTraceType.WidgetRMDir:
                    fileTraceEvent.LogData.LogType1 = new PS3TMAPI.FileTraceLogType1();
                    uint storage4 = 0U;
                    IntPtr unmanagedBuf7 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref storage4);
                    if (storage4 > 0U)
                    {
                        PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf7, ref fileTraceEvent.LogData.LogType1.Path);
                        break;
                    }
                    else
                        break;
                case PS3TMAPI.FileTraceType.Rename:
                case PS3TMAPI.FileTraceType.WidgetRename:
                    fileTraceEvent.LogData.LogType2 = new PS3TMAPI.FileTraceLogType2();
                    uint storage5 = 0U;
                    IntPtr unmanagedBuf8 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref storage5);
                    uint storage6 = 0U;
                    IntPtr unmanagedBuf9 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf8, ref storage6);
                    if (storage5 > 0U)
                        unmanagedBuf9 = PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf9, ref fileTraceEvent.LogData.LogType2.Path1);
                    if (storage6 > 0U)
                    {
                        PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf9, ref fileTraceEvent.LogData.LogType2.Path2);
                        break;
                    }
                    else
                        break;
                case PS3TMAPI.FileTraceType.Truncate:
                case PS3TMAPI.FileTraceType.TruncateNoAlloc:
                case PS3TMAPI.FileTraceType.Truncate2:
                case PS3TMAPI.FileTraceType.Truncate2NoInit:
                    fileTraceEvent.LogData.LogType3 = new PS3TMAPI.FileTraceLogType3();
                    IntPtr unmanagedBuf10 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType3.Arg);
                    uint storage7 = 0U;
                    IntPtr unmanagedBuf11 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf10, ref storage7);
                    if (storage7 > 0U)
                    {
                        PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf11, ref fileTraceEvent.LogData.LogType3.Path);
                        break;
                    }
                    else
                        break;
                case FileTraceType.OpenDir:
                case FileTraceType.WidgetOpenDir:
                case FileTraceType.CHMod:
                case FileTraceType.MkDir:
                    fileTraceEvent.LogData.LogType4 = new FileTraceLogType4();
                    IntPtr unmanagedBuf12 = ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType4.Mode);
                    uint storage8 = 0U;
                    IntPtr unmanagedBuf13 = ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf12, ref storage8);
                    if (storage8 > 0U)
                    {
                        ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf13, ref fileTraceEvent.LogData.LogType4.Path);
                        break;
                    }
                    else
                        break;
                case PS3TMAPI.FileTraceType.UTime:
                    fileTraceEvent.LogData.LogType6 = new PS3TMAPI.FileTraceLogType6();
                    IntPtr unmanagedBuf14 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType6.Arg1), ref fileTraceEvent.LogData.LogType6.Arg2);
                    uint storage9 = 0U;
                    IntPtr unmanagedBuf15 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf14, ref storage9);
                    if (storage9 > 0U)
                    {
                        PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf15, ref fileTraceEvent.LogData.LogType6.Path);
                        break;
                    }
                    else
                        break;
                case FileTraceType.Open:
                case FileTraceType.WidgetOpen:
                    fileTraceEvent.LogData.LogType8 = new PS3TMAPI.FileTraceLogType8();
                    IntPtr unmanagedBuf16 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType8.ProcessInfo), ref fileTraceEvent.LogData.LogType8.Arg1), ref fileTraceEvent.LogData.LogType8.Arg2), ref fileTraceEvent.LogData.LogType8.Arg3), ref fileTraceEvent.LogData.LogType8.Arg4);
                    uint storage10 = 0U;
                    IntPtr unmanagedBuf17 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf16, ref storage10);
                    uint storage11 = 0U;
                    IntPtr unmanagedBuf18 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(unmanagedBuf17, ref storage11);
                    fileTraceEvent.LogData.LogType8.VArg = new byte[storage10];
                    for (int index = 0; (long)index < (long)storage10; ++index)
                        unmanagedBuf18 = PS3TMAPI.ReadDataFromUnmanagedIncPtr<byte>(unmanagedBuf18, ref fileTraceEvent.LogData.LogType8.VArg[index]);
                    if (storage11 > 0U)
                    {
                        PS3TMAPI.ReadAnsiStringFromUnmanagedIncPtr(unmanagedBuf18, ref fileTraceEvent.LogData.LogType8.Path);
                        break;
                    }
                    else
                        break;
                case FileTraceType.Close:
                case FileTraceType.CloseDir:
                case FileTraceType.FSync:
                case FileTraceType.ReadDir:
                case FileTraceType.FStat:
                case FileTraceType.FGetBlockSize:
                    fileTraceEvent.LogData.LogType9 = new FileTraceLogType9();
                    num3 = ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType9.ProcessInfo);
                    break;
                case FileTraceType.Read:
                case FileTraceType.Write:
                case FileTraceType.GetDirEntries:
                    fileTraceEvent.LogData.LogType10 = new FileTraceLogType10();
                    num3 = ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType10.ProcessInfo), ref fileTraceEvent.LogData.LogType10.Size), ref fileTraceEvent.LogData.LogType10.Address), ref fileTraceEvent.LogData.LogType10.TxSize);
                    break;
                case FileTraceType.ReadOffset:
                case FileTraceType.WriteOffset:
                    fileTraceEvent.LogData.LogType11 = new PS3TMAPI.FileTraceLogType11();
                    ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType11.ProcessInfo), ref fileTraceEvent.LogData.LogType11.Size), ref fileTraceEvent.LogData.LogType11.Address), ref fileTraceEvent.LogData.LogType11.Offset), ref fileTraceEvent.LogData.LogType11.TxSize);
                    break;
                case FileTraceType.FTruncate:
                case PS3TMAPI.FileTraceType.FTruncateNoAlloc:
                    fileTraceEvent.LogData.LogType12 = new PS3TMAPI.FileTraceLogType12();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType12.ProcessInfo), ref fileTraceEvent.LogData.LogType12.TargetSize);
                    break;
                case PS3TMAPI.FileTraceType.LSeek:
                    fileTraceEvent.LogData.LogType13 = new PS3TMAPI.FileTraceLogType13();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType13.ProcessInfo), ref fileTraceEvent.LogData.LogType13.Size), ref fileTraceEvent.LogData.LogType13.Offset), ref fileTraceEvent.LogData.LogType13.CurPos);
                    break;
                case PS3TMAPI.FileTraceType.SetIOBuffer:
                    fileTraceEvent.LogData.LogType14 = new PS3TMAPI.FileTraceLogType14();
                    PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTraceProcessInfo>(unmanagedBuf6, ref fileTraceEvent.LogData.LogType14.ProcessInfo), ref fileTraceEvent.LogData.LogType14.MaxSize), ref fileTraceEvent.LogData.LogType14.Page), ref fileTraceEvent.LogData.LogType14.ContainerID);
                    break;
            }
            PS3TMAPI.ms_userFileTraceCallbacks[target].m_callback(target, result, fileTraceEvent, PS3TMAPI.ms_userFileTraceCallbacks[target].m_userData);
        }

        public static SNRESULT StartFileTrace(int target, uint processID, uint size, string filename)
        {
            if (!Is32Bit())
                return StartFileTraceX64(target, processID, size, filename);
            else
                return StartFileTraceX86(target, processID, size, filename);
        }

        public static SNRESULT StopFileTrace(int target, uint processID)
        {
            if (!Is32Bit())
                return StopFileTraceX64(target, processID);
            else
                return StopFileTraceX86(target, processID);
        }

        public static SNRESULT InstallPackage(int target, string packagePath)
        { 
            return Is32Bit() ? InstallPackageX86(target, packagePath) : InstallPackageX64(target, packagePath);
        }

        public static SNRESULT UploadFile(int target, string source, string dest, out uint txID)
        {
            if (!Is32Bit())
                return UploadFileX64(target, source, dest, out txID);
            else
                return UploadFileX86(target, source, dest, out txID);
        }

        public static SNRESULT GetFileTransferList(int target, out FileTransferInfo[] fileTransfers)
        {
            fileTransfers = (FileTransferInfo[])null;
            IntPtr fileTransferInfo = IntPtr.Zero;
            uint count = 0U;
            SNRESULT res1 = Is32Bit() ? GetFileTransferListX86(target, ref count, fileTransferInfo) : GetFileTransferListX64(target, ref count, fileTransferInfo);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)((long)Marshal.SizeOf(typeof(FileTransferInfoPriv)) * (long)count));
            SNRESULT res2 = Is32Bit() ? GetFileTransferListX86(target, ref count, num) : GetFileTransferListX64(target, ref count, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                fileTransfers = new FileTransferInfo[count];
                for (uint index = 0U; index < count; ++index)
                {
                    FileTransferInfoPriv storage = new FileTransferInfoPriv();
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTransferInfoPriv>(unmanagedBuf, ref storage);
                    fileTransfers[index] = new FileTransferInfo();
                    fileTransfers[index].TransferID = storage.TransferId;
                    fileTransfers[index].Status = (FileTransferStatus)storage.Status;
                    fileTransfers[index].SourcePath = storage.SourcePath;
                    fileTransfers[index].DestinationPath = storage.DestinationPath;
                    fileTransfers[index].Size = storage.Size;
                    fileTransfers[index].BytesRead = storage.BytesRead;
                }
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetFileTransferInfo(int target, uint txID, out FileTransferInfo fileTransferInfo)
        {
            fileTransferInfo = new FileTransferInfo();
            FileTransferInfoPriv fileTransferInfo1 = new FileTransferInfoPriv();
            SNRESULT res = Is32Bit() ? GetFileTransferInfoX86(target, txID, out fileTransferInfo1) : GetFileTransferInfoX64(target, txID, out fileTransferInfo1);
            if (FAILED(res))
                return res;
            fileTransferInfo.TransferID = fileTransferInfo1.TransferId;
            fileTransferInfo.Status = (FileTransferStatus)fileTransferInfo1.Status;
            fileTransferInfo.SourcePath = fileTransferInfo1.SourcePath;
            fileTransferInfo.DestinationPath = fileTransferInfo1.DestinationPath;
            fileTransferInfo.Size = fileTransferInfo1.Size;
            fileTransferInfo.BytesRead = fileTransferInfo1.BytesRead;
            return res;
        }

        public static SNRESULT CancelFileTransfer(int target, uint txID)
        {
            if (!Is32Bit())
                return CancelFileTransferX64(target, txID);
            else
                return CancelFileTransferX86(target, txID);
        }

        public static SNRESULT RetryFileTransfer(int target, uint txID, bool bForce)
        {
            if (!Is32Bit())
                return RetryFileTransferX64(target, txID, bForce);
            else
                return RetryFileTransferX86(target, txID, bForce);
        }

        public static SNRESULT RemoveTransferItemsByStatus(int target, uint filter)
        {
            if (!Is32Bit())
                return SNPS3RemoveTransferItemsByStatusX64(target, filter);
            else
                return RemoveTransferItemsByStatusX86(target, filter);
        }

        public static SNRESULT GetDirectoryList(int target, string directory, out DirEntry[] dirEntries)
        {
            IntPtr dirEntryList = IntPtr.Zero;
            dirEntries = (DirEntry[])null;
            uint numDirEntries = 0U;
            SNRESULT res1 = Is32Bit() ? GetDirectoryListX86(target, directory, ref numDirEntries, dirEntryList) : GetDirectoryListX64(target, directory, ref numDirEntries, dirEntryList);
            if (FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(DirEntry)));
            SNRESULT res2 = Is32Bit() ? GetDirectoryListX86(target, directory, ref numDirEntries, num) : GetDirectoryListX64(target, directory, ref numDirEntries, num);
            if (FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                dirEntries = new DirEntry[numDirEntries];
                for (int index = 0; (long)index < (long)numDirEntries; ++index)
                    unmanagedBuf = ReadDataFromUnmanagedIncPtr<PS3TMAPI.DirEntry>(unmanagedBuf, ref dirEntries[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT GetDirectoryListEx(int target, string directory, out DirEntryEx[] dirEntries, out TargetTimezone timeZone)
        {
            IntPtr dirEntryListEx = IntPtr.Zero;
            dirEntries = (PS3TMAPI.DirEntryEx[])null;
            timeZone = new PS3TMAPI.TargetTimezone();
            uint numDirEntries = 0U;
            PS3TMAPI.SNRESULT res1 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListExX86(target, directory, ref numDirEntries, dirEntryListEx, ref timeZone) : PS3TMAPI.GetDirectoryListExX64(target, directory, ref numDirEntries, dirEntryListEx, ref timeZone);
            if (PS3TMAPI.FAILED(res1))
                return res1;
            IntPtr num = Marshal.AllocHGlobal((int)numDirEntries * Marshal.SizeOf(typeof(PS3TMAPI.DirEntryEx)));
            PS3TMAPI.SNRESULT res2 = PS3TMAPI.Is32Bit() ? PS3TMAPI.GetDirectoryListExX86(target, directory, ref numDirEntries, num, ref timeZone) : PS3TMAPI.GetDirectoryListExX64(target, directory, ref numDirEntries, num, ref timeZone);
            if (PS3TMAPI.FAILED(res2))
            {
                Marshal.FreeHGlobal(num);
                return res2;
            }
            else
            {
                IntPtr unmanagedBuf = num;
                dirEntries = new PS3TMAPI.DirEntryEx[numDirEntries];
                for (int index = 0; (long)index < (long)numDirEntries; ++index)
                    unmanagedBuf = PS3TMAPI.ReadDataFromUnmanagedIncPtr<PS3TMAPI.DirEntryEx>(unmanagedBuf, ref dirEntries[index]);
                Marshal.FreeHGlobal(num);
                return res2;
            }
        }

        public static SNRESULT MakeDirectory(int target, string directory, uint mode)
        {
            if (!Is32Bit())
                return MakeDirectoryX64(target, directory, mode);
            else
                return MakeDirectoryX86(target, directory, mode);
        }

        public static SNRESULT DeleteFile(int target, string path)
        {
            if (!Is32Bit())
                return DeleteFileX64(target, path);
            else
                return DeleteFileX86(target, path);
        }

        public static SNRESULT DeleteFileEx(int target, string path, uint msTimeout)
        {
            if (!Is32Bit())
                return DeleteFileExX64(target, path, msTimeout);
            else
                return DeleteFileExX86(target, path, msTimeout);
        }

        public static SNRESULT RenameFile(int target, string source, string dest)
        {
            if (!Is32Bit())
                return RenameFileX64(target, source, dest);
            else
                return RenameFileX86(target, source, dest);
        }

        public static SNRESULT DownloadFile(int target, string source, string dest, out uint txID)
        {
            if (!Is32Bit())
                return DownloadFileX64(target, source, dest, out txID);
            else
                return DownloadFileX86(target, source, dest, out txID);
        }

        public static SNRESULT DownloadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            if (!Is32Bit())
                return DownloadDirectoryX64(target, source, dest, out lastTxID);
            else
                return DownloadDirectoryX86(target, source, dest, out lastTxID);
        }

        public static SNRESULT UploadDirectory(int target, string source, string dest, out uint lastTxID)
        {
            if (!Is32Bit())
                return UploadDirectoryX64(target, source, dest, out lastTxID);
            else
                return UploadDirectoryX86(target, source, dest, out lastTxID);
        }

        public static SNRESULT StatTargetFile(int target, string file, out DirEntry dirEntry)
        {
            if (!Is32Bit())
                return StatTargetFileX64(target, file, out dirEntry);
            else
                return StatTargetFileX86(target, file, out dirEntry);
        }

        public static SNRESULT StatTargetFileEx(int target, string file, out DirEntryEx dirEntryEx, out TargetTimezone timeZone)
        {
            if (!Is32Bit())
                return StatTargetFileExX64(target, file, out dirEntryEx, out timeZone);
            else
                return StatTargetFileExX86(target, file, out dirEntryEx, out timeZone);
        }

        public static SNRESULT ChMod(int target, string filePath, ChModFilePermission mode)
        {
            if (!Is32Bit())
                return CHModX64(target, filePath, (uint)mode);
            else
                return CHModX86(target, filePath, (uint)mode);
        }

        public static SNRESULT InstallGameEx(int target, string paramSFOPath, out string titleID, out string targetPath, out uint txID)
        {
            IntPtr titleId;
            IntPtr targetPath1;
            SNRESULT snresult = Is32Bit() ? InstallGameExX86(target, paramSFOPath, out titleId, out targetPath1, out txID) : InstallGameExX64(target, paramSFOPath, out titleId, out targetPath1, out txID);
            titleID = Marshal.PtrToStringAnsi(titleId);
            targetPath = Marshal.PtrToStringAnsi(targetPath1);
            return snresult;
        }

        public static SNRESULT SetFileTime(int target, string filePath, ulong accessTime, ulong modifiedTime)
        {
            if (!Is32Bit())
                return SetFileTimeX64(target, filePath, accessTime, modifiedTime);
            else
                return SetFileTimeX86(target, filePath, accessTime, modifiedTime);
        }

        public static SNRESULT FormatHDD(int target, uint initRegistry)
        {
            if (!Is32Bit())
                return FormatHDDX64(target, initRegistry);
            else
                return FormatHDDX86(target, initRegistry);
        }

        public static SNRESULT UninstallGame(int target, string gameDirectory)
        {
            if (!Is32Bit())
                return UninstallGameX64(target, gameDirectory);
            else
                return UninstallGameX86(target, gameDirectory);
        }

        public static SNRESULT WaitForFileTransfer(int target, uint txID, out FileTransferNotificationType notificationType, uint msTimeout)
        {
            return Is32Bit() ? WaitForFileTransferX86(target, txID, out notificationType, msTimeout) : WaitForFileTransferX64(target, txID, out notificationType, msTimeout);
        }

        public static SNRESULT EnableInternalKick(bool bEnable)
        {
            if (!Is32Bit())
                return EnableInternalKickX64(bEnable);
            return EnableInternalKickX86(bEnable);
        }

        public static SNRESULT BDTransferImage(int target, string sourceFileName, string destinationDevice, out uint transactionId)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(sourceFileName));
            if (!Is32Bit())
                return BDTransferImageX64(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
            return BDTransferImageX86(target, scopedGlobalHeapPtr.Get(), destinationDevice, out transactionId);
        }

        public static SNRESULT BDInsert(int target, string deviceName)
        {
            if (!Is32Bit())
                return BDInsertX64(target, deviceName);
            return BDInsertX86(target, deviceName);
        }

        public static SNRESULT BDEject(int target, string deviceName)
        {
            if (!Is32Bit())
                return BDEjectX64(target, deviceName);
            return BDEjectX86(target, deviceName);
        }

        public static SNRESULT BDFormat(int target, string deviceName, uint formatMode)
        {
            if (!Is32Bit())
                return BDFormatX64(target, deviceName, formatMode);
            return BDFormatX86(target, deviceName, formatMode);
        }

        public static SNRESULT BDQuery(int target, string deviceName, ref BDInfo info)
        {
            BDInfoPriv infoPriv = new BDInfoPriv();
            SNRESULT res = Is32Bit() ? BDQueryX86(target, deviceName, ref infoPriv) : BDQueryX64(target, deviceName, ref infoPriv);
            if (SUCCEEDED(res))
            {
                info.bdemu_data_size = infoPriv.bdemu_data_size;
                info.bdemu_total_entry = infoPriv.bdemu_total_entry;
                info.bdemu_selected_index = infoPriv.bdemu_selected_index;
                info.image_index = infoPriv.image_index;
                info.image_type = infoPriv.image_type;
                info.image_file_name = Utf8FixedSizeByteArrayToString(infoPriv.image_file_name);
                info.image_file_size = infoPriv.image_file_size;
                info.image_product_code = Utf8FixedSizeByteArrayToString(infoPriv.image_product_code);
                info.image_producer = Utf8FixedSizeByteArrayToString(infoPriv.image_producer);
                info.image_author = Utf8FixedSizeByteArrayToString(infoPriv.image_author);
                info.image_date = Utf8FixedSizeByteArrayToString(infoPriv.image_date);
                info.image_sector_layer0 = infoPriv.image_sector_layer0;
                info.image_sector_layer1 = infoPriv.image_sector_layer1;
                info.image_memorandum = Utf8FixedSizeByteArrayToString(infoPriv.image_memorandum);
            }
            return res;
        }

        public static SNRESULT FSGetFreeSize(int target, string fsDir, out uint blockSize, out ulong freeBlockCount)
        {
            return Is32Bit() ? FSGetFreeSizeX86(target, fsDir, out blockSize, out freeBlockCount) : FSGetFreeSizeX64(target, fsDir, out blockSize, out freeBlockCount);
        }

        public static SNRESULT GetLogOptions(out LogCategory category)
        {
            return Is32Bit() ? GetLogOptionsX86(out category) : GetLogOptionsX64(out category);
        }

        public static SNRESULT SetLogOptions(LogCategory category)
        {
            return Is32Bit() ? SetLogOptionsX86(category) : SetLogOptionsX64(category);
        }

        public static SNRESULT ProcessOfflineFileTrace(int target, string path)
        {
            return Is32Bit() ? ProcessOfflineFileTraceX86(target, path) : ProcessOfflineFileTraceX64(target, path);
        }

        public static SNRESULT RegisterTargetEventHandler(int target, TargetEventCallback callback, ref object userData)
        {
            if (callback == null)
                return SNRESULT.SN_E_BAD_PARAM;
            SNRESULT res = Is32Bit() ? RegisterTargetEventHandlerX86(target, ms_eventHandlerWrapper, IntPtr.Zero) : RegisterTargetEventHandlerX64(target, ms_eventHandlerWrapper, IntPtr.Zero);
            if (SUCCEEDED(res))
                ms_userTargetCallbacks[target] = new TargetCallbackAndUserData()
                {
                    m_callback = callback,
                    m_userData = userData
                };
            return res;
        }

        public static SNRESULT SearchForTargets(string ipAddressFrom, string ipAddressTo, SearchTargetsCallback callback, object userData, int port)
        {
            SearchForTargetsCallbackHandler targetsCallbackHandler = new SearchForTargetsCallbackHandler(callback, userData);
            SearchTargetsCallbackPriv callback1 = new SearchTargetsCallbackPriv(SearchForTargetsCallbackHandler.SearchForTargetsCallback);
            IntPtr userData1 = GCHandle.ToIntPtr(GCHandle.Alloc((object)targetsCallbackHandler));
            if (!Is32Bit())
                return SearchForTargetsX64(ipAddressFrom, ipAddressTo, callback1, userData1, port);
            return SearchForTargetsX86(ipAddressFrom, ipAddressTo, callback1, userData1, port);
        }

        public static SNRESULT StopSearchForTargets()
        {
            if (!Is32Bit())
                return StopSearchForTargetsX64();
            return StopSearchForTargetsX86();
        }

        public static SNRESULT IsScanning()
        {
            if (!Is32Bit())
                return IsScanningX64();
            return IsScanningX86();
        }

        public static SNRESULT IsValidResolution(uint monitorType, uint startupResolution)
        {
            if (!Is32Bit())
                return IsValidResolutionX64(monitorType, startupResolution);
            return IsValidResolutionX86(monitorType, startupResolution);
        }

        public static SNRESULT SetDisplaySettings(int target, string executable, uint monitorType, uint connectorType, uint startupResolution, bool HDCP, bool resetAfter)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(executable));
            if (!Is32Bit())
                return SetDisplaySettingsX64(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
            return SetDisplaySettingsX86(target, scopedGlobalHeapPtr.Get(), monitorType, connectorType, startupResolution, HDCP, resetAfter);
        }

        public static SNRESULT MapFileSystem(char driveLetter)
        {
            if (!Is32Bit())
                return MapFileSystemX64(driveLetter);
            return MapFileSystemX86(driveLetter);
        }

        public static SNRESULT UnmapFileSystem()
        {
            if (!Is32Bit())
                return UnmapFileSystemX64();
            return UnmapFileSystemX86();
        }

        public static SNRESULT CancelTargetEvents(int target)
        {
            SNRESULT res = Is32Bit() ? CancelTargetEventsX86(target) : CancelTargetEventsX64(target);
            if (SUCCEEDED(res))
                ms_userTargetCallbacks.Remove(target);
            return res;
        }

        public static SNRESULT ImportTargetSettings(int target, string fileName)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(PS3TMAPI.AllocUtf8FromString(fileName));
            if (!Is32Bit())
                return ImportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return ImportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        public static SNRESULT ExportTargetSettings(int target, string fileName)
        {
            ScopedGlobalHeapPtr scopedGlobalHeapPtr = new ScopedGlobalHeapPtr(AllocUtf8FromString(fileName));
            if (!Is32Bit())
                return ExportTargetSettingsX64(target, scopedGlobalHeapPtr.Get());
            return ExportTargetSettingsX86(target, scopedGlobalHeapPtr.Get());
        }

        private static void MarshalTargetEvent(int target, uint param, SNRESULT result, uint length, IntPtr data)
        {
            List<TargetEvent> list = new List<TargetEvent>();
            uint num1 = length;
            while (num1 > 0U)
            {
                TargetEvent targetEvent = new TargetEvent();
                uint storage = 0U;
                IntPtr num2 = data;
                num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref storage);
                num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.TargetID);
                int num3 = Marshal.ReadInt32(num2, 0);
                num2 = new IntPtr(num2.ToInt64() + (long)Marshal.SizeOf((object)num3));
                targetEvent.Type = (PS3TMAPI.TargetEventType)num3;
                targetEvent.Type.GetType();
                switch (targetEvent.Type)
                {
                    case TargetEventType.UnitStatusChange:
                        targetEvent.EventData = new TargetEventData();
                        int num4 = Marshal.ReadInt32(num2, 0);
                        num2 = new IntPtr(num2.ToInt64() + (long)Marshal.SizeOf((object)num4));
                        targetEvent.EventData.UnitStatusChangeData.Unit = (UnitType)num4;
                        int num5 = Marshal.ReadInt32(num2, 0);
                        num2 = new IntPtr(num2.ToInt64() + (long)Marshal.SizeOf((object)num5));
                        targetEvent.EventData.UnitStatusChangeData.Status = (UnitStatus)num5;
                        break;
                    case TargetEventType.Details:
                        targetEvent.EventData = new TargetEventData();
                        num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.EventData.DetailsData.Flags);
                        break;
                    case TargetEventType.ModuleLoad:
                    case TargetEventType.ModuleRunning:
                    case TargetEventType.ModuleStopped:
                        targetEvent.EventData = new TargetEventData();
                        num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.EventData.ModuleEventData.Unit);
                        num2 = ReadDataFromUnmanagedIncPtr<uint>(num2, ref targetEvent.EventData.ModuleEventData.ModuleID);
                        break;
                    case TargetEventType.TargetSpecific:
                        targetEvent.TargetSpecific = MarshalTargetSpecificEvent(storage, num2);
                        break;
                }
                list.Add(targetEvent);
                num1 -= storage;
                data = new IntPtr(data.ToInt64() + (long)storage);
            }
            ms_userTargetCallbacks[target].m_callback(target, result, list.ToArray(), ms_userTargetCallbacks[target].m_userData);
        }

        private static IntPtr DirEntryExMarshalHelper(IntPtr dataPtr, ref DirEntryEx dirEntryEx)
        {
            DirEntryExPriv storage = new DirEntryExPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr<PS3TMAPI.DirEntryExPriv>(dataPtr, ref storage);
            dirEntryEx.Type = (DirEntryType)storage.Type;
            dirEntryEx.Mode = storage.Mode;
            dirEntryEx.AccessTimeUTC = storage.AccessTimeUTC;
            dirEntryEx.ModifiedTimeUTC = storage.ModifiedTimeUTC;
            dirEntryEx.CreateTimeUTC = storage.CreateTimeUTC;
            dirEntryEx.Size = storage.Size;
            dirEntryEx.Name = Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        private static IntPtr DirEntryMarshalHelper(IntPtr dataPtr, ref DirEntry dirEntry)
        {
            DirEntryPriv storage = new DirEntryPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr<DirEntryPriv>(dataPtr, ref storage);
            dirEntry.Type = (DirEntryType)storage.Type;
            dirEntry.Mode = storage.Mode;
            dirEntry.AccessTime = storage.AccessTime;
            dirEntry.ModifiedTime = storage.ModifiedTime;
            dirEntry.CreateTime = storage.CreateTime;
            dirEntry.Size = storage.Size;
            dirEntry.Name = Utf8FixedSizeByteArrayToString(storage.Name);
            return dataPtr;
        }

        private static IntPtr FileTransferInfoMarshalHelper(IntPtr dataPtr, ref FileTransferInfo fileTransferInfo)
        {
            FileTransferInfoPriv storage = new FileTransferInfoPriv();
            dataPtr = ReadDataFromUnmanagedIncPtr<PS3TMAPI.FileTransferInfoPriv>(dataPtr, ref storage);
            fileTransferInfo.TransferID = storage.TransferId;
            fileTransferInfo.Status = (FileTransferStatus)storage.Status;
            fileTransferInfo.Size = storage.Size;
            fileTransferInfo.BytesRead = storage.BytesRead;
           // fileTransferInfo.SourcePath = Utf8FixedSizeByteArrayToString(storage.SourcePath);
           // fileTransferInfo.DestinationPath = Utf8FixedSizeByteArrayToString(storage.DestinationPath);
            return dataPtr;
        }

        private static TargetSpecificEvent MarshalTargetSpecificEvent(uint eventSize, IntPtr data)
        {
            TargetSpecificEvent targetSpecificEvent = new TargetSpecificEvent();
            TargetSpecificData targetSpecificData = new TargetSpecificData();
            uint storage = 0U;
            data = ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.CommandID);
            data = ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.RequestID);
            data = ReadDataFromUnmanagedIncPtr<uint>(data, ref storage);
            data = ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.ProcessID);
            data = ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificEvent.Result);
            int num1 = Marshal.ReadInt32(data, 0);
            data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num1));
            targetSpecificData.Type = (TargetSpecificEventType)num1;
            int num2 = 20;
            switch (targetSpecificData.Type)
            {
                case TargetSpecificEventType.CoreDumpComplete:
                    targetSpecificData.CoreDumpComplete = new CoreDumpComplete();
                    targetSpecificData.CoreDumpComplete.Filename = Marshal.PtrToStringAnsi(data);
                    break;
                case TargetSpecificEventType.Footswitch:
                    targetSpecificData.Footswitch = new FootswitchData();
                    data = ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventSource);
                    data = ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData1);
                    data = ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData2);
                    data = ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.EventData3);
                    data = ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.Footswitch.Reserved);
                    break;
                case TargetSpecificEventType.InstallPackageProgress:
                    targetSpecificData.InstallPackageProgress = new InstallPackageProgress();
                    data = ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.InstallPackageProgress.Percent);
                    break;
                case PS3TMAPI.TargetSpecificEventType.InstallPackagePath:
                    targetSpecificData.InstallPackagePath = new InstallPackagePath();
                    targetSpecificData.InstallPackagePath.Path = Marshal.PtrToStringAnsi(data);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PRXLoad:
                    targetSpecificData.PRXLoad = new PS3TMAPI.NotifyPRXLoadData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXLoad.PPUThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PRXLoad.PRXID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXLoad.Timestamp);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PRXUnload:
                    targetSpecificData.PRXUnload = new PS3TMAPI.NotifyPRXUnloadData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXUnload.PPUThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PRXUnload.PRXID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PRXUnload.Timestamp);
                    break;
                case PS3TMAPI.TargetSpecificEventType.ProcessCreate:
                    targetSpecificData.PPUProcessCreate = new PS3TMAPI.PPUProcessCreateData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUProcessCreate.ParentProcessID);
                    if ((long)storage - (long)num2 - 4L > 0L)
                    {
                        targetSpecificData.PPUProcessCreate.Filename = Marshal.PtrToStringAnsi(data);
                        break;
                    }
                    else
                        break;
                case PS3TMAPI.TargetSpecificEventType.ProcessExit:
                    targetSpecificData.PPUProcessExit = new PS3TMAPI.PPUProcessExitData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUProcessExit.ExitCode);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcTrap:
                case PS3TMAPI.TargetSpecificEventType.PPUExcPrevInt:
                case PS3TMAPI.TargetSpecificEventType.PPUExcIllInst:
                case PS3TMAPI.TargetSpecificEventType.PPUExcTextHtabMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcTextSlbMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataHtabMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcFloat:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataSlbMiss:
                case PS3TMAPI.TargetSpecificEventType.PPUExcDabrMatch:
                case PS3TMAPI.TargetSpecificEventType.PPUExcStop:
                case PS3TMAPI.TargetSpecificEventType.PPUExcStopInit:
                    targetSpecificData.PPUException = new PS3TMAPI.PPUExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcAlignment:
                    targetSpecificData.PPUAlignmentException = new PS3TMAPI.PPUAlignmentExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUAlignmentException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.DSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.DAR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUAlignmentException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUExcDataMAT:
                    targetSpecificData.PPUDataMatException = new PS3TMAPI.PPUDataMatExceptionData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.PPUDataMatException.HWThreadNumber);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.DSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.DAR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.PC);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUDataMatException.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUThreadCreate:
                    targetSpecificData.PPUThreadCreate = new PS3TMAPI.PPUThreadCreateData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUThreadCreate.ThreadID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.PPUThreadExit:
                    targetSpecificData.PPUThreadExit = new PS3TMAPI.PPUThreadExitData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.PPUThreadExit.ThreadID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStart:
                    targetSpecificData.SPUThreadStart = new PS3TMAPI.SPUThreadStartData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStart.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStart.ThreadID);
                    if ((long)storage - (long)num2 - 8L > 0L)
                    {
                        targetSpecificData.SPUThreadStart.ElfFilename = Marshal.PtrToStringAnsi(data);
                        break;
                    }
                    else
                        break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStop:
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStopInit:
                    targetSpecificData.SPUThreadStop = new PS3TMAPI.SPUThreadStopData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.PC);
                    int num3 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num3));
                    targetSpecificData.SPUThreadStop.Reason = (PS3TMAPI.SPUThreadStopReason)num3;
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStop.SP);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadGroupDestroy:
                    targetSpecificData.SPUThreadGroupDestroyData = new PS3TMAPI.SPUThreadGroupDestroyData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadGroupDestroyData.ThreadGroupID);
                    break;
                case PS3TMAPI.TargetSpecificEventType.SPUThreadStopEx:
                    targetSpecificData.SPUThreadStopEx = new PS3TMAPI.SPUThreadStopExData();
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.ThreadGroupID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.ThreadID);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.PC);
                    int num4 = Marshal.ReadInt32(data, 0);
                    data = new IntPtr(data.ToInt64() + (long)Marshal.SizeOf((object)num4));
                    targetSpecificData.SPUThreadStopEx.Reason = (PS3TMAPI.SPUThreadStopReason)num4;
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<uint>(data, ref targetSpecificData.SPUThreadStopEx.SP);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDSISR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDSIPR);
                    data = PS3TMAPI.ReadDataFromUnmanagedIncPtr<ulong>(data, ref targetSpecificData.SPUThreadStopEx.MFCDAR);
                    break;
            }
            targetSpecificEvent.Data = targetSpecificData;
            return targetSpecificEvent;
        }

        private static void EventHandlerWrapper(int target, EventType type, uint param, SNRESULT result, uint length, IntPtr data, IntPtr userData)
        {
            switch (type)
            {
                case EventType.TTY:
                    MarshalTTYEvent(target, param, result, length, data);
                    break;
                case EventType.Target:
                    MarshalTargetEvent(target, param, result, length, data);
                    break;
                case EventType.FTP:
                    MarshalFTPEvent(target, param, result, length, data);
                    break;
                case EventType.PadCapture:
                    MarshalPadCaptureEvent(target, param, result, length, data);
                    break;
                case EventType.FileTrace:
                    MarshalFileTraceEvent(target, param, result, length, data);
                    break;
                case EventType.PadPlayback:
                    MarshalPadPlaybackEvent(target, param, result, length, data);
                    break;
            }
        }
        #endregion

        #region Calling Helpers
        private static IntPtr WriteDataToUnmanagedIncPtr<T>(T storage, IntPtr unmanagedBuf)
        {
            bool fDeleteOld = false;
            Marshal.StructureToPtr((object)storage, unmanagedBuf, fDeleteOld);
            return new IntPtr(unmanagedBuf.ToInt64() + (long)Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadDataFromUnmanagedIncPtr<T>(IntPtr unmanagedBuf, ref T storage)
        {
            storage = (T)Marshal.PtrToStructure(unmanagedBuf, typeof(T));
            return new IntPtr(unmanagedBuf.ToInt64() + (long)Marshal.SizeOf((object)storage));
        }

        private static IntPtr ReadAnsiStringFromUnmanagedIncPtr(IntPtr unmanagedBuf, ref string inputString)
        {
            inputString = Marshal.PtrToStringAnsi(unmanagedBuf);
            return new IntPtr(unmanagedBuf.ToInt64() + (long)inputString.Length + 1L);
        }

        private static IntPtr AllocUtf8FromString(string wcharString)
        {
            if (wcharString == null)
            {
                return IntPtr.Zero;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(wcharString);
            IntPtr destination = Marshal.AllocHGlobal((int)(bytes.Length + 1));
            Marshal.Copy(bytes, 0, destination, bytes.Length);
            Marshal.WriteByte((IntPtr)(destination.ToInt64() + bytes.Length), 0);
            return destination;
        }

        private static string Utf8FixedSizeByteArrayToString(byte[] byteArray)
        {
            if (byteArray == null)
                return "";
            int count = 0;
            byte[] numArray = byteArray;
            for (int index = 0; index < numArray.Length && (int)numArray[index] != 0; ++index)
                ++count;
            byte[] bytes = new byte[count];
            Buffer.BlockCopy((Array)byteArray, 0, (Array)bytes, 0, count);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion
    }
}