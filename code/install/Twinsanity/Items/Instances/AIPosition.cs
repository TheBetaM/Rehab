using System.IO;

namespace Twinsanity
{
    public class AIPosition : TwinsItem
    {
        public Pos Pos { get; set; } // W is node weight?
        public NodeType Node { get; set; }
        private ushort Num { get; set; }

        public override void Load(BinaryReader reader, int size)
        {
            Pos = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Num = reader.ReadUInt16();
            Node = (NodeType)Num;
        }

        public enum NodeType : ushort
        {
            Ground = 0, // Default AI node
            Air = 2, // For jetpack ant / bat
            WormPath = 4, // Earth worm / farm chickens
            CortexPoint = 16, // Cortex interest point, also used for bird paths in L03
        }
    }
}
