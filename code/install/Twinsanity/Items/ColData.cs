using System.Collections.Generic;
using System.IO;

namespace Twinsanity
{
    public sealed class ColData : TwinsItem
    {
        public uint someNumber;
        private readonly uint mask = 0x3FFFF;

        public ColData()
        {
            Triggers = new List<Trigger>();
            Groups = new List<GroupInfo>();
            Tris = new List<ColTri>();
            Vertices = new List<Pos>();
        }

        /////////PARENTS FUNCTION//////////
        public override void Load(BinaryReader reader, int size)
        {
            if (size < 20)
            {
                return;
            }
            someNumber = reader.ReadUInt32();
            uint triggerCount = reader.ReadUInt32();
            uint groupCount = reader.ReadUInt32();
            uint triCount = reader.ReadUInt32();
            uint vertexCount = reader.ReadUInt32();
            Triggers.Clear();
            Groups.Clear();
            Tris.Clear();
            Vertices.Clear();
            for (int i = 0; i < triggerCount; i++)
            {
                Trigger trg = new Trigger
                {
                    X1 = reader.ReadSingle(),
                    Y1 = reader.ReadSingle(),
                    Z1 = reader.ReadSingle(),
                    Flag1 = reader.ReadInt32(),
                    X2 = reader.ReadSingle(),
                    Y2 = reader.ReadSingle(),
                    Z2 = reader.ReadSingle(),
                    Flag2 = reader.ReadInt32()
                };
                Triggers.Add(trg);
            }
            for (int i = 0; i < groupCount; i++)
            {
                GroupInfo grp = new GroupInfo
                {
                    Size = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32()
                };
                Groups.Add(grp);
            }
            for (int i = 0; i < triCount; i++)
            {
                ColTri tri = new ColTri();
                ulong legacy = reader.ReadUInt64();
                tri.Vert1 = (int)(legacy & mask);
                tri.Vert2 = (int)((legacy >> 18 * 1) & mask);
                tri.Vert3 = (int)((legacy >> 18 * 2) & mask);
                tri.Surface = (int)(legacy >> (18 * 3));
                Tris.Add(tri);
            }
            for (int i = 0; i < vertexCount; i++)
            {
                Pos vtx = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Vertices.Add(vtx);
            }
        }

        #region STRUCTURES
        public struct Trigger
        {
            public float X1;
            public float Y1;
            public float Z1;
            public int Flag1;
            public float X2;
            public float Y2;
            public float Z2;
            public int Flag2;
        }
        public struct GroupInfo
        {
            public uint Size;
            public uint Offset;
        }
        public struct ColTri
        {
            public int Vert1;
            public int Vert2;
            public int Vert3;
            public int Surface;
        }
        #endregion

        public List<Trigger> Triggers { get; set; }
        public List<GroupInfo> Groups { get; set; }
        public List<ColTri> Tris { get; set; }
        public List<Pos> Vertices { get; set; }
    }
}
