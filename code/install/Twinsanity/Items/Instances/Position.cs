using System.IO;

namespace Twinsanity
{
    public class Position : TwinsItem
    {
        public Pos Pos { get; set; }

        public override void Load(BinaryReader reader, int size)
        {
            Pos = new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
