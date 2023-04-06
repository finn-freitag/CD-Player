using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class WaveData : IAudioStorage, ICloneable
    {
        public List<List<Int64>> data = new List<List<Int64>>();

        public ISampleMixer SampleMixer = new AverageSampleMixer();
        
        private WaveFormat fmt;
        private bool isLoading = false;

        public WaveFormat format
        {
            get
            {
                return fmt;
            }
            set
            {
                fmt = value;
                if (!isLoading) fmt.parent = this;
            }
        }

        public TimeSpan Seconds
        {
            get
            {
                return TimeSpan.FromSeconds((DataLength - 44) / format.AverageBytesPerSecond);
            }
        }

        public virtual long DataLength
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                Save(ms);
                return ms.ToArray().Length - 44;
            }
        }

        public WaveData()
        {
            format = new WaveFormat(this);
            //format.FormatChanged = Format_FormatChanged;
        }

        public void FormatChanged(object sender, FormatChangedEventArgs e)
        {
            Repair(e.oldFormat);
        }

        public virtual WaveData GetData()
        {
            return (WaveData)Clone();
        }

        public virtual void LoadFrom(IAudioStorage data)
        {
            this.data = data.GetData().data; // TODO fix this problem
            this.format = data.GetData().format;
        }

        public virtual void LoadFrom(Stream stream)
        {
            try
            {
                isLoading = true;
                BinaryReader reader = new BinaryReader(stream);
                if (Encoding.UTF8.GetString(reader.ReadBytes(4)).ToUpper() != "RIFF") throw new InvalidCastException();
                uint firstmarker = reader.ReadUInt32();
                uint fileSize = firstmarker + 8;
                if (Encoding.UTF8.GetString(reader.ReadBytes(4)).ToUpper() != "WAVE") throw new InvalidCastException();
                if (Encoding.UTF8.GetString(reader.ReadBytes(4)).ToLower() != "fmt ") throw new InvalidCastException();
                int waveFormatLength = reader.ReadInt32();
                format = new WaveFormat();
                short encoding = reader.ReadInt16();
                format.Channels = reader.ReadInt16();
                format.SampleRate = reader.ReadInt32();
                int abps = reader.ReadInt32();
                short blockAlign = reader.ReadInt16();
                format.bitsPerSample = (BitsPerSample)reader.ReadInt16();

                string ContainsData = "";
                while (!ContainsData.Contains("data"))
                {
                    ContainsData += Encoding.UTF8.GetString(new byte[] { reader.ReadByte() });
                    if (ContainsData.Length > 4) throw new InvalidCastException();
                }

                int dataLength = reader.ReadInt32();
                int readedLength = 0;
                data = new List<List<long>>();

                for (int c = 0; c < format.Channels; c++)
                {
                    data.Add(new List<long>());
                }

                while (dataLength > readedLength)
                {
                    if (format.Channels <= 0) throw new InvalidCastException();
                    for (int c = 0; c < format.Channels; c++)
                    {
                        if (format.bitsPerSample == BitsPerSample.WAV16)
                        {
                            data[c].Add(reader.ReadInt16());
                            readedLength += 2;
                        }
                        if (format.bitsPerSample == BitsPerSample.WAV32)
                        {
                            data[c].Add(reader.ReadInt32());
                            readedLength += 4;
                        }
                        if (format.bitsPerSample == BitsPerSample.WAV64)
                        {
                            data[c].Add(reader.ReadInt64());
                            readedLength += 8;
                        }
                    }
                }

                isLoading = false;
                format.parent = this;

                int checkLength = data[0].Count;
                foreach (List<long> l in data)
                {
                    if (checkLength != l.Count) throw new InvalidCastException();
                }
            }
            catch
            {
                throw new InvalidCastException();
            }
        }

        public virtual void Save(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(Encoding.UTF8.GetBytes("RIFF"));
            writer.Write((int)0); // placeholder
            writer.Write(Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(Encoding.UTF8.GetBytes("fmt "));
            writer.Write((int)(16)); // wave format length
            writer.Write((short)1);// PCM Encoding
            writer.Write((short)format.Channels);
            writer.Write((int)format.SampleRate);
            writer.Write((int)format.AverageBytesPerSecond);
            writer.Write((short)format.BlockAlign);
            writer.Write((short)format.bitsPerSample);

            writer.Write(Encoding.UTF8.GetBytes("data"));
            writer.Write((int)0); // placeholder

            //MemoryStream ms = new MemoryStream();
            //BinaryWriter bw = new BinaryWriter(ms);
            RepairChannelLength();
            int maxCount = 0;
            foreach (List<long> l in data)
            {
                if (l.Count > maxCount) maxCount = l.Count;
            }
            for (int dataPos = 0; dataPos < maxCount; dataPos++)
            {
                foreach (List<long> channel in data)
                {
                    if (format.bitsPerSample == BitsPerSample.WAV16) writer.Write((short)channel[dataPos]);
                    if (format.bitsPerSample == BitsPerSample.WAV32) writer.Write((int)channel[dataPos]);
                    if (format.bitsPerSample == BitsPerSample.WAV64) writer.Write(channel[dataPos]);
                }
            }

            //writer.Flush();
            //writer.Close();
            //writer.Dispose();

            // Fill placeholders
            int fileSize = Convert.ToInt32(ms.Length);
            uint firstMarker = (uint)(fileSize - 8);
            uint secondMarker = (uint)(fileSize - 44);
            byte[] firstMarkerBytes = BitConverter.GetBytes(firstMarker);
            byte[] secondMarkerBytes = BitConverter.GetBytes(secondMarker);
            byte[] FILE = ms.ToArray();
            FILE[4] = firstMarkerBytes[0];
            FILE[5] = firstMarkerBytes[1];
            FILE[6] = firstMarkerBytes[2];
            FILE[7] = firstMarkerBytes[3];
            FILE[40] = secondMarkerBytes[0];
            FILE[41] = secondMarkerBytes[1];
            FILE[42] = secondMarkerBytes[2];
            FILE[43] = secondMarkerBytes[3];

            writer.Close();
            writer.Dispose();
            ms.Close();
            ms.Dispose();

            stream.Write(FILE, 0, FILE.Length);

            GC.Collect();
        }

        public virtual void Add(params IAudioStorage[] data)
        {
            int channels = format.Channels;
            int sampleRate = format.SampleRate;
            foreach (IAudioStorage ias in data)
            {
                if (ias.GetData().format.Channels > channels) channels = ias.GetData().format.Channels;
                if (ias.GetData().format.SampleRate > sampleRate) sampleRate = ias.GetData().format.SampleRate;
            }
            WaveFormat waveFormat = (WaveFormat)format.Clone();
            format.Channels = (short)channels;
            format.SampleRate = sampleRate;
            Repair(waveFormat);
            foreach (IAudioStorage ias in data)
            {
                int c = 0;
                WaveData wd = (WaveData)ias.GetData().Clone();
                WaveFormat Format = (WaveFormat)wd.format.Clone();
                wd.format.SampleRate = sampleRate;
                wd.format.Channels = (short)channels;
                wd.RepairSampleRate(Format);
                foreach (List<long> bytes in wd.data)
                {
                    this.data[c].AddRange(bytes);
                    c++;
                }
            }
        }

        public void Repair(WaveFormat oldWaveFormat)
        {
            RepairChannels();
            RepairChannelLength();
            RepairSampleRate(oldWaveFormat);

            RepairEvents();
        }

        public void RepairChannels()
        {
            format.parent = null;
            //format.FormatChanged = null;
            //Add Channels if required
            if (format.Channels > data.Count)
            {
                int dataCount = data.Count;
                for (int i = 0; i < format.Channels - dataCount; i++)
                {
                    data.Add(new List<long>());
                }
            }
            //If more channels exist as in WaveFormat then update WaveFormat
            if (format.Channels < data.Count)
            {
                format.Channels = (short)data.Count;
            }
            RepairEvents();
        }

        public void RepairChannelLength()
        {
            //Add zeros to too short channels
            long maxData = 0;
            for (int c = 0; c < format.Channels; c++)
            {
                if (data[c].Count > maxData) maxData = data[c].Count;
            }
            for (int c = 0; c < format.Channels; c++)
            {
                for (int b = data[c].Count - 1; b < maxData - 1; b++)
                {
                    data[c].Add(0);
                }
            }
        }

        public void RepairSampleRate(WaveFormat oldWaveFormat)
        {
            if (oldWaveFormat.SampleRate != format.SampleRate)
            {
                for (int c = 0; c < data.Count; c++)
                {
                    List<long> result = new List<long>();
                    double jumpForeward = (double)oldWaveFormat.SampleRate / (double)format.SampleRate;
                    for (double i = 0; i < data[c].Count; i += jumpForeward)
                    {
                        result.Add(data[c][Convert.ToInt32(Math.Min(Math.Round(i), data[c].Count - 1))]);
                    }
                    data[c] = result;
                }
            }
        }

        public void RepairSampleRateSmooth(WaveFormat oldWaveFormat)
        {

        }

        public void RepairEvents()
        {
            format.parent = this;
            //format.FormatChanged = Format_FormatChanged;
        }

        public void DeleteChannel(int channel)
        {
            data.RemoveAt(channel);
        }

        public void MixChannels(int resultChannelIndex, params int[] channels)
        {
            RepairChannelLength();
            List<List<long>> channel = new List<List<long>>();
            foreach (int i in channels)
            {
                channel.Add(data[i]);
                data[i] = null;
            }
            List<int> channelsList = channels.ToList();
            channelsList.Sort();
            channelsList.Reverse();
            foreach(int i in channelsList)
            {
                data.RemoveAt(i);
            }
            GC.Collect();
            List<long> result = new List<long>();
            for (int i = 0; i < channel[0].Count; i++)
            {
                /*long average = 0;
                for (int c = 0; c < channel.Count; c++)
                {
                    average += channel[c][i];
                }
                result.Add(average / channel.Count);*/
                List<long> samples = new List<long>();
                for (int c = 0; c < channel.Count; c++)
                {
                    samples.Add(channel[c][i]);
                }
                result.Add(SampleMixer.MixSamples(format.bitsPerSample, samples.ToArray()));
            }
            if (resultChannelIndex < 0) resultChannelIndex = 0;
            if(data.Count <= 0)
            {
                data.Add(result);
            }
            else
            {
                data.Insert(Math.Min(resultChannelIndex, data.Count - 1), result);
            }
        }

        public void CreateMonoChannel()
        {
            List<int> channel = new List<int>();
            for (int i = 0; i < data.Count; i++)
            {
                channel.Add(i);
            }
            MixChannels(0, channel.ToArray());
        }

        public object Clone()
        {
            Repair(format);
            WaveData wd = new WaveData();
            wd.format = (WaveFormat)format.Clone();
            List<List<long>> d = new List<List<long>>();
            for (int c = 0; c < data.Count; c++)
            {
                d.Add(new List<long>());
                for (int l = 0; l < data[c].Count; l++)
                {
                    d[c].Add(data[c][l]);
                }
            }
            wd.data = d;
            wd.SampleMixer = SampleMixer;
            wd.RepairEvents();
            return wd;
        }

        public void Dispose()
        {
            data = new List<List<long>>();
            format = new WaveFormat();
            format.parent = this;
            GC.Collect();
        }
    }
}
