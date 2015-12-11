using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace DFDLab.DFDTools
{
    public partial class ARsignalTab : GeneralSignalTab //: UserControl
    {
    //    public delegate void VailidateHadler(bool flag);
    //    public event VailidateHadler SetValidate;
        
        //public delegate void SetNHadler(int N);
        //public event SetNHadler SetNparam;

        public ARSignal arsignl;
        NumberFormatInfo provider = new NumberFormatInfo();
        private int validFg;


        public ARsignalTab(AbstractSignal signl)
        {
            InitializeComponent();
            validFg = 0;
            provider.NumberDecimalSeparator = ".";

            this.arsignl = (ARSignal)signl;

            textBox1.Text = arsignl.N.ToString();
            textBox2.Text = arsignl.ArCoef;
            textBox3.Text = arsignl.Sigma.ToString(provider);
            textBox4.Text = arsignl.Initial;

            this.Dock = DockStyle.Fill;
        }
        public ARsignalTab(AbstractSignal signl, Boolean flag)
            : this(signl)
        {
            textBox1.Enabled = false;
        }

        private bool ValidTb1()
        {
            bool Ret = false;
            if (textBox1.Text.CompareTo("") != 0)
            {

                //if (ceb1fil.Order != dtmp)
                {
                    arsignl.SetUpdatedSignal();
                    arsignl.N = System.Convert.ToInt32(textBox1.Text);
                }
                Ret = arsignl.ValidN();
            }
            validFg = (validFg & ~1) | (Convert.ToInt32(Ret == false) << 0);
            return Ret;
        }
        private bool ValidTb2()
        {
            bool Ret = false;
            if (textBox2.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.R != dtmp)
                {
                    arsignl.SetUpdatedSignal();
                    arsignl.ArCoef = textBox2.Text;
                }
                Ret = arsignl.ValidCoef();

            }
            validFg = (validFg & ~(1 << 1)) | (Convert.ToInt32(Ret == false) << 1);
            return Ret;
        }
        private bool ValidTb3()
        {
            bool Ret = false;
            if (textBox3.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.R != dtmp)
                {
                    arsignl.SetUpdatedSignal();
                    
                    arsignl.Sigma = Convert.ToDouble(textBox3.Text, provider);
                }
                Ret = arsignl.ValidSigma();

            }
            validFg = (validFg & ~(1 << 2)) | (Convert.ToInt32(Ret == false) << 2);
            return Ret;
        }
        private bool ValidTb4()
        {
            bool Ret = false;
            if (textBox4.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.R != dtmp)
                {
                    arsignl.SetUpdatedSignal();
                    arsignl.Initial = textBox4.Text;
                }
                Ret = arsignl.ValidCoef();

            }
            validFg = (validFg & ~(1 << 3)) | (Convert.ToInt32(Ret == false) << 3);
            return Ret;
        }
        private void textBox_1_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb1();
            if (flagValidate == true)
            {
                RunSetNParam(arsignl.N);
            }
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
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar != '\b'))
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

        public void SetTextBox1(int n)
        {
            arsignl.N = n;
            textBox1.Text = n.ToString();
        }
    }
}
