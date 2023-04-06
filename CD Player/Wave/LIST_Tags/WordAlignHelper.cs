using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags
{
    public class WordAlignHelper
    {
        public static byte[] Align(byte[] bytes)
        {
            if ((bytes.Length / 2.0) % 1.0 == 0.0) return bytes;
            byte[] res = new byte[bytes.Length + 1];
            Array.Copy(bytes, res, bytes.Length);
            res[res.Length - 1] = 0;
            return res;
        }
    }
}
