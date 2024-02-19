using System.IO;
using System.Collections.Generic;

namespace Twinsanity
{
    public class Path : TwinsItem
    {
        public List<Pos> Positions { get; set; } = new List<Pos>();
        public List<PathParam> Params { get; set; } = new List<PathParam>();

        public override void Load(BinaryReader reader, int size)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                Positions.Add(new Pos(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                Params.Add(new PathParam { P1 = reader.ReadSingle(), P2 = reader.ReadSingle() });
            }
        }

        #region STRUCTURES
        public class PathParam
        {
            public float P1;
            public float P2;
        }
        #endregion
    }
}
