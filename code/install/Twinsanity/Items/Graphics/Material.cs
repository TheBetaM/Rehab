using System.IO;
using System;
using System.Collections.Generic;

namespace Twinsanity
{
    public class Material : TwinsItem
    {
        public string Name { get; set; }
        public ulong Header { get; set; }
        public int Unknown { get; set; }
        public List<TwinsShader> Shaders = new List<TwinsShader>();
        public bool ForceDemo { get; set; }

        public override void Load(BinaryReader reader, int size)
        {
            Header = reader.ReadUInt64();
            Unknown = reader.ReadInt32();
            var nameLen = reader.ReadInt32();
            Name = new string(reader.ReadChars(nameLen));
            var shdCnt = reader.ReadInt32();
            Shaders.Clear();
            for (var i = 0; i < shdCnt; ++i)
            {
                TwinsShader shd = new TwinsShader();
                shd.Read(reader, 0, ParentType == SectionType.MaterialDemo || ForceDemo);
                Shaders.Add(shd);
            }
        }

        public override string ToString()
        {
            return $"{ID:X8}-{Name}";
        }
    }
}
