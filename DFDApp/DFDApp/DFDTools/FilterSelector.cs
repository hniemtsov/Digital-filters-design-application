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
    [DefaultEvent("FillList")]
    public partial class FilterSelector : UserControl
    {
        public event EventHandler FillList;
        public FilterSelector()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (FillList != null)
            {
                FillList(this, e);
            }
        }
    }
}
