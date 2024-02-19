using System.IO;

namespace Twinsanity
{
    public class Skydome : TwinsItem
    {
        public uint Unknown { get; set; }
        public uint[] ModelIDs { get; set; }

        public override void Load(BinaryReader reader, int size)
        {
            Unknown = reader.ReadUInt32();
            var count = reader.ReadInt32();
            ModelIDs = new uint[count];
            for (int i = 0; i < count; ++i)
                ModelIDs[i] = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return $"SkyDome_{DefaultHashes.ToName(ParentType, ID)}";
        }
    }
}
