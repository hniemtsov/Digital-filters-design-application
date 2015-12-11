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
    public class ComputeKorr : WraperISCMS
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 noarK(WraperISCMS.CallBackCancel callback, double[] ar, Int32 n, double std1, double std2, double[] K, Int32 Len);

        public ComputeKorr(AbstractFilter filter)
            : base(filter)
        {
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (filter.computecoef(this))
            {
                int ret;
                double[] korrCoef = new double[500 / ((NoarFilter)filter).Order];
                double[] ar = new double[((NoarFilter)filter).DenCoef.Length-1];
                for (int i = 0; i < ar.Length; i++)
                {
                    ar[i] = ((NoarFilter)filter).DenCoef[i+1];
                }

                    ret = noarK(this.IsCancelChaeck, ar, ((NoarFilter)filter).Order, ((NoarFilter)filter).STD1, ((NoarFilter)filter).STD2, korrCoef, 500);

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
                    e.Result = korrCoef;
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
