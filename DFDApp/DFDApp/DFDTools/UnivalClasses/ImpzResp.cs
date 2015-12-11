using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DFDLab.DFDTools
{
    public class ImpzResp : WraperISCMS
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 impresp(WraperISCMS.CallBackCancel callback, double[] Num, double[] Den, Int32 NumLen, Int32 NumDen, double[] A, Int32 nPoint);

        public ImpzResp(AbstractFilter filter)
            : base(filter)
        {
            Name = "tSBImpulse";
            Image = "Impulse2.bmp";
            Chart = WraperISCMS.TypeChart.OneLine;
            Text = "Impulse Response";
            MinY = -1;
            dX = 1;
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            //if (updated == false)
            {
                if (filter.computecoef(this))
                {
                    int ret;
                    int nImpRsp = 128;
                    
                    if (filter.DenCoef.Length == 1)
                    {
                        nImpRsp = filter.NumCoef.Length;
                    }
                    double[] ImpCoef = new double[nImpRsp];
                    ret = impresp(this.IsCancelChaeck, filter.NumCoef, filter.DenCoef, filter.NumCoef.Length, filter.DenCoef.Length, ImpCoef, nImpRsp);

                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        //KillerISCMC();
                        return;
                    }
                    if (ret < 0)
                    {
                        throw new Exception("Error in impulse resp computation!");
                    }
                    else
                    {
                        resUlt = ImpCoef;
                        //e.Result = ImpCoef;
                    }
                }
                else
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        //KillerISCMC();
                        return;
                    }
                    throw new Exception("Error in filter coefficient compute");
                }
            }
            /*
            else
            {
                e.Cancel = true;
                return;
            }
             * */
        }
    }
}
