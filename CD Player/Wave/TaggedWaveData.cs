using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class TaggedWaveData : WaveData
    {
        public LIST_Tag LISTTags = new LIST_Tag();

        public override long DataLength
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                base.Save(ms);
                return ms.ToArray().Length - 44;
            }
        }

        public override void LoadFrom(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] streamData = reader.ReadBytes((int)stream.Length);
            base.LoadFrom(new MemoryStream(streamData));
            long datalength = DataLength;
            byte[] headerBytes = new byte[streamData.Length - (datalength + 44)];
            Array.Copy(streamData, datalength + 44, headerBytes, 0, headerBytes.Length);
            try
            {
                BinaryReader br = new BinaryReader(new MemoryStream(headerBytes));
                LISTTags = new LIST_Tag();
                LISTTags.FindAndRead(br);
            }
            catch { }
        }

        public override void Save(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            base.Save(ms);
            if(LISTTags.Tags.Count > 0)
            {
                MemoryStream ms2 = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms2);
                LISTTags.Write(bw);
                bw.Close();
                bw.Dispose();
                byte[] msB = ms.ToArray();
                byte[] ms2B = ms2.ToArray();

                ms.Dispose();
                ms2.Dispose();
                byte[] data = new byte[msB.Length + ms2B.Length];
                Array.Copy(msB, data, msB.Length);
                Array.Copy(ms2B, 0, data, msB.Length, ms2B.Length);
                byte[] size = BitConverter.GetBytes((uint)(data.Length - 8));
                data[4] = size[0];
                data[5] = size[1];
                data[6] = size[2];
                data[7] = size[3];
                ms = new MemoryStream(data);
            }
            byte[] bytes = ms.ToArray();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
