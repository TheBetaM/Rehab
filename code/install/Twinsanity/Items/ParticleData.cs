using System.Collections.Generic;
using System;
using System.IO;

namespace Twinsanity
{
    public sealed class ParticleData : TwinsItem
    {
        public bool IsStandalone = false;
        public bool IsDefault = false;

        public long DataSize;

        public uint Version;
        public List<ParticleSystemDefinition> ParticleTypes;
        public List<ParticleSystemInstance> ParticleInstances;

        public uint ParticleTextureID_1;
        public uint ParticleMaterialID_1;
        public uint ParticleTextureID_2;
        public uint ParticleMaterialID_2;
        public uint ParticleTextureID_3;
        public uint ParticleMaterialID_3;
        public byte[] Remain;
        public uint DecalTextureID;
        public uint DecalMaterialID;

        public ParticleData()
        {

        }

        protected override int GetSize()
        {
            if (Version == 0x616E6942)
            {
                return (int)DataSize;
            }
            int count = 12;
            if (IsDefault)
            {
                count += 28;
            }
            count += Remain.Length;
            count += (int)ParticleInstances.Count * 0x44;
            if (ParticleTypes.Count > 0)
            {
                for (int i = 0; i < ParticleTypes.Count; i++)
                {
                    count += 0x4E;
                    count += ParticleTypes[i].Remain.Length;
                }
            }
            return count;
        }

        public override void Save(BinaryWriter writer)
        {
            if (Version == 0x616E6942)
            {
                writer.Write(Data);
                return;
            }

            if (IsDefault)
            {
                writer.Write(ParticleTextureID_1);
                writer.Write(ParticleMaterialID_1);
                writer.Write(ParticleTextureID_2);
                writer.Write(ParticleMaterialID_2);
                writer.Write(ParticleTextureID_3);
                writer.Write(ParticleMaterialID_3);
            }

            writer.Write(Version);
            writer.Write(ParticleTypes.Count);

            if (ParticleTypes.Count > 0)
            {
                for (int i = 0; i < ParticleTypes.Count; i++)
                {
                    ParticleSystemDefinition ParticleSystem = ParticleTypes[i];
                    string tempName = ParticleTypes[i].Name.Replace(' ', '\0');
                    char[] tempName2 = tempName.ToCharArray();
                    writer.Write(tempName2);

                    writer.Write(ParticleSystem.Header1);
                    writer.Write(ParticleSystem.MaxParticleCount);
                    writer.Write(ParticleSystem.Emitter_OverTime);
                    writer.Write(ParticleSystem.Emitter_OverTimeRandom);
                    writer.Write(ParticleSystem.Emitter_OffTime);
                    writer.Write(ParticleSystem.Emitter_OffTimeRandom);
                    writer.Write((ushort)ParticleSystem.GSort);
                    writer.Write((ushort)ParticleSystem.TextureFilter);
                    writer.Write(ParticleSystem.UnkFloat);
                    writer.Write(ParticleSystem.CutOnRadius);
                    writer.Write(ParticleSystem.CutOffRadius);
                    writer.Write(ParticleSystem.DrawCutOff);
                    writer.Write(ParticleSystem.Velocity);
                    writer.Write(ParticleSystem.Random_Emit_X);
                    writer.Write(ParticleSystem.Random_Emit_Y);
                    writer.Write(ParticleSystem.Random_Emit_Z);
                    writer.Write(ParticleSystem.Random_Start_X);
                    writer.Write(ParticleSystem.Random_Start_Y);
                    writer.Write(ParticleSystem.Random_Start_Z);

                    writer.Write(ParticleSystem.Remain);
                }
            }

            if (!IsDefault)
            {
                writer.Write(ParticleInstances.Count);
            }
            if (ParticleInstances.Count > 0 && ParticleInstances.Count < 65536)
            {
                for (int i = 0; i < ParticleInstances.Count; i++)
                {
                    writer.Write(ParticleInstances[i].Position.X);
                    writer.Write(ParticleInstances[i].Position.Y);
                    writer.Write(ParticleInstances[i].Position.Z);
                    writer.Write(ParticleInstances[i].Position.W);
                    writer.Write(ParticleInstances[i].Rot_X);
                    writer.Write(ParticleInstances[i].Rot_Y);
                    writer.Write(ParticleInstances[i].Rot_Z);
                    writer.Write(ParticleInstances[i].UnkZero);
                    string tempName = ParticleInstances[i].Name.Replace(' ', '\0');
                    char[] tempName2 = tempName.ToCharArray();
                    writer.Write(tempName2);
                    for (int a = 0; a < ParticleInstances[i].UnkShorts.Length; a++)
                    {
                        writer.Write(ParticleInstances[i].UnkShorts[a]);
                    }
                    writer.Write(ParticleInstances[i].UnkFloat);
                    writer.Write(ParticleInstances[i].EndZero);
                }
            }

            if (IsDefault)
            {
                writer.Write(DecalTextureID);
                writer.Write(DecalMaterialID);
            }

            if (Remain.Length > 0)
            {
                writer.Write(Remain);
            }
        }

        private bool isMonkeyBall = false;
        public void Load(BinaryReader reader, int size, bool isMB)
        {
            isMonkeyBall = isMB;
            Load(reader, size);
        }

        public override void Load(BinaryReader reader, int size)
        {
            long start_pos = reader.BaseStream.Position;
            DataSize = size;

            ParticleTypes = new List<ParticleSystemDefinition>();
            ParticleInstances = new List<ParticleSystemInstance>();

            Version = reader.ReadUInt32();

            // Some PTL files are "BinaryIntermediate" files
            if (Version == 0x616E6942)
            {
                reader.BaseStream.Position = start_pos;
                Data = reader.ReadBytes(size);
                return;
            }

            //Default.rm2 has some pre-header data: 3x (texture ID + material ID)
            if (Version > 0xFF)
            {
                IsDefault = true;
                ParticleTextureID_1 = Version;
                ParticleMaterialID_1 = reader.ReadUInt32();
                ParticleTextureID_2 = reader.ReadUInt32();
                ParticleMaterialID_2 = reader.ReadUInt32();
                ParticleTextureID_3 = reader.ReadUInt32();
                ParticleMaterialID_3 = reader.ReadUInt32();
                Version = reader.ReadUInt32();

                if (isMonkeyBall)
                {
                    // todo
                    int RemainBytes1 = (int)((start_pos + size) - reader.BaseStream.Position);
                    if (RemainBytes1 > 0)
                    {
                        Remain = reader.ReadBytes(RemainBytes1);
                    }
                    else
                    {
                        Remain = new byte[0];
                    }
                    return;
                }
            }

            uint ParticleTypeCount = reader.ReadUInt32();

            // size 0x33C (0x330 if header is 0x1c)
            if (ParticleTypeCount > 0)
            {
                for (int i = 0; i < ParticleTypeCount; i++)
                {
                    ParticleSystemDefinition ParticleSystem = new ParticleSystemDefinition();
                    string tempName = new string(reader.ReadChars(0x10));
                    ParticleSystem.Name = tempName.Replace('\0', ' ');
                    int bufferSize = 0x320;
                    if (Version == 0x1E)
                    {
                        bufferSize += 0xC;
                    }
                    if (Version == 0x15)
                    {
                        bufferSize += 0x10;
                    }

                    bufferSize -= 0x3E; // the known values so far below

                    ParticleSystem.Header1 = reader.ReadInt16();
                    ParticleSystem.MaxParticleCount = reader.ReadUInt32();
                    ParticleSystem.Emitter_OverTime = reader.ReadUInt16();
                    ParticleSystem.Emitter_OverTimeRandom = reader.ReadUInt16();
                    ParticleSystem.Emitter_OffTime = reader.ReadUInt16();
                    ParticleSystem.Emitter_OffTimeRandom = reader.ReadUInt16();
                    ParticleSystem.GSort = (ParticleSystemDefinition.GenSort)reader.ReadUInt16();
                    ParticleSystem.TextureFilter = (ParticleSystemDefinition.TextureFiltering)reader.ReadUInt16();
                    ParticleSystem.UnkFloat = reader.ReadSingle();
                    ParticleSystem.CutOnRadius = reader.ReadSingle();
                    ParticleSystem.CutOffRadius = reader.ReadSingle();
                    ParticleSystem.DrawCutOff = reader.ReadSingle();
                    ParticleSystem.Velocity = reader.ReadSingle();
                    ParticleSystem.Random_Emit_X = reader.ReadSingle();
                    ParticleSystem.Random_Emit_Y = reader.ReadSingle();
                    ParticleSystem.Random_Emit_Z = reader.ReadSingle();
                    ParticleSystem.Random_Start_X = reader.ReadSingle();
                    ParticleSystem.Random_Start_Y = reader.ReadSingle();
                    ParticleSystem.Random_Start_Z = reader.ReadSingle();

                    ParticleSystem.Remain = reader.ReadBytes(bufferSize);
                    ParticleTypes.Add(ParticleSystem);
                }
            }

            if (reader.BaseStream.Position == start_pos + DataSize)
            {
                Remain = new byte[0];
                return;
            }

            uint InstanceCheck = reader.ReadUInt32();
            uint ParticleInstanceCount = InstanceCheck;
            if (!IsDefault && ParticleInstanceCount > 0 && ParticleInstanceCount < 65536)
            {
                ParticleInstances = new List<ParticleSystemInstance>();

                // size 0x44
                for (int i = 0; i < ParticleInstanceCount; i++)
                {
                    ParticleSystemInstance ParticleInstance = new ParticleSystemInstance();
                    ParticleInstance.Position = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    ParticleInstance.Rot_X = reader.ReadUInt16();
                    ParticleInstance.Rot_Y = reader.ReadUInt16();
                    ParticleInstance.Rot_Z = reader.ReadUInt16();
                    ParticleInstance.UnkZero = reader.ReadUInt32();
                    string tempName = new string(reader.ReadChars(0x10));
                    ParticleInstance.Name = tempName.Replace('\0', ' ');
                    ParticleInstance.UnkShorts = new ushort[10];
                    for (int a = 0; a < ParticleInstance.UnkShorts.Length; a++)
                    {
                        ParticleInstance.UnkShorts[a] = reader.ReadUInt16();
                    }
                    ParticleInstance.UnkFloat = reader.ReadSingle();
                    ParticleInstance.EndZero = reader.ReadUInt16();

                    ParticleInstances.Add(ParticleInstance);
                }
            }
            else
            {
                ParticleInstanceCount = 0;
            }

            // Default.rm has some extra data (decal stuff)
            if (IsDefault)
            {
                DecalTextureID = InstanceCheck;
                DecalMaterialID = reader.ReadUInt32();
            }

            // todo: more data after this in default

            int RemainBytes = (int)((start_pos + size) - reader.BaseStream.Position);
            if (RemainBytes > 0)
            {
                Remain = reader.ReadBytes(RemainBytes);
            }
            else if (RemainBytes < 0)
            {
                throw new Exception("Invalid particle parsing");
            }
            else
            {
                Remain = new byte[0];
            }
        }

        public class ParticleSystemDefinition
        {
            public string Name;
            public byte[] Remain;

            public short Header1;
            public uint MaxParticleCount;
            public ushort Emitter_OverTime;
            public ushort Emitter_OverTimeRandom;
            public ushort Emitter_OffTime;
            public ushort Emitter_OffTimeRandom;
            public GenSort GSort;
            public TextureFiltering TextureFilter;
            public float UnkFloat;
            public float CutOnRadius;
            public float CutOffRadius;
            public float DrawCutOff;

            public float Velocity;
            public float Random_Emit_X;
            public float Random_Emit_Y;
            public float Random_Emit_Z;
            public float Random_Start_X;
            public float Random_Start_Y;
            public float Random_Start_Z;

            public float ParticleLifeTime;
            public float Gravity;

            public float Jibber_X_Freq;
            public float Jibber_X_Amp;
            public float Jibber_Y_Freq;
            public float Jibber_Y_Amp;



            public enum GenSort :ushort
            {
                Normal = 0,
                Radial = 0x06,
                RadialRotor = 0x07,
                Spheroid = 0x08,
                BounceY = 0x09,
                BounceXZ = 0x0A,
                ImprovedRadial = 0x0B,
            }

            public enum TextureFiltering : ushort
            {
                Additive = 0,
                Modulation = 0x02,
                Subtractive = 0x03,
                Glass = 0x07,
            }
        }

        public class ParticleSystemInstance
        {
            public string Name;
            public Pos Position;
            public ushort Rot_X;
            public ushort Rot_Y;
            public ushort Rot_Z;
            public uint UnkZero;
            public ushort[] UnkShorts; //10
            public float UnkFloat;
            public ushort EndZero;
        }

    }
}
