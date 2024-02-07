using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Twinsanity.Video
{
    public class XMV_Video : TwinsItem
    {
        public List<AudioTrack> AudioTracks = new List<AudioTrack>();

        public override void Load(BinaryReader reader, int length)
        {
            uint NextPacketSize = reader.ReadUInt32();
            uint PacketSize = reader.ReadUInt32();
            uint MaxPacksetSize = reader.ReadUInt32();
            uint IDcheck = reader.ReadUInt32();
            if (IDcheck != 0x58636F78) // "xobX"
            {
                throw new Exception("Wrong file header.");
            }
            uint Version = reader.ReadUInt32();
            uint Video_Width = reader.ReadUInt32();
            uint Video_Height = reader.ReadUInt32();
            uint Video_Duration = reader.ReadUInt32();
            ushort AudioTrackCount = reader.ReadUInt16();
            reader.ReadBytes(2);
            for (int i = 0; i < AudioTrackCount; i++)
            {
                var track = new AudioTrack();
                track.Compression = reader.ReadUInt16();
                track.Channels = reader.ReadUInt16();
                track.SampleRate = reader.ReadUInt32();
                track.BitsPerSample = reader.ReadUInt16();
                track.Flags = reader.ReadUInt16();
                AudioTracks.Add(track);
            }
            long PacketLength = PacketSize - reader.BaseStream.Position;

            while (PacketSize != 0)
            {
                PacketSize = reader.ReadUInt32();
                ulong Header = reader.ReadUInt32();
                var VideoDataSize = (uint)(Header & 0x7FFFFF);
                uint Header2 = reader.ReadUInt32();
                var VideoFrameCount = Header2 & 0xFF;
                bool VideoHasExtraData = (Header & 0x800000) != 0;
                for (int i = 0; i < AudioTrackCount; i++)
                {
                    uint AudioHeader = reader.ReadUInt32();
                    var AudioDataSize = (uint)(Header & 0x7FFFFF);
                }
                for (int i = 0 ; i < VideoFrameCount; i++)
                {

                }
            }

        }

        public class AudioTrack
        {
            public ushort Compression;
            public ushort Channels;
            public uint SampleRate;
            public ushort BitsPerSample;
            public ushort Flags;
        }
    }
}