using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public interface ILIST_Tag
    {
        string GetIdentifier();     // returns the identifier (example: "IGNR")
        string GetCaption();        // returns a possible caption (example: "Genre")
        byte[] GetData();           // returns the data (example: "Jazz")
        bool Parse(byte[] data);    // returns true if the data could be parsed
    }
}
