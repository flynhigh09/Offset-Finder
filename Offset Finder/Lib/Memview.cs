using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Offset_Finder
{
    class Memview
    {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);

        uint DELETE = 0x00010000;
        uint READ_CONTROL = 0x00020000;
        uint WRITE_DAC = 0x00040000;
        uint WRITE_OWNER = 0x00080000;
        uint SYNCHRONIZE = 0x00100000;

        uint END = 0xFFF; //if you have WinXP or Windows Server 2003 you must change this to 0xFFFF

        uint PROCESS_ALL_ACCESS = 0;
        public int processHandle = 0;
        public int processid = 0;
        public int processSize = 0;

        int YOUR_OFFSET = 0;

        public Memview()
        {
            END = (uint)(IsWinVistaHigher() ? 0xFFF : 0xFFFF);

            PROCESS_ALL_ACCESS = (DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER | SYNCHRONIZE | END);
        }

        public bool Attach(int pid)
        {
            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            processid = pid;

            return processHandle > 0;
        }

        public bool PauseProcess()
        {
            if (processid <= 0)
                return false;

            var proc = Process.GetProcessById(processid);
            proc.Suspend();
            return true;
        }

        public bool ContinueProcess()
        {
            if (processid <= 0)
                return false;

            var proc = Process.GetProcessById(processid);
            proc.Resume();
            return true;
        }

        public bool isSuspended()
        {
            if (processid <= 0)
                return false;

            return Process.GetProcessById(processid).isSuspended();
        }

        public void KillProcess()
        {
            if (processid <= 0)
                return;

            Process.GetProcessById(processid).Kill();
        }

        public byte[] ReadMemory(int adress, int processSize, int processHandle)
        {            
            byte[] buffer = new byte[processSize];
            ReadProcessMemory(processHandle, adress, buffer, processSize, 0);
            return buffer;
        }

        public bool ReadMemory(int adress, ref byte[] Bytes)
        {
            if (processHandle <= 0)
                return false;

            return ReadProcessMemory(processHandle, adress, Bytes, Bytes.Length, 0);
        }

        public void WriteMemory(int adress, byte[] processBytes)
        {
            if (processHandle <= 0)
                return;

            WriteProcessMemory(processHandle, adress, processBytes, processBytes.Length, 0);
        }

        static bool IsWinxporHigher()
        {
            OperatingSystem Os = Environment.OSVersion;
            return (Os.Platform == PlatformID.Win32NT) && ((Os.Version.Major > 5) ||(Os.Version.Major == 5) && (Os.Version.Major >= 5));
        }

        static bool IsWinVistaHigher()
        {
            OperatingSystem Os = Environment.OSVersion;
            return (Os.Platform == PlatformID.Win32NT) && ((Os.Version.Major >= 6));
        }

        public int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
        }
    }

    public static class ProcessExtension
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static bool isSuspended(this Process proc)
        {
            bool ret = false;
            foreach (ProcessThread thread in proc.Threads)
            {
                ret |= thread.ThreadState == ThreadState.Running;
            }
            return !ret;
        }

        public static void Suspend(this Process proc)
        {
            foreach (ProcessThread thread in proc.Threads)
            {
                var openthread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (openthread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(openthread);
            }
        }

        public static void Resume(this Process proc)
        {
            foreach (ProcessThread thread in proc.Threads)
            {
                var openthread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (openthread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(openthread);
            }
        }
}