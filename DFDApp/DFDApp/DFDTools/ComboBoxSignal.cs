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
    public partial class ComboBoxSignal : ComboBox
    {
        ImageList img = new ImageList();

        public ComboBoxSignal()
        {//Time of duration
            
            Items.Add("Sine function");
            Items.Add("Heart signal");
            Items.Add("White noise");
            

            DrawMode = DrawMode.OwnerDrawFixed;

            string srt = "DFDLab.DFDTools.SignalIcon.Sine.bmp";
            System.IO.Stream strem = GetType().Assembly.GetManifestResourceStream(srt);
            img.Images.Add("Sine function",new Bitmap(strem));

            srt = "DFDLab.DFDTools.SignalIcon.WhiteN.bmp";
            strem = GetType().Assembly.GetManifestResourceStream(srt);
            img.Images.Add("White noise",new Bitmap(strem));

            srt = "DFDLab.DFDTools.SignalIcon.WhiteN.bmp";
            strem = GetType().Assembly.GetManifestResourceStream(srt);
            img.Images.Add("White noise", new Bitmap(strem));

            SelectedIndex = 0;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Font fnt = new Font(FontFamily.GenericSerif, 10, FontStyle.Regular);
            if ((e.Index != -1) && (e.Index < 3))
            {
                e.Graphics.DrawImage(img.Images[e.Index], e.Bounds.X, e.Bounds.Y);
                if (e.State == DrawItemState.Selected)
                {
                    e.Graphics.DrawString("     " + Items[e.Index], fnt, Brushes.White, e.Bounds.X, e.Bounds.Y);
                }
                else
                {
                    e.Graphics.DrawString("     " + Items[e.Index], fnt, Brushes.Black, e.Bounds.X, e.Bounds.Y);
                }
            }

        }
    }
}
