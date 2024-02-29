using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class MusicHashDemo : TwinsSection
    {
        public List<Track> Tracks = new List<Track>();
        public int interleave;

        public class Track
        {
            public int Size;
            public uint Offset;
            public int SampleRate;
            public string Name;
            public byte[] SoundData;
            public int inter;
            public uint ID;
            public bool isStereo;

            public override string ToString()
            {
                return $"{Name} - ID: {ID} Size: {Size} SampleRate: {SampleRate}";
            }
        }

        public override void Load(BinaryReader reader, int size)
        {
            
            reader.ReadUInt32(); // file size
            reader.ReadUInt32(); // padding size
            int count = reader.ReadInt32();
            while (count-- > 0)
            {
                Tracks.Add(new Track()
                {
                    Size = reader.ReadInt32(),
                    ID = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32(),
                    SampleRate = reader.ReadInt32(),
                    inter = 65536,//32768 in proto
                });
            }

            return;
        }
    }

    public class MusicBankDemo : TwinsSection
    {
        public MusicHashDemo Hash;
        public List<Sound> Sounds = new List<Sound>();

        public override void Load(BinaryReader reader, int size)
        {
            if (Hash == null) return;

            for (int i = 0; i < Hash.Tracks.Count; i++)
            {
                var Track = Hash.Tracks[i];
                Track.Name = $"Track {i}";
                if (ParentFile.FileName.ToUpper().Contains("MUSIC"))
                {
                    Track.Name = DefaultHashes.ToName(SectionType.MSB, (uint)i);
                }
                if (Track.SampleRate == 48000 || Track.SampleRate == 32000)
                {
                    if (Track.SampleRate == 48000)
                    {
                        Track.SampleRate = 44100; // proto thing
                    }
                    Track.isStereo = true;
                }
                reader.BaseStream.Position = Track.Offset;
                //reader.ReadBytes(0x10);
                Track.SoundData =  reader.ReadBytes(Track.Size);

                Sounds.Add(new Sound() { track = Track });
                RecordIDs.Add((uint)i + 1, Records.Count);
                Records.Add(Sounds[i]);
                
            }
        }

        public class Sound : TwinsItem
        {
            public MusicHashDemo.Track track;

            public override string ToString()
            {
                return $"{track.Name} - ID: {track.ID} Freq: {track.SampleRate} Size: {track.Size:X8} Off: {track.Offset:X8}";
            }
        }
    }
}
