using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace Offset_Finder
{
    public class TMAPI
    {
        private static int NumberOffsets = 0;
        public static bool AssemblyLoaded = true;
        private static bool connection = false;

        public static PS3TMAPI.ResetParameter resetParameter;
        public static PS3TMAPI.ConnectStatus connectStatus;       

        public TMAPI()
        {
            PS3TMAPI_NET();
        }

        public class SCECMD
        {
            /// <summary>Get the target status and return the string value.</summary>
            public string SNRESULT()
            {
                return Parameters.snresult;
            }

            /// <summary>Get the target name.</summary>
            public string GetTargetName()
            {
                if (Parameters.ConsoleName == null || Parameters.ConsoleName == String.Empty)
                {
                    PS3TMAPI.InitTargetComms();
                    PS3TMAPI.TargetInfo TargetInfo = new PS3TMAPI.TargetInfo();
                    TargetInfo.Flags = PS3TMAPI.TargetInfoFlag.TargetID;
                    TargetInfo.Target = PS3TMAPI.Target;
                    PS3TMAPI.GetTargetInfo(ref TargetInfo);
                    Parameters.ConsoleName = TargetInfo.Name;
                }
                return Parameters.ConsoleName;
            }

            /// <summary>Get the target status and return the string value.</summary>
            public static string GetStatus()
            {
                if (AssemblyLoaded)
                    return "NotConnected";
                Parameters.connectStatus = new PS3TMAPI.ConnectStatus();
                PS3TMAPI.GetConnectStatus(PS3TMAPI.Target, out Parameters.connectStatus, out Parameters.usage);
                Parameters.Status = Parameters.connectStatus.ToString();
                return Parameters.Status;
            }

            /// <summary>Get the ProcessID by the current process.</summary>
            public uint ProcessID()
            {
                return Parameters.ProcessID;
            }

            /// <summary>Get an array of processID's.</summary>
            public uint[] ProcessIDs()
            {
                return Parameters.processIDs;
            }

            /// <summary>Get some details from your target.</summary>
            public PS3TMAPI.ConnectStatus DetailStatus()
            {
                return Parameters.connectStatus;
            }
        }

        public SCECMD SCE
        {
            get { return new SCECMD(); }
        }

        public class Parameters
        {
            public static string
                usage,
                info,
                snresult,
                Status,
                MemStatus,
                ConsoleName,
                GameName;
            public static uint
                ProcessID;
            public static uint[]
                processIDs;
            public static byte[]
                Retour;
            public static PS3TMAPI.ConnectStatus
                connectStatus;
        }

        /// <summary>Enum of flag reset.</summary>
        public enum ResetTarget
        {
            Hard,
            Quick,
            ResetEx,
            Soft
        }

        public void InitConnection()
        {
            if (!connection)
            {
                ConnectedAttached(0);
                connection = true;
            }
            else if (connection)
            {
                if (MessageBox.Show("PS3 Already Connected and Attached!\nDo you want to Reconnect?", "Reconnect?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                ConnectedAttached(0);
            }
            else
            {
                MessageBox.Show("PS3 Connection or Process Attach Failed!", "PS3 Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                connection = false;
            }
        }

        public void InitComms()
        {
            PS3TMAPI.InitTargetComms();
        }

        /// <summary>Connect the default target and initialize the dll. Possible to put an int as arugment for determine which target to connect.</summary>
        public bool ConnectTarget(int TargetIndex = 0)
        {
            bool result = false;
            if (AssemblyLoaded)
                PS3TMAPI_NET();
            AssemblyLoaded = false;
            PS3TMAPI.Target = TargetIndex;
            result = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
            result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(TargetIndex, null));
            return result;
        }

        /// <summary>
        /// Connects to and attach target.
        /// </summary>
        public bool ConnectedAttached(int TargetIndex = 0)
        {
            try
            {
                bool isOK = false;

                if (AssemblyLoaded)
                    PS3TMAPI_NET();
                AssemblyLoaded = false;
                PS3TMAPI.Target = TargetIndex;

                PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
                PS3TMAPI.Connect(PS3TMAPI.Target, null);

                /*run twice to make sure it retrieves it*/            
                PS3TMAPI.GetProcessList(PS3TMAPI.Target, out Parameters.processIDs);
                if (Parameters.processIDs.Length > 0)
                    isOK = true;
                else isOK = false;
                if (isOK)
                {
                    ulong uProcess = Parameters.processIDs[0];
                    Parameters.ProcessID = Convert.ToUInt32(uProcess);
                    PS3TMAPI.ProcessAttach(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID);
                    PS3TMAPI.ProcessContinue(PS3TMAPI.Target, Parameters.ProcessID);
                    Parameters.info = "The Process 0x" + Parameters.ProcessID.ToString("X8") + " Has Been Attached !";
                    Inits();
                }
               return isOK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
                return false;
            }
        }

        /// <summary>Connect the target by is name.</summary>
        public bool ConnectTarget(string TargetName)
        {
            bool result = false;
            if (AssemblyLoaded)
                PS3TMAPI_NET();
            AssemblyLoaded = false;
            result = PS3TMAPI.SUCCEEDED(PS3TMAPI.InitTargetComms());
            if (result)
            {
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.GetTargetFromName(TargetName, out PS3TMAPI.Target));
                result = PS3TMAPI.SUCCEEDED(PS3TMAPI.Connect(PS3TMAPI.Target, null));
            }
            return result;
        }

        /// <summary>Disconnect the target.</summary>
        public void DisconnectTarget()
        {
            PS3TMAPI.Disconnect(PS3TMAPI.Target);
        }

        /// <summary>
        /// Retrieve target Ip.
        /// </summary>
        public string GetIP()
        {
            PS3TMAPI.TCPIPConnectProperties ip = new PS3TMAPI.TCPIPConnectProperties();
            PS3TMAPI.GetConnectionInfo(0, out ip);
            return ip.IPAddress.ToString();
        }

        /// <summary>
        /// Retrieve target Mac.
        /// </summary>
        public string GetMac()
        {
            string macAddress = null;
            PS3TMAPI.GetMACAddress(PS3TMAPI.Target, out macAddress);
            return macAddress;
        }

        /// <summary>
        /// Get game update version.
        /// </summary>
        public string GetGame()
        {
            PS3TMAPI.ProcessInfo infos = new PS3TMAPI.ProcessInfo();
            PS3TMAPI.GetProcessInfo(PS3TMAPI.Target, Parameters.ProcessID, out infos);
            string[] str = infos.Hdr.ELFPath.Split('/');
            string ID = str[3];
            try
            {
                System.Net.WebClient seeker = new System.Net.WebClient();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string content = seeker.DownloadString("https://a0.ww.np.dl.playstation.net/tpl/np/" + ID + "/" + ID + "-ver.xml").Replace("<TITLE>", ";");
                string name = content.Split(';')[1].Replace("</TITLE>", ";");
                return name.Split(';')[0];
            }
            catch
            {
                return ID;
            }
        }

        /// <summary>Install Pkg File.</summary>
        public void InstallPackage(string packagePath)
        {
            PS3TMAPI.InstallPackage(PS3TMAPI.Target, packagePath);
        }

        /// <summary>Upload File.</summary>
        public void UploadFile(string source, string dest, out uint txID)
        {
            PS3TMAPI.UploadFile(PS3TMAPI.Target, source, dest, out txID);
        }

        public void GetProcessName(uint processId, out string name)
        {
            PS3TMAPI.GetProcessName(processId, out name);
        }

        public uint Inits()
        {
            uint proc = Parameters.ProcessID = GetProcessID();
            return proc;
        }

        /// <summary>Get Process list.</summary>
        public uint GetProcessID()
        {
            uint[] numArray;
            GetProcessList(out numArray);
            return numArray[0];
        }

        ///CCAPI
        public int GetProcesslist(out uint[] processIDs)
        {
            return PS3TMAPI.GetProcessList(out processIDs);
        }

        public uint[] GetProcessID(out uint[] processId)
        {
            GetProcessList(out processId);
            return processId;
        }

        /// <summary>Get Process list.</summary>
        public PS3TMAPI.SNRESULT GetProcessList(out uint[] processIDs)
        {
            return PS3TMAPI.GetProcessList(PS3TMAPI.Target, out processIDs);
        }
      
        /// <summary>Get Module list.</summary>// New
        public uint[] GetModule(out uint[] modules)
        {
            PS3TMAPI.GetModuleList(PS3TMAPI.Target, Parameters.ProcessID, out modules);
            return modules;
        }

        /// <summary>Get Module list.</summary>
        public PS3TMAPI.SNRESULT GetModuleList(out uint[] modules)
        {
           return PS3TMAPI.GetModuleList(PS3TMAPI.Target, Parameters.ProcessID, out modules);
        }

        /// <summary>Get Module Info.</summary>
        public PS3TMAPI.SNRESULT GetModuleInfo(uint moduleID, PS3TMAPI.ModuleInfo moduleInfo)
        {
           return PS3TMAPI.GetModuleInfo(PS3TMAPI.Target, Parameters.ProcessID, moduleID, out moduleInfo);
        }

        /// <summary>Get thread list.</summary>
        public PS3TMAPI.SNRESULT GetThreadList(out ulong[] ppuThreadIDs, out ulong[] spuThreadIDs)
        {
            return PS3TMAPI.GetThreadList(PS3TMAPI.Target, Parameters.ProcessID, out ppuThreadIDs, out spuThreadIDs);
        }

        public PS3TMAPI.ProcessInfo ReturnProcessInfo()
        {
            PS3TMAPI.ProcessInfo pInfo;
            GetProcessInfo(out pInfo);
            return pInfo;
        }

        public PS3TMAPI.SNRESULT GetProcessInfo(out PS3TMAPI.ProcessInfo pInfo)
        {
            return PS3TMAPI.GetProcessInfo(PS3TMAPI.Target, Parameters.ProcessID, out pInfo);
        }

        /// <summary>Get thread list.</summary>
        public PS3TMAPI.SNRESULT GetPPUThreadInfo(ulong threadID, out PS3TMAPI.PPUThreadInfo threadInfo)
        {
            return PS3TMAPI.GetPPUThreadInfo(PS3TMAPI.Target, Parameters.ProcessID, threadID, out threadInfo);
        }

        /// <summary>Power on selected target.</summary>
        public void PowerOn(int numTarget = 0)
        {
            if (PS3TMAPI.Target != 0xFF)
                numTarget = PS3TMAPI.Target;
            PS3TMAPI.PowerOn(numTarget);
        }

        /// <summary>Power off selected target.</summary>
        public void PowerOff(bool Force)
        {
            PS3TMAPI.PowerOff(PS3TMAPI.Target, Force);
        }

        /// <summary>Attach and continue the current process from the target.</summary>
        public bool AttachProcess()
        {
            bool isOK = false;
            PS3TMAPI.GetProcessList(PS3TMAPI.Target, out Parameters.processIDs);
            if (Parameters.processIDs.Length > 0)
                isOK = true;
            else isOK = false;
            if (isOK)
            {
                ulong uProcess = Parameters.processIDs[0];
                Parameters.ProcessID = Convert.ToUInt32(uProcess);
                PS3TMAPI.ProcessAttach(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID);
                PS3TMAPI.ProcessContinue(PS3TMAPI.Target, Parameters.ProcessID);
                Parameters.info = "The Process 0x" + Parameters.ProcessID.ToString("X8") + " Has Been Attached !";
            }
            return isOK;
        }

        /// <summary>Attach the current process from the target.</summary>
        public bool AttachProcOnly()
        {
            bool isOK = false;
            PS3TMAPI.GetProcessList(PS3TMAPI.Target, out Parameters.processIDs);
            if (Parameters.processIDs.Length > 0)
                isOK = true;
            else isOK = false;
            if (isOK)
            {
                ulong uProcess = Parameters.processIDs[0];
                Parameters.ProcessID = Convert.ToUInt32(uProcess);
                PS3TMAPI.ProcessAttach(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID);
                Parameters.info = "The Process 0x" + Parameters.ProcessID.ToString("X8") + " Has Been Attached !";
            }
            return isOK;
        }

        public void ContinueProcess()
        {
            PS3TMAPI.ProcessContinue(PS3TMAPI.Target, Parameters.ProcessID);
        }

        /// <summary>Clear BreakPopint in mem.</summary>// New
        public PS3TMAPI.SNRESULT ClearBreakPoint(ulong address)
        {
            ulong threadID = 0;
            return PS3TMAPI.ClearBreakPoint(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, threadID, address);
        }

        /// <summary>Set BreakPopint To mem.</summary>// New
        public PS3TMAPI.SNRESULT SetBreakPoint(ulong address)
        {
            ulong threadID = 0;
          return  PS3TMAPI.SetBreakPoint(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, threadID, address);
        }

        /// <summary>Get Compressed memory from address.</summary>// New
        public PS3TMAPI.SNRESULT GetMemoryCompressed(uint address, ref byte[] buff)
        {
            PS3TMAPI.MemoryCompressionLevel compressionLevel = PS3TMAPI.MemoryCompressionLevel.Default;
            return PS3TMAPI.GetMemoryCompressed(PS3TMAPI.Target, Parameters.ProcessID, compressionLevel, address, ref buff);
        }

        /// <summary>Get Compressed memory from address.</summary>// New
        public PS3TMAPI.SNRESULT GetMemory64Compressed(uint address, ref byte[] buff)
        {
            PS3TMAPI.MemoryCompressionLevel compressionLevel = PS3TMAPI.MemoryCompressionLevel.Default;
            return PS3TMAPI.GetMemory64Compressed(PS3TMAPI.Target, Parameters.ProcessID, compressionLevel, address, ref buff);
        }

        /// <summary>Set memory to the target (byte[]).</summary>
        public void SetMemory(uint Address, byte[] Bytes)
        {
            PS3TMAPI.ProcessSetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Bytes);
        }
        /// <summary>Set memory to the address (byte[]).</summary>
        public void SetMemory(uint Address, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            PS3TMAPI.ProcessSetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, bytes);
        }
        public void SetMemory(uint Address, byte[] Bytes, uint thread = 0)
        {
            PS3TMAPI.InitTargetComms();
            PS3TMAPI.ProcessSetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, thread, Address, Bytes);
        }
        public void SetMemory(uint Address, uint value, int size)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            PS3TMAPI.ProcessSetMemorySize(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, bytes, size);
        }
        public void SetMemory(uint Address, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            PS3TMAPI.ProcessSetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, bytes);
        }
        /// <summary>Set memory with value as string hexadecimal to the address (string).</summary>
        public void SetMemory(uint Address, string hexadecimal)
        {
            byte[] Entry = Extension.StringBAToBA(hexadecimal);
            Array.Reverse(Entry);
            PS3TMAPI.ProcessSetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, Entry);
        }

        public byte[] GetMemoryInt(uint address, int length)
        {
            byte[] numArray = new byte[length];
            PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0UL, (ulong)address, ref numArray);
            return numArray;
        }
        
        /// <summary>Get memory from the address.</summary>
        public PS3TMAPI.SNRESULT GetMemory(uint Address, byte[] Bytes)
        {
            return PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Bytes);
        }
        public PS3TMAPI.SNRESULT GetMemory(uint Address, int length)
        {
            byte[] memout = new byte[length];
            return PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref memout);
        }
        public byte[] GetMem(uint Address, int length)
        {
            byte[] memout = new byte[length];
            PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref memout);
            return memout;
        }

        /// <summary>Get a bytes array with the length input.</summary>
        public byte[] GetBytes(uint Address, uint lengthByte)
        {
            byte[] Longueur = new byte[lengthByte];
            PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
            return Longueur;
        }

        /// <summary>Get a string with the length input.</summary>
        public string GetString(uint Address, uint lengthString)
        {
            byte[] Longueur = new byte[lengthString];
            PS3TMAPI.ProcessGetMemory(PS3TMAPI.Target, PS3TMAPI.UnitType.PPU, Parameters.ProcessID, 0, Address, ref Longueur);
            string StringResult = Hex2Ascii(ReplaceString(Longueur));
            return StringResult;
        }     

        internal byte[] Reverse(byte[] buff)
        {
            Array.Reverse(buff);
            return buff;
        }

        internal static string Hex2Ascii(string hex)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i <= (hex.Length - 2); i += 2)
            {
                builder.Append(Convert.ToString(Convert.ToChar(int.Parse(hex.Substring(i, 2), NumberStyles.HexNumber))));
            }
            return builder.ToString();
        }

        internal static string ReplaceString(byte[] bytes)
        {
            string PSNString = BitConverter.ToString(bytes);
            PSNString = PSNString.Replace("00", string.Empty);
            PSNString = PSNString.Replace("-", string.Empty);
            for (int i = 0; i < 10; i++)
                PSNString = PSNString.Replace("^" + i.ToString(), string.Empty);
            return PSNString;
        }       
      
        public ulong GetPointeradr(ulong addr, int step)
        {
            return (ulong)(readInt((uint)addr + (uint)step));
        }

        public ulong GetPointer(ulong addr)
        {
            ulong pointer = (ulong)(readInt((uint)addr));
            return (ulong)(readInt((uint)pointer));
        }

        public void SetPointeradr(ulong addr, byte[] mem)
        {
            ulong pointer = (ulong)(readInt((uint)addr));
            SetMemory((uint)pointer, mem);
        }

        public void SetPointeradr(ulong addr, byte[] mem, int step)
        {
           int pointer = (int)addr + step;

           SetMemory((uint)pointer, mem);
        }

        private static byte[] myBuffer = new byte[0x20];
        public short ReadShort(uint address, bool dvar = false)
        {
            byte[] buff = GetMem(address, 2);
            if (!dvar) { Array.Reverse(buff); }
            short val = BitConverter.ToInt16(buff, 0);
            return val;
        }
        public byte ReadByte(uint address)
        {
            byte[] buff = GetMem(address, 1);
            return buff[0];
        }
        public void ReadBytes(uint address, byte[] mem)
        {
            int memory = BitConverter.ToInt32(mem, 0);
            GetMemory(address, memory);
        }
        public byte[] ReadintByte(uint address, int mem)
        {
            return GetMem(address, mem);
        }
        public byte[] ReadInt(uint address, int mem)
        {
            GetMemory(address, mem);
            byte[] mem2 = BitConverter.GetBytes(mem);
            return mem2;
        }
        public int readInt(uint Address, int mem)
        {
            Byte[] read = GetMem(Address, mem);
            Array.Reverse(read);
            return BitConverter.ToInt32(read, 0);
        }
        public int readInt(uint Address)
        {
            int mem = 0;
            Byte[] read = GetMem(Address, mem);
            Array.Reverse(read);
            return BitConverter.ToInt32(read, 0);
        }
        public uint ReadUInt(uint address)
        {
            byte[] buff = new byte[4]; //declaring buff as byte, and byte buff = 4 bytes long [4]
            GetMemory(address, buff); // Ps3.Getmemory(youraddress, buff) which buff = 4 bytes long
            Array.Reverse(buff); // reverses the 4 bytes from array, so
            uint val = BitConverter.ToUInt32(buff, 0); //Converts buff from byte to Uint32
            return val; //val = your output 
        }
        public float ReadFloat(uint offset)
        {
            GetMemory(offset, myBuffer);
            Array.Reverse(myBuffer, 0, 4);
            return BitConverter.ToSingle(myBuffer, 0);
        }
        public uint ReadUIntB(uint address, byte[] length)
        {
            byte[] buff = new byte[4];
            GetMemory(address, buff);
            Array.Reverse(buff);
            uint val = BitConverter.ToUInt32(buff, 0);
            return val;
        }
        public string ReadString(uint address)
        {
            int length = 40;
            int message = 0;
            string source = "";
            do
            {
                byte[] memory = GetMemoryInt(address + ((uint)message), length);
                source = source + Encoding.UTF8.GetString(memory);
                message += length;
            } while (!source.Contains<char>('\0'));
            int index = source.IndexOf('\0');
            string str2 = source.Substring(0, index);
            source = string.Empty;
            return str2;
        }
        public void Or_Int32(uint address, int input)
        {
            int num = ReadInt32(address) | input;
            WriteInt32(address, num);
        }
        public bool ReadBool(uint address)
        {
            return (GetMemoryInt(address, 1)[0] != 0);
        }
        public byte ReadByteB(uint address)
        {
            byte[] buff = GetMemoryInt(address, 1);
            return buff[0];
        }
        public byte[] ReadBytes(uint address, int length)
        {
            return GetMemoryInt(address, length);
        }
        public double ReadDouble(uint address)
        {
            byte[] memory = GetMemoryInt(address, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToDouble(memory, 0);
        }
        public double[] ReadDouble(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 8);
            Reverse(memory);
            double[] numArray = new double[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToSingle(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }
        public short ReadInt16(uint address)
        {
            byte[] memory = GetMemoryInt(address, 2);
            Array.Reverse(memory, 0, 2);
            return BitConverter.ToInt16(memory, 0);
        }
        public short[] ReadInt16(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 2);
            Reverse(memory);
            short[] numArray = new short[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt16(memory, ((length - 1) - i) * 2);
            }
            return numArray;
        }
        public int ReadInt32(uint address)
        {
            byte[] memory = GetMemoryInt(address, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToInt32(memory, 0);
        }
        public int[] ReadInt32(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 4);
            Reverse(memory);
            int[] numArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt32(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }
        public long ReadInt64(uint address)
        {
            byte[] memory = GetMemoryInt(address, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToInt64(memory, 0);
        }
        public long[] ReadInt64(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 8);
            Reverse(memory);
            long[] numArray = new long[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt64(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }
        public sbyte ReadSByte(uint address)
        {
            return (sbyte)GetMemoryInt(address, 1)[0];
        }
        public sbyte[] ReadSBytes(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length);
            sbyte[] numArray = new sbyte[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = (sbyte)memory[i];
            }
            return numArray;
        }
        public float ReadSingle(uint address)
        {
            byte[] memory = GetMemoryInt(address, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToSingle(memory, 0);
        }
        public int ReadInt(uint address)
        {
            byte[] buff = GetMemoryInt(address, 4);
            Array.Reverse(buff);
            int val = BitConverter.ToInt32(buff, 0);
            return val;
        }
        public int ReadShort(uint address)
        {
            byte[] buffer = GetMemoryInt(address, 2);
            return (short)((buffer[0] << 8) + buffer[1]);
        }
        public float[] ReadSingle(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 4);
            Reverse(memory);
            float[] numArray = new float[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToSingle(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }
        public ushort ReadUInt16(uint address)
        {
            byte[] memory = GetMemoryInt(address, 2);
            Array.Reverse(memory, 0, 2);
            return BitConverter.ToUInt16(memory, 0);
        }
        public ushort[] ReadUInt16(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 2);
            Reverse(memory);
            ushort[] numArray = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt16(memory, ((length - 1) - i) * 2);
            }
            return numArray;
        }
        public uint ReadUInt32(uint address)
        {
            byte[] memory = GetMemoryInt(address, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToUInt32(memory, 0);
        }
        public uint[] ReadUInt32(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 4);
            Reverse(memory);
            uint[] numArray = new uint[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt32(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }
        public ulong ReadUInt64(uint address)
        {
            byte[] memory = GetMemoryInt(address, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToUInt64(memory, 0);
        }
        public ulong[] ReadUInt64(uint address, int length)
        {
            byte[] memory = GetMemoryInt(address, length * 8);
            Reverse(memory);
            ulong[] numArray = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt64(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }
        public void WriteInt(uint address, int val)
        {
            SetMemory(address, Reverse(BitConverter.GetBytes(val)), 0);
        }
        public void WriteShort(uint address, int val, bool dvar = false)
        {
            byte[] data = BitConverter.GetBytes(val);
            if (!dvar)
                SetMemory(address, new byte[] { data[0], data[1] }, 0);
            else
                SetMemory(address, new byte[] { data[1], data[0] }, 0);
        }
        public void WriteUInt(uint address, uint val)
        {
            SetMemory(address, Reverse(BitConverter.GetBytes(val)), 0);
        }
        public void WriteBool(uint address, bool input)
        {
            byte[] bytes = { input ? ((byte)1) : ((byte)0) };
            SetMemory(address, bytes);
        }
        public void WriteByte(uint address, byte input)
        {
            SetMemory(address, new[] { input });
        }
        public void WriteBytes(uint address, byte[] input)
        {
            SetMemory(address, input);
        }
        public bool WriteBytesToggle(uint Offset, byte[] On, byte[] Off)
        {
            bool flag = ReadByte(Offset) == On[0];
            WriteBytes(Offset, !flag ? On : Off);
            return flag;
        }
        public void WriteDouble(uint address, double input)
        {
            byte[] array = new byte[8];
            BitConverter.GetBytes(input).CopyTo(array, 0);
            Array.Reverse(array, 0, 8);
            SetMemory(address, array);
        }
        public void WriteDouble(uint address, double[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 8));
            }
            SetMemory(address, array);
        }
        public void WriteInt(uint address, int[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            SetMemory(address, array);
        }
        public void WriteInt16(uint address, short input)
        {
            byte[] array = new byte[2];
            Reverse(BitConverter.GetBytes(input)).CopyTo(array, 0);
            SetMemory(address, array);
        }
        public void WriteInt16(uint address, short[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 2));
            }
            SetMemory(address, array);
        }
        public void WriteInt32(uint address)
        {
            int[] input = new int[0];
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            SetMemory(address, array);
        }
        public void WriteInt32(uint address, int input)
        {
            byte[] array = new byte[4];
            Reverse(BitConverter.GetBytes(input)).CopyTo(array, 0);
            SetMemory(address, array);
        }
        public void WriteInt32(uint address, int[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            SetMemory(address, array);
        }
        public void WriteInt64(uint address, long input)
        {
            byte[] array = new byte[8];
            Reverse(BitConverter.GetBytes(input)).CopyTo(array, 0);
            SetMemory(address, array);
        }
        public void WriteInt64(uint address, long[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 8));
            }
            SetMemory(address, array);
        }
        public void WriteSByte(uint address, sbyte input)
        {
            byte[] bytes = { (byte)input };
            SetMemory(address, bytes);
        }
        public void WriteSBytes(uint address, sbyte[] input)
        {
            int length = input.Length;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)input[i];
            }
            SetMemory(address, bytes);
        }
        public void WriteSingle(uint address, float input)
        {
            byte[] array = new byte[4];
            BitConverter.GetBytes(input).CopyTo(array, 0);
            Array.Reverse(array, 0, 4);
            SetMemory(address, array);
        }
        public void WriteSingle(uint address, float[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            SetMemory(address, array);
        }
        public void WriteString(uint address, string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            Array.Resize<byte>(ref bytes, bytes.Length + 1);
            SetMemory(address, bytes);
        }
        public void WriteUInt16(uint address, ushort input)
        {
            byte[] array = new byte[2];
            BitConverter.GetBytes(input).CopyTo(array, 0);
            Array.Reverse(array, 0, 2);
            SetMemory(address, array);
        }
        public void WriteUInt16(uint address, ushort[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 2));
            }
            SetMemory(address, array);
        }
        public void WriteUInt32(uint address, uint input)
        {
            byte[] array = new byte[4];
            BitConverter.GetBytes(input).CopyTo(array, 0);
            Array.Reverse(array, 0, 4);
            SetMemory(address, array);
        }
        public void WriteUInt32(uint address, uint[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            SetMemory(address, array);
        }
        public void WriteUInt64(uint address, ulong input)
        {
            byte[] array = new byte[8];
            BitConverter.GetBytes(input).CopyTo(array, 0);
            Array.Reverse(array, 0, 8);
            SetMemory(address, array);
        }
        public void WriteUInt64(uint address, ulong[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                Reverse(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 8));
            }
            SetMemory(address, array);
        }

        public void WriteFloat(uint address, float val)
        {
            SetMemory(address, Reverse(BitConverter.GetBytes(val)));
        }

        public class Extension
        {
            public static string ByteArrayToString(byte[] buffer, int startIndex, int maxLength = 0)
            {
                int max = startIndex + maxLength;
                if (max == startIndex)
                    max = buffer.Length;
                string ret = "";

                for (int x = startIndex; x < max; x++)
                {
                    if (buffer[x] == 0)
                        break;
                    ret += ((char)buffer[x]).ToString();
                }
                return ret;
            }

            public static uint ContainsSequence(byte[] toSearch, byte[] toFind, uint StartOffset, int bytes)
            {
                for (int i = 0; (i + toFind.Length) < toSearch.Length; i += bytes)
                {
                    bool flag = true;
                    for (int j = 0; j < toFind.Length; j++)
                    {
                        if (toSearch[i + j] != toFind[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        NumberOffsets++;
                        int num3 = ((int)StartOffset) + i;
                        return (uint)num3;
                    }
                }
                return 0;
            }

            private static uint ContainsSequence2(byte[] toSearch, byte[] toFind, uint StartOffset, int bytes)
            {
                int num = 0;
                while (num + toFind.Length < toSearch.Length)
                {
                    bool flag = true;
                    for (int index = 0; index < toFind.Length; ++index)
                    {
                        if ((int)toSearch[num + index] != (int)toFind[index])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        ++NumberOffsets;
                        return StartOffset + (uint)num;
                    }
                    num += bytes;
                }
                return 0U;
            }

            public static byte[] StringBAToBA(string str)
            {
                if (str == null || (str.Length % 2) == 1)
                    return new byte[0];
                byte[] ret = new byte[str.Length / 2];
                for (int x = 0; x < str.Length; x += 2)
                    ret[x / 2] = byte.Parse(sMid(str, x, 2), System.Globalization.NumberStyles.HexNumber);
                return ret;
            }
            public static string sMid(string text, int index, int length)
            {
                if (length < 0)
                    throw new ArgumentOutOfRangeException("length", length, "length must be > 0");
                else if (length == 0 || text.Length == 0)
                    return "";
                else if (text.Length < (length + index))
                    return text;
                else
                    return text.Substring(index, length);
            }

            public static bool ArrayCompare(byte[] b1, byte[] b2)
            {
                if (b1.Length == b2.Length)
                {
                    for (int i = 0; i < b1.Length; i++)
                        if (b1[i] != b2[i])
                            return false;
                    return true;
                }
                return false;
            }

            /// <summary>Reset target to XMB , Sometimes the target restart quickly.</summary>
            public static void ResetToXMB(ResetTarget flag)
            {
                if (flag == ResetTarget.Hard)
                    resetParameter = PS3TMAPI.ResetParameter.Hard;
                else if (flag == ResetTarget.Quick)
                    resetParameter = PS3TMAPI.ResetParameter.Quick;
                else if (flag == ResetTarget.ResetEx)
                    resetParameter = PS3TMAPI.ResetParameter.ResetEx;
                else if (flag == ResetTarget.Soft)
                    resetParameter = PS3TMAPI.ResetParameter.Soft;
                PS3TMAPI.Reset(PS3TMAPI.Target, resetParameter);
            }
        }

        internal static Assembly LoadApi;
        ///<summary>Load the PS3 API for use with your Application .NET.</summary>
        public Assembly PS3TMAPI_NET()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var filename = new AssemblyName(e.Name).Name;
                var x = string.Format(@"C:\Program Files\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                var x64 = string.Format(@"C:\Program Files (x64)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                var x86 = string.Format(@"C:\Program Files (x86)\SN Systems\PS3\bin\ps3tmapi_net.dll", filename);
                if (File.Exists(x))
                    LoadApi = Assembly.LoadFile(x);
                else
                {
                    if (File.Exists(x64))
                        LoadApi = Assembly.LoadFile(x64);

                    else
                    {
                        if (File.Exists(x86))
                            LoadApi = Assembly.LoadFile(x86);
                        else
                        {
                            MessageBox.Show("Target Manager API cannot be founded to:\r\n\r\n" + x86, "Error with PS3 API!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return LoadApi;
            };
            return LoadApi;
        }
    }
}
