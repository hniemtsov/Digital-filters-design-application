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

namespace DFDLab.DFDTools
{
    public partial class EllipTab : UserControl
    {
        public delegate void VailidateHadler(bool flag);
        public event VailidateHadler SetValidate;
        public EllipFilter ceb1fil;

        public EllipTab(FilterProperty fs, AbstractFilter fil)
        {
            InitializeComponent();

            this.ceb1fil = (EllipFilter)fil;
            textBox1.Text = ceb1fil.Order.ToString();
            textBox2.Text = ceb1fil.Rp.ToString();
            textBox5.Text = ceb1fil.Rs.ToString();
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
            return Ret;
        }

        private bool ValidTb2()
        {
            bool Ret = false;
            if (textBox2.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox2.Text);
                //if (ceb1fil.R != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.Rp = dtmp;
                }
                Ret = ceb1fil.ValidRippleP();
                
            }
            return Ret;
        }
        private bool ValidTb5()
        {
            bool Ret = false;
            if (textBox5.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox5.Text);
                //if (ceb1fil.R != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.Rs = dtmp;
                }
                Ret = ceb1fil.ValidRippleS();

            }
            return Ret;
        }
        private bool ValidTb3()
        {
            bool Ret = false;
            if (textBox3.Text.CompareTo("") != 0)
            {
                dtmp = System.Convert.ToDouble(textBox3.Text);
                //if (ceb1fil.Fs != dtmp)
                {
                    ceb1fil.SetUpdatedCoef();
                    ceb1fil.Fs = dtmp;
                }
                Ret = ceb1fil.ValidFs();
            }
            return Ret;
        }
        private bool ValidTb4()
        {
            bool Ret = false;
            if ((textBox4.Text.CompareTo("") != 0) && (textBox3.Text.CompareTo("") != 0))
            {
                MatchCollection F1F2 = Regex.Matches(textBox4.Text, "[0-9]+\\w?\\b", RegexOptions.ExplicitCapture);
                dtmp = Convert.ToDouble(F1F2[0].ToString());
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
                        case "Bandpass":
                        case "Bandstop":
                            if (F1F2.Count == 2)
                            {
                                dtmp = Convert.ToDouble(F1F2[1].ToString());
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
            return Ret;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb1() && ValidTb2() && ValidTb3() && ValidTb4();
            if (ValidTb1() == false)
            {
                textBox1.BackColor = Color.MistyRose;
            }
            else
            {
                textBox1.BackColor = Color.White;
            }

            if (ValidTb2() == false)
            {
                textBox2.BackColor = Color.MistyRose;
            }
            else
            {
                textBox2.BackColor = Color.White;
            }

            if (ValidTb5() == false)
            {
                textBox5.BackColor = Color.MistyRose;
            }
            else
            {
                textBox5.BackColor = Color.White;
            }

            if (ValidTb3() == false)
            {
                textBox3.BackColor = Color.MistyRose;
            }
            else
            {
                textBox3.BackColor = Color.White;
            }
            if (ValidTb4() == false)
            {
                textBox4.BackColor = Color.MistyRose;
            }
            else
            {
                textBox4.BackColor = Color.White;
            }

            if (SetValidate != null)
            {
                foreach (VailidateHadler setvalid in SetValidate.GetInvocationList())
                {
                    setvalid(flagValidate);//всех подписавшихся изыестить об изменении параметров 
                }
            }
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
            ceb1fil.SetUpdatedCoef();
            //sosrz.SetUpdatedInParam();
        }
    }
}
