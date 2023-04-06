using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop
{
    public struct TRACK_DATA
    {
        public byte Reserved;

        private byte BitMapped;

        public byte Control
        {
            get
            {
                return (byte)(BitMapped & 0x0F);
            }
            set
            {
                BitMapped = (byte)((BitMapped & 0xF0) | (value & (byte)0x0F));
            }
        }

        public byte Adr
        {
            get
            {
                return (byte)((BitMapped & (byte)0xF0) >> 4);
            }
            set
            {
                BitMapped = (byte)((BitMapped & (byte)0x0F) | (value << 4));
            }
        }

        public byte TrackNumber;

        public byte Reserved1;

        public byte Address_0;

        public byte Address_1;

        public byte Address_2;

        public byte Address_3;
    }
}
