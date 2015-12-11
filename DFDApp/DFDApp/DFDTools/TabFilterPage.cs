using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFDLab.DFDTools
{
    public class TabFilterPage : TabPage
    {
        public SplitContainer SCPage;
        public TabFilterPage(string Name)
        {
            SCPage = new SplitContainer();

            SCPage.Dock = DockStyle.Fill;
            SCPage.SplitterDistance = 75;
            SCPage.Name = "split_container_page";
            SCPage.Panel1.AutoScroll = true;

            this.Text = Name;
            this.Controls.Add(SCPage);
        }
    }
}
