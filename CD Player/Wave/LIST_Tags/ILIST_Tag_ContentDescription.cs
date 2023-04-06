using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_ContentDescription : ILIST_Tag
    {
        public string ContentDescription = "";

        public ILIST_Tag_ContentDescription() { }

        public ILIST_Tag_ContentDescription(string contentDescription)
        {
            ContentDescription = contentDescription;
        }

        public string GetCaption()
        {
            return "Content description";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(ContentDescription));
        }

        public string GetIdentifier()
        {
            return "ISBJ";
        }

        public bool Parse(byte[] data)
        {
            ContentDescription = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_ContentDescription contentDescription)
        {
            return contentDescription.ContentDescription;
        }

        public static implicit operator ILIST_Tag_ContentDescription(string contentDescription)
        {
            return new ILIST_Tag_ContentDescription(contentDescription);
        }
    }
}
