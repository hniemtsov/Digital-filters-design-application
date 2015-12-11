using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DFDLab.DFDTools
{
    public partial class IirLpTab : UserControl
    {
        public IirLpFilter filter;
        NumberFormatInfo provider = new NumberFormatInfo();
                
        private int validFg;

        public IirLpTab(FilterProperty fs, AbstractFilter fil)
        {
            InitializeComponent();
            validFg = 0;
            provider.NumberDecimalSeparator = ".";
            this.filter = (IirLpFilter)fil;
            textBox1.Text = (filter.Order + 1).ToString();
            textBox2.Text = (filter.OrderDen + 1).ToString();
            int nTmp = filter.NumF;
            for (int i = 0; i < nTmp; i++)
            {
                textBox3.Text = textBox3.Text + filter.edgeFqs[i].ToString(provider) + " ";
                textBox4.Text = textBox4.Text + filter.edges[i].ToString(provider) + " ";
                textBox5.Text = textBox5.Text + filter.des_mag[i].ToString(provider) + " ";
            }
            textBox6.Text = filter.weights[0].ToString() + " " + filter.weights[1].ToString();
            
            this.Dock = DockStyle.Fill;


            WebBrowser wb = fs.Controls["groupBox1"].Controls["groupBox2"].Controls["pBtSBHelp"] as WebBrowser;
            PictureBox pb = fs.Controls["groupBox1"].Controls["groupBox2"].Controls["pBtSBSpecification"] as PictureBox;
            ToolStrip ts = fs.Controls["groupBox1"].Controls["toolStrip1"] as ToolStrip;
            ToolStripButton tsbSpec = ts.Items["tSBSpecification"] as ToolStripButton;
            ToolStripButton tsbHelp = ts.Items["tSBHelp"] as ToolStripButton;
            pb.Visible = false;
            wb.Visible = true;
            tsbSpec.Visible = false;
            tsbHelp.Visible = true;

            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = System.IO.Path.GetDirectoryName(a.Location);
            baseDir = baseDir + "\\Help\\";
            baseDir = baseDir + "QuasyNewton1.htm";
            wb.Url = new Uri(baseDir);

            fs.TSBtnCurr = tsbHelp;
            fs.TSBtnPrev = tsbHelp;

            this.fs = fs;

        }

        private FilterProperty fs;
        private ArrayList listitem = new ArrayList();
        private double dtmp;

        private bool ValidTb1()
        {
            bool Ret = false;
            if (textBox1.Text.CompareTo("") != 0)
            {
                filter.SetUpdatedCoef();
                filter.Order = System.Convert.ToInt32(textBox1.Text);
                Ret = filter.ValidOrder();
            }
            validFg = (validFg & ~1)|(Convert.ToInt32(Ret==false)<<0);
            return Ret;
        }
        private bool ValidTb2()
        {
            bool Ret = false;
            if (textBox2.Text.CompareTo("") != 0)
            {
                filter.SetUpdatedCoef();
                filter.OrderDen = System.Convert.ToInt32(textBox2.Text);
                Ret = filter.ValidOrderDen();
            }
            validFg = (validFg & ~(1<<1)) | (Convert.ToInt32(Ret == false) << 1);
            return Ret;
        }

        private bool ValidTb3()
        {
            bool Ret = false;
            if (textBox3.Text.CompareTo("") != 0)
            {
                MatchCollection FQ = Regex.Matches(textBox3.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (FQ.Count < 250)
                {
                    filter.NumF = FQ.Count;
                    filter.SetUpdatedCoef();
                    for (int i = 0; i < FQ.Count; i++)
                    {
                        filter.edgeFqs[i] = Convert.ToDouble(FQ[i].ToString(), provider);
                    }
                }
                Ret = filter.ValidEdgeFqs(FQ.Count);
            }
            validFg = (validFg & ~(1 << 2)) | (Convert.ToInt32(Ret == false) << 2);
            return Ret;
        }
        private bool ValidTb4()
        {
            bool Ret = false;
            if (textBox4.Text.CompareTo("") != 0)
            {
                MatchCollection FQ = Regex.Matches(textBox4.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (FQ.Count < 250)
                {
                    filter.NumE = FQ.Count;
                    filter.SetUpdatedCoef();
                    for (int i = 0; i < FQ.Count; i++)
                    {
                        filter.edges[i] = Convert.ToDouble(FQ[i].ToString(), provider);
                    }
                }
                Ret = filter.ValidEdges(FQ.Count);
            }
            validFg = (validFg & ~(1 << 3)) | (Convert.ToInt32(Ret == false) << 3);
            return Ret;
        }
        private bool ValidTb5()
        {
            bool Ret = false;
            if (textBox5.Text.CompareTo("") != 0)
            {
                MatchCollection DM = Regex.Matches(textBox5.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (DM.Count < 250)
                {
                    filter.SetUpdatedCoef();
                    for (int i = 0; i < DM.Count; i++)
                    {
                        filter.des_mag[i] = Convert.ToDouble(DM[i].ToString());
                    }
                }
                Ret = filter.ValidDesrdMag(DM.Count);

            }
            validFg = (validFg & ~(1 << 4)) | (Convert.ToInt32(Ret == false) << 4);
            return Ret;
        }

        private bool ValidTb6()
        {
            bool Ret = false;
            if (textBox6.Text.CompareTo("") != 0)
            {
                MatchCollection WT = Regex.Matches(textBox6.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (WT.Count < 250)
                {
                    //firlsfil.NumF = WT.Count;
                    filter.SetUpdatedCoef();
                    for (int i = 0; i < WT.Count; i++)
                    {
                        filter.weights[i] = Convert.ToDouble(WT[i].ToString(), provider);
                    }
                }
                Ret = filter.ValidWeights(WT.Count);
            }
            validFg = (validFg & ~(1 << 5)) | (Convert.ToInt32(Ret == false) << 5);
            return Ret;
        }
        private void textBox_TextChanged_add(object sender, bool flag)
        {
            TextBox tb = sender as TextBox;
            if (flag == false)
            {
                tb.BackColor = Color.Red;
            }
            else
            {
                tb.BackColor = Color.White;
            }
            if (filter.SetValidate != null)
            {
                foreach (AbstractFilter.VailidateHadler setvalid in filter.SetValidate.GetInvocationList())
                {
                    setvalid((validFg == 0));//всех подписавшихся изыестить об изменении параметров 
                }
            }
        }
        private void textBox_1_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb1();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_2_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb2();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_3_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb3();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_4_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb4();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_5_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb5();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_6_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb6();
            textBox_TextChanged_add(sender, flagValidate);
        }
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Если это не цифра.
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar != '\b'))
            {
                // Запрет на ввод более одной десятичной точки.
                if (e.KeyChar != '.' || tb.Text.IndexOf(".") != -1)
                {
                    e.Handled = true;
                }
            }
        }

        bool point = false;
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Если это не цифра и не пробел.
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar != '\b') && (!Char.IsWhiteSpace(e.KeyChar)))
            {
                // Запрет на ввод более одной десятичной точки.
                if (e.KeyChar != '.')
                {
                    e.Handled = true;
                }
                else if (point == true)
                {
                    e.Handled = true;
                }
                else
                {
                    point = true;
                }
            }
            else
            {
                point = false;
            }
        }

    }
}
