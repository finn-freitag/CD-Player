using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class WaveFormat : ICloneable
    {
        //public event EventHandler<FormatChangedEventArgs> FormatChanged;

        //public delegate void FormatChangedEvent(object sender, FormatChangedEventArgs fcea);

        //public FormatChangedEvent FormatChanged;

        private short channels = 2;

        private bool changedActions = true;

        public object parent = null;

        public short Channels
        {
            get
            {
                if(parent != null && parent is WaveData)
                {
                    return (short)((WaveData)parent).data.Count();
                }
                return channels;
            }
            set
            {
                WaveFormat wf = null;
                if (changedActions) wf = (WaveFormat)Clone();
                channels = value;
                if (parent != null && parent is WaveData && changedActions) ((WaveData)parent).FormatChanged(this, new FormatChangedEventArgs(wf));
                //if (FormatChanged != null && changedActions) FormatChanged(this, new FormatChangedEventArgs(wf));
            }
        }

        private int sampleRate = 44100;

        public int SampleRate
        {
            get { return sampleRate; }
            set
            {
                WaveFormat wf = null;
                if (changedActions) wf = (WaveFormat)Clone();
                sampleRate = value;
                if (parent != null && parent is WaveData && changedActions) ((WaveData)parent).FormatChanged(this, new FormatChangedEventArgs(wf));
                //if (FormatChanged != null && changedActions) FormatChanged(this, new FormatChangedEventArgs(wf));
            }
        }

        public int AverageBytesPerSecond { get { return sampleRate * BlockAlign; } }

        private BitsPerSample bitsperSample = BitsPerSample.WAV64;

        public BitsPerSample bitsPerSample
        {
            get { return bitsperSample; }
            set
            {
                WaveFormat wf = null;
                if (changedActions) wf = (WaveFormat)Clone();
                bitsperSample = value;
                if (parent != null && parent is WaveData && changedActions) ((WaveData)parent).FormatChanged(this, new FormatChangedEventArgs(wf));
                //if (FormatChanged != null && changedActions) FormatChanged(this, new FormatChangedEventArgs(wf));
            }
        }

        public short BlockAlign { get { return (short)(channels * ((short)bitsperSample / 8)); } }

        public WaveFormat()
        {

        }

        public WaveFormat(object parent)
        {
            this.parent = parent;
        }

        public object Clone()
        {
            WaveFormat wf = new WaveFormat();
            wf.sampleRate = this.sampleRate;
            wf.channels = this.Channels;
            wf.bitsperSample = this.bitsPerSample;
            return wf;
        }
    }
}
