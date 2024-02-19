using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class XWB : TwinsSection
    {
        public List<Sound> Sounds = new List<Sound>();

        public override void Load(BinaryReader reader, int size)
        {
            var startPos = reader.BaseStream.Position;
            reader.ReadUInt32(); // WBND
            uint Version = reader.ReadUInt32(); //3
            uint WaveBankInfoOffset = reader.ReadUInt32();
            uint WaveBankInfoSize = reader.ReadUInt32();
            uint FileRecordsOffset = reader.ReadUInt32();
            uint FileRecordsSize = reader.ReadUInt32();
            uint FileNamesOffset = reader.ReadUInt32();
            uint FileNamesSize=  reader.ReadUInt32();
            uint FileDataOffset = reader.ReadUInt32();
            uint FileDataSize = reader.ReadUInt32();

            // Wave Bank Info
            reader.BaseStream.Position = WaveBankInfoOffset;
            byte[] WaveBankInfoFlags = reader.ReadBytes(4);
            uint FileCount = reader.ReadUInt32();
            byte[] WaveBankName = reader.ReadBytes(0x10);
            uint RecordSize = reader.ReadUInt32();
            uint EntryNameBlockSize = reader.ReadUInt32();
            uint WaveBankFileDataOffset = reader.ReadUInt32();
            reader.ReadUInt32();

            // File Records
            reader.BaseStream.Position = FileRecordsOffset;
            for (int i = 0; i < FileCount; i++)
            {
                Sound sfx = new Sound();
                reader.ReadUInt32();
                uint MagicValue = reader.ReadUInt32();

                sfx.Format = (byte)((MagicValue << 29) >> 31);
                sfx.Channels = (byte)((MagicValue >> 2) & 7);
                sfx.BitsPerSample = (MagicValue >> 31) != 0;
                sfx.SampleRate = MagicValue >> 5;
                if (sfx.BitsPerSample) sfx.SampleRate -= 0x80000000;

                sfx.FileOffset = reader.ReadUInt32();
                sfx.FileSize = reader.ReadUInt32();
                sfx.LoopRegionOffset = reader.ReadUInt32();
                sfx.LoopRegionLength = reader.ReadUInt32();
                Sounds.Add(sfx);
            }

            // File Names
            reader.BaseStream.Position = FileNamesOffset;
            for (int i = 0; i < FileCount; i++)
            {
                Sounds[i].FileName = new string(reader.ReadChars(0x40)).TrimEnd('\0');
            }

            for (int i = 0; i < FileCount; i++)
            {
                reader.BaseStream.Position = FileDataOffset + Sounds[i].FileOffset;
                Sounds[i].SoundData = reader.ReadBytes((int)Sounds[i].FileSize);
                RecordIDs.Add((uint)i + 1, Records.Count);
                Records.Add(Sounds[i]);
            }

            return;
        }

        public class Sound : TwinsItem
        {
            public uint FileOffset;
            public uint FileSize;
            public uint LoopRegionOffset;
            public uint LoopRegionLength;
            public string FileName = string.Empty;
            public byte[] SoundData;

            public byte Format; // 0 - 2
            public byte Channels; // 0 - 6
            public uint SampleRate;
            public bool BitsPerSample; // 8 / 16

            public override string ToString()
            {
                return $"{FileName} BPS: {BitsPerSample} SampleRate: {SampleRate}";
            }
        }
    }
}
