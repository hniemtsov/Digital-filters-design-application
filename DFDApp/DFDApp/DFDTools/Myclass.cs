using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DFDLab.DFDTools
{
    public class myclas
    {
        public string Name;
        public Bitmap Valuu;
        public string paramName;
        public myclas(string Name, string paramName, Bitmap Value)
        {
            this.Name = Name;
            this.Valuu = Value;
            this.paramName = paramName;
        }
        public string nName
        {
            get
            {
                return Name;
            }
        }
        public Bitmap nValuu
        {
            get
            {
                return Valuu;
            }
        }
    }
}
