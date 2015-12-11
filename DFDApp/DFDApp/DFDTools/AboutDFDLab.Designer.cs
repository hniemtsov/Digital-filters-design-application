namespace DFDLab.DFDTools
{
    public partial class AboutDFDLab
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDFDLab));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.CNetHelpProvider = new System.Windows.Forms.HelpProvider();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.03837F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.96163F));
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.tableLayoutPanel, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_tableLayoutPanel");
            this.CNetHelpProvider.SetHelpNavigator(this.tableLayoutPanel, System.Windows.Forms.HelpNavigator.Topic);
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.92308F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.89744F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.48718F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.46154F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.128205F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.10256F));
            this.CNetHelpProvider.SetShowHelp(this.tableLayoutPanel, true);
            this.tableLayoutPanel.Size = new System.Drawing.Size(417, 195);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.logoPictureBox, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_logoPictureBox");
            this.CNetHelpProvider.SetHelpNavigator(this.logoPictureBox, System.Windows.Forms.HelpNavigator.Topic);
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.CNetHelpProvider.SetShowHelp(this.logoPictureBox, true);
            this.logoPictureBox.Size = new System.Drawing.Size(210, 189);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.labelProductName, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_labelProductName");
            this.CNetHelpProvider.SetHelpNavigator(this.labelProductName, System.Windows.Forms.HelpNavigator.Topic);
            this.labelProductName.Location = new System.Drawing.Point(222, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelProductName.Name = "labelProductName";
            this.CNetHelpProvider.SetShowHelp(this.labelProductName, true);
            this.labelProductName.Size = new System.Drawing.Size(192, 17);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.labelVersion, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_labelVersion");
            this.CNetHelpProvider.SetHelpNavigator(this.labelVersion, System.Windows.Forms.HelpNavigator.Topic);
            this.labelVersion.Location = new System.Drawing.Point(222, 33);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelVersion.Name = "labelVersion";
            this.CNetHelpProvider.SetShowHelp(this.labelVersion, true);
            this.labelVersion.Size = new System.Drawing.Size(192, 17);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.labelCopyright, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_labelCopyright");
            this.CNetHelpProvider.SetHelpNavigator(this.labelCopyright, System.Windows.Forms.HelpNavigator.Topic);
            this.labelCopyright.Location = new System.Drawing.Point(222, 64);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCopyright.Name = "labelCopyright";
            this.CNetHelpProvider.SetShowHelp(this.labelCopyright, true);
            this.labelCopyright.Size = new System.Drawing.Size(192, 17);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.labelCompanyName, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_labelCompanyName");
            this.CNetHelpProvider.SetHelpNavigator(this.labelCompanyName, System.Windows.Forms.HelpNavigator.Topic);
            this.labelCompanyName.Location = new System.Drawing.Point(222, 101);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCompanyName.Name = "labelCompanyName";
            this.CNetHelpProvider.SetShowHelp(this.labelCompanyName, true);
            this.labelCompanyName.Size = new System.Drawing.Size(192, 17);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "Company Name";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CNetHelpProvider.SetHelpKeyword(this.okButton, "DFDTools_Form_AboutDFDLab.htm#AboutDFDLab_okButton");
            this.CNetHelpProvider.SetHelpNavigator(this.okButton, System.Windows.Forms.HelpNavigator.Topic);
            this.okButton.Location = new System.Drawing.Point(339, 169);
            this.okButton.Name = "okButton";
            this.CNetHelpProvider.SetShowHelp(this.okButton, true);
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // CNetHelpProvider
            // 
            this.CNetHelpProvider.HelpNamespace = "DFDTools.chm";
            // 
            // AboutDFDLab
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 213);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.CNetHelpProvider.SetHelpKeyword(this, "DFDTools_Form_AboutDFDLab.htm");
            this.CNetHelpProvider.SetHelpNavigator(this, System.Windows.Forms.HelpNavigator.Topic);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDFDLab";
            this.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.CNetHelpProvider.SetShowHelp(this, true);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About  DFDLab";
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.HelpProvider CNetHelpProvider;
    }
}
