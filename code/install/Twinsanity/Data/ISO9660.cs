using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DiscUtils.Iso9660;

namespace RehabSetup
{
    public class ISO9660
    {
        public bool CNF_Only = false;
        public bool ExtractFiles = false;
        public string ExtractPath;
        public string[] CNF_Buffer;
        public GameVersion Version = GameVersion.USA_ver2;

        public bool DetectPS2(string filePath)
        {
            bool isISO = false;
            string dirPath = string.Empty;
            FileInfo? xbe = new FileInfo(filePath);
            if (xbe == null) return false;

            CNF_Only = true;
            ExtractFiles = false;

            if (xbe.Extension.ToLower() == ".iso")
            {
                isISO = true;
                ExtractPath = AppDomain.CurrentDomain.BaseDirectory;
                using (FileStream file = new FileStream(filePath, FileMode.Open))
                {
                    CDReader cd;

                    if (!CDReader.Detect(file))
                    {
                        return false;
                    }
                    else
                    {
                        cd = new CDReader(file, true);
                    }

                    if (cd.FileExists(@"SYSTEM.CNF"))
                    {
                        using (StreamReader sr = new StreamReader(cd.OpenFile(@"SYSTEM.CNF", FileMode.Open)))
                        {
                            CNF_Buffer = new string[3];
                            CNF_Buffer[0] = sr.ReadLine();
                            CNF_Buffer[1] = sr.ReadLine();
                            CNF_Buffer[2] = sr.ReadLine();
                        }
                    }
                    cd.Dispose();
                    cd = null;
                }
            }
            else
            {
                CNF_Buffer = System.IO.File.ReadAllLines(filePath);
            }

            Version = GameVersion.Unknown;
            foreach (var pair in TitleIDs)
            {
                if (CNF_Buffer[0].Contains(pair.Key))
                {
                    Version = pair.Value;
                    break;
                }
            }
            if (Version == GameVersion.Unknown) return false;
            if (Version == GameVersion.USA_ver2)
            {
                if (CNF_Buffer[1].Contains("1.00"))
                {
                    Version = GameVersion.USA_ver1;
                }
            }
            else if (Version == GameVersion.DEMO_USA)
            {
                if (CNF_Buffer[2].Contains("PAL"))
                {
                    Version = GameVersion.DEMO_EUR;
                }
            }
            return true;
        }

        public async Task ExportISO(string inputPath, string outputPath)
        {
            CNF_Only = false;
            ExtractFiles = true;
            ExtractPath = outputPath;
            //Directory.CreateDirectory(ExtractPath);

            IList<Task> extractTaskList = new List<Task>();
            Dictionary<string, string> Paths = new Dictionary<string, string>();
            
            using (FileStream extract_isoStream = System.IO.File.Open(inputPath, FileMode.Open))
            {
                using (CDReader extract_reader = new CDReader(extract_isoStream, true))
                {
                    Recursive_MakePaths(extract_reader, "", ref Paths);
                }
            }

            foreach (KeyValuePair<string, string> Path in Paths)
            {
                extractTaskList.Add(ISO_ExtractAsync(inputPath, Path.Key, Path.Value));
            }

            await Task.WhenAll(extractTaskList);

            extractTaskList.Clear();
        }


        private void Recursive_MakePaths(CDReader cd, string dir, ref Dictionary<string, string> Paths)
        {
            if (cd.GetDirectoryInfo(dir).GetFiles().Length > 0)
            {
                foreach (string file in cd.GetFiles(dir))
                {
                    string filename = ExtractPath.TrimEnd('\\') + file;
                    filename = filename.Replace(";1", string.Empty);
                    Paths.Add(file, filename);
                }
            }
            if (cd.GetDirectories(dir).Length > 0)
            {
                foreach (string directory in cd.GetDirectories(dir))
                {
                    Recursive_MakePaths(cd, directory, ref Paths);
                }
            }
        }

        private async Task ISO_ExtractAsync(string input, string file, string path)
        {
            // CDReader doesn't work in async, so this is the workaround
            using (FileStream iso = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 0x10000, System.IO.FileOptions.SequentialScan))
            {
                using (CDReader cd = new CDReader(iso, true))
                {
                    using (Stream fileStreamFrom = cd.OpenFile(file, FileMode.Open))
                    {
                        uint size = (uint)fileStreamFrom.Length;
                        byte[] data = new byte[size];
                        await fileStreamFrom.ReadAsync(data, 0, (int)size);
                        AssetExporter.BufferFiles.Add(path, (0, size, data));
                        /*
                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                        using (Stream fileStreamTo = System.IO.File.Open(path, FileMode.OpenOrCreate))
                        {
                            await fileStreamFrom.CopyToAsync(fileStreamTo);
                            //fileStreamFrom.CopyTo(fileStreamTo);
                        }
                        */
                    }
                }
            }
        }

        public enum GameVersion
        {
            Unknown = -1,
            USA_ver1 = 0,
            USA_ver2 = 1,
            EUR = 2,
            JPN = 3,
            DEMO_USA = 4,
            DEMO_EUR = 5,
        }

        Dictionary<string, GameVersion> TitleIDs = new Dictionary<string, GameVersion>(){
            ["SLUS_209.09"] = GameVersion.USA_ver2,
            ["SLES_515.68"] = GameVersion.EUR,
            ["SLPM_658.01"] = GameVersion.JPN,
            ["CRASH6\\CRASH6.ELF"] = GameVersion.DEMO_USA,
            ["SLUS_291.01"] = GameVersion.DEMO_USA,
        };
    }
}