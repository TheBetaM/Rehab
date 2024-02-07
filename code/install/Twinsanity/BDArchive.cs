using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Twinsanity
{
    public class BD_Archive
    {
        public async Task ExtractAsync(string in_name_bd, string in_name_bh, string out_dir)
        {
            string bdname = in_name_bd;
            string bhname = in_name_bh;
            if (!File.Exists(bhname))
                throw new ArgumentException("BH file could not be found.");
            if (!File.Exists(bdname))
                throw new ArgumentException("BD file could not be found.");

            Dictionary<uint, string> Files = new Dictionary<uint, string>();
            Dictionary<uint, uint> Sizes = new Dictionary<uint, uint>();

            using (BinaryReader hr = new BinaryReader(new FileStream(bhname, FileMode.Open)))
            {
                int magic = hr.ReadInt32();
                while (hr.BaseStream.Position < hr.BaseStream.Length)
                {
                    int namelen = hr.ReadInt32();
                    string name = Encoding.ASCII.GetString(hr.ReadBytes(namelen));
                    uint offset = hr.ReadUInt32();
                    uint size = hr.ReadUInt32();
                    Files.Add(offset, System.IO.Path.Combine(out_dir, name));
                    Sizes.Add(offset, size);
                }
            }

            IList<Task> editTaskList = new List<Task>();

            foreach (KeyValuePair<uint, string> file in Files)
            {
                editTaskList.Add(ExtractFileAsync(bdname, file.Value, file.Key, Sizes));
            }

            await Task.WhenAll(editTaskList);
            editTaskList.Clear();

            File.Delete(bdname);
            File.Delete(bhname);
        }

        private async Task ExtractFileAsync(string bdname, string Path, uint offset, Dictionary<uint, uint> Sizes)
        {
            FileStream BD = new FileStream(bdname, FileMode.Open, FileAccess.Read, FileShare.Read);
            BD.Seek(offset, SeekOrigin.Begin);

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            Stream file = File.Open(Path, FileMode.Create, FileAccess.Write);
            uint size = Sizes[offset];
            byte[] Data = new byte[size];
            try
            {
                await BD.ReadAsync(Data, 0, (int)size);
            }
            catch
            {
                Console.WriteLine("Read Error: " + Path);
            }
            try
            {
                await file.WriteAsync(Data, 0, (int)size);
            }
            catch
            {
                Console.WriteLine("Write Error: " + Path);
            }

            file.Close();
            BD.Close();
        }
    }
}