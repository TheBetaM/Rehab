using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RehabSetup;

namespace Twinsanity
{
    public class BD_Archive
    {

        public async Task ExtractAsync(byte[] bd, byte[] bh)
        {
            //Dictionary<uint, string> Files = new Dictionary<uint, string>();
            //Dictionary<uint, uint> Sizes = new Dictionary<uint, uint>();
            await Task.Run(
                () =>
                {
                    using (BinaryReader hr = new BinaryReader(new MemoryStream(bh)))
                    {
                        int magic = hr.ReadInt32();
                        while (hr.BaseStream.Position < hr.BaseStream.Length)
                        {
                            int namelen = hr.ReadInt32();
                            string name = Encoding.ASCII.GetString(hr.ReadBytes(namelen));
                            uint offset = hr.ReadUInt32();
                            uint size = hr.ReadUInt32();
                            //Files.Add(offset, name);
                            //Sizes.Add(offset, size);
                            AssetExporter.BufferFiles.Add(name, (offset + 1, size, null));
                        }
                    }
                }
                );
            /*
            IList<Task> editTaskList = new List<Task>();

            foreach (KeyValuePair<uint, string> file in Files)
            {
                editTaskList.Add(ExtractFileAsync(bd, file.Value, file.Key, Sizes));
            }

            await Task.WhenAll(editTaskList);
            editTaskList.Clear();
            */
        }

        private async Task ExtractFileAsync(byte[] bd, string Path, uint offset, Dictionary<uint, uint> Sizes)
        {
            using (MemoryStream BD = new(bd))
            {
                BD.Seek(offset, SeekOrigin.Begin);
                uint size = Sizes[offset];
                byte[] Data = new byte[size];
                await BD.ReadAsync(Data, 0, (int)size);
                AssetExporter.BufferFiles.Add(Path, (0, size, Data));
            }
        }
    }
}