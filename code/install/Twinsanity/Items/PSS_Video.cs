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
            uint Header = reader.ReadUInt32();
            while (Header != (uint)PSS_SectionType.MPEG_PROGRAM_END)
            {
                switch (Header)
                {
                    case (uint)PSS_SectionType.MPEG_PACK_HEAD:
                    {
                        uint id = reader.ReadUInt32();
                        byte[] clockRef = reader.ReadBytes(6);
                        byte[] muxRate = reader.ReadBytes(3);
                        byte padding = reader.ReadByte();
                    }
                    break;
                    case (uint)PSS_SectionType.MPEG_VIDEO:
                    break;
                    case (uint)PSS_SectionType.MPEG_PRIVATE_STREAM1:
                    break;
                    case (uint)PSS_SectionType.MPEG_SYSTEM_HEAD:
                    break;
                    case (uint)PSS_SectionType.MPEG_PADDING:
                    {
                        uint id = reader.ReadUInt32();
                        ushort size = reader.ReadUInt16();
                    }
                    break;
                    default:
                    throw new Exception("Bad PSS header.");
                }
                Header = reader.ReadUInt32();
            }
        }

        enum PSS_SectionType
        {
            MPEG_SEQUENCE_HEAD = 0x000001B3,
            MPEG_SEQUENCE_END =	0x000001B7,
            MPEG_PROGRAM_END = 0x000001B9,
            MPEG_PACK_HEAD = 0x000001BA,
            MPEG_SYSTEM_HEAD = 0x000001BB,
            MPEG_PRIVATE_STREAM1 = 0x000001BD,
            MPEG_PADDING = 0x000001BE,
            MPEG_VIDEO = 0x000001E0,
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

        public override float ReadSingle()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

    }

}