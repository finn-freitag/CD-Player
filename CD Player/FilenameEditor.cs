using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CD_Player
{
    public partial class FilenameEditor : Form
    {
        string path = "";

        public FilenameEditor(string path)
        {
            InitializeComponent();
            this.path = path;
            textBox1.Text = File.ReadAllText(path);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%ARTIST%");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%TRACK%");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%GENRE%");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%ALBUM%");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%TITLE%");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Paste("%COMMENT%");
        }

        private void FilenameEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(path, textBox1.Text);
        }
    }
}
