using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DFDLab.DFDTools;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Threading;

namespace DFDLab.DFDTools
{
    public class EllipFilter : AbstractFilter
    {

        private double rp;
        private double rs;

        private StringBuilder Output = null;

        public EllipFilter(Dictionary<string, AbstractFilter> filters_form)
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            Order = 4;
            Fs = 1000;
            F1 = 300;
            Rp = 3;
            Rs = 50;
            TypeFilt = "low";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
        }

        public double Rp
        {
            get { return rp; }
            set { rp = value; }
        }
        public double Rs
        {
            get { return rs; }
            set { rs = value; }
        }
        protected override void SetFilterProperty()
        {

        }

        protected bool GetNumDenCoef(WraperISCMS bw)
        {
            Output = new StringBuilder("");
            String argumentText = "[NaB, NbB] = ellip(" + Order.ToString() + "," + Rp.ToString() + ","+ Rs.ToString() + ",";
            if (TypeFilt.Equals("bandpass") || TypeFilt.Equals("stop"))
            {
                argumentText = argumentText + "[" + Convert.ToString(2.0 * F1 / Fs) + " " + Convert.ToString(2.0 * F2 / Fs) + "],'";
            }
            else
            {
                argumentText = argumentText + Convert.ToString(2.0 * F1 / Fs) + ",'";
            }
            argumentText = argumentText + TypeFilt + "'," + "'z');\n";

            if ((updated_coef == false) /*|| (updated_param == false)*/)
            {
                //updated_param = true;
                /*
                prcs.OutputDataReceived += new DataReceivedEventHandler(prcs_CoefReceived);
                prcs.BeginOutputReadLine();

                StreamWriter myStreamWriter = prcs.StandardInput;
                myStreamWriter.Write("'BeginEllip'\n");
                myStreamWriter.Write(argumentText);
                myStreamWriter.Write("NaB=real(NaB)'\n");
                myStreamWriter.Write("NbB=real(NbB)'\n");
                myStreamWriter.Write("'EndEllip'\n");
                */
                //mySignal.WaitOne();

                if (bw.backgroundWorker1.CancellationPending)
                {
                    //prcs.CancelOutputRead();
                    //prcs.OutputDataReceived -= new DataReceivedEventHandler(prcs_CoefReceived);
                    return false;
                }
                MatchCollection Match = Regex.Matches(Output.ToString(), "\\bN.B =[ \\n\\r0-9-.E]*", RegexOptions.ExplicitCapture);
                MatchCollection NaB = Regex.Matches(Match[0].ToString(), "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                MatchCollection NbB = Regex.Matches(Match[1].ToString(), "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                NumCoef = new double[NaB.Count];
                DenCoef = new double[NbB.Count];
                for (int i = 0; i < NaB.Count; i++)
                {
                    NumCoef[i] = Convert.ToDouble(NaB[i].ToString());
                }
                for (int i = 0; i < NbB.Count; i++)
                {
                    DenCoef[i] = Convert.ToDouble(NbB[i].ToString());
                }

                updated_coef = (NaB.Count > 0) & (NbB.Count > 0);
            }

            return updated_coef;
        }

        void prcs_CoefReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                if (e.Data.Contains("EndEllip"))
                {
                    //prcs.CancelOutputRead();
                    //prcs.OutputDataReceived -= new DataReceivedEventHandler(prcs_CoefReceived);
                    //mySignal.Set();
                }
                Output.Append(e.Data);
                Output.Append(" \r\n");
            }
        }
        public bool ValidOrder()
        {
            bool Ret = false;
            if (Order < 13 && Order > 2)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidRippleP()
        {
            bool Ret = false;
            if (Rp > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidRippleS()
        {
            bool Ret = false;
            if ((Rs > 0)&(Rs > Rp))
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidFs()
        {
            bool Ret = false;
            if (Fs > 2 && (Math.Round(Fs) - Fs) == 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidF1()
        {
            bool Ret = false;
            if (F1 < Fs / 2)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidF2()
        {
            bool Ret = false;
            if ((F2 < Fs / 2) & (F1 < F2))
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidTypeFilt()
        {
            bool Ret = false;
            switch (TypeFilt)
            {
                case "low":
                case "high":
                    Ret = ValidF1();
                    break;
                case "stop":
                case "bandpass":
                    Ret = ValidF2();
                    break;
                default:
                    break;
            }
            return Ret;
        }
        public override bool CheckValidParam()
        {
            bool Ret = false;
            Ret = ValidOrder() & ValidRippleP() & ValidRippleS() & ValidFs() & ValidTypeFilt();
            return Ret;
        }
    }
}
