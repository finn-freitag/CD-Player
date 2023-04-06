using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Wave
{
    public class AdditionSampleMixer : ISampleMixer
    {
        public long MixSamples(BitsPerSample bps, params long[] samples)
        {
            long addition = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                addition += samples[i];
            }
            if (bps == BitsPerSample.WAV16)
            {
                if (addition < short.MinValue) addition = short.MinValue;
                if (addition > short.MaxValue) addition = short.MaxValue;
            }
            if (bps == BitsPerSample.WAV32)
            {
                if (addition < int.MinValue) addition = int.MinValue;
                if (addition > int.MaxValue) addition = int.MaxValue;
            }
            if (bps == BitsPerSample.WAV64)
            {
                if (addition < long.MinValue) addition = long.MinValue;
                if (addition > long.MaxValue) addition = long.MaxValue;
            }
            return addition;
        }
    }
}
