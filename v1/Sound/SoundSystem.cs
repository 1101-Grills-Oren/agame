
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using agame.IDs;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NVorbis;

namespace agame.Sound{
    class SoundSystem
    {
        public static SoundEffectInstance[] effects=new SoundEffectInstance[256];
        public static void Update()
        {
        }
        public static void PlaySound(int id,float pitch=0f)
        {
            
            //SoundID.effects[id].
            int slot=FindEmptySoundSlot();
            if (slot != -1)
            {
                effects[slot]=SoundID.effects[id].CreateInstance();
                effects[slot].Pitch=pitch;
                effects[slot].Play();
            }
            
        }
        private static int FindEmptySoundSlot()
        {
            for(int i = 0; i < 256; i++)
            {
                if (effects[i] == null)
                {
                    return i;
                }else if (effects[i].IsDisposed)
                {
                    return i;
                }
            }
            return -1;
        }
        public static SoundEffect LoadSound(string name)
        {
            string soundPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content\\sound_"+name);
            
            using (var vorbis = new VorbisReader(soundPath))
            {
                int channels = vorbis.Channels;
                int sampleRate = vorbis.SampleRate;

                // Read all samples
                float[] floatBuffer = new float[vorbis.TotalSamples * channels];
                int samplesRead = vorbis.ReadSamples(floatBuffer, 0, floatBuffer.Length);

                // Convert float [-1,1] to 16-bit PCM
                byte[] byteBuffer = new byte[samplesRead * sizeof(short)];
                int offset = 0;
                for (int i = 0; i < samplesRead; i++)
                {
                    short s = (short)Math.Clamp(floatBuffer[i] * short.MaxValue, short.MinValue, short.MaxValue);
                    byteBuffer[offset++] = (byte)(s & 0xFF);
                    byteBuffer[offset++] = (byte)((s >> 8) & 0xFF);
                }
                
                return new SoundEffect(byteBuffer, sampleRate, (AudioChannels)channels);
            }
            // Load PNG from file stream (bypasses Content Pipeline)
            //try{
                /*using (FileStream stream = new FileStream(soundPath, FileMode.Open))
                {
                    SoundEffect sound = SoundEffect.FromStream(stream);
                    return sound;
                }*/
            /*}
            catch
            {
                return default(SoundEffect);
            }*/
        }
    }
}