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
    public partial class WNsignalTab : GeneralSignalTab //: UserControl
    {
        public WNSignal wnsignl;

        private int validFg;

        public WNsignalTab(AbstractSignal signl)
        {
            InitializeComponent();
            validFg = 0;

            this.wnsignl = (WNSignal)signl;

            textBox1.Text = wnsignl.N.ToString();
            textBox2.Text = wnsignl.Mean.ToString();
            textBox3.Text = wnsignl.Sigma.ToString();

            this.Dock = DockStyle.Fill;
        }
        public WNsignalTab(AbstractSignal signl, Boolean flag)
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
                    wnsignl.SetUpdatedSignal();
                    wnsignl.N = System.Convert.ToInt32(textBox1.Text);
                }
                Ret = wnsignl.ValidN();
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
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";

                    wnsignl.SetUpdatedSignal();
                    wnsignl.Mean = Convert.ToDouble(textBox2.Text, provider);
                }
                Ret = wnsignl.ValidMean();

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
                    wnsignl.SetUpdatedSignal();
                    NumberFormatInfo provider = new NumberFormatInfo();
                    provider.NumberDecimalSeparator = ".";
                    wnsignl.Sigma = Convert.ToDouble(textBox3.Text, provider);
                }
                Ret = wnsignl.ValidSigma();

            }
            validFg = (validFg & ~(1 << 2)) | (Convert.ToInt32(Ret == false) << 2);
            return Ret;
        }
        

        private void textBox_1_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb1();
            if (flagValidate == true)
            {
                RunSetNParam(wnsignl.N);
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
            wnsignl.N = n;
            textBox1.Text = n.ToString();
        }
    }
}
