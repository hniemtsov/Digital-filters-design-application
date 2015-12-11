using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace DFDLab.DFDTools
{
    public class CodeGen : TF2SOS
    {
        public CodeGen(AbstractFilter filter, string strName)
            : base(filter, strName)
        {
        }

        public override void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder strCode = new StringBuilder("");
            base.backgroundWorker1_DoWork(sender, e);
            
            
            if (!e.Cancel)
            {
                SOSection h;
                strCode.AppendLine("/*");
                strCode.Append("   IIR");
                switch(filter.TypeFilt)
                {
                    case "low":
                        strCode.AppendLine(" LPF: "+filter.F1+"Hz with Fs =" + filter.Fs+"Hz");
                        break;
                    case "high":
                        strCode.AppendLine(" HPF: "+filter.F1+"Hz with Fs =" + filter.Fs+"Hz");
                        break;
                    case "bandpass":
                        strCode.AppendLine(" BPF: "+filter.F1+"Hz .. " + filter.F2+"Hz");
                        break;
                    case "stop":
                        strCode.AppendLine(" BSF: "+filter.F1+"Hz .. " + filter.F2+"Hz");
                        break;
                    default:
                        break;
                }
                strCode.AppendLine("   Transfer function of filter realize as a cascaded series of second-order biquad sections");
                strCode.AppendLine("   Each biquad section is implemented in direct form 2");
                strCode.AppendLine("   Copyright (C) 2011 DFDLab ");
                strCode.AppendLine("*/ ");
                strCode.Append("\r\n\r\n");
                strCode.AppendLine("float mem[" + filter.realization.H.Count + "][3];");
                strCode.Append("static const float Bcoef["+filter.realization.H.Count+"][3] = {");
                for (int i = 0; i < filter.realization.H.Count-1; i++)
                {
                    h = filter.realization.H[i] as SOSection;
                    strCode.AppendFormat("{{ {0,5:F5}f, {1,5:F5}f, {2,5:F5}f }}, ", h.b[0], h.b[1], h.b[2]);
                }
                h = filter.realization.H[filter.realization.H.Count - 1] as SOSection;
                strCode.AppendFormat("{{ {0,5:F5}f, {1,5:F5}f, {2,5:F5}f }} }};\r\n", h.b[0], h.b[1], h.b[2]);
                strCode.Append("static const float Acoef[" + filter.realization.H.Count + "][3] = {");
                for (int i = 0; i < filter.realization.H.Count-1; i++)
                {
                    h = filter.realization.H[i] as SOSection;
                    strCode.AppendFormat("{{ {0,5:F5}f, {1,5:F5}f, {2,5:F5}f }}, ", h.a[0], h.a[1], h.a[2]);
                }
                h = filter.realization.H[filter.realization.H.Count - 1] as SOSection;
                strCode.AppendFormat("{{ {0,5:F5}f, {1,5:F5}f, {2,5:F5}f }} }};\r\n ", h.a[0], h.a[1], h.a[2]);

                strCode.Append("\r\n");
                strCode.AppendLine("void iir_filter(const float *in, // filter input signal");
                strCode.AppendLine("                float *out,      // filter output signal");
                strCode.AppendLine("                int N)           // length of input signal");
                strCode.AppendLine("{");
                strCode.AppendLine("   int i;");
                strCode.AppendLine("   const float *x = in;");
                strCode.AppendLine("   float *y = out;");
                strCode.AppendLine("   float xi, yi;\r\n");
                strCode.AppendLine("   for (i = 0; i < N; i++)");
                strCode.AppendLine("   {");
                strCode.AppendFormat("      yi = {0:F10}f*x[i];\r\n",filter.realization.Gain);
                for (int i = 0; i < filter.realization.H.Count; i++)
                {
                    strCode.AppendLine("      xi = yi;");
                    strCode.AppendFormat("      yi = xi*Bcoef[{0}][0] + mem[{0}][0];\r\n",i);
                    strCode.AppendFormat("      mem[{0}][0] = mem[{0}][1] + xi*Bcoef[{0}][1] - yi*Acoef[{0}][1];\r\n", i);
                    strCode.AppendFormat("      mem[{0}][1] = xi*Bcoef[{0}][2] - yi*Acoef[{0}][2];\r\n", i);
                }
                strCode.AppendLine("      y[i] = yi;");
                strCode.AppendLine("   }");
                strCode.AppendLine("");
                strCode.AppendLine("   return;");
                strCode.AppendLine("}");
                //e.Result = strCode.ToString();
                resUlt = strCode.ToString();
            }

        }
    }
}
