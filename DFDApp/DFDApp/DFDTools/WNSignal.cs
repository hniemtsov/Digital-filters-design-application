using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DFDLab.DFDTools
{
    public class WNSignal : AbstractSignal
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 wn_model(WraperISCMS.CallBackCancel callback, Int32 n, double std, double m, double[] smpls);

        private double std;
        private double mean;

        public WNSignal()
        {
            std = 0.0;
            N = 100;
            mean = 0.0;
        }
        public WNSignal(int NN)
        {
            std = 0.0;
            N = NN;
            mean = 0.0;
        }

        public override bool GetSamples(WraperISCMS bw)
        {
            if (updated_signal == false)
            {
                int ret = 0;
                if ((Samples == null) || (Samples.Length < N))
                {
                    Samples = new double[N];
                }

                ret = wn_model(bw.IsCancelChaeck, N, Sigma, Mean, Samples);
             
                updated_signal = (ret == 0);
            }
            return updated_signal;
        }

        public double Sigma
        {
            get { return std; }
            set { std = value; }
        }
        public double Mean
        {
            get { return mean; }
            set { mean = value; }
        }
        public bool ValidSigma()
        {
            bool Ret = false;
            if (Sigma >= 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidMean()
        {
            bool Ret = false;
            //if (Mean > 0)
            {
                Ret = true;
            }
            return Ret;
        }
    }
}
