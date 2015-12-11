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
    public partial class FirFSTab : UserControl
    {
        public FirFSFilter firlsfil;
        NumberFormatInfo provider = new NumberFormatInfo();
        private int validFg;

        public FirFSTab(FilterProperty fs, AbstractFilter fil)
        {
            int nTmp;
            InitializeComponent();
            validFg = 0;
            provider.NumberDecimalSeparator = ".";
            this.firlsfil = (FirFSFilter)fil;
            textBox1.Text = (firlsfil.Order+1).ToString();
            textBox3.Text = firlsfil.Fs.ToString();
            nTmp = firlsfil.NumF;
            for (int i = 0; i < nTmp; i++)
            {
                textBox4.Text = textBox4.Text + firlsfil.edgeFqs[i].ToString(provider) + " ";
                textBox2.Text = textBox2.Text + firlsfil.des_mag[i].ToString(provider) + " ";
            }
            textBox5.Text = firlsfil.nDFT.ToString();

            this.Dock = DockStyle.Fill;

            listitem.Add(new myclas("Symmetric and Odd Length ", "type1", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.LinearPhaseSpecBMP.FreqSampleType1.bmp"))));
            listitem.Add(new myclas("Symmetric and Even Length", "type2", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.LinearPhaseSpecBMP.FreqSampleType2.bmp"))));
//            listitem.Add(new myclas("Antisymmetric Odd Length ", "type3" ,new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1Band.bmp"))));
//            listitem.Add(new myclas("Antisymmetric Even Length", "type4", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1Stop.bmp"))));

            this.fs = fs;

            string tmp = (string)firlsfil.TypeFilt.Clone();

            comboBox1.DataSource = listitem;
            comboBox1.ValueMember = "nValuu";
            comboBox1.DisplayMember = "nName";
            PictureBox pb = fs.Controls["groupBox1"].Controls["groupBox2"].Controls["pBtSBSpecification"] as PictureBox;
            comboBox1.DataBindings.Add("SelectedValue", pb, "Image");
            comboBox1.DataBindings[0].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            foreach (myclas mc in listitem)
            {
                if (mc.paramName.Equals(tmp))
                {
                    pb.Image = ((myclas)listitem[comboBox1.FindString(mc.nName)]).nValuu;
                    break;
                }
            }
        }
        public void SetOrder(int n)
        {
            this.textBox1.Text = n.ToString() ;
        }
        private FilterProperty fs;
        private ArrayList listitem = new ArrayList();
        private int ntmp;
        private double dtmp;

        private bool ValidTb1()
        {
            bool Ret = false;
            if (textBox1.Text.CompareTo("") != 0)
            {
                ntmp = System.Convert.ToInt32(textBox1.Text);
                //if (firlsfil.Order != dtmp)
                {
                    firlsfil.SetUpdatedCoef();
                    firlsfil.Order = ntmp - 1;
                }
                Ret = firlsfil.ValidOrder();
            }
            validFg = (validFg & ~1) | (Convert.ToInt32(Ret == false) << 0);
            return Ret;
        }

        private bool ValidTb2()
        {
            bool Ret = false;
            if (textBox2.Text.CompareTo("") != 0)
            {
                MatchCollection DM = Regex.Matches(textBox2.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (DM.Count < 250)
                {
                    firlsfil.SetUpdatedCoef();
                    for (int i = 0; i < DM.Count; i++)
                    {
                        firlsfil.des_mag[i] = Convert.ToDouble(DM[i].ToString());
                    }
                }
                Ret = firlsfil.ValidDesrdMag(DM.Count);
                
            }
            validFg = (validFg & ~(1 << 1)) | (Convert.ToInt32(Ret == false) << 1);
            return Ret;
        }
        private bool ValidTb3()
        {
            bool Ret = false;
            if (textBox3.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox3.Text);
                //if (firlsfil.Fs != dtmp)
                {
                    firlsfil.SetUpdatedCoef();
                    firlsfil.Fs = dtmp;
                }
                Ret = firlsfil.ValidFs();
            }
            validFg = (validFg & ~(1 << 2)) | (Convert.ToInt32(Ret == false) << 2);
            return Ret;
        }
        private bool ValidTb4()
        {
            bool Ret = false;
            if ((textBox4.Text.CompareTo("") != 0) && (textBox3.Text.CompareTo("") != 0))
            {
                MatchCollection FQ = Regex.Matches(textBox4.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (FQ.Count < 250)
                {
                    firlsfil.NumF = FQ.Count;
                    firlsfil.SetUpdatedCoef();
                    for (int i = 0; i < FQ.Count; i++)
                    {
                        firlsfil.edgeFqs[i] = Convert.ToDouble(FQ[i].ToString(), provider);
                    }
                }
                Ret = firlsfil.ValidEdgeFqs(FQ.Count);
            }
            validFg = (validFg & ~(1 << 3)) | (Convert.ToInt32(Ret == false) << 3);
            return Ret;
        }
        private bool ValidTb5()
        {
            bool Ret = false;
            if (textBox5.Text.CompareTo("") != 0)
            {
                MatchCollection WT = Regex.Matches(textBox5.Text, "[0-9]+\\.?[0-9]*", RegexOptions.ExplicitCapture);
                if (WT.Count < 250)
                {
                    //firlsfil.NumF = WT.Count;
                    firlsfil.SetUpdatedCoef();
                    for (int i = 0; i < WT.Count; i++)
                    {
                        firlsfil.nDFT = Convert.ToInt32(textBox5.Text);
                    }
                }
                Ret = firlsfil.ValidnDFT();
            }
            validFg = (validFg & ~(1 << 4)) | (Convert.ToInt32(Ret == false) << 4);
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
            if (firlsfil.SetValidate != null)
            {
                foreach (AbstractFilter.VailidateHadler setvalid in firlsfil.SetValidate.GetInvocationList())
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

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Если это не цифра.
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar!='\b'))
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            firlsfil.TypeFilt = ((myclas)comboBox1.SelectedItem).paramName;
            bool flagValidate = ValidTb1();
            textBox_TextChanged_add(textBox1, flagValidate);
        }

    }
}
