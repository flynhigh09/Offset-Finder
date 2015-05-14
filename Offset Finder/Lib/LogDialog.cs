using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Offset_Finder
{
	public partial class LogDialog : Form
	{
        PS3MAPI PS3MAPI = null;
        public LogDialog()
		{
			InitializeComponent();
		}

        public LogDialog(PS3MAPI MyPS3MAPI)
            : this()
        {
            PS3MAPI = MyPS3MAPI;
        }

        private void LogDialog_Refresh(object sender, EventArgs e)
        {
            if (PS3MAPI != null) tB_Log.Text = PS3MAPI.Log;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
	}
}
