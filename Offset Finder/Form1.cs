using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SPRXCALLS;

namespace Offset_Finder
{
    public partial class Form1 : Form
    {
        private PS3MAPI _ps3mapi = new PS3MAPI();
        private static TMAPI _tmapi = new TMAPI();
        private static CCAPI _ccapi = new CCAPI();
        private Memview _memview = new Memview();
        
        private int Api = 0;
        private uint NumberOffsets = 0U;
        private uint ZeroOffset;
        private uint OffsetFounded = 0;
        private static uint progress = 0U;

        private static uint TableOfContents = 0, HookFuncAddr = 0;
        private static uint Start = 0x10020000;

        private static bool IsCONNECTED = false;
        private static bool IsATTACHED = false;
        private bool Checkinout;

        private List<String> Addresses = new List<String>();

        #region ><><>< All ><><><
        #region <><> Main <><>
        public static class Globals
        {
            public static string TITLE;
            public static string TEMPFOLDER = System.IO.Path.Combine(System.IO.Path.GetTempPath().ToString(), Guid.NewGuid().ToString()).ToString();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode)
            {
                Checkinout = true;
                Opacity = 0;
                Screentimer.Enabled = true;
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel == true)
                return;
            if (Opacity > 0)
            {
                Checkinout = false;
                Screentimer.Enabled = true;
                e.Cancel = true;
            }
        }
        private void Screentimer_Tick(object sender, EventArgs e)
        {
            if (Checkinout == false)
            {
                Opacity -= (Screentimer.Interval / 750.0);
                if (Opacity > 0)
                    Screentimer.Enabled = true;
                else
                {
                    Screentimer.Enabled = false;
                    Close();
                }
            }
            else
            {
                Opacity += (Screentimer.Interval / 750.0);
                Screentimer.Enabled = (Opacity < 1.0);
                Checkinout = (Opacity < 1.0);
            }
        }
        public void GetIP()
        {
            if (Api == 1)
            {
                IP.Text = _tmapi.GetIP();
                IP.ForeColor = Color.Chocolate;
            }
            else if (Api == 2)
            {
                IP.Text = CEXIPBox.Text;
                IP.ForeColor = Color.Chocolate;
            }
        }
        public void GetMac()
        {
            User.Text = _tmapi.GetMac();
            User.ForeColor = Color.Chocolate;
        }
        public void NotConnected()
        {
            Connection.ForeColor = Color.Red;
            Connection.Text = ("Not Connected");
        }
        public void IsConnected()
        {
            if (Api == 1)
            {
                Connection.Text = TMAPI.SCECMD.GetStatus();
                Connection.ForeColor = Color.LimeGreen;
                IsCONNECTED = true;
            }
            else if (Api ==2)
            {
                Connection.Text = "Connected";
                Connection.ForeColor = Color.LimeGreen;
                IsCONNECTED = true;
            }
        }
        public void NotAttached()
        {
            Attached.ForeColor = Color.Red;
            Attached.Text = ("Not Attached");
        }
        public void IsAttached()
        {
            Attached.ForeColor = Color.DarkBlue;
            Attached.Text = ("Attached:");
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void AllButtons_MouseEnter(object sender, EventArgs e)
        {
            var currentButton = sender as Button;
            var name = currentButton.Name;
            currentButton.ForeColor = Color.Chartreuse;
            currentButton.BackgroundImage = Properties.Resources.ready;
            currentButton.BackgroundImageLayout = ImageLayout.Stretch;
        }
        private void AllButtons_MouseLeave(object sender, EventArgs e)
        {
            var currentButton = sender as Button;
            var name = currentButton.Name;
            currentButton.ForeColor = Color.Black;
            currentButton.BackgroundImage = Properties.Resources.PS3_2;
            currentButton.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public static void SV_GameSendServerCommand(int client, string command)
        {                   //MLB15 
            SPRX.Call<string>(0X821CEC, true, new object[] { (int)client, 0, command });
        }
        public static void iPrintln(int client, string txt)
        {
            SV_GameSendServerCommand(client, "f \"" + txt + "\"");
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GamesList.Enabled = false;
            foreach (var button in Controls.OfType<Button>())
            {
                button.MouseEnter += AllButtons_MouseEnter;
                button.MouseLeave += AllButtons_MouseLeave;
            }
            string VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Globals.TITLE = this.Text; this.Text = this.Text + " - v" + VERSION;
        }
        private void Form1_Closed(object sender, EventArgs e)
        {
        }
        public void ReConnectAndAttach()
        {
            try
            {
                if (Api == 1)
                {
                    _tmapi.InitConnection(); IsConnected();
                    IsConnected();
                }
                else if (Api == 2)
                {
                    _ccapi.ConnectTarget(CEXIPBox.Text);
                    _ccapi.AttachProcess(); IsConnected();
                }
                else if (Api == 3)
                {
                   _ps3mapi.AttachProcess(_ps3mapi.Process.Process_Pid); IsConnected();
                }
                else
                {
                    MessageBox.Show("PS3 Connection or Attach Failed!", "PS3 Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    IsCONNECTED = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void Connect_Click(object sender, EventArgs e)
        {
            if (Api == 1)
            {
                if (_tmapi == null)
                    _tmapi = new TMAPI();
                _tmapi.InitConnection();

                IsConnected(); IsAttached();
                GetIP(); GetMac();
                Text = _tmapi.GetGame(); Game.ForeColor = Color.CadetBlue;

                SPRX.setTOC(0x010548D8);
                SPRX.setHookAddress(0x988B28);
                SV_GameSendServerCommand(0, "Hey Fly");

                var procs = new uint[64];
                _tmapi.GetProcesslist(out procs);  // 0x01140200 = ptr = {82484176} Eboot.bin
                var tt = _tmapi.ReturnProcessInfo();
                // uint[] modules = _tmapi.GetModule(out procs);
                // uint[] module = _tmapi.GetProcessID(out procs);

                ProcsBox.Items.Clear();
                for (int i = 0; i < procs.Length; i++)
                {
                    string name = String.Empty;
                    _tmapi.GetProcessName(procs[i], out name);
                    ProcsBox.Items.Add(name);
                }
            }
            else if (Api == 2)
            {
                if (_ccapi == null)
                    _ccapi = new CCAPI();

                _ccapi.ConnectTarget(CEXIPBox.Text);
                _ccapi.AttachProcess();
                IsConnected(); IsAttached(); GetIP();

                var procs = new uint[64];
                int proc = _ccapi.GetProcessList(out procs);
                ProcsBox.Items.Clear();
                for (int i = 0; i < procs.Length; i++)
                {
                    string name = String.Empty;
                    _ccapi.GetProcessName(procs[i], out name);
                    ProcsBox.Items.Add(name);
                }
            }
            else if (Api == 3)
            {
                if (_ps3mapi == null)
                    _ps3mapi = new PS3MAPI();

                if (_ps3mapi.ConnectTarget())
                {
                    _ps3mapi.AttachProcess();                  

                    ProcsBox.Items.Clear();
                    foreach (uint pid in _ps3mapi.Process.GetPidProcesses())
                    {

                        if (pid != 0) ProcsBox.Items.Add(_ps3mapi.Process.GetName(pid));
                        else break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Unable to Connect/Attach", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                NotAttached(); NotConnected();
            }
        }
        private void CEXIPBox_TextChanged(object sender, EventArgs e)
        {
            if (CEXIPBox.TextLength != 0)
            {
                Connect.Enabled = true;
            }
            else
            {
                Connect.Enabled = false;
            }
        }
        private void DEX_CheckedChanged(object sender, EventArgs e)
        {
            if (DEX.Checked)
            {
                Api = 1;
                CEXIPBox.Visible = false;
                Connect.Enabled = true;
            }
        }
        private void CEX_CheckedChanged(object sender, EventArgs e)
        {
            if (CEX.Checked)
            {
                Api = 2;
                CEXIPBox.Visible = true;
                CEXIPBox.Enabled = true;
                Connect.Enabled = false;
            }
        }
        private void Ps3Mapi_CheckedChanged(object sender, EventArgs e)
        {
            if (Ps3Mapi.Checked)
            {
                Api = 3;
                GamesList.Enabled = true;
                CEXIPBox.Visible = false;
                Connect.Enabled = true;
            }
            else
              GamesList.Enabled = false;
        }
        private void Juststring_CheckedChanged(object sender, EventArgs e)
        {
            if (Juststring.Checked)
            {
                HighBox.Text = "0x10000";
                OffsetBox.Text = "0x00010000";

                OffsetLowDiff.Text = "Search Max Length";
                OffsetLowDiff.Location = new System.Drawing.Point(2, 205);

                OffsetHighDiff.Text = "Search Size";
                OffsetHighDiff.Location = new System.Drawing.Point(65, 246);
            }
            else if (!Juststring.Checked)
            {
                HighBox.Text = "";
                OffsetBox.Text = "";

                OffsetLowDiff.Text = "String @Low Diff";
                OffsetLowDiff.Location = new System.Drawing.Point(17, 205);

                OffsetHighDiff.Text = "String @High Diff";
                OffsetHighDiff.Location = new System.Drawing.Point(11, 246);
            }
        }
        private void TOCBox_CheckedChanged(object sender, EventArgs e)
        {
            if (TOCBox.Checked)
            {
                HighBox.Enabled = false;

                OffsetBox.Text = "0x01000000";
                LowBox.Text = "0x02000000";
            }
            else if (!TOCBox.Checked)
            {
                HighBox.Enabled = true;

                OffsetBox.Text = "";
                LowBox.Text = "";
            }
        }
        private void InstallPkg_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFd = new OpenFileDialog();
            OFd.ShowDialog();
            if (OFd == null)
                return;
            if (_tmapi == null)
                _tmapi = new TMAPI();
            _tmapi.InstallPackage(OFd.FileName.Replace("\\", "/"));
        }
        #endregion

        #region ><><>< Bg Workers ><><><
        #region ><><> Bg Worker1 <><><
        private void FlyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReConnectAndAttach(); BackgroundWorker Flyworker = sender as BackgroundWorker;
            uint offsetstart = Convert.ToUInt32(OffsetBox.Text.Replace("0x", ""), 16);
            uint Lowoffset = Convert.ToUInt32(LowBox.Text.Replace("0x", ""), 16);
            uint Highoffset = Convert.ToUInt32(HighBox.Text.Replace("0x", ""), 16);
            bool Bools = Convert.ToBoolean(Convert.ToUInt32(BoolBox.Text));

            for (int i = 0; i <= 1; i++)
            {
                Flyworker.ReportProgress(i);
                //Flyworker.ReportProgress(i * 10, i);//reports ProgressPercentage AND Userstate
                Thread.Sleep(1000);
                if (FlyWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }

            if (Api == 1)
            {
                uint SearchBytez = CombineHL(FindAddress(SearchBytes.Text, offsetstart), Highoffset, Lowoffset, Bools);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = (uint)SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            else if (Api == 2)
            {
                uint SearchBytez = CombineHL(FindAddressCcapi(SearchBytes.Text, offsetstart), Highoffset, Lowoffset, Bools);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            else if (Api == 3)
            {
                uint SearchBytez = CombineHL(FindAddress(SearchBytes.Text, offsetstart), Highoffset, Lowoffset, Bools);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            for (int i = 95; i <= 100; i++)
            {
                Flyworker.ReportProgress(i);
                Thread.Sleep(500);
            }
        }

        private void FlyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 0)
            {
                ProgressBar.Value = e.ProgressPercentage;
                OffsetFound.Text = "Searching..." + ProgressBar.Value.ToString() + "%";
                OffsetFound.ForeColor = Color.DarkRed;
            }
            else
            { }
        }
        private void FlyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (String Temp in Addresses)
            {
                if (e.Cancelled)
                {
                    OffsetFound.Text = "You've cancelled Flyworker!";
                }
                else
                {
                    OffsetFound.Text = Temp + Environment.NewLine;
                    OffsetFound.ForeColor = Color.DodgerBlue;
                    // OffsetFound.Text = ((int)e.UserState).ToString();//casts the userstate into integer and adds it to a List
                }
            }
        }
        #endregion

        #region ><><> Bg Worker2 <><><
        private void FlyWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            ReConnectAndAttach(); BackgroundWorker Fly = sender as BackgroundWorker;
            uint offsetstart = Convert.ToUInt32(OffsetBox.Text.Replace("0x", ""), 16);
            uint Lowoffset = Convert.ToUInt32(LowBox.Text.Replace("0x", ""), 16);
            uint Highoffset = Convert.ToUInt32(HighBox.Text.Replace("0x", ""), 16);

            for (int i = 0; i <= 1; i++)
            {
                Fly.ReportProgress(i);
                Thread.Sleep(500);//1000
                if (FlyWorker2.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
            }

            if (Api == 1)
            {
                uint SearchBytez = FindAddress(SearchBytes.Text, offsetstart, 0U, Lowoffset, 4U, Highoffset);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = (uint)SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            if (Api == 2)
            {
                uint SearchBytez = FindAddressCcapi(SearchBytes.Text, offsetstart, 0U, Lowoffset, 4U, Highoffset);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = (uint)SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            if (Api == 3)
            {
                uint SearchBytez = FindAddress(SearchBytes.Text, offsetstart, 0U, Lowoffset, 4U, Highoffset);
                if (SearchBytez == ZeroOffset)
                {
                    OffsetFounded = 0x0;
                    Addresses.Add("Search Again");
                    OffsetFound.ForeColor = Color.Red;
                }
                else
                {
                    uint realoffset = (uint)SearchBytez;
                    OffsetFounded = realoffset;
                    Addresses.Add("Found: " + string.Format("0x{0:X}", OffsetFounded));
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }

            for (int i = 95; i <= 100; i++)
            {
                Fly.ReportProgress(i);
                Thread.Sleep(500);
            }
        }

        private void FlyWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 0)
            {
                ProgressBar.Value = e.ProgressPercentage;
                OffsetFound.Text = "Searching String.." + ProgressBar.Value.ToString() + "%";
                OffsetFound.ForeColor = Color.DarkRed;
            }
            else
            { }
        }
        private void FlyWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (String Temp in Addresses)
            {
                if (e.Cancelled)
                {
                    OffsetFound.Text = "You've cancelled Flyworker2!";
                }
                else
                {
                    OffsetFound.Text = Temp + "\n";
                    OffsetFound.ForeColor = Color.Chartreuse;
                }
            }
        }
        #endregion
        #endregion

        #region ><><> Searches <><><
        private void Cancel_Click(object sender, EventArgs e)
        {
            if (!Juststring.Checked)
            {
                if (FlyWorker.WorkerSupportsCancellation == true)
                {
                    FlyWorker.CancelAsync();
                    ProgressBar.Visible = false;
                }
            }
            else if (Juststring.Checked)
            {
                if (FlyWorker2.WorkerSupportsCancellation == true)
                {
                    FlyWorker2.CancelAsync();
                    ProgressBar.Visible = false;
                }
            }
        }
        private void Search_Click(object sender, EventArgs e)
        {
            if (TOCBox.Checked)
            {
                GetToc();
            }
            else
            {
                if (Juststring.Checked)
                {
                    if (FlyWorker2.IsBusy != true)
                    {
                        ProgressBar.Visible = true;
                        FlyWorker2.RunWorkerAsync();
                    }
                }
                if (!Juststring.Checked)
                {
                    if (FlyWorker.IsBusy != true)
                    {
                        ProgressBar.Visible = true;
                        FlyWorker.RunWorkerAsync();
                    }
                }                           
            }       
        }       
        private void ProcsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Modules
            lV_Modules.Items.Clear();
            foreach (int prx_id in _ps3mapi.Process.Modules.GetPrxIdModules(_ps3mapi.Process.Processes_Pid[ProcsBox.SelectedIndex]))
            {
                if (prx_id != 0)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = _ps3mapi.Process.Modules.GetName(_ps3mapi.Process.Processes_Pid[ProcsBox.SelectedIndex], prx_id);
                    lvi.SubItems.Add(_ps3mapi.Process.Modules.GetFilename(_ps3mapi.Process.Processes_Pid[ProcsBox.SelectedIndex], prx_id));
                    lV_Modules.Items.Add(lvi);
                }
                else break;
            }
        }
        //WEBMAN
        private void GamesList_Click(object sender, EventArgs e)
        {
           Process.Start("http://" + CEXIPBox.Text + "/index.ps3");
        }       
        private void btn_FileManager_Click(object sender, EventArgs e)
        {
            Process.Start("http://" + CEXIPBox.Text);
        }
        #endregion

        #region  ><><> Form Helpers
        private void Add1k_Click(object sender, EventArgs e)
        {
            if (Juststring.Checked)
            {
                MessageBox.Show("Not Used.");
            }
            if (!Juststring.Checked)
            {
                MessageBox.Show("Bool 0 False: or 1 True: will minus 10000 to found Address.");
            }
        }
        private void StringofBytes_Click(object sender, EventArgs e)
        {
            if (Juststring.Checked)
            {
                MessageBox.Show("String of Bytes is the unique bytes found for code.");
            }
            if (!Juststring.Checked)
            {
                MessageBox.Show("String of Bytes is the unique bytes found before @H and @L.");
            }          
        }
        private void OffsetStart_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Area of unique bytes starting point.");
        }
        private void OffsetLowDiff_Click(object sender, EventArgs e)
        {
            if (Juststring.Checked)
            {
                MessageBox.Show("Max Length of search for string.");
            }
            if (!Juststring.Checked)
            {
                MessageBox.Show("Length from String Address to @L pointer");
            }
        }
        private void OffsetHighDiff_Click(object sender, EventArgs e)
        {
            if (Juststring.Checked)
            {
                MessageBox.Show("Length of search size.");
            }
            if (!Juststring.Checked)
            {
                MessageBox.Show("Length from String Address to @H pointer.");
            }
        }
        #endregion

        #region ><>< Helpers
        public void NewThread(Action task)
		{
			new Thread((() => task()))
			{
				Name = task.Method.Name
			}

			.Start();
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
        private string StringFix(string h)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= h.Length - 2; i += 2)
            {
              sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(h.Substring(i, 2), NumberStyles.HexNumber))));
            }
            sb.Replace((char)0x00, ' ');
           return sb.ToString();
        }
        private static char ToHexDigit(int i)
        {
            return (char)(i < 10 ? i + 48 : i + 55);
        }
        public static string ToHexString(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2 + 2];
            chars[0] = '0';
            chars[1] = 'x';
            for (int i = 0; i < bytes.Length; i++)
            {
                chars[2 * i + 2] = ToHexDigit(bytes[i] / 16);
                chars[2 * i + 3] = ToHexDigit(bytes[i] % 16);
            }
          return new string(chars);
        }
        public static byte[] strToArray(string hex)
        {
            string str = hex.Replace("0x", "").Replace(" ", "");
            if (str.Length % 2 == 1)
                throw new Exception("Binary cannot have an odd number of digits");
            byte[] numArray = new byte[str.Length / 2];
            string[] strArray = hex.Split(new char[1]
            {
                ' '
            });
            int index = 0;
            foreach (string s in strArray)
            {
                numArray[index] = (byte)int.Parse(s, NumberStyles.HexNumber);
                ++index;
            }
            return numArray;
        }
        public void GetToc()
        {
            uint offsetstart = Convert.ToUInt32(OffsetBox.Text.Replace("0x", ""), 16);
            uint Lowoffset = Convert.ToUInt32(LowBox.Text.Replace("0x", ""), 16);

            byte[] array = StringBAToBA(SearchBytes.Text);
            Array.Resize(ref array, array.Length);

            byte[] numArray = new byte[0x1000000];
            _tmapi.GetMemory(offsetstart, numArray);

            OffsetFound.Text = "TOC = 0x" + Convert.ToString(SearchFor(array, offsetstart, (int)Lowoffset, 4), 16).ToUpper();
        }
        public static void setHookAddress(uint Address)
        {
            HookFuncAddr = Address;
            _tmapi.WriteUInt32(Start + 0x304, Address);
            sendCommand("Init Hook");
            Thread.Sleep(100);
        }
        private static void setAddress(uint val)
        {
            _tmapi.WriteUInt32(Start + 0x308, val);
        }
        private static void sendCommand(string Command)
        {
            _tmapi.WriteString(Start - 0x300, Command);
        }
        private int CF_Hook()
        {
            return 0; //CallFunctions();
        }
        private void PatchInJump(int Address, int Destination, bool Linked)
        { // use this data to copy over the address
            int[] FuncBytes = new int[4];

            // Get the actual destination address
            Destination = (int)Destination;

            FuncBytes[0] = 0x3D600000 + ((Destination >> 16) & 0xFFFF); // lis  %r11, dest>>16
            if (Destination == 0x8000) FuncBytes[0] += 1; // If bit 16 is 1
            FuncBytes[1] = 0x396B0000 + (Destination & 0xFFFF); // addi %r11, %r11, dest&0xFFFF
            FuncBytes[2] = 0x7D6903A6; // mtctr %r11
            FuncBytes[3] = 0x4E800420; // bctr
            if (Linked)
                FuncBytes[3] += 1; // bctrl
            _tmapi.SetMemory((uint)Address, (uint)(object)FuncBytes, 4 * 4);
        }
        public uint CombineHL(uint startOffset, uint High, uint Low, bool take1 = false)
        { // this will Return the Offset by Reading the High and low Offsets to get the XREF its pointing to...
            uint HighOffset = 2 + startOffset + High;
            uint LowOffset = 2 + startOffset + Low;
            byte[] Highval1 = new byte[2];
            byte[] Lowval1 = new byte[2];
            if (Api == 1)
            {
                _tmapi.GetMemory(HighOffset, Highval1);
                _tmapi.GetMemory(LowOffset, Lowval1);
            }
            if (Api == 2)
            {
                _ccapi.GetMemory(HighOffset, Highval1);
                _ccapi.GetMemory(LowOffset, Lowval1);
            }
            if (Api == 3)
            {
                _ps3mapi.Process.Memory.Get(_ps3mapi.Process.Process_Pid, HighOffset, Highval1);
                _ps3mapi.Process.Memory.Get(_ps3mapi.Process.Process_Pid, LowOffset, Lowval1);
            }
            if (take1)
            {
                Highval1[1] -= 0x1;
            }
           string HiLow = Highval1[0].ToString("X") + Highval1[1].ToString("X") + Lowval1[0].ToString("X") + Lowval1[1].ToString("X");
         return (uint)int.Parse(HiLow, NumberStyles.HexNumber);
        }
        public uint FindAddress(string UniqueBytes, uint startOffset = 268435456U, uint difference = 0U, uint maxoffset = 2566914048U, uint skip = 4U, uint size = 65536U)
        {
            byte[] numArray1 = StringBAToBA(UniqueBytes);
            int length1 = UniqueBytes.Replace(" ", "").Length / 2;
            byte[] numArray2 = new byte[length1];
            uint num1 = 0U;
            while (num1 < maxoffset - startOffset)
            {
                byte[] length2 = new byte[(int)(IntPtr)size];
                if (Api == 1)
                {
                   _tmapi.GetMemory(startOffset + num1, length2);
                }
                if (Api == 2)
                {
                    _ccapi.GetMemory(startOffset + num1, length2);
                }
                if (Api == 3)
                {
                    _ps3mapi.Process.Memory.Get(_ps3mapi.Process.Process_Pid, startOffset + num1, length2);
                }
                uint num2 = 0U;
                while (num2 < size - 4U)
                {
                    int num3 = length1;
                    for (int index = 0; index < length1; ++index)
                    {
                        if (length2[(int)(IntPtr)(checked((long)(ulong)unchecked((long)num2 + (long)index)))] != numArray1[index])
                        {
                            --num3;
                            break;
                        }
                    }
                    if (num3 == length1)
                        return startOffset + num1 + num2 + difference;
                    num2 += skip;
                }
                num1 += size;
            }
            return 69U;
        }
        public uint FindAddressCcapi(string UniqueBytes, uint startOffset = 268435456U, uint difference = 0U, uint maxoffset = 2566914048U, uint skip = 4U, uint size = 65536U)
        {
            byte[] numArray1 = StringBAToBA(UniqueBytes);
            int length1 = UniqueBytes.Replace(" ", "").Length / 2;
            byte[] numArray2 = new byte[length1];
            uint num1 = 0U;
            while (num1 < maxoffset - startOffset)
            {
                byte[] length2 = new byte[(int)(IntPtr)size];
                _ccapi.GetMemory(startOffset + num1, length2);
                uint num2 = 0U;
                while (num2 < size - 4U)
                {
                    int num3 = length1;
                    for (int index = 0; index < length1; ++index)
                    {
                        if (length2[(int)(IntPtr)(checked((long)(ulong)unchecked((long)num2 + (long)index)))] != numArray1[index])
                        {
                            --num3;
                            break;
                        }
                    }
                    if (num3 == length1)
                        return startOffset + num1 + num2 + difference;
                    num2 += skip;
                }
                num1 += size;
            }
            return 69U;
        }
        public uint ContainsSequence(byte[] toSearch, byte[] toFind, uint StartOffset)
        {
            int num = 0;
            while (num + toFind.Length < toSearch.Length)
            {
                bool flag = true;
                for (int index = 0; index <= toFind.Length - 1; index++)
                {
                    if (Convert.ToInt32(toSearch[num + index]) != Convert.ToInt32(toFind[index]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return StartOffset + Convert.ToUInt32(num);
                }
                num += 4;
            }
            return 0u;
        }
        private uint ContainsSequence2(byte[] toSearch, byte[] toFind, uint StartOffset, int bytes)
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
        private uint SearchFor(byte[] toSearch, uint Start, int Length, int bytes)
        {
            uint num1 = ContainsSequence2(_tmapi.ReadBytes(Start, Length), toSearch, Start, bytes);
            if (num1.Equals(ZeroOffset))
                return 0U;
            int num2 = 0;
            foreach (int num3 in toSearch)
            {
                if (num3 == 1)
                    ++num2;
            }
            return num1 + (uint)num2;
        }
        private static int ScanBytes(byte[] bytesToScan, byte[] BytePattern, uint startOffset = 268435456U, uint MaxSearchLength = 2415919103U, uint skip = 4U)
        {
            int num = -1;
            if ((int)MaxSearchLength == int.MaxValue)
                MaxSearchLength = (uint)bytesToScan.Length;
            for (int index1 = (int)startOffset;
                (long)index1 < (long)(MaxSearchLength - startOffset) - (long)BytePattern.Length - 1L;
               ++index1)
            {
                if (bytesToScan[index1] == BytePattern[0])
                {
                    int index2 = 1;
                    while (index2 < BytePattern.Length && bytesToScan[index1 + index2] == BytePattern[index2])
                    {
                        ++index2;
                        if (bytesToScan[index1 + index2] == BytePattern[index2] && index2 == BytePattern.Length - 1)
                      return (int)(startOffset + index1);
                    }
                }
            }
          return num;
        }
        #endregion
        #endregion
    }
}