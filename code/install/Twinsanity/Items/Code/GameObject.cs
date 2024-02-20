using System;
using System.Collections.Generic;
using System.IO;

namespace Twinsanity
{
    public class GameObject : TwinsItem
    {
        private int size;
        public uint UnkBitfield { get; set; }
        public List<Byte> ScriptSlots { get; set; } = new List<Byte>(); // Pairs;Scripts;GameObjects;UInt32s;Sounds;00;00;00 (last 3 are potentially a side effect of needing object name's length to be word aligned)
        public List<UInt32> UI32 { get; set; } = new List<UInt32>();
        public List<UInt16> OGIs { get; set; } = new List<UInt16>();
        public List<UInt16> Anims { get; set; } = new List<UInt16>();
        public List<UInt16> Scripts { get; set; } = new List<UInt16>();
        public List<UInt16> Objects { get; set; } = new List<UInt16>();
        public List<UInt16> Sounds { get; set; } = new List<UInt16>();
        public uint PHeader { get; set; } // Inst;Pos;Path;00
        public uint PUI32 { get; set; }
        public List<UInt32> instFlagsList = new List<UInt32>();
        public List<Single> instFloatsList = new List<Single>();
        public List<UInt32> instIntegerList = new List<UInt32>();
        public uint flag;
        public List<UInt16> cObjects = new List<UInt16>();
        public List<UInt16> cOGIs = new List<UInt16>();
        public List<UInt16> cAnims = new List<UInt16>();
        public List<UInt16> cCM = new List<UInt16>();
        public List<UInt16> cScripts = new List<UInt16>();
        public List<UInt16> cUnk = new List<UInt16>();
        public List<UInt16> cSounds = new List<UInt16>();
        public int scriptCommandsAmount;
        public List<UInt16> scriptParams = new List<UInt16>();
        public int scriptGameVersion = 0;
        //public Script.MainScript.ScriptCommand scriptCommand = null;
        //public List<Script.MainScript.ScriptCommand> scriptCommands = new List<Script.MainScript.ScriptCommand>();
        public bool IsDemo = false;

        public string Name { get; set; }
        public GameObject()
        {
            while (ScriptSlots.Count < 8)
            {
                ScriptSlots.Add(0);
            }
        }
        private void UpdateSlots()
        {
            ScriptSlots[0] = (Byte)OGIs.Count;
            ScriptSlots[1] = (Byte)Scripts.Count;
            ScriptSlots[2] = (Byte)Objects.Count;
            ScriptSlots[3] = (Byte)UI32.Count;
            ScriptSlots[4] = (Byte)Sounds.Count;
            ScriptSlots[5] = 0;
            ScriptSlots[6] = 0;
            ScriptSlots[7] = 0;
        }
        public override void Load(BinaryReader reader, int size)
        {
            if (ParentType == SectionType.Object)
            {
                scriptGameVersion = 1;
            }
            else if (ParentType == SectionType.ObjectDemo)
            {
                scriptGameVersion = 2;
                IsDemo = true;
            }
            else
            {
                scriptGameVersion = 0;
            }

            UnkBitfield = reader.ReadUInt32();
            byte Type = (byte)(UnkBitfield >> 0x14 & 0xFF);
            byte UnkTypeValue = (byte)(UnkBitfield >> 0xC & 0xFF);
            byte JointIDCount = (byte)(UnkBitfield >> 0x6 & 0x3F);
            byte ExitPointCount = (byte)(UnkBitfield & 0x3F);

            if (!IsDemo)
            {
                for (int i = 0; i < 8; ++i)
                {
                    ScriptSlots[i] = reader.ReadByte();
                }

                var len = reader.ReadInt32();
                Name = new string(reader.ReadChars(len));

                // Read UInt32 script slots
                var cnt = reader.ReadInt32();
                UI32.Clear();
                for (int i = 0; i < cnt; ++i)
                    UI32.Add(reader.ReadUInt32());

                // Read OGI script slots
                cnt = reader.ReadInt32();
                OGIs.Clear();
                for (int i = 0; i < cnt; ++i)
                    OGIs.Add(reader.ReadUInt16());

                // Read Animation script slots
                cnt = reader.ReadInt32();
                Anims.Clear();
                for (int i = 0; i < cnt; ++i)
                    Anims.Add(reader.ReadUInt16());

                // Read Script script slots
                cnt = reader.ReadInt32();
                Scripts.Clear();
                for (int i = 0; i < cnt; ++i)
                    Scripts.Add(reader.ReadUInt16());

                // Read Object script slots
                cnt = reader.ReadInt32();
                Objects.Clear();
                for (int i = 0; i < cnt; ++i)
                    Objects.Add(reader.ReadUInt16());

                // Read Sound script slots
                cnt = reader.ReadInt32();
                Sounds.Clear();
                for (int i = 0; i < cnt; ++i)
                    Sounds.Add(reader.ReadUInt16());

                // Read instance properties
                if ((UnkBitfield & 0x20000000) != 0x0)
                {
                    PHeader = reader.ReadUInt32();
                    PUI32 = reader.ReadUInt32();

                    cnt = reader.ReadInt32();
                    instFlagsList.Clear();
                    for (int i = 0; i < cnt; ++i)
                        instFlagsList.Add(reader.ReadUInt32());

                    cnt = reader.ReadInt32();
                    instFloatsList.Clear();
                    for (int i = 0; i < cnt; ++i)
                        instFloatsList.Add(reader.ReadSingle());

                    cnt = reader.ReadInt32();
                    instIntegerList.Clear();
                    for (int i = 0; i < cnt; ++i)
                        instIntegerList.Add(reader.ReadUInt32());
                }
                else
                {
                    PHeader = 0;
                    PUI32 = 0;
                    instFlagsList.Clear();
                    instFloatsList.Clear();
                    instIntegerList.Clear();
                }

                
            }
            else
            {
                var Count_OGI = reader.ReadByte();
                var Count_Anim = Count_OGI;
                var Count_Script = reader.ReadByte();
                var Count_GameObject = reader.ReadByte();
                var Count_UnkI32 = reader.ReadByte();
                //reader.ReadUInt32();
                var Count_Sound = reader.ReadUInt32();

                var len = reader.ReadInt32();
                Name = new string(reader.ReadChars(len));

                // Read UInt32 script slots
                UI32.Clear();
                for (int i = 0; i < Count_UnkI32; ++i)
                    UI32.Add(reader.ReadUInt32());

                // Read OGI script slots
                OGIs.Clear();
                for (int i = 0; i < Count_OGI; ++i)
                    OGIs.Add(reader.ReadUInt16());

                // Read Animation script slots
                Anims.Clear();
                for (int i = 0; i < Count_Anim; ++i)
                    Anims.Add(reader.ReadUInt16());

                // Read Script script slots
                Scripts.Clear();
                for (int i = 0; i < Count_Script; ++i)
                    Scripts.Add(reader.ReadUInt16());

                // Read Object script slots
                Objects.Clear();
                for (int i = 0; i < Count_GameObject; ++i)
                    Objects.Add(reader.ReadUInt16());

                // Read Sound script slots
                Sounds.Clear();
                for (int i = 0; i < Count_Sound; ++i)
                    Sounds.Add(reader.ReadUInt16());

                // Read instance properties
                if ((UnkBitfield & 0x20000000) != 0x0)
                {
                    //PHeader = reader.ReadUInt32();
                    //reader.BaseStream.Position -= 4;
                    var Count_Flags = reader.ReadByte();
                    var Count_Floats = reader.ReadByte();
                    var Count_Ints = reader.ReadByte();
                    reader.ReadByte();
                    PUI32 = reader.ReadUInt32();

                    instFlagsList.Clear();
                    for (int i = 0; i < Count_Flags; ++i)
                        instFlagsList.Add(reader.ReadUInt32());

                    instFloatsList.Clear();
                    for (int i = 0; i < Count_Floats; ++i)
                        instFloatsList.Add(reader.ReadSingle());

                    instIntegerList.Clear();
                    for (int i = 0; i < Count_Ints; ++i)
                        instIntegerList.Add(reader.ReadUInt32());
                }
                else
                {
                    PHeader = 0;
                    PUI32 = 0;
                    instFlagsList.Clear();
                    instFloatsList.Clear();
                    instIntegerList.Clear();
                }
            }

            // Read IDs needed for instance creation
            if ((UnkBitfield & 0x40000000) != 0x0)
            {
                flag = reader.ReadUInt32();
                if ((flag & 0x00000001) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cObjects.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cObjects.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000002) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cOGIs.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cOGIs.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000004) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cAnims.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cAnims.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000008) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cCM.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cCM.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000010) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cScripts.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cScripts.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000020) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cUnk.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cUnk.Add(reader.ReadUInt16());
                }
                if ((flag & 0x00000040) != 0)
                {
                    int cnt = reader.ReadInt32();
                    cSounds.Clear();
                    for (int i = 0; i < cnt; ++i)
                        cSounds.Add(reader.ReadUInt16());
                }
            }

            scriptCommandsAmount = (int)reader.ReadUInt32();
            /*
            if (scriptCommandsAmount != 0)
            {
                scriptCommand = new Script.MainScript.ScriptCommand(reader, scriptGameVersion);
                var command = scriptCommand;
                do
                {
                    scriptCommands.Add(command);
                    command = command.nextCommand;
                } while (command != null);
            }
            else
            {
                scriptCommand = null;
            }
            */
            this.size = size;
        }

        private void updateFlag()
        {
            flag = 0;
            if (cObjects.Count > 0) flag |= 0x01;
            if (cOGIs.Count > 0) flag |= 0x02;
            if (cAnims.Count > 0) flag |= 0x04;
            if (cCM.Count > 0) flag |= 0x08;
            if (cScripts.Count > 0) flag |= 0x10;
            if (cUnk.Count > 0) flag |= 0x20;
            if (cSounds.Count > 0) flag |= 0x40;
        }

        public override string ToString()
        {
            return $"{DefaultHashes.ToName(ParentType, ID)}";
        }
    }
}
