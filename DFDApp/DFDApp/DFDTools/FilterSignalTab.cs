using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFDLab.DFDTools
{
    public partial class FilterSignalTab : UserControl
    {
        private Dictionary<string, string> sig_dic;
        private Font myFont = new Font("Times New roman", 8);
        private int ScrollY = 0;
        private int ScrollX = 0;

        public FilterSignalTab(Dictionary<string, string> SigDic)
        {
            InitializeComponent();
            sig_dic = SigDic;

            this.Dock = DockStyle.Fill;
        }


        private void FilterSignalTab_Paint(object sender, PaintEventArgs e)
        {
            int i = 30 + ScrollX;
            int width = 130;
            Graphics g = e.Graphics;

            
            foreach (KeyValuePair<string, string> sd in sig_dic)
            {
                g.DrawLine(Pens.Black, i - 25, 100 + ScrollY, i, 100 + ScrollY);
                g.DrawLine(Pens.Black, i - 5, 100 - 5 + ScrollY, i, 100 + ScrollY);
                g.DrawLine(Pens.Black, i - 5, 100 + 5 + ScrollY, i, 100 + ScrollY);
                g.DrawString(sd.Key, myFont, Brushes.Green, new RectangleF(i, 100 - 15 - 15 + ScrollY, width, 31));
                g.DrawString(sd.Value, myFont, Brushes.Green, new RectangleF(i, 100 - 7 + ScrollY, width, 31));
                g.DrawRectangle(Pens.Black, i, 100 - 15 + ScrollY, width, 31);
                i = i + width + 25;
            }
            g.DrawLine(Pens.Black, i - 25, 100 + ScrollY, i, 100 + ScrollY);
            g.DrawLine(Pens.Black, i - 5, 100 - 5 + ScrollY, i, 100 + ScrollY);
            g.DrawLine(Pens.Black, i - 5, 100 + 5 + ScrollY, i, 100 + ScrollY);
            if (i > 450 && ScrollX == 0)
            {
                AutoScrollMinSize = new Size(i, AutoScrollMinSize.Height);
            }
        }

        private void FilterSignalTab_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                ScrollY = -e.NewValue;
            }
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ScrollX = -e.NewValue;
            }

            Invalidate();
        }
    }
}
