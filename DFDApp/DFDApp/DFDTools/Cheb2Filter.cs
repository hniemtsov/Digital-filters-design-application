﻿using System;
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
    public class Cheb2Filter : AbstractFilter
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 cheby2(WraperISCMS.CallBackCancel callback, Int32 n, double w1, double w2, double R, double[] Num, double[] Den, double[] ZRe, double[] ZIm, double[] PRe, double[] PIm, ref double ga, Int32 type);
        
        private double r;

        public Cheb2Filter()
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            compute_zp += new ComputeCoef(GetNumDenCoef);
            Order = 4;
            Fs = 1000;
            F1 = 300;
            F2 = 350;
            R = 40;
            TypeFilt = "low";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
            SetFilterProperty();
        }

        public Cheb2Filter(Dictionary<string, AbstractFilter> filters_form) : this()
        {
            this.flrs_form = filters_form;
        }

        public double R
        {
            get { return r; }
            set { r = value; }
        }

        protected bool GetNumDenCoef(WraperISCMS bw)
        {
            if (updated_coef == false)
            {
                Int32 Filterlength = 0;
                Int32 ttype = 0;
                int ret = 0;

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

                NumCoef = new double[Filterlength];
                DenCoef = new double[Filterlength];
                pzArray = new PoleZeroArray();
                pzArray.ZRe = new double[NumCoef.Length - 1];
                pzArray.ZIm = new double[NumCoef.Length - 1];
                pzArray.PRe = new double[DenCoef.Length - 1];
                pzArray.PIm = new double[DenCoef.Length - 1];

                ret = cheby2(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), ((Int32)Order), 2.0 * F1 / Fs, 2.0 * F2 / Fs, R, NumCoef, DenCoef, pzArray.ZRe, pzArray.ZIm, pzArray.PRe, pzArray.PIm, ref pzArray.ggain, ttype);

                UZPM = (ret == 0);

                updated_coef = (ret == 0);
            }
            return updated_coef;
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
        public bool ValidRipple()
        {
            bool Ret = false;
            if (R > 0)
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
            Ret = ValidOrder() & ValidRipple() & ValidFs() & ValidTypeFilt();
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
