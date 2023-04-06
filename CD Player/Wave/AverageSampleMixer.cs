using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class AverageSampleMixer : ISampleMixer
    {
        public AverageSampleMixer()
        {

        }

        public long MixSamples(BitsPerSample bps, params long[] samples)
        {
            long average = 0;
            for(int i = 0; i < samples.Length; i++)
            {
                average += samples[i];
            }
            return average / samples.Length;
        }
    }
}
