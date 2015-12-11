using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFDLab.DFDTools
{
    public partial class DFDWizard : Form
    {
        public delegate void ChooseMethodEvent(string method);
        public event ChooseMethodEvent GetMethod;
        public DFDWizard(Point Location)
        {
            InitializeComponent();

            this.filterSelector1 = new DFDTools.FilterSelector();
            this.methodSelector1 = new DFDTools.MethodSelector();


            this.SCMain.Panel1.Controls.Add(this.filterSelector1);
            this.SCMain.Panel2.Controls.Add(this.methodSelector1);
            // 
            // filterSelector1
            // 
            this.filterSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CNetHelpProvider.SetHelpKeyword(this.filterSelector1, "DFDTools_Form_DFDWizard.htm#DFDWizard_FilterSelector");
            this.CNetHelpProvider.SetHelpNavigator(this.filterSelector1, System.Windows.Forms.HelpNavigator.Topic);
            this.filterSelector1.Location = new System.Drawing.Point(0, 0);
            this.filterSelector1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.filterSelector1.Name = "filterSelector1";
            this.CNetHelpProvider.SetShowHelp(this.filterSelector1, true);
            this.filterSelector1.Size = new System.Drawing.Size(236, 425);
            this.filterSelector1.TabIndex = 0;
            // 
            // methodSelector1
            // 
            this.methodSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodSelector1.FilterName = null;
            this.CNetHelpProvider.SetHelpKeyword(this.methodSelector1, "DFDTools_Form_DFDWizard.htm#DFDWizard_MethodSelector");
            this.CNetHelpProvider.SetHelpNavigator(this.methodSelector1, System.Windows.Forms.HelpNavigator.Topic);
            this.methodSelector1.Location = new System.Drawing.Point(0, 0);
            this.methodSelector1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.methodSelector1.Name = "methodSelector1";
            this.CNetHelpProvider.SetShowHelp(this.methodSelector1, true);
            this.methodSelector1.Size = new System.Drawing.Size(467, 425);
            this.methodSelector1.TabIndex = 0;

            filterSelector1.FillList += new EventHandler(methodSelector1.filterSelector1_FillList);
            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
            AcceptButton = button1;
            CancelButton = button2;
            methodSelector1.IsSelected += new MethodSelector.ButtonOKEventHandler(methodSelector1_IsSelected);

            this.Location = Location + SystemInformation.CaptionButtonSize +
                            SystemInformation.FrameBorderSize; 

        }

        void methodSelector1_IsSelected(bool IsEnable,string text)
        {
            button1.Enabled = IsEnable;
            textBox1.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GetMethod != null)
            {
                GetMethod(methodSelector1.GetSelectedMethod());
            }
        }

    }
}
