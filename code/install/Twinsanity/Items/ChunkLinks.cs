using System.Collections.Generic;
using System.IO;
using System;

namespace Twinsanity
{
    public class ChunkLinks : TwinsItem
    {
        public List<ChunkLink> Links { get; set; } = new List<ChunkLink>();

        public override void Load(BinaryReader reader, int size)
        {
            Links.Clear();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                ChunkLink link = new ChunkLink(reader.ReadInt32(), new string(reader.ReadChars(reader.ReadInt32())), reader.ReadUInt32());
                for (int j = 0; j < 4; ++j)
                {
                    link.ObjectMatrix[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                for (int j = 0; j < 4; ++j)
                {
                    link.ChunkMatrix[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
                if ((link.Flags & 0x80000) != 0)
                {
                    for (int j = 0; j < 4; ++j)
                    {
                        link.LoadWall[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                }

                link.TreeRoot = ReadTree(reader, link.Type);

                Links.Add(link);
            }

            //Console.WriteLine("end pos: " + (reader.BaseStream.Position - start_pos) + " target: " + size);

        }

        private ChunkLink.LinkTree ReadTree(BinaryReader reader, int Head)
        {
            if ((Head & 0x1) == 0)
            {
                return null;
            }

            ChunkLink.LinkTree Node = new ChunkLink.LinkTree
            {
                Header = reader.ReadInt32()
            };

            ushort[] header = new ushort[11];
            for (var i = 0; i < 11; ++i)
            {
                header[i] = reader.ReadUInt16();
            }
            int blobSize = reader.ReadInt32();

            for (int j = 0; j < 8; ++j)
            {
                Node.LoadArea[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            for (int j = 0; j < 6; ++j)
            {
                Node.AreaMatrix[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            for (int j = 0; j < 6; ++j)
            {
                Node.UnknownMatrix[j] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            byte[] Blob = reader.ReadBytes(blobSize - 320);
            Node.GI_Type = new GraphicsInfo.GI_CollisionData() { Header = header, collisionDataBlob = Blob };

            Node.Next = ReadTree(reader, Node.Header);

            return Node;
        }

        #region STRUCTURES
        public class ChunkLink
        {
            public ChunkLink(int type, string path, uint flags)
            {
                Type = type;
                Path = path;
                Flags = flags;
                ObjectMatrix = new Pos[4];
                ChunkMatrix = new Pos[4];
                LoadWall = new Pos[4];
                for (int i = 0; i < 4; ++i)
                {
                    ObjectMatrix[i] = new Pos(0, 0, 0, 1);
                    ChunkMatrix[i] = new Pos(0, 0, 0, 1);
                    LoadWall[i] = new Pos(0, 0, 0, 1);
                }
            }

            public int Type;
            public string Path;
            public uint Flags;
            public Pos[] ObjectMatrix; // 4
            public Pos[] ChunkMatrix; // 4
            public Pos[] LoadWall; // 4
            public LinkTree TreeRoot;

            public bool IsVisible => (Flags & 0x1) != 0 || (Flags & 0x2) != 0;
            public bool Flag1 => (Flags & 0x1) != 0;
            public bool Flag2 => (Flags & 0x2) != 0;
            public bool Flag3 => (Flags & 0x80) != 0;
            public bool WallIsEnabled => (Flags & 0x100) != 0;
            public bool HasWall => (Flags & 0x80000) != 0;
            public bool HasTree => (Type & 0x1) != 0;

            public class LinkTree
            {
                public LinkTree()
                {
                    LoadArea = new Pos[8];
                    AreaMatrix = new Pos[6];
                    UnknownMatrix = new Pos[6];
                    for (int i = 0; i < 8; ++i)
                    {
                        LoadArea[i] = new Pos(0, 0, 0, 1);
                    }
                    for (int i = 0; i < 6; ++i)
                    {
                        AreaMatrix[i] = new Pos(0, 0, 0, 1);
                        UnknownMatrix[i] = new Pos(0, 0, 0, 1);
                    }
                    GI_Type = new GraphicsInfo.GI_CollisionData() { Header = new ushort[11] { 8, 12, 6, 3, 3, 128, 224, 272, 320, 326, 356 }, collisionDataBlob = new byte[60] { 0, 5, 10, 15, 20, 25, 4, 2, 3, 1, 0, 4, 4, 5, 3, 2, 4, 6, 7, 5, 4, 4, 0, 1, 7, 6, 4, 3, 5, 7, 1, 4, 4, 2, 0, 6, 0, 1, 1, 3, 3, 2, 2, 0, 3, 5, 5, 4, 4, 2, 5, 7, 7, 6, 6, 4, 7, 1, 0, 6, } };
                }

                public int Header; // count of leaf nodes attached to this
                public GraphicsInfo.GI_CollisionData GI_Type;
                public Pos[] LoadArea; // 8
                public Pos[] AreaMatrix; // 6
                public Pos[] UnknownMatrix; // 6

                public LinkTree Next;
            }
        }
        #endregion
    }
}
