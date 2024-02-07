using System.IO;
using System;

namespace Twinsanity
{
    public static class RIFF
    {
        public static byte[] SaveRiff(byte[] pcm, short channels, int samplerate, int format = 1, int bitspersample = 16)
        {
            byte[] data = new byte[pcm.Length + 44];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(data));
            writer.Write("RIFF".ToCharArray());
            writer.Write(36 + pcm.Length);
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(16);
            writer.Write((ushort)format);
            writer.Write(channels);
            writer.Write(samplerate);
            writer.Write(samplerate * channels * 2); // byte rate
            writer.Write((short)(channels * 2)); // block align
            writer.Write((ushort)bitspersample);
            writer.Write("data".ToCharArray());
            writer.Write(pcm.Length);
            writer.Write(pcm);
            writer.Close();
            return data;
        }

        // Saves as IMA ADPCM wav (does not import into Godot)
        public static byte[] SaveRiffX(byte[] pcm, short channels, int samplerate, int format = 1, int bitspersample = 16)
        {
            byte[] data = new byte[pcm.Length + 48];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(data));
            writer.Write("RIFF".ToCharArray());
            writer.Write(40 + pcm.Length);
            writer.Write("WAVE".ToCharArray());
            writer.Write("fmt ".ToCharArray());
            writer.Write(20);
            writer.Write((ushort)format);
            writer.Write(channels);
            writer.Write(samplerate);
            writer.Write(samplerate * channels / 2); // bytes per second
            writer.Write((short)(channels * 0x24)); // block align
            writer.Write((short)0x04); // bits per sample
            writer.Write((short)0x02);
            writer.Write((short)0x40);
            //writer.Write((ushort)bitspersample);
            writer.Write("data".ToCharArray());
            writer.Write(pcm.Length);
            writer.Write(pcm);
            writer.Close();
            return data;
        }
    }
}
