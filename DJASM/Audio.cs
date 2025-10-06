using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace DJASM
{
    internal class Audio
    {
        public static unsafe void Init()
        {
            Raylib.InitAudioDevice();


            float[] wavedata = new float[16];
            float[] samples = { 1, 1, 1, 1, 1, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1 };

            for (int i = 0; i < 16; i++)
            {
                wavedata[i] = (samples[i] / 15.0f) * 2.0f - 1.0f;
            }
            float[] fullbuffer = new float[8000];

            for (int i = 0; i < 8000; i++)
            {
                fullbuffer[i] = wavedata[i % 16];
            }

            IntPtr bufferPtr = Marshal.AllocHGlobal(sizeof(float) * 16);
            Marshal.Copy(fullbuffer, 0, bufferPtr, 16);


            Wave wave1 = new Wave
            {
                SampleCount = 8000,
                SampleRate = 8000,
                SampleSize = 32,
                Channels = 1,
                Data = (void*)bufferPtr
        };

            

            Sound output = Raylib.LoadSoundFromWave(wave1);
            Raylib.PlaySound(output);

        }
    }
}
