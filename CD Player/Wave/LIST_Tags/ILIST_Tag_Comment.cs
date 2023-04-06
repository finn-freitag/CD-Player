using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class ILIST_Tag_Comment : ILIST_Tag
    {
        public string Comment = "";

        public ILIST_Tag_Comment() { }

        public ILIST_Tag_Comment(string comment)
        {
            Comment = comment;
        }

        public string GetCaption()
        {
            return "Comment";
        }

        public byte[] GetData()
        {
            return WordAlignHelper.Align(Encoding.UTF8.GetBytes(Comment));
        }

        public string GetIdentifier()
        {
            return "ICMT";
        }

        public bool Parse(byte[] data)
        {
            Comment = Encoding.UTF8.GetString(data).Trim('\0');
            return true;
        }

        public static implicit operator string(ILIST_Tag_Comment comment)
        {
            return comment.Comment;
        }

        public static implicit operator ILIST_Tag_Comment(string comment)
        {
            return new ILIST_Tag_Comment(comment);
        }
    }
}
