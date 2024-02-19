using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity.Items
{
    public class TwinsPTC : TwinsSection
    {
        public uint TexID;
        public uint MatID;
        public Texture Texture;
        public TextureX TextureX;
        public Material Material;

        public override void Load(BinaryReader reader, int size)
        {
            TexID = reader.ReadUInt32();
            MatID = reader.ReadUInt32();
            if (IsXbox)
            {
                TextureX = new TextureX();
                TextureX.Load(reader, 0);
                TextureX.ID = TexID;
            }
            else
            {
                Texture = new Texture();
                Texture.Load(reader, 0);
                Texture.ID = TexID;
            }
            Material = new Material();
            Material.ForceDemo = IsDemo;
            Material.Load(reader, 0);
            Material.ID = MatID;

            Level = 2;
            Type = SectionType.PTC;
            RecordIDs.Add(TexID, Records.Count);
            if (IsXbox)
            {
                Records.Add(TextureX);
            }
            else
            {
                Records.Add(Texture);
            }
            RecordIDs.Add(MatID, Records.Count);
            Records.Add(Material);
        }
    }
}
