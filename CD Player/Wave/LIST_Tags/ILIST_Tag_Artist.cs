using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Artist : ILIST_Tag
    {
        public string Artist = "";

        public ILIST_Tag_Artist() { }

        public ILIST_Tag_Artist(string artist)
        {
            Artist = artist;
        }

        public string GetCaption()
        {
            return "Artist";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Artist));
        }

        public string GetIdentifier()
        {
            return "IART";
        }

        public bool Parse(byte[] data)
        {
            Artist = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_Artist artist)
        {
            return artist.Artist;
        }

        public static implicit operator ILIST_Tag_Artist(string artist)
        {
            return new ILIST_Tag_Artist(artist);
        }
    }
}
