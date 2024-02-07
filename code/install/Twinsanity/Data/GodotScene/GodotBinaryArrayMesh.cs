using System;
using System.Collections.Generic;
using System.Numerics;
using Twinsanity;
using Twinsanity.Items;

namespace RehabSetup
{
    public class GodotBinaryArrayMesh : GodotBinaryResourceFile
    {

        public override string ResType => "ArrayMesh";

        public GodotBinaryArrayMesh()
        {

        }
        public GodotBinaryArrayMesh(ModelX Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x80000101B; // 4121 or 0x1019 no normals / 4123 or 0x101B normals
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();

                var Sub = Model.SubModels[i];
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                foreach (var vert in Sub.VData)
                {
                    byte R = vert.R;
                    byte G = vert.G;
                    byte B = vert.B;
                    byte A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    //byte[] NX = BitConverter.GetBytes(vert.NX);
                    //byte[] NY = BitConverter.GetBytes(vert.NY);
                    //byte[] NZ = BitConverter.GetBytes(vert.NZ);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;

                    VertexData.AddRange(X);
                    VertexData.AddRange(Y);
                    VertexData.AddRange(Z);
                    //VertexData.Add(255); 
                    //VertexData.Add(NX); 
                    //VertexData.Add(NY); 
                    //VertexData.Add(NZ); 
                    AttributeData.Add(R);
                    AttributeData.Add(G);
                    AttributeData.Add(B);
                    AttributeData.Add(A);
                    AttributeData.AddRange(UV_X);
                    AttributeData.AddRange(UV_Y);
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    VertexData.Add(255); 
                    VertexData.Add(NX); 
                    VertexData.Add(NY); 
                    VertexData.Add(NZ); 
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }
            
            res.Add("_surfaces", surfArray.ToArray());
            Resources.Add(res);
        }

        public GodotBinaryArrayMesh(SkinX Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x800001C1B; // 7193 or 0x1C19 no normals, 7195 or 0x1C1B normals
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();
                List<byte> SkinData = new();

                var Sub = Model.SubModels[i];
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                foreach (var vert in Sub.VData)
                {
                    byte R = vert.R;
                    byte G = vert.G;
                    byte B = vert.B;
                    byte A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    byte[] Bone1 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone2 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone3 = new byte[2] { 0x00, 0x00 };
                    int Joint1 = (vert.Joint1 - 16) / 4;
                    int Joint2 = (vert.Joint2 - 16) / 4;
                    int Joint3 = (vert.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint1]);
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint2]);
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint3]);
                    }
                    ushort ConvWeight1 = (ushort)(vert.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;
                    
                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }

                    VertexData.AddRange(X);
                    VertexData.AddRange(Y);
                    VertexData.AddRange(Z);
                    //VertexData.Add(255); 
                    //VertexData.Add(NX); 
                    //VertexData.Add(NY); 
                    //VertexData.Add(NZ); 
                    AttributeData.Add(R);
                    AttributeData.Add(G);
                    AttributeData.Add(B);
                    AttributeData.Add(A);
                    AttributeData.AddRange(UV_X);
                    AttributeData.AddRange(UV_Y);
                    SkinData.AddRange(Bone1);
                    SkinData.AddRange(Bone2);
                    SkinData.AddRange(Bone3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                    SkinData.AddRange(Weight1);
                    SkinData.AddRange(Weight2);
                    SkinData.AddRange(Weight3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    VertexData.Add(255); 
                    VertexData.Add(NX); 
                    VertexData.Add(NY); 
                    VertexData.Add(NZ); 
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("skin_data", SkinData.ToArray());
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }

            res.Add("_surfaces", surfArray.ToArray());
            Resources.Add(res);
        }

        public GodotBinaryArrayMesh(BlendSkinX Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();

            List<string> BlendShapeNames = new();
            for (int a = 0; a < Model.BlendShapeCount; a++)
            {
                BlendShapeNames.Add($"morph_{a}");
            }

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x800001C1B;
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();
                List<byte> SkinData = new();
                List<byte> BlendShapeData = new();

                var Sub = Model.SubModels[i];
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (int g = 0; g < Sub.GroupList.Count; g++)
                {
                    for (int a = 0; a < Sub.GroupList[g] - 2; ++a)
                    {
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 1 : a + 0)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 0 : a + 1)));
                        idx.Add((ushort)(VertCount + ((a & 0x1) == 0x1 ? a + 2 : a + 2)));
                    }
                    VertCount += (int)Sub.GroupList[g];
                }
                int GroupID = 0;
                int GroupVert = 0;
                foreach (var vert in Sub.VData)
                {
                    byte R = vert.R;
                    byte G = vert.G;
                    byte B = vert.B;
                    byte A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.UV_X);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.UV_Y);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    byte[] Bone1 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone2 = new byte[2] { 0x00, 0x00 };
                    byte[] Bone3 = new byte[2] { 0x00, 0x00 };
                    int Joint1 = (vert.Joint1 - 16) / 4;
                    int Joint2 = (vert.Joint2 - 16) / 4;
                    int Joint3 = (vert.Joint3 - 16) / 4;
                    if (Joint1 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone1 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint1]);
                    }
                    if (Joint2 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone2 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint2]);
                    }
                    if (Joint3 < Sub.GroupJoints[GroupID].Count)
                    {
                        Bone3 = BitConverter.GetBytes((ushort)Sub.GroupJoints[GroupID][Joint3]);
                    }
                    ushort ConvWeight1 = (ushort)(vert.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;

                    GroupVert++;
                    if (GroupVert > Sub.GroupList[GroupID] - 1)
                    {
                        GroupVert = 0;
                        GroupID++;
                    }

                    VertexData.AddRange(X);
                    VertexData.AddRange(Y);
                    VertexData.AddRange(Z);
                    //VertexData.Add(255); 
                    //VertexData.Add(NX); 
                    //VertexData.Add(NY); 
                    //VertexData.Add(NZ); 
                    AttributeData.Add(R);
                    AttributeData.Add(G);
                    AttributeData.Add(B);
                    AttributeData.Add(A);
                    AttributeData.AddRange(UV_X);
                    AttributeData.AddRange(UV_Y);
                    SkinData.AddRange(Bone1);
                    SkinData.AddRange(Bone2);
                    SkinData.AddRange(Bone3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                    SkinData.AddRange(Weight1);
                    SkinData.AddRange(Weight2);
                    SkinData.AddRange(Weight3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                }
                foreach (var vert in Sub.VData)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    VertexData.Add(255); 
                    VertexData.Add(NX); 
                    VertexData.Add(NY); 
                    VertexData.Add(NZ); 
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                for (int a = 0; a < Model.BlendShapeCount; a++)
                {
                    for (int x = 0; x < Sub.VData.Count; x++)
                    {
                        float BS_X = -Sub.VData[x].BlendShapes[a].X;
                        float BS_Y = Sub.VData[x].BlendShapes[a].Y;
                        float BS_Z = Sub.VData[x].BlendShapes[a].Z;
                        byte[] BSX = BitConverter.GetBytes(BS_X);
                        byte[] BSY = BitConverter.GetBytes(BS_Y);
                        byte[] BSZ = BitConverter.GetBytes(BS_Z);
                        BlendShapeData.AddRange(BSX);
                        BlendShapeData.AddRange(BSY);
                        BlendShapeData.AddRange(BSZ);
                    }
                    for (int x = 0; x < Sub.VData.Count; x++)
                    {
                        byte NX = (byte)(Sub.VData[x].NX * 127);
                        byte NY = (byte)(Sub.VData[x].NY * 127);
                        byte NZ = (byte)(Sub.VData[x].NZ * 127);
                        BlendShapeData.Add(255);
                        BlendShapeData.Add(NX);
                        BlendShapeData.Add(NY);
                        BlendShapeData.Add(NZ);
                    }
                }

                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("blend_shapes", BlendShapeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("skin_data", SkinData.ToArray());
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }

            res.Add("_blend_shape_names", BlendShapeNames.ToArray());
            res.Add("_surfaces", surfArray.ToArray());
            res.Add("blend_shape_mode", 0);
            Resources.Add(res);
        }

        public GodotBinaryArrayMesh(Model Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x80000101B; // 4121 no normals / 4123 normals
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();

                var Sub = Model.SubModels[i];
                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)(refIndex + 2));
                            }
                            else
                            {
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 2));
                            }
                        }
                        ++refIndex;
                    }
                    VertCount++;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    byte R = vert.R;
                    byte G = vert.G;
                    byte B = vert.B;
                    byte A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.U);
                    byte[] UV_Y = BitConverter.GetBytes(vert.V);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    //byte NX = (byte)(vert.NX * 127);
                    //byte NY = (byte)(vert.NY * 127);
                    //byte NZ = (byte)(vert.NZ * 127);
                    //byte[] NX = BitConverter.GetBytes(-vert.NX);
                    //byte[] NY = BitConverter.GetBytes(vert.NY);
                    //byte[] NZ = BitConverter.GetBytes(vert.NZ);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;

                    VertexData.AddRange(X);
                    VertexData.AddRange(Y);
                    VertexData.AddRange(Z);
                    //VertexData.Add(255); 
                    //VertexData.Add(NX); 
                    //VertexData.Add(NY); 
                    //VertexData.Add(NZ); 
                    AttributeData.Add(R);
                    AttributeData.Add(G);
                    AttributeData.Add(B);
                    AttributeData.Add(A);
                    AttributeData.AddRange(UV_X);
                    AttributeData.AddRange(UV_Y);
                }
                foreach (var vert in Sub.Vertexes)
                {
                    byte NX = (byte)(vert.NX * 127);
                    byte NY = (byte)(vert.NY * 127);
                    byte NZ = (byte)(vert.NZ * 127);
                    VertexData.Add(255); 
                    VertexData.Add(NX); 
                    VertexData.Add(NY); 
                    VertexData.Add(NZ); 
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }

            res.Add("_surfaces", surfArray.ToArray());
            Resources.Add(res);
        }

        public GodotBinaryArrayMesh(Skin Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();     

            for (int i = 0; i < Model.SubModels.Count; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x800001C19;
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();
                List<byte> SkinData = new();

                var Sub = Model.SubModels[i];
                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                for (var j = 0; j < Sub.Vertexes.Count; ++j)
                {
                    if (j < Sub.Vertexes.Count - 2)
                    {
                        if (Sub.Vertexes[j + 2].Conn)
                        {
                            if ((/*offset +*/ j) % 2 == 0)
                            {
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)(refIndex + 2));
                            }
                            else
                            {
                                idx.Add((ushort)(refIndex + 1));
                                idx.Add((ushort)refIndex);
                                idx.Add((ushort)(refIndex + 2));
                            }
                        }
                        ++refIndex;
                    }
                    VertCount++;
                }
                foreach (var vert in Sub.Vertexes)
                {
                    byte R = vert.R;
                    byte G = vert.G;
                    byte B = vert.B;
                    byte A = vert.A;
                    byte[] UV_X = BitConverter.GetBytes(vert.U);
                    byte[] UV_Y = BitConverter.GetBytes(-vert.V);
                    byte[] X = BitConverter.GetBytes(-vert.X);
                    byte[] Y = BitConverter.GetBytes(vert.Y);
                    byte[] Z = BitConverter.GetBytes(vert.Z);
                    byte[] Bone1 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex1);
                    byte[] Bone2 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex2);
                    byte[] Bone3 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex3);
                    ushort ConvWeight1 = (ushort)(vert.Joint.Weight1 * 65535);
                    ushort ConvWeight2 = (ushort)(vert.Joint.Weight2 * 65535);
                    ushort ConvWeight3 = (ushort)(vert.Joint.Weight3 * 65535);
                    byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                    byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                    byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                    if (-vert.X < MinX)
                        MinX = -vert.X;
                    if (-vert.X > MaxX)
                        MaxX = -vert.X;
                    if (vert.Y < MinY)
                        MinY = vert.Y;
                    if (vert.Y > MaxY)
                        MaxY = vert.Y;
                    if (vert.Z < MinZ)
                        MinZ = vert.Z;
                    if (vert.Z > MaxZ)
                        MaxZ = vert.Z;

                    VertexData.AddRange(X);
                    VertexData.AddRange(Y);
                    VertexData.AddRange(Z); 
                    AttributeData.Add(R);
                    AttributeData.Add(G);
                    AttributeData.Add(B);
                    AttributeData.Add(A);
                    AttributeData.AddRange(UV_X);
                    AttributeData.AddRange(UV_Y);
                    SkinData.AddRange(Bone1);
                    SkinData.AddRange(Bone2);
                    SkinData.AddRange(Bone3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                    SkinData.AddRange(Weight1);
                    SkinData.AddRange(Weight2);
                    SkinData.AddRange(Weight3);
                    SkinData.Add(0);
                    SkinData.Add(0);
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("skin_data", SkinData.ToArray());
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }

            res.Add("_surfaces", surfArray.ToArray());
            Resources.Add(res);
        }

        public GodotBinaryArrayMesh(BlendSkin Model)
        {
            var res = new Resource(ResType, $"local://{ResType}_aaaaa");
            var surfArray = new List<object>();

            List<string> BlendShapeNames = new();
            for (int a = 0; a < Model.BlendShapeCount; a++)
            {
                BlendShapeNames.Add($"morph_{a}");
            }

            for (int i = 0; i < Model.Models.Length; i++)
            {
                var dict = new Dictionary<object, object>();
                AABB boundingBox = new AABB();
                List<byte> AttributeData = new();
                ulong format = 0x800001C19;
                int index_count = 0;
                List<byte> IndexData = new();
                int primitive = 3;
                int vertex_count = 0;
                List<byte> VertexData = new();
                List<byte> SkinData = new();
                List<byte> BlendShapeData = new();

                int refIndex = 0;
                int VertCount = 0;
                float MinX = 99999f;
                float MaxX = -99999f;
                float MinY = 99999f;
                float MaxY = -99999f;
                float MinZ = 99999f;
                float MaxZ = -99999f;
                List<ushort> idx = new();
                foreach (var Sub in Model.Models[i].SubModels)
                {
                    for (var j = 0; j < Sub.Vertexes.Count; ++j)
                    {
                        if (j < Sub.Vertexes.Count - 2)
                        {
                            if (Sub.Vertexes[j + 2].Conn)
                            {
                                if ((/*offset +*/ j) % 2 == 0)
                                {
                                    idx.Add((ushort)refIndex);
                                    idx.Add((ushort)(refIndex + 1));
                                    idx.Add((ushort)(refIndex + 2));
                                }
                                else
                                {
                                    idx.Add((ushort)(refIndex + 1));
                                    idx.Add((ushort)refIndex);
                                    idx.Add((ushort)(refIndex + 2));
                                }
                            }
                            ++refIndex;
                        }
                        VertCount++;
                    }
                    refIndex += 2;
                    foreach (var vert in Sub.Vertexes)
                    {
                        byte R = vert.R;
                        byte G = vert.G;
                        byte B = vert.B;
                        byte A = vert.A;
                        byte[] UV_X = BitConverter.GetBytes(vert.U);
                        byte[] UV_Y = BitConverter.GetBytes(-vert.V);
                        byte[] X = BitConverter.GetBytes(-vert.X);
                        byte[] Y = BitConverter.GetBytes(vert.Y);
                        byte[] Z = BitConverter.GetBytes(vert.Z);
                        byte[] Bone1 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex1);
                        byte[] Bone2 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex2);
                        byte[] Bone3 = BitConverter.GetBytes((ushort)vert.Joint.JointIndex3);
                        ushort ConvWeight1 = (ushort)(vert.Joint.Weight1 * 65535);
                        ushort ConvWeight2 = (ushort)(vert.Joint.Weight2 * 65535);
                        ushort ConvWeight3 = (ushort)(vert.Joint.Weight3 * 65535);
                        byte[] Weight1 = BitConverter.GetBytes(ConvWeight1);
                        byte[] Weight2 = BitConverter.GetBytes(ConvWeight2);
                        byte[] Weight3 = BitConverter.GetBytes(ConvWeight3);
                        if (-vert.X < MinX)
                            MinX = -vert.X;
                        if (-vert.X > MaxX)
                            MaxX = -vert.X;
                        if (vert.Y < MinY)
                            MinY = vert.Y;
                        if (vert.Y > MaxY)
                            MaxY = vert.Y;
                        if (vert.Z < MinZ)
                            MinZ = vert.Z;
                        if (vert.Z > MaxZ)
                            MaxZ = vert.Z;
                        
                        VertexData.AddRange(X);
                        VertexData.AddRange(Y);
                        VertexData.AddRange(Z);
                        AttributeData.Add(R);
                        AttributeData.Add(G);
                        AttributeData.Add(B);
                        AttributeData.Add(A);
                        AttributeData.AddRange(UV_X);
                        AttributeData.AddRange(UV_Y);
                        SkinData.AddRange(Bone1);
                        SkinData.AddRange(Bone2);
                        SkinData.AddRange(Bone3);
                        SkinData.Add(0);
                        SkinData.Add(0);
                        SkinData.AddRange(Weight1);
                        SkinData.AddRange(Weight2);
                        SkinData.AddRange(Weight3);
                        SkinData.Add(0);
                        SkinData.Add(0);
                    }
                }
                for (int a = 0; a < idx.Count; a++)
                {
                    byte[] id = BitConverter.GetBytes(idx[a]);
                    IndexData.AddRange(id);
                }

                MinX -= 0.1f;
                MaxX += 0.1f;
                MinY -= 0.1f;
                MaxY += 0.1f;
                MinZ -= 0.1f;
                MaxZ += 0.1f;
                for (int a = 0; a < Model.BlendShapeCount; a++)
                {
                    for (int s1 = 0; s1 < Model.Models[i].SubModels.Length; s1++)
                    {
                        var Sub = Model.Models[i].SubModels[s1];
                        for (int x = 0; x < Sub.Vertexes.Count; x++)
                        {
                            BlendSkin.BlendShapeVertex bs = Sub.BlendShapes[a].ShapeVertecies[x];
                            float BS_X = -Sub.Vertexes[x].X - bs.Offset.X;
                            float BS_Y = Sub.Vertexes[x].Y + bs.Offset.Y;
                            float BS_Z = Sub.Vertexes[x].Z + bs.Offset.Z;
                            byte[] BSX = BitConverter.GetBytes(BS_X);
                            byte[] BSY = BitConverter.GetBytes(BS_Y);
                            byte[] BSZ = BitConverter.GetBytes(BS_Z);
                            BlendShapeData.AddRange(BSX);
                            BlendShapeData.AddRange(BSY);
                            BlendShapeData.AddRange(BSZ);
                        }
                    }
                }

                boundingBox.Position = new Vector3(MinX, MinY, MinZ);
                boundingBox.Size = new Vector3(Math.Abs(MaxX - MinX), Math.Abs(MaxY - MinY), Math.Abs(MaxZ - MinZ));

                index_count = idx.Count;
                vertex_count = VertCount;

                dict.Add("aabb", boundingBox);
                dict.Add("attribute_data", AttributeData.ToArray());
                dict.Add("blend_shapes", BlendShapeData.ToArray());
                dict.Add("format", format);
                dict.Add("index_count", index_count);
                dict.Add("index_data", IndexData.ToArray());
                dict.Add("primitive", primitive);
                dict.Add("skin_data", SkinData.ToArray());
                dict.Add("vertex_count", vertex_count);
                dict.Add("vertex_data", VertexData.ToArray());
                surfArray.Add(dict);
            }

            res.Add("_blend_shape_names", BlendShapeNames.ToArray());
            res.Add("_surfaces", surfArray.ToArray());
            res.Add("blend_shape_mode", 0);
            Resources.Add(res);
        }

    }
}