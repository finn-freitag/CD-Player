using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CD_Player
{
    public static class Player // https://www.codeproject.com/articles/17279/using-mcisendstring-to-play-media-files
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(
            string command,
            StringBuilder returnValue,
            int returnLength,
            IntPtr winHandle);

        public static event EventHandler Finished;

        private static NotifyForm notifyForm;

        private static string medianame = "CDPlayer";

        public static bool Playing { get; private set; } = false;

        public static bool SessionActive { get; private set; } = false;

        public static void Init(Icon icon, string title)
        {
            notifyForm = new NotifyForm();
            notifyForm.Icon = icon;
            notifyForm.Text = title;
            notifyForm.SoundFinished += soundFinished;
        }

        private static void soundFinished(object sender, EventArgs e)
        {
            if (Finished != null) Finished(null, EventArgs.Empty);
        }

        public static void Play(string filename)
        {
            try
            {
                if (SessionActive) Stop();
                mciSendString("Open \"" + filename + "\" type waveaudio alias " + medianame, null, 0, IntPtr.Zero);
                mciSendString("Play " + medianame + " notify", null, 0, notifyForm.Handle);
                Playing = true;
                SessionActive = true;
            }
            catch { }
        }

        public static void Pause()
        {
            try
            {
                if (SessionActive)
                {
                    mciSendString("Pause " + medianame, null, 0, IntPtr.Zero);
                    Playing = false;
                }
            }
            catch { }
        }

        public static void Resume()
        {
            try
            {
                if (SessionActive)
                {
                    mciSendString("Resume " + medianame, null, 0, IntPtr.Zero);
                    Playing = true;
                }
            }
            catch { }
        }

        public static void Stop()
        {
            try
            {
                if (SessionActive)
                {
                    notifyForm.SoundFinished -= soundFinished;
                    Pause();
                    mciSendString("Stop " + medianame, null, 0, IntPtr.Zero);
                    mciSendString("Close " + medianame, null, 0, IntPtr.Zero);
                    Playing = false;
                    SessionActive = false;
                    notifyForm.SoundFinished += soundFinished;
                }
            }
            catch { }
        }

        private class NotifyForm : Form
        {
            public event EventHandler SoundFinished;

            protected override void WndProc(ref Message msg)
            {
                if (msg.Msg == 0x3B9)
                {
                    if (msg.WParam.ToInt32() == 0x01) SoundFinished(this, EventArgs.Empty);
                }
                base.WndProc(ref msg);
            }
        }
    }
}
