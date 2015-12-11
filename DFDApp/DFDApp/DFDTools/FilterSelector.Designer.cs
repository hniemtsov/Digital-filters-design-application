namespace DFDLab.DFDTools
{
    partial class FilterSelector
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Least Mean Squares");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Recursive Least Squares");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Affine Projection");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Frequency Domain");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Lattice");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Adaptive Filters", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Optimal Filters");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("IIR");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("FIR");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Using Optimization", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Using Windows");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Using FFT");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("From Analog");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(341, 450);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select design technics";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(4, 20);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node5";
            treeNode1.Text = "Least Mean Squares";
            treeNode2.Name = "Node9";
            treeNode2.Text = "Recursive Least Squares";
            treeNode3.Name = "Node1";
            treeNode3.Text = "Affine Projection";
            treeNode4.Name = "Node2";
            treeNode4.Text = "Frequency Domain";
            treeNode5.Name = "Node3";
            treeNode5.Text = "Lattice";
            treeNode6.Name = "Node4";
            treeNode6.Text = "Adaptive Filters";
            treeNode7.Name = "Node0";
            treeNode7.Text = "Optimal Filters";
            treeNode8.Name = "Node0";
            treeNode8.Text = "IIR";
            treeNode9.Name = "Node1";
            treeNode9.Text = "FIR";
            treeNode10.Name = "Node7";
            treeNode10.Text = "Using Optimization";
            treeNode11.Name = "Node6";
            treeNode11.Text = "Using Windows";
            treeNode12.Name = "Node0";
            treeNode12.Text = "Using FFT";
            treeNode13.Name = "Node2";
            treeNode13.Text = "From Analog";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
            this.treeView1.Size = new System.Drawing.Size(332, 426);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // FilterSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FilterSelector";
            this.Size = new System.Drawing.Size(341, 450);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeView1;

    }
}
