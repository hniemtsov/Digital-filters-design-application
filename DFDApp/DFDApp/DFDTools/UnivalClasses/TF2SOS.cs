using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace DFDLab.DFDTools
{
    public class TF2SOS : WraperISCMS
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 zp2sos(WraperISCMS.CallBackCancel callback, Int32 nZ, double[] ZRe, double[] ZIm, Int32 nP, double[] PRe, double[] PIm, Int32 order, Int32 scale, ref double Gain, double[] zk, ref Int32 nzk, double[] pk, ref Int32 npk);

        public TF2SOS(AbstractFilter filter, string strName)
            : base(filter)
        {
            Name = strName;
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (filter.computecoef(this))
            {
                Int32 sos_order;
                Int32 sos_scale;
                switch (filter.realization.SOSOrder)
                {
                    case "up":
                        sos_order = 0;
                        break;
                    case "down":
                        sos_order = 1;
                        break;
                    default:
                        sos_order = -1;
                        break;
                }
                switch (filter.realization.SOSScale)
                {
                    case "none":
                        sos_scale = 0;
                        break;
                    case "inf":
                        sos_scale = 1;
                        break;
                    case "two":
                        sos_scale = 2;
                        break;
                    default:
                        sos_scale = -1;
                        break;
                }
                double gain = 1.0;
                double[] zk = new double[1024];
                double[] pk = new double[1024];
                Int32 nzk = 0;
                Int32 npk = 0;

                int ret = zp2sos(this.IsCancelChaeck, filter.pzArray.ZRe.Length, filter.pzArray.ZRe, filter.pzArray.ZIm, filter.pzArray.PRe.Length, filter.pzArray.PRe, filter.pzArray.PIm, sos_order, sos_scale, ref gain, zk, ref nzk, pk, ref npk);
                gain = filter.pzArray.ggain;
                String str = "";
                Int32 L = Convert.ToInt32(Math.Floor(Math.Log(Convert.ToDouble(filter.pzArray.PRe.Length), 2.0)));
                Int32 M = filter.pzArray.PRe.Length - (1 << L);
                /*if (L + M != (npk/3))
                {
                    ret = -1;
                }*/
                if (ret < 0)
                {
                    throw new Exception("Error in zp2sos()!");
                }
                filter.realization.H.Clear();
                for (int i = 0; i < (npk / 3); i++)
                {
                    SOSection H = new SOSection();
                    str = str + "H" + i.ToString() + "\r\n";
                    for (int j = 0; j < 3; j++)
                    {
                        H.b[j] = zk[j + 3 * i];
                        str = str + "  b[" + j.ToString() + "] = " + H.b[j] + "\r\n";
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        H.a[j] = pk[j + 3 * i];
                        str = str + "  a[" + j.ToString() + "] = " + H.a[j] + "\r\n";
                    }
                    filter.realization.H.Add(H);
                }
                filter.realization.Gain = gain;
                str = str + "K = " + filter.realization.Gain + "\r\n";

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
