using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

namespace DFDLab.DFDTools
{
    public class TestFilter
    {
        public double[] Distorted;
        public double[] Filtered;
    }
    public class TestSignal : WraperISCMS
    {

        [DllImport("DFilter.dll")]
        private static extern Int32 do_filter(WraperISCMS.CallBackCancel callback, double[] Num, double[] Den, Int32 NumLen, Int32 NumDen, double[] In, double[] Out, Int32 N);

        [DllImport("DFilter.dll")]
        private static extern Int32 noar(WraperISCMS.CallBackCancel callback, double[] ar, Int32 n, double std1, double std2, double[] x, double[] y, Int32 Len);

        public TestSignal(AbstractFilter filter, string strName)
            : base(filter)
        {
            Name = strName;
        }
        /*
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            int i = 0;
            int j;
            int ret = 0;

            string[] keesar = filter.SignalList.Keys.ToArray<string>();

            if (filter.flrs_form[keesar[0]].computecoef(this))
            {
                bool flag = (!filter.flrs_form[keesar[0]].UsSi.IsSignal) || (!filter.flrs_form[keesar[0]].NoSi.IsSignal);
                if ((filter.flrs_form[keesar[0]].filtered_sig == false)
                 || flag)
                {
                    for (i = 0; i < keesar.Length; i++)
                    {
                        filter.flrs_form[keesar[i]].filtered_sig = false;
                    }
                    if (filter.flrs_form[keesar[0]].UsSi.GetSamples(this)
                     && filter.flrs_form[keesar[0]].NoSi.GetSamples(this))
                    {
                        if (flag)
                        {
                            if (!filter.flrs_form[keesar[0]].UsSi.Samples.Equals(filter.flrs_form[keesar[0]].InSignal))
                            {
                                filter.flrs_form[keesar[0]].InSignal = filter.flrs_form[keesar[0]].UsSi.Samples;
                            }
                            for (i = 0; i < filter.flrs_form[keesar[0]].InSignal.Length; i++)
                            {
                                filter.flrs_form[keesar[0]].InSignal[i] += filter.flrs_form[keesar[0]].NoSi.Samples[i];
                            }
                        }
                    }
                    else
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        throw new Exception("Error in signal generating.");
                    }
                    //coefs were generated successfuly
                    if (filter.flrs_form[keesar[0]].filtered_sig == false)
                    {
                        if ((filter.flrs_form[keesar[0]].OutSignal == null) || (filter.flrs_form[keesar[0]].OutSignal.Length < filter.flrs_form[keesar[0]].UsSi.N))
                        {
                            filter.flrs_form[keesar[0]].OutSignal = new double[filter.flrs_form[keesar[0]].UsSi.N];
                        }
                        ret = do_filter(this.IsCancelChaeck, filter.flrs_form[keesar[0]].NumCoef, filter.flrs_form[keesar[0]].DenCoef, filter.flrs_form[keesar[0]].NumCoef.Length, filter.flrs_form[keesar[0]].DenCoef.Length, filter.flrs_form[keesar[0]].InSignal, filter.flrs_form[keesar[0]].OutSignal, filter.flrs_form[keesar[0]].OutSignal.Length);
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        if (ret < 0)
                        {
                            throw new Exception("Error in signal filtering !");
                        }
                        else
                        {
                            filter.flrs_form[keesar[0]].filtered_sig = true;
                        }
                    }
                }
            }//if (filter.flrs_form[keesar[0]].computecoef(this))
            else
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                throw new Exception("Error in filter coefficients compute");
            }//end if (filter.flrs_form[keesar[0]].computecoef(this))

            for (j = 1; j < keesar.Length; j++)
            {
                if (filter.flrs_form[keesar[j]].filtered_sig == false)
                {
                    break;
                }
            }

            for (i = j; i < keesar.Length; i++)
            {
                if (filter.flrs_form[keesar[i]].computecoef(this))
                {
                    
                    if (!filter.flrs_form[keesar[i - 1]].OutSignal.Equals(filter.flrs_form[keesar[i]].InSignal))
                    {
                        filter.flrs_form[keesar[i]].InSignal = filter.flrs_form[keesar[i - 1]].OutSignal;
                    }
                    if (!filter.flrs_form[keesar[i]].NoSi.IsSignal)
                    {
                        filter.flrs_form[keesar[i]].NoSi.GetSamples(this);
                        for (int k = 0; k < filter.flrs_form[keesar[i]].InSignal.Length; k++)
                        {
                            filter.flrs_form[keesar[i]].InSignal[k] = filter.flrs_form[keesar[i]].UsSi.Samples[k] + filter.flrs_form[keesar[i]].NoSi.Samples[k];
                        }
                    }
                    if ((filter.flrs_form[keesar[i]].OutSignal == null) || (filter.flrs_form[keesar[i]].OutSignal.Length < filter.flrs_form[keesar[i]].InSignal.Length))
                    {
                        filter.flrs_form[keesar[i]].OutSignal = new double[filter.flrs_form[keesar[i]].InSignal.Length];
                    }
                    ret = do_filter(this.IsCancelChaeck, filter.flrs_form[keesar[i]].NumCoef, filter.flrs_form[keesar[i]].DenCoef, filter.flrs_form[keesar[i]].NumCoef.Length, filter.flrs_form[keesar[i]].DenCoef.Length, filter.flrs_form[keesar[i]].InSignal, filter.flrs_form[keesar[i]].OutSignal, filter.flrs_form[keesar[i]].OutSignal.Length);
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (ret < 0)
                    {
                        throw new Exception("Error in signal filtering !");
                    }
                    else
                    {
                        filter.flrs_form[keesar[i]].filtered_sig = true;
                    }
                }
                else
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    throw new Exception("Error in filter coefficients compute");
                }
            }//end circle
            if (ret < 0)
            {
                throw new Exception("Error in signal filtering !");
            }
            else
            {
                e.Result = filter.OutSignal;
            }                      
  
        }*/
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            int i = 0;
            int j;
            int ret = 0;

            string[] keesar = filter.SignalList.Keys.ToArray<string>();

            if (filter.flrs_form[keesar[0]].computecoef(this))
            {
                bool flag = false;
                if ((!filter.flrs_form[keesar[0]].UsSi.IsSignal) ||
                    (!filter.flrs_form[keesar[0]].NoSi.IsSignal))
                {
                    flag = true;
                    if (filter.flrs_form[keesar[0]].UsSi.GetSamples(this) 
                     && filter.flrs_form[keesar[0]].NoSi.GetSamples(this))
                    {
                        if ((filter.flrs_form[keesar[0]].InSignal == null) || (filter.flrs_form[keesar[0]].InSignal.Length < filter.flrs_form[keesar[0]].UsSi.Samples.Length))
                        {
                            filter.flrs_form[keesar[0]].InSignal = new double[filter.flrs_form[keesar[0]].UsSi.Samples.Length];
                        }
                        for (int k = 0; k < filter.flrs_form[keesar[0]].UsSi.Samples.Length; k++)
                        {
                            filter.flrs_form[keesar[0]].InSignal[k] = filter.flrs_form[keesar[0]].UsSi.Samples[k] + filter.flrs_form[keesar[0]].NoSi.Samples[k];
                        }
                        filter.flrs_form[keesar[0]].filtered_sig = false;
                    }
                    else
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        throw new Exception("Error in signal generating.");
                    }
                }
                if (filter.flrs_form[keesar[0]].filtered_sig == false)
                {
                    flag = true;
                    if ((filter.flrs_form[keesar[0]].OutSignal == null) || (filter.flrs_form[keesar[0]].OutSignal.Length < filter.flrs_form[keesar[0]].InSignal.Length))
                    {
                        filter.flrs_form[keesar[0]].OutSignal = new double[filter.flrs_form[keesar[0]].InSignal.Length];
                    }
                    if (filter.flrs_form[keesar[i]].TypeFilt.Equals("Optimal"))
                    {
                        double[] ar = new double[filter.flrs_form[keesar[0]].DenCoef.Length-1];
                        Array.Copy(filter.flrs_form[keesar[0]].DenCoef,1,ar,0,ar.Length);
                        ret = noar(this.IsCancelChaeck, ar, ar.Length, ((NoarFilter)filter.flrs_form[keesar[0]]).STD1, ((NoarFilter)filter.flrs_form[keesar[0]]).STD2, filter.flrs_form[keesar[0]].InSignal, filter.flrs_form[keesar[0]].OutSignal, filter.flrs_form[keesar[0]].OutSignal.Length);
                    }
                    else
                    {
                        ret = do_filter(this.IsCancelChaeck, filter.flrs_form[keesar[0]].NumCoef, filter.flrs_form[keesar[0]].DenCoef, filter.flrs_form[keesar[0]].NumCoef.Length, filter.flrs_form[keesar[0]].DenCoef.Length, filter.flrs_form[keesar[0]].InSignal, filter.flrs_form[keesar[0]].OutSignal, filter.flrs_form[keesar[0]].OutSignal.Length);
                    }
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (ret < 0)
                    {
                        throw new Exception("Error in signal filtering !");
                    }
                    else
                    {
                        filter.flrs_form[keesar[0]].filtered_sig = true;
                    }
                }
                if (flag)
                {
                    for (i = 1; i < keesar.Length; i++)
                    {
                        filter.flrs_form[keesar[i]].filtered_sig = false;
                    }
                }
            }//if (filter.flrs_form[keesar[0]].computecoef(this))
            else
            {
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                throw new Exception("Error in filter coefficients compute");
            }//end if (filter.flrs_form[keesar[0]].computecoef(this))

            for (j = 1; j < keesar.Length; j++)
            {
                if (filter.flrs_form[keesar[j]].filtered_sig == false)
                {
                    break;
                }
                if ((filter.flrs_form[keesar[j]].UsSi != null)
                 && (!filter.flrs_form[keesar[j]].UsSi.IsSignal))
                {
                    break;
                }
                if ((filter.flrs_form[keesar[j]].NoSi != null)
                 && (!filter.flrs_form[keesar[j]].NoSi.IsSignal))
                {
                    break;
                }
            }
////////////////////////////////////////////////////////////////////////////////////////
            for (i = j; i < keesar.Length; i++)
            {
                if (filter.flrs_form[keesar[i]].computecoef(this))
                {
                    if (filter.flrs_form[keesar[i]].NoSi.GetSamples(this))
                    {
                        if ((filter.flrs_form[keesar[i]].InSignal == null) || (filter.flrs_form[keesar[i]].InSignal.Length < filter.flrs_form[keesar[i - 1]].OutSignal.Length))
                        {
                            filter.flrs_form[keesar[i]].InSignal = new double[filter.flrs_form[keesar[i - 1]].OutSignal.Length];
                        }
                        for (int k = 0; k < filter.flrs_form[keesar[i-1]].OutSignal.Length; k++)
                        {
                            filter.flrs_form[keesar[i]].InSignal[k] = filter.flrs_form[keesar[i-1]].OutSignal[k] + filter.flrs_form[keesar[i]].NoSi.Samples[k];
                        }
                    }
                    else
                    {
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        throw new Exception("Error in signal generating.");
                    }
                    if (filter.flrs_form[keesar[i]].filtered_sig == false)
                    {
                        if ((filter.flrs_form[keesar[i]].OutSignal == null) || (filter.flrs_form[keesar[i]].OutSignal.Length < filter.flrs_form[keesar[i]].InSignal.Length))
                        {
                            filter.flrs_form[keesar[i]].OutSignal = new double[filter.flrs_form[keesar[i]].InSignal.Length];
                        }
                        if (filter.flrs_form[keesar[i]].TypeFilt.Equals("optimal"))
                        {

                        }
                        else
                        {
                            ret = do_filter(this.IsCancelChaeck, filter.flrs_form[keesar[i]].NumCoef, filter.flrs_form[keesar[i]].DenCoef, filter.flrs_form[keesar[i]].NumCoef.Length, filter.flrs_form[keesar[i]].DenCoef.Length, filter.flrs_form[keesar[i]].InSignal, filter.flrs_form[keesar[i]].OutSignal, filter.flrs_form[keesar[i]].OutSignal.Length);
                        }
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        if (ret < 0)
                        {
                            throw new Exception("Error in signal filtering !");
                        }
                        else
                        {
                            filter.flrs_form[keesar[i]].filtered_sig = true;
                        }
                    }
                }
                else
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    throw new Exception("Error in filter coefficients compute");
                }
            }//end circle
            if (ret < 0)
            {
                throw new Exception("Error in signal filtering !");
            }
            else
            {
                //e.Result = filter.OutSignal;
                resUlt = filter.OutSignal;
            }

        }
    }
}
