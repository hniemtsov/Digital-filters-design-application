namespace DFDLab.DFDTools
{
    partial class SOSRealization
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SOSRealization));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pBtSBCodeGen = new System.Windows.Forms.TextBox();
            this.pBtSBTF2SOS = new System.Windows.Forms.TextBox();
            this.pBtSBSpecification = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tSBSpecification = new System.Windows.Forms.ToolStripButton();
            this.tSBTF2SOS = new System.Windows.Forms.ToolStripButton();
            this.tSBCodeGen = new System.Windows.Forms.ToolStripButton();
            this.Stop = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBtSBSpecification)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.toolStrip1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(200, 185);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Realization";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pBtSBCodeGen);
            this.groupBox2.Controls.Add(this.pBtSBTF2SOS);
            this.groupBox2.Controls.Add(this.pBtSBSpecification);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(4, 44);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(192, 137);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "SOS Specification";
            // 
            // pBtSBCodeGen
            // 
            this.pBtSBCodeGen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBtSBCodeGen.Location = new System.Drawing.Point(4, 19);
            this.pBtSBCodeGen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pBtSBCodeGen.Multiline = true;
            this.pBtSBCodeGen.Name = "pBtSBCodeGen";
            this.pBtSBCodeGen.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.pBtSBCodeGen.Size = new System.Drawing.Size(184, 114);
            this.pBtSBCodeGen.TabIndex = 2;
            this.pBtSBCodeGen.Visible = false;
            this.pBtSBCodeGen.WordWrap = false;
            // 
            // pBtSBTF2SOS
            // 
            this.pBtSBTF2SOS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBtSBTF2SOS.Location = new System.Drawing.Point(4, 19);
            this.pBtSBTF2SOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pBtSBTF2SOS.Multiline = true;
            this.pBtSBTF2SOS.Name = "pBtSBTF2SOS";
            this.pBtSBTF2SOS.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.pBtSBTF2SOS.Size = new System.Drawing.Size(184, 114);
            this.pBtSBTF2SOS.TabIndex = 1;
            this.pBtSBTF2SOS.Visible = false;
            this.pBtSBTF2SOS.WordWrap = false;
            // 
            // pBtSBSpecification
            // 
            this.pBtSBSpecification.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBtSBSpecification.Location = new System.Drawing.Point(4, 19);
            this.pBtSBSpecification.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pBtSBSpecification.Name = "pBtSBSpecification";
            this.pBtSBSpecification.Size = new System.Drawing.Size(184, 114);
            this.pBtSBSpecification.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBtSBSpecification.TabIndex = 0;
            this.pBtSBSpecification.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSBSpecification,
            this.tSBTF2SOS,
            this.tSBCodeGen,
            this.Stop});
            this.toolStrip1.Location = new System.Drawing.Point(4, 19);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(192, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tSBSpecification
            // 
            this.tSBSpecification.Checked = true;
            this.tSBSpecification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tSBSpecification.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSBSpecification.Image = ((System.Drawing.Image)(resources.GetObject("tSBSpecification.Image")));
            this.tSBSpecification.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSBSpecification.Name = "tSBSpecification";
            this.tSBSpecification.Size = new System.Drawing.Size(23, 22);
            this.tSBSpecification.Text = "SOS Specification";
            this.tSBSpecification.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tSBTF2SOS
            // 
            this.tSBTF2SOS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSBTF2SOS.Image = ((System.Drawing.Image)(resources.GetObject("tSBTF2SOS.Image")));
            this.tSBTF2SOS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSBTF2SOS.Name = "tSBTF2SOS";
            this.tSBTF2SOS.Size = new System.Drawing.Size(23, 22);
            this.tSBTF2SOS.Text = "Coefficients";
            this.tSBTF2SOS.Click += new System.EventHandler(this.toolStripButton23_Click);
            // 
            // tSBCodeGen
            // 
            this.tSBCodeGen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSBCodeGen.Image = ((System.Drawing.Image)(resources.GetObject("tSBCodeGen.Image")));
            this.tSBCodeGen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSBCodeGen.Name = "tSBCodeGen";
            this.tSBCodeGen.Size = new System.Drawing.Size(23, 22);
            this.tSBCodeGen.Text = "Generate \"C\" code";
            this.tSBCodeGen.Click += new System.EventHandler(this.toolStripButton23_Click);
            // 
            // Stop
            // 
            this.Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Stop.Image = ((System.Drawing.Image)(resources.GetObject("Stop.Image")));
            this.Stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(23, 22);
            this.Stop.Text = "toolStripButton4";
            this.Stop.Visible = false;
            // 
            // SOSRealization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SOSRealization";
            this.Size = new System.Drawing.Size(200, 185);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBtSBSpecification)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tSBSpecification;
        private System.Windows.Forms.ToolStripButton tSBTF2SOS;
        private System.Windows.Forms.PictureBox pBtSBSpecification;
        private System.Windows.Forms.TextBox pBtSBTF2SOS;
        private System.Windows.Forms.ToolStripButton tSBCodeGen;
        private System.Windows.Forms.ToolStripButton Stop;
        private System.Windows.Forms.TextBox pBtSBCodeGen;

    }
}
