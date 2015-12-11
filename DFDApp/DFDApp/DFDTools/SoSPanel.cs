using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DFDLab.DFDTools
{
    public partial class SoSPanel : UserControl
    {
        //public event EventHandler paramchanged;

        public SoSPanel(SOSRealization sosrz, AbstractFilter filter)
        {
            InitializeComponent();

            this.sosrz = sosrz;
            this.ceb1fil = filter;
            this.Dock = DockStyle.Fill;

            listitem.Add(new myclas("direct form 1", "df1", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.SOSRealizationBMP.DF1.bmp"))));
            listitem.Add(new myclas("direct form 2", "df2", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.SOSRealizationBMP.DF2.bmp"))));
            listitem.Add(new myclas("direct form 1 transposed", "df1t", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.SOSRealizationBMP.DF1.bmp"))));
            listitem.Add(new myclas("direct form 2 transposed", "df2t", new Bitmap(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.SOSRealizationBMP.DF2.bmp"))));
            
            comboBox3.DataSource = listitem;
            comboBox3.ValueMember = "nValuu";
            comboBox3.DisplayMember = "nName";
            PictureBox pb = sosrz.Controls["groupBox1"].Controls["groupBox2"].Controls["pBtSBSpecification"] as PictureBox;
            comboBox3.DataBindings.Add("SelectedValue", pb, "Image");
            comboBox3.DataBindings[0].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            foreach (myclas mc in listitem)
            {
                if (mc.nName.Equals(this.ceb1fil.realization.SOSStruct))
                {
                    pb.Image = ((myclas)listitem[comboBox3.FindString(mc.nName)]).nValuu;
                    break;
                }
            }
            
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            
        }
        private SOSRealization sosrz;
        private AbstractFilter ceb1fil;
        private ArrayList listitem = new ArrayList();

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           ceb1fil.realization.SOSOrder = comboBox1.SelectedItem.ToString();
           sosrz.SetUpdatedInParam();
           sosrz.SetEnableBtn(true);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ceb1fil.realization.SOSScale = comboBox2.SelectedItem.ToString();
            sosrz.SetUpdatedInParam();
            sosrz.SetEnableBtn(true);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ceb1fil.realization.SOSStruct = ((myclas)comboBox3.SelectedItem).nName;
            sosrz.SetUpdatedInParam();
            sosrz.SetEnableBtn(true);
        }
    }
}
