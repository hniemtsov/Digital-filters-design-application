using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DFDLab.DFDTools
{
    public class TF2TXT : WraperISCMS
    {
        public TF2TXT(AbstractFilter filter)
            : base(filter)
        {
            Name = "tSBTF2TXT";
            Image = "NumDenCoef";
            Chart = WraperISCMS.TypeChart.TextArea;
            Text = "Filter coefficients";
        }
        public TF2TXT(AbstractFilter filter, string strName)
            : base(filter)
        {
            this.frName = strName;
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (filter.computecoef(this))
            {
                String str = "";
                str = str + "Numerator coefficients \r\n\r\n";
                for (int i = 0; i < filter.NumCoef.Length; i++)
                {
                    str = str + filter.NumCoef[i].ToString() + "\r\n";
                }
                str = str + "\r\n" + "Denominator coefficients \r\n\r\n";
                for (int i = 0; i < filter.DenCoef.Length; i++)
                {
                    str = str + filter.DenCoef[i].ToString() + "\r\n";
                }
                str = str + "\r\n";

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //e.Result = str.Clone();
                resUlt = str.Clone();
            }
            else
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                throw new Exception("Error in filter coefficient compute");
            }
        }
    }
}
