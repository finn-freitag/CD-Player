using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using EasyCodeClass.Multimedia.Audio.Wave;

namespace EasyCodeClass.Multimedia.Audio
{
    public interface IAudioStorage : ICloneable, IDisposable
    {
        WaveData GetData();

        void LoadFrom(IAudioStorage data);

        void LoadFrom(Stream stream);

        void Save(Stream stream);

        void Add(params IAudioStorage[] data);
    }
}
