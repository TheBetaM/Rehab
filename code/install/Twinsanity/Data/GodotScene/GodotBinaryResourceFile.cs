using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;

namespace RehabSetup
{
    public class GodotBinaryResourceFile
    {
        public virtual string ResType => "";
        public List<ExternalResource> ExtResources = new List<ExternalResource>();
        public List<Resource> Resources = new List<Resource>();
        public byte[] WriteBuffer;

        public void WriteToFile(string path)
        {
            if (AssetExporter.Check(path)) return;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Write(writer);
                    if (!AssetExporter.Check(path))
                    {
                        stream.Position = 0;
                        AssetExporter.Add(path, stream.ToArray());
                    }
                }
            }
        }
        public void WriteToFileForce(string path)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Write(writer);
                    stream.Position = 0;
                    //File.WriteAllBytes(path, stream.ToArray());
                    AssetExporter.Add(path, stream.ToArray());
                }
            }
        }

        public void WriteResourceToBuffer()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Write(writer);
                    stream.Position = 0;
                    WriteBuffer = stream.ToArray();
                }
            }
        }
        public void WriteBufferToFile(string path)
        {
            if (!AssetExporter.Check(path))
            {
                AssetExporter.Add(path, WriteBuffer);
            }
        }

        public void Write(BinaryWriter writer)
        {
            HashSet<string> StringTable = new HashSet<string>();
            Dictionary<string, int> HashDict = new Dictionary<string, int>();
            for (int i = 0; i < Resources.Count; i++)
            {
                for (int p = 0; p < Resources[i].Props.Count; p++)
                {
                    bool add = StringTable.Add(Resources[i].Props[p].Name);
                    if (add)
                    {
                        HashDict.Add(Resources[i].Props[p].Name, StringTable.Count - 1);
                    }
                    if (Resources[i].Props[p].Val is NodePath path)
                    {
                        foreach (var name in path.Names)
                        {
                            add = StringTable.Add(name);
                            if (add)
                            {
                                HashDict.Add(name, StringTable.Count - 1);
                            }
                        }
                        foreach (var name in path.SubNames)
                        {
                            add = StringTable.Add(name);
                            if (add)
                            {
                                HashDict.Add(name, StringTable.Count - 1);
                            }
                        }
                    }
                }
            }
            writer.Write("RSRC".ToCharArray());
            writer.Write((uint)0); // endianness
            writer.Write((uint)0);
            writer.Write((uint)4); // version major
            writer.Write((uint)1); // version minor
            writer.Write((uint)5); // format version
            WriteString(writer, ResType);
            writer.Write((long)0);
            writer.Write((uint)3); // format flags
            writer.Write(0xFFFFFFFFFFFFFFFF); // resource uid
            for (int i = 0; i < 11; i++)
            {
                writer.Write((uint)0); // reserved fields
            }
            writer.Write(StringTable.Count); // string table count
            foreach (var name in StringTable)
            {
                WriteString(writer, name);
            }
            writer.Write((uint)ExtResources.Count); // external resources count
            // save order here?
            for (int i = 0; i < ExtResources.Count; i++)
            {
                WriteString(writer, ExtResources[i].Type);
                WriteString(writer, ExtResources[i].Path);
                writer.Write(ExtResources[i].UID);
            }
            writer.Write((uint)Resources.Count); // internal resources count
            List<long> resOffsets1 = new List<long>();
            List<long> resOffsets2 = new List<long>();
            for (int i = 0; i < Resources.Count; i++)
            {
                WriteString(writer, Resources[i].UniqueID);
                resOffsets1.Add(writer.BaseStream.Position);
                resOffsets2.Add(0);
                writer.Write((ulong)0);
            }
            for (int i = 0; i < Resources.Count; i++)
            {
                resOffsets2[i] = writer.BaseStream.Position;
                WriteString(writer, Resources[i].Type);
                writer.Write(Resources[i].Props.Count);
                for (int p = 0; p < Resources[i].Props.Count; p++)
                {
                    writer.Write(HashDict[Resources[i].Props[p].Name]); // string table index
                    Resources[i].Props[p].Write(writer, Resources[i].Props[p].Val, HashDict);
                }
            }

            writer.Write("RSRC".ToCharArray());

            for (int i = 0; i < Resources.Count; i++)
            {
                writer.BaseStream.Position = resOffsets1[i];
                writer.Write(resOffsets2[i]);
            }
        }

        public void WriteString(BinaryWriter writer, string text)
        {
            writer.Write(text.Length + 1);
            writer.Write(text.ToCharArray());
            writer.Write((byte)0);
        }

        public class ExternalResource
        {
            public uint ID;
            public string Type;
            public string Path;
            public ulong UID = 0xFFFFFFFFFFFFFFFF;
        }

        public class Resource
        {
            public uint ID;
            public string UniqueID;
            public string Type;
            public List<Prop> Props = new List<Prop>();

            public Resource(){

            }
            public Resource(string t, string u)
            {
                Type = t;
                UniqueID = u;
            }

            public void Add(string n, object v)
            {
                Props.Add(new Prop(n, v));
            }

            public class Prop 
            {
                public string Name;
                public object Val;

                public Prop(){

                }
                public Prop(string n, object v)
                {
                    Name = n;
                    Val = v;
                }
                public void Write(BinaryWriter writer, object Value, Dictionary<string, int> HashDict)
                {
                    // write property value
                    if (Value is null)
                    {
                        writer.Write((uint)1);
                    }
                    else if (Value is bool blean)
                    {
                        writer.Write((uint)2);
                        writer.Write(blean ? 1 : 0);
                    }
                    else if (Value is int vint)
                    {
                        writer.Write((uint)3);
                        writer.Write(vint);
                    }
                    else if (Value is uint vuint)
                    {
                        writer.Write((uint)3);
                        writer.Write(vuint);
                    }
                    else if (Value is long vlong)
                    {
                        writer.Write((uint)40);
                        writer.Write(vlong);
                    }
                    else if (Value is ulong vulong)
                    {
                        writer.Write((uint)40);
                        writer.Write(vulong);
                    }
                    else if (Value is short vshort)
                    {
                        writer.Write((uint)3);
                        writer.Write((int)vshort);
                    }
                    else if (Value is ushort vushort)
                    {
                        writer.Write((uint)3);
                        writer.Write((uint)vushort);
                    }
                    else if (Value is float vfloat)
                    {
                        writer.Write((uint)4);
                        writer.Write(vfloat);
                    }
                    else if (Value is double vdouble)
                    {
                        writer.Write((uint)41);
                        writer.Write(vdouble);
                    }
                    else if (Value is string vstring)
                    {
                        writer.Write((uint)5);
                        writer.Write(vstring.Length + 1);
                        writer.Write(vstring.ToCharArray());
                        writer.Write((byte)0);
                    }
                    else if (Value is Vector2 vec2)
                    {
                        writer.Write((uint)10);
                        writer.Write(vec2.X);
                        writer.Write(vec2.Y);
                    }
                    else if (Value is Vector3 vec3)
                    {
                        writer.Write((uint)12);
                        writer.Write(vec3.X);
                        writer.Write(vec3.Y);
                        writer.Write(vec3.Z);
                    }
                    else if (Value is Vector4 vec4)
                    {
                        writer.Write((uint)50);
                        writer.Write(vec4.X);
                        writer.Write(vec4.Y);
                        writer.Write(vec4.Z);
                        writer.Write(vec4.W);
                    }
                    else if (Value is Quaternion quat)
                    {
                        writer.Write((uint)14);
                        writer.Write(quat.X);
                        writer.Write(quat.Y);
                        writer.Write(quat.Z);
                        writer.Write(quat.W);
                    }
                    else if (Value is Twinsanity.Color tcolor)
                    {
                        writer.Write((uint)20);
                        writer.Write(tcolor.R / 255f);
                        writer.Write(tcolor.G / 255f);
                        writer.Write(tcolor.B / 255f);
                        writer.Write(tcolor.A / 255f);
                    }
                    else if (Value is AABB aabb)
                    {
                        writer.Write((uint)15);
                        writer.Write(aabb.Position.X);
                        writer.Write(aabb.Position.Y);
                        writer.Write(aabb.Position.Z);
                        writer.Write(aabb.Size.X);
                        writer.Write(aabb.Size.Y);
                        writer.Write(aabb.Size.Z);
                    }
                    else if (Value is NodePath path)
                    {
                        writer.Write((uint)22);
                        writer.Write((ushort)path.Names.Count);
                        ushort subnames = (ushort)path.SubNames.Count;
                        if (path.IsAbsolute)
                        {
                            subnames |= 0x8000;
                        }
                        writer.Write(subnames);
                        // todo if missing from dict, write the actual string
                        foreach (var name in path.Names)
                        {
                            writer.Write(HashDict[name]);
                        }
                        foreach (var name in path.SubNames)
                        {
                            writer.Write(HashDict[name]);
                        }
                    }
                    else if (Value is Resource intRes)
                    {
                        writer.Write((uint)24);
                        writer.Write((uint)2);
                        writer.Write(intRes.ID);// internal resource id
                    }
                    else if (Value is ExternalResource extRes)
                    {
                        writer.Write((uint)24);
                        writer.Write((uint)3);
                        writer.Write(extRes.ID); // external resource id
                    }
                    else if (Value is Dictionary<object, object> dict)
                    {
                        writer.Write((uint)26);
                        writer.Write(dict.Count);
                        foreach (var pair in dict)
                        {
                            Write(writer, pair.Key, HashDict);
                            Write(writer, pair.Value, HashDict);
                        }
                    }
                    else if (Value is byte[] byteArray)
                    {
                        writer.Write((uint)31);
                        writer.Write(byteArray.Length);
                        writer.Write(byteArray);
                        Padding(writer, (uint)byteArray.Length);
                    }
                    else if (Value is Twinsanity.Color[] colorArray)
                    {
                        writer.Write((uint)36);
                        writer.Write(colorArray.Length);
                        foreach (var color in colorArray)
                        {
                            writer.Write(color.R / 255f);
                            writer.Write(color.G / 255f);
                            writer.Write(color.B / 255f);
                            writer.Write(color.A / 255f);
                        }
                    }
                    else if (Value is string[] strArray)
                    {
                        writer.Write((uint)34);
                        writer.Write(strArray.Length);
                        foreach (var str in strArray)
                        {
                            writer.Write(str.Length + 1);
                            writer.Write(str.ToCharArray());
                            writer.Write((byte)0);
                        }
                    }
                    else if (Value is float[] floatArray)
                    {
                        writer.Write((uint)33);
                        writer.Write(floatArray.Length);
                        foreach (var str in floatArray)
                        {
                            writer.Write(str);
                        }
                    }
                    else if (Value is Vector3[] vec3array)
                    {
                        writer.Write((uint)35);
                        writer.Write(vec3array.Length);
                        foreach (var vec in vec3array)
                        {
                            writer.Write(vec.X);
                            writer.Write(vec.Y);
                            writer.Write(vec.Z);
                        }
                    }
                    else if (Value is object[] varray)
                    {
                        writer.Write((uint)30);
                        writer.Write(varray.Length);
                        foreach (var obj in varray)
                        {
                            Write(writer, obj, HashDict);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                public void Padding(BinaryWriter writer, uint size)
                {
                    uint extra = 4 - (size % 4);
                    if (extra < 4)
                    {
                        for (uint i = 0; i < extra; i++)
                        {
                            writer.BaseStream.Position++;
                        }
                    }
                }
            }
        }

        public class AABB
        {
            public Vector3 Position = new Vector3();
            public Vector3 Size = new Vector3();
        }

        public class NodePath
        {
            public List<string> Names = new List<string>();
            public List<string> SubNames = new List<string>();
            public bool IsAbsolute = false;

            public NodePath(){

            }
            public NodePath(string subname)
            {
                SubNames.Add(subname);
            }

            public NodePath(string name, string subname)
            {
                Names.Add(name);
                SubNames.Add(subname);
            }
        }

    }
}
