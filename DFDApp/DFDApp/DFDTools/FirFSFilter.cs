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

namespace DFDLab.DFDTools
{
    public class FirFSFilter : AbstractFilter
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 firfs(WraperISCMS.CallBackCancel callback, Int32 len, double[] fs, double[] A, Int32 lenA, Int32 nFFT, Int32 symet, double[] coef);
        public double[] des_mag = new double[250];
        public double[] edgeFqs = new double[250];
        public int nDFT;
        private int num_frq;

        //private StringBuilder Output = null;

        public FirFSFilter()
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            Order = 8;
            Fs = 2;
            NumF = 4;
            edgeFqs[0] = 0.0;
            edgeFqs[1] = 0.5;
            edgeFqs[2] = 0.6;
            edgeFqs[3] = 1.0;
            nDFT = 512;
            des_mag[0] = 1; des_mag[1] = 1;
            des_mag[2] = 0; des_mag[3] = 0;
            TypeFilt = "type1";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
            DenCoef = new double[1];
            DenCoef[0] = 1;
        }
        public FirFSFilter(Dictionary<string, AbstractFilter> filters_form) : this()
        {
            this.flrs_form = filters_form;
        }
        public int NumF
        {
            get { return num_frq; }
            set { num_frq = value; }
        }

        protected bool GetNumDenCoef(WraperISCMS bw)
        {

            if (updated_coef == false)
            {
                int ret = 0;
                NumCoef = new double[Order + 1];

                ret = firfs(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), Order + 1, edgeFqs, des_mag, NumF, nDFT, 0, NumCoef);
                
                updated_coef = (ret == 0);
            }
            return updated_coef;
        }
        public bool ValidOrder()
        {
            bool Ret = true;
            if (Order >= 250 || Order < 2)
            {
                Ret = false;
            }
            switch (TypeFilt)
            {
                case "type1":
                case "type3":
                    Ret &= Convert.ToBoolean((Order+1) & 1);
                    break;
                case "type2":
                case "bandpass":
                    Ret &= (false == Convert.ToBoolean((Order + 1) & 1));
                    break;
                default:
                    break;
            }

            return Ret;
        }
        public bool ValidDesrdMag(int count)
        {
            bool Ret = true;
            if (count != NumF)
            {
                Ret = false;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (des_mag[i] < 0 || des_mag[i] > 30)
                    {
                        Ret = false;
                        break;
                    }
                }
            }
            return Ret;
        }

        public bool ValidnDFT()
        {
            bool Ret = true;
            if ((nDFT &0x01) == 1)
            {
                Ret = false;
            }
            return Ret;
        }
 
        public bool ValidFs()
        {
            bool Ret = false;
            if (Fs >= 2 && (Math.Round(Fs) - Fs) == 0)
            {
                Ret = true;
            }
            return Ret;
        }
        public bool ValidEdgeFqs(int count)
        {
            bool Ret = true;
            for (int i = 1; i < count; i++)
            {
                if ((edgeFqs[i - 1] >= edgeFqs[i]) || (edgeFqs[i - 1] >= Fs/2))
                {
                    Ret = false;
                    break;
                }
            }
            if (edgeFqs[count - 1] > Fs / 2) Ret = false;
            return Ret;
        }
        public override bool CheckValidParam()
        {
            bool Ret = false;
            Ret = ValidOrder() & ValidDesrdMag(NumF) & ValidFs() & ValidEdgeFqs(NumF) & ValidnDFT();
            return Ret;
        }
        protected override void SetFilterProperty()
        {
        }
    }
}
