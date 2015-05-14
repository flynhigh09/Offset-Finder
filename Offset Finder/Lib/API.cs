using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offset_Finder
{
    public class API
    {
        public API()
        {

        }

        public bool GetBytes(ulong address, ref byte[] bytes)
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();

            bytes[0] = 1;
            _ps3mapi.Process.Memory.Get(_ps3mapi.Process.Process_Pid, (uint)address, bytes);

            return true;
        }

        /// <summary>
        /// Write bytes to the memory of target process.
        /// </summary>
        public void SetBytes(ulong address, byte[] bytes)
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();

            _ps3mapi.Process.Memory.Set(_ps3mapi.Process.Process_Pid, (uint)address, bytes);
        }

        /// <summary>
        /// Shutdown game or platform
        /// </summary>
        public void Shutdown()
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();

            _ps3mapi.PS3.Power(PS3MAPI.PS3_CMD.PowerFlags.ShutDown);
        }

        private PS3MAPI _ps3mapi;
        /// <summary>
        /// Connects to target.
        /// If platform doesn't require connection, just return true.
        /// </summary>
        public bool Connect()
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();


            bool ret = false;
            if (_ps3mapi.IPAddr == "127.0.0.1")
                ret = _ps3mapi.ConnectTarget();
            else
                ret = _ps3mapi.ConnectTarget(_ps3mapi.IPAddr);

            return ret;
        }

        /// <summary>
        /// Disconnects from target.
        /// </summary>
        public void Disconnect()
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();

            _ps3mapi.DisconnectTarget();

            _ps3mapi = new PS3MAPI();
        }

        /// <summary>
        /// Attaches to target process.
        /// This should automatically continue the process if it is stopped.
        /// </summary>
        public bool Attach()
        {
            if (_ps3mapi == null)
                _ps3mapi = new PS3MAPI();

            if (_ps3mapi.Process.Process_Pid > 0)
                return _ps3mapi.AttachProcess(_ps3mapi.Process.Process_Pid);
            return _ps3mapi.AttachProcess();
        }

        /// <summary>
        /// Pauses the attached process (return false if not available feature)
        /// </summary>
        public bool PauseProcess()
        {
            return false;
        }

        /// <summary>
        /// Continues the attached process (return false if not available feature)
        /// </summary>
        public bool ContinueProcess()
        {
            return false;
        }

        /// <summary>
        /// Tells NetCheat if the process is currently stopped (return false if not available feature)
        /// </summary>
        public bool isProcessStopped()
        {
            return false;
        }
    }
}
