using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DFDLab.DFDTools;
using System.Diagnostics;

namespace DFDLab
{
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {

        }
        private void ChangeRightPanel(Type tp)
        {
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            Control.ControlCollection ctrl = tmp.SCPage.Panel2.Controls;
            foreach (Control c in ctrl)
            {
                if (c.GetType() == tp)
                {
                    UpdateRightPanel(tp, tmp.Text);
                }
            }
        }
        private void UpdateRightPanel(Type tp, string tabName)
        {
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            /*
            for (int i = 0; i < tabControl1.TabPages.Count; i++)//May be exist another better method to
            {                                                   // find tab page?
                if (tabControl1.Controls[i].Text.Equals(tabName))
                {
                    tmp = (TabFilterPage)tabControl1.Controls[i];
                    break;
                }
            }
            */
            Control.ControlCollection ctrl = tmp.SCPage.Panel2.Controls;
            //ctrl.ContainsKey();

            foreach (Control c in ctrl)
            {
                if (c.GetType() == tp)
                {
                    c.Visible = true;

                    tSPEnabled[tabName].EnabledBtn = filters_form[tabName].CheckValidParam();
                    tSPEnabled[tabName].EStop = false;
                }
                else
                {
                    c.Visible = false;
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            TabFilterPage tbPage = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            Control ctrl = tbPage.SCPage.Panel1.Controls[0];

            ToolStripButton tmp = sender as ToolStripButton;
            tSPEnabled[tabControl1.SelectedTab.Text].TSBtnCurr = tmp;
            ChangeLeftPanel(ctrl.GetType());
            ChangeRightPanel(typeof(FilterProperty));
            
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            tSPEnabled[tabControl1.SelectedTab.Text].TSBtnCurr = sender as ToolStripButton;

            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);

            if (!tmp.SCPage.Panel2.Controls.ContainsKey("SOSRealization"))
            {
                SOSRealization SosRealization = new SOSRealization(filters_form[tmp.Name]);
                SoSPanel SosPanel = new SoSPanel(SosRealization, filters_form[tmp.Name]);
                filters_form[tmp.Name].SetValidate += new AbstractFilter.VailidateHadler(SosRealization.SetEnableBtn);
                tmp.SCPage.Panel2.Controls.Add(SosRealization);
                tmp.SCPage.Panel1.Controls.Add(SosPanel);
            }
            ChangeLeftPanel(typeof(SoSPanel));
            ChangeRightPanel(typeof(SOSRealization));
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            tSPEnabled[tabControl1.SelectedTab.Text].TSBtnCurr = sender as ToolStripButton; 
            
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            
            if (!tmp.SCPage.Panel2.Controls.ContainsKey("SignalProperty"))
            {
                SignalProperty sigProp = new SignalProperty(filters_form[tmp.Name]);
                SignalParameters sigParam = new SignalParameters(sigProp, filters_form[tmp.Name]);
               
                filters_form[tmp.Name].SetValidate += new AbstractFilter.VailidateHadler(sigProp.SetEnableBtn);
                filters_form[tmp.Name].AddRow = new AbstractFilter.CmbxVal(sigParam.AddSign2Cmbx);
                filters_form[tmp.Name].RemoveRow = new AbstractFilter.CmbxVal(sigParam.RemoveSign2Cmbx);
                filters_form[tmp.Name].SetBorderAll = new AbstractFilter.ParamChanged(sigProp.SetAllBorder);
                
                tmp.SCPage.Panel2.Controls.Add(sigProp);
                tmp.SCPage.Panel1.Controls.Add(sigParam);
            }
            ChangeLeftPanel(typeof(SignalParameters));
            ChangeRightPanel(typeof(SignalProperty));
        }


        private void ChangeLeftPanel(Type tp)
        {
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            Control.ControlCollection ctrl = tmp.SCPage.Panel1.Controls;
            foreach (Control c in ctrl)
            {
                if (c.GetType() == tp)
                {
                    c.Visible = true;
                }
                else
                {
                    c.Visible = false;
                }
                
            }
        }

          private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process SysInfo = new Process();
                SysInfo.StartInfo.ErrorDialog = true;
                SysInfo.StartInfo.FileName = "hh.exe";
                SysInfo.StartInfo.Arguments = "DFDLabHelp.chm";
                SysInfo.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDFDLab about = new AboutDFDLab();
            about.ShowDialog();
        }

    }
}
