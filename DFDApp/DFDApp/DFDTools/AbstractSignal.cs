using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DFDLab.DFDTools
{
    abstract public class AbstractSignal : IDisposable
    {
        
        private double[] samples = null;
        private int n;

        protected bool updated_signal = false;

        public AbstractSignal()
        {
        }
        
        public double[] Samples
        {
            get { return samples; }
            set { samples = value; }
        }
        
        public int N
        {
            get { return n; }
            set { n = value; }
        }

        public bool IsSignal
        {
            get { return updated_signal; }
            set { updated_signal = value; }
        }

        public abstract bool GetSamples(WraperISCMS bw);

        public void SetUpdatedSignal()
        {
            updated_signal = false;
           /* if (paramchanged != null)
            {
                foreach (ParamChanged pch in paramchanged.GetInvocationList())
                {
                    pch();//всех подписавшихся изыестить об изменении параметров 
                }
            }
            **/
        }

        public bool ValidN()
        {
            bool Ret = false;
            if (N > 0)
            {
                Ret = true;
            }
            return Ret;
        }
        

        public void Dispose()
        {
        
        }
    }
}
