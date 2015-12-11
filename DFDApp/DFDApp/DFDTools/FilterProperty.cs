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
    public partial class FilterProperty : UserControl
    {
        ToolStripButton tsBtnCurr;
        ToolStripButton tsBtnPrev;
        AbsFreqz magnitude;
        ComputeKorr compkorr;
        ImpzResp impulse;
        ZeroPoleMap zpmap;
        Phase phase;
        FilterMeandr step;
        TF2TXT tf2txt;

        public FilterProperty(AbstractFilter filter)
        {
            InitializeComponent();
            toolStrip1.SuspendLayout();
            this.filter = filter;
            this.Dock = DockStyle.Fill;
            TSBtnCurr = tSBSpecification;
            tsBtnPrev = tSBSpecification;
            /*
            magnitude = new AbsFreqz(filter, tSBMagnitude.Name);
            magnitude.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            magnitude.FillControl += new WraperISCMS.FillControlHandler(magnitude_FillControl);
            magnitude.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            magnitude.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);
            */
            /*
            tf2txt = new TF2TXT(filter, tSBTF2TXT.Name);
            tf2txt.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            tf2txt.FillControl += new WraperISCMS.FillControlHandler(tf2txt_FillControl);
            tf2txt.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            tf2txt.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);
            */
            /*impulse = new ImpzResp(filter, tSBImpulse.Name);
            impulse.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            impulse.FillControl += new WraperISCMS.FillControlHandler(impulse_FillControl);
            impulse.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            impulse.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);
           
            zpmap = new ZeroPoleMap(filter, tSBZeroPole.Name);
            zpmap.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            zpmap.FillControl += new WraperISCMS.FillControlHandler(zeropole_FillControl);
            zpmap.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            zpmap.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);
           */ 
            //phase = new Phase(filter, tSBPhase.Name);
            //phase.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            //phase.FillControl += new WraperISCMS.FillControlHandler(magnitude_FillControl);
            //phase.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            //phase.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);

            //step = new FilterMeandr(filter, tSBStep.Name);
            //step.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            //step.FillControl += new WraperISCMS.FillControlHandler(impulse_FillControl);
            //step.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            //step.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);

            //compkorr = new ComputeKorr(filter, tSBKorr.Name);
            //compkorr.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            //compkorr.FillControl += new WraperISCMS.FillControlHandler(compkorr_FillControl);
            //compkorr.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            //compkorr.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);

            listworks = filter.listFilterProp;
            //listworks.Add(tSBTF2TXT.Name, 
            /* new Dictionary<string, WraperISCMS>()
            {
                {tSBMagnitude.Name, magnitude},
                {tSBImpulse.Name  , impulse  },
                {tSBZeroPole.Name , zpmap    },
                {tSBPhase.Name , phase    },
                {tSBStep.Name , step    },
                {tSBKorr.Name , compkorr    },
                {tSBTF2TXT.Name, tf2txt}
            };*/
            foreach (KeyValuePair<string, WraperISCMS> elem in listworks)
            {
                elem.Value.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
                switch (elem.Value.Chart)
                {
                    case WraperISCMS.TypeChart.OneLine:
                        elem.Value.FillControl += new WraperISCMS.FillControlHandler(oneLine_FillControl);
                        break;
                    case WraperISCMS.TypeChart.TwoLine:
                        break;
                    case WraperISCMS.TypeChart.Polar:
                        elem.Value.FillControl += new WraperISCMS.FillControlHandler(zeropole_FillControl);
                        break;
                    case WraperISCMS.TypeChart.TextArea:
                        elem.Value.FillControl += new WraperISCMS.FillControlHandler(tf2txt_FillControl);
                        break;
                    default:
                        break;
                }
                
                elem.Value.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
                elem.Value.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);
                
                this.toolStrip1.Items[elem.Value.Name].Visible = true;
                
                toolStrip1.ResumeLayout(false);
            }

            
            //this.filter.paramchanged += new AbstractFilter.ParamChanged(phase.SetUpdated);
            //this.filter.paramchanged += new AbstractFilter.ParamChanged(step.SetUpdated);
            //this.filter.paramchanged += new AbstractFilter.ParamChanged(tf2txt.SetUpdated);
            //this.filter.paramchanged += new AbstractFilter.ParamChanged(compkorr.SetUpdated);

            this.filter.compute += new AbstractFilter.ComputeHandler(SetDisableBtn);

            SetEnableBtn(filter.CheckValidParam());
            
        }

        public void SetDisableBtn(string btnName, bool flag)
        {
            for (int i = 1; i < toolStrip1.Items.Count; i++)
            {
                if (!toolStrip1.Items[i].Name.Equals(btnName))
                {
                    toolStrip1.Items[i].Enabled = flag;
                }
            }
        }

        public void SetEnableBtn(bool flag)
        {
            foreach (KeyValuePair<string, WraperISCMS> frname in listworks)
            {
                SetBorderBtn(frname.Key, true);
            }
            for (int i = 1; i < toolStrip1.Items.Count; i++)
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
                toolStrip1.Items[btnName].ImageTransparentColor = Color.Magenta;
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
                    WraperISCMS elemPrev = null;
                    WraperISCMS elemCurr = null;
                    if (listworks.TryGetValue(btnName, out elemCurr) && listworks.TryGetValue(tsBtnPrev.Name, out elemPrev))
                    {
                        if (!elemPrev.Chart.ToString().Equals(elemCurr.Chart.ToString()))
                        {
                            groupBox2.Controls["pB" + elemPrev.Chart.ToString()].Visible = false;
                            groupBox2.Controls["pB" + elemCurr.Chart.ToString()].Visible = true;
                        }
                    }
                    if (listworks.TryGetValue(btnName, out elemCurr) && !listworks.TryGetValue(tsBtnPrev.Name, out elemPrev))
                    {
                        groupBox2.Controls["pB" + tsBtnPrev.Name].Visible = false;
                        groupBox2.Controls["pB" + elemCurr.Chart.ToString()].Visible = true;
                    }
                    if (!listworks.TryGetValue(btnName, out elemCurr) && listworks.TryGetValue(tsBtnPrev.Name, out elemPrev))
                    {
                        groupBox2.Controls["pB" + elemPrev.Chart.ToString()].Visible = false;
                        groupBox2.Controls["pB" + tsBtnCurr.Name].Visible = true;

                    }
                    if (!listworks.TryGetValue(btnName, out elemCurr) && !listworks.TryGetValue(tsBtnPrev.Name, out elemPrev))
                    {
                        groupBox2.Controls["pB" + tsBtnPrev.Name].Visible = false;
                        groupBox2.Controls["pB" + tsBtnCurr.Name].Visible = true;
                    }

                    tsBtnPrev.Checked = false;
                    tsBtnCurr.Checked = true;
                    groupBox2.Text = tsBtnCurr.Text;
                    tsBtnPrev = tsBtnCurr;
                }
            }
        }

        void tf2txt_FillControl(object content, string btnName)
        {
            string str = content as string;
            pBTextArea.Text = str;

            SetBorderBtn(btnName, false);
            filter.ComputeFlag = "none";
        }
        void oneLine_FillControl(object content, string btnName)
        {
            double[] Result = content as double[];

            if (Result != null)
            {
                WraperISCMS elem = null;
                listworks.TryGetValue(btnName, out elem);
                pBOneLine.ChartAreas[0].AxisY.Minimum = elem.MinY;
                pBOneLine.ChartAreas[0].AxisY.Maximum = elem.MaxY;
                pBOneLine.Series[0].Points.Clear();
                for (int i = 0; i < Result.Length; i++)
                {
                    pBOneLine.Series[0].Points.Add(new DataPoint(Convert.ToDouble(i) * elem.dX, Result[i]));
                }
                SetBorderBtn(btnName, false);
                filter.ComputeFlag = "none";
            }
        }
        void zeropole_FillControl(object content, string btnName)
        {
            PoleZeroArray zpArray = content as PoleZeroArray;
            pBPolar.Series["Pole"].Points.Clear();
            pBPolar.Series["Zero"].Points.Clear();
            for (int i = 0; i < zpArray.PRe.Length; i++)
            {
                pBPolar.Series["Pole"].Points.AddXY(Math.Atan2(zpArray.PIm[i], zpArray.PRe[i]) * 180.0 / Math.PI, Math.Sqrt(zpArray.PRe[i] * zpArray.PRe[i] + zpArray.PIm[i] * zpArray.PIm[i]));
            }
            for (int i = 0; i < zpArray.ZRe.Length; i++)
            {
                pBPolar.Series["Zero"].Points.AddXY(Math.Atan2(zpArray.ZIm[i], zpArray.ZRe[i]) * 180.0 / Math.PI, Math.Sqrt(zpArray.ZRe[i] * zpArray.ZRe[i] + zpArray.ZIm[i] * zpArray.ZIm[i]));
            }

            SetBorderBtn(btnName, false);
            filter.ComputeFlag = "none";
        }

        void compkorr_FillControl(object content, string btnName)
        {
            double[] KorrCoef = content as double[];
            if (KorrCoef != null)
            {
                Chart chart = groupBox2.Controls["pB" + btnName] as Chart;
                chart.Series[0].Points.Clear();
                for (int i = 0; i < KorrCoef.Length; i++)
                {
                    chart.Series[0].Points.Add(new DataPoint(Convert.ToDouble(i) , KorrCoef[i]));
                }
                SetBorderBtn(btnName, false);
                filter.ComputeFlag = "none";
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
               // while (listworks[btn.Name].backgroundWorker1.IsBusy)
                //    Application.DoEvents(); // Иначе IsBusy так и останется true навсегда
                filter.ComputeFlag = "none";
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
        public ToolStripButton TSBtnPrev
        {
            get { return tsBtnPrev; }
            set { tsBtnPrev = value; }
        }


        public void Change()
        {
            if (tsBtnPrev != null) tsBtnCurr = tsBtnPrev;
        }

        private Dictionary<string, WraperISCMS> listworks;// = new Dictionary<string, WraperISCMS>();
        private System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(FilterProperty));
        private AbstractFilter filter;
    }
}
