using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCodeClass.Multimedia.Audio.Wave.LIST_Tags;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class LIST_Tag
    {
        public List<ILIST_Tag> Tags = new List<ILIST_Tag>();

        public LIST_Tag()
        {

        }

        public void FindAndRead(BinaryReader reader)
        {
            bool run = true;
            long pos = 0;
            while (run)
            {
                pos = reader.BaseStream.Position;
                if ((Encoding.ASCII.GetString(new byte[] { reader.ReadByte() })) == "L")
                {
                    if ((Encoding.ASCII.GetString(new byte[] { reader.ReadByte() })) == "I")
                    {
                        if ((Encoding.ASCII.GetString(new byte[] { reader.ReadByte() })) == "S")
                        {
                            if((Encoding.ASCII.GetString(new byte[] { reader.ReadByte() })) == "T")
                            {
                                reader.BaseStream.Position = pos;
                                run = false;
                            }
                        }
                    }
                }
                if (reader.BaseStream.Position >= reader.BaseStream.Length) return;
            }
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            string list = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (list.ToLower() != "list") throw new InvalidCastException();
            uint size = reader.ReadUInt32();
            uint remainingSize = size;
            string info = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (info.ToLower() != "info") throw new InvalidCastException();
            remainingSize -= 4;
            while(remainingSize > 7)
            {
                string identifier = Encoding.ASCII.GetString(reader.ReadBytes(4));
                remainingSize -= 4;
                uint tagSize = reader.ReadUInt32();
                remainingSize -= 4;
                byte[] data = reader.ReadBytes((int)tagSize);
                remainingSize -= tagSize;
                ILIST_Tag tag = LIST_TagRegistry.GetTag(identifier);
                if(tag != null)
                {
                    if (tag.Parse(data)) Tags.Add(tag);
                }
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write('L');
            bw.Write('I');
            bw.Write('S');
            bw.Write('T');
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            for(int i = 0; i < Tags.Count; i++)
            {
                writer.Write(Encoding.ASCII.GetBytes(Tags[i].GetIdentifier()));
                byte[] data = Tags[i].GetData();
                int length = data.Length + 1;
                bool byteNeeded = (data.Length + 1) % 2 != 0;
                if (byteNeeded) length++;
                writer.Write((uint)length);
                writer.Write(data);
                writer.Write((byte)0); // marker -> end of tag
                if (byteNeeded) writer.Write((byte)0); // fill up if byte amount is odd
            }
            byte[] tags = ms.ToArray();
            writer.Close();
            writer.Dispose();
            ms.Close();
            ms.Dispose();
            bw.Write((uint)tags.Length);
            bw.Write('I');
            bw.Write('N');
            bw.Write('F');
            bw.Write('O');
            bw.Write(tags);
        }
    }
}
