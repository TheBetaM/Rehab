using System.Collections.Generic;
using System.IO;
using System;

namespace Twinsanity
{
    public class SceneryData : TwinsItem
    {

        public uint HeaderUnk1;
        public string ChunkName;
        public uint FogPreset;
        public uint HeaderUnk3;
        public byte HeaderUnk4;
        public uint SkydomeID;
        public SceneryStruct SceneryRoot;
        public List<LightAmbient> LightsAmbient;
        public List<LightDirectional> LightsDirectional;
        public List<LightPoint> LightsPoint;
        public List<LightNegative> LightsNegative;

        public uint unkVar5;

        public SceneryData()
        {

        }

        public override void Load(BinaryReader reader, int size)
        {
            //long start_pos = reader.BaseStream.Position;

            HeaderUnk1 = reader.ReadUInt32();
            uint chunkNameLength = reader.ReadUInt32();
            ChunkName = new string(reader.ReadChars((int)chunkNameLength));
            FogPreset = reader.ReadUInt32();
            HeaderUnk3 = reader.ReadUInt32();
            HeaderUnk4 = reader.ReadByte();
            if ((HeaderUnk1 & 0x10000) != 0)
            {
                SkydomeID = reader.ReadUInt32();
            }

            LightsAmbient = new List<LightAmbient>();
            LightsDirectional = new List<LightDirectional>();
            LightsPoint = new List<LightPoint>();
            LightsNegative = new List<LightNegative>();

            if ((HeaderUnk1 & 0x20000) != 0)
            {
                reader.ReadBytes(0x400);

                uint LightsNum = reader.ReadUInt32();

                uint LightAmbientNum = reader.ReadUInt32();
                uint LightDirectionalNum = reader.ReadUInt32();
                uint LightPointNum = reader.ReadUInt32();
                uint LightNegativeNum = reader.ReadUInt32();

                if (LightAmbientNum > 0)
                {
                    for (int i = 0; i < LightAmbientNum; i++)
                    {
                        LightAmbient light = new LightAmbient();

                        light.Flags = reader.ReadBytes(4);
                        light.Radius = reader.ReadSingle();
                        light.Color_R = reader.ReadSingle();
                        light.Color_G = reader.ReadSingle();
                        light.Color_B = reader.ReadSingle();
                        light.Color_Unk = reader.ReadSingle();
                        light.Position = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        LightsAmbient.Add(light);
                    }
                }
                if (LightDirectionalNum > 0)
                {
                    for (int i = 0; i < LightDirectionalNum; i++)
                    {
                        LightDirectional light = new LightDirectional();

                        light.Flags = reader.ReadBytes(4);
                        light.Radius = reader.ReadSingle();
                        light.Color_R = reader.ReadSingle();
                        light.Color_G = reader.ReadSingle();
                        light.Color_B = reader.ReadSingle();
                        light.Color_Unk = reader.ReadSingle();
                        light.Position = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        light.Vector3 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.unkShort = reader.ReadUInt16();

                        LightsDirectional.Add(light);
                    }
                }
                if (LightPointNum > 0)
                {
                    for (int i = 0; i < LightPointNum; i++)
                    {
                        LightPoint light = new LightPoint();

                        light.Flags = reader.ReadBytes(4);
                        light.Radius = reader.ReadSingle();
                        light.Color_R = reader.ReadSingle();
                        light.Color_G = reader.ReadSingle();
                        light.Color_B = reader.ReadSingle();
                        light.Color_Unk = reader.ReadSingle();
                        light.Position = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        light.unkShort = reader.ReadUInt16();

                        LightsPoint.Add(light);
                    }
                }
                if (LightNegativeNum > 0)
                {
                    for (int i = 0; i < LightNegativeNum; i++)
                    {
                        LightNegative light = new LightNegative();

                        light.Flags = reader.ReadBytes(4);
                        light.Radius = reader.ReadSingle();
                        light.Color_R = reader.ReadSingle();
                        light.Color_G = reader.ReadSingle();
                        light.Color_B = reader.ReadSingle();
                        light.Color_Unk = reader.ReadSingle();
                        light.Position = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.Vector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        light.Vector3 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        light.unkFloat1 = reader.ReadSingle();
                        light.unkFloat2 = reader.ReadSingle();
                        light.unkUInt1 = reader.ReadUInt32();
                        light.unkUInt2 = reader.ReadUInt32();
                        light.unkUShort1 = reader.ReadUInt16();
                        light.unkUShort2 = reader.ReadUInt16();

                        LightsNegative.Add(light);
                    }
                }
            }

            SceneryRoot = null;
            if (HeaderUnk3 == 0x160A)
            {
                unkVar5 = reader.ReadUInt32();
                SceneryRoot = LoadScenery(reader);
            }
            else
            {
                //Console.WriteLine("no scenery!! bug?");
            }

            //reader.BaseStream.Position = start_pos;
            //Data = reader.ReadBytes(size);

            //long cur_pos = reader.BaseStream.Position;
            //Remain = reader.ReadBytes((int)((start_pos + size) - cur_pos));
            //reader.BaseStream.Position = cur_pos;

            //Console.WriteLine("end pos: " + (reader.BaseStream.Position - start_pos) + " target: " + size);
        }

        private SceneryModelStruct LoadSceneryModel(BinaryReader reader)
        {
            SceneryModelStruct scenery = new SceneryModelStruct();
            scenery.Header = reader.ReadUInt32();
            scenery.Models = new List<ScenerySubModel>();
            if (scenery.Header == 0x1613)
            {
                ushort modelCount = reader.ReadUInt16();
                ushort specialModelCount = reader.ReadUInt16();

                if (modelCount + specialModelCount != 0)
                {
                    for (int i = 0; i < modelCount + specialModelCount; i++)
                    {
                        ScenerySubModel newModel = new ScenerySubModel();
                        newModel.ModelMatrix = new Pos[4];
                        newModel.ModelBoundingBoxVector1 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        newModel.ModelBoundingBoxVector2 = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        scenery.Models.Add(newModel);
                    }
                    for (int i = 0; i < modelCount + specialModelCount; i++)
                    {
                        if (i > modelCount - 1)
                        {
                            scenery.Models[i].ModelID = reader.ReadUInt32();
                            scenery.Models[i].isSpecial = true;
                        }
                        else
                        {
                            scenery.Models[i].ModelID = reader.ReadUInt32();
                            scenery.Models[i].isSpecial = false;
                        }
                    }
                    for (int i = 0; i < modelCount + specialModelCount; i++)
                    {
                        scenery.Models[i].ModelMatrix[0] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        scenery.Models[i].ModelMatrix[1] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        scenery.Models[i].ModelMatrix[2] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        scenery.Models[i].ModelMatrix[3] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                }
            }
            scenery.UnkPos = new Pos[4];
            scenery.UnkPos[0] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            scenery.UnkPos[1] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            scenery.UnkPos[2] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            scenery.UnkPos[3] = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            scenery.LightBitFields = new uint[4];
            scenery.LightBitFields[0] = reader.ReadUInt32();
            scenery.LightBitFields[1] = reader.ReadUInt32();
            scenery.LightBitFields[2] = reader.ReadUInt32();
            scenery.LightBitFields[3] = reader.ReadUInt32();

            return scenery;
        }

        public SceneryStruct LoadScenery(BinaryReader reader)
        {
            SceneryStruct scen = new SceneryStruct();
            int[] sceneryTypes = new int[8];

            scen.Model = LoadSceneryModel(reader);

            scen.Links = new object[8];

            for (int i = 0; i < 8; i++)
            {
                sceneryTypes[i] = reader.ReadInt32();
            }
            
            for (int i = 0; i < 8; i++)
            {
                if (sceneryTypes[i] == 0x1600)
                {
                    scen.Links[i] = LoadScenery(reader);
                }
                else if (sceneryTypes[i] == 0x1605)
                {
                    scen.Links[i] = LoadSceneryModel(reader);
                }
                else
                {
                    // if type 3 - it's nothing
                    scen.Links[i] = null;
                }
            }

            //Console.WriteLine($"Adding scenery {sceneryModels.Count}");
            //sceneryModels.Add(scen);
            return scen;
        }

        public class SceneryModelStruct
        {
            public uint Header;
            public List<ScenerySubModel> Models;
            public Pos[] UnkPos; //4
            public uint[] LightBitFields; //4
        }

        public class SceneryStruct
        {
            public SceneryModelStruct Model;
            public object[] Links; //8
        }

        public class ScenerySubModel
        {
            public bool isSpecial;
            public uint ModelID;
            public Pos ModelBoundingBoxVector1;
            public Pos ModelBoundingBoxVector2;
            public Pos[] ModelMatrix; // 4
        }

        public class LightBase
        {
            public byte[] Flags; //4: [0] light type (0-3), [1] always 1, [2] always 0, [3] always 0
            public float Radius;
            public float Color_R;
            public float Color_G;
            public float Color_B;
            public float Color_Unk; // always 0
            public Pos Position;
            public Pos Vector1; // Bounding box 1?
            public Pos Vector2; // Bounding box 2?
        }

        public class LightAmbient : LightBase
        {

        }

        public class LightDirectional : LightBase
        {
            public Pos Vector3; // direction
            public ushort unkShort;
        }

        public class LightPoint : LightBase
        {
            public ushort unkShort; // 0, 1 or 2
        }

        public class LightNegative : LightBase
        {
            public Pos Vector3; // direction
            public float unkFloat1; // between +0 and +1
            public float unkFloat2; // between +0 and +1, the same or less than unkFloat1
            public uint unkUInt1; // This Light ID
            public uint unkUInt2;
            public ushort unkUShort1; // 0 or 917
            public ushort unkUShort2; // always 0
        }

    }

}
