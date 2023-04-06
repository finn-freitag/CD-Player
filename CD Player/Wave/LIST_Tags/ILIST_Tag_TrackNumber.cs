using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_TrackNumber : ILIST_Tag
    {
        public int TrackNumber = 1;

        public ILIST_Tag_TrackNumber() { }

        public ILIST_Tag_TrackNumber(int trackNumber)
        {
            TrackNumber = trackNumber;
        }

        public string GetCaption()
        {
            return "Track number";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Convert.ToString(TrackNumber)));
        }

        public string GetIdentifier()
        {
            return "ITRK";
        }

        public bool Parse(byte[] data)
        {
            TrackNumber = Convert.ToInt32(Encoding.UTF8.GetString(data).Trim('\0'));
            return true;
        }

        public static implicit operator int(ILIST_Tag_TrackNumber trackNumber)
        {
            return trackNumber.TrackNumber;
        }

        public static implicit operator ILIST_Tag_TrackNumber(int trackNumber)
        {
            return new ILIST_Tag_TrackNumber(trackNumber);
        }
    }
}
