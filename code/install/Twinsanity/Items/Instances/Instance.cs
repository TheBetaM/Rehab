using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;

namespace Twinsanity
{
    public class Instance : TwinsItem
    {
        public Pos Pos { get; set; }
        public ushort RotX { get; set; }
        public ushort RotY { get; set; }
        public ushort RotZ { get; set; }
        public ushort COMRotX { get; set; }
        public ushort COMRotY { get; set; }
        public ushort COMRotZ { get; set; }
        public List<ushort> InstanceIDs { get; set; } = new List<ushort>();
        public List<ushort> PositionIDs { get; set; } = new List<ushort>();
        public List<ushort> PathIDs { get; set; } = new List<ushort>();
        public int SomeNum1 { get; set; }
        public int SomeNum2 { get; set; }
        public int SomeNum3 { get; set; }
        public ushort ObjectID { get; set; }
        public short RefList { get; set; }
        public short ScriptID { get; set; }
        public uint PHeader { get; set; } = 0;
        public uint Flags { get; set; } = 0;
        public List<uint> UnkI321 { get; set; } = new List<uint>();
        public List<float> UnkI322 { get; set; } = new List<float>();
        public List<uint> UnkI323 { get; set; } = new List<uint>();
        public byte[] Remain = new byte[0];

        public override void Load(BinaryReader reader, int size)
        {
            long start_pos = reader.BaseStream.Position;
            int n;
            InstanceIDs.Clear();
            PositionIDs.Clear();
            PathIDs.Clear();
            UnkI321.Clear();
            UnkI322.Clear();
            UnkI323.Clear();

            Pos = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            RotX = reader.ReadUInt16();
            COMRotX = reader.ReadUInt16();
            RotY = reader.ReadUInt16();
            COMRotY = reader.ReadUInt16();
            RotZ = reader.ReadUInt16();
            COMRotZ = reader.ReadUInt16();

            n = reader.ReadInt32();
            n = reader.ReadInt32();
            SomeNum1 = reader.ReadInt32();
            for (int i = 0; i < n; ++i)
                InstanceIDs.Add(reader.ReadUInt16());
            n = reader.ReadInt32();
            n = reader.ReadInt32();
            SomeNum2 = reader.ReadInt32();
            for (int i = 0; i < n; ++i)
                PositionIDs.Add(reader.ReadUInt16());
            n = reader.ReadInt32();
            n = reader.ReadInt32();
            SomeNum3 = reader.ReadInt32();
            for (int i = 0; i < n; ++i)
                PathIDs.Add(reader.ReadUInt16());
            ObjectID = reader.ReadUInt16();
            RefList = reader.ReadInt16();
            ScriptID = reader.ReadInt16();
            if (ParentType == SectionType.ObjectInstance)
            {
                PHeader = reader.ReadUInt32();
                Flags = reader.ReadUInt32();
                n = reader.ReadInt32();
                for (int i = 0; i < n; ++i)
                    UnkI321.Add(reader.ReadUInt32());
                n = reader.ReadInt32();
                for (int i = 0; i < n; ++i)
                    UnkI322.Add(reader.ReadSingle());
                n = reader.ReadInt32();
                for (int i = 0; i < n; ++i)
                    UnkI323.Add(reader.ReadUInt32());
            }
            else if (ParentType == SectionType.ObjectInstanceDemo)
            {
                var Count_Flags = reader.ReadByte();
                var Count_Floats = reader.ReadByte();
                var Count_Ints = reader.ReadByte();
                reader.ReadByte();

                Flags = reader.ReadUInt32();
                UnkI321.Clear();
                for (int i = 0; i < Count_Flags; ++i)
                    UnkI321.Add(reader.ReadUInt32());
                UnkI322.Clear();
                for (int i = 0; i < Count_Floats; ++i)
                    UnkI322.Add(reader.ReadSingle());
                UnkI323.Clear();
                for (int i = 0; i < Count_Ints; ++i)
                    UnkI323.Add(reader.ReadUInt32());
            }
            else
            {
                Remain = reader.ReadBytes((int)((start_pos + size) - reader.BaseStream.Position));
            }
        }

    }
}
