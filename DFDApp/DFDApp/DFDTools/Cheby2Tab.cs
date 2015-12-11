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
    public partial class Cheby2Tab : UserControl
    {
        public Cheb2Filter ceb1fil;

        private int validFg;

        NumberFormatInfo provider = new NumberFormatInfo();
        public Cheby2Tab(FilterProperty fs, AbstractFilter fil)
        {
            InitializeComponent();
            validFg = 0;
            provider.NumberDecimalSeparator = ".";

            this.ceb1fil = (Cheb2Filter)fil;
            textBox1.Text = ceb1fil.Order.ToString();
            textBox2.Text = ceb1fil.R.ToString();
            textBox3.Text = ceb1fil.Fs.ToString();
            if (ceb1fil.F2 != 0)
            {
                textBox4.Text = ceb1fil.F1.ToString() + " " + ceb1fil.F2.ToString();
            }
            else
            {
                textBox4.Text = ceb1fil.F1.ToString();
            }

            this.Dock = DockStyle.Fill;

            listitem.Add(new myclas("Lowpass",  "low", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1Low.bmp"))));
            listitem.Add(new myclas("Highpass", "high", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1High.bmp"))));
            listitem.Add(new myclas("Bandpass", "bandpass" ,new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1Band.bmp"))));
            listitem.Add(new myclas("Bandstop", "stop", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Cheb1SpecBMP.Cheb1Stop.bmp"))));

            this.fs = fs;

            string tmp = (string)ceb1fil.TypeFilt.Clone();

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
        private double dtmp;

        private bool ValidTb1()
        {
            bool Ret = false;
            if (textBox1.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.Order != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.Order = System.Convert.ToInt32(textBox1.Text);
                }
                Ret = ceb1fil.ValidOrder();
            }
            validFg = (validFg & ~1) | (Convert.ToInt32(Ret == false) << 0);
            return Ret;
        }

        private bool ValidTb2()
        {
            bool Ret = false;
            if (textBox2.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox2.Text, provider);
                //if (ceb1fil.R != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.R = dtmp;
                }
                Ret = ceb1fil.ValidRipple();
                
            }
            validFg = (validFg & ~(1 << 1)) | (Convert.ToInt32(Ret == false) << 1);
            return Ret;
        }
        private bool ValidTb3()
        {
            bool Ret = false;
            if (textBox3.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox3.Text, provider);
                //if (ceb1fil.Fs != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.Fs = dtmp;
                }
                Ret = ceb1fil.ValidFs();
            }
            validFg = (validFg & ~(1 << 2)) | (Convert.ToInt32(Ret == false) << 2);
            return Ret;
        }
        private bool ValidTb4()
        {
            bool Ret = false;
            if ((textBox4.Text.CompareTo("") != 0) && (textBox3.Text.CompareTo("") != 0))
            {
                MatchCollection F1F2 = Regex.Matches(textBox4.Text, "[0-9]+\\w?\\b", RegexOptions.ExplicitCapture);
                dtmp = Convert.ToDouble(F1F2[0].ToString(), provider);
                //if (ceb1fil.F1 != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.F1 = dtmp;
                }
                Ret = ceb1fil.ValidF1();
                if (comboBox1.SelectedItem != null)
                {
                    string stmp = ((myclas)comboBox1.SelectedItem).paramName;
                    //if (ceb1fil.TypeFilt.Equals(stmp))
                    {
                        ceb1fil.SetUpdatedCoef();
                        ceb1fil.TypeFilt = stmp.Clone() as string;
                    }
                    switch (((myclas)comboBox1.SelectedItem).Name)
                    {
                        case "Lowpass":
                        case "Highpass":
                            if (F1F2.Count == 2)
                            {
                                Ret = false;
                            }
                            break;
                        case "Bandpass":
                        case "Bandstop":
                            if (F1F2.Count == 2)
                            {
                                dtmp = Convert.ToDouble(F1F2[1].ToString(), provider);
                                //if (ceb1fil.F2 != dtmp)
                                {
                                    ceb1fil.SetUpdatedCoef();
                                    ceb1fil.F2 = dtmp;
                                }
                                Ret = Ret & ceb1fil.ValidF2();
                            }
                            else
                            {
                                Ret = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            validFg = (validFg & ~(1 << 3)) | (Convert.ToInt32(Ret == false) << 3);
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
            if (ceb1fil.SetValidate != null)
            {
                foreach (AbstractFilter.VailidateHadler setvalid in ceb1fil.SetValidate.GetInvocationList())
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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // Если это не цифра и не пробел.
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar != '\b') && (!Char.IsWhiteSpace(e.KeyChar)))
            {
                // Запрет на ввод более одной десятичной точки.
                if (e.KeyChar != '.' || tb.Text.IndexOf(".") != -1)
                {
                    e.Handled = true;
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ceb1fil.TypeFilt = ((myclas)comboBox1.SelectedItem).nName;
            bool flagValidate = ValidTb4();
            textBox_TextChanged_add(textBox4, flagValidate);
        }
    }
}
