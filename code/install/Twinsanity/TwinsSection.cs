using System;
using System.Collections.Generic;
using System.IO;

namespace Twinsanity
{
    #region ENUMS & STRUCTS
    /// <summary>
    /// Enumerator that determines what type of section this TwinsSection is. Preferable to making new classes for each section since they basically all have the same format.
    /// 
    /// Please append more section types at the END of this list, BEFORE "Last".
    /// </summary>
    public enum SectionType {
        Null,
        Graphics, GraphicsX, GraphicsD, GraphicsMB,
        Code, CodeDemo, CodeX, CodeMB,
        Instance, InstanceDemo, InstanceMB,
        ParticleData,
        SceneryMB, Unknown,

        Texture, TextureX, TextureMB,
        Material, MaterialDemo,
        Model, ModelX, ModelMB,
        RigidModel,
        Skin, SkinX,
        BlendSkin, BlendSkinX,
        Mesh,
        LodModel,
        Skydome,

        Object, ObjectDemo, ObjectMB,
        Script, ScriptX, ScriptDemo, ScriptMB,
        Animation,
        OGI, GraphicsInfo, GraphicsInfoMB,
        CustomAgent, CustomAgentX, CustomAgentDemo,
        SE, Xbox_SE, MB_SE,
        SE_Eng, Xbox_SE_Eng,
        SE_Fre, Xbox_SE_Fre,
        SE_Ger, Xbox_SE_Ger,
        SE_Spa, Xbox_SE_Spa,
        SE_Ita, Xbox_SE_Ita,
        SE_Jpn, Xbox_SE_Jpn,

        InstanceTemplate, InstanceTemplateDemo, InstanceTemplateMB,
        AIPosition,
        AIPath,
        Position,
        Path,
        CollisionSurface,
        ObjectInstance, ObjectInstanceDemo, ObjectInstanceMB,
        Trigger,
        Camera, CameraDemo,

        PSM, PTC, PSF, XWB, BD, MB, BH, MH, MSB, MSH,

        Last
    }

    public struct TwinsSubInfo
    {
        public uint Off;
        public int Size;
        public uint ID;
    }
    #endregion

    public class TwinsSection : TwinsItem
    {
        protected readonly uint magic = 0x00010001;
        protected readonly uint magicV2 = 0x00010003;
        protected int size;

        public uint Magic { get; set; }
        public List<TwinsItem> Records = new List<TwinsItem>();
        public Dictionary<uint, int> RecordIDs = new Dictionary<uint, int>();
        public SectionType Type { get; set; }
        public int Level { get; set; }

        public byte[] ExtraData { get; set; }
        public bool IsProto { get; set; }
        public bool IsXbox { get; set; }
        public bool IsDemo { get; set; }
        private bool isMonkeyBallPS2 { get; set; }
        public static Dictionary<SectionType, List<uint>> DupeFiles = new Dictionary<SectionType, List<uint>>();

        public void Load(BinaryReader reader, int size, bool isMB)
        {
            isMonkeyBallPS2 = isMB;
            Load(reader, size);
        }

        /// <summary>
        /// Loads the section from a file.
        /// </summary>
        /// <param name="reader">BinaryReader already seeked to where the section begins.</param>
        /// <param name="size">Size of the section.</param>
        public override void Load(BinaryReader reader, int size)
        {
            this.size = size;
            Records = new List<TwinsItem>();
            RecordIDs = new Dictionary<uint, int>();
            if (size < 0xC)
                return;
            
            if (IsProto)
            {
                string magicString = new string(reader.ReadChars(0x12));
                if (magicString != "BinaryIntermediate")
                {
                    return;
                }
            }
            else
            {
                if (((Magic = reader.ReadUInt32()) != magic && Magic != magicV2))
                    return;
            }

            int count = 0;
            if (isMonkeyBallPS2)
            {
                count = reader.ReadInt16();
                reader.ReadByte();
            }
            else
            {
                count = reader.ReadInt32();
            }
            var sec_size = reader.ReadUInt32();
            if (isMonkeyBallPS2)
            {
                reader.ReadByte();
            }

            var start_sk = reader.BaseStream.Position - 12;
            long extra_begin = 12;
            List<TwinsSubInfo> SubItems = new List<TwinsSubInfo>();
            for (int i = 0; i < count; i++)
            {
                TwinsSubInfo sub = new TwinsSubInfo
                {
                    Off = reader.ReadUInt32(),
                    Size = reader.ReadInt32(),
                    ID = reader.ReadUInt32()
                };
                extra_begin = Math.Max(sub.Off + sub.Size, extra_begin);
                var sk = reader.BaseStream.Position;
                //reader.BaseStream.Position = sk - (i + 2) * 0xC + sub.Off;
                sub.Off = (uint)(sk - (i + 2) * 0xC + sub.Off);
                //var m = reader.ReadUInt32(); //get magic number [obsolete?]
                //reader.BaseStream.Position -= 4;
                //LoadSectionItem(reader, sub);
                //reader.BaseStream.Position = sk;
                SubItems.Add(sub);
            }
            switch (Type)
            {
                default: break;
                case SectionType.Graphics:
                case SectionType.GraphicsX:
                case SectionType.GraphicsD:
                case SectionType.Instance:
                case SectionType.InstanceDemo:
                case SectionType.Code:
                case SectionType.CodeX:
                case SectionType.CodeDemo:

                case SectionType.Material:
                case SectionType.MaterialDemo:
                case SectionType.Mesh:
                case SectionType.LodModel:
                case SectionType.OGI:
                case SectionType.Object:
                case SectionType.ObjectDemo:
                
                case SectionType.AIPosition:
                case SectionType.AIPath:
                case SectionType.Position:
                case SectionType.Path:
                case SectionType.ObjectInstance:
                case SectionType.ObjectInstanceDemo:
                case SectionType.Trigger:
                case SectionType.Camera:
                case SectionType.CameraDemo:
                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Position = SubItems[i].Off;
                    LoadSectionItem(reader, SubItems[i]);
                }
                break;
                // skipping items that only need to be loaded once
                case SectionType.Skydome:
                for (int i = 0; i < count; i++)
                {
                    if (!DupeFiles[Type].Contains(SubItems[i].ID) || ParentFile.Type == TwinsFile.FileType.DemoSM2)
                    {
                        if (!DupeFiles[Type].Contains(SubItems[i].ID))
                        {
                            DupeFiles[Type].Add(SubItems[i].ID);
                        }
                        reader.BaseStream.Position = SubItems[i].Off;
                        LoadSectionItem(reader, SubItems[i]);
                    }
                }
                break;
                case SectionType.SE:
		        case SectionType.SE_Eng:
                case SectionType.SE_Fre:
                case SectionType.SE_Ger:
                case SectionType.SE_Ita:
                case SectionType.SE_Spa:
                case SectionType.Xbox_SE:
		        case SectionType.Xbox_SE_Eng:
                case SectionType.Xbox_SE_Fre:
                case SectionType.Xbox_SE_Ger:
                case SectionType.Xbox_SE_Ita:
                case SectionType.Xbox_SE_Spa:
	            case SectionType.Animation:
                case SectionType.Skin:
                case SectionType.SkinX:
                case SectionType.BlendSkin:
                case SectionType.BlendSkinX:
                for (int i = 0; i < count; i++)
                {
                    if (!DupeFiles[Type].Contains(SubItems[i].ID))
                    {
                        DupeFiles[Type].Add(SubItems[i].ID);
                        reader.BaseStream.Position = SubItems[i].Off;
                        LoadSectionItem(reader, SubItems[i]);
                    }
                }
                break;
                case SectionType.Texture:
                case SectionType.TextureX:
                for (int i = 0; i < count; i++)
                {
                    if (!DefaultHashes.DupeTextureIDs.Contains(SubItems[i].ID))
                    {
                        if (!DupeFiles[Type].Contains(SubItems[i].ID))
                        {
                            DupeFiles[Type].Add(SubItems[i].ID);
                            reader.BaseStream.Position = SubItems[i].Off;
                            LoadSectionItem(reader, SubItems[i]);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Position = SubItems[i].Off;
                        LoadSectionItem(reader, SubItems[i]);
                    }
                }
                break;
                case SectionType.RigidModel:
                for (int i = 0; i < count; i++)
                {
                    if (DefaultHashes.Hash_RigidModels.ContainsKey(SubItems[i].ID))
                    {
                        if (!DupeFiles[Type].Contains(SubItems[i].ID))
                        {
                            DupeFiles[Type].Add(SubItems[i].ID);
                            reader.BaseStream.Position = SubItems[i].Off;
                            LoadSectionItem(reader, SubItems[i]);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Position = SubItems[i].Off;
                        LoadSectionItem(reader, SubItems[i]);
                    }
                }
                break;
                case SectionType.Model:
                case SectionType.ModelX:
                for (int i = 0; i < count; i++)
                {
                    if (DefaultHashes.Hash_Models.ContainsKey(SubItems[i].ID))
                    {
                        if (!DupeFiles[Type].Contains(SubItems[i].ID))
                        {
                            DupeFiles[Type].Add(SubItems[i].ID);
                            reader.BaseStream.Position = SubItems[i].Off;
                            LoadSectionItem(reader, SubItems[i]);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Position = SubItems[i].Off;
                        LoadSectionItem(reader, SubItems[i]);
                    }
                }
                break;
                
            }
            reader.BaseStream.Position = start_sk + extra_begin;
            ExtraData = reader.ReadBytes((int)(size - extra_begin));
        }

        protected void LoadSectionItem(BinaryReader reader, TwinsSubInfo sub)
        {
            switch (Type)
            {
                case SectionType.Graphics:
                case SectionType.GraphicsX:
                case SectionType.GraphicsD:
                case SectionType.GraphicsMB:
                    switch (sub.ID)
                    {
                        case 0:
                            if (Type == SectionType.GraphicsX)
                                LoadSection(reader, sub, SectionType.TextureX);
                            else if (Type == SectionType.GraphicsMB)
                                LoadSection(reader, sub, SectionType.TextureMB);
                            else
                                LoadSection(reader, sub, SectionType.Texture);
                            break;
                        case 1:
                            if (Type == SectionType.GraphicsD)
                                LoadSection(reader, sub, SectionType.MaterialDemo);
                            else
                                LoadSection(reader, sub, SectionType.Material);
                            break;
                        case 2:
                            if (Type == SectionType.GraphicsX)
                                LoadSection(reader, sub, SectionType.ModelX);
                            else if (Type == SectionType.GraphicsMB)
                                LoadSection(reader, sub, SectionType.ModelMB);
                            else
                                LoadSection(reader, sub, SectionType.Model);
                            break;
                        case 3:
                            LoadSection(reader, sub, SectionType.RigidModel);
                            break;
                        case 4:
                            if (Type == SectionType.GraphicsX)
                                LoadSection(reader, sub, SectionType.SkinX);
                            else
                                LoadSection(reader, sub, SectionType.Skin);
                            break;
                        case 5:
                            if (Type == SectionType.GraphicsX)
                                LoadSection(reader, sub, SectionType.BlendSkinX);
                            else
                                LoadSection(reader, sub, SectionType.BlendSkin);
                            break;
                        case 6:
                            LoadSection(reader, sub, SectionType.Mesh);
                            break;
                        case 7:
                            LoadSection(reader, sub, SectionType.LodModel);
                            break;
                        case 8:
                            LoadSection(reader, sub, SectionType.Skydome);
                            break;
                        default:
                            LoadItem<TwinsItem>(reader, sub, Type);
                            break;
                    }
                    break;
                case SectionType.Instance:
                case SectionType.InstanceDemo:
                    switch (sub.ID)
                    {
                        case 0:
                            //if (Type == SectionType.InstanceDemo)
                            //    LoadSection(reader, sub, SectionType.InstanceTemplateDemo);
                            //else
                            //    LoadSection(reader, sub, SectionType.InstanceTemplate);
                            break;
                        case 1:
                            LoadSection(reader, sub, SectionType.AIPosition);
                            break;
                        case 2:
                            LoadSection(reader, sub, SectionType.AIPath);
                            break;
                        case 3:
                            LoadSection(reader, sub, SectionType.Position);
                            break;
                        case 4:
                            LoadSection(reader, sub, SectionType.Path);
                            break;
                        case 5:
                            //LoadSection(reader, sub, SectionType.CollisionSurface);
                            break;
                        case 6:
                            if (Type == SectionType.InstanceDemo)
                                LoadSection(reader, sub, SectionType.ObjectInstanceDemo);
                            else
                                LoadSection(reader, sub, SectionType.ObjectInstance);
                            break;
                        case 7:
                            LoadSection(reader, sub, SectionType.Trigger);
                            break;
                        case 8:
                            if (Type == SectionType.InstanceDemo)
                                LoadSection(reader, sub, SectionType.CameraDemo);
                            else
                                LoadSection(reader, sub, SectionType.Camera);
                            break;
                        default:
                            LoadItem<TwinsItem>(reader, sub, Type);
                            break;
                    }
                    break;
                case SectionType.InstanceMB:
                    switch (sub.ID)
                    {
                        case 0:
                            LoadSection(reader, sub, SectionType.InstanceTemplateMB);
                            break;
                        case 1:
                            LoadSection(reader, sub, SectionType.AIPosition);
                            break;
                        case 2:
                            LoadSection(reader, sub, SectionType.AIPath);
                            break;
                        case 3:
                            LoadSection(reader, sub, SectionType.Position);
                            break;
                        case 4:
                            LoadSection(reader, sub, SectionType.Path);
                            break;
                        case 5:
                            LoadSection(reader, sub, SectionType.CollisionSurface);
                            break;
                        case 6:
                            LoadSection(reader, sub, SectionType.ObjectInstanceMB);
                            break;
                        case 7:
                            LoadSection(reader, sub, SectionType.Trigger);
                            break;
                        case 8:
                            LoadSection(reader, sub, SectionType.Camera);
                            break;
                        default:
                            LoadItem<TwinsItem>(reader, sub, Type);
                            break;
                    }
                    break;
                case SectionType.Code:
                case SectionType.CodeX:
                case SectionType.CodeDemo:
                    switch (sub.ID)
                    {
                        case 0:
                            if (Type == SectionType.CodeDemo)
                                LoadSection(reader, sub, SectionType.ObjectDemo);
                            else
                                LoadSection(reader, sub, SectionType.Object);
                            break;
                        case 1:
                            //if (Type == SectionType.CodeDemo)
                            //    LoadSection(reader, sub, SectionType.ScriptDemo);
                            //else if (Type == SectionType.CodeX)
                            //    LoadSection(reader, sub, SectionType.ScriptX);
                            //else
                            //    LoadSection(reader, sub, SectionType.Script);
                            break;
                        case 2:
                            LoadSection(reader, sub, SectionType.Animation);
                            break;
                        case 3:
                            LoadSection(reader, sub, SectionType.OGI);
                            break;
                        case 4:
                            //if (Type == SectionType.CodeDemo)
                            //    LoadSection(reader, sub, SectionType.CustomAgentDemo);
                            //else if (Type == SectionType.CodeX)
                            //    LoadSection(reader, sub, SectionType.CustomAgentX);
                            //else
                            //    LoadSection(reader, sub, SectionType.CustomAgent);
                            break;
                        case 6:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE);
                            else
                                LoadSection(reader, sub, SectionType.SE);
                            break;
                        case 7:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE_Eng);
                            else
                                LoadSection(reader, sub, SectionType.SE_Eng);
                            break;
                        case 8:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE_Fre);
                            else
                                LoadSection(reader, sub, SectionType.SE_Fre);
                            break;
                        case 9:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE_Ger);
                            else
                                LoadSection(reader, sub, SectionType.SE_Ger);
                            break;
                        case 10:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE_Spa);
                            else
                                LoadSection(reader, sub, SectionType.SE_Spa);
                            break;
                        case 11:
                            if (Type == SectionType.CodeX)
                                LoadSection(reader, sub, SectionType.Xbox_SE_Ita);
                            else
                                LoadSection(reader, sub, SectionType.SE_Ita);
                            break;
                        case 12:
                            //if (Type == SectionType.CodeX)
                            //    LoadSection(reader, sub, SectionType.Xbox_SE_Jpn);
                            //else
                            //    LoadSection(reader, sub, SectionType.SE_Jpn);
                            break;
                        default:
                            LoadItem<TwinsItem>(reader, sub, Type);
                            break;
                    }
                    break;
                case SectionType.CodeMB:
                    switch (sub.ID)
                    {
                        case 0:
                            LoadSection(reader, sub, SectionType.ObjectMB);
                            break;
                        case 1:
                            LoadSection(reader, sub, SectionType.ScriptMB);
                            break;
                        case 2:
                            LoadSection(reader, sub, SectionType.Animation);
                            break;
                        case 3:
                            LoadSection(reader, sub, SectionType.GraphicsInfoMB);
                            break;
                        case 4:
                            LoadSection(reader, sub, SectionType.CustomAgent);
                            break;
                        case 5:
                            LoadSection(reader, sub, SectionType.Unknown);
                            break;
                        //case 6:
                        //loads forever
                        //LoadSection(reader, sub, SectionType.MB_SE);
                        //break;
                        case 7:
                            LoadSection(reader, sub, SectionType.SE_Eng);
                            break;
                        case 8:
                            LoadSection(reader, sub, SectionType.SE_Fre);
                            break;
                        case 9:
                            LoadSection(reader, sub, SectionType.SE_Ger);
                            break;
                        case 10:
                            LoadSection(reader, sub, SectionType.SE_Spa);
                            break;
                        case 11:
                            LoadSection(reader, sub, SectionType.SE_Ita);
                            break;
                        case 12:
                            LoadSection(reader, sub, SectionType.SE_Jpn);
                            break;
                        default:
                            LoadItem<TwinsItem>(reader, sub, Type);
                            break;
                    }
                    break;
                case SectionType.Texture:
                    LoadItem<Texture>(reader, sub, Type);
                    break;
                case SectionType.TextureX: //XBOX textures
                    LoadItem<TextureX>(reader, sub, Type);
                    break;
                case SectionType.Material:
                    LoadItem<Material>(reader, sub, Type);
                    break;
                case SectionType.MaterialDemo: //PS2 DEMO Materials
                    LoadItem<Material>(reader, sub, Type);
                    break;
                case SectionType.Model:
                    LoadItem<Model>(reader, sub, Type);
                    break;
                case SectionType.ModelX: //XBOX meshes
                    LoadItem<ModelX>(reader, sub, Type);
                    break;
                case SectionType.RigidModel:
                case SectionType.Mesh:
                    LoadItem<RigidModel>(reader, sub, Type);
                    break;
                case SectionType.Skydome:
                    LoadItem<Skydome>(reader, sub, Type);
                    break;
                case SectionType.Object:
                    LoadItem<GameObject>(reader, sub, Type);
                    break;
                case SectionType.ObjectDemo: //PS2 DEMO objects
                    LoadItem<GameObject>(reader, sub, Type);
                    break;
                case SectionType.CustomAgent:
                case SectionType.CustomAgentX:
                case SectionType.CustomAgentDemo:
                    //LoadItem<CustomAgent>(reader, sub, Type);
                    break;
                case SectionType.Script:
                case SectionType.ScriptX:
                case SectionType.ScriptDemo:
                case SectionType.ScriptMB:
                    //LoadItem<Script>(reader, sub, Type);
                    break;
                case SectionType.SE:
                case SectionType.SE_Eng:
                case SectionType.SE_Fre:
                case SectionType.SE_Ger:
                case SectionType.SE_Ita:
                case SectionType.SE_Spa:
                case SectionType.SE_Jpn:
                    LoadItem<SoundEffect>(reader, sub, Type);
                    break;
                case SectionType.Xbox_SE:
                case SectionType.Xbox_SE_Eng:
                case SectionType.Xbox_SE_Fre:
                case SectionType.Xbox_SE_Ger:
                case SectionType.Xbox_SE_Ita:
                case SectionType.Xbox_SE_Jpn:
                case SectionType.Xbox_SE_Spa:
                    LoadItem<SoundEffectX>(reader, sub, Type);
                    break;
                case SectionType.AIPosition:
                    LoadItem<AIPosition>(reader, sub, Type);
                    break;
                case SectionType.AIPath:
                    LoadItem<AIPath>(reader, sub, Type);
                    break;
                case SectionType.Position:
                    LoadItem<Position>(reader, sub, Type);
                    break;
                case SectionType.Path:
                    LoadItem<Path>(reader, sub, Type);
                    break;
                case SectionType.ObjectInstance:
                    LoadItem<Instance>(reader, sub, Type);
                    break;
                case SectionType.ObjectInstanceDemo: //PS2 DEMO instances
                    LoadItem<Instance>(reader, sub, Type);
                    break;
                case SectionType.ObjectInstanceMB:
                    LoadItem<Instance>(reader, sub, Type);
                    break;
                case SectionType.Trigger:
                    LoadItem<Trigger>(reader, sub, Type);
                    break;
                case SectionType.Camera:
                    LoadItem<Camera>(reader, sub, Type);
                    break;
                case SectionType.CameraDemo:
                    LoadItem<Camera>(reader, sub, Type);
                    break;
                case SectionType.OGI:
                    LoadItem<GraphicsInfo>(reader, sub, Type);
                    break;
                case SectionType.Skin:
                    LoadItem<Skin>(reader, sub, Type);
                    break;
                case SectionType.SkinX: //XBOX Armature Models
                    LoadItem<SkinX>(reader, sub, Type);
                    break;
                case SectionType.LodModel:
                    LoadItem<LodModel>(reader, sub, Type);
                    break;
                case SectionType.ParticleData:
                    LoadItem<ParticleData>(reader, sub, Type);
                    break;
                case SectionType.CollisionSurface:
                    //LoadItem<CollisionSurface>(reader, sub, Type);
                    break;
                case SectionType.InstanceTemplate:
                    //LoadItem<InstanceTemplate>(reader, sub, Type);
                    break;
                case SectionType.InstanceTemplateDemo:
                    //LoadItem<InstanceTemplate>(reader, sub, Type);
                    break;
                case SectionType.Animation:
                    LoadItem<Animation>(reader, sub, Type);
                    break;
                case SectionType.BlendSkin:
                    LoadItem<BlendSkin>(reader, sub, Type);
                    break;
                case SectionType.BlendSkinX:
                    LoadItem<BlendSkinX>(reader, sub, Type);
                    break;
                default:
                    LoadItem<TwinsItem>(reader, sub, Type);
                    break;
            }
        }

        protected void LoadItem<T>(BinaryReader reader, TwinsSubInfo sub, SectionType type) where T : TwinsItem, new()
        {
            T rec = new T
            {
                ID = sub.ID,
                ParentFile = this.ParentFile,
                Parent = this
            };
            rec.ParentType = type;
            rec.Load(reader, sub.Size);
            RecordIDs.Add(sub.ID, Records.Count);
            Records.Add(rec);
        }

        private void LoadSection(BinaryReader reader, TwinsSubInfo sub, SectionType type)
        {
            TwinsSection sec = new TwinsSection {
                ID = sub.ID,
                Level = Level + 1,
                Offset = (uint)reader.BaseStream.Position,
                Type = type,
                ParentFile = this.ParentFile,
                Parent = this
            };
            sec.Load(reader, sub.Size);
            RecordIDs.Add(sub.ID, Records.Count);
            Records.Add(sec);
        }

        public override string ToString()
        {
            return $"Section: {Type}";// Magic {Magic:X8}";
        }

        public T GetItem<T>(uint id) where T : TwinsItem
        {
            return Records[RecordIDs[id]] as T;
        }

        public void AddItem(uint id, TwinsItem item)
        {
            RecordIDs.Add(id, Records.Count);
            Records.Add(item);
        }

        public bool TryAddItem(uint id, TwinsItem item)
        {
            if (RecordIDs.ContainsKey(id))
                return false;
            RecordIDs.Add(id, Records.Count);
            Records.Add(item);
            return true;
        }

        public void RemoveItem(uint id)
        {
            var index = RecordIDs[id];
            RecordIDs.Remove(id);
            Records.RemoveAt(index);
            var new_recs = new Dictionary<uint, int>(RecordIDs);
            RecordIDs.Clear();
            foreach (var i in new_recs)
            {
                if (i.Value >= index)
                    RecordIDs.Add(i.Key, i.Value - 1);
                else
                    RecordIDs.Add(i.Key, i.Value);
            }
        }

        public bool ContainsItem(uint id)
        {
            return RecordIDs.ContainsKey(id);
        }

        public static void ResetCache()
        {
            DupeFiles.Clear();
            DupeFiles.Add(SectionType.Texture, new List<uint>());
            DupeFiles.Add(SectionType.TextureX, new List<uint>());
            DupeFiles.Add(SectionType.RigidModel, new List<uint>());
            DupeFiles.Add(SectionType.ModelX, new List<uint>());
            DupeFiles.Add(SectionType.Model, new List<uint>());
            DupeFiles.Add(SectionType.Skydome, new List<uint>());
            DupeFiles.Add(SectionType.Object, new List<uint>());
            DupeFiles.Add(SectionType.ObjectDemo, new List<uint>());
            DupeFiles.Add(SectionType.SE, new List<uint>());
            DupeFiles.Add(SectionType.SE_Eng, new List<uint>());
            DupeFiles.Add(SectionType.SE_Fre, new List<uint>());
            DupeFiles.Add(SectionType.SE_Ger, new List<uint>());
            DupeFiles.Add(SectionType.SE_Ita, new List<uint>());
            DupeFiles.Add(SectionType.SE_Spa, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE_Eng, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE_Fre, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE_Ger, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE_Ita, new List<uint>());
            DupeFiles.Add(SectionType.Xbox_SE_Spa, new List<uint>());
            DupeFiles.Add(SectionType.OGI, new List<uint>());
            DupeFiles.Add(SectionType.Animation, new List<uint>());
            DupeFiles.Add(SectionType.Skin, new List<uint>());
            DupeFiles.Add(SectionType.SkinX, new List<uint>());
            DupeFiles.Add(SectionType.BlendSkin, new List<uint>());
            DupeFiles.Add(SectionType.BlendSkinX, new List<uint>());
        }
    }
}
