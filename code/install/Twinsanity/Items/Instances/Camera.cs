using System.IO;
using System.Collections.Generic;
using System;

namespace Twinsanity
{
    public class Camera : TwinsItem
    {

        public uint Header { get; set; }
        public uint Enabled { get; set; }
        public float SomeFloat { get; set; }
        public Pos[] Coords { get; set; } = new Pos[3]; // rot/pos/size
        public uint SectionHead { get; set; }
        public List<ushort> Instances { get; set; }

        public ushort CamHeader { get; set; }
        public ushort CamHeader2 { get; set; }
        public ushort UnkShort { get; set; }
        public float UnkFloat1 { get; set; }
        public Pos UnkCoords1 { get; set; }
        public Pos UnkCoords2 { get; set; }
        public float? UnkFloat2 { get; set; }
        public float? UnkFloat3 { get; set; }
        public uint? UnkUInt1 { get; set; }
        public uint? UnkUInt2 { get; set; }
        public uint? UnkUInt3 { get; set; }
        public uint? UnkUInt4 { get; set; }
        public int? UnkInt5 { get; set; }
        public int? UnkInt6 { get; set; }
        public float? UnkFloat4 { get; set; }
        public float? UnkFloat5 { get; set; }
        public float? UnkFloat6 { get; set; }
        public float? UnkFloat7 { get; set; }
        public uint? UnkUInt7 { get; set; }
        public int? UnkInt8 { get; set; }
        public uint? UnkUInt9 { get; set; }
        public float? UnkFloat8 { get; set; }
        public uint CameraType1 { get; set; }
        public uint CameraType2 { get; set; }
        public object[] Cameras { get; set; } = new object[2];
        public byte UnkByte { get; set; }

        public bool[] Mask
        {
            get
            {
                bool[] m = new bool[9] { false, false, false, false, false, false, false, false, false };

                for (int i = 0; i < m.Length; i++)
                {
                    if ((Enabled >> i & 0x1) != 0)
                    {
                        m[i] = true;
                    }
                }

                return m;
            }
            set
            {
                Enabled = 0;

                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i])
                    {
                        Enabled |= (uint)(1 << i);
                    }
                }
            }
        }

        public enum CameraType : uint
        {
            None = 3,
            Boss = 0xA19,
            Point = 0x1C02,
            Line = 0x1C03,
            Path = 0x1C04,
            NULL = 0x1C05,
            Spline = 0x1C06,
            Unk1 = 0x1C09,
            Point2 = 0x1C0B,
            Unk2 = 0x1C0C,
            Line2 = 0x1C0D,
            NULL2 = 0x1C0E,
            Zone = 0x1C0F,
        }

        public override void Load(BinaryReader reader, int size)
        {
            Header = reader.ReadUInt32();
            Enabled = reader.ReadUInt32();
            SomeFloat = reader.ReadSingle();
            for (int i = 0; i < 3; ++i)
            {
                Coords[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            var n = reader.ReadInt32();
            n = reader.ReadInt32();
            SectionHead = reader.ReadUInt32();
            Instances = new List<ushort>(n);
            for (int i = 0; i < n; ++i)
                Instances.Add(reader.ReadUInt16());

            CamHeader = reader.ReadUInt16();
            CamHeader2 = reader.ReadUInt16();
            if (ParentType != SectionType.CameraDemo)
            {
                UnkShort = reader.ReadUInt16();
            }
            else
            {
                UnkShort = 0;
            }
            UnkFloat1 = reader.ReadSingle();

            UnkCoords1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnkCoords2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnkFloat2 = ReadFloatOrNull(reader);
            UnkFloat3 = ReadFloatOrNull(reader);
            UnkUInt1 = ReadUIntOrNull(reader);
            UnkUInt2 = ReadUIntOrNull(reader);
            UnkUInt3 = ReadUIntOrNull(reader);
            UnkUInt4 = ReadUIntOrNull(reader);
            UnkInt5 = ReadIntOrNull(reader);
            UnkInt6 = ReadIntOrNull(reader);
            UnkFloat4 = ReadFloatOrNull(reader);
            UnkFloat5 = ReadFloatOrNull(reader);
            UnkFloat6 = ReadFloatOrNull(reader);
            UnkFloat7 = ReadFloatOrNull(reader);
            UnkUInt7 = ReadUIntOrNull(reader);
            UnkInt8 = ReadIntOrNull(reader);
            UnkUInt9 = ReadUIntOrNull(reader);
            UnkFloat8 = ReadFloatOrNull(reader);

            CameraType1 = reader.ReadUInt32();
            CameraType2 = reader.ReadUInt32();

            if (ParentType != SectionType.CameraDemo)
            {
                UnkByte = reader.ReadByte();
            }
            else
            {
                UnkByte = 0;
            }

            if (CameraType1 != 3)
            {
                ReadCamera(reader, CameraType1, 0);
            }
            if (CameraType2 != 3)
            {
                ReadCamera(reader, CameraType2, 1);
            }
        }

        private float? ReadFloatOrNull(BinaryReader reader)
        {
            float? Value;
            uint Test = reader.ReadUInt32();
            if (Test == 0xCDCDCDCD)
            {
                Value = null;
            }
            else
            {
                reader.BaseStream.Position -= 4;
                Value = reader.ReadSingle();
            }
            return Value;
        }
        private uint? ReadUIntOrNull(BinaryReader reader)
        {
            uint? Value;
            uint Test = reader.ReadUInt32();
            if (Test == 0xCDCDCDCD)
                Value = null;
            else
                Value = Test;
            return Value;
        }
        private int? ReadIntOrNull(BinaryReader reader)
        {
            int? Value;
            uint Test = reader.ReadUInt32();
            if (Test == 0xCDCDCDCD)
                Value = null;
            else
            {
                reader.BaseStream.Position -= 4;
                Value = reader.ReadInt32();
            }
            return Value;
        }

        private void ReadCamera(BinaryReader reader, uint Type, uint ID)
        {
            switch (Type)
            {
                default:
                    throw new NotImplementedException();
                case 0xA19:
                    Camera_Boss Camera1 = new Camera_Boss();

                    Camera1.unkInt = reader.ReadUInt32();
                    Camera1.unkFloat1 = reader.ReadSingle();
                    Camera1.unkFloat2 = reader.ReadSingle();
                    Camera1.unkMatrix1 = new Pos[4];
                    for (int i = 0; i < Camera1.unkMatrix1.Length; ++i)
                    {
                        Camera1.unkMatrix1[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera1.unkMatrix2 = new Pos[4];
                    for (int i = 0; i < Camera1.unkMatrix2.Length; ++i)
                    {
                        Camera1.unkMatrix2[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera1.unkVector = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Camera1.unkByte1 = reader.ReadByte();
                    Camera1.unkFloat3 = reader.ReadSingle();
                    Camera1.unkFloat4 = reader.ReadSingle();
                    Camera1.unkFloat5 = reader.ReadSingle();
                    Camera1.unkFloat6 = reader.ReadSingle();
                    Camera1.unkByte2 = reader.ReadByte();

                    Cameras[ID] = Camera1;
                    break;
                case 0x1C02:
                    Camera_Point Camera2 = new Camera_Point();

                    Camera2.unkInt = reader.ReadUInt32();
                    Camera2.unkFloat1 = reader.ReadSingle();
                    Camera2.unkFloat2 = reader.ReadSingle();
                    Camera2.unkVector = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                    Cameras[ID] = Camera2;
                    break;
                case 0x1C03:
                    Camera_Line Camera3 = new Camera_Line();

                    Camera3.unkInt = reader.ReadUInt32();
                    Camera3.unkFloat1 = reader.ReadSingle();
                    Camera3.unkFloat2 = reader.ReadSingle();
                    Camera3.unkBoundingBoxVector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Camera3.unkBoundingBoxVector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                    Cameras[ID] = Camera3;
                    break;
                case 0x1C04:
                    Camera_Path Camera4 = new Camera_Path();

                    Camera4.unkInt = reader.ReadUInt32();
                    Camera4.unkFloat1 = reader.ReadSingle();
                    Camera4.unkFloat2 = reader.ReadSingle();
                    uint VectorCount = reader.ReadUInt32();
                    Camera4.unkVectors = new Pos[VectorCount];
                    for (int i = 0; i < VectorCount; ++i)
                    {
                        Camera4.unkVectors[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera4.unkInt2 = reader.ReadInt32();
                    Camera4.unkData = reader.ReadBytes(Camera4.unkInt2 * 0x8);

                    Cameras[ID] = Camera4;
                    break;
                case 0x1C05:
                    Camera_0x1C05 Camera5 = new Camera_0x1C05();
                    Cameras[ID] = Camera5;
                    break;
                case 0x1C06:
                    Camera_Spline Camera6 = new Camera_Spline();

                    Camera6.unkInt = reader.ReadInt32();
                    Camera6.unkFloat1 = reader.ReadSingle();
                    Camera6.unkFloat2 = reader.ReadSingle();
                    Camera6.unkUInt = reader.ReadUInt32();
                    Camera6.unkFloat3 = reader.ReadSingle();
                    Camera6.unkVectors = new Pos[(Camera6.unkUInt + 1) * 2];
                    for (int i = 0; i < Camera6.unkVectors.Length; ++i)
                    {
                        Camera6.unkVectors[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera6.unkData = reader.ReadBytes((int)Camera6.unkUInt * 0x8);
                    Camera6.unkShort = reader.ReadUInt16();

                    Cameras[ID] = Camera6;
                    break;
                case 0x1C09:
                    Camera_0x1C09 Camera7 = new Camera_0x1C09();

                    Camera7.unkInt = reader.ReadUInt32();
                    Camera7.unkFloat1 = reader.ReadSingle();
                    Camera7.unkFloat2 = reader.ReadSingle();

                    Cameras[ID] = Camera7;
                    break;
                case 0x1C0B:
                    Camera_Point2 Camera8 = new Camera_Point2();

                    Camera8.unkInt = reader.ReadUInt32();
                    Camera8.unkFloat1 = reader.ReadSingle();
                    Camera8.unkFloat2 = reader.ReadSingle();
                    Camera8.unkVector = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Camera8.unkFloat3 = reader.ReadSingle();
                    Camera8.unkByte = reader.ReadByte();

                    Cameras[ID] = Camera8;
                    break;
                case 0x1C0C:
                    Camera_0x1C0C Camera9 = new Camera_0x1C0C();

                    Camera9.unkByte1 = reader.ReadByte();
                    Camera9.unkByte2 = reader.ReadByte();
                    Camera9.unkByte3 = reader.ReadByte();
                    Camera9.unkByte4 = reader.ReadByte();

                    Cameras[ID] = Camera9;
                    break;
                case 0x1C0D:
                    Camera_Line2 Camera10 = new Camera_Line2();

                    Camera10.unkInt = reader.ReadUInt32();
                    Camera10.unkFloat1 = reader.ReadSingle();
                    Camera10.unkFloat2 = reader.ReadSingle();
                    Camera10.unkBoundingBoxVector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Camera10.unkBoundingBoxVector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    Camera10.unkFloat3 = reader.ReadSingle();
                    Camera10.unkFloat4 = reader.ReadSingle();

                    Cameras[ID] = Camera10;
                    break;
                case 0x1C0E:
                    Camera_0x1C0E Camera11 = new Camera_0x1C0E();
                    Cameras[ID] = Camera11;
                    break;
                case 0x1C0F:
                    Camera_Zone Camera12 = new Camera_Zone();

                    //Camera12.unkData1 = reader.ReadBytes(0x50);
                    //Camera12.unkData2 = reader.ReadBytes(0x50);

                    Camera12.Data1_Vectors = new Pos[4];
                    for (int i = 0; i < Camera12.Data1_Vectors.Length; i++)
                    {
                        Camera12.Data1_Vectors[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera12.Data1_unkInt1 = reader.ReadUInt32();
                    Camera12.Data1_unkInt2 = reader.ReadUInt32();
                    Camera12.Data1_padding = reader.ReadUInt64();

                    Camera12.Data2_Vectors = new Pos[4];
                    for (int i = 0; i < Camera12.Data2_Vectors.Length; i++)
                    {
                        Camera12.Data2_Vectors[i] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    Camera12.Data2_unkInt1 = reader.ReadUInt32();
                    Camera12.Data2_unkInt2 = reader.ReadUInt32();
                    Camera12.Data2_padding = reader.ReadUInt64();

                    Cameras[ID] = Camera12;
                    break;

            }
        }

        public class Camera_Boss // Boss Camera (Centers on Boss) 0xA19
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos[] unkMatrix1; // 4
            public Pos[] unkMatrix2; // 4
            public Pos unkVector;
            public byte unkByte1;
            public float unkFloat3;
            public float unkFloat4;
            public float unkFloat5;
            public float unkFloat6;
            public byte unkByte2;
        }

        public class Camera_Point // Point 0x1C02
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos unkVector;
        }

        public class Camera_Line // Line 0x1C03
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos unkBoundingBoxVector1;
            public Pos unkBoundingBoxVector2;
        }

        public class Camera_Path // Polygonal chain 0x1C04
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos[] unkVectors; // uint vectorAmount 
            public int unkInt2;
            public byte[] unkData; //unkInt2 * 0x8
        }

        public class Camera_0x1C05 // NULL 0x1C05
        {
            // Can't be read because methods are set to NULL
        }

        public class Camera_Spline // Spline 0x1C06
        {
            public int unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public uint unkUInt;
            public float unkFloat3;
            public Pos[] unkVectors; // unkInt2 * 2
            public byte[] unkData; // unkInt2 * 0x8
            public ushort unkShort;
        }

        public class Camera_0x1C09
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
        }

        public class Camera_Point2 // Point (Ukafight) 0x1C0B
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos unkVector;
            public float unkFloat3;
            public byte unkByte;
        }

        public class Camera_0x1C0C
        {
            public byte unkByte1;
            public byte unkByte2;
            public byte unkByte3;
            public byte unkByte4;
        }

        public class Camera_Line2 // Line (Gpa12/Throne) 0x1C0D
        {
            public uint unkInt;
            public float unkFloat1;
            public float unkFloat2;
            public Pos unkBoundingBoxVector1;
            public Pos unkBoundingBoxVector2;
            public float unkFloat3;
            public float unkFloat4;
        }

        public class Camera_0x1C0E // Empty 0x1C0E
        {
            // Nothing, method empty
        }

        public class Camera_Zone // Zone 0x1C0F
        {
            //public byte[] unkData1; //0x50
            //public byte[] unkData2; //0x50

            public Pos[] Data1_Vectors; //4
            public uint Data1_unkInt1;
            public uint Data1_unkInt2;
            public ulong Data1_padding;

            public Pos[] Data2_Vectors; //4
            public uint Data2_unkInt1;
            public uint Data2_unkInt2;
            public ulong Data2_padding;
        }
    }
}
