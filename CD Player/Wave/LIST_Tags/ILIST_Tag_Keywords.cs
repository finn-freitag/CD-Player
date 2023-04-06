using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Keywords : List<string>, ILIST_Tag
    {
        public ILIST_Tag_Keywords() { }

        public ILIST_Tag_Keywords(List<string> keywords)
        {
            this.AddRange(keywords);
        }

        public string GetCaption()
        {
            return "Keywords";
        }

        public byte[] GetData()
        {
            string final = "";
            for(int i = 0; i < this.Count; i++)
            {
                final += ";" + this[i];
            }
            final = final.Substring(1);
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(final));
        }

        public string GetIdentifier()
        {
            return "IKEY";
        }

        public bool Parse(byte[] data)
        {
            string raw = Encoding.UTF8.GetString(data).Trim('\0');
            string[] parts = raw.Split(';');
            for(int i = 0; i < parts.Length; i++)
            {
                this.Add(parts[i]);
            }
            return true;
        }

        public static implicit operator string[](ILIST_Tag_Keywords keywords)
        {
            return keywords.ToArray();
        }

        public static implicit operator ILIST_Tag_Keywords(string[] keywords)
        {
            ILIST_Tag_Keywords k = new ILIST_Tag_Keywords();
            k.AddRange(keywords);
            return k;
        }
    }
}
