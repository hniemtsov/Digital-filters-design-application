using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFDLab.DFDTools
{
    public class GeneralSignalTab : UserControl
    {
        public delegate void VailidateHadler(bool flag);
        public event VailidateHadler SetValidate;
        
        public delegate void SetNHadler(int N);
        public event SetNHadler SetNparam;

        private int validFg;


        public GeneralSignalTab()
        {
            validFg = 0;

                    }

        public void RunSetNParam(int n)
        {
            if (SetNparam != null)
            {
                SetNparam(n);
            }
        }

        public void RunSetValidate(bool flag)
        {
            if (SetValidate != null)
            {
                SetValidate(flag);
            }
        }

        protected void textBox_TextChanged_add(object sender, bool flag)
        {
            TextBox tb = sender as TextBox;
            if (flag == false)
            {
                tb.BackColor = Color.MistyRose;
            }
            else
            {
                tb.BackColor = Color.White;
            }
            RunSetValidate(validFg == 0);
        }
    }
}
