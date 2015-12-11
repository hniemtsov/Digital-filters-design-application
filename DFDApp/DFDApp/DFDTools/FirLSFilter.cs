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
    public class FirLSFilter : AbstractFilter
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 firls(WraperISCMS.CallBackCancel callback, double[] pfFilter, int N, int nSymet, double[] pfFs, double[] pfMag, double[] pfWeights, int nLenpfFs);

        public double[] des_mag = new double[250];
        public double[] edgeFqs = new double[250];
        public double[] weights = new double[250];
        private int num_frq;

        //private StringBuilder Output = null;

        public FirLSFilter()
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            //compute_zp += new ComputeCoef(GetNumDenCoef); TODO
            Order = 4;
            Fs = 2;
            NumF = 4;
            edgeFqs[0] = 0.0; edgeFqs[1] = 0.2;
            edgeFqs[2] = 0.3; edgeFqs[3] = 1.0;

            weights[0] = 1;    weights[1] = 1;
            des_mag[0] = 1; des_mag[1] = 1;
            des_mag[2] = 0; des_mag[3] = 0;
            TypeFilt = "Symmetric and Odd Length ";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
            DenCoef = new double[1];
            DenCoef[0] = 1;
            SetFilterProperty();
        }
        public FirLSFilter(Dictionary<string, AbstractFilter> filters_form) : this()
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
                Int32 nSymet = 0;
                int ret = 0;

                switch (TypeFilt)
                {
                    case "Symmetric and Odd Length ":
                    case "Symmetric and Even Length":
                        nSymet = 0;
                        break;
                    case "Antisymmetric Odd Length ":
                    case "Antisymmetric Even Length":
                        nSymet = 1;
                        break;
                    default:
                        break;
                }
                NumCoef = new double[(Int32)Order + 1];
                ret = firls(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), NumCoef, (Int32)Order + 1, nSymet, edgeFqs, des_mag, weights, (NumF/2));
                //    private static extern Int32 firls(WraperISCMS.CallBackCancel callback, double[] pfFilter, int nSymet, int N, double[] pfFs, int nLenpfFs, double[] pfMag, double[] pfWeights);
                UZPM = (ret != 0);
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
                case "Symmetric and Odd Length ":
                case "Antisymmetric Odd Length ":
                    Ret = Convert.ToBoolean((Order+1) & 1);
                    break;
                case "Symmetric and Even Length":
                case "Antisymmetric Even Length":
                    Ret = (false == Convert.ToBoolean((Order + 1) & 1));
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
        public bool ValidWeights(int count)
        {
            bool Ret = true;
            if (count != (NumF>>1))
            {
                Ret = false;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (weights[i] < 0.1 || weights[i] > 50)
                    {
                        Ret = false;
                        break;
                    }
                }
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
            if (1 == (count & 0x01))
            {
                Ret = false;
            }
            else
            {
                for (int i = 1; i < count; i++)
                {
                    if ((edgeFqs[i - 1] >= edgeFqs[i]) || (edgeFqs[i - 1] > Fs / 2))
                    {
                        Ret = false;
                        break;
                    }
                }
            }
            if (edgeFqs[count - 1] > Fs / 2) Ret = false;
            return Ret;
        }
        public override bool CheckValidParam()
        {
            bool Ret = false;
            Ret = ValidOrder() & ValidDesrdMag(NumF) & ValidFs() & ValidEdgeFqs(NumF) & ValidWeights((NumF>>1));
            return Ret;
        }
        protected override void SetFilterProperty()
        {
            AbsFreqz magnitude = new AbsFreqz(this);
            ImpzResp impulse = new ImpzResp(this);
            ZeroPoleMap zpmap = new ZeroPoleMap(this);
            TF2TXT tf2txt = new TF2TXT(this);

            listFilterProp.Add(magnitude.Name, magnitude);
            listFilterProp.Add(impulse.Name, impulse);
            listFilterProp.Add(zpmap.Name, zpmap);
            listFilterProp.Add(tf2txt.Name, tf2txt);
            this.paramchanged += new AbstractFilter.ParamChanged(magnitude.SetUpdated);
            this.paramchanged += new AbstractFilter.ParamChanged(impulse.SetUpdated);
            this.paramchanged += new AbstractFilter.ParamChanged(zpmap.SetUpdated);
            this.paramchanged += new AbstractFilter.ParamChanged(tf2txt.SetUpdated);
        }
    }
}
