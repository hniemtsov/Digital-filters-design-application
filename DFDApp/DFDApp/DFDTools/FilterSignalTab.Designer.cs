namespace DFDLab.DFDTools
{
    partial class FilterSignalTab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterSignalTab));
            this.SuspendLayout();
            // 
            // FilterSignalTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(450, 150);
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Name = "FilterSignalTab";
            this.Size = new System.Drawing.Size(450, 150);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FilterSignalTab_Scroll);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FilterSignalTab_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
