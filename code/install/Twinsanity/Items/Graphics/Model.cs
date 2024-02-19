using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Twinsanity.VIF;
using System.Linq;

namespace Twinsanity
{
    public class Model : TwinsItem
    {
        public long ItemSize { get; set; }

        public List<SubModel> SubModels { get; set; } = new List<SubModel>();

        public override void Load(BinaryReader reader, int size)
        {
            var sk = reader.BaseStream.Position;
            var count = reader.ReadInt32();

            SubModels.Clear();
            for (int i = 0; i < count; i++)
            {
                var sub = new SubModel();

                sub.VertexCount = (int)reader.ReadUInt32();
                int vertexLen = reader.ReadInt32();
                sub.VifCode = reader.ReadBytes(vertexLen);
                int blobLen = reader.ReadInt32();
                sub.UnusedBlob = reader.ReadBytes(blobLen);

                sub.Vertexes = CalculateData(sub);

                SubModels.Add(sub);
            }

            ItemSize = size;

            //Console.WriteLine("end pos: " + (reader.BaseStream.Position - sk) + " target: " + size);

            //Remain = reader.ReadBytes((size) - (int)(reader.BaseStream.Position - sk));

        }

        [Flags]
        private enum FieldsPresent
        {
            Vertex = 0,
            UV_Color = 1,
            Normals = 2,
            EmitColors = 4
        }

        public List<VertexData> CalculateData(SubModel model)
        {
            var vertexes = new List<VertexData>();

            var interpreter = VIFInterpreter.InterpretCode(model.VifCode);
            var data = interpreter.GetMem();
            var Vertexes = new List<Vector4>();
            var UVW = new List<Vector4>();
            var EmitColor = new List<Vector4>();
            var Colors = new List<Color>();
            var Normals = new List<Vector4>();
            var Connection = new List<bool>();
            var index = 0;
            for (var i = 0; i < data.Count;)
            {
                var verts = (data[i][0].GetBinaryX() & 0xFF);
                var fieldsPresent = FieldsPresent.Vertex;
                var outputAddr = interpreter.GetAddressOutput();
                var fields = 0;
                foreach (var addr in outputAddr[index++])
                {
                    switch (addr)
                    {
                        case 0x3:
                            fieldsPresent |= FieldsPresent.Vertex;
                            fields++;
                            break;
                        case 0x4:
                            fieldsPresent |= FieldsPresent.UV_Color;
                            fields++;
                            break;
                        case 0x5:
                            fieldsPresent |= FieldsPresent.Normals;
                            fields++;
                            break;
                        case 0x6:
                            fieldsPresent |= FieldsPresent.EmitColors;
                            fields++;
                            break;
                    }
                    if (i + fields + 2 >= data.Count)
                        break;

                }
                Vertexes.AddRange(data[i + 2].Where((v) => v != null));
                if (fieldsPresent.HasFlag(FieldsPresent.UV_Color))
                {
                    var uv_con = data[i + 3].Where((v) => v != null);
                    foreach (var e in uv_con)
                    {
                        var conn = (e.GetBinaryW() & 0xFF00) >> 8;
                        Connection.Add(conn == 128 ? false : true);
                        var r = Math.Min(e.GetBinaryX() & 0xFF, 255);
                        var g = Math.Min(e.GetBinaryY() & 0xFF, 255);
                        var b = Math.Min(e.GetBinaryZ() & 0xFF, 255);
                        var a = (e.GetBinaryW() & 0xFF) << 1;

                        Color col = new Color((byte)r, (byte)g, (byte)b, (byte)a);
                        Colors.Add(col);

                        Vector4 uv = new Vector4(e);
                        uv.SetBinaryX(uv.GetBinaryX() & 0xFFFFFF00);
                        uv.SetBinaryY(uv.GetBinaryY() & 0xFFFFFF00);
                        uv.SetBinaryZ(uv.GetBinaryZ() & 0xFFFFFF00);
                        uv.Y = 1 - uv.Y;
                        UVW.Add(uv);
                    }
                }
                if (fieldsPresent.HasFlag(FieldsPresent.Normals))
                {
                    foreach (var e in data[i + 4])
                    {
                        if (e == null)
                            break;
                        Normals.Add(new Vector4(e.X, e.Y, e.Z, 1.0f));
                    }
                }
                if (fieldsPresent.HasFlag(FieldsPresent.EmitColors))
                {
                    foreach (var e in data[i + fields + 1])
                    {
                        if (e == null)
                            break;
                        Vector4 emit = new Vector4(e);
                        emit.X = (emit.GetBinaryX() & 0xFF);// / 256.0f;
                        emit.Y = (emit.GetBinaryY() & 0xFF);// / 256.0f;
                        emit.Z = (emit.GetBinaryZ() & 0xFF);// / 256.0f;
                        emit.W = (emit.GetBinaryW() & 0xFF);// / 256.0f;
                        EmitColor.Add(emit);
                    }
                }
                i += fields + 2;
                TrimList(UVW, Vertexes.Count);
                TrimList(EmitColor, Vertexes.Count);
                TrimList(Normals, Vertexes.Count, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            }

            for (int i = 0; i < Vertexes.Count; i++)
            {
                var vertData = new VertexData
                {
                    X = Vertexes[i].X,
                    Y = Vertexes[i].Y,
                    Z = Vertexes[i].Z,
                    U = UVW[i].X,
                    V = UVW[i].Y,
                    R = Colors[i].R,
                    G = Colors[i].G,
                    B = Colors[i].B,
                    A = Colors[i].A,
                    NX = Normals[i].X,
                    NY = Normals[i].Y,
                    NZ = Normals[i].Z,
                    ER = (byte)EmitColor[i].X,
                    EG = (byte)EmitColor[i].Y,
                    EB = (byte)EmitColor[i].Z,
                    EA = (byte)EmitColor[i].W,
                    Conn = Connection[i]
                };
                vertexes.Add(vertData);
            }

            return vertexes;
        }

        private void TrimList(List<Vector4> list, Int32 desiredLength, Vector4 defaultValue = null)
        {
            if (list != null)
            {
                if (list.Count > desiredLength)
                {
                    list.RemoveRange(desiredLength, list.Count - desiredLength);
                }
                while (list.Count < desiredLength)
                {
                    if (defaultValue != null)
                    {
                        list.Add(new Vector4(defaultValue));
                    }
                    else
                    {
                        list.Add(new Vector4());
                    }
                }
            }
        }

        #region STRUCTURES
        public struct SubModel
        {
            // Primary Header
            public int VertexCount;
            public Byte[] VifCode { get; set; }
            public Byte[] UnusedBlob { get; set; }
            public List<VertexData> Vertexes;
        }
        public struct VertexData
        {
            public float X, Y, Z;
            public float NX, NY, NZ;
            public float U, V;
            public byte R, G, B, A;
            public byte ER, EG, EB, EA; // Emit colors
            public bool Conn;
        }
        #endregion
    }
}
