using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DFDLab.DFDTools;
using System.Drawing;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace DFDLab
{
    public partial class MainForm : Form
    {
        public class tSPE
        {
            bool enabledBtn;
            bool enabledStop;
            ToolStripButton tsBtnCurr;
            ToolStripButton tsBtnPrev;

            public tSPE(ToolStripButton btn, bool flag)
            {
                tsBtnCurr = btn;
                enabledBtn = flag;
                tsBtnPrev = null;
                enabledStop = false;
            }
            public ToolStripButton TSBtnCurr
            {
                get { return tsBtnCurr; }
                set
                {
                    tsBtnCurr.Checked = false;
                    tsBtnPrev = tsBtnCurr;
                    tsBtnCurr = value;
                    tsBtnCurr.Checked = true;
                }
            }
            public void Change()
            {
                if (tsBtnPrev!=null) tsBtnPrev.Checked = true;
                if (tsBtnCurr!=null) tsBtnCurr.Checked = false;
                if (tsBtnPrev != null) tsBtnCurr = tsBtnPrev;
            }
            public bool EnabledBtn
            {
                get { return enabledBtn; }
                set { enabledBtn = value; }
            }
            
            public bool EStop
            {
                get { return enabledStop; }
                set { enabledStop = value; }
            }
        }

        const string strFilter = "Digital Filter files (*.xml)|" +
                                 "*.xml|All files (*.*)|*.*";
        
        private int IndexFilter = 0;
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = strFilter;
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(dlg.FileName);
                XmlSerializer xmlser = new XmlSerializer(filters_form[tmp.Text].GetType(),new Type []{typeof(FilterRealization), typeof(SOSection)});
                xmlser.Serialize(sw, (AbstractFilter)filters_form[tmp.Text]);
                sw.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = strFilter;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(dlg.FileName);

                XDocument xdc = XDocument.Load(dlg.FileName);
                string Name = xdc.Root.Name.ToString();
                AbstractFilter filter = null;
                FilterProperty FilterSpecific = null;
                UserControl InputParamForm = null;                
                switch (Name)
                {
                    case "Cheb1Filter" :
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(Cheb1Filter));
                            filter = xmlser.Deserialize(sr) as Cheb1Filter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new Cheby1Tab(FilterSpecific, filter);
                        }
                        break;
                    case "Cheb2Filter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(Cheb2Filter));
                            Cheb2Filter tmp = xmlser.Deserialize(sr) as Cheb2Filter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new Cheby2Tab(FilterSpecific, filter);
                        }
                        break;
                    case "EllipFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(EllipFilter));
                            EllipFilter tmp = xmlser.Deserialize(sr) as EllipFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new EllipTab(FilterSpecific, filter);
                        }
                        break;
                    case "ButterFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(ButterFilter));
                            ButterFilter tmp = xmlser.Deserialize(sr) as ButterFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new ButterTab(FilterSpecific, filter);
                        }
                        break;
                    case "FirLSFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(FirLSFilter));
                            FirLSFilter tmp = xmlser.Deserialize(sr) as FirLSFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new FirLSTab(FilterSpecific, filter);
                        }
                        break;
                    case "FirFSFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(FirFSFilter));
                            FirFSFilter tmp = xmlser.Deserialize(sr) as FirFSFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new FirFSTab(FilterSpecific, filter);
                        }
                        break;
                    case "IirLpFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(IirLpFilter));
                            IirLpFilter tmp = xmlser.Deserialize(sr) as IirLpFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new IirLpTab(FilterSpecific, filter);
                        }
                        break;
                    case "NoarFilter":
                        {
                            XmlSerializer xmlser = new XmlSerializer(typeof(Cheb1Filter));
                            filter = xmlser.Deserialize(sr) as NoarFilter;
                            FilterSpecific = new FilterProperty(filter);
                            InputParamForm = new NoarTab(FilterSpecific, filter);
                        }
                        break;
                    default:
                        break;
                }
                AddFilter(filter, InputParamForm, FilterSpecific);
                sr.Close();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DFDWizard WF = new DFDWizard(this.Location);
            WF.GetMethod += new DFDWizard.ChooseMethodEvent(WF_GetMethod);
            DialogResult result = WF.ShowDialog();
            AbstractFilter filter = null;
            FilterProperty FilterSpecific = null;
            UserControl InputParamForm = null;
            if (result == DialogResult.OK)
            {
                if (Method.Equals("cheby1"))
                {
                    filter = new Cheb1Filter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new Cheby1Tab(FilterSpecific, filter);
                }
                if (Method.Equals("cheby2"))
                {
                    filter = new Cheb2Filter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new Cheby2Tab(FilterSpecific, filter);
                }
                if (Method.Equals("ellip.m"))
                {
                    filter = new EllipFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new EllipTab(FilterSpecific, filter);
                }
                if (Method.Equals("butter"))
                {
                    filter = new ButterFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new ButterTab(FilterSpecific, filter);
                }
                if (Method.Equals("firls"))
                {
                    filter = new FirLSFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new FirLSTab(FilterSpecific, filter);
                }
                if (Method.Equals("firfs"))
                {
                    filter = new FirFSFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new FirFSTab(FilterSpecific, filter);
                }
                if (Method.Equals("iirlpnorm"))
                {
                    filter = new IirLpFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new IirLpTab(FilterSpecific, filter);
                }
                if (Method.Equals("noar"))
                {
                    filter = new NoarFilter(filters_form);
                    FilterSpecific = new FilterProperty(filter);
                    InputParamForm = new NoarTab(FilterSpecific, filter);
                }
                if (filter != null)
                {
                    AddFilter(filter, InputParamForm, FilterSpecific);
                }
            }
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabFilterPage tmp = (TabFilterPage)tabControl1.GetControl(tabControl1.SelectedIndex);
            tabControl1.Controls.Remove(tmp);
            tSPEnabled[tmp.Name].TSBtnCurr.Checked = false;
            filters_form[tmp.Name].Dispose();
            filters_form.Remove(tmp.Name);
            tSPEnabled.Remove(tmp.Name);
            if (filters_form.Count == 0)
            {
                toolStripButton3.Enabled = false;
                toolStripButton6.Enabled = false;
                toolStripButton1.Enabled = false;
                toolStripButton3.Checked = false;
                toolStripButton6.Checked = false;
                toolStripButton1.Checked = false;
            }
        }

        void cheb1_SetValidate(bool flag)
        {
            tSPEnabled[tabControl1.SelectedTab.Text].EnabledBtn = flag;
        }

        private void tabControl1_Deselected(object sender, TabControlEventArgs e)
        {
            if ((e != null)&&(e.TabPage!= null))
            {
                tSPEnabled[e.TabPage.Text].TSBtnCurr.Checked = false;
            }
        }

        private void tabControl1_Selected_1(object sender, TabControlEventArgs e)
        {
            if (tabControl1.Controls.Count == 0)
            {
                closeToolStripMenuItem.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
            }
            else
            {
                tSPEnabled[e.TabPage.Text].TSBtnCurr.Checked = true;
            }
        }       

        void WF_GetMethod(string method)
        {
            Method = method;
        }

        private void SetStatusStripLabel(string btnName, bool notbusy)
        {
            if (notbusy)
            {
                statusStrip1.Items[0].Text = "Ready";
            }
            else
            {
                statusStrip1.Items[0].Text = "Busy";
            }
        }
        private void AddFilter(AbstractFilter filter, UserControl InputParamForm, FilterProperty FilterSpecific)
        {
            filter.flrs_form = filters_form;
            filter.compute += new AbstractFilter.ComputeHandler(SetStatusStripLabel);

            filter.SetValidate += new AbstractFilter.VailidateHadler(FilterSpecific.SetEnableBtn);
            bool flagvalid = filter.CheckValidParam();

            filter.Name = "Filter_" + IndexFilter;

            TabFilterPage tabpg = new TabFilterPage("Filter_" + IndexFilter);

            filters_form.Add("Filter_" + IndexFilter, filter);
            toolStripButton3.Checked = true;
            toolStripButton3.Enabled = true;
            toolStripButton6.Enabled = true;
            toolStripButton1.Enabled = true;
            tSPEnabled.Add("Filter_" + IndexFilter, new tSPE(toolStripButton3, flagvalid));

            tabpg.SCPage.Panel2.Controls.Add(FilterSpecific);
            tabpg.SCPage.Panel1.Controls.Add(InputParamForm);

            tabpg.Name = "Filter_" + IndexFilter;
            tabControl1.Controls.Add(tabpg);

            tabControl1.SelectedTab = tabpg;
            cheb1_SetValidate(flagvalid);

            saveToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            IndexFilter++;

        }
        private Dictionary<string, tSPE> tSPEnabled = new Dictionary<string, tSPE>();
        private string Method = null;
        public Dictionary<string,AbstractFilter> filters_form = new Dictionary<string,AbstractFilter>();
        private System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

    }
}
