using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DFDLab.DFDTools
{
    public partial class SOSRealization : UserControl
    {
        ToolStripButton tsBtnCurr;
        ToolStripButton tsBtnPrev;
        TF2SOS tf2sos;
        CodeGen ccode;

        public SOSRealization(AbstractFilter filter)
        {
            InitializeComponent();
            this.filter = filter;
            this.Dock = DockStyle.Fill;
            TSBtnCurr = tSBSpecification;
            tsBtnPrev = tSBSpecification;

            tf2sos = new TF2SOS(filter, tSBTF2SOS.Name);
            tf2sos.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            tf2sos.FillControl += new WraperISCMS.FillControlHandler(tf2sos_FillControl);
            tf2sos.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            tf2sos.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);

            ccode = new CodeGen(filter, tSBCodeGen.Name);
            ccode.SetVisibl += new WraperISCMS.UpdateVisible(UpdatePanel);
            ccode.FillControl += new WraperISCMS.FillControlHandler(tf2sos_FillControl);
            ccode.SetStop += new WraperISCMS.SetStopHandler(SetStopWorker);
            ccode.SetBorder += new WraperISCMS.SetBorderHandler(SetBorderBtn);


            listworks = new Dictionary<string, WraperISCMS>()
            {
                {tSBTF2SOS.Name, tf2sos},
                {tSBCodeGen.Name, ccode}
//                {tSBZeroPole.Name, zpmap},
            };

            this.filter.paramchanged += new AbstractFilter.ParamChanged(tf2sos.SetUpdated);
            this.filter.paramchanged += new AbstractFilter.ParamChanged(ccode.SetUpdated);

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

        public void SetUpdatedInParam()
        {
            tf2sos.SetUpdated();
            ccode.SetUpdated();
            
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
            }
            else
            {
                toolStrip1.Items[btnName].ImageTransparentColor = Color.Black;
            }
            toolStrip1.Items[btnName].Image = ((System.Drawing.Image)(res.GetObject(toolStrip1.Items[btnName].Name + ".Image")));
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

        void tf2sos_FillControl(object content,string btnName)
        {
            string str = content as string;
            groupBox2.Controls["pB" + btnName].Text = str;
            //pBtSBTF2SOS.Text = str;

            SetBorderBtn(btnName, false);
            filter.ComputeFlag = "none";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            TSBtnCurr = btn;
            UpdatePanel(null, btn.Name);
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            if (listworks[btn.Name].backgroundWorker1.IsBusy)
            {
                SetBorderBtn(btn.Name, true);
                Change();
                listworks[btn.Name].backgroundWorker1.CancelAsync();
                //listworks[btn.Name].KillerISCMC();
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

        private Dictionary<string, WraperISCMS> listworks;
        private System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(SOSRealization));
        private AbstractFilter filter;

    }
}
