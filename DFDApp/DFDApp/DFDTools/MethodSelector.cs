using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace DFDLab.DFDTools
{   
    [DefaultEvent("IsSelected")]
    public partial class MethodSelector : UserControl
    {
        public delegate void ButtonOKEventHandler(bool IsEnable, string text);
        public event ButtonOKEventHandler IsSelected;
        XDocument xdoc;
        string strFilter;
        public MethodSelector()
        {
            InitializeComponent();
            XmlTextReader reader = new XmlTextReader(GetType().Assembly.GetManifestResourceStream("DFDLab.DFDTools.Config.xml"));
            xdoc = XDocument.Load(reader);
        }
        
        [Category("Data"), Description("String represent methods")]
        public string FilterName
        {
            set
            {
                strFilter = value;
            }
            get
            {
                return strFilter;
            }
        }
        
        public void filterSelector1_FillList(object sender, EventArgs e)
        {
            TreeViewEventArgs args = e as TreeViewEventArgs;
            if (IsSelected != null) IsSelected(false,"");
            this.listView1.Items.Clear();
            var query = from meaple in xdoc.Descendants()
                        where (string)meaple.Attribute("Text") == args.Node.Text
                        from add in meaple.Descendants("MFILE")
                        select new
                        {
                            FileName = add.Attribute("File").Value,
                            ImgIco = add.Element("IMG").Attribute("SRC").Value,
                            Descr = add.Element("DSCR").Attribute("Text").Value
                        };
            int i = 0;
            ImageList img = new ImageList();
            DescrTxt.Clear();
            foreach (var iten in query)
            {
                string srt = "DFDLab.DFDTools.FromAnalogIco." + iten.ImgIco;
                System.IO.Stream strem = GetType().Assembly.GetManifestResourceStream(srt);
                if (strem != null)
                {
                    img.Images.Add(new Icon(strem));
                    this.listView1.Items.Add(new ListViewItem(iten.FileName, i));
                    i++;
                }
                DescrTxt.Add(iten.Descr);

            }

            listView1.View = View.LargeIcon;

            img.ImageSize = new Size(32, 32);
            listView1.LargeImageList = img;
            
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsSelected != null)
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    IsSelected(true,DescrTxt[listView1.SelectedIndices[0]]);
                }
                else
                {
                    IsSelected(false,"");
                }
            }
        }
        public string GetSelectedMethod()
        {
            string Ret = null;
            if (listView1.SelectedItems.Count != 0)
            {
                Ret = listView1.SelectedItems[0].Text;
            }
            return Ret;
        }
        private List<string> DescrTxt = new List<string>();
    }
}
