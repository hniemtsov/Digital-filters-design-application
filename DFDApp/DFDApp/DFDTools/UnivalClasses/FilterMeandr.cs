using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace DFDLab.DFDTools
{
    public class FilterMeandr : WraperISCMS
    {
        public FilterMeandr(AbstractFilter filter)
            : base(filter)
        {
        }
        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            //this.prcs = filter.prcs;

            //if ((updated == false) /*|| (filter.UpdatedParam == false)*/)
            {

                if (/*filter.computecoef(bw)*/true)
                {
                    Output = new StringBuilder("");
                    String argumentText = "[NaB] = filter([";
                    for (int i = 0; i < filter.NumCoef.Length - 1; i++)
                    {
                        argumentText = argumentText + filter.NumCoef[i] + " ";
                    }
                    argumentText = argumentText + filter.NumCoef[filter.NumCoef.Length - 1] + "],[";
                    for (int i = 0; i < filter.DenCoef.Length - 1; i++)
                    {
                        argumentText = argumentText + filter.DenCoef[i] + " ";
                    }
                    argumentText = argumentText + filter.DenCoef[filter.NumCoef.Length - 1] + "],[zeros(1,100) ones(1,100) zeros(1,100) ones(1,100)])'\n";
                    /*
                    prcs.OutputDataReceived += new DataReceivedEventHandler(prcs_DataReceived);
                    prcs.BeginOutputReadLine();

                    StreamWriter myStreamWriter = prcs.StandardInput;
                    myStreamWriter.Write("'Begin" + frName + "'\n");
                    myStreamWriter.Write(argumentText);
                    myStreamWriter.Write("'End" + frName + "'\n");
                    */
                    //filter.mySignal.WaitOne();

                    if (bw.CancellationPending)
                    {
                        //prcs.CancelOutputRead();
                        //prcs.OutputDataReceived -= new DataReceivedEventHandler(prcs_DataReceived);
                        e.Cancel = true;
                        return;
                    }
                    MatchCollection Match = Regex.Matches(Output.ToString(), "\\bNaB =[ \\n\\r0-9-.E]*", RegexOptions.ExplicitCapture);
                    //MatchCollection NaB = Regex.Matches(Match[0].ToString(), "\\-?[0-9]+\\.?[0-9]+E?[\\+|\\-]?[0-9]*\\b", RegexOptions.ExplicitCapture);
                    MatchCollection NaB = Regex.Matches(Match[0].ToString(), strParseNum, RegexOptions.ExplicitCapture);

                    double[] MagCoef = new double[NaB.Count];
                    for (int i = 0; i < NaB.Count; i++)
                    {
                        MagCoef[i] = Convert.ToDouble(NaB[i].ToString());
                    }
                    e.Result = MagCoef;

                }
                else
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    //e.Cancel = true;
                    return;
                }
            }
            /*else
            {
                e.Cancel = true;
                return;
            }*/
        }
    }
}
