using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DFDLab.DFDTools
{
    public class BPSKSignal : AbstractSignal
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 bpsk_model(WraperISCMS.CallBackCancel callback, Int32 n, double fc, double fs, double br,  double[] bs, Int32 len_bs, double[] smpls);

        private double fc; // frequency carrier in kHz
        private string bs; // bit stream
        private double time_dur;//time duration
        private double fs; //frequency sampling in kHz
        private double br; //bit rate in kbit/sec

        public BPSKSignal()
        {
            fc = 250.0;
            N = 100;
            fs = 30.0*fc;
            bs = "1 0 1 1 1 0 0 1 1 0 1 0 1"; // kod Barkera N=13
            time_dur = (N - 1) * (1.0 / (fs*1000.0));
            Br = 2.0 * fc / 2.0;
        }

        public BPSKSignal(int NN) 
        {
            fc = 250.0;
            N = NN;
            fs = 30.0 * fc;
            bs = "1 1 1 1 1 0 0 1 1 0 1 0 1"; // kod Barkera N=13
            time_dur = (N - 1) * (1.0 / (fs*1000.0));
            Br = 2.0 * fc / 2.0/10.0;
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

                MatchCollection Mbs = Regex.Matches(Bs, "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                double[] dbs = new double[Mbs.Count];
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                for (int i = 0; i < Mbs.Count; i++)
                {
                    dbs[i] = 2.0*Convert.ToDouble(Mbs[i].ToString(), provider) - 1.0;
                }

                ret = bpsk_model(bw.IsCancelChaeck, N, fc, fs, Br, dbs, dbs.Length, Samples);
             
                updated_signal = (ret == 0);
            }
            return updated_signal;
        }

        public double Fc
        {
            get { return fc; }
            set { fc = value; }
        }
        public double Fs
        {
            get { return fs; }
            set { fs = value; }
        }
        public double TimeD
        {
            get { return time_dur; }
            set { time_dur = value; }
        }
        public string Bs
        {
            get { return bs; }
            set { bs = value; }
        }
        public double Br
        {
            get { return br; }
            set { br = value; }
        }

        public bool ValidFc()
        {
            bool Ret = false;
            if (Fc < Fs)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidFs()
        {
            bool Ret = false;
            if (Fs > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidBs()
        {
            bool Ret = false;
            //if (Fs > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidBr()
        {
            bool Ret = false;
            //if (Fs > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidTimeD()
        {
            bool Ret = false;
            //if (Fs > 0)
            {
                Ret = true;
            }
            return Ret;
        }
    }
}
