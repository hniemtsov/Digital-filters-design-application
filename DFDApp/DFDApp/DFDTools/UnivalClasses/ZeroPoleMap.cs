using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace DFDLab.DFDTools
{
    public class PoleZeroArray
    {
        public double[] ZRe;
        public double[] ZIm;
        public double[] PRe;
        public double[] PIm;
        public double ggain;
    }
    public class ZeroPoleMap : WraperISCMS
    {
        public ZeroPoleMap(AbstractFilter filter)
            : base(filter)
        {
            Name = "tSBZeroPole";
            Image = "ZeroPole.bmp";
            Chart = WraperISCMS.TypeChart.Polar;
            Text = "Zero/Pole Map";
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (filter.computecoef(this))
            {
                if ((filter.compute_zp!=null) && filter.compute_zp(this))
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        //e.Result = filter.pzArray;
                        resUlt = filter.pzArray;
                    }
                }
                else
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    throw new Exception("Error in zero-pole computation!");
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
