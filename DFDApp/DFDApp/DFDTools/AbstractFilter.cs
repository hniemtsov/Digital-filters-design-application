using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DFDLab.DFDTools;
using System.Threading;
using System.Collections;
using System.IO;

namespace DFDLab.DFDTools
{

    public class SOSection
    {
        public double[] b;
        public double[] a;
        public SOSection()
        {
            this.b = new double[3];
            this.a = new double[3];
        }

    }

    public class FilterRealization
    {
        private String sos_order;
        private String sos_scale;
        private String sos_struct;
        private double gain;
        
        public List<SOSection> H = new List<SOSection>();

        public FilterRealization()
        {
        }

        public string SOSOrder
        {
            get { return sos_order; }
            set { sos_order = value; }
        }
        public string SOSScale
        {
            get { return sos_scale; }
            set { sos_scale = value; }
        }

        public string SOSStruct
        {
            get { return sos_struct; }
            set { sos_struct = value; }
        }
        public double Gain
        {
            get { return gain; }
            set { gain = value; }
        }
    }
    abstract public class AbstractFilter : IDisposable
    {
        

        private double fs;
        private double[] numcoef;
        private double[] dencoef;

        private double[] input_signal;
        private double[] output_signal;

        [XmlIgnore]
        public AbstractSignal UsSi;
        [XmlIgnore]
        public AbstractSignal NoSi;

        [XmlIgnore]
        public PoleZeroArray pzArray;

        private int order_num;
        private int order_den;

        protected bool updated_coef = false;
        protected bool updated_zpm = false;
        public bool filtered_sig = false;

        private String typefilt;

        private double f1;
        private double f2;

        private string filter_name;

        public AbstractFilter()
        {
            //SetFilterProperty();
            realization = new FilterRealization();
        }
        public delegate void CollectParam();
        public delegate bool ComputeCoef(WraperISCMS bw);
        public delegate void ParamChanged();
        public delegate void ComputeHandler(string taskName, bool flag);

        public delegate void VailidateHadler(bool flag);

        public delegate void CmbxVal(string name);
        


        //public delegate bool ComputeSignal(WraperISCMS bw);

        [XmlIgnore]
        public ParamChanged SetBorderAll;

        [XmlIgnore]
        public CmbxVal AddRow;
        [XmlIgnore]
        public CmbxVal RemoveRow;

        [XmlIgnore]
        public VailidateHadler SetValidate;
        [XmlIgnore]
        public ComputeCoef computecoef = null;
        [XmlIgnore]
        public ComputeCoef compute_zp = null;
        [XmlIgnore]
        public ParamChanged paramchanged = null;
        [XmlIgnore]
        public ComputeHandler compute = null;
        [XmlIgnore]
        public string computeflag = null;

        //[XmlIgnore]
        //public ComputeSignal computesignl = null;

        [XmlIgnore]
        public string ComputeFlag
        {
            get { return computeflag; }
            set { computeflag = value;
            if (compute != null)
            {
                foreach (ComputeHandler comph in compute.GetInvocationList())
                {
                    comph(computeflag, value.Equals("none"));//всех подписавшихся изыестить об изменении параметров 
                }
            }
            }
        }

        public string TypeFilt
        {
            get { return typefilt; }
            set { typefilt = value; }
        }

        public string Name
        {
            get { return filter_name; }
            set { filter_name = value; }
        }

        public double Fs
        {
            get { return fs; }
            set { fs = value; }
        }
        public double F1
        {
            get { return f1; }
            set { f1 = value; }
        }
        public double F2
        {
            get { return f2; }
            set { f2 = value; }
        }

        public double[] NumCoef
        { 
            get { return numcoef; }
            set { numcoef = value; }
        }
        public double[] DenCoef
        {
            get { return dencoef; }
            set { dencoef = value; }
        }
        [XmlIgnore]
        public double[] InSignal
        {
            get { return input_signal; }
            set { input_signal = value; }
        }
        [XmlIgnore]
        public double[] OutSignal
        {
            get { return output_signal; }
            set { output_signal = value; }
        }

        public int Order
        {
            get { return order_num; }
            set { order_num = value; }
        }
        public int OrderDen
        {
            get { return order_den; }
            set { order_den = value; }
        }

        [XmlIgnore]
        public bool UZPM
        {
            get { return updated_zpm; }
            set { updated_zpm = value; }
        }

        public FilterRealization realization;

        public void SetUpdatedCoef()
        {
            updated_coef = false;
            filtered_sig = false;
            if (paramchanged != null)
            {
                foreach (ParamChanged pch in paramchanged.GetInvocationList())
                {
                    pch();//всех подписавшихся изыестить об изменении параметров 
                }
            }
        }

        public string GetFilterDescr()
        {
            StringBuilder ret_str = new StringBuilder("");

            switch (TypeFilt)
            {
                case "low":
                    ret_str.AppendLine(" LPF: " + F1 + "Hz with Fs =" + Fs + "Hz");
                    break;
                case "high":
                    ret_str.AppendLine(" HPF: " + F1 + "Hz with Fs =" + Fs + "Hz");
                    break;
                case "bandpass":
                    ret_str.AppendLine(" BPF: " + F1 + "Hz .. " + F2 + "Hz");
                    break;
                case "stop":
                    ret_str.AppendLine(" BSF: " + F1 + "Hz .. " + F2 + "Hz");
                    break;
                default:
                    break;
            }

            return ret_str.ToString();
        }


        public abstract bool CheckValidParam();
        protected abstract void SetFilterProperty();
        public Dictionary<string, WraperISCMS> listFilterProp = new Dictionary<string,WraperISCMS>();

        public void Dispose()
        {
           /* if (prcs.HasExited == false)
            {
                prcs.Kill();
                prcs.WaitForExit();
            }
            prcs.Close();
            prcs.Dispose();*/
        }
        [XmlIgnore]
        public Dictionary<string, AbstractFilter> flrs_form;
        [XmlIgnore]
        public Dictionary<string, string> SignalList = new Dictionary<string, string>();
    }
}
