using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class TwinsPSF : TwinsSection
    {
        public List<TwinsPTC> FontPages = new List<TwinsPTC>();
        public List<TwinsVector4> Vectors = new List<TwinsVector4>();
        public int UnkInt;

        public override void Load(BinaryReader reader, int size)
        {
            var pages = reader.ReadInt32();
            for (var i = 0; i < pages; ++i)
            {
                var page = new TwinsPTC();
                page.IsXbox = IsXbox;
                page.IsDemo = IsDemo;
                page.Load(reader, 0);
                FontPages.Add(page);
            }
            var vecAmt = reader.ReadInt32();
            UnkInt = reader.ReadInt32();
            for (var i = 0; i < vecAmt; ++i)
            {
                var vec = new TwinsVector4();
                vec.Load(reader, 16);
                Vectors.Add(vec);
            }

            for (uint i = 0; i < FontPages.Count; i++)
            {
                RecordIDs.Add(i + 1, Records.Count);
                Records.Add(FontPages[(int)i]);
            }
        }
    }
}
