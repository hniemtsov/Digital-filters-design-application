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
    public partial class BPSKSignalTab : GeneralSignalTab
    {
        //public delegate void VailidateHadler(bool flag);
        //public event VailidateHadler SetValidate;

        public BPSKSignal signl;
        NumberFormatInfo provider = new NumberFormatInfo();
                
        private int validFg;

        public BPSKSignalTab(AbstractSignal signl)
        {
            InitializeComponent();
            validFg = 0;
            provider.NumberDecimalSeparator = ".";
            this.signl = (BPSKSignal)signl;

            textBox1.Text = this.signl.TimeD.ToString();
            textBox2.Text = this.signl.Fs.ToString();
            textBox3.Text = this.signl.Fc.ToString();
            textBox4.Text = this.signl.Bs;
            textBox5.Text = this.signl.Br.ToString();

            this.Dock = DockStyle.Fill;
        }
        public BPSKSignalTab(AbstractSignal signl, Boolean flag) : this(signl)
        {
            textBox1.Enabled = false;
            provider.NumberDecimalSeparator = ".";
        }

        private bool ValidTb1()
        {
            bool Ret = false;
            if (textBox1.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.Order != dtmp)
                {
                    signl.SetUpdatedSignal();
                    signl.TimeD = System.Convert.ToDouble(textBox1.Text, provider);
                    signl.N = System.Convert.ToInt32(signl.TimeD * signl.Fs * 1000.0 + 1.0);
                }
                Ret = signl.ValidTimeD();
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
                    signl.SetUpdatedSignal();
                    signl.Fs = Convert.ToDouble(textBox2.Text, provider);
                }
                Ret = signl.ValidFs();

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
                    signl.SetUpdatedSignal();
                    signl.Fc = Convert.ToDouble(textBox3.Text, provider);
                }
                Ret = signl.ValidFc();

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
                    signl.SetUpdatedSignal();
                    signl.Bs = textBox4.Text;
                }
                Ret = signl.ValidBs();

            }
            validFg = (validFg & ~(1 << 3)) | (Convert.ToInt32(Ret == false) << 3);
            return Ret;
        }
        private bool ValidTb5()
        {
            bool Ret = false;
            if (textBox5.Text.CompareTo("") != 0)
            {
                //if (ceb1fil.R != dtmp)
                {
                    signl.SetUpdatedSignal();
                    signl.Br = Convert.ToDouble(textBox5.Text, provider);
                }
                Ret = signl.ValidBr();

            }
            validFg = (validFg & ~(1 << 4)) | (Convert.ToInt32(Ret == false) << 4);
            return Ret;
        }

        private void textBox_1_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb1();
            if (flagValidate == true)
            {
                RunSetNParam(signl.N);
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
        private void textBox_5_TextChanged(object sender, EventArgs e)
        {
            bool flagValidate = ValidTb5();
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
            signl.N = n;
            signl.TimeD = (signl.N - 1) * (1.0 / (signl.Fs*1000.0));
            textBox1.Text = signl.TimeD.ToString();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
