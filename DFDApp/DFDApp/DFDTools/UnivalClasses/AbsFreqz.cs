using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace DFDLab.DFDTools
{
    public class AbsFreqz : WraperISCMS
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 magnitude(WraperISCMS.CallBackCancel callback, double[] Num, double[] Den, Int32 NumLen, Int32 NumDen, double[] A, Int32 nPoint);

        public AbsFreqz(AbstractFilter filter)
            : base(filter)
        {
            Name  = "tSBMagnitude";
            Image = "Magnitude";
            Chart = WraperISCMS.TypeChart.OneLine;
            Text  = "Amplitude frequency response";
            Set_dX();
        }
        public void Set_dX()
        {
            dX = filter.Fs / 2.0 / 1024.0;
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (filter.computecoef(this))
            {
                int ret;
                double[] MagCoef = new double[1024];
                    
                ret = magnitude(this.IsCancelChaeck, filter.NumCoef, filter.DenCoef, filter.NumCoef.Length, filter.DenCoef.Length, MagCoef, MagCoef.Length);

                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (ret < 0)
                {
                    throw new Exception("Error in magnitude computation!");
                }
                else
                {
                    //e.Result = MagCoef;
                    resUlt = MagCoef;
                }
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
