using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DFDLab.DFDTools
{
    public class ARSignal : AbstractSignal
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 ar_model(WraperISCMS.CallBackCancel callback, Int32 n, double std, double[] ar, double[] mem, Int32 lenar, double[] smpls);

        private double std;
        private string arcoef;
        private string mem;

        public ARSignal()
        {
            std = 0.1;
            N = 100;
            arcoef = "1 -0.9 0.2 -0.05";
            mem = "0 0 0";
        }
        public ARSignal(int NN)
        {
            std = 0.1;
            N = NN;
            arcoef = "1 -0.9 0.2 -0.05";
            mem = "0 0 0";
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

                MatchCollection ArCfts = Regex.Matches(ArCoef, "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                MatchCollection MeM = Regex.Matches(Initial, "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                
                double[] ar = new double[ArCfts.Count];
                double[] mem = new double[ArCfts.Count-1];

                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                for (int i = 0; i < ArCfts.Count; i++ )
                {
                    ar[i] = Convert.ToDouble(ArCfts[i].ToString(), provider);
                }
                for (int i = 0; i < MeM.Count; i++)
                {
                    mem[i] = Convert.ToDouble(MeM[i].ToString(), provider);
                }

                ret = ar_model(bw.IsCancelChaeck, N, Sigma, ar, mem, ar.Length, Samples);
             
                updated_signal = (ret == 0);
            }
            return updated_signal;
        }

        public string ArCoef
        {
            get { return arcoef; }
            set { arcoef = value; }
        }
        public string Initial
        {
            get { return mem; }
            set { mem = value; }
        }

        public double Sigma
        {
            get { return std; }
            set { std = value; }
        }

        public bool ValidCoef()
        {
            bool Ret = false;
            //if (Sigma > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidMem()
        {
            bool Ret = false;
            //if (Sigma > 0)
            {
                Ret = true;
            }
            return Ret;
        }

        public bool ValidSigma()
        {
            bool Ret = false;
            if (Sigma > 0)
            {
                Ret = true;
            }
            return Ret;
        }

    }
}
