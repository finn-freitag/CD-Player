using EasyCodeClass.Multimedia.Audio.Windows.CDRom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace CD_Player
{
    public partial class Form1 : Form
    {
        SoundPlayer audio = new SoundPlayer();

        CompactDisc[] discs;

        CompactDisc currentDisc;

        string path = "";
        string wavePath = "";
        string filenameFile = "";

        int trackcount = 0;

        bool isLoading = false;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged; // Add this event after component initializing to prevent null reference exceptions
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CDPlayer\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            filenameFile = path + "filename.dat";
            if (!File.Exists(filenameFile)) File.WriteAllText(filenameFile, "Audio_%TRACK%");
            wavePath = path + "files\\";
            if (!Directory.Exists(wavePath)) Directory.CreateDirectory(wavePath);

            LoadDiscs();
        }

        private void LoadDiscs()
        {
            openToolStripMenuItem.DropDownItems.Clear();
            toolStripDropDownButton1.DropDownItems.Clear();
            discs = CompactDisc.GetAllDiscs();
            for(int i = 0; i < discs.Length; i++)
            {
                ToolStripMenuItem cd_item = new ToolStripMenuItem();
                cd_item.Text = "(" + discs[i].DriveLetter + "://) " + discs[i].Name;
                cd_item.ToolTipText = cd_item.Text;
                cd_item.Tag = i;
                cd_item.Click += DiscClicked;
                openToolStripMenuItem.DropDownItems.Add(cd_item);
                cd_item = new ToolStripMenuItem();
                cd_item.Text = "(" + discs[i].DriveLetter + "://) " + discs[i].Name;
                cd_item.ToolTipText = cd_item.Text;
                cd_item.Tag = i;
                cd_item.Click += DiscClicked;
                toolStripDropDownButton1.DropDownItems.Add(cd_item);
            }
        }

        private void DiscClicked(object sender, EventArgs e)
        {
            currentDisc = discs[(int)((ToolStripMenuItem)sender).Tag];
            trackcount = currentDisc.TrackCount;

            dataGridView1.Rows.Clear();

            toolStripProgressBar1.Maximum = trackcount;
            toolStripProgressBar1.Value = 0;
            toolStrip4.Visible = true;

            StopAndDisposeAudio();
            ClearDirectory(wavePath);

            Thread t = new Thread(new ThreadStart(StartLoading));
            t.Priority = ThreadPriority.AboveNormal;
            t.Name = "Load tracks...";
            isLoading = true;
            t.Start();
        }

        private void StartLoading()
        {
            try
            {
                currentDisc.GetData(TrackReaded);
            }
            catch
            {
                // thrown if the program was terminated during track loading
            }
        }

        private void TrackReaded(TrackBytesReadEventArgs e)
        {
            e.trackNumber++;
            File.WriteAllBytes(wavePath + e.trackNumber + ".wav", e.track);
            e.track = null;

            dataGridView1.Invoke(new Action(() => {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[dataGridView1.Columns["Selected"].Index].Value = false;
                row.Cells[dataGridView1.Columns["Track"].Index].Value = "" + e.trackNumber;
                row.Cells[dataGridView1.Columns["Artist"].Index].Value = "";
                row.Cells[dataGridView1.Columns["Genre"].Index].Value = "";
                row.Cells[dataGridView1.Columns["Album"].Index].Value = "";
                row.Cells[dataGridView1.Columns["Title"].Index].Value = "";
                row.Cells[dataGridView1.Columns["Keywords"].Index].Value = "";
                row.Cells[dataGridView1.Columns["Comment"].Index].Value = "";
                dataGridView1.Rows.Add(row);

                toolStripProgressBar1.Value = e.trackNumber;
                if (!e.hasMoreTracks)
                {
                    toolStrip4.Visible = false;
                    isLoading = false;
                }
            }));
        }

        private void StopAndDisposeAudio()
        {

        }

        private void SaveTrackAs(int trackNum, string path)
        {
            
        }

        private string GeneratePath(int trackNum)
        {
            int rowindex = GetRowIndex(trackNum);
            string filename = File.ReadAllText(filenameFile);
            filename = filename.Replace("%TRACK%", Convert.ToString(trackNum));
            filename = filename.Replace("%ARTIST%", (string)dataGridView1.Rows[rowindex].Cells[dataGridView1.Columns["Artist"].Index].Value);
            filename = filename.Replace("%GENRE%", (string)dataGridView1.Rows[rowindex].Cells[dataGridView1.Columns["Genre"].Index].Value);
            filename = filename.Replace("%ALBUM%", (string)dataGridView1.Rows[rowindex].Cells[dataGridView1.Columns["Album"].Index].Value);
            filename = filename.Replace("%TITLE%", (string)dataGridView1.Rows[rowindex].Cells[dataGridView1.Columns["Title"].Index].Value);
            filename = filename.Replace("%COMMENT%", (string)dataGridView1.Rows[rowindex].Cells[dataGridView1.Columns["Comment"].Index].Value);
            return filename + ".wav";
        }

        private int GetRowIndex(int trackNum)
        {
            int rowindex = trackNum;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32((string)row.Cells[dataGridView1.Columns["Track"].Index].Value) == trackNum)
                {
                    rowindex = row.Index;
                    break;
                }
            }
            return rowindex;
        }

        private void Equalize(int column)
        {
            string first = "";
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if ((string)row.Cells[column].Value != "") first = (string)row.Cells[column].Value;
            }
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[column].Value = first;
            }
        }

        private void ClearDirectory(string dir)
        {
            foreach(string file in  Directory.GetFiles(dir))
            {
                File.Delete(file);
            }
        }

        private void saveAudioAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Location for audio...";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    int tracknum = Convert.ToInt32((string)row.Cells[dataGridView1.Columns["Track"].Index].Value);
                    SaveTrackAs(tracknum, fbd.SelectedPath + "\\" + GeneratePath(tracknum));
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (currentDisc != null && !isLoading)
            {
                currentDisc.Eject();
                currentDisc = null;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            int selectedBoxIndex = dataGridView1.Columns["Selected"].Index;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[selectedBoxIndex].Value = false;
            }
            foreach(DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                dataGridView1.Rows[cell.RowIndex].Cells[selectedBoxIndex].Value = true;
                foreach(DataGridViewCell c in dataGridView1.Rows[cell.RowIndex].Cells)
                {
                    c.Selected = true;
                }
            }
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*if(e.ColumnIndex == dataGridView1.Columns["Selected"].Index)
            {
                dataGridView1.SelectionChanged -= dataGridView1_SelectionChanged;
                dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
                bool selected = (string)dataGridView1[e.RowIndex, e.ColumnIndex].Value == "selected";
                foreach(DataGridViewCell cell in dataGridView1.Rows[e.RowIndex].Cells)
                {
                    cell.Selected = selected;
                }
                dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
            }*/
        }

        private void artistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Equalize(dataGridView1.Columns["Artist"].Index);
        }

        private void genreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Equalize(dataGridView1.Columns["Genre"].Index);
        }

        private void albumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Equalize(dataGridView1.Columns["Album"].Index);
        }

        private void keywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Equalize(dataGridView1.Columns["Keywords"].Index);
        }

        private void commentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Equalize(dataGridView1.Columns["Comment"].Index);
        }
    }
}
