using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Title : ILIST_Tag
    {
        public string Title = "";

        public ILIST_Tag_Title() { }

        public ILIST_Tag_Title(string title)
        {
            Title = title;
        }

        public string GetCaption()
        {
            return "Title";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Title));
        }

        public string GetIdentifier()
        {
            return "INAM";
        }

        public bool Parse(byte[] data)
        {
            Title = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_Title title)
        {
            return title.Title;
        }

        public static implicit operator ILIST_Tag_Title(string title)
        {
            return new ILIST_Tag_Title(title);
        }
    }
}
