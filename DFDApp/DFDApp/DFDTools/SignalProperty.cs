using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DFDLab.DFDTools
{
    public partial class SignalProperty : UserControl
    {
        ToolStripButton tsBtnCurr;
        ToolStripButton tsBtnPrev;
        
        TestSignal sigtest;

        /*
        ImpzResp impulse;
        ZeroPoleMap zpmap;
        Phase phase;
        FilterMeandr step;
        */

        public SignalProperty(AbstractFilter filter)
        {
            InitializeComponent();
            this.filter = filter;
            this.Dock = DockStyle.Fill;
            TSBtnCurr = tSBSigHelp;// tSBTestsamples;
            tsBtnPrev = tSBSigHelp;// tSBTestsamples;

            sigtest = new TestSignal(filter, tSBTestsamples.Name);
            sigtest.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            sigtest.FillControl += new WraperISCMS.FillControlHandler(sigtest_FillControl);
            sigtest.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);

            listworks = new Dictionary<string, WraperISCMS>()
            {
                {tSBTestsamples.Name, sigtest}
/*                {tSBImpulse.Name  , impulse  },
                {tSBZeroPole.Name , zpmap    },
                {tSBPhase.Name , phase    },
                {tSBStep.Name , step    }*/
            };

            this.filter.paramchanged += new AbstractFilter.ParamChanged(sigtest.SetUpdated);

            this.filter.compute += new AbstractFilter.ComputeHandler(SetDisableBtn);

            SetEnableBtn(filter.CheckValidParam());
        }

        public void SetDisableBtn(string btnName, bool flag)
        {
            for (int i = 0; i < toolStrip1.Items.Count; i++)
            {
                if (!toolStrip1.Items[i].Name.Equals(btnName))
                {
                    toolStrip1.Items[i].Enabled = flag;
                }
            }
        }

        public void SetAllBorder()
        {
            foreach (KeyValuePair<string, WraperISCMS> frname in listworks)
            {
                SetBorderBtn(frname.Key, true);
                frname.Value.SetUpdated();
            }
        }

        public void SetEnableBtn(bool flag)
        {
            foreach (KeyValuePair<string, AbstractFilter> af in filter.flrs_form)
            {
                if (!af.Value.Name.Equals(filter.Name))
                {
                    if ((af.Value.RemoveRow != null))
                    {
                        //if (!af.Value.UsSi.Equals(filter.UsSi))
                        if (!af.Value.SignalList.ContainsKey(filter.Name))
                        {
                            af.Value.RemoveRow(filter.Name);
                        }
                    }
                }
                if (af.Value.SignalList.ContainsKey(filter.Name))
                {
                    if (af.Value.SetBorderAll != null)
                    {
                        af.Value.SetBorderAll();
                        //af.Value.NoSi.IsSignal = false;
                        af.Value.filtered_sig = false;
                    }
                }
            }
            for (int i = 0; i < toolStrip1.Items.Count; i++)
            {
                toolStrip1.Items[i].Enabled = flag;
            }
        }

        private void SetStopWorker(string btnName)
        {
            toolStrip1.Items[btnName].Image = ((System.Drawing.Image)(res.GetObject("Stop.Image")));
            filter.ComputeFlag = btnName;
        }

        private void SetBorderBtn(string btnName, bool flag)
        {
            if (flag == true)
            {
                toolStrip1.Items[btnName].ImageTransparentColor = Color.Green;
                toolStrip1.Items[btnName].Image = ((System.Drawing.Image)(res.GetObject(toolStrip1.Items[btnName].Name + ".Image")));
                Change();
                filter.ComputeFlag = "none";
            }
            else
            {
                toolStrip1.Items[btnName].ImageTransparentColor = Color.Black;
                toolStrip1.Items[btnName].Image = ((System.Drawing.Image)(res.GetObject(toolStrip1.Items[btnName].Name + ".Image")));
            }

        }
        
        private void UpdatePanel(Type tp, string btnName)
        {
            if (toolStrip1.Items[btnName] == tsBtnCurr)
            {
                if (tsBtnCurr.Checked == false)
                {
                    groupBox2.Controls["pB" + tsBtnPrev.Name].Visible = false;
                    groupBox2.Controls["pB" + tsBtnCurr.Name].Visible = true;
                    tsBtnPrev.Checked = false;
                    tsBtnCurr.Checked = true;
                    groupBox2.Text = tsBtnCurr.Text;
                    tsBtnPrev = tsBtnCurr;
                }
            }
        }
        void sigtest_FillControl(object content, string btnName)
        {
            double[] filtered = content as double[];
            
            //Chart chart = groupBox2.Controls["pB" + btnName] as Chart;
            pBtSBTestsamples.Series["Distorted"].Points.Clear();
            pBtSBTestsamples.Series["Filtered"].Points.Clear();
            pBtSBTestsamples.Series["Original"].Points.Clear();
            pBtSBTestsamples.ChartAreas[0].AxisY.Minimum = filter.InSignal.Min();
            pBtSBTestsamples.ChartAreas[0].AxisY.Maximum = filter.InSignal.Max();
            for (int i = 0; i < filter.UsSi.Samples.Length; i++)
            {
                pBtSBTestsamples.Series["Distorted"].Points.AddXY(Convert.ToDouble(i), filter.InSignal[i]);
                pBtSBTestsamples.Series["Original"].Points.AddXY(Convert.ToDouble(i), filter.UsSi.Samples[i]);
            }
            for (int i = 0; i < filtered.Length; i++)
            {
                pBtSBTestsamples.Series["Filtered"].Points.AddXY(Convert.ToDouble(i), filtered[i]);
            }
            SetBorderBtn(btnName, false);
            filter.ComputeFlag = "none";
            foreach (KeyValuePair<string, AbstractFilter> af in filter.flrs_form)
            {
                if (!af.Value.Name.Equals(filter.Name))
                {
                    //if ((af.Value.AddRow != null) && (!af.Value.UsSi.Equals(filter.UsSi)))
                    //if ((af.Value.AddRow != null) && (!af.Value.SignalList.ContainsKey(filter.Name)))
                    if ((af.Value.AddRow != null) && (!filter.SignalList.ContainsKey(af.Value.Name)))
                    {
                        af.Value.AddRow(filter.Name);
                    }
                }
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            TSBtnCurr = btn;
            UpdatePanel(null, btn.Name);
        }

        private void toolStripButton234_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            if (listworks[btn.Name].backgroundWorker1.IsBusy)
            {
                SetBorderBtn(btn.Name, true);
                Change();
                listworks[btn.Name].backgroundWorker1.CancelAsync();
                filter.ComputeFlag = "none";
                //filter.mySignal.Set();
            }
            else
            {
                TSBtnCurr = btn;
                listworks[btn.Name].RunISC();
            }
        }

        public ToolStripButton TSBtnCurr
        {
            get { return tsBtnCurr; }
            set { tsBtnCurr = value; }
        }

        public void Change()
        {
            if (tsBtnPrev != null) tsBtnCurr = tsBtnPrev;
        }

        private Dictionary<string, WraperISCMS> listworks;// = new Dictionary<string, WraperISCMS>();
        private System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(SignalProperty));
        private AbstractFilter filter;
    }
}
