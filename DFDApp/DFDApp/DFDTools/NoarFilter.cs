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
using System.Runtime.InteropServices;
using System.Globalization;

namespace DFDLab.DFDTools
{
    public class NoarFilter : AbstractFilter
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 noarK(WraperISCMS.CallBackCancel callback, double[] ar, Int32 n, double std1, double std2, double[] K, Int32 Len);

        private double std1;
        private double std2;
        private string arcoef;
        private double[] K = new double[500];

        public NoarFilter()
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            //compute_zp += new ComputeCoef(GetNumDenCoef);
            Order = 3;
            arcoef = "-1.9550 0.9999 -0.0013";
            Fs = 1000;
            std1 = 0.0071;
            std2 = 0.1;
            NumCoef = new double[1];
            NumCoef[0] = 1;

            TypeFilt = "optimal";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "stsp"; //state space
            SetFilterProperty();
        }

        public NoarFilter(Dictionary<string, AbstractFilter> filters_form) : this()
        {
            this.flrs_form = filters_form;
        }

        public string ArCoef
        {
            get { return arcoef; }
            set { arcoef = value; }
        }

        public double STD1
        {
            get { return std1; }
            set { std1 = value; }
        }

        public double STD2
        {
            get { return std2; }
            set { std2 = value; }
        }

        protected bool GetNumDenCoef(WraperISCMS bw)
        {
            if (updated_coef == false)
            {
                Int32 Filterlength = ((Int32)Order) + 1; 
                int ret = 0;
               /*
                switch (TypeFilt)
                {
                    case "low":
                        Filterlength = ((Int32)Order) + 1;
                        break;
                    case "high":
                        ttype = 1;
                        Filterlength = ((Int32)Order) + 1;
                        break;
                    case "bandpass":
                        ttype = 2;
                        Filterlength = 2 * (Int32)Order + 1;
                        break;
                    case "stop":
                        ttype = 3;
                        Filterlength = (2 * (Int32)Order) + 1;
                        break;
                    default:
                        break;
                }
                */
                //NumCoef = new double[h];
                MatchCollection ArCfts = Regex.Matches(ArCoef, "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                double[] ar = new double[ArCfts.Count];
                DenCoef = new double[ArCfts.Count+1];
                DenCoef[0] = 1;
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                for (int i = 0; i < ArCfts.Count; i++)
                {
                    DenCoef[1+i] = Convert.ToDouble(ArCfts[i].ToString(), provider);
                }
                /*
                pzArray = new PoleZeroArray();
                pzArray.ZRe = new double[NumCoef.Length - 1];
                pzArray.ZIm = new double[NumCoef.Length - 1];
                pzArray.PRe = new double[DenCoef.Length - 1];
                pzArray.PIm = new double[DenCoef.Length - 1];
                */
                //ret = noarK(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), ar, Order, STD1, STD2, K, 500);
                ret = 0;

                //UZPM = (ret == 0);

                updated_coef = (ret == 0);
            }
            return updated_coef;
        }
        
        public bool ValidOrder()
        {
            bool Ret = false;
            if (Order < 13 && Order >= 2)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidSTD(double std)
        {
            bool Ret = false;
            if (std > 0)
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
        public bool ValidCoef()
        {
            bool Ret = false;
            //if (Sigma > 0)
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
                case "stop":
                case "bandpass":
                case "stsp":
                    Ret = true;
                    break;
                default:
                    break;
            }
            return Ret;
        }
        public override bool CheckValidParam()
        {
            bool Ret = false;
            Ret = ValidOrder() & ValidSTD(std1) & ValidSTD(std2) & ValidFs() & ValidTypeFilt() & ValidCoef();
            return Ret;
        }
        protected override void SetFilterProperty()
        {
        }
    }
}
