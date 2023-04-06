using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Album : ILIST_Tag
    {
        public string Album = "";

        public ILIST_Tag_Album() { }

        public ILIST_Tag_Album(string album)
        {
            Album = album;
        }

        public string GetCaption()
        {
            return "Album";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Album));
        }

        public string GetIdentifier()
        {
            return "IPRD";
        }

        public bool Parse(byte[] data)
        {
            Album = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_Album album)
        {
            return album.Album;
        }

        public static implicit operator ILIST_Tag_Album(string album)
        {
            return new ILIST_Tag_Album(album);
        }
    }
}
