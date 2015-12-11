using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace DFDLab.DFDTools
{
    abstract public class WraperISCMS
    {
        public double MinY
        {
            get;
            set;
        }
        public double MaxY
        {
            get;
            set;
        }
        public double dX
        {
            get;
            set;
        }
        public enum TypeChart
        {
            OneLine,
            TwoLine,
            Polar,
            TextArea
        }
        public TypeChart Chart
        {
            get;
            set;
        }
        public string Name
        {
            get { return frName; }
            set {frName = value; }
        }
        public string Image
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public delegate void UpdateVisible(Type tp, string tabName);
        public event UpdateVisible SetVisibl;

        public delegate void SetStopHandler(string tabName);
        public event SetStopHandler SetStop;

        public delegate void SetBorderHandler(string tabName, bool flag);
        public event SetBorderHandler SetBorder;
        
        public delegate void FillControlHandler(object content, string btnName);
        public event FillControlHandler FillControl;

        public delegate bool CallBackCancel();

        protected bool updated = false;

        protected StringBuilder Output = null;

        public WraperISCMS(AbstractFilter filter)
        {
            this.filter = filter;
            MinY = 0;
            MaxY = 1;
            dX = 1;
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }
        protected object resUlt = null;
        public void RunISC()
        {
            if (updated == false)
            {
                if (SetStop != null)
                {
                    SetStop(frName);
                }
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                if (FillControl != null)
                {
                    //FillControl(e.Result, frName);
                    FillControl(resUlt, frName);
                }
                if (SetVisibl != null)
                {
                    SetVisibl(GetType(), frName);
                }
            }
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

            }
            else if (e.Error != null)
            {
                string msg = String.Format("An error occurred: {0}", e.Error.Message);
                MessageBox.Show(msg);
                if (SetBorder != null)
                {
                    SetBorder(frName, true);
                }
            }
            else 
            {
                {
                    updated = true;
                    if (FillControl != null)
                    {
                        //FillControl(e.Result, frName);
                        FillControl(resUlt, frName);
                    }
                    if (SetVisibl != null)
                    {
                        SetVisibl(GetType(), frName);
                    }
                }
            }
        }
        public abstract void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e);

        public bool IsCancelChaeck()
        {
            return this.backgroundWorker1.CancellationPending;
        }

        public void SetUpdated()
        {
            updated = false;// запускать или нет DoWork(). переключиться на нужный экран или перед этим запустить ещё DoWork()
        }

        protected string strParseNum = "\\-?[0-9]+\\.?[0-9]*E?[\\+|\\-]?[0-9]*\\b";
        protected AbstractFilter filter;
        public string frName = null;
        public BackgroundWorker backgroundWorker1;
    }
}
