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
    public partial class AttachDialog : Form
	{
        public AttachDialog()
		{
			InitializeComponent();
		}

        public AttachDialog(PS3MAPI MyPS3MAPI)
            : this()
        {
            comboBox1.Items.Clear();
            foreach (uint pid in MyPS3MAPI.Process.GetPidProcesses())
            {
                if (pid != 0) comboBox1.Items.Add(MyPS3MAPI.Process.GetName(pid));
                else break;
            }
            comboBox1.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
            return;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }
    }
}
