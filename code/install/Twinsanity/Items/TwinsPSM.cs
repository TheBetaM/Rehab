using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class TwinsPSM : TwinsSection
    {
        public List<TwinsPTC> PTCs = new List<TwinsPTC>();
        

        public override void Load(BinaryReader reader, int size)
        {
            var startPos = reader.BaseStream.Position;
            while (reader.BaseStream.Position < startPos + size)
            {
                var ptc = new TwinsPTC();
                ptc.IsXbox = IsXbox;
                ptc.IsDemo = IsDemo;
                ptc.Load(reader, 0);
                PTCs.Add(ptc);
            }

            for (uint i = 0; i < PTCs.Count; i++)
            {
                RecordIDs.Add(i + 1, Records.Count);
                Records.Add(PTCs[(int)i]);
            }
        }

        public override void Save(BinaryWriter writer)
        {
            foreach (var ptc in PTCs)
            {
                ptc.Save(writer);
            }
        }
    }
}
