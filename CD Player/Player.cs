using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CD_Player
{
    public static class Player
    {
        public static event EventHandler Finished;

        private static SoundPlayer sp = new SoundPlayer();

        private static long PausePos = 0;

        public static bool Playing { get; private set; } = false;

        private static byte[] bytes;

        public static void Play(byte[] bytes)
        {
            Player.bytes = bytes;
            StopAndDispose();
            sp.Stream = new MemoryStream(bytes);
            Thread t = new Thread(new ThreadStart(ThreadPlay));
            t.Start();
        }

        private static void ThreadPlay()
        {
            Playing = true;
            sp.PlaySync();
            if (Finished != null) Finished(null, EventArgs.Empty);
        }

        public static void SwitchPlayPause()
        {
            if (Playing)
            {
                sp.Stop();
                PausePos = sp.Stream.Position;
            }
            else
            {
                Play(bytes);
                sp.Stream.Seek(PausePos, SeekOrigin.Begin);
            }
        }

        public static void StopAndDispose()
        {
            if (sp != null)
            {
                sp.Stop();
                if (sp.Stream != null) sp.Stream.Dispose();
                Playing = false;
            }
        }
    }
}
