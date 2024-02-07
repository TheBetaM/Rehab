using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class MusicHash : TwinsSection
    {
        public List<Track> Tracks = new List<Track>();
        public int interleave;

        public class Track
        {
            public int Type;
            public int Size;
            public uint Offset;
            public int SampleRate;
            public int Unknown;
            public string Name;
            public byte[] SoundData;
            public int inter;

            public override string ToString()
            {
                return $"{Name} - Type: {Type} Size: {Size} SampleRate: {SampleRate} Unk: {Unknown}";
            }
        }

        public override void Load(BinaryReader reader, int size)
        {

            int count = reader.ReadInt32();
            interleave = reader.ReadInt32();
            while (count-- > 0)
            {
                Tracks.Add(new Track()
                {
                    Type = reader.ReadInt32(),
                    Size = reader.ReadInt32(),
                    Offset = reader.ReadUInt32(),
                    SampleRate = reader.ReadInt32(),
                    Unknown = reader.ReadInt32(),
                    inter = interleave,
                });
            }

            return;
        }

        public override void Save(BinaryWriter writer)
        {
            
        }
    }

    public class MusicBank : TwinsSection
    {
        public MusicHash Hash;
        public List<Sound> Sounds = new List<Sound>();

        public override void Load(BinaryReader reader, int size)
        {
            if (Hash == null) return;

            for (int i = 0; i < Hash.Tracks.Count; i++)
            {
                var Track = Hash.Tracks[i];
                if (Track.Type == 0)
                {
                    // Mono
                    reader.BaseStream.Position = Track.Offset;
                    byte[] header = reader.ReadBytes(0x40);
                    char[] name = new char[0x10];
                    Array.Copy(header, 0x20, name, 0, 0x10);
                    Track.Name = new string(name).TrimEnd('\0');
                    Track.SoundData = reader.ReadBytes(Track.Size - 0x40);

                    Sounds.Add(new Sound() { track = Track });
                    RecordIDs.Add((uint)i + 1, Records.Count);
                    Records.Add(Sounds[i]);
                }
                else if (Track.Type == 1)
                {
                    // Stereo
                    // todo: there's an extra moment of silence at the start of the track which shouldn't be there
                    Track.Name = $"Track {i}";
                    if (ParentFile.FileName.ToUpper().Contains("MUSIC"))
                    {
                        Track.Name = DefaultHashes.ToName(SectionType.MB, (uint)i);
                    }
                    reader.BaseStream.Position = Track.Offset;
                    //reader.ReadBytes(0x10);
                    Track.SoundData =  reader.ReadBytes(Track.Size);

                    Sounds.Add(new Sound() { track = Track });
                    RecordIDs.Add((uint)i + 1, Records.Count);
                    Records.Add(Sounds[i]);
                }
                else
                {
                    // Null
                    Track.Name = $"Track {i} (NULL)";
                    Track.SoundData = new byte[0];

                    Sounds.Add(new Sound() { track = Track });
                    RecordIDs.Add((uint)i + 1, Records.Count);
                    Records.Add(Sounds[i]);
                }
                
            }
        }

        public override void Save(BinaryWriter writer)
        {
            
        }

        public class Sound : TwinsItem
        {
            public MusicHash.Track track;

            public override string ToString()
            {
                return $"{track.Name} - Type: {track.Type} Freq: {track.SampleRate} Size: {track.Size:X8} Unk: {track.Unknown} Off: {track.Offset:X8}";
            }
        }
    }
}
