using System;
using System.Collections.Generic;
using Twinsanity;
using Twinsanity.Items;

namespace RehabSetup
{
    public class GodotBinaryImageTexture : GodotBinaryResourceFile
    {

        public override string ResType => "ImageTexture";

        public GodotBinaryImageTexture()
        {

        }
        public GodotBinaryImageTexture(TextureX tex)
        {
            var res = new Resource("Image", $"local://Image_aaaaa");
            var dict = new Dictionary<object, object>();
            List<byte> RawData = new List<byte>();
            for (int i = 0; i < tex.RawData.Length; i++)
            {
                RawData.Add(tex.RawData[i].R);
                RawData.Add(tex.RawData[i].G);
                RawData.Add(tex.RawData[i].B);
                RawData.Add(tex.RawData[i].A);
            }
            dict.Add("width", tex.Width);
            dict.Add("height", tex.Height);
            dict.Add("format", "RGBA8");
            dict.Add("mipmaps", false);
            dict.Add("data", RawData.ToArray());
            res.Add("data", dict);
            Resources.Add(res);
            var res2 = new Resource(ResType, $"local://{ResType}_aaaab");
            res2.Add("image", res);
            Resources.Add(res2);
        }

        public GodotBinaryImageTexture(Texture tex)
        {
            var res = new Resource("Image", $"local://Image_aaaaa");
            var dict = new Dictionary<object, object>();
            List<byte> RawData = new List<byte>();
            for (int i = 0; i < tex.RawData.Length; i++)
            {
                RawData.Add(tex.RawData[i].R);
                RawData.Add(tex.RawData[i].G);
                RawData.Add(tex.RawData[i].B);
                RawData.Add(tex.RawData[i].A);
            }
            dict.Add("width", tex.Width);
            dict.Add("height", tex.Height);
            dict.Add("format", "RGBA8");
            dict.Add("mipmaps", false);
            dict.Add("data", RawData.ToArray());
            res.Add("data", dict);
            Resources.Add(res);
            var res2 = new Resource(ResType, $"local://{ResType}_aaaab");
            res2.Add("image", res);
            Resources.Add(res2);
        }

        public GodotBinaryImageTexture(List<Color> tex, int Width, int Height)
        {
            var res = new Resource("Image", $"local://Image_aaaaa");
            var dict = new Dictionary<object, object>();
            List<byte> RawData = new List<byte>();
            for (int i = 0; i < tex.Count; i++)
            {
                RawData.Add(tex[i].R);
                RawData.Add(tex[i].G);
                RawData.Add(tex[i].B);
                RawData.Add(tex[i].A);
            }
            dict.Add("width", Width);
            dict.Add("height", Height);
            dict.Add("format", "RGBA8");
            dict.Add("mipmaps", false);
            dict.Add("data", RawData.ToArray());
            res.Add("data", dict);
            Resources.Add(res);
            var res2 = new Resource(ResType, $"local://{ResType}_aaaab");
            res2.Add("image", res);
            Resources.Add(res2);
        }

        
        // combined texture (PSM)
        public GodotBinaryImageTexture(List<List<Color>> Textures, List<int> Widths, List<int> Heights)
        {
             List<byte> RawData = new List<byte>();
            int TexCount = Textures.Count;
            int ogWidth = Widths[0];
            int ogHeight = Heights[0];
            int maxWidth = ogWidth;
            int maxHeight = ogHeight;

            if (TexCount > 1)
            {
                maxWidth += ogWidth;
                if (TexCount > 2)
                {
                    maxWidth += ogWidth;
                }
                if (TexCount > 3)
                {
                    maxWidth += ogWidth;
                }
            }
            int rows = (TexCount / 4);
            if (rows == 0)
                rows = 1;

            maxHeight = maxHeight * rows;

            int ptc = 0;
            for (int y = 0; y < maxHeight; y++)
            {
                int maxptc = 4;
                if (TexCount > 4 && y >= maxHeight / 2)
                {
                    ptc = 4;
                    maxptc = TexCount;
                }
                else
                {
                    ptc = 0;
                }
                while (ptc < TexCount && ptc < maxptc)
                {
                    int c = y * ogWidth;
                    if (ptc >= 4)
                    {
                        c = (y - (maxHeight / 2)) * ogWidth;
                    }
                    for (int x = 0; x < ogWidth; x++)
                    {
                        if (c < Textures[ptc].Count)
                        {
                            RawData.Add(Textures[ptc][c].R);
                            RawData.Add(Textures[ptc][c].G);
                            RawData.Add(Textures[ptc][c].B);
                            RawData.Add(Textures[ptc][c].A);
                        }
                        else
                        {
                            RawData.Add(0);
                            RawData.Add(0);
                            RawData.Add(0);
                            RawData.Add(255);
                        }
                        c++;
                    }
                    ptc++;
                }
            }

            var res = new Resource("Image", $"local://Image_aaaaa");
            var dict = new Dictionary<object, object>();
            dict.Add("width", maxWidth);
            dict.Add("height", maxHeight);
            dict.Add("format", "RGBA8");
            dict.Add("mipmaps", false);
            dict.Add("data", RawData.ToArray());
            res.Add("data", dict);
            Resources.Add(res);
            var res2 = new Resource(ResType, $"local://{ResType}_aaaab");
            res2.Add("image", res);
            Resources.Add(res2);
        }

    }
}