using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace DFDLab.DFDTools
{

    public partial class SignalParameters : UserControl
    {
        Font fnt = new Font(FontFamily.GenericSerif, 10, FontStyle.Regular);
        ImageList img = new ImageList();
        AbstractFilter filter;
        SignalProperty sigprop;

        private WebBrowser wb;

        public SignalParameters(SignalProperty sp, AbstractFilter fil)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.filter = fil;
            this.sigprop = sp;

            wb = sp.Controls["groupBox1"].Controls["groupBox2"].Controls["pBtSBSigHelp"] as WebBrowser;
            
            InitCombobox(toolStripComboBox1, 2);
            InitCombobox(toolStripComboBox2, 1);
            
            foreach (KeyValuePair<string, AbstractFilter> af in filter.flrs_form)
            {
                if (!af.Value.Name.Equals(filter.Name))
                {
                    if (af.Value.UsSi != null)
                    {
                        if (af.Value.UsSi.IsSignal /*&& af.Value.SignalList.Count == 0*/)
                        {
                            toolStripComboBox1.ComboBox.Items.Add(af.Value.Name);
                        }
                    }
                }
            }
            
        }
        public void AddSign2Cmbx(string name)
        {
            if (!toolStripComboBox1.ComboBox.Items.Contains(name))
            {
                toolStripComboBox1.ComboBox.Items.Add(name);
            }
        }
        public void RemoveSign2Cmbx(string name)
        {
            toolStripComboBox1.ComboBox.Items.Remove(name);
        }

        private void InitCombobox(ToolStripComboBox tcmbx, int idx)
        {
            tcmbx.ComboBox.Items.Add("BPSK  signal");
            tcmbx.ComboBox.Items.Add("White noise ");
            tcmbx.ComboBox.Items.Add("AR model    ");

            tcmbx.ComboBox.SelectedIndex = idx;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSignal = ((ToolStripComboBox)sender).SelectedItem as string;

            if (groupBox2.Controls.Count != 0)
            {
                groupBox2.Controls.Remove(groupBox2.Controls[0]);
            }
            filter.SignalList.Clear();
            GeneralSignalTab ucnt = null;
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = System.IO.Path.GetDirectoryName(a.Location);
            baseDir = baseDir + "\\Help\\";
            switch (selectedSignal)
            {
                case "White noise ":
                    filter.UsSi = new WNSignal();
                    ucnt = new WNsignalTab(filter.UsSi);
                    ((WNsignalTab)ucnt).SetValidate += new WNsignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox2.Controls.Add(ucnt);
                    baseDir = baseDir + "White noise.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                case "BPSK  signal":
                    filter.UsSi = new BPSKSignal();
                    ucnt = new BPSKSignalTab(filter.UsSi);
                    ((BPSKSignalTab)ucnt).SetValidate += new BPSKSignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox2.Controls.Add(ucnt);
                    baseDir = baseDir + "BPSK signal.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                case "AR model    ":
                    filter.UsSi = new ARSignal();
                    ucnt = new ARsignalTab(filter.UsSi);
                    ((ARsignalTab)ucnt).SetValidate += new ARsignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox2.Controls.Add(ucnt);
                    baseDir = baseDir + "AR Model.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                default:
                    filter.UsSi = filter.flrs_form[selectedSignal].UsSi;
                    filter.NoSi.N = filter.UsSi.N;

                    filter.SignalList = filter.SignalList.Concat(filter.flrs_form[selectedSignal].SignalList).ToDictionary( var => var.Key, var => var.Value );
                    
                    FilterSignalTab ucntf = new FilterSignalTab(filter.SignalList);
                    groupBox2.Controls.Add(ucntf);
                    break;
            };
            if (setNh != null )
            {
                setNh(filter.UsSi.N);
                if (ucnt != null)
                {
                    ucnt.SetNparam += setNh;
                }
            }

            filter.SignalList.Add(filter.Name, filter.GetFilterDescr());
            filter.filtered_sig = false;
            //sigprop.SetAllBorder();
            sigprop.SetEnableBtn(true);
            //.SetAllBorder();

        }
        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSignal = ((ToolStripComboBox)sender).SelectedItem as string;

            if (groupBox3.Controls.Count != 0)
            {
                groupBox3.Controls.Remove(groupBox3.Controls[0]);
            }
            GeneralSignalTab ucnt;
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = System.IO.Path.GetDirectoryName(a.Location);
            baseDir = baseDir + "\\Help\\";
            if (setNh != null && (groupBox2.Controls[0] is GeneralSignalTab))
            {
                ((GeneralSignalTab)groupBox2.Controls[0]).SetNparam -= setNh;
            }
            switch (selectedSignal)
            {
                case "White noise ":
                    filter.NoSi = new WNSignal(filter.UsSi.N);
                    ucnt = new WNsignalTab(filter.NoSi, false);
                    if (groupBox2.Controls[0] is GeneralSignalTab)
                    {
                        setNh = new GeneralSignalTab.SetNHadler(((WNsignalTab)ucnt).SetTextBox1);
                        ((GeneralSignalTab)groupBox2.Controls[0]).SetNparam += setNh;
                    }
                    ((WNsignalTab)ucnt).SetValidate += new WNsignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox3.Controls.Add(ucnt);
                    baseDir = baseDir + "White noise.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                case "BPSK  signal":
                    filter.NoSi = new BPSKSignal(filter.UsSi.N);
                    
                    ucnt = new BPSKSignalTab(filter.NoSi, false);

                    if (groupBox2.Controls[0] is GeneralSignalTab)
                    {
                        setNh = new GeneralSignalTab.SetNHadler(((BPSKSignalTab)ucnt).SetTextBox1);
                        ((GeneralSignalTab)groupBox2.Controls[0]).SetNparam += setNh;
                    }

                    ((BPSKSignalTab)ucnt).SetValidate += new BPSKSignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox3.Controls.Add(ucnt);
                    baseDir = baseDir + "BPSK signal.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                case "AR model    ":
                    filter.NoSi = new ARSignal(filter.UsSi.N);
                    ucnt = new ARsignalTab(filter.NoSi, false);
                    if (groupBox2.Controls[0] is GeneralSignalTab)
                    {
                        setNh = new GeneralSignalTab.SetNHadler(((ARsignalTab)ucnt).SetTextBox1);
                        ((GeneralSignalTab)groupBox2.Controls[0]).SetNparam += setNh;
                    }

                    ((ARsignalTab)ucnt).SetValidate += new ARsignalTab.VailidateHadler(sigprop.SetEnableBtn);
                    groupBox3.Controls.Add(ucnt);
                    baseDir = baseDir + "AR Model.htm";
                    wb.Url = new Uri(baseDir);
                    break;
                default:
                    break;
            };
            sigprop.SetEnableBtn(true);
        }
        private GeneralSignalTab.SetNHadler setNh = null;
    }
}
