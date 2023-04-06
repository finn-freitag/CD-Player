using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public class CDROM_TOC // TOC -> Table of contents
    {
        public ushort Length;
        public byte FirstTrack = 0;
        public byte LastTrack = 0;

        public TrackDataList TrackData;

        public CDROM_TOC()
        {
            TrackData = new TrackDataList();
            Length = (ushort)Marshal.SizeOf(this);
        }
    }
}
