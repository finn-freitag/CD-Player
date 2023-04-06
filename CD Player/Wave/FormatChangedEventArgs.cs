using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class FormatChangedEventArgs : EventArgs
    {
        public WaveFormat oldFormat;

        public FormatChangedEventArgs(WaveFormat wf)
        {
            oldFormat = wf;
        }
    }
}
