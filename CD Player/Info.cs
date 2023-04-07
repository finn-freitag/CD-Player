using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CD_Player
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
            label1.Text = AssemblyInfoHelper.GetTitle();
            label4.Text = AssemblyInfoHelper.GetDescription();
            label3.Text = AssemblyInfoHelper.GetCompany() + " " + AssemblyInfoHelper.GetCopyright();
            int index = 1;
            foreach(KeyValuePair<string,string> pair in AssemblyInfoHelper.GetImageCredits())
            {
                Label l = new Label();
                l.Text = pair.Key;
                l.Tag = pair.Value;
                l.Left = 35;
                l.Top = 140 + 30 * index;
                l.ForeColor = Color.Blue;
                l.Font = new Font(l.Font, FontStyle.Underline);
                l.Click += creditsClicked;
                this.Controls.Add(l);
                index++;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Process.Start(AssemblyInfoHelper.GetURL());
        }

        private void creditsClicked(object sender, EventArgs e)
        {
            Process.Start((string)((Label)sender).Tag);
        }
    }
}
