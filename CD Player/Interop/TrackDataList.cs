using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public class TrackDataList
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constances.MAXIMUM_NUMBER_TRACKS * 8)]

        private byte[] Data;

        public TRACK_DATA this[int Index]
        {
            get
            {
                if ((Index < 0) | (Index >= Constances.MAXIMUM_NUMBER_TRACKS))
                {
                    throw new IndexOutOfRangeException();
                }
                TRACK_DATA res;
                GCHandle handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                try
                {
                    IntPtr buffer = handle.AddrOfPinnedObject();
                    buffer = (IntPtr)(buffer.ToInt32() + (Index * Marshal.SizeOf(typeof(TRACK_DATA))));
                    res = (TRACK_DATA)Marshal.PtrToStructure(buffer, typeof(TRACK_DATA));
                }
                finally
                {
                    handle.Free();
                }
                return res;
            }
        }

        public TrackDataList()
        {
            Data = new byte[Constances.MAXIMUM_NUMBER_TRACKS * Marshal.SizeOf(typeof(TRACK_DATA))];
        }
    }
}
