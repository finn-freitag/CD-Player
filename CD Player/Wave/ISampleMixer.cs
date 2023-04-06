using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public interface ISampleMixer
    {
        long MixSamples(BitsPerSample bps, params long[] samples);
    }
}
