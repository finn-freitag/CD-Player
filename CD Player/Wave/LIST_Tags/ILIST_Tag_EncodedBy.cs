using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_EncodedBy : ILIST_Tag
    {
        public string EncodedBy = "";

        public ILIST_Tag_EncodedBy() { }

        public ILIST_Tag_EncodedBy(string encodedBy)
        {
            EncodedBy = encodedBy;
        }

        public string GetCaption()
        {
            return "Encoded by";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(EncodedBy));
        }

        public string GetIdentifier()
        {
            return "ISFT";
        }

        public bool Parse(byte[] data)
        {
            EncodedBy = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_EncodedBy encodedBy)
        {
            return encodedBy.EncodedBy;
        }

        public static implicit operator ILIST_Tag_EncodedBy(string encodedBy)
        {
            return new ILIST_Tag_EncodedBy(encodedBy);
        }
    }
}
