using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Twinsanity.Video
{
    public class PSS_Video : TwinsItem
    {

        public override void Load(BinaryReader reader, int length)
        {
            using (BinaryReader2 reader2 = new BinaryReader2(reader.BaseStream))
            {
                uint Header = reader2.ReadUInt32();
                int CurrentTrack = 0;
                while (Header != (uint)PSS_SectionType.FileEnd && reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    switch (Header)
                    {
                        case (uint)PSS_SectionType.FileStart:
                        {
                            reader.BaseStream.Position += 0x0A;
                        }
                        break;
                        default:
                        if (Header <= 0x1AF)
                        {
                            reader.BaseStream.Position += 0x0A;
                        }
                        else if (AudioStreamBlockIDs.Contains(Header))
                        {
                            ushort BlockSize = reader2.ReadUInt16();
                            CurrentTrack = 0;
                            reader.BaseStream.Position += 0x10;
                            CurrentTrack = reader.ReadByte();
                            if (CurrentTrack != 0 && CurrentTrack > AudioTracks.Count - 1)
                            {
                                AudioTracks.Add(new AudioTrack());
                            }
                            byte[] Buffer;
                            if (AudioTracks[CurrentTrack].DataList.Count == 0)
                            {
                                reader.ReadUInt32(); // SShd
                                reader.ReadUInt32(); // size
                                AudioTracks[CurrentTrack].Codec = reader.ReadUInt32();
                                AudioTracks[CurrentTrack].SampleRate = reader.ReadUInt32();
                                AudioTracks[CurrentTrack].Channels = reader.ReadUInt32();
                                AudioTracks[CurrentTrack].Interleave = reader.ReadUInt32();
                                reader.BaseStream.Position += 0x10; // padding, SSbd, size
                                Buffer = reader.ReadBytes(BlockSize - 0x39);
                            }
                            else
                            {
                                Buffer = reader.ReadBytes(BlockSize - 0x11);
                            }
                            AudioTracks[CurrentTrack].DataList.AddRange(Buffer);
                        }
                        else if (VideoStreamBlockIDs.Contains(Header))
                        {
                            ushort BlockSize = reader2.ReadUInt16();
                            reader.BaseStream.Position += BlockSize;
                        }
                        else if (Header == (uint)PSS_SectionType.SystemHeader || Header == (uint)PSS_SectionType.PaddingStream)
                        {
                            ushort BlockSize = reader2.ReadUInt16();
                            reader.BaseStream.Position += BlockSize;
                        }
                        else
                        {
                            throw new Exception($"PSS: Unknown block ID {Header:X8}.");
                        }
                        break;
                    }

                    if (reader.BaseStream.Position < reader.BaseStream.Length)
                        Header = reader2.ReadUInt32();
                }
            }
        }

        static List<uint> AudioStreamBlockIDs = new List<uint>()
        {
            0x01C0,
            0x01C1,
            0x01C2,
            0x01C3,
            0x01C4,
            0x01C5,
            0x01C6,
            0x01C7,
            0x01C8,
            0x01C9,
            0x01CA,
            0x01CB,
            0x01CC,
            0x01CD,
            0x01CE,
            0x01CF,
            0x01D0,
            0x01D1,
            0x01D2,
            0x01D3,
            0x01D4,
            0x01D5,
            0x01D6,
            0x01D7,
            0x01D8,
            0x01D9,
            0x01DA,
            0x01DB,
            0x01DC,
            0x01DD,
            0x01DE,
            0x01DF,

            0x01BD,
            0x01BF,
        };
        static List<uint> VideoStreamBlockIDs = new List<uint>()
        {
            0x01E0,
            0x01E1,
            0x01E2,
            0x01E3,
            0x01E4,
            0x01E5,
            0x01E6,
            0x01E7,
            0x01E8,
            0x01E9,
            0x01EA,
            0x01EB,
            0x01EC,
            0x01ED,
            0x01EE,
            0x01EF,
        };
        enum PSS_SectionType
        {
            FileStart = 0x000001BA,
            FileEnd = 0x000001B9,
            SystemHeader = 0x000001BB,
            PaddingStream = 0x000001BE,
        }

        public List<AudioTrack> AudioTracks = new List<AudioTrack>() { new AudioTrack()};

        public class AudioTrack
        {
            public uint TrackID;
            public uint Codec; // 1 - PCM16LE
            public uint SampleRate;
            public uint Channels;
            public uint Interleave;
            public List<byte> DataList = new List<byte>();
            public bool IsSideL = true;
        }
    }
    

    public class BinaryReader2 : BinaryReader
    {
        public BinaryReader2(System.IO.Stream stream) : base(stream) { }

        public override int ReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override Int16 ReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override Int64 ReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public override UInt32 ReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        public override UInt16 ReadUInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        public override float ReadSingle()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

    }

}