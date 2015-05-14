namespace Offset_Finder
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Connectiontoolstrip = new System.Windows.Forms.ToolStripDropDownButton();
            this.Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.User = new System.Windows.Forms.ToolStripStatusLabel();
            this.Connection = new System.Windows.Forms.ToolStripStatusLabel();
            this.IP = new System.Windows.Forms.ToolStripStatusLabel();
            this.Attached = new System.Windows.Forms.ToolStripStatusLabel();
            this.CEX = new System.Windows.Forms.RadioButton();
            this.DEX = new System.Windows.Forms.RadioButton();
            this.CEXIPBox = new System.Windows.Forms.TextBox();
            this.SearchBytes = new System.Windows.Forms.TextBox();
            this.LowBox = new System.Windows.Forms.TextBox();
            this.HighBox = new System.Windows.Forms.TextBox();
            this.OffsetBox = new System.Windows.Forms.TextBox();
            this.Search = new System.Windows.Forms.Button();
            this.StringofBytes = new System.Windows.Forms.Label();
            this.OffsetStart = new System.Windows.Forms.Label();
            this.OffsetLowDiff = new System.Windows.Forms.Label();
            this.OffsetHighDiff = new System.Windows.Forms.Label();
            this.BoolBox = new System.Windows.Forms.TextBox();
            this.Add1k = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.OffsetFound = new System.Windows.Forms.Label();
            this.FlyWorker = new System.ComponentModel.BackgroundWorker();
            this.Game = new System.Windows.Forms.Label();
            this.Juststring = new System.Windows.Forms.CheckBox();
            this.FlyWorker2 = new System.ComponentModel.BackgroundWorker();
            this.Cancel = new System.Windows.Forms.Button();
            this.Screentimer = new System.Windows.Forms.Timer(this.components);
            this.ProcsBox = new System.Windows.Forms.ListBox();
            this.Ps3Mapi = new System.Windows.Forms.RadioButton();
            this.GamesList = new System.Windows.Forms.Button();
            this.lV_Modules = new System.Windows.Forms.ListView();
            this.cH_Modules_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cH_Modules_Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TOCBox = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Black;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Connectiontoolstrip,
            this.User,
            this.Connection,
            this.IP,
            this.Attached});
            this.statusStrip1.Location = new System.Drawing.Point(1, -2);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
            this.statusStrip1.Size = new System.Drawing.Size(115, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Connectiontoolstrip
            // 
            this.Connectiontoolstrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Connectiontoolstrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Connect});
            this.Connectiontoolstrip.Image = ((System.Drawing.Image)(resources.GetObject("Connectiontoolstrip.Image")));
            this.Connectiontoolstrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Connectiontoolstrip.Name = "Connectiontoolstrip";
            this.Connectiontoolstrip.Size = new System.Drawing.Size(97, 24);
            this.Connectiontoolstrip.Text = "Connection";
            // 
            // Connect
            // 
            this.Connect.BackColor = System.Drawing.Color.Black;
            this.Connect.ForeColor = System.Drawing.Color.IndianRed;
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(132, 24);
            this.Connect.Text = "Connect";
            this.Connect.Click += new System.EventHandler(this.Connect_Click);
            // 
            // User
            // 
            this.User.Name = "User";
            this.User.Size = new System.Drawing.Size(0, 21);
            // 
            // Connection
            // 
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(0, 21);
            // 
            // IP
            // 
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(0, 21);
            // 
            // Attached
            // 
            this.Attached.Name = "Attached";
            this.Attached.Size = new System.Drawing.Size(0, 21);
            // 
            // CEX
            // 
            this.CEX.AutoSize = true;
            this.CEX.Location = new System.Drawing.Point(89, 53);
            this.CEX.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.CEX.Name = "CEX";
            this.CEX.Size = new System.Drawing.Size(80, 29);
            this.CEX.TabIndex = 1;
            this.CEX.Text = "Cappi";
            this.CEX.UseVisualStyleBackColor = true;
            this.CEX.CheckedChanged += new System.EventHandler(this.CEX_CheckedChanged);
            // 
            // DEX
            // 
            this.DEX.AutoSize = true;
            this.DEX.Location = new System.Drawing.Point(4, 54);
            this.DEX.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.DEX.Name = "DEX";
            this.DEX.Size = new System.Drawing.Size(85, 29);
            this.DEX.TabIndex = 2;
            this.DEX.Text = "TMapi";
            this.DEX.UseVisualStyleBackColor = true;
            this.DEX.CheckedChanged += new System.EventHandler(this.DEX_CheckedChanged);
            // 
            // CEXIPBox
            // 
            this.CEXIPBox.BackColor = System.Drawing.Color.Black;
            this.CEXIPBox.ForeColor = System.Drawing.Color.IndianRed;
            this.CEXIPBox.Location = new System.Drawing.Point(1, 22);
            this.CEXIPBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.CEXIPBox.Name = "CEXIPBox";
            this.CEXIPBox.Size = new System.Drawing.Size(125, 33);
            this.CEXIPBox.TabIndex = 3;
            this.CEXIPBox.Text = "192.168.1.";
            this.CEXIPBox.Visible = false;
            this.CEXIPBox.TextChanged += new System.EventHandler(this.CEXIPBox_TextChanged);
            // 
            // SearchBytes
            // 
            this.SearchBytes.Location = new System.Drawing.Point(139, 132);
            this.SearchBytes.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.SearchBytes.Name = "SearchBytes";
            this.SearchBytes.Size = new System.Drawing.Size(317, 33);
            this.SearchBytes.TabIndex = 4;
            // 
            // LowBox
            // 
            this.LowBox.Location = new System.Drawing.Point(184, 208);
            this.LowBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.LowBox.Name = "LowBox";
            this.LowBox.Size = new System.Drawing.Size(132, 33);
            this.LowBox.TabIndex = 5;
            // 
            // HighBox
            // 
            this.HighBox.Location = new System.Drawing.Point(184, 249);
            this.HighBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.HighBox.Name = "HighBox";
            this.HighBox.Size = new System.Drawing.Size(132, 33);
            this.HighBox.TabIndex = 6;
            // 
            // OffsetBox
            // 
            this.OffsetBox.Location = new System.Drawing.Point(184, 169);
            this.OffsetBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.OffsetBox.Name = "OffsetBox";
            this.OffsetBox.Size = new System.Drawing.Size(132, 33);
            this.OffsetBox.TabIndex = 7;
            // 
            // Search
            // 
            this.Search.BackColor = System.Drawing.Color.Black;
            this.Search.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Search.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.Search.FlatAppearance.MouseDownBackColor = System.Drawing.Color.IndianRed;
            this.Search.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Search.Font = new System.Drawing.Font("Segoe Script", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Search.Location = new System.Drawing.Point(339, 200);
            this.Search.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(111, 82);
            this.Search.TabIndex = 8;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = false;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // StringofBytes
            // 
            this.StringofBytes.AutoSize = true;
            this.StringofBytes.Font = new System.Drawing.Font("Segoe Script", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StringofBytes.Location = new System.Drawing.Point(10, 136);
            this.StringofBytes.Name = "StringofBytes";
            this.StringofBytes.Size = new System.Drawing.Size(126, 23);
            this.StringofBytes.TabIndex = 10;
            this.StringofBytes.Text = "String of Bytes";
            this.StringofBytes.Click += new System.EventHandler(this.StringofBytes_Click);
            // 
            // OffsetStart
            // 
            this.OffsetStart.AutoSize = true;
            this.OffsetStart.Location = new System.Drawing.Point(70, 172);
            this.OffsetStart.Name = "OffsetStart";
            this.OffsetStart.Size = new System.Drawing.Size(108, 25);
            this.OffsetStart.TabIndex = 11;
            this.OffsetStart.Text = "Offset Start";
            this.OffsetStart.Click += new System.EventHandler(this.OffsetStart_Click);
            // 
            // OffsetLowDiff
            // 
            this.OffsetLowDiff.AutoSize = true;
            this.OffsetLowDiff.Location = new System.Drawing.Point(28, 211);
            this.OffsetLowDiff.Name = "OffsetLowDiff";
            this.OffsetLowDiff.Size = new System.Drawing.Size(157, 25);
            this.OffsetLowDiff.TabIndex = 12;
            this.OffsetLowDiff.Text = "String @Low Diff";
            this.OffsetLowDiff.Click += new System.EventHandler(this.OffsetLowDiff_Click);
            // 
            // OffsetHighDiff
            // 
            this.OffsetHighDiff.AutoSize = true;
            this.OffsetHighDiff.Location = new System.Drawing.Point(22, 252);
            this.OffsetHighDiff.Name = "OffsetHighDiff";
            this.OffsetHighDiff.Size = new System.Drawing.Size(163, 25);
            this.OffsetHighDiff.TabIndex = 13;
            this.OffsetHighDiff.Text = "String @High Diff";
            this.OffsetHighDiff.Click += new System.EventHandler(this.OffsetHighDiff_Click);
            // 
            // BoolBox
            // 
            this.BoolBox.ForeColor = System.Drawing.Color.Black;
            this.BoolBox.Location = new System.Drawing.Point(139, 96);
            this.BoolBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.BoolBox.Name = "BoolBox";
            this.BoolBox.Size = new System.Drawing.Size(30, 33);
            this.BoolBox.TabIndex = 15;
            this.BoolBox.Text = "0";
            // 
            // Add1k
            // 
            this.Add1k.AutoSize = true;
            this.Add1k.Location = new System.Drawing.Point(31, 96);
            this.Add1k.Name = "Add1k";
            this.Add1k.Size = new System.Drawing.Size(105, 25);
            this.Add1k.TabIndex = 16;
            this.Add1k.Text = "Minus 10k";
            this.Add1k.Click += new System.EventHandler(this.Add1k_Click);
            // 
            // ProgressBar
            // 
            this.ProgressBar.BackColor = System.Drawing.Color.Black;
            this.ProgressBar.ForeColor = System.Drawing.Color.DodgerBlue;
            this.ProgressBar.Location = new System.Drawing.Point(12, 470);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(444, 13);
            this.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar.TabIndex = 17;
            this.ProgressBar.Visible = false;
            // 
            // OffsetFound
            // 
            this.OffsetFound.AutoSize = true;
            this.OffsetFound.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OffsetFound.Location = new System.Drawing.Point(228, 96);
            this.OffsetFound.Name = "OffsetFound";
            this.OffsetFound.Size = new System.Drawing.Size(117, 25);
            this.OffsetFound.TabIndex = 18;
            this.OffsetFound.Text = "Offset found";
            // 
            // FlyWorker
            // 
            this.FlyWorker.WorkerReportsProgress = true;
            this.FlyWorker.WorkerSupportsCancellation = true;
            this.FlyWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FlyWorker_DoWork);
            this.FlyWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.FlyWorker_ProgressChanged);
            this.FlyWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FlyWorker_RunWorkerCompleted);
            // 
            // Game
            // 
            this.Game.AutoSize = true;
            this.Game.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Game.Location = new System.Drawing.Point(132, 22);
            this.Game.Name = "Game";
            this.Game.Size = new System.Drawing.Size(0, 25);
            this.Game.TabIndex = 19;
            // 
            // Juststring
            // 
            this.Juststring.AutoSize = true;
            this.Juststring.Location = new System.Drawing.Point(314, 64);
            this.Juststring.Name = "Juststring";
            this.Juststring.Size = new System.Drawing.Size(145, 29);
            this.Juststring.TabIndex = 20;
            this.Juststring.Text = "String Search";
            this.Juststring.UseVisualStyleBackColor = true;
            this.Juststring.CheckedChanged += new System.EventHandler(this.Juststring_CheckedChanged);
            // 
            // FlyWorker2
            // 
            this.FlyWorker2.WorkerReportsProgress = true;
            this.FlyWorker2.WorkerSupportsCancellation = true;
            this.FlyWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FlyWorker2_DoWork);
            this.FlyWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.FlyWorker2_ProgressChanged);
            this.FlyWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FlyWorker2_RunWorkerCompleted);
            // 
            // Cancel
            // 
            this.Cancel.BackColor = System.Drawing.Color.Black;
            this.Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Cancel.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.Cancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.IndianRed;
            this.Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Cancel.Font = new System.Drawing.Font("Segoe Script", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel.Location = new System.Drawing.Point(351, 167);
            this.Cancel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(80, 30);
            this.Cancel.TabIndex = 21;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = false;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Screentimer
            // 
            this.Screentimer.Tick += new System.EventHandler(this.Screentimer_Tick);
            // 
            // ProcsBox
            // 
            this.ProcsBox.FormattingEnabled = true;
            this.ProcsBox.ItemHeight = 25;
            this.ProcsBox.Location = new System.Drawing.Point(12, 292);
            this.ProcsBox.Name = "ProcsBox";
            this.ProcsBox.Size = new System.Drawing.Size(444, 79);
            this.ProcsBox.TabIndex = 28;
            this.ProcsBox.SelectedIndexChanged += new System.EventHandler(this.ProcsBox_SelectedIndexChanged);
            // 
            // Ps3Mapi
            // 
            this.Ps3Mapi.AutoSize = true;
            this.Ps3Mapi.Location = new System.Drawing.Point(175, 54);
            this.Ps3Mapi.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.Ps3Mapi.Name = "Ps3Mapi";
            this.Ps3Mapi.Size = new System.Drawing.Size(104, 29);
            this.Ps3Mapi.TabIndex = 29;
            this.Ps3Mapi.Text = "Ps3Mapi";
            this.Ps3Mapi.UseVisualStyleBackColor = true;
            this.Ps3Mapi.CheckedChanged += new System.EventHandler(this.Ps3Mapi_CheckedChanged);
            // 
            // GamesList
            // 
            this.GamesList.BackColor = System.Drawing.Color.Black;
            this.GamesList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GamesList.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.GamesList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.IndianRed;
            this.GamesList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.GamesList.Font = new System.Drawing.Font("Segoe Script", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GamesList.Location = new System.Drawing.Point(380, 493);
            this.GamesList.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.GamesList.Name = "GamesList";
            this.GamesList.Size = new System.Drawing.Size(76, 34);
            this.GamesList.TabIndex = 30;
            this.GamesList.Text = "GamesList";
            this.GamesList.UseVisualStyleBackColor = false;
            this.GamesList.Click += new System.EventHandler(this.GamesList_Click);
            // 
            // lV_Modules
            // 
            this.lV_Modules.AutoArrange = false;
            this.lV_Modules.BackColor = System.Drawing.SystemColors.Control;
            this.lV_Modules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cH_Modules_Name,
            this.cH_Modules_Path});
            this.lV_Modules.FullRowSelect = true;
            this.lV_Modules.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lV_Modules.HideSelection = false;
            this.lV_Modules.Location = new System.Drawing.Point(12, 377);
            this.lV_Modules.MultiSelect = false;
            this.lV_Modules.Name = "lV_Modules";
            this.lV_Modules.ShowGroups = false;
            this.lV_Modules.Size = new System.Drawing.Size(444, 87);
            this.lV_Modules.TabIndex = 31;
            this.lV_Modules.UseCompatibleStateImageBehavior = false;
            this.lV_Modules.View = System.Windows.Forms.View.Details;
            // 
            // cH_Modules_Name
            // 
            this.cH_Modules_Name.Text = "Name";
            this.cH_Modules_Name.Width = 135;
            // 
            // cH_Modules_Path
            // 
            this.cH_Modules_Path.Text = "Path";
            this.cH_Modules_Path.Width = 275;
            // 
            // TOCBox
            // 
            this.TOCBox.AutoSize = true;
            this.TOCBox.Location = new System.Drawing.Point(382, 29);
            this.TOCBox.Name = "TOCBox";
            this.TOCBox.Size = new System.Drawing.Size(68, 29);
            this.TOCBox.TabIndex = 47;
            this.TOCBox.Text = "TOC";
            this.TOCBox.UseVisualStyleBackColor = true;
            this.TOCBox.CheckedChanged += new System.EventHandler(this.TOCBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(471, 552);
            this.Controls.Add(this.TOCBox);
            this.Controls.Add(this.lV_Modules);
            this.Controls.Add(this.GamesList);
            this.Controls.Add(this.Ps3Mapi);
            this.Controls.Add(this.ProcsBox);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Juststring);
            this.Controls.Add(this.Game);
            this.Controls.Add(this.OffsetFound);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.Add1k);
            this.Controls.Add(this.BoolBox);
            this.Controls.Add(this.OffsetHighDiff);
            this.Controls.Add(this.OffsetLowDiff);
            this.Controls.Add(this.OffsetStart);
            this.Controls.Add(this.StringofBytes);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.OffsetBox);
            this.Controls.Add(this.HighBox);
            this.Controls.Add(this.LowBox);
            this.Controls.Add(this.SearchBytes);
            this.Controls.Add(this.CEXIPBox);
            this.Controls.Add(this.DEX);
            this.Controls.Add(this.CEX);
            this.Controls.Add(this.statusStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Segoe Script", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Lime;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.Name = "Form1";
            this.Opacity = 0.95D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS3 Address Locator";
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton Connectiontoolstrip;
        private System.Windows.Forms.ToolStripMenuItem Connect;
        private System.Windows.Forms.ToolStripStatusLabel Connection;
        private System.Windows.Forms.ToolStripStatusLabel IP;
        private System.Windows.Forms.ToolStripStatusLabel Attached;
        private System.Windows.Forms.RadioButton CEX;
        private System.Windows.Forms.RadioButton DEX;
        private System.Windows.Forms.TextBox CEXIPBox;
        private System.Windows.Forms.TextBox SearchBytes;
        private System.Windows.Forms.TextBox LowBox;
        private System.Windows.Forms.TextBox HighBox;
        private System.Windows.Forms.TextBox OffsetBox;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Label StringofBytes;
        private System.Windows.Forms.Label OffsetStart;
        private System.Windows.Forms.Label OffsetLowDiff;
        private System.Windows.Forms.Label OffsetHighDiff;
        private System.Windows.Forms.TextBox BoolBox;
        private System.Windows.Forms.Label Add1k;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label OffsetFound;
        private System.ComponentModel.BackgroundWorker FlyWorker;
        private System.Windows.Forms.Label Game;
        private System.Windows.Forms.ToolStripStatusLabel User;
        private System.Windows.Forms.CheckBox Juststring;
        private System.ComponentModel.BackgroundWorker FlyWorker2;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Timer Screentimer;
        private System.Windows.Forms.ListBox ProcsBox;
        private System.Windows.Forms.RadioButton Ps3Mapi;
        private System.Windows.Forms.Button GamesList;
        private System.Windows.Forms.ListView lV_Modules;
        private System.Windows.Forms.ColumnHeader cH_Modules_Name;
        private System.Windows.Forms.ColumnHeader cH_Modules_Path;
        private System.Windows.Forms.CheckBox TOCBox;
    }
}

