using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Genre : ILIST_Tag
    {
        public string Genre = "";

        public ILIST_Tag_Genre() { }

        public ILIST_Tag_Genre(string genre)
        {
            Genre = genre;
        }

        public string GetCaption()
        {
            return "Genre";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Genre));
        }

        public string GetIdentifier()
        {
            return "IGNR";
        }

        public bool Parse(byte[] data)
        {
            Genre = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_Genre genre)
        {
            return genre.Genre;
        }

        public static implicit operator ILIST_Tag_Genre(string genre)
        {
            return new ILIST_Tag_Genre(genre);
        }
    }
}
