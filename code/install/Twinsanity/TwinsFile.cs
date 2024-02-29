using System;
using System.Collections.Generic;
using System.IO;

namespace Twinsanity
{
    /// <summary>
    /// Represents a Twinsanity RM/SM file, a full pair corresponds to a complete level "chunk"
    /// </summary>
    public class TwinsFile : TwinsSection
    {
        public string FileName { get; set; }
        public string SafeFileName { get; set; }

        public new FileType Type { get; set; }
        public ConsoleType Console { get; set; }
        public Items.MusicHash musicHash { get; set;}
        public Items.MusicHashDemo musicHashDemo { get; set;}


        public void LoadFile(string fileName, FileType type)
        {
            FileName = fileName;

            byte[] buffer;
            using (var br = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 0x10000, FileOptions.SequentialScan))
            {
                buffer = new byte[br.Length];
                br.Read(buffer, 0, buffer.Length);
            }
            using (var memoryStream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    LoadFileStream(reader, type, fileName);
                }
            }
        }

        /// <summary>
        /// Load an RM/SM file.
        /// </summary>
        /// <param name="path">Path to the file to load from.</param>
        /// <param name="type">Filetype. RM2, SM2, etc.</param>
        public void LoadFileStream(BinaryReader reader, FileType type, string path)
        {
            //if (!File.Exists(path))
            //    return;
            Records = new List<TwinsItem>();
            RecordIDs = new Dictionary<uint, int>();
            //BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            Type = type;
            Console = ConsoleType.PS2;
            if (type == FileType.RMX || type == FileType.SMX) Console = ConsoleType.XBOX;
            FileName = path;
            if (type == FileType.PTL)
            {
                ParticleData rec = new ParticleData() { ID = 0, ParentFile = this };
                var sk = reader.BaseStream.Position;
                //reader.BaseStream.Position = rec.Offset = s_off;
                rec.Load(reader, (int)reader.BaseStream.Length);
                reader.BaseStream.Position = sk;
                RecordIDs.Add(0, Records.Count);
                Records.Add(rec);
                reader.Close();
                return;
            }
            else if (type == FileType.TRI)
            {
                ColData rec = new ColData() { ID = 0, ParentFile = this };
                var sk = reader.BaseStream.Position;
                //reader.BaseStream.Position = rec.Offset = s_off;
                rec.Load(reader, (int)reader.BaseStream.Length);
                reader.BaseStream.Position = sk;
                RecordIDs.Add(0, Records.Count);
                Records.Add(rec);
                reader.Close();
                return;
            }
            else if (type == FileType.PTC || type == FileType.PTC_XBOX || type == FileType.DemoPTC)
            {
                Items.TwinsPTC sec = new Items.TwinsPTC() { ID = 0, ParentFile = this };
                sec.Type = SectionType.PTC;
                sec.IsXbox = false;
                sec.IsDemo = false;
                if (type == FileType.PTC_XBOX)
                {
                    sec.IsXbox = true;
                }
                if (type == FileType.DemoPTC)
                {
                    sec.IsDemo = true;
                }
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.PSF || type == FileType.PSF_XBOX || type == FileType.DemoPSF)
            {
                Items.TwinsPSF sec = new Items.TwinsPSF() { ID = 0, ParentFile = this };
                sec.Type = SectionType.PSF;
                sec.IsXbox = false;
                sec.IsDemo = false;
                if (type == FileType.PSF_XBOX)
                {
                    sec.IsXbox = true;
                }
                if (type == FileType.DemoPSF)
                {
                    sec.IsDemo = true;
                }
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.PSM || type == FileType.PSM_XBOX || type == FileType.DemoPSM)
            {
                Items.TwinsPSM sec = new Items.TwinsPSM() { ID = 0, ParentFile = this };
                sec.Type = SectionType.PSM;
                sec.IsXbox = false;
                sec.IsDemo = false;
                if (type == FileType.PSM_XBOX)
                {
                    sec.IsXbox = true;
                }
                if (type == FileType.DemoPSM)
                {
                    sec.IsDemo = true;
                }
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.BIN || type == FileType.BIN_XBOX)
            {
                TwinsSection sec = new TwinsSection() { ID = 0, ParentFile = this };
                sec.Type = SectionType.SE;
                if (type == FileType.BIN_XBOX)
                {
                    sec.Type = SectionType.Xbox_SE;
                }
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.XWB)
            {
                Items.XWB sec = new Items.XWB() { ID = 0, ParentFile = this };
                sec.Type = SectionType.XWB;
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.BD)
            {
                return;
            }
            else if (type == FileType.MB)
            {
                Items.MusicBank sec = new Items.MusicBank() { ID = 0, ParentFile = this };
                sec.Hash = musicHash;
                sec.Type = SectionType.MB;
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.MH)
            {
                Items.MusicHash sec = new Items.MusicHash() { ID = 0, ParentFile = this };
                sec.Type = SectionType.MH;
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.MSB)
            {
                Items.MusicBankDemo sec = new Items.MusicBankDemo() { ID = 0, ParentFile = this };
                sec.Hash = musicHashDemo;
                sec.Type = SectionType.MSB;
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }
            else if (type == FileType.MSH)
            {
                Items.MusicHashDemo sec = new Items.MusicHashDemo() { ID = 0, ParentFile = this };
                sec.Type = SectionType.MSH;
                var sk = reader.BaseStream.Position;
                sec.Level = 1;
                sec.Load(reader, (int)reader.BaseStream.Length);
                RecordIDs.Add(0, Records.Count);
                Records.Add(sec);
                reader.Close();
                return;
            }

            if (type == FileType.RS2)
            {
                string magicString = new string(reader.ReadChars(0x12));
                if (magicString != "BinaryIntermediate")
                {
                    throw new Exception("LoadFile: Magic string is wrong.");
                }
            }
            else if ((Magic = reader.ReadUInt32()) != magic)
            {
                throw new Exception("LoadFile: Magic number is wrong.");
            }
            //var count = reader.ReadInt32();
            int count = 0;
            bool miniFix = false;
            if (type == FileType.MonkeyBallRM || type == FileType.MonkeyBallSM)
            {
                var sk = reader.BaseStream.Position;
                count = reader.ReadInt16();
                uint test = reader.ReadUInt16();
                if (test != 0) // PS2 starts off weird
                {
                    reader.BaseStream.Position = sk;
                    count = reader.ReadInt16();
                    reader.ReadByte();
                    miniFix = true;
                }
                else
                {
                    Console = ConsoleType.PSP;
                    reader.BaseStream.Position = sk;
                    count = reader.ReadInt32();
                }
            }
            else
            {
                count = reader.ReadInt32();
            }
            var sec_size = reader.ReadUInt32();
            if (miniFix)
            {
                reader.ReadByte();
            }
            uint s_off = 0, s_id = 0;
            int s_size = 0;
            for (int i = 0; i < count; i++)
            {
                s_off = reader.ReadUInt32();
                s_size = reader.ReadInt32();
                s_id = reader.ReadUInt32();
                switch (type)
                {
                    case FileType.DemoRM2:
                    case FileType.RMX:
                    case FileType.RM2:
                        {
                            switch (s_id)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 10:
                                case 11:
                                    {
                                        TwinsSection sec = new TwinsSection() { ID = s_id, ParentFile = this };
                                        if (s_id <= 7)
                                            if (type == FileType.DemoRM2)
                                                sec.Type = SectionType.InstanceDemo;
                                            else
                                                sec.Type = SectionType.Instance;
                                        else if (s_id == 10)
                                            if (type == FileType.DemoRM2)
                                                sec.Type = SectionType.CodeDemo;
                                            else if (type == FileType.RMX)
                                                sec.Type = SectionType.CodeX;
                                            else
                                                sec.Type = SectionType.Code;
                                        else if (s_id == 11)
                                            if (type == FileType.RMX)
                                                sec.Type = SectionType.GraphicsX;
                                            else if (type == FileType.DemoRM2)
                                                sec.Type = SectionType.GraphicsD;
                                            else
                                                sec.Type = SectionType.Graphics;
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = sec.Offset = s_off;
                                        sec.Level = 1;
                                        sec.Load(reader, s_size);
                                        sec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(sec);
                                        break;
                                    }
                                case 9:
                                    {
                                        ColData rec = new ColData() { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                                case 8:
                                    {
                                        ParticleData rec = new ParticleData() { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                                default:
                                    {
                                        TwinsItem rec = new TwinsItem { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                            }
                        }
                        break;
                    case FileType.DemoSM2:
                    case FileType.SM2:
                    case FileType.SMX:
                        {
                            switch (s_id)
                            {
                                case 6:
                                    {
                                        SectionType targetType = SectionType.Graphics;
                                        if (type == FileType.SMX)
                                            targetType = SectionType.GraphicsX;
                                        if (type == FileType.DemoSM2)
                                            targetType = SectionType.GraphicsD;
                                        TwinsSection sec = new TwinsSection
                                        {
                                            ID = s_id,
                                            Type = targetType,
                                            ParentFile = this,
                                            Level = 1
                                        };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = sec.Offset = s_off;
                                        sec.Load(reader, s_size);
                                        sec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(sec);
                                        break;
                                    }
                                case 5:
                                    {
                                        ChunkLinks rec = new ChunkLinks { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                                case 0:
                                    {
                                        SceneryData rec = new SceneryData { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                                case 4:
                                    {
                                        DynamicSceneryData rec = new DynamicSceneryData { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                                default:
                                    {
                                        TwinsItem rec = new TwinsItem { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                            }
                        }
                        break;
                    case FileType.BIN:
                        {
                            switch (s_id)
                            {
                                default:
                                    {
                                        SoundEffect rec = new SoundEffect { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                            }
                        }
                        break;
                    case FileType.DIR:
                        {
                            switch (s_id)
                            {
                                default:
                                    {
                                        TwinsItem rec = new TwinsItem { ID = s_id, ParentFile = this };
                                        var sk = reader.BaseStream.Position;
                                        reader.BaseStream.Position = rec.Offset = s_off;
                                        rec.Load(reader, s_size);
                                        rec.Parent = this;
                                        reader.BaseStream.Position = sk;
                                        RecordIDs.Add(s_id, Records.Count);
                                        Records.Add(rec);
                                        break;
                                    }
                            }
                        }
                        break;
                    case FileType.RS2:
                        {
                            TwinsSection sec = new TwinsSection() { ID = s_id, ParentFile = this };
                            sec.Type = SectionType.Null;
                            sec.IsProto = true;
                            var sk = reader.BaseStream.Position;
                            reader.BaseStream.Position = sec.Offset = s_off;
                            sec.Level = 1;
                            sec.Load(reader, s_size);
                            sec.Parent = this;
                            reader.BaseStream.Position = sk;
                            RecordIDs.Add(s_id, Records.Count);
                            Records.Add(sec);
                        }
                        break;
                }
            }
            //reader.Close();
        }

        //NOTE: Do NOT use "First"
        public enum FileType { First = SectionType.Last, RM2, SM2, DemoRM2, DemoSM2, RMX, SMX, PTL, BIN, DIR, TRI, LIGHTS, GHG, RS2, PSM, PSM_XBOX, PTC, PTC_XBOX, PSF, PSF_XBOX, DemoPSM, DemoPTC, DemoPSF, BIN_XBOX, OldPTL, HGO, MonkeyBallRM, MonkeyBallSM, NuGeom, RawTexture, XWB, BD, MB, BH, MH, MSB, MSH };

        public enum ConsoleType { First = SectionType.Last, PS2, PSP, XBOX, GCN, }
    }
}
