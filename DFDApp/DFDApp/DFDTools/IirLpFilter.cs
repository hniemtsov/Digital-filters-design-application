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
    public class IirLpFilter : AbstractFilter
    {
        [DllImport("DFilter.dll")]
        private static extern Int32 iirlpn(WraperISCMS.CallBackCancel callback, Int32 NN, Int32 ND, double[] pfFs, double[] pfEdg, double[] pfMag, double[] pfWeights, Int32 FreqNum, Int32 EdgesNum, Int32 WeigNum, double[] Num, double[] Den, double[] ZRe, double[] ZIm, double[] PRe, double[] PIm, Int32 p, ref double ga);

        public double[] des_mag = new double[250];
        public double[] edgeFqs = new double[250];
        public double[] edges   = new double[250];
        public double[] weights = new double[250];
        private int num_frq;
        private int num_edg;
        public IirLpFilter()
        {
            computecoef += new ComputeCoef(GetNumDenCoef);
            compute_zp += new ComputeCoef(GetNumDenCoef);
            Order = 10;
            OrderDen = 15;
            NumF = 4;
            NumE = 4;
            edgeFqs[0] = 0.0; edgeFqs[1] = 0.49;
            edgeFqs[2] = 0.51; edgeFqs[3] = 1.0;
            edges[0] = 0.0; edges[1]  = 0.49;
            edges[2] = 0.51; edges[3] = 1.0;

            weights[0] = 1;    weights[1] = 1;
            des_mag[0] = 1; des_mag[1] = 1;
            des_mag[2] = 0; des_mag[3] = 0;
            TypeFilt = "Symmetric and Odd Length ";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
            Fs = 2;
            //TypeFilt = "bandpass";
            realization.SOSOrder = "up";
            realization.SOSScale = "none";
            realization.SOSStruct = "direct form 1";
            SetFilterProperty();
        }

        public IirLpFilter(Dictionary<string, AbstractFilter> filters_form): this()
        {
            this.flrs_form = filters_form;
        }

        public int NumF
        {
            get { return num_frq; }
            set { num_frq = value; }
        }
        public int NumE
        {
            get { return num_edg; }
            set { num_edg = value; }
        }
        protected bool GetNumDenCoef(WraperISCMS bw)
        {
            if (updated_coef == false)
            {
                int ret = 0;

                NumCoef = new double[(Int32)Order + 1];
                DenCoef = new double[(Int32)OrderDen + 1];
                pzArray = new PoleZeroArray();
                pzArray.ZRe = new double[NumCoef.Length - 1];
                pzArray.ZIm = new double[NumCoef.Length - 1];
                pzArray.PRe = new double[DenCoef.Length - 1];
                pzArray.PIm = new double[DenCoef.Length - 1];

                ret = iirlpn(new WraperISCMS.CallBackCancel(bw.IsCancelChaeck), ((Int32)Order), ((Int32)OrderDen), edgeFqs, edges, des_mag, weights, NumF, NumE, NumF/2, NumCoef, DenCoef, pzArray.ZRe, pzArray.ZIm, pzArray.PRe, pzArray.PIm, (Int32)2, ref pzArray.ggain);
                
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
        public bool ValidOrderDen()
        {
            bool Ret = false;
            if (OrderDen < 17 && OrderDen > 2)
            {
                Ret = true;
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
            if (count != (NumF >> 1))
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
        public bool ValidEdges(int count)
        {
            bool Ret = true;
                for (int i = 1; i < count; i++)
                {
                    if ((edgeFqs[i - 1] >= edgeFqs[i]) || (edgeFqs[i - 1] > Fs / 2))
                    {
                        Ret = false;
                        break;
                    }
                }
            if (edges[count - 1] > Fs / 2) Ret = false;
            return Ret;
        }
        public override bool CheckValidParam()
        {
            bool Ret = false;
            Ret = ValidOrder() & ValidOrderDen() & ValidDesrdMag(NumF) & ValidFs() & ValidEdgeFqs(NumF) & ValidEdges(NumE) & ValidWeights((NumF >> 1));
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
