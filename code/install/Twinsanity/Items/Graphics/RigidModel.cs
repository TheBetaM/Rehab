using System.IO;

namespace Twinsanity
{
    public class RigidModel : TwinsItem
    {
        public uint Header { get; set; }
        public uint[] MaterialIDs { get; set; }
        public uint MeshID { get; set; }

        public override void Load(BinaryReader reader, int size)
        {
            Header = reader.ReadUInt32();
            var count = reader.ReadInt32();
            MaterialIDs = new uint[count];
            for (int i = 0; i < count; ++i)
                MaterialIDs[i] = reader.ReadUInt32();
            MeshID = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return $"{DefaultHashes.ToName(ParentType, ID)}";
        }
    }
}
